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
    public abstract class CrudControllerBase<T, TViewModel, TId> : Controller
        where T : class,  IWithId<TId>
        where TViewModel : class
    {
        protected ICrudService<T, TId> Service { get; }
        protected IMapper Mapper { get; }
        private readonly ILogger _logger;


        protected virtual async Task<T> GetEntity(TId id)
        {
            return await Service.GetByIdAsync(id);
        }

        protected virtual TViewModel GetViewModel(T obj)
        {
            return Mapper.Map<T, TViewModel>(obj);
        }


        public virtual ActionResult Index()
        {
            return View();
        }


        public virtual async Task<IActionResult> Details(TId id)
        {
            var obj = await GetEntity(id);
            if (obj == null)
            {
                return NotFound();
            }

            return View(GetViewModel(obj));
        }


        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TViewModel objVm)
        {
            var obj = Mapper.Map<TViewModel, T>(objVm);
            if (ModelState.IsValid && await Service.CreateAsync(obj))
            {
                _logger?.LogInformation($"{obj.GetType().Name} Id {obj.Id} created by {User?.Identity.Name}.");
                return RedirectToAction("Index");
            }
            _logger?.LogInformation($"Failed to create {obj.GetType().Name} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(objVm);
        }

        public virtual async Task<IActionResult> Edit(TId id)
        {
            var obj = await GetEntity(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(GetViewModel(obj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(TViewModel objVm)
        {
            var obj = Mapper.Map<TViewModel, T>(objVm);
            if (ModelState.IsValid && await Service.UpdateAsync(obj))
            {
                _logger?.LogInformation($"{obj.GetType().Name} Id {obj.Id} updated by {User?.Identity.Name}.");
                return RedirectToAction("Index");
            }
            _logger?.LogInformation($"Failed to update {obj.GetType().Name} Id {obj.Id} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(objVm);
        }


        public virtual async Task<IActionResult> Delete(TId id)
        {
            var obj = await GetEntity(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(GetViewModel(obj));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(TId id)
        {
            if (await Service.DeleteByIdAsync(id))
            {
                _logger?.LogInformation($"{typeof(T).Name} Id {id} removed by {User?.Identity.Name}.");
                return RedirectToAction("Index");
            }
            _logger?.LogInformation($"Failed to remove {typeof(T).Name} Id {id} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(GetViewModel(await Service.GetByIdAsync(id)));
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
