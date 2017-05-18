using System.Linq;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.RoleServices
{
    public class RolePaginationProvider : PaginationDataProviderBase<IdentityRole, string, IdentityServerDbContext, object>, IPaginationDataProvider<IdentityRole, object>
    {
        protected override IPaginationTemplate<IdentityRole, object> DefaultPaginationTemplate => new RolePaginationDefaultTemplate();


        public RolePaginationProvider(IdentityServerDbContext dbContext, ILogger<RolePaginationProvider> logger) : base(dbContext, logger)
        {
        }

        public RolePaginationProvider(IQueryable<IdentityRole> baseQuery, ILogger<RolePaginationProvider> logger) : base(baseQuery, logger)
        {
        }

    }
}
