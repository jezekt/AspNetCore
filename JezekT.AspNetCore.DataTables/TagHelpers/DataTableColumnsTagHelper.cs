using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.DataTables.TagHelpers
{
    [HtmlTargetElement("data-table-columns", ParentTag = "data-table")]
    [RestrictChildren("data-table-column")]
    public class DataTableColumnsTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            await output.GetChildContentAsync();
            output.SuppressOutput();
        }
    }
}
