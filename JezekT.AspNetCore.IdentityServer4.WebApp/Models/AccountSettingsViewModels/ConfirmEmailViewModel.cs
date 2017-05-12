using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountSettingsViewModels
{
    public class ConfirmEmailViewModel
    {
        [Display(Name = "Email", ResourceType = typeof(Resources.Models.AccountSettingsViewModels.ConfirmEmailViewModel))]
        public string Email { get; set; }

    }
}
