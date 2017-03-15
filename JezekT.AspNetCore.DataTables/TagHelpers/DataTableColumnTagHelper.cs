using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.DataTables.TagHelpers
{
    [HtmlTargetElement("data-table-column", ParentTag = "data-table-columns")]
    [RestrictChildren("data-table-column-action")]
    public class DataTableColumnTagHelper : TagHelper
    {
        private const string ColumnPropertyName = "asp-for";
        private const string PropertyNameName = "property-name";

        [HtmlAttributeName(ColumnPropertyName)]
        public ModelExpression ColumnProperty { get; set; }
        [HtmlAttributeName(PropertyNameName)]
        public string PropertyName { get; set; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            await output.GetChildContentAsync();

            var dataTableContext = (DataTableContext)context.Items[typeof(DataTableTagHelper)];
            if (dataTableContext == null)
            {
                throw new ArgumentNullException($"{nameof(dataTableContext)} nebyl nalezen.");
            }

            if (dataTableContext.ColumnsProperties == null)
            {
                dataTableContext.ColumnsProperties = new List<ModelExpression>();
            }

            if (ColumnProperty != null)
            {
                dataTableContext.ColumnsProperties.Add(ColumnProperty);
            }
            else
            {
                if (!string.IsNullOrEmpty(PropertyName))
                {
                    if (dataTableContext.ModelExplorer == null)
                    {
                        throw new ArgumentNullException($"V případě použití {PropertyNameName} musí být naplněna instance ModelExploreru v {nameof(DataTableContext)}.");
                    }
                    var explorer = dataTableContext.ModelExplorer.GetExplorerForProperty(PropertyName);
                    dataTableContext.ColumnsProperties.Add(new ModelExpression(PropertyName, explorer));
                }
            }
            output.SuppressOutput();
        }
    }
}

