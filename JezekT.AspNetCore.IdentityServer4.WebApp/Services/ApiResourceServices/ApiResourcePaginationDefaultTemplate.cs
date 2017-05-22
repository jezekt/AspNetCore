using System;
using System.Linq.Expressions;
using IdentityServer4.EntityFramework.Entities;
using JezekT.NetStandard.Pagination.DataProviders;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.ApiResourceServices
{
    public class ApiResourcePaginationDefaultTemplate : IPaginationTemplate<ApiResource, object>
    {
        public Expression<Func<ApiResource, object>> GetSelector()
        {
            return x => new
            {
                x.Id,
                x.Enabled,
                x.Name,
                x.DisplayName,
                x.Description
            };
        }

        public Expression<Func<ApiResource, bool>> GetSearchTermExpression(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }
            return x => x.Name.Contains(term) || x.DisplayName.Contains(term);
        }
    }
}
