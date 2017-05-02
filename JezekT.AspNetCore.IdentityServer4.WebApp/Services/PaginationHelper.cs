using JezekT.NetStandard.Pagination;

namespace JezekT.AspNetCore.IdentityServer4.WebApp.Services
{
    public class PaginationHelper
    {
        public static object GetDataObject<T>(IPaginationData<T> paginationData, int draw) where T : class
        {
            return new
            {
                draw,
                data = paginationData.Items,
                recordsTotal = paginationData.RecordsTotal,
                recordsFiltered = paginationData.RecordsFiltered
            };
        }
    }
}
