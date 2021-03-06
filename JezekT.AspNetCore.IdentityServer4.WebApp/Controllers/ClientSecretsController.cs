﻿using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientSecretViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class ClientSecretsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly ILogger _logger;


        public IActionResult Create(int clientId)
        {
            var vm = new ClientSecretInputViewModel{ ClientId = clientId, Expiration = DateTime.Today.AddYears(1)};
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientSecretInputViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var client = await _dbContext.Clients.FindAsync(vm.ClientId);
                if (client == null)
                {
                    return BadRequest();
                }

                var clientSecret = new ClientSecret
                {
                    Client = client,
                    Description = vm.Description,
                    Value = vm.Value.Sha256(),
                    Expiration = vm.Expiration
                };
                if (!string.IsNullOrEmpty(vm.Type))
                {
                    clientSecret.Type = vm.Type;
                }

                _dbContext.Set<ClientSecret>().Add(clientSecret);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Client secret Id {clientSecret.Id} created by {User?.Identity?.Name}.");
                    return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
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
        public async Task<IActionResult> Edit(ClientSecretViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var obj = await _dbContext.Set<ClientSecret>().FindAsync(vm.Id);
                if (obj == null)
                {
                    return NotFound();
                }

                obj.Type = vm.Type;
                obj.Description = vm.Description;
                obj.Expiration = vm.Expiration;

                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Client secret Id {obj.Id} updated by {User?.Identity?.Name}.");
                    return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
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
        public async Task<IActionResult> Delete(ClientSecretViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var obj = await _dbContext.Set<ClientSecret>().FindAsync(vm.Id);
            if (obj == null)
            {
                return NotFound();
            }

            _dbContext.Remove(obj);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Client secret Id {obj.Id} removed by {User?.Identity?.Name}.");
                return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }
        }


        public ClientSecretsController(ConfigurationDbContext dbContext, ILogger<ClientSecretsController> logger)
        {
            if (dbContext == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _logger = logger;
        }


        private async Task<ClientSecretViewModel> GetViewModel(int id)
        {
            var clientSecret = await _dbContext.Set<ClientSecret>().Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id);
            if (clientSecret != null)
            {
                return new ClientSecretViewModel
                {
                    Id = clientSecret.Id,
                    ClientId = clientSecret.Client.Id,
                    Type = clientSecret.Type,
                    Description = clientSecret.Description,
                    Expiration = clientSecret.Expiration
                };
            }
            return null;
        }
    }
}
