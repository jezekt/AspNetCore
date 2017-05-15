using System;
using System.Diagnostics.Contracts;
using IdentityServer4.Models;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.HomeViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;


        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error(string error)
        {
            var exception = HttpContext.Features.Get<IExceptionHandlerFeature>();
            var vm = new ErrorViewModel();
            var logMessage = "Error";
            if (exception != null)
            {
                vm.Error = new ErrorMessage
                {
                    Error = HttpContext.Response.StatusCode.ToString(),
                    ErrorDescription = exception.Error.Message
                };

                var logError = new
                {
                    StatusCode = HttpContext.Response.StatusCode.ToString(),
                    ErrorMessage = exception.Error.GetBaseException()?.Message ??
                                   exception.Error.Message,
                    exception.Error.StackTrace
                };
                logMessage = JsonConvert.SerializeObject(logError);
            }
            _logger.LogError(logMessage);
            return View("Error", vm);
        }


        public HomeController(ILogger<HomeController> logger)
        {
            if (logger == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _logger = logger;
        }
    }
}
