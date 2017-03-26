﻿using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AutoMapper;
using JezekT.AspNetCore.Mvc.Extensions;
using JezekT.NetStandard.Services;
using Microsoft.AspNetCore.Mvc;

namespace JezekT.AspNetCore.Mvc.Controllers
{
    public abstract class CrudControllerBase<T, TViewModel, TId> : Controller
        where T : class
        where TViewModel : class
    {
        protected ICrudService<T, TId> Service { get; }
        protected IMapper Mapper { get; }


        public virtual ActionResult Index()
        {
            return View();
        }


        public virtual async Task<IActionResult> Details(TId id)
        {
            var obj = await Service.GetByIdAsync(id);
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
        public virtual async Task<IActionResult> Create(TViewModel objVm)
        {
            if (ModelState.IsValid && await Service.CreateAsync(Mapper.Map<TViewModel, T>(objVm)))
            {
                return RedirectToAction("Index");
            }
            Service.ResolveErrors(ModelState, ViewData);
            return View(objVm);
        }

        public virtual async Task<IActionResult> Edit(TId id)
        {
            var obj = await Service.GetByIdAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<T, TViewModel>(obj));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(TViewModel objVm)
        {
            if (ModelState.IsValid && await Service.UpdateAsync(Mapper.Map<TViewModel, T>(objVm)))
            {
                return RedirectToAction("Index");
            }
            Service.ResolveErrors(ModelState, ViewData);
            return View(objVm);
        }


        public virtual async Task<IActionResult> Delete(TId id)
        {
            var obj = await Service.GetByIdAsync(id);
            if (obj == null)
            {
                return NotFound();
            }
            return View(Mapper.Map<T, TViewModel>(obj));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> DeleteConfirmed(TId id)
        {
            if (await Service.DeleteByIdAsync(id))
            {
                return RedirectToAction("Index");
            }
            Service.ResolveErrors(ModelState, ViewData);
            return View(Mapper.Map<T, TViewModel>(await Service.GetByIdAsync(id)));
        }


        protected CrudControllerBase(ICrudService<T, TId> service, IMapper mapper)
        {
            if (service == null || mapper == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            Service = service;
            Mapper = mapper;
        }

    }
}