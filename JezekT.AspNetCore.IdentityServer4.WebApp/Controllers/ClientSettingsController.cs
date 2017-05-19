using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public abstract class ClientSettingsController<TViewModel, TEntity> : Controller where TViewModel : ClientStringSettingsViewModel where TEntity : class
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly ILogger _logger;


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
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Client settings {vm.GetType().Name} value {vm.Value} created by {User?.Identity?.Name}.");
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
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"Client settings {vm.GetType().Name} Id {vm.Id} updated by {User?.Identity?.Name}.");
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
            var vm = await GetViewModel(id, _dbContext);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TViewModel vm)
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
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Client settings {vm.GetType().Name} Id {vm.Id} removed by {User?.Identity?.Name}.");
                return RedirectToAction("Edit", "Clients", new { id = vm.ClientId });
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }
        }


        protected abstract void AddEntityToContext(TViewModel vm, ConfigurationDbContext dbContext, Client client);
        protected abstract void EntityUpdate(TEntity obj, TViewModel vm);
        protected abstract Task<TViewModel> GetViewModel(int id, ConfigurationDbContext dbContext);


        protected ClientSettingsController(ConfigurationDbContext dbContext, ILogger logger)
        {
            if (dbContext == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _logger = logger;
        }
        
    }
}
