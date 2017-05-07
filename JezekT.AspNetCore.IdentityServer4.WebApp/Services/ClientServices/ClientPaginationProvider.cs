using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.ClientServices
{
    public class ClientPaginationProvider : PaginationDataProviderBase<Client, int, ConfigurationDbContext, object>, IPaginationDataProvider<Client, object>
    {
        protected override IPaginationTemplate<Client, object> DefaultPaginationTemplate => new ClientPaginationDefaultTemplate();


        public ClientPaginationProvider(ConfigurationDbContext dbContext) : base(dbContext)
        {
        }

        public ClientPaginationProvider(IQueryable<Client> baseQuery) : base(baseQuery)
        {
        }

    }
}
