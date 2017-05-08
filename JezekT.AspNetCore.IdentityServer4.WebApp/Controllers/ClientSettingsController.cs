using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientGrantTypeViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public abstract class ClientSettingsController<TViewModel, TEntity> : Controller where TViewModel : ClientStringSettingsViewModel where TEntity : class
    {
        private readonly ConfigurationDbContext _dbContext;


        public IActionResult Create(int clientId)
        {
            var vm = Activator.CreateInstance<TViewModel>();
            vm.ClientId = clientId;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TViewModel vm)
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
                
                AddEntityToContext(vm, _dbContext, client);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
            }
            return View(vm);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var vm = await GetViewModel(id, _dbContext);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var obj = await _dbContext.Set<TEntity>().FindAsync(vm.Id);
                if (obj == null)
                {
                    return NotFound();
                }

                EntityUpdate(obj, vm);
                await _dbContext.SaveChangesAsync();

                return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
            }
            return View(vm);
        }

        
        public async Task<IActionResult> Delete(int id)
        {
            var vm = await GetViewModel(id, _dbContext);
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

            var obj = await _dbContext.Set<TEntity>().FindAsync(vm.Id);
            if (obj == null)
            {
                return NotFound();
            }

            _dbContext.Remove(obj);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
        }


        protected abstract void AddEntityToContext(TViewModel vm, ConfigurationDbContext dbContext, Client client);
        protected abstract void EntityUpdate(TEntity obj, TViewModel vm);
        protected abstract Task<TViewModel> GetViewModel(int id, ConfigurationDbContext dbContext);


        protected ClientSettingsController(ConfigurationDbContext dbContext)
        {
            if (dbContext == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
        }
        
    }
}
