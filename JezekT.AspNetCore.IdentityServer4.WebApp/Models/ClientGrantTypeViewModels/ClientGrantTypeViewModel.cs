using System.ComponentModel.DataAnnotations;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientGrantTypeViewModels
{
    public class ClientGrantTypeViewModel : ClientStringSettingsViewModel
    {
        [Display(Name = "GrantType", ResourceType = typeof(Resources.Models.ClientGrantTypeViewModels.ClientGrantTypeViewModel))]
        public string GrantType { get; set; }
        public override string Value => GrantType;
    }
}
