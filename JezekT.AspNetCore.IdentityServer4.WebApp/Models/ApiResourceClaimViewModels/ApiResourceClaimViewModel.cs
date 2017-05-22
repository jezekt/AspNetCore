using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiResourceClaimViewModels
{
    public class ApiResourceClaimViewModel
    {
        public int Id { get; set; }
        public int ApiResourceId { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Type", ResourceType = typeof(Resources.Models.ApiResourceClaimViewModels.ApiResourceClaimViewModel))]
        public string Type { get; set; }
    }
}
