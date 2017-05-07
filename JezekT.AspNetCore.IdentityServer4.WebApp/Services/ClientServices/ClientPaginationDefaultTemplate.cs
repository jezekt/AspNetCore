using System;
using System.Linq.Expressions;
using IdentityServer4.EntityFramework.Entities;
using JezekT.NetStandard.Pagination.DataProviders;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services.ClientServices
{
    public class ClientPaginationDefaultTemplate : IPaginationTemplate<Client, object>
    {
        public Expression<Func<Client, object>> GetSelector()
        {
            return x => new
            {
                x.Id,
                x.ClientId,
                x.ClientName,
                x.Enabled,
            };
        }

        public Expression<Func<Client, bool>> GetSearchTermExpression(string term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return null;
            }
            return x => x.ClientId.Contains(term) || x.ClientName.Contains(term);
        }
    }
}
