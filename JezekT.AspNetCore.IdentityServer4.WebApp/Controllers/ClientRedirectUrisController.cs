using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientRedirectUriViewModels;
using Microsoft.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public class ClientRedirectUrisController : ClientSettingsController<ClientRedirectUriViewModel, ClientRedirectUri>
    {
        protected override void AddEntityToContext(ClientRedirectUriViewModel vm, ConfigurationDbContext dbContext, Client client)
        {
            Contract.Requires(vm != null && dbContext != null && client != null);

            var clientRedirectUri = new ClientRedirectUri
            {
                Client = client,
                RedirectUri = vm.RedirectUri
            };
            dbContext.Set<ClientRedirectUri>().Add(clientRedirectUri);
        }

        protected override void EntityUpdate(ClientRedirectUri obj, ClientRedirectUriViewModel vm)
        {
            obj.RedirectUri = vm.RedirectUri;
        }

        protected override async Task<ClientRedirectUriViewModel> GetViewModel(int id, ConfigurationDbContext dbContext)
        {
            Contract.Requires(dbContext != null);

            var clientRedirectUri = await dbContext.Set<ClientRedirectUri>().Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id);
            if (clientRedirectUri != null)
            {
                return new ClientRedirectUriViewModel { Id = clientRedirectUri.Id, ClientId = clientRedirectUri.Client.Id, RedirectUri = clientRedirectUri.RedirectUri };
            }
            return null;
        }


        public ClientRedirectUrisController(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

    }
}
