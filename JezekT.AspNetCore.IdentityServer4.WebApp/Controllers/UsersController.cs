using System;
using System.Data.Common;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.AspNetCore.IdentityServer4.WebApp.Extensions;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.UserViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.RoleServices;
using JezekT.AspNetCore.Select2.Data;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    [Authorize(Policy = "UserAdministratorOnly")]
    public class UsersController : Controller
    {

        private readonly IdentityServerDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly IPaginationDataProvider<User, object> _userPaginationProvider;
        private readonly IPaginationDataProvider<IdentityUserClaim<string>, object> _userClaimPaginationProvider;
        private readonly IPaginationDataProvider<IdentityRole, object> _rolePaginationProvider;
        private readonly ILogger _logger;


        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Details(string id)
        {
            var vm = await GetUserViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }


        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(new User { UserName = vm.Username, Email = vm.Email }, vm.Password);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {vm.Username} created by {User?.Identity?.Name}.");
                    return RedirectToAction("Index");
                }

                _logger.LogError($"User create error. {string.Join(";", result.Errors.Select(x => x.ToString()).ToArray())}");
                ModelState.AddErrors(result.Errors.ToArray());
            }
            return View(vm);

        }


        public async Task<IActionResult> Edit(string id)
        {
            var vm = await GetUserViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var user = await _dbContext.Users.FindAsync(vm.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.Email = vm.Email;
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    _logger.LogInformation($"User {vm.Username} updated by {User?.Identity?.Name}.");
                    return RedirectToAction("Index");
                }

                _logger.LogError($"User update error. {string.Join(";", result.Errors.Select(x => x.ToString()).ToArray())}");
                ModelState.AddErrors(result.Errors.ToArray());
            }
            return View(vm);
        }


        public async Task<IActionResult> Delete(string id)
        {
            var vm = await GetUserViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(UserViewModel vm)
        {
            var user = await _dbContext.Users.FindAsync(vm.Id);
            if (user == null)
            {
                return NotFound();
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                _logger.LogInformation($"User {user.UserName} removed by {User?.Identity?.Name}.");
                return RedirectToAction("Index");
            }

            _logger.LogError($"User delete error. {string.Join(";", result.Errors.Select(x => x.ToString()).ToArray())}");
            ModelState.AddErrors(result.Errors.ToArray());
            return View(vm);
        }


        public IActionResult AddRole(string userId)
        {
            return View(new UserRoleViewModel {UserId = userId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRole(UserRoleViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var user = await _dbContext.Users.FindAsync(vm.UserId);
                if (user == null)
                {
                    return BadRequest();
                }

                var role = await _dbContext.Roles.FindAsync(vm.RoleId);
                if (role == null)
                {
                    return BadRequest();
                }

                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    var result = await _userManager.AddToRoleAsync(user, role.Name);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation($"Role Id {vm.RoleId} assigned to User Id {vm.UserId} by {User?.Identity?.Name}.");
                        return RedirectToAction("Edit", new { id = vm.UserId });
                    }

                    _logger.LogError($"Role assign error. {string.Join(";", result.Errors.Select(x => x.ToString()).ToArray())}");
                    ModelState.AddErrors(result.Errors.ToArray());
                }
                else
                {
                    ModelState.AddModelError(nameof(UserRoleViewModel.RoleId), Resources.Controllers.SharedResources.UserAlreadyInRole);
                }
            }

            return View(vm);
        }

        public async Task<IActionResult> DeleteRole(string userId, string roleId)
        {
            var userRole = await _dbContext.UserRoles.SingleOrDefaultAsync(x => x.UserId == userId && x.RoleId == roleId);
            if (userRole == null)
            {
                return NotFound();
            }

            return View(new UserRoleViewModel { UserId = userId, RoleId = roleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRole(UserRoleViewModel vm)
        {
            var userRole = await _dbContext.UserRoles.SingleOrDefaultAsync(x => x.UserId == vm.UserId && x.RoleId == vm.RoleId);
            if (userRole == null)
            {
                return NotFound();
            }

            _dbContext.UserRoles.Remove(userRole);
            try
            {
                await _dbContext.SaveChangesAsync();
                _logger.LogInformation($"Role Id {vm.RoleId} removed from User Id {vm.UserId} by {User?.Identity?.Name}.");
            }
            catch (DbException ex)
            {
                _logger.LogError(ex.GetBaseException()?.Message ?? ex.Message);
                throw;
            }

            return RedirectToAction("Edit", "Users", new {id = vm.UserId});
        }

        public async Task<IActionResult> GetTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection, string queryIds)
        {
            var filterIds = queryIds.ToIdsArray<string>();
            var filterIdsExpression = filterIds != null ? (Expression<Func<User, bool>>)(x => filterIds.Contains(x.Id)) : null;
            var paginationData = await _userPaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, filterIdsExpression);
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }

        public async Task<IActionResult> GetClaimsTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection, string queryIds)
        {
            var filterIds = queryIds.ToIdsArray<int>();
            var filterIdsExpression = filterIds != null ? (Expression<Func<IdentityUserClaim<string>, bool>>)(x => filterIds.Contains(x.Id)) : null;
            var paginationData = await _userClaimPaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, filterIdsExpression);
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }

        public async Task<IActionResult> GetRolesTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection, string queryIds)
        {
            var filterIds = queryIds.ToIdsArray<string>();
            var filterIdsExpression = filterIds != null ? (Expression<Func<IdentityRole, bool>>)(x => filterIds.Contains(x.Id)) : null;
            var paginationData = await _rolePaginationProvider.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection, filterIdsExpression);
            return Json(PaginationHelper.GetDataObject(paginationData, draw));
        }


        public async Task<IActionResult> GetRolesSelectDataJsonAsync(string term, string page, int pageSize)
        {
            var start = int.Parse(page) - 1;
            var paginationData = await _rolePaginationProvider.GetPaginationDataAsync<RolePaginationSelect2Template>(start, pageSize, term);
            return Json(new SelectDropdownDataResponse(paginationData.Items, paginationData.RecordsTotal, paginationData.RecordsFiltered, start, pageSize));
        }

        public async Task<IActionResult> GetRoleSelectDataJsonAsync(string id)
        {
            var filterIds = new[] {id};
            var filterIdsExpression = (Expression<Func<IdentityRole, bool>>) (x => filterIds.Contains(x.Id));
            var paginationData = await _rolePaginationProvider.GetPaginationDataAsync<RolePaginationSelect2Template>(0, 1, null, null, null, filterIdsExpression);
            return Json(paginationData?.Items?.FirstOrDefault());
        }


        public UsersController(IdentityServerDbContext dbContext, IPaginationDataProvider<User, object> userPaginationProvider, 
            IPaginationDataProvider<IdentityUserClaim<string>, object> userClaimPaginationProvider, 
            IPaginationDataProvider<IdentityRole, object> rolePaginationProvider, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, 
            ILogger<UsersController> logger)
        {
            if (dbContext == null || userPaginationProvider == null || userClaimPaginationProvider == null || 
                rolePaginationProvider == null || userManager == null || logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _dbContext = dbContext;
            _userPaginationProvider = userPaginationProvider;
            _userClaimPaginationProvider = userClaimPaginationProvider;
            _rolePaginationProvider = rolePaginationProvider;
            _userManager = userManager;
            _logger = logger;
        }


        private async Task<UserViewModel> GetUserViewModelAsync(string id)
        {
            var user = await _dbContext.Users.Where(x => x.Id == id).Include(x => x.Roles).FirstOrDefaultAsync();
            if (user != null)
            {
                var claimIds = await _dbContext.UserClaims.Where(x => x.UserId == id).Select(x => x.Id).ToArrayAsync();
                var roleIds = user.Roles.Select(x => x.RoleId).ToArray();
                return new UserViewModel
                {
                    Id = id,
                    Email = user.Email,
                    Username = user.UserName,
                    UserClaimIds = claimIds.ToIdsString(),
                    RoleIds = roleIds.ToIdsString()
                };
            }
            return null;
        }
    }
}
