using System.ComponentModel.DataAnnotations;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientPostLogoutRedirectUriViewModels
{
    public class ClientPostLogoutRedirectUriViewModel : ClientStringSettingsViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "PostLogoutRedirectUri", ResourceType = typeof(Resources.Models.ClientPostLogoutRedirectUriViewModels.ClientPostLogoutRedirectUriViewModel))]
        public string PostLogoutRedirectUri { get; set; }
        public override string Value => PostLogoutRedirectUri;
    }
}
