using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.AspNetCore.IdentityServer4.WebApp.Extensions;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.RoleViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.RoleServices;
using JezekT.NetStandard.Pagination.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "UserAdministratorOnly")]
    public class RolesController : Controller
    {
        private readonly IdentityServerDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger _logger;
        private readonly ILogger<RolePaginationProvider> _paginationLogger;


        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var result = await _roleManager.CreateAsync(new IdentityRole { Name = vm.Name });
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role {vm.Name} created by {User?.Identity?.Name}.");
                    return RedirectToAction("Index");
                }

                _logger.LogError($"Role create error. {string.Join(";", result.Errors.Select(x => x.ToString()).ToArray())}");
                ModelState.AddErrors(result.Errors.ToArray());
            }
            return View(vm);
        }


        public async Task<IActionResult> Edit(string id)
        {
            var vm = await GetRoleViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(RoleViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var role = await _dbContext.Roles.FindAsync(vm.Id);
                if (role == null)
                {
                    return NotFound();
                }

                role.Name = vm.Name;
                var result = await _roleManager.UpdateAsync(role);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Role {vm.Name} updated by {User?.Identity?.Name}.");
                    return RedirectToAction("Index");
                }

                _logger.LogError($"Role edit error. {string.Join(";", result.Errors.Select(x => x.ToString()).ToArray())}");
                ModelState.AddErrors(result.Errors.ToArray());
            }
            return View(vm);
        }


        public async Task<IActionResult> Delete(string id)
        {
            var vm = await GetRoleViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(RoleViewModel vm)
        {
            var role = await _dbContext.Roles.FindAsync(vm.Id);
            if (role == null)
            {
                return NotFound();
            }
            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                _logger.LogInformation($"Role {role.Name} removed by {User?.Identity?.Name}.");
                return RedirectToAction("Index");
            }

            _logger.LogError($"Role delete error. {string.Join(";", result.Errors.Select(x => x.ToString()).ToArray())}");
            ModelState.AddErrors(result.Errors.ToArray());
            return View(vm);
        }


        public async Task<IActionResult> GetTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection, string queryIds)
        {
            var rolePaginationProvider = new RolePaginationProvider(_dbContext.Roles, _paginationLogger);
            var paginationData = await rolePaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, queryIds.ToIdsArray<string>());
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }


        public RolesController(IdentityServerDbContext dbContext, RoleManager<IdentityRole> roleManager, ILogger<RolesController> logger, ILogger<RolePaginationProvider> paginationLogger)
        {
            if (dbContext == null || roleManager == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _roleManager = roleManager;
            _logger = logger;
            _paginationLogger = paginationLogger;
        }


        private async Task<RoleViewModel> GetRoleViewModelAsync(string roleId)
        {
            var role = await _dbContext.Roles.FindAsync(roleId);
            if (role != null)
            {
                return new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                };
            }
            return null;
        }

    }
}
