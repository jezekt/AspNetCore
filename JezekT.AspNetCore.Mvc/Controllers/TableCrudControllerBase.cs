using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AutoMapper;
using JezekT.AspNetCore.DataTables.Data;
using JezekT.NetStandard.Data;
using JezekT.NetStandard.Services.EntityOperations;
using JezekT.NetStandard.Services.Pagination;
using Microsoft.AspNetCore.Mvc;

namespace JezekT.AspNetCore.Mvc.Controllers
{
    public abstract class TableCrudControllerBase<T, TViewModel, TId, TPaginationItem> : CrudControllerBase<T, TViewModel, TId>
        where T : class, IWithId<TId>
        where TViewModel : class
        where TPaginationItem : class 
    {
        protected IPaginationService<T, TId, TPaginationItem> PaginationService { get; }

        
        public virtual async Task<IActionResult> GetTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection)
        {
            var paginationData = await PaginationService.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection);
            return Json(new DataTableDataResponse<TPaginationItem>(paginationData, draw).ResponseData);
        }


        protected TableCrudControllerBase(IPaginationService<T, TId, TPaginationItem> paginationService, ICrudService<T, TId> crudService, IMapper mapper) : base(crudService, mapper)
        {
            if (paginationService == null || mapper == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            PaginationService = paginationService;
        }
    }
}
