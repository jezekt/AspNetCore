using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientClaimViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "UserAdministratorOnly")]
    public class ClientClaimsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly ILogger _logger;


        public IActionResult Create(int clientId)
        {
            return View(new ClientClaimViewModel { ClientId = clientId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.Where(x => x.Id == vm.ClientId).Include(x => x.Claims).FirstOrDefaultAsync();
                if (client == null)
                {
                    return BadRequest();
                }

                client.Claims.Add(new ClientClaim
                {
                    Type = vm.ClaimType,
                    Value = vm.ClaimValue
                });
                _dbContext.Entry(client).State = EntityState.Modified;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Client claim Type {vm.ClaimType}, Value {vm.ClaimValue} created by {User?.Identity?.Name}.");
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                    throw;
                }
            }

            return RedirectToAction("Edit", "Clients", new {id = vm.ClientId});
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vm = await GetClientClaimViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.Where(x => x.Id == vm.ClientId).Include(x => x.Claims).FirstOrDefaultAsync();
                if (client == null)
                {
                    return NotFound();
                }

                var claim = client.Claims.FirstOrDefault(x => x.Id == vm.Id);
                if (claim == null)
                {
                    return NotFound();
                }

                claim.Type = vm.ClaimType;
                claim.Value = vm.ClaimValue;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Client claim Type {vm.ClaimType}, Value {vm.ClaimValue} updated by {User?.Identity?.Name}.");
                }
                catch (DbException ex)
                {
                    _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                    throw;
                }
            }

            return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vm = await GetClientClaimViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(ClientClaimViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var client = await _dbContext.Clients.Where(x => x.Id == vm.ClientId).Include(x => x.Claims).FirstOrDefaultAsync();
            if (client == null)
            {
                return NotFound();
            }
            var claim = client.Claims.FirstOrDefault(x => x.Id == vm.Id);
            if (claim == null)
            {
                return NotFound();
            }

            _dbContext.Entry(claim).State = EntityState.Deleted;

            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Client claim Type {vm.ClaimType}, Value {vm.ClaimValue} deleted by {User?.Identity?.Name}.");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }

            return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
        }


        public ClientClaimsController(ConfigurationDbContext dbContext, ILogger<UserClaimsController> logger)
        {
            if (dbContext == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _logger = logger;
        }


        private async Task<ClientClaimViewModel> GetClientClaimViewModelAsync(int id)
        {
            var claim = await _dbContext.Clients.SelectMany(x => x.Claims).Where(x => x.Id == id).Include(x => x.Client).FirstOrDefaultAsync();
            if (claim != null)
            {
                return new ClientClaimViewModel
                {
                    Id = claim.Id,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value,
                    ClientId = claim.Client.Id
                };
            }
            return null;
        }

    }
}
