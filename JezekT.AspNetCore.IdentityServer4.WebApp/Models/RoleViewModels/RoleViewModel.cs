using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.RoleViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Name", ResourceType = typeof(Resources.Models.RoleViewModels.RoleViewModel))]
        public string Name { get; set; }
    }
}
