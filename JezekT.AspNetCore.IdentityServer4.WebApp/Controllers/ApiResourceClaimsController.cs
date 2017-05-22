using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiResourceClaimViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class ApiResourceClaimsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly ILogger _logger;


        public IActionResult Create(int apiResourceId)
        {
            var vm = new ApiResourceClaimViewModel { ApiResourceId = apiResourceId };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiResourceClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var apiResource = await _dbContext.ApiResources.FindAsync(vm.ApiResourceId);
                if (apiResource == null)
                {
                    return BadRequest();
                }

                var apiResourceClaim = new ApiResourceClaim
                {
                    Type = vm.Type,
                    ApiResource = apiResource
                };

                _dbContext.Set<ApiResourceClaim>().Add(apiResourceClaim);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API resource claim Id {apiResourceClaim.Id} created by {User?.Identity?.Name}.");
                    return RedirectToAction("Edit", "ApiResources", new { id = vm.ApiResourceId });
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                    throw;
                }
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
        public async Task<IActionResult> Edit(ApiResourceClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var obj = await _dbContext.Set<ApiResourceClaim>().FindAsync(vm.Id);
                if (obj == null)
                {
                    return NotFound();
                }

                obj.Type = vm.Type;
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API resource claim Id {obj.Id} updated by {User?.Identity?.Name}.");
                    return RedirectToAction("Edit", "ApiResources", new { id = vm.ApiResourceId });
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                    throw;
                }
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
        public async Task<IActionResult> Delete(ApiResourceClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var obj = await _dbContext.Set<ApiResourceClaim>().FindAsync(vm.Id);
            if (obj == null)
            {
                return NotFound();
            }

            _dbContext.Remove(obj);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"API resource claim Id {vm.Id} removed by {User?.Identity?.Name}.");
                return RedirectToAction("Edit", "ApiResources", new { id = vm.ApiResourceId });
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }
        }


        public ApiResourceClaimsController(ConfigurationDbContext dbContext, ILogger<ApiResourceClaimsController> logger)
        {
            if (dbContext == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _logger = logger;
        }


        private async Task<ApiResourceClaimViewModel> GetViewModel(int id)
        {
            var identityClaim = await _dbContext.Set<ApiResourceClaim>().Include(x => x.ApiResource).FirstOrDefaultAsync(x => x.Id == id);
            if (identityClaim != null)
            {
                return new ApiResourceClaimViewModel
                {
                    Id = identityClaim.Id,
                    ApiResourceId = identityClaim.ApiResource.Id,
                    Type = identityClaim.Type
                };
            }
            return null;
        }

    }
}
