using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientGrantTypeViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientSecretViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class ClientSecretsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;


        public IActionResult Create(int clientId)
        {
            var vm = new ClientSecretViewModel{ ClientId = clientId, Expiration = DateTime.Today.AddYears(1)};
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientSecretViewModel vm)
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
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
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
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
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
        public async Task<IActionResult> Delete(ClientGrantTypeViewModel vm)
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
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
        }


        public ClientSecretsController(ConfigurationDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
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
