using System;
using System.Diagnostics.Contracts;
using JezekT.NetStandard.Pagination;

namespace JezekT.AspNetCore.DataTables.Data
{
    public class DataTableDataResponse : IPaginationData
    {
        public int Draw { get; set; }
        public object[] Items { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }

        public object ResponseData => new {draw = Draw, data = Items, recordsTotal = RecordsTotal, recordsFiltered = RecordsFiltered};


        public DataTableDataResponse()
        {
        }

        public DataTableDataResponse(IPaginationData paginationData, int draw)
        {
            if (paginationData == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            Draw = draw;
            Items = paginationData.Items;
            RecordsTotal = paginationData.RecordsTotal;
            RecordsFiltered = paginationData.RecordsFiltered;
        }
    }
}
