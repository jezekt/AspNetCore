using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.UserViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "UserName", ResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        public string Username { get; set; }
        [EmailAddress]
        [Display(Name = "Email", ResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        public string Email { get; set; }

        public string UserClaimIds { get; set; }
        public string RoleIds { get; set; }
        
    }
}
