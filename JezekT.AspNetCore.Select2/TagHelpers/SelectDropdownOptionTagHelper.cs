using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.Select2.TagHelpers
{
    [HtmlTargetElement("select-dropdown-option", ParentTag = "select-dropdown-options")]
    public class SelectDropdownOptionTagHelper : TagHelper
    {
        private const string ValueName = "value";
        private const string TextName = "text";

        [HtmlAttributeName(ValueName)]
        public string Value { get; set; }
        [HtmlAttributeName(TextName)]
        public string Text { get; set; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            await output.GetChildContentAsync();

            var dataTableContext = (SelectDropdownContext)context.Items[typeof(SelectDropdownTagHelper)];
            if (dataTableContext == null)
            {
                throw new ArgumentNullException(nameof(dataTableContext));
            }

            if (dataTableContext.OptionDataSet == null)
            {
                dataTableContext.OptionDataSet = new List<OptionData>();
            }

            if (!string.IsNullOrEmpty(Value))
            {
                dataTableContext.OptionDataSet.Add(new OptionData {Value = Value, Text = Text});
            }
            output.SuppressOutput();
        }

    }
}
