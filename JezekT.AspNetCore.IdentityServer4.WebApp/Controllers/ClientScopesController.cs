using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientScopeViewModels;
using Microsoft.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public class ClientScopesController : ClientSettingsController<ClientScopeViewModel, ClientScope>
    {
        protected override void AddEntityToContext(ClientScopeViewModel vm, ConfigurationDbContext dbContext, Client client)
        {
            Contract.Requires(vm != null && dbContext != null && client != null);

            var clientScope = new ClientScope
            {
                Client = client,
                Scope = vm.Scope
            };
            dbContext.Set<ClientScope>().Add(clientScope);
        }

        protected override void EntityUpdate(ClientScope obj, ClientScopeViewModel vm)
        {
            obj.Scope = vm.Scope;
        }

        protected override async Task<ClientScopeViewModel> GetViewModel(int id, ConfigurationDbContext dbContext)
        {
            Contract.Requires(dbContext != null);

            var clientScope = await dbContext.Set<ClientScope>().Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id);
            if (clientScope != null)
            {
                return new ClientScopeViewModel { Id = clientScope.Id, ClientId = clientScope.Client.Id, Scope = clientScope.Scope };
            }
            return null;
        }


        public ClientScopesController(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

    }
}
