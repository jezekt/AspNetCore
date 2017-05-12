using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ConsentViewModels;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.ConsentServices
{
    public class ProcessConsentResult
    {
        public bool IsRedirect => RedirectUri != null;
        public string RedirectUri { get; set; }

        public bool HasViewModel => ViewModel != null;
        public ConsentViewModel ViewModel { get; set; }

        public bool HasValidationError => ValidationError != null;
        public string ValidationError { get; set; }
    }
}
