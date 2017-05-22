using System;
using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiSecretViewModels
{
    public class ApiSecretViewModel
    {
        public int Id { get; set; }
        public int ApiResourceId { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Type", ResourceType = typeof(Resources.Models.ApiSecretViewModels.ApiSecretViewModel))]
        public string Type { get; set; } = "SharedSecret";
        [Display(Name = "Description", ResourceType = typeof(Resources.Models.ApiSecretViewModels.ApiSecretViewModel))]
        public string Description { get; set; }
        [Display(Name = "Expiration", ResourceType = typeof(Resources.Models.ApiSecretViewModels.ApiSecretViewModel))]
        public DateTime? Expiration { get; set; }

    }
}
