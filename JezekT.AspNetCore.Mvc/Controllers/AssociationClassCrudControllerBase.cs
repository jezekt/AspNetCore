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
    public abstract class AssociationClassCrudControllerBase<T, TViewModel, FirstId, SecondId> : Controller
        where T : class, IAssociationClass<FirstId, SecondId>
        where TViewModel : class
    {
        protected IAssociationClassCrudService<T, FirstId, SecondId> Service { get; }
        protected IMapper Mapper { get; }
        private readonly ILogger _logger;


        public virtual ActionResult Index()
        {
            return View();
        }


        public virtual async Task<IActionResult> Details(FirstId firstObjId, SecondId secondObjId)
        {
            var obj = await Service.GetByIdsAsync(firstObjId, secondObjId);
            if (obj == null)
            {
                return NotFound();
            }

            return View(Mapper.Map<T, TViewModel>(obj));
        }


        public virtual ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(TViewModel objVm, string redirectUrl = null)
        {
            var obj = Mapper.Map<TViewModel, T>(objVm);
            if (ModelState.IsValid && await Service.CreateAsync(obj))
            {
                _logger?.LogInformation($"{obj.GetType().Name} FirstId {obj.FirstObjId} SecondId {obj.SecondObjId} created by {User?.Identity.Name}.");
                return Redirect(redirectUrl);
            }
            _logger?.LogInformation($"Failed to create {obj.GetType().Name} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(objVm);
        }

        public virtual async Task<IActionResult> Edit(FirstId firstObjId, SecondId secondObjId)
        {
            var obj = await Service.GetByIdsAsync(firstObjId, secondObjId);
            if (obj == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<T, TViewModel>(obj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(TViewModel objVm, string redirectUrl = null)
        {
            var obj = Mapper.Map<TViewModel, T>(objVm);
            if (ModelState.IsValid && await Service.UpdateAsync(obj))
            {
                _logger?.LogInformation($"{obj.GetType().Name} FirstId {obj.FirstObjId} SecondId {obj.SecondObjId} updated by {User?.Identity.Name}.");
                return Redirect(redirectUrl);
            }
            _logger?.LogInformation($"Failed to update {obj.GetType().Name} FirstId {obj.FirstObjId} SecondId {obj.SecondObjId} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(objVm);
        }

        [HttpGet]
        public virtual async Task<IActionResult> Delete(FirstId firstObjId, SecondId secondObjId)
        {
            var obj = await Service.GetByIdsAsync(firstObjId, secondObjId);
            if (obj == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<T, TViewModel>(obj));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(FirstId firstObjId, SecondId secondObjId, string redirectUrl = null)
        {
            if (await Service.DeleteByIdsAsync(firstObjId, secondObjId))
            {
                _logger?.LogInformation($"{typeof(T).Name} FirstId {firstObjId} SecondId {secondObjId} removed by {User?.Identity.Name}.");
                return Redirect(redirectUrl);
            }
            _logger?.LogInformation($"Failed to delete {typeof(T).Name} FirstId {firstObjId} SecondId {secondObjId} by {User?.Identity.Name}.");
            Service.ResolveErrors(ModelState);
            return View(Mapper.Map<T, TViewModel>(await Service.GetByIdsAsync(firstObjId, secondObjId)));
        }


        protected AssociationClassCrudControllerBase(IAssociationClassCrudService<T, FirstId, SecondId> service, IMapper mapper, ILogger logger = null)
        {
            if (service == null || mapper == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            Service = service;
            Mapper = mapper;
            _logger = logger;
        }

    }
}
