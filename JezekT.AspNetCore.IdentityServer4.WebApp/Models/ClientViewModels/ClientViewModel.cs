using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientGrantTypeViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientPostLogoutRedirectUriViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientRedirectUriViewModels;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientScopeViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Enabled", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public bool Enabled { get; set; }
        [Required(ErrorMessageResourceName = "RequiredErrorMessageX", ErrorMessageResourceType = typeof(Resources.Models.Shared.ViewModelsShared))]
        [Display(Name = "ClientId", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public string ClientId { get; set; }
        [Display(Name = "ClientName", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public string ClientName { get; set; }
        [Display(Name = "AllowOfflineAccess", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public bool AllowOfflineAccess { get; set; }
        [Display(Name = "AlwaysSendClientClaims", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public bool AlwaysSendClientClaims { get; set; }
        [Display(Name = "IdentityTokenLifetime", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public int? IdentityTokenLifetime { get; set; }
        [Display(Name = "AccessTokenLifetime", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public int? AccessTokenLifetime { get; set; }

        [Display(Name = "AllowedScopes", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public List<ClientScopeViewModel> AllowedScopes { get; set; } = new List<ClientScopeViewModel>();
        [Display(Name = "AllowedGrantTypes", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public List<ClientGrantTypeViewModel> AllowedGrantTypes { get; set; } = new List<ClientGrantTypeViewModel>();
        [Display(Name = "RedirectUris", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public List<ClientRedirectUriViewModel> RedirectUris { get; set; } = new List<ClientRedirectUriViewModel>();
        [Display(Name = "PostLogoutRedirectUris", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public List<ClientPostLogoutRedirectUriViewModel> PostLogoutRedirectUris { get; set; } = new List<ClientPostLogoutRedirectUriViewModel>();
        [Display(Name = "ClientSecrets", ResourceType = typeof(Resources.Models.ClientViewModels.ClientViewModel))]
        public List<ClientSecretViewModel> ClientSecrets { get; set; } = new List<ClientSecretViewModel>();

        public string AllowedScopesJson => GetJson(AllowedScopes);
        public string AllowedGrantTypesJson => GetJson(AllowedGrantTypes);
        public string RedirectUrisJson => GetJson(RedirectUris);
        public string PostLogoutRedirectUrisJson => GetJson(PostLogoutRedirectUris);
        public string ClientSecretsJson => GetJson(ClientSecrets);


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
