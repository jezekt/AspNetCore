using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientViewModels
{
    public class ClientViewModel
    {
        public string ClientId { get; set; }
        public string ClientName { get; set; }
        public string AllowedGranTypes { get; set; }
        public List<string> RedirectUris { get; set; }
        public List<string> PostLogoutRedirectUris { get; set; }

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
