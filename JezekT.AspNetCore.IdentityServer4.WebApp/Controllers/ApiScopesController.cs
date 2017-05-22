using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiScopeClaimViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiScopeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class ApiScopesController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly ILogger _logger;


        public async Task<IActionResult> Details(int id)
        {
            var vm = await GetApiScopeViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }


        public IActionResult Create(int apiResourceId)
        {
            return View(new ApiScopeViewModel{ApiResourceId = apiResourceId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiScopeViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var apiResource = await _dbContext.Set<ApiResource>().FindAsync(vm.ApiResourceId);
                if (apiResource == null)
                {
                    return BadRequest();
                }
                var obj = GetApiScope(vm, apiResource);
                _dbContext.Set<ApiScope>().Add(obj);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API scope Id {obj.Id} created by {User?.Identity?.Name}.");
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
            var vm = await GetApiScopeViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApiScopeViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var apiScope = await _dbContext.Set<ApiScope>().FindAsync(vm.Id);
                apiScope.Name = vm.Name;
                apiScope.DisplayName = vm.DisplayName;
                apiScope.Description = vm.Description;
                apiScope.Required = vm.Required;
                apiScope.ShowInDiscoveryDocument = vm.ShowInDiscoveryDocument;
                apiScope.Emphasize = vm.Emphasize;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API scope Id {apiScope.Id} updated by {User?.Identity?.Name}.");
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
            var vm = await GetApiScopeViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ApiScopeViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var apiScope = await _dbContext.Set<ApiScope>().FindAsync(vm.Id);
            if (apiScope == null)
            {
                return NotFound();
            }

            _dbContext.Set<ApiScope>().Remove(apiScope);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"API scope Id {vm.Id} removed by {User?.Identity?.Name}.");
                return RedirectToAction("Edit", "ApiResources", new { id = vm.ApiResourceId });
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }
        }


        public ApiScopesController(ConfigurationDbContext dbContext, ILogger<ApiScopesController> logger)
        {
            if (dbContext == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _logger = logger;
        }


        private async Task<ApiScopeViewModel> GetApiScopeViewModelAsync(int id)
        {
            var apiScope = await _dbContext.Set<ApiScope>().Include(x => x.ApiResource).Include(x => x.UserClaims).FirstOrDefaultAsync(x => x.Id == id);
            if (apiScope != null)
            {
                return new ApiScopeViewModel
                {
                    Id = apiScope.Id,
                    ApiResourceId = apiScope.ApiResource.Id,
                    Name = apiScope.Name,
                    DisplayName = apiScope.DisplayName,
                    Description = apiScope.Description,
                    Required = apiScope.Required,
                    Emphasize = apiScope.Emphasize,
                    ShowInDiscoveryDocument = apiScope.ShowInDiscoveryDocument,
                    Claims = apiScope.UserClaims.Select(x => new ApiScopeClaimViewModel { Id = x.Id, ApiScopeId = apiScope.Id, Type = x.Type }).ToList()
                };
            }
            return null;
        }

        private ApiScope GetApiScope(ApiScopeViewModel vm, ApiResource apiResource)
        {
            Contract.Requires(apiResource != null);

            if (vm != null)
            {
                return new ApiScope
                {
                    Name = vm.Name,
                    DisplayName = vm.DisplayName,
                    Description = vm.Description,
                    Required = vm.Required,
                    ShowInDiscoveryDocument = vm.ShowInDiscoveryDocument,
                    Emphasize = vm.Emphasize,
                    ApiResource = apiResource
                };
            }
            return null;
        }

    }
}
