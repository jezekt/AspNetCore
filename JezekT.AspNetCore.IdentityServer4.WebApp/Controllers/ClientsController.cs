using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;
using Microsoft.AspNetCore.Mvc;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public class ClientsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public async Task<IActionResult> Details(int id)
        {
            var vm = await GetClientViewModelAsync(id);
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
        public async Task<IActionResult> Create(ClientViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                    
            }
            return View(vm);
        }


        public async Task<IActionResult> Edit(int id)
        {
            var vm = await GetClientViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                
            }
            return View(vm);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vm = await GetClientViewModelAsync(id);
            if (vm == null)
            {
                return NotFound();
            }
            return View(vm);
        }

        public async Task<IActionResult> Delete(ClientViewModel vm)
        {
            if (vm == null)
            {
                return BadRequest();
            }

            return View(vm);
        }

        public ClientsController()
        {
            
        }


        private async Task<ClientViewModel> GetClientViewModelAsync(int id)
        {
            return new ClientViewModel
            {
                ClientId = "mvc",
                ClientName = "MVC Client",
                AllowedScopes = new List<ValueIdPairViewModel>
                {
                    new ValueIdPairViewModel{Id = 1, Value = "openid"},
                    new ValueIdPairViewModel{Id = 2, Value = "profile"}
                },
                AllowedGrantTypes = new List<ValueIdPairViewModel>
                {
                    new ValueIdPairViewModel{Id=1, Value = "implicit"}
                },
                RedirectUris = new List<ValueIdPairViewModel>
                {
                    new ValueIdPairViewModel{Id=1, Value = "http://localhost:5002/signin-oidc"}
                },
                PostLogoutRedirectUris = new List<ValueIdPairViewModel>
                {
                    new ValueIdPairViewModel{Id=1, Value = "http://localhost:5002/signout-callback-oidc"}
                },
                ClientSecrets = new List<ClientSecretViewModel>
                {
                    new ClientSecretViewModel
                    {
                        Id = 1,
                        Description = "Client default secret",
                        Expiration = DateTime.Today.AddMonths(1)
                    }
                }
            };
        }

    }
}
