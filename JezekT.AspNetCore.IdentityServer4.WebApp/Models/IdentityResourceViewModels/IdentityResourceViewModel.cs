using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.IdentityClaimViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.IdentityResourceViewModels
{
    public class IdentityResourceViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Enabled", ResourceType = typeof(Resources.Models.IdentityResourceViewModels.IdentityResourceViewModel))]
        public bool Enabled { get; set; } = true;
        [Display(Name = "ShowInDiscoveryDocument", ResourceType = typeof(Resources.Models.IdentityResourceViewModels.IdentityResourceViewModel))]
        public bool ShowInDiscoveryDocument { get; set; } = true;
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Name", ResourceType = typeof(Resources.Models.IdentityResourceViewModels.IdentityResourceViewModel))]
        public string Name { get; set; }
        [Display(Name = "DisplayName", ResourceType = typeof(Resources.Models.IdentityResourceViewModels.IdentityResourceViewModel))]
        public string DisplayName { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources.Models.IdentityResourceViewModels.IdentityResourceViewModel))]
        public string Description { get; set; }
        [Display(Name = "Required", ResourceType = typeof(Resources.Models.IdentityResourceViewModels.IdentityResourceViewModel))]
        public bool Required { get; set; }
        [Display(Name = "Emphasize", ResourceType = typeof(Resources.Models.IdentityResourceViewModels.IdentityResourceViewModel))]
        public bool Emphasize { get; set; }
        [Display(Name = "UserClaims", ResourceType = typeof(Resources.Models.IdentityResourceViewModels.IdentityResourceViewModel))]
        public List<IdentityClaimViewModel> UserClaims { get; set; }

        public string UserClaimsJson => GetJson(UserClaims);


        private string GetJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Ignore
            });
        }

    }
}
