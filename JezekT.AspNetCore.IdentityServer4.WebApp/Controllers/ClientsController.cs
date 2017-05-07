using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        //private readonly IMapper _mapper;
        private readonly IPaginationDataProvider<Client, object> _clientPaginationProvider;


        public IActionResult Index()
        {
            return View();
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var vm = await GetClientViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }
        

        public IActionResult Create()
        {
            return View(new ClientViewModel{Enabled = true});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }
            
            if (ModelState.IsValid)
            {
                _dbContext.Clients.Add(GetClient(vm));
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(vm);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var vm = await GetClientViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.FindAsync(vm.Id);
                client.Enabled = vm.Enabled;
                client.ClientId = vm.ClientId;
                client.ClientName = vm.ClientName;
                client.AllowOfflineAccess = vm.AllowOfflineAccess;
                client.AlwaysSendClientClaims = vm.AlwaysSendClientClaims;
                client.IdentityTokenLifetime = vm.IdentityTokenLifetime;
                client.AccessTokenLifetime = vm.AccessTokenLifetime;
                await _dbContext.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(vm);
        }


        public async Task<IActionResult> Delete(int id)
        {
            var vm = await GetClientViewModelAsync(id);
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

            var client = await _dbContext.Clients.FindAsync(vm.Id);
            if (client == null)
            {
                return NotFound();
            }
            _dbContext.Clients.Remove(client);
            await _dbContext.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> GetTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection, string queryIds)
        {
            var filterIds = queryIds.ToIdsArray<int>();
            var filterIdsExpression = filterIds != null ? (Expression<Func<Client, bool>>)(x => filterIds.Contains(x.Id)) : null;
            var paginationData = await _clientPaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, filterIdsExpression);
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }


        public ClientsController(ConfigurationDbContext dbContext, IPaginationDataProvider<Client, object> clientPaginationDataProvider)
        {
            if (dbContext == null || clientPaginationDataProvider == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            //_mapper = mapper;
            _clientPaginationProvider = clientPaginationDataProvider;
        }


        private async Task<ClientViewModel> GetClientViewModelAsync(int id)
        {
            var client = await _dbContext.Clients.FindAsync(id);
            if (client != null)
            {
                return new ClientViewModel
                {
                    ClientId = client.ClientId,
                    ClientName = client.ClientName,
                    AllowOfflineAccess = client.AllowOfflineAccess,
                    AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                    IdentityTokenLifetime = client.IdentityTokenLifetime,
                    AccessTokenLifetime = client.AccessTokenLifetime
                };
            }
            return null;
        }

        private Client GetClient(ClientViewModel vm)
        {
            if (vm != null)
            {
                return new Client
                {
                    Enabled = vm.Enabled,
                    ClientId = vm.ClientId,
                    ClientName = vm.ClientName,
                    AllowOfflineAccess = vm.AllowOfflineAccess,
                    AlwaysSendClientClaims = vm.AlwaysSendClientClaims,
                    IdentityTokenLifetime = vm.IdentityTokenLifetime,
                    AccessTokenLifetime = vm.AccessTokenLifetime
                };
            }
            return null;
        }

        /*
                client.AllowedScopes = vm.AllowedScopes.Select(x => new ClientScope {Client = client, Scope = x.Value}).ToList();
                client.AllowedGrantTypes = vm.AllowedScopes.Select(x => new ClientGrantType { Client = client, GrantType = x.Value }).ToList();
                client.RedirectUris = vm.AllowedScopes.Select(x => new ClientRedirectUri { Client = client, RedirectUri = x.Value }).ToList();
                client.PostLogoutRedirectUris = vm.AllowedScopes.Select(x => new ClientPostLogoutRedirectUri { Client = client, PostLogoutRedirectUri = x.Value }).ToList();
                client.ClientSecrets = vm.AllowedScopes.Select(x => new ClientScope { Client = client, Scope = x.Value }).ToList();

         */
    }
}
