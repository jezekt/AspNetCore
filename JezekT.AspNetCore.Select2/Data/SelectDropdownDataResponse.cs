namespace JezekT.AspNetCore.Select2.Data
{
    public class SelectDropdownDataResponse
    {
        public object[] Items { get; set; }
        public int RecordsTotal { get; set; }
        public int RecordsFiltered { get; set; }
        public bool MorePages { get; set; }

        public object ResponseData => new { items = Items, more = MorePages };

        public SelectDropdownDataResponse(object[] items, int recordsTotal, int recordsFiltered, int start, int pageSize)
        {
            Items = items;
            RecordsTotal = recordsTotal;
            RecordsFiltered = recordsFiltered;
            MorePages = (start + pageSize) < RecordsFiltered;
        }

    }
}
