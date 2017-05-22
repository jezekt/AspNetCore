using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiScopeClaimViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class ApiScopeClaimsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly ILogger _logger;


        public IActionResult Create(int apiScopeId)
        {
            return View(new ApiScopeClaimViewModel { ApiScopeId = apiScopeId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiScopeClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var apiScope = await _dbContext.Set<ApiScope>().FindAsync(vm.ApiScopeId);
                if (apiScope == null)
                {
                    return BadRequest();
                }
                var obj = new ApiScopeClaim
                {
                    Type = vm.Type,
                    ApiScope = apiScope
                };
                _dbContext.Set<ApiScopeClaim>().Add(obj);

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API scope claim Id {obj.Id} created by {User?.Identity?.Name}.");
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                    throw;
                }
            }

            return RedirectToAction("Edit", "ApiScopes", new { id = vm.ApiScopeId });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await GetClaimViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApiScopeClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var claim = await _dbContext.Set<ApiScopeClaim>().FindAsync(vm.Id);
                if (claim == null)
                {
                    return BadRequest();
                }

                claim.Type = vm.Type;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API scope claim Id {claim.Id} updated by {User?.Identity?.Name}.");
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                    throw;
                }
            }

            return RedirectToAction("Edit", "ApiScopes", new { id = vm.ApiScopeId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vm = await GetClaimViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ApiScopeClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var claim = await _dbContext.Set<ApiScopeClaim>().FindAsync(vm.Id);
            if (claim == null)
            {
                return BadRequest();
            }

            _dbContext.Set<ApiScopeClaim>().Remove(claim);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"API scope claim Id {vm.Id} removed by {User?.Identity?.Name}.");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }

            return RedirectToAction("Edit", "ApiScopes", new { id = vm.ApiScopeId });
        }


        public ApiScopeClaimsController(ConfigurationDbContext dbContext, ILogger<ApiScopeClaimsController> logger)
        {
            if (dbContext == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _logger = logger;
        }


        private async Task<ApiScopeClaimViewModel> GetClaimViewModelAsync(int id)
        {
            var claim = await _dbContext.Set<ApiScopeClaim>().Include(x => x.ApiScope).FirstOrDefaultAsync(x => x.Id == id);
            if (claim != null)
            {
                return new ApiScopeClaimViewModel
                {
                    Id = claim.Id,
                    ApiScopeId = claim.ApiScope.Id,
                    Type = claim.Type,
                };
            }
            return null;
        }

    }
}
