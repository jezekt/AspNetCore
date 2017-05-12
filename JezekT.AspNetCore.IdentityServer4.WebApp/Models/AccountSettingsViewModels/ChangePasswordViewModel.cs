using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountSettingsViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [DataType(DataType.Password)]
        [Display(Name = "CurrentPassword", ResourceType = typeof(Resources.Models.AccountSettingsViewModels.ChangePasswordViewModel))]
        public string OldPassword { get; set; }

        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [StringLength(100, ErrorMessage = "PasswordValidationErrorMessage", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "NewPassword", ResourceType = typeof(Resources.Models.AccountSettingsViewModels.ChangePasswordViewModel))]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Models.AccountSettingsViewModels.ChangePasswordViewModel))]
        [Compare("NewPassword", ErrorMessage = "PasswordsNotMatch")]
        public string ConfirmPassword { get; set; }
    }
}
