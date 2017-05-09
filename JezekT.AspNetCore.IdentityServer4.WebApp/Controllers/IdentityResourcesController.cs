using System;
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

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class IdentityResourcesController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IPaginationDataProvider<IdentityResource, object> _identityResourcePaginationProvider;


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
                _dbContext.IdentityResources.Add(GetIdentityResource(vm));
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
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
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
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
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> GetTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection, string queryIds)
        {
            var filterIds = queryIds.ToIdsArray<int>();
            var filterIdsExpression = filterIds != null ? (Expression<Func<IdentityResource, bool>>)(x => filterIds.Contains(x.Id)) : null;
            var paginationData = await _identityResourcePaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, filterIdsExpression);
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }


        public IdentityResourcesController(ConfigurationDbContext dbContext, IPaginationDataProvider<IdentityResource, object> identityResourcePaginationDataProvider)
        {
            if (dbContext == null || identityResourcePaginationDataProvider == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _identityResourcePaginationProvider = identityResourcePaginationDataProvider;
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
