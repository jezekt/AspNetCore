using System;
using System.Linq.Expressions;
using JezekT.NetStandard.Pagination.DataProviders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.UserClaimServices
{
    public class UserClaimPaginationDefaultTemplate : IPaginationTemplate<IdentityUserClaim<string>, object>
    {
        public Expression<Func<IdentityUserClaim<string>, object>> GetSelector()
        {
            return x => new
            {
                x.Id,
                x.ClaimType,
                x.ClaimValue
            };
        }

        public Expression<Func<IdentityUserClaim<string>, bool>> GetSearchTermExpression(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }
            return x => x.ClaimType.Contains(term) || x.ClaimValue.Contains(term);
        }
    }
}
