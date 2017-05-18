using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.AspNetCore.IdentityServer4.WebApp.Extensions;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.AccountServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly AccountService _account;
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IPasswordResetSender _resetPwdSender;
        private readonly ILogger _logger;


        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl)
        {
            var vm = await _account.BuildLoginViewModelAsync(returnUrl);
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(vm.Username, vm.Password, vm.RememberLogin, true);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {vm.Username} logged in.");
                    return RedirectToLocal(vm.ReturnUrl);
                }
                if (result.RequiresTwoFactor)
                {
                    _logger.LogInformation("Two factor authentication not supported.");
                    return BadRequest();
                }
                if (result.IsLockedOut)
                {
                    _logger.LogInformation($"User {vm.Username} locked out.");
                    return View("Lockout");
                }

                ModelState.AddModelError(string.Empty, Resources.Controllers.SharedResources.InvalidLoginAttempt);
            }

            return View(vm);
        }


        [AllowAnonymous]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = await _account.BuildLogoutViewModelAsync(logoutId);
            if (vm.ShowLogoutPrompt == false)
            {
                return await Logout(vm);
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            var vm = await _account.BuildLoggedOutViewModelAsync(model.LogoutId);
            await _signInManager.SignOutAsync();

            if (vm.ClientName == null)
            {
                _logger.LogInformation($"User {User?.Identity.Name} logged out.");
            }
            else
            {
                _logger.LogInformation($"Client {vm.ClientName} logged out.");
            }
            
            return View("LoggedOut", vm);
        }


        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [AllowAnonymous]
        public async Task<IActionResult> EmailConfirmed(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogInformation($"User Id {userId} not found.");
                return NotFound();
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} confirmed his email {user.Email}.");
                return View();
            }
            ModelState.AddErrors(result.Errors.ToList());
            return View("Error");
        }

        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                {
                    return View("ForgotPasswordConfirmation");
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code, email = model.Email }, HttpContext.Request.Scheme);
                await _resetPwdSender.SendPasswordResetAsync(user.Email, callbackUrl);
                return View("ForgotPasswordConfirmation");
            }

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null, string userId = null)
        {
            return code == null || userId == null ? View("Error") : View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return NotFound();
                }
                var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {user.UserName} reset his password.");
                    return RedirectToAction(nameof(ResetPasswordConfirmation), "Account");
                }
                ModelState.AddErrors(result.Errors.ToList());
            }
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, IIdentityServerInteractionService interaction, 
            IHttpContextAccessor httpContextAccessor, IClientStore clientStore, IPasswordResetSender resetPwdSender, ILogger<AccountController> logger)
        {
            if (signInManager == null || userManager == null || interaction == null || httpContextAccessor == null || 
                resetPwdSender == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _signInManager = signInManager;
            _userManager = userManager;
            _resetPwdSender = resetPwdSender;
            _account = new AccountService(interaction, httpContextAccessor);
            _logger = logger;
        }


        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

    }
}