using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientGrantTypeViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientPostLogoutRedirectUriViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientRedirectUriViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientScopeViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientSecretViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;
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
    public class ClientsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly IPaginationDataProvider<Client, object> _clientPaginationProvider;
        private readonly ILogger _logger;


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
                var obj = GetClient(vm);
                _dbContext.Clients.Add(obj);

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Client Id {obj.Id} created by {User?.Identity?.Name}.");
                    return RedirectToAction("Index");
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message?? ex.Message);
                    throw;
                }
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
                if (vm.IdentityTokenLifetime != null)
                {
                    client.IdentityTokenLifetime = (int)vm.IdentityTokenLifetime;
                }
                if (vm.AccessTokenLifetime != null)
                {
                    client.AccessTokenLifetime = (int)vm.AccessTokenLifetime;
                }

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Client Id {client.Id} updated by {User?.Identity?.Name}.");
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
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Client Id {client.Id} removed by {User?.Identity?.Name}.");
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
            var filterIdsExpression = filterIds != null ? (Expression<Func<Client, bool>>)(x => filterIds.Contains(x.Id)) : null;
            var paginationData = await _clientPaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, filterIdsExpression);
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }


        public ClientsController(ConfigurationDbContext dbContext, IPaginationDataProvider<Client, object> clientPaginationDataProvider, ILogger<ClientsController> logger)
        {
            if (dbContext == null || clientPaginationDataProvider == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _clientPaginationProvider = clientPaginationDataProvider;
            _logger = logger;
        }


        private async Task<ClientViewModel> GetClientViewModelAsync(int id)
        {
            var client = await _dbContext.Clients
                .Include(x => x.AllowedGrantTypes).ThenInclude(x => x.Client)
                .Include(x => x.AllowedScopes).ThenInclude(x => x.Client)
                .Include(x => x.RedirectUris).ThenInclude(x => x.Client)
                .Include(x => x.PostLogoutRedirectUris).ThenInclude(x => x.Client)
                .Include(x => x.ClientSecrets).ThenInclude(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (client != null)
            {
                return new ClientViewModel
                {
                    Id = client.Id,
                    Enabled = client.Enabled,
                    ClientId = client.ClientId,
                    ClientName = client.ClientName,
                    AllowOfflineAccess = client.AllowOfflineAccess,
                    AlwaysSendClientClaims = client.AlwaysSendClientClaims,
                    IdentityTokenLifetime = client.IdentityTokenLifetime,
                    AccessTokenLifetime = client.AccessTokenLifetime,
                    AllowedScopes = client.AllowedScopes.Select(x => new ClientScopeViewModel { Id = x.Id, ClientId = x.Client.Id, Scope = x.Scope }).ToList(),
                    AllowedGrantTypes = client.AllowedGrantTypes.Select(x => new ClientGrantTypeViewModel{Id = x.Id, ClientId = x.Client.Id, GrantType = x.GrantType}).ToList(),
                    RedirectUris = client.RedirectUris.Select(x => new ClientRedirectUriViewModel { Id = x.Id, ClientId = x.Client.Id, RedirectUri = x.RedirectUri }).ToList(),
                    PostLogoutRedirectUris = client.PostLogoutRedirectUris.Select(x => new ClientPostLogoutRedirectUriViewModel { Id = x.Id, ClientId = x.Client.Id, PostLogoutRedirectUri = x.PostLogoutRedirectUri }).ToList(),
                    ClientSecrets = client.ClientSecrets.Select(x => new ClientSecretViewModel{Id = x.Id, ClientId = x.Client.Id, Type = x.Type, Description = x.Description, Expiration = x.Expiration}).ToList()
                };
            }
            return null;
        }

        private Client GetClient(ClientViewModel vm)
        {
            if (vm != null)
            {
                var client = new Client
                {
                    Enabled = vm.Enabled,
                    ClientId = vm.ClientId,
                    ClientName = vm.ClientName,
                    AllowOfflineAccess = vm.AllowOfflineAccess,
                    AlwaysSendClientClaims = vm.AlwaysSendClientClaims,
                };

                if (vm.IdentityTokenLifetime != null)
                {
                    client.IdentityTokenLifetime = (int)vm.IdentityTokenLifetime;
                }
                if (vm.AccessTokenLifetime != null)
                {
                    client.AccessTokenLifetime = (int)vm.AccessTokenLifetime;
                }
                return client;
            }
            return null;
        }
    }
}
