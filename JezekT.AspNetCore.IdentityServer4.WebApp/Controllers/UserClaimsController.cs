using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.UserClaimViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "UserAdministratorOnly")]
    public class UserClaimsController : Controller
    {
        private readonly IdentityServerDbContext _dbContext;


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
                _dbContext.UserClaims.Add(new IdentityUserClaim<string>
                {
                    UserId = vm.UserId,
                    ClaimType = vm.ClaimType,
                    ClaimValue = vm.ClaimValue
                });
                await _dbContext.SaveChangesAsync();
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
                _dbContext.Entry(claim).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();
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
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Edit", "Users", new { id = vm.UserId });
        }


        public UserClaimsController(IdentityServerDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
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
