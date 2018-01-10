using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.UserClaimViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "UserAdministratorOnly")]
    public class UserClaimsController : Controller
    {
        private readonly IdentityServerDbContext _dbContext;
        private readonly ILogger _logger;

        public async Task<IActionResult> Details(int id)
        {
            var vm = await GetUserClaimViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }


        public IActionResult Create(string userId)
        {
            return View(new UserClaimViewModel { UserId = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var user = await _dbContext.Users.FindAsync(vm.UserId);
                if (user == null)
                {
                    return BadRequest();
                }
                var obj = new IdentityUserClaim<string>
                {
                    UserId = vm.UserId,
                    ClaimType = vm.ClaimType,
                    ClaimValue = vm.ClaimValue
                };
                _dbContext.UserClaims.Add(obj);

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"User claim Id {obj.Id} created by {User?.Identity?.Name}.");
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                    throw;
                }
            }

            return RedirectToAction("Edit", "Users", new {id = vm.UserId});
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await GetUserClaimViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var claim = await _dbContext.UserClaims.FindAsync(vm.Id);
                if (claim == null)
                {
                    return BadRequest();
                }

                claim.ClaimType = vm.ClaimType;
                claim.ClaimValue = vm.ClaimValue;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"User claim Id {claim.Id} updated by {User?.Identity?.Name}.");
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                    throw;
                }
            }

            return RedirectToAction("Edit", "Users", new { id = vm.UserId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vm = await GetUserClaimViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(UserClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var claim = await _dbContext.UserClaims.FindAsync(vm.Id);
            if (claim == null)
            {
                return BadRequest();
            }

            _dbContext.UserClaims.Remove(claim);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"User claim Id {vm.Id} removed by {User?.Identity?.Name}.");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }

            return RedirectToAction("Edit", "Users", new { id = vm.UserId });
        }


        public UserClaimsController(IdentityServerDbContext dbContext, ILogger<UserClaimsController> logger)
        {
            if (dbContext == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _logger = logger;
        }


        private async Task<UserClaimViewModel> GetUserClaimViewModelAsync(int id)
        {
            var claim = await _dbContext.UserClaims.FindAsync(id);
            if (claim != null)
            {
                return new UserClaimViewModel
                {
                    Id = claim.Id,
                    ClaimType = claim.ClaimType,
                    ClaimValue = claim.ClaimValue,
                    UserId = claim.UserId
                };
            }
            return null;
        }

    }
}
