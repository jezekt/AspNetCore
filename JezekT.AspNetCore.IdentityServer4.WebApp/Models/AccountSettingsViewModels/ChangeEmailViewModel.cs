using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountSettingsViewModels
{
    public class ChangeEmailViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [EmailAddress(ErrorMessage = "EmailValidationErrorMessage")]
        [Display(Name = "NewEmail", ResourceType = typeof(Resources.Models.AccountSettingsViewModels.ChangeEmailViewModel))]
        public string NewEmail { get; set; }

    }
}
