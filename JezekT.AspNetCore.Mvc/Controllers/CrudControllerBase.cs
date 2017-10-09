using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AutoMapper;
using JezekT.AspNetCore.Mvc.Extensions;
using JezekT.NetStandard.Data;
using JezekT.NetStandard.Services.EntityOperations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.Mvc.Controllers
{
    public abstract class CrudControllerBase<T, TCreateVM, TDetailsVM, TEditVM, TDeleteVM, TId> : Controller
        where T : class, IWithId<TId>
        where TCreateVM : class
        where TDetailsVM : class
        where TEditVM : class
        where TDeleteVM : class
    {
        protected readonly ILogger Logger;
        protected ICrudService<T, TId> Service { get; }
        protected IMapper Mapper { get; }


        protected virtual IActionResult CreateRedirect => RedirectToAction("Index");
        protected virtual IActionResult EditRedirect => RedirectToAction("Index");
        protected virtual IActionResult DeleteRedirect => RedirectToAction("Index");

        protected virtual IActionResult GetCreateErrorActionResult(TCreateVM vm) => null;
        protected virtual IActionResult GetEditErrorActionResult(TEditVM vm) => null;
        protected virtual IActionResult GetDeleteErrorActionResult(TDeleteVM vm) => null;


        protected virtual async Task<T> GetEntityAsync(TId id)
        {
            return await Service.GetByIdAsync(id);
        }

        protected virtual async Task<TCreateVM> GetCreateViewModelAsync()
        {
            await Task.CompletedTask;
            return null;
        }

        protected virtual async Task<TDetailsVM> GetDetailsViewModelAsync(T obj)
        {
            await Task.CompletedTask;
            return Mapper.Map<T, TDetailsVM>(obj);
        }

        protected virtual async Task<TEditVM> GetEditViewModelAsync(T obj)
        {
            await Task.CompletedTask;
            return Mapper.Map<T, TEditVM>(obj);
        }

        protected virtual async Task<TDeleteVM> GetDeleteViewModelAsync(T obj)
        {
            await Task.CompletedTask;
            return Mapper.Map<T, TDeleteVM>(obj);
        }

        public virtual IActionResult Index()
        {
            return View();
        }

        public virtual async Task<IActionResult> Details(TId id)
        {
            var obj = await GetEntityAsync(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(await GetDetailsViewModelAsync(obj));
        }


        public virtual async Task<IActionResult> Create()
        {
            return View(await GetCreateViewModelAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TCreateVM objVm, string redirectUrl = null)
        {
            var obj = Mapper.Map<TCreateVM, T>(objVm);
            if (ModelState.IsValid && await Service.CreateAsync(obj))
            {
                Logger?.LogInformation($"{obj.GetType().Name} Id {obj.Id} created by {User?.Identity.Name}.");
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    return CreateRedirect;
                }
                return Redirect(redirectUrl);
            }
            Logger?.LogInformation($"Failed to create {obj.GetType().Name} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);

            var errorActionResult = GetCreateErrorActionResult(objVm);
            if (errorActionResult != null) return errorActionResult;
            return View(objVm);
        }


        public virtual async Task<IActionResult> Edit(TId id)
        {
            var obj = await GetEntityAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(await GetEditViewModelAsync(obj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(TEditVM objVm, string redirectUrl = null)
        {
            var obj = Mapper.Map<TEditVM, T>(objVm);
            if (ModelState.IsValid && await Service.UpdateAsync(obj))
            {
                Logger?.LogInformation($"{obj.GetType().Name} Id {obj.Id} updated by {User?.Identity.Name}.");
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    return EditRedirect;
                }
                return Redirect(redirectUrl);
            }
            Logger?.LogInformation($"Failed to update {obj.GetType().Name} Id {obj.Id} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);

            var errorActionResult = GetEditErrorActionResult(objVm);
            if (errorActionResult != null) return errorActionResult;
            return View("",objVm);
        }


        public virtual async Task<IActionResult> Delete(TId id)
        {
            var obj = await GetEntityAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(await GetDeleteViewModelAsync(obj));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(TId id, string redirectUrl = null)
        {
            if (await Service.DeleteByIdAsync(id))
            {
                Logger?.LogInformation($"{typeof(T).Name} Id {id} removed by {User?.Identity.Name}.");
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    return DeleteRedirect;
                }
                return Redirect(redirectUrl);
            }
            Logger?.LogInformation($"Failed to remove {typeof(T).Name} Id {id} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);

            var objVm = await GetDeleteViewModelAsync(await GetEntityAsync(id));
            var errorActionResult = GetDeleteErrorActionResult(objVm);
            if (errorActionResult != null) return errorActionResult;
            return View(objVm);
        }


        protected CrudControllerBase(ICrudService<T, TId> service, IMapper mapper, ILogger logger = null)
        {
            if (service == null || mapper == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            Service = service;
            Mapper = mapper;
            Logger = logger;
        }

    }
}
