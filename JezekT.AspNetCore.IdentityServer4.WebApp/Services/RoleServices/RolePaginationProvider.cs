using System.Linq;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.RoleServices
{
    public class RolePaginationProvider : PaginationDataProviderBase<IdentityRole, string, IdentityServerDbContext, object>, IPaginationDataProvider<IdentityRole, object>
    {
        protected override IPaginationTemplate<IdentityRole, object> DefaultPaginationTemplate => new RolePaginationDefaultTemplate();


        public RolePaginationProvider(IdentityServerDbContext dbContext) : base(dbContext)
        {
        }

        public RolePaginationProvider(IQueryable<IdentityRole> baseQuery) : base(baseQuery)
        {
        }

    }
}
