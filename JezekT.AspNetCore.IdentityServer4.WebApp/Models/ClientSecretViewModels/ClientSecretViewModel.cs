using System;
using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientSecretViewModels
{
    public class ClientSecretViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }

        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Type", ResourceType = typeof(Resources.Models.ClientSecretViewModels.ClientSecretViewModel))]
        public string Type { get; set; } = "SharedSecret";
        [Display(Name = "Description", ResourceType = typeof(Resources.Models.ClientSecretViewModels.ClientSecretViewModel))]
        public string Description { get; set; }
        [Display(Name = "Expiration", ResourceType = typeof(Resources.Models.ClientSecretViewModels.ClientSecretViewModel))]
        public DateTime? Expiration { get; set; }
    }
}
