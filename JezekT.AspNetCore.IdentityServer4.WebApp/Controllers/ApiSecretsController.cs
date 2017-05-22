using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using IdentityServer4.Models;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiSecretViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "AdministratorOnly")]
    public class ApiSecretsController : Controller
    {
        private readonly ConfigurationDbContext _dbContext;
        private readonly ILogger _logger;


        public IActionResult Create(int apiResourceId)
        {
            var vm = new ApiSecretInputViewModel{ ApiResourceId = apiResourceId, Expiration = DateTime.Today.AddYears(1)};
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiSecretInputViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var apiResource = await _dbContext.ApiResources.FindAsync(vm.ApiResourceId);
                if (apiResource == null)
                {
                    return BadRequest();
                }

                var apiSecret = new ApiSecret
                {
                    ApiResource = apiResource,
                    Description = vm.Description,
                    Value = vm.Value.Sha256(),
                    Expiration = vm.Expiration
                };
                if (!string.IsNullOrEmpty(vm.Type))
                {
                    apiSecret.Type = vm.Type;
                }

                _dbContext.Set<ApiSecret>().Add(apiSecret);
                try
                {
                    await _dbContext.SaveChangesAsync();
                    _logger.LogInformation($"API secret Id {apiSecret.Id} created by {User?.Identity?.Name}.");
                    return RedirectToAction("Edit", "ApiResources", new { id = vm.ApiResourceId });
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
        public async Task<IActionResult> Edit(ApiSecretViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var obj = await _dbContext.Set<ApiSecret>().FindAsync(vm.Id);
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
                    _logger.LogInformation($"API secret Id {obj.Id} updated by {User?.Identity?.Name}.");
                    return RedirectToAction("Edit", "ApiResources", new { id = vm.ApiResourceId });
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
        public async Task<IActionResult> Delete(ApiSecretViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            var obj = await _dbContext.Set<ApiSecret>().FindAsync(vm.Id);
            if (obj == null)
            {
                return NotFound();
            }

            _dbContext.Remove(obj);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"API secret Id {obj.Id} removed by {User?.Identity?.Name}.");
                return RedirectToAction("Edit", "ApiResources", new { id = vm.ApiResourceId });
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }
        }


        public ApiSecretsController(ConfigurationDbContext dbContext, ILogger<ApiSecretsController> logger)
        {
            if (dbContext == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _logger = logger;
        }


        private async Task<ApiSecretViewModel> GetViewModel(int id)
        {
            var apiSecret = await _dbContext.Set<ApiSecret>().Include(x => x.ApiResource).FirstOrDefaultAsync(x => x.Id == id);
            if (apiSecret != null)
            {
                return new ApiSecretViewModel
                {
                    Id = apiSecret.Id,
                    ApiResourceId = apiSecret.ApiResource.Id,
                    Type = apiSecret.Type,
                    Description = apiSecret.Description,
                    Expiration = apiSecret.Expiration
                };
            }
            return null;
        }
    }
}
