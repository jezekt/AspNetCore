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
        where T : class,  IWithId<TId>
        where TCreateVM : class
        where TDetailsVM : class
        where TEditVM : class
        where TDeleteVM : class
    {
        private readonly ILogger _logger;
        protected ICrudService<T, TId> Service { get; }
        protected IMapper Mapper { get; }


        protected virtual IActionResult CreateRedirect => RedirectToAction("Index");
        protected virtual IActionResult EditRedirect => RedirectToAction("Index");
        protected virtual IActionResult DeleteRedirect => RedirectToAction("Index");

        protected virtual async Task<T> GetEntityAsync(TId id)
        {
            return await Service.GetByIdAsync(id);
        }

        protected virtual TDetailsVM GetDetailsViewModel(T obj)
        {
            return Mapper.Map<T, TDetailsVM>(obj);
        }

        protected virtual TEditVM GetEditViewModel(T obj)
        {
            return Mapper.Map<T, TEditVM>(obj);
        }

        protected virtual TDeleteVM GetDeleteViewModel(T obj)
        {
            return Mapper.Map<T, TDeleteVM>(obj);
        }



        public virtual ActionResult Index()
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

            return View(GetDetailsViewModel(obj));
        }


        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TCreateVM objVm, string redirectUrl = null)
        {
            var obj = Mapper.Map<TCreateVM, T>(objVm);
            if (ModelState.IsValid && await Service.CreateAsync(obj))
            {
                _logger?.LogInformation($"{obj.GetType().Name} Id {obj.Id} created by {User?.Identity.Name}.");
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    return CreateRedirect;
                }
                return Redirect(redirectUrl);
            }
            _logger?.LogInformation($"Failed to create {obj.GetType().Name} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(objVm);
        }

        public virtual async Task<IActionResult> Edit(TId id)
        {
            var obj = await GetEntityAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(GetEditViewModel(obj));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(TEditVM objVm, string redirectUrl = null)
        {
            var obj = Mapper.Map<TEditVM, T>(objVm);
            if (ModelState.IsValid && await Service.UpdateAsync(obj))
            {
                _logger?.LogInformation($"{obj.GetType().Name} Id {obj.Id} updated by {User?.Identity.Name}.");
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    return EditRedirect;
                }
                return Redirect(redirectUrl);
            }
            _logger?.LogInformation($"Failed to update {obj.GetType().Name} Id {obj.Id} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(objVm);
        }


        public virtual async Task<IActionResult> Delete(TId id)
        {
            var obj = await GetEntityAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(GetDeleteViewModel(obj));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(TId id, string redirectUrl = null)
        {
            if (await Service.DeleteByIdAsync(id))
            {
                _logger?.LogInformation($"{typeof(T).Name} Id {id} removed by {User?.Identity.Name}.");
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    return DeleteRedirect;
                }
                return Redirect(redirectUrl);
            }
            _logger?.LogInformation($"Failed to remove {typeof(T).Name} Id {id} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(GetDeleteViewModel(await Service.GetByIdAsync(id)));
        }

        
        protected CrudControllerBase(ICrudService<T, TId> service, IMapper mapper, ILogger logger = null)
        {
            if (service == null || mapper == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            Service = service;
            Mapper = mapper;
            _logger = logger;
        }

    }
}
