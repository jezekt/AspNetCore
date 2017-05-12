using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.AccountViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Password", ResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "PasswordConfirmation", ResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        [Compare("Password", ErrorMessageResourceName = "PasswordConfirmationMessage", ErrorMessageResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }
}
