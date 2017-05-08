using System.ComponentModel.DataAnnotations;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels
{
    public abstract class ClientStringSettingsViewModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        public abstract string Value { get; }
    }
}
