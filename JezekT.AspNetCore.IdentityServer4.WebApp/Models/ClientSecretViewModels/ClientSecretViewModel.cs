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
        public string Type { get; set; } = "SharedResource";
        [Display(Name = "Description", ResourceType = typeof(Resources.Models.ClientSecretViewModels.ClientSecretViewModel))]
        public string Description { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [DataType(DataType.Password)]
        [Display(Name = "Password", ResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        public string Value { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "PasswordConfirmation", ResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        [Compare("Value", ErrorMessageResourceName = "PasswordConfirmationMessage", ErrorMessageResourceType = typeof(Resources.Models.UserViewModels.UserViewModel))]
        public string PasswordConfirmation { get; set; }
        [Display(Name = "Expiration", ResourceType = typeof(Resources.Models.ClientSecretViewModels.ClientSecretViewModel))]
        public DateTime? Expiration { get; set; }
    }
}
