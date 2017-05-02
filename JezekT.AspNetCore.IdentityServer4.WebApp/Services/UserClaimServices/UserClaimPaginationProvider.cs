using System.Linq;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserClaimServices
{
    public class UserClaimPaginationProvider : PaginationDataProviderBase<IdentityUserClaim<string>, int, IdentityServerDbContext, object>, IPaginationDataProvider<IdentityUserClaim<string>, object>
    {
        protected override IPaginationTemplate<IdentityUserClaim<string>, object> DefaultPaginationTemplate => new UserClaimPaginationDefaultTemplate();


        public UserClaimPaginationProvider(IdentityServerDbContext dbContext) : base(dbContext)
        {
        }

        public UserClaimPaginationProvider(IQueryable<IdentityUserClaim<string>> baseQuery) : base(baseQuery)
        {
        }

    }
}
