using System.Linq;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserServices
{
    public class UserPaginationProvider : PaginationDataProviderBase<User, string, IdentityServerDbContext, object>, IPaginationDataProvider<User, object>
    {
        protected override IPaginationTemplate<User, object> DefaultPaginationTemplate => new UserPaginationDefaultTemplate();


        public UserPaginationProvider(IdentityServerDbContext dbContext) : base(dbContext)
        {
        }

        public UserPaginationProvider(IQueryable<User> baseQuery) : base(baseQuery)
        {
        }

    }
}
