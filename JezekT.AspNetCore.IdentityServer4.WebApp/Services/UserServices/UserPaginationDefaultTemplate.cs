using System;
using System.Linq.Expressions;
using JezekT.AspNetCore.IdentityServer4.WebApp.Data;
using JezekT.NetStandard.Pagination.DataProviders;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserServices
{
    public class UserPaginationDefaultTemplate : IPaginationTemplate<User, object>
    {
        public Expression<Func<User, object>> GetSelector()
        {
            return x => new
            {
                x.Id,
                username = x.UserName
            };
        }

        public Expression<Func<User, bool>> GetSearchTermExpression(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }
            return x => x.UserName.Contains(term);
        }
    }
}
