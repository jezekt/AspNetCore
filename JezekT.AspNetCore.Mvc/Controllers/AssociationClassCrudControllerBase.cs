using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AutoMapper;
using JezekT.AspNetCore.Mvc.Extensions;
using JezekT.NetStandard.Data;
using JezekT.NetStandard.Services.EntityOperations;
using Microsoft.AspNetCore.Mvc;

namespace JezekT.AspNetCore.Mvc.Controllers
{
    public abstract class AssociationClassCrudControllerBase<T, TViewModel, FirstId, SecondId> : Controller
        where T : class, IAssociationClass<FirstId, SecondId>
        where TViewModel : class
    {
        protected IAssociationClassCrudService<T, FirstId, SecondId> Service { get; }
        protected IMapper Mapper { get; }


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
            if (ModelState.IsValid && await Service.CreateAsync(Mapper.Map<TViewModel, T>(objVm)))
            {
                return Redirect(redirectUrl);
            }
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
            if (ModelState.IsValid && await Service.UpdateAsync(Mapper.Map<TViewModel, T>(objVm)))
            {
                return Redirect(redirectUrl);
            }
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
                return Redirect(redirectUrl);
            }
            Service.ResolveErrors(ModelState);
            return View(Mapper.Map<T, TViewModel>(await Service.GetByIdsAsync(firstObjId, secondObjId)));
        }


        protected AssociationClassCrudControllerBase(IAssociationClassCrudService<T, FirstId, SecondId> service, IMapper mapper)
        {
            if (service == null || mapper == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            Service = service;
            Mapper = mapper;
        }

    }
}
