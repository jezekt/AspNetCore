using System.Linq;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.NetStandard.Pagination.DataProviders;
using JezekT.NetStandard.Pagination.EntityFrameworkCore.DataProviders;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserServices
{
    public class UserPaginationProvider : PaginationDataProviderBase<User, string, IdentityServerDbContext, object>, IPaginationDataProvider<User, object>
    {
        protected override IPaginationTemplate<User, object> DefaultPaginationTemplate => new UserPaginationDefaultTemplate();


        public UserPaginationProvider(IdentityServerDbContext dbContext, ILogger<UserPaginationProvider> logger) : base(dbContext, logger)
        {
        }

        public UserPaginationProvider(IQueryable<User> baseQuery, ILogger<UserPaginationProvider> logger) : base(baseQuery, logger)
        {
        }

    }
}
