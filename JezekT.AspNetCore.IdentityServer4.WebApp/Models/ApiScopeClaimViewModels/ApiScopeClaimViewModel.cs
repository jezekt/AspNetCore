using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiScopeClaimViewModels
{
    public class ApiScopeClaimViewModel
    {
        public int Id { get; set; }
        public int ApiScopeId { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Type", ResourceType = typeof(Resources.Models.ApiResourceClaimViewModels.ApiResourceClaimViewModel))]
        public string Type { get; set; }
    }
}
