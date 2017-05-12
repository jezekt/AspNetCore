using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ConsentViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Services.ConsentServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public class ConsentController : Controller
    {
        private readonly ConsentService _consent;


        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await _consent.BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return View("Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ConsentInputModel model)
        {
            var result = await _consent.ProcessConsent(model);
            if (result.IsRedirect)
            {
                return Redirect(result.RedirectUri);
            }
            if (result.HasValidationError)
            {
                ModelState.AddModelError("", result.ValidationError);
            }
            if (result.HasViewModel)
            {
                return View("Index", result.ViewModel);
            }
            return View("Error");
        }


        public ConsentController(IIdentityServerInteractionService interaction, IClientStore clientStore, IResourceStore resourceStore, ILogger<ConsentController> logger)
        {
            _consent = new ConsentService(interaction, clientStore, resourceStore, logger);
        }

    }
}