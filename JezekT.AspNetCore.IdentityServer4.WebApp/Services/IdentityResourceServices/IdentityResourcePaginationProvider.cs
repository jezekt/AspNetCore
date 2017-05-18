using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.IdentityResourceServices
{
    public class IdentityResourcePaginationProvider : PaginationDataProviderBase<IdentityResource, int, ConfigurationDbContext, object>, IPaginationDataProvider<IdentityResource, object>
    {
        protected override IPaginationTemplate<IdentityResource, object> DefaultPaginationTemplate => new IdentityResourcePaginationDefaultTemplate();


        public IdentityResourcePaginationProvider(ConfigurationDbContext dbContext, ILogger<IdentityResourcePaginationProvider> logger) : base(dbContext, logger)
        {
        }

        public IdentityResourcePaginationProvider(IQueryable<IdentityResource> baseQuery, ILogger<IdentityResourcePaginationProvider> logger) : base(baseQuery, logger)
        {
        }

    }
}
