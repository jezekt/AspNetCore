using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.IdentityClaimViewModels
{
    public class IdentityClaimViewModel
    {
        public int Id { get; set; }
        public int IdentityResourceId { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Type", ResourceType = typeof(Resources.Models.IdentityClaimViewModels.IdentityClaimViewModel))]
        public string Type { get; set; }
    }
}
