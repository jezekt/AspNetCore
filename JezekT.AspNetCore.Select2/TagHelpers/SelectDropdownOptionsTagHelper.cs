using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.Select2.TagHelpers
{
    [HtmlTargetElement("select-dropdown-options", ParentTag = "select-dropdown")]
    [RestrictChildren("select-dropdown-option")]
    public class SelectDropdownOptionsTagHelper : TagHelper
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
