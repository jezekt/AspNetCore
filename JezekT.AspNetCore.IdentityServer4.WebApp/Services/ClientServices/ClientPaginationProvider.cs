using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.ClientServices
{
    public class ClientPaginationProvider : PaginationDataProviderBase<Client, int, ConfigurationDbContext, object>, IPaginationDataProvider<Client, object>
    {
        protected override IPaginationTemplate<Client, object> DefaultPaginationTemplate => new ClientPaginationDefaultTemplate();


        public ClientPaginationProvider(ConfigurationDbContext dbContext, ILogger<ClientPaginationProvider> logger) : base(dbContext, logger)
        {
        }

        public ClientPaginationProvider(IQueryable<Client> baseQuery, ILogger<ClientPaginationProvider> logger) : base(baseQuery, logger)
        {
        }

    }
}
