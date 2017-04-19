using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using JezekT.AspNetCore.Bootstrap.Datepicker.Settings;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.Bootstrap.Datepicker.TagHelpers
{
    [HtmlTargetElement("datepicker")]
    public class DatepickerTagHelper : TagHelper
    {
        private const string DateTimePropertyName = "datetime-property";
        private const string FormatName = "format";

        private readonly IHtmlGenerator _generator;
        
        [HtmlAttributeName(DateTimePropertyName)]
        public ModelExpression DateTimeProperty { get; set; }
        [HtmlAttributeName(FormatName)]
        public string Format { get; set; }


        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            output.TagName = "";
            
            if (DateTimeProperty != null)
            {
                var inputBuilder = _generator.GenerateTextBox(ViewContext, DateTimeProperty.ModelExplorer, DateTimeProperty.Name, DateTimeProperty.ModelExplorer.Model, null, null);

                if (inputBuilder != null)
                {
                    var key = DateTimeProperty.Name.ToLower();
                    var className = $"datepicker-{key}";

                    MergeClassAttributes(className, context, inputBuilder);

                    var sb = new StringBuilder();

                    AppendDatepickerHtml(sb, className, inputBuilder);

                    sb.AppendLine("<script type=\"text/javascript\">");
                    sb.AppendLine("$(function (){");
                    sb.AppendLine($"$('#{className}').datetimepicker({{");
                    if (!string.IsNullOrEmpty(DatepickerSettings.LanguageCode))
                    {
                        sb.AppendLine($"locale: '{DatepickerSettings.LanguageCode}',");
                    }
                    if (!string.IsNullOrEmpty(Format))
                    {
                        sb.AppendLine($"format: '{Format}',");
                    }
                    sb.AppendLine("});");
                    sb.AppendLine("});");
                    sb.AppendLine("</script>");

                    output.Content.AppendHtml(sb.ToString());
                }
            }
        }
        

        public DatepickerTagHelper(IHtmlGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _generator = generator;
        }


        private void MergeClassAttributes(string className, TagHelperContext context, TagBuilder inputBuilder)
        {
            Contract.Requires(context != null && inputBuilder != null && !string.IsNullOrEmpty(className));

            var classes = className;
            if (context.AllAttributes.TryGetAttribute("class", out TagHelperAttribute classTag))
            {
                classes = $"{classes} {classTag.Value}";
            }

            if (inputBuilder.Attributes.ContainsKey("class"))
            {
                inputBuilder.Attributes["class"] = $"{inputBuilder.Attributes["class"]} {classes}";
            }
            else
            {
                inputBuilder.Attributes.Add("class", classes);
            }
        }

        private void AppendDatepickerHtml(StringBuilder sb, string className, TagBuilder inputBuilder)
        {
            Contract.Requires(sb != null && inputBuilder != null && !string.IsNullOrEmpty(className));

            using (var stringWriter = new StringWriter())
            {
                inputBuilder.WriteTo(stringWriter, HtmlEncoder.Default);
                sb.AppendLine($"<div class='input-group date' id='{className}'>");
                    sb.AppendLine(stringWriter.ToString());
                    sb.AppendLine("<span class=\"input-group-addon\">");
                        sb.AppendLine("<span class=\"glyphicon glyphicon-calendar\"></span>");
                    sb.AppendLine("</span>");
                sb.AppendLine("</div>");
            }
        }
    }
}
