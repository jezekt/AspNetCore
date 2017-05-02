using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.UserViewModels
{
    public class UserRoleViewModel
    {
        public string UserId { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Role", ResourceType = typeof(Resources.Models.UserViewModels.UserRoleViewModel))]
        public string RoleId { get; set; }
    }
}
