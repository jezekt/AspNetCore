using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using AutoMapper;
using JezekT.NetStandard.Data;
using JezekT.NetStandard.Pagination;
using JezekT.NetStandard.Services.EntityOperations;
using JezekT.NetStandard.Services.Pagination;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace JezekT.AspNetCore.Mvc.Controllers
{
    public abstract class TableCrudControllerBase<T, TCreateVM, TDetailsVM, TEditVM, TDeleteVM, TId, TPaginationItem> : CrudControllerBase<T, TCreateVM, TDetailsVM, TEditVM, TDeleteVM, TId>
        where T : class, IWithId<TId>
        where TCreateVM : class
        where TDetailsVM : class
        where TEditVM : class
        where TDeleteVM : class
        where TPaginationItem : class
    {
        protected IPaginationService<T, TId, TPaginationItem> PaginationService { get; }
        protected Func<IPaginationData<TPaginationItem>, int, object> PaginationJsonObjectExpression => (data, draw) => new
        {
            draw,
            data = data.Items,
            recordsTotal = data.RecordsTotal,
            recordsFiltered = data.RecordsFiltered
        };

        public virtual async Task<IActionResult> GetTableDataJsonAsync(int draw, string term, int start, int pageSize, string orderField, string orderDirection)
        {
            var paginationData = await PaginationService.GetPaginationDataAsync(start, pageSize, term, orderField, orderDirection);
            return Json(PaginationJsonObjectExpression.Invoke(paginationData, draw));
        }


        protected TableCrudControllerBase(IPaginationService<T, TId, TPaginationItem> paginationService, ICrudService<T, TId> crudService, IMapper mapper, ILogger logger = null)
            : base(crudService, mapper, logger)
        {
            if (paginationService == null || mapper == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            PaginationService = paginationService;
        }
    }
}
