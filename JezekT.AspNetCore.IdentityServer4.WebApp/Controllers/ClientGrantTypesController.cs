using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.AspNetCore.IdentityServer4.WebApp.Models.ClientGrantTypeViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Controllers
{
    public class ClientGrantTypesController : ClientSettingsController<ClientGrantTypeViewModel, ClientGrantType>
    {
        protected override void AddEntityToContext(ClientGrantTypeViewModel vm, ConfigurationDbContext dbContext, Client client)
        {
            Contract.Requires(vm != null && dbContext != null && client != null);

            var clientGrantType = new ClientGrantType
            {
                Client = client,
                GrantType = vm.GrantType
            };
            dbContext.Set<ClientGrantType>().Add(clientGrantType);
        }

        protected override void EntityUpdate(ClientGrantType obj, ClientGrantTypeViewModel vm)
        {
            obj.GrantType = vm.GrantType;
        }

        protected override async Task<ClientGrantTypeViewModel> GetViewModel(int id, ConfigurationDbContext dbContext)
        {
            Contract.Requires(dbContext != null);

            var clientGrantType = await dbContext.Set<ClientGrantType>().Include(x => x.Client).FirstOrDefaultAsync(x => x.Id == id);
            if (clientGrantType != null)
            {
                return new ClientGrantTypeViewModel { Id = clientGrantType.Id, ClientId = clientGrantType.Client.Id, GrantType = clientGrantType.GrantType };
            }
            return null;
        }


        public ClientGrantTypesController(ConfigurationDbContext dbContext, ILogger<ClientGrantTypesController> logger) : base(dbContext, logger)
        {
        }

    }
}
