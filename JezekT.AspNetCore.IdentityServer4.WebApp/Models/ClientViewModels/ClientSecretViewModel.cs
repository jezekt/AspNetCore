using System;
using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels
{
    public class ClientSecretViewModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Type", ResourceType = typeof(Resources.Models.ClientViewModels.ClientSecretViewModel))]
        public string Type { get; set; } = "SharedSecret";
        [Display(Name = "Description", ResourceType = typeof(Resources.Models.ClientViewModels.ClientSecretViewModel))]
        public string Description { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [DataType(DataType.Password)]
        [Display(Name = "Value", ResourceType = typeof(Resources.Models.ClientViewModels.ClientSecretViewModel))]
        public string Value { get; set; }
        [Display(Name = "Expiration", ResourceType = typeof(Resources.Models.ClientViewModels.ClientSecretViewModel))]
        public DateTime? Expiration { get; set; }
    }
}
