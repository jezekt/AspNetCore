using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.UserViewModels
{
    public class CreateUserViewModel : UserViewModel
    {
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "PasswordConfirmation", ResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        [Compare("Password", ErrorMessageResourceName = "PasswordConfirmationMessage", ErrorMessageResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        public string PasswordConfirmation { get; set; }
    }
}
