using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiScopeClaimViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiScopeViewModels
{
    public class ApiScopeViewModel
    {
        public int Id { get; set; }
        public int ApiResourceId { get; set; }

        [Display(Name = "ShowInDiscoveryDocument", ResourceType = typeof(Resources.Models.ApiScopeViewModels.ApiScopeViewModel))]
        public bool ShowInDiscoveryDocument { get; set; }
        [Display(Name = "Name", ResourceType = typeof(Resources.Models.ApiScopeViewModels.ApiScopeViewModel))]
        public string Name { get; set; }
        [Display(Name = "DisplayName", ResourceType = typeof(Resources.Models.ApiScopeViewModels.ApiScopeViewModel))]
        public string DisplayName { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources.Models.ApiScopeViewModels.ApiScopeViewModel))]
        public string Description { get; set; }
        [Display(Name = "Required", ResourceType = typeof(Resources.Models.ApiScopeViewModels.ApiScopeViewModel))]
        public bool Required { get; set; }
        [Display(Name = "Emphasize", ResourceType = typeof(Resources.Models.ApiScopeViewModels.ApiScopeViewModel))]
        public bool Emphasize { get; set; }
        [Display(Name = "UserClaims", ResourceType = typeof(Resources.Models.ApiScopeViewModels.ApiScopeViewModel))]
        public List<ApiScopeClaimViewModel> Claims { get; set; }

        public string ClaimsJson => GetJson(Claims);


        private string GetJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                NullValueHandling = NullValueHandling.Include
            });
        }

    }
}
