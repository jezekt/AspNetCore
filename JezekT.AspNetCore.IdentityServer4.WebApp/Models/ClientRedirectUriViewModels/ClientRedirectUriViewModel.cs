using System.ComponentModel.DataAnnotations;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientRedirectUriViewModels
{
    public class ClientRedirectUriViewModel : ClientStringSettingsViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "RedirectUri", ResourceType = typeof(Resources.Models.ClientRedirectUriViewModels.ClientRedirectUriViewModel))]
        public string RedirectUri { get; set; }
        public override string Value => RedirectUri;
    }
}
