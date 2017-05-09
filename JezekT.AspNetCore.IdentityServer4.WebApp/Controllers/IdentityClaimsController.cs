using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.IdentityClaimViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class IdentityClaimsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;


        public IActionResult Create(int identityResourceId)
        {
            var vm = new IdentityClaimViewModel { IdentityResourceId = identityResourceId };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var identityResource = await _dbContext.IdentityResources.FindAsync(vm.IdentityResourceId);
                if (identityResource == null)
                {
                    return BadRequest();
                }

                var identityClaim = new IdentityClaim
                {
                    Type = vm.Type,
                    IdentityResource = identityResource
                };
                _dbContext.Set<IdentityClaim>().Add(identityClaim);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Edit", "IdentityResources", new { id = vm.IdentityResourceId });
            }
            return View(vm);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var vm = await GetViewModel(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var obj = await _dbContext.Set<IdentityClaim>().FindAsync(vm.Id);
                if (obj == null)
                {
                    return NotFound();
                }

                obj.Type = vm.Type;
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Edit", "IdentityResources", new { id = vm.IdentityResourceId });
            }
            return View(vm);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var vm = await GetViewModel(id);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(IdentityClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var obj = await _dbContext.Set<IdentityClaim>().FindAsync(vm.Id);
            if (obj == null)
            {
                return NotFound();
            }

            _dbContext.Remove(obj);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Edit", "IdentityResources", new { id = vm.IdentityResourceId });
        }


        public IdentityClaimsController(ConfigurationDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
        }


        private async Task<IdentityClaimViewModel> GetViewModel(int id)
        {
            var identityClaim = await _dbContext.Set<IdentityClaim>().Include(x => x.IdentityResource).FirstOrDefaultAsync(x => x.Id == id);
            if (identityClaim != null)
            {
                return new IdentityClaimViewModel
                {
                    Id = identityClaim.Id,
                    IdentityResourceId = identityClaim.IdentityResource.Id,
                    Type = identityClaim.Type
                };
            }
            return null;
        }

    }
}
