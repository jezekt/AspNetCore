using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientPostLogoutRedirectUriViewModels;
using Microsoft.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public class ClientPostLogoutRedirectUrisController : ClientSettingsController<ClientPostLogoutRedirectUriViewModel, ClientPostLogoutRedirectUri>
    {
        protected override void AddEntityToContext(ClientPostLogoutRedirectUriViewModel vm, ConfigurationDbContext dbContext, Client client)
        {
            Contract.Requires(vm != null && dbContext != null && client != null);

            var clientPostLogoutRedirectUri = new ClientPostLogoutRedirectUri
            {
                Client = client,
                PostLogoutRedirectUri = vm.PostLogoutRedirectUri
            };
            dbContext.Set<ClientPostLogoutRedirectUri>().Add(clientPostLogoutRedirectUri);
        }

        protected override void EntityUpdate(ClientPostLogoutRedirectUri obj, ClientPostLogoutRedirectUriViewModel vm)
        {
            obj.PostLogoutRedirectUri = vm.PostLogoutRedirectUri;
        }

        protected override async Task<ClientPostLogoutRedirectUriViewModel> GetViewModel(int id, ConfigurationDbContext dbContext)
        {
            Contract.Requires(dbContext != null);

            var clientPostLogoutRedirectUri = await dbContext.Set<ClientPostLogoutRedirectUri>().Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id);
            if (clientPostLogoutRedirectUri != null)
            {
                return new ClientPostLogoutRedirectUriViewModel { Id = clientPostLogoutRedirectUri.Id, ClientId = clientPostLogoutRedirectUri.Client.Id, PostLogoutRedirectUri = clientPostLogoutRedirectUri.PostLogoutRedirectUri };
            }
            return null;
        }


        public ClientPostLogoutRedirectUrisController(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

    }
}
