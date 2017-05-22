using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiResourceClaimViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiResourceViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiScopeViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiSecretViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class ApiResourcesController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IPaginationDataProvider<ApiResource, object> _apiResourcePaginationProvider;
        private readonly ILogger _logger;


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var vm = await GetApiResourceViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }


        public IActionResult Create()
        {
            return View(new ApiResourceViewModel { Enabled = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiResourceViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var obj = GetApiResource(vm);
                _dbContext.ApiResources.Add(obj);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API resource Id {obj.Id} created by {User?.Identity?.Name}.");
                    return RedirectToAction("Index");
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
            var vm = await GetApiResourceViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ApiResourceViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var apiResource = await _dbContext.IdentityResources.FindAsync(vm.Id);
                apiResource.Enabled = vm.Enabled;
                apiResource.Name = vm.Name;
                apiResource.DisplayName = vm.DisplayName;
                apiResource.Description = vm.Description;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API resource Id {apiResource.Id} updated by {User?.Identity?.Name}.");
                    return RedirectToAction("Index");
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
            var vm = await GetApiResourceViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ApiResourceViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var apiResource = await _dbContext.ApiResources.FindAsync(vm.Id);
            if (apiResource == null)
            {
                return NotFound();
            }

            _dbContext.ApiResources.Remove(apiResource);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"API resource Id {vm.Id} removed by {User?.Identity?.Name}.");
                return RedirectToAction("Index");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }
        }


        public async Task<IActionResult> GetTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection, string queryIds)
        {
            var filterIds = queryIds.ToIdsArray<int>();
            var filterIdsExpression = filterIds != null ? (Expression<Func<ApiResource, bool>>)(x => filterIds.Contains(x.Id)) : null;
            var paginationData = await _apiResourcePaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, filterIdsExpression);
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }


        public ApiResourcesController(ConfigurationDbContext dbContext, IPaginationDataProvider<ApiResource, object> apiResourcePaginationDataProvider,
            ILogger<ApiResourcesController> logger)
        {
            if (dbContext == null || apiResourcePaginationDataProvider == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _apiResourcePaginationProvider = apiResourcePaginationDataProvider;
            _logger = logger;
        }


        private async Task<ApiResourceViewModel> GetApiResourceViewModelAsync(int id)
        {
            var apiResource = await _dbContext.ApiResources
                .Include(x => x.UserClaims)
                .Include(x => x.Scopes)
                .Include(x => x.Secrets)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (apiResource != null)
            {
                return new ApiResourceViewModel
                {
                    Id = apiResource.Id,
                    Enabled = apiResource.Enabled,
                    Name = apiResource.Name,
                    DisplayName = apiResource.DisplayName,
                    Description = apiResource.Description,
                    Claims = apiResource.UserClaims.Select(x => new ApiResourceClaimViewModel { Id = x.Id, ApiResourceId = apiResource.Id, Type = x.Type }).ToList(),
                    Secrets = apiResource.Secrets.Select(x => new ApiSecretViewModel {Id = x.Id, ApiResourceId = apiResource.Id, Type = x.Type, Description = x.Description, Expiration = x.Expiration}).ToList(),
                    Scopes = apiResource.Scopes.Select(x => new ApiScopeViewModel {Id = x.Id, ApiResourceId = apiResource.Id, Description = x.Description, Name = x.Name,
                        DisplayName = x.DisplayName, Required = x.Required, Emphasize = x.Emphasize, ShowInDiscoveryDocument = x.ShowInDiscoveryDocument}).ToList()
                };
            }
            return null;
        }

        private ApiResource GetApiResource(ApiResourceViewModel vm)
        {
            if (vm != null)
            {
                return new ApiResource
                {
                    Enabled = vm.Enabled,
                    Name = vm.Name,
                    DisplayName = vm.DisplayName,
                    Description = vm.Description,
                };
            }
            return null;
        }

    }
}
