using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [EmailAddress]
        public string Email { get; set; }
    }
}
