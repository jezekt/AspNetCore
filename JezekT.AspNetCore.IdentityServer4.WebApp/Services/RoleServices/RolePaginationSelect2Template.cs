using System;
using System.Linq.Expressions;
using JezekT.NetStandard.Pagination.DataProviders;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.RoleServices
{
    public class RolePaginationSelect2Template : IPaginationTemplate<IdentityRole, object>
    {
        public Expression<Func<IdentityRole, object>> GetSelector()
        {
            return x => new { id = x.Id, text = x.Name };
        }

        public Expression<Func<IdentityRole, bool>> GetSearchTermExpression(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }
            return x => x.Name.Contains(term);
        }
    }
}
