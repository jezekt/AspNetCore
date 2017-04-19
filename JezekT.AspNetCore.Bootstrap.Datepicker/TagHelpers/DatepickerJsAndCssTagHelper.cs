using System;
using System.Diagnostics.Contracts;
using System.Text;
using JezekT.AspNetCore.Bootstrap.Datepicker.Settings;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.Bootstrap.Datepicker.TagHelpers
{
    [HtmlTargetElement("datepicker-jscss")]
    public class DatepickerJsAndCssTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            var sb = new StringBuilder();

            AppendScript(sb, "https://cdnjs.cloudflare.com/ajax/libs/moment.js/2.18.1/moment.js");
            if (!string.IsNullOrEmpty(DatepickerSettings.LocalizationUrl))
            {
                AppendScript(sb, DatepickerSettings.LocalizationUrl);
            }
            AppendScript(sb, "https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/4.17.47/js/bootstrap-datetimepicker.min.js");

            sb.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"https://cdnjs.cloudflare.com/ajax/libs/bootstrap-datetimepicker/4.17.47/css/bootstrap-datetimepicker.css\"/>");

            output.Content.AppendHtml(sb.ToString());

        }

        private void AppendScript(StringBuilder sb, string src)
        {
            Contract.Requires(sb != null && !string.IsNullOrEmpty(src));

            sb.AppendLine($"<script type=\"text/javascript\" src=\"{src}\"></script>");
        }
    }
}
