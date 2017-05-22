using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiResourceClaimViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiScopeViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiSecretViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ApiResourceViewModels
{
    public class ApiResourceViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Enabled", ResourceType = typeof(Resources.Models.ApiResourceViewModels.ApiResourceViewModel))]
        public bool Enabled { get; set; } = true;
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "Name", ResourceType = typeof(Resources.Models.ApiResourceViewModels.ApiResourceViewModel))]
        public string Name { get; set; }
        [Display(Name = "DisplayName", ResourceType = typeof(Resources.Models.ApiResourceViewModels.ApiResourceViewModel))]
        public string DisplayName { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources.Models.ApiResourceViewModels.ApiResourceViewModel))]
        public string Description { get; set; }
        [Display(Name = "Secrets", ResourceType = typeof(Resources.Models.ApiResourceViewModels.ApiResourceViewModel))]
        public List<ApiSecretViewModel> Secrets { get; set; }
        [Display(Name = "Scopes", ResourceType = typeof(Resources.Models.ApiResourceViewModels.ApiResourceViewModel))]
        public List<ApiScopeViewModel> Scopes { get; set; }
        [Display(Name = "UserClaims", ResourceType = typeof(Resources.Models.ApiResourceViewModels.ApiResourceViewModel))]
        public List<ApiResourceClaimViewModel> Claims { get; set; }

        public string SecretsJson => GetJson(Secrets);
        public string ScopesJson => GetJson(Scopes);
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
