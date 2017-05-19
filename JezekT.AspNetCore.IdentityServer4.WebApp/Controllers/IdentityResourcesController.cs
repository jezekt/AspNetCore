using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.IdentityClaimViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.IdentityResourceViewModels;
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
    public class IdentityResourcesController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IPaginationDataProvider<IdentityResource, object> _identityResourcePaginationProvider;
        private readonly ILogger _logger;


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(int id)
        {
            var vm = await GetIdentityResourceViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }


        public IActionResult Create()
        {
            return View(new IdentityResourceViewModel { Enabled = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(IdentityResourceViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var obj = GetIdentityResource(vm);
                _dbContext.IdentityResources.Add(obj);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Identity resource Id {obj.Id} created by {User?.Identity?.Name}.");
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
            var vm = await GetIdentityResourceViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(IdentityResourceViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var identityResource = await _dbContext.IdentityResources.FindAsync(vm.Id);
                identityResource.Enabled = vm.Enabled;
                identityResource.Name = vm.Name;
                identityResource.DisplayName = vm.DisplayName;
                identityResource.Description = vm.Description;
                identityResource.Required = vm.Required;
                identityResource.ShowInDiscoveryDocument = vm.ShowInDiscoveryDocument;
                identityResource.Emphasize = vm.Emphasize;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Identity resource Id {identityResource.Id} updated by {User?.Identity?.Name}.");
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
            var vm = await GetIdentityResourceViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ClientViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var identityResource = await _dbContext.IdentityResources.FindAsync(vm.Id);
            if (identityResource == null)
            {
                return NotFound();
            }

            _dbContext.IdentityResources.Remove(identityResource);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Identity resource Id {vm.Id} removed by {User?.Identity?.Name}.");
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
            var filterIdsExpression = filterIds != null ? (Expression<Func<IdentityResource, bool>>)(x => filterIds.Contains(x.Id)) : null;
            var paginationData = await _identityResourcePaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, filterIdsExpression);
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }


        public IdentityResourcesController(ConfigurationDbContext dbContext, IPaginationDataProvider<IdentityResource, object> identityResourcePaginationDataProvider, 
            ILogger<IdentityResourcesController> logger)
        {
            if (dbContext == null || identityResourcePaginationDataProvider == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _identityResourcePaginationProvider = identityResourcePaginationDataProvider;
            _logger = logger;
        }


        private async Task<IdentityResourceViewModel> GetIdentityResourceViewModelAsync(int id)
        {
            var identityResource = await _dbContext.IdentityResources.Include(x => x.UserClaims).FirstOrDefaultAsync(x => x.Id == id);
            if (identityResource != null)
            {
                return new IdentityResourceViewModel
                {
                    Id = identityResource.Id,
                    Enabled = identityResource.Enabled,
                    Name = identityResource.Name,
                    DisplayName = identityResource.DisplayName,
                    Description = identityResource.Description,
                    Required = identityResource.Required,
                    Emphasize = identityResource.Emphasize,
                    ShowInDiscoveryDocument = identityResource.ShowInDiscoveryDocument,
                    UserClaims = identityResource.UserClaims.Select(x => new IdentityClaimViewModel{Id = x.Id, IdentityResourceId = identityResource.Id, Type = x.Type}).ToList()
                };
            }
            return null;
        }

        private IdentityResource GetIdentityResource(IdentityResourceViewModel vm)
        {
            if (vm != null)
            {
                return new IdentityResource
                {
                    Enabled = vm.Enabled,
                    Name = vm.Name,
                    DisplayName = vm.DisplayName,
                    Description = vm.Description,
                    Required = vm.Required,
                    ShowInDiscoveryDocument = vm.ShowInDiscoveryDocument,
                    Emphasize = vm.Emphasize
                };
            }
            return null;
        }

    }
}
