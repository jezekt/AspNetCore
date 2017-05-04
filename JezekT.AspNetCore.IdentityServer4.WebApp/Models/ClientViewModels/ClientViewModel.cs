using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels
{
    public class ClientViewModel
    {
        public int Id { get; set; }
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public List<ValueIdPairViewModel> AllowedScopes { get; set; } = new List<ValueIdPairViewModel>();
        public List<ValueIdPairViewModel> AllowedGrantTypes { get; set; } = new List<ValueIdPairViewModel>();
        public List<ValueIdPairViewModel> RedirectUris { get; set; } = new List<ValueIdPairViewModel>();
        public List<ValueIdPairViewModel> PostLogoutRedirectUris { get; set; } = new List<ValueIdPairViewModel>();
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

    /*
     * new Client
                {
                    ClientId = "mvc",
                    ClientName = "MVC Client",
                    AllowedGrantTypes = GrantTypes.Implicit,

                    RequireConsent = false,

                    //ClientSecrets =
                    //{
                    //    new Secret("secret".Sha256())
                    //},

                    RedirectUris = { "http://localhost:5002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:5002/signout-callback-oidc" },

                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                    },

                    //AllowOfflineAccess = true
                }
     */

}
