using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.AspNetCore.IdentityServer4.WebApp.Extensions;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountSettingsViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize]
    public class AccountSettingsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;
        private readonly IEmailConfirmationSender _confirmationSender;


        public async Task<IActionResult> Index(string message = null)
        {
            ViewData["StatusMessage"] = message;

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return View("Error");
            }
            var vm = new IndexViewModel
            {
                HasPassword = await _userManager.HasPasswordAsync(user),
                HasEmail = !string.IsNullOrEmpty(user.Email),
                EmailConfirmed = user.EmailConfirmed
            };
            return View(vm);
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    _logger.LogInformation("Current user not found.");
                    return NotFound();
                }
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    _logger.LogInformation($"User {user.UserName} changed his password.");
                    return RedirectToAction(nameof(Index), new { Message = Resources.Controllers.AccountSettings.AccountSettingsController.PasswordChanged });
                }
                ModelState.AddErrors(result.Errors.ToList());
            }
            return View(model);
        }


        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                if (user == null)
                {
                    _logger.LogInformation("Current user not found.");
                    return NotFound();
                }
                var result = await _userManager.SetEmailAsync(user, model.NewEmail);
                if (result.Succeeded)
                {
                    await EmailConfirmation(user);
                    _logger.LogInformation($"User {user.UserName} changed his email.");
                    return RedirectToAction(nameof(Index), new { Message = Resources.Controllers.AccountSettings.AccountSettingsController.EmailChanged });
                }
                ModelState.AddErrors(result.Errors.ToList());
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            return View(new ConfirmEmailViewModel { Email = user.Email });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                _logger.LogInformation("Current user not found.");
                return NotFound();
            }

            if (!user.EmailConfirmed)
            {
                await EmailConfirmation(user);
                return RedirectToAction(nameof(Index), new { Message = string.Format(Resources.Controllers.AccountSettings.AccountSettingsController.ConfirmationSentToX, user.Email) });
            }
            return RedirectToAction(nameof(Index), new { Message = string.Format(Resources.Controllers.AccountSettings.AccountSettingsController.EmailAlreadyConfirmed, user.Email) });
        }


        public AccountSettingsController(UserManager<User> userManager, SignInManager<User> signInManager, ILoggerFactory loggerFactory, IEmailConfirmationSender confirmationSender)
        {
            if (userManager == null || signInManager == null || loggerFactory == null || confirmationSender == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _userManager = userManager;
            _signInManager = signInManager;
            _logger = loggerFactory.CreateLogger<AccountSettingsController>();
            _confirmationSender = confirmationSender;
        }


        private async Task EmailConfirmation(User user)
        {
            Contract.Requires(user != null);

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("EmailConfirmed", "Account", new { userId = user.Id, code }, HttpContext.Request.Scheme);
            await _confirmationSender.SendEmailConfirmationAsync(user.Email, callbackUrl);
            _logger.LogInformation($"Email confirmation of user {user.UserName} has been sent to {user.Email}.");
        }

    }
}
