using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientClaimViewModels
{
    public class ClientClaimViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "ClaimType", ResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        public string ClaimType { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "ClaimValue", ResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        public string ClaimValue { get; set; }

        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        public int ClientId { get; set; }

    }
}
