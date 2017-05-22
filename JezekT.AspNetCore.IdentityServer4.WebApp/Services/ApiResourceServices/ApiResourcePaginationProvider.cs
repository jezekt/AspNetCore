using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.ApiResourceServices
{
    public class ApiResourcePaginationProvider : PaginationDataProviderBase<ApiResource, int, ConfigurationDbContext, object>, IPaginationDataProvider<ApiResource, object>
    {
        protected override IPaginationTemplate<ApiResource, object> DefaultPaginationTemplate => new ApiResourcePaginationDefaultTemplate();


        public ApiResourcePaginationProvider(ConfigurationDbContext dbContext, ILogger<ApiResourcePaginationProvider> logger) : base(dbContext, logger)
        {
        }

        public ApiResourcePaginationProvider(IQueryable<ApiResource> baseQuery, ILogger<ApiResourcePaginationProvider> logger) : base(baseQuery, logger)
        {
        }

    }
}
