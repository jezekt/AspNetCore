using System;
using System.Diagnostics.Contracts;
using JezekT.NetStandard.Pagination;

namespace JezekT.AspNetCore.Select2.Data
{
    public class SelectDropdownDataResponse : IPaginationData<object>
    {
        public object[] Items { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public bool MorePages { get; set; }

        public object ResponseData => new { items = Items, more = MorePages };


        public SelectDropdownDataResponse()
        {
        }

        public SelectDropdownDataResponse(IPaginationData<object> paginationData, int start, int pageSize)
        {
            if (paginationData == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            Items = paginationData.Items;
            RecordsTotal = paginationData.RecordsTotal;
            RecordsFiltered = paginationData.RecordsFiltered;
            MorePages = (start + pageSize) < RecordsFiltered;
        }

    }
}
