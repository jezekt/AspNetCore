using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.Select2.TagHelpers
{
    [HtmlTargetElement("select-dropdown")]
    public class SelectDropdownTagHelper : TagHelper
    {
        private const string SelectedIdName = "selected-id-property";
        private const string SelectedValueName = "selected-value-property";
        private const string AllDataUrlName = "url-all-data";
        private const string SingleDataUrlName = "url-single-data";
        private const string PageSizeName = "page-size";
        private const string DelayName = "delay";
        private const string CacheName = "cache";
        private const string ThemeName = "theme";
        private const string InputLengthMinName = "input-length-min";
        private const string InputLengthMaxName = "input-length-max";
        private const string FilterIdsName = "filter-ids";

        private readonly IHtmlGenerator _generator;

        [HtmlAttributeName(SelectedIdName)]
        public ModelExpression SelectedIdProperty { get; set; }
        [HtmlAttributeName(SelectedValueName)]
        public ModelExpression SelectedValueProperty { get; set; }
        [HtmlAttributeName(AllDataUrlName)]
        public string AllDataUrl { get; set; }
        [HtmlAttributeName(SingleDataUrlName)]
        public string SingleDataUrl { get; set; }
        [HtmlAttributeName(PageSizeName)]
        public int PageSize { get; set; } = 10;
        [HtmlAttributeName(DelayName)]
        public int Delay { get; set; } = 250;
        [HtmlAttributeName(CacheName)]
        public bool Cache { get; set; } = true;
        [HtmlAttributeName(ThemeName)]
        public string Theme { get; set; }
        [HtmlAttributeName(InputLengthMinName)]
        public int InputLengthMin { get; set; } = 1;
        [HtmlAttributeName(InputLengthMaxName)]
        public int InputLengthMax { get; set; } = 20;
        [HtmlAttributeName(FilterIdsName)]
        public string FilterIds { get; set; }


        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            var selectDropdownContext = new SelectDropdownContext();
            context.Items.Add(typeof(SelectDropdownTagHelper), selectDropdownContext);

            await output.GetChildContentAsync();

            var modelExpression = SelectedValueProperty ?? SelectedIdProperty;
            var key = modelExpression.Name.ToLower();
            var className = $"select2-dropdown-{key}";
            var currentValues = _generator.GetCurrentValues(ViewContext, modelExpression.ModelExplorer, modelExpression.Name, false);
            var selectBuilder = _generator.GenerateSelect(ViewContext, modelExpression.ModelExplorer, null, modelExpression.Name, Enumerable.Empty<SelectListItem>(), currentValues, false, null);

            if (selectBuilder != null)
            {
                var classes = className;
                TagHelperAttribute classTag;
                if (context.AllAttributes.TryGetAttribute("class", out classTag))
                {
                    classes = $"{classes} {classTag.Value}";
                }

                if (selectBuilder.Attributes.ContainsKey("class"))
                {
                    selectBuilder.Attributes["class"] = $"{selectBuilder.Attributes["class"]} {classes}";
                }
                else
                {
                    selectBuilder.Attributes.Add("class", classes);
                }
                output.Content.AppendHtml(selectBuilder);
            }

            output.Content.AppendLine();

            output.TagName = "";

            var sb = new StringBuilder();
            AppendLocalizationScript(sb);

            var initializeFunctionName = "initializeFunction";
            sb.AppendLine("<script type=\"text/javascript\">");

                if (SelectedValueProperty != null)
                {
                    AppendInitializeSelectEnumFunction(sb, selectDropdownContext, initializeFunctionName, className);
                }
                else if (SelectedIdProperty != null)
                {
                    AppendInitializeSelectAjaxFunction(sb, initializeFunctionName, className);
                }
                else
                {
                    throw new NotImplementedException();
                }
                sb.AppendLine(initializeFunctionName + "();");
            sb.AppendLine("</script>");

            output.Content.AppendHtml(sb.ToString());
        }


        public SelectDropdownTagHelper(IHtmlGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _generator = generator;
        }


        private void AppendInitializeSelectAjaxFunction(StringBuilder sb, string functionName, string className)
        {
            Contract.Requires(sb != null);

            sb.AppendLine("function " + functionName + "(){");
                sb.AppendLine("var select = $(\"." + className + "\").select2({");
                    sb.Append("width:\"100%\",");
                    sb.AppendLine("ajax: {");
                        sb.AppendLine("url: \"" + AllDataUrl + "\",");
                        sb.AppendLine("dataType: 'json',");
                        sb.AppendLine("delay: " + Delay + ",");
                        sb.AppendLine("cache: " + Cache.ToString().ToLower() + ",");
                        sb.AppendLine("data: function(params) { return {");
                        sb.AppendLine("term: params.term || \"\",");
                        sb.AppendLine("page: params.page || 1,");
                        if (!string.IsNullOrEmpty(FilterIds))
                        {
                            sb.AppendLine("filterIds: '" + FilterIds + "',");
                        }
                        sb.AppendLine("pageSize:" + PageSize);
                    sb.AppendLine("};},");
                    sb.AppendLine("processResults: function(data, params) {");
                        sb.AppendLine("params.page = params.page || 1;");
                        sb.AppendLine("return {");
                            sb.AppendLine("results: data.items,");
                            sb.AppendLine("pagination: { more: data.more }");
                    sb.AppendLine("};},},");
                    sb.AppendLine("escapeMarkup: function (markup) { return markup; },");
                    sb.AppendLine("minimumInputLength: " + InputLengthMin + ",");
                    sb.AppendLine("maximumInputLength: " + InputLengthMax + ",");
                    if (!string.IsNullOrEmpty(Resources.TagHelpers.SelectDropdownTagHelper.LanguageCode))
                    {
                        sb.AppendLine("language: '" + Resources.TagHelpers.SelectDropdownTagHelper.LanguageCode + "',");
                    }

                    if (!string.IsNullOrEmpty(Theme))
                    {
                        sb.AppendLine("theme: \"" + Theme + "\",");
                    }
                    sb.AppendLine("templateResult: formatResult, templateSelection: formatSelection");
                sb.AppendLine("});");
                sb.AppendLine("function formatResult(item) { if (item.loading) return \"" + Resources.TagHelpers.SelectDropdownTagHelper.Loading + "\"; return item.text; }");
                sb.AppendLine("function formatSelection(item) { return item.text; }");

                if (SelectedIdProperty?.Model != null && !HasDefaultValue(SelectedIdProperty.Model))
                {
                    sb.AppendLine("var option = $('<option selected>" + Resources.TagHelpers.SelectDropdownTagHelper.Loading + "</option>').val(" + SelectedIdProperty.Model + ");");
                    sb.AppendLine("select.append(option).trigger('change');");
                    sb.AppendLine("$.ajax({ type: 'GET', url: '" + SingleDataUrl + "/" + SelectedIdProperty.Model +
                                    "', dataType: 'json'}).then(function (data) { option.text(data.text).val(data.id); option.removeData();select.trigger('change');});");
                }
            sb.AppendLine("}");
        }

        private void AppendInitializeSelectEnumFunction(StringBuilder sb, SelectDropdownContext selectDropdownContext, string functionName, string className)
        {
            Contract.Requires(sb != null && selectDropdownContext != null);

            string dataString = null;
            if (selectDropdownContext.OptionDataSet != null)
            {
                var elements = selectDropdownContext.OptionDataSet.Select(x => $"{{id:'{x.Value}', text:'{x.Text}'}}").ToArray();
                dataString = string.Join(",", elements);
            }
            sb.AppendLine("function " + functionName + "(){");
                sb.AppendLine("var select = $(\"." + className + "\").select2({");
                sb.AppendLine("data: [" + dataString + "]");
                sb.AppendLine("});");
            sb.AppendLine("}");
        }

        private void AppendLocalizationScript(StringBuilder sb)
        {
            Contract.Requires(sb != null);

            if (!string.IsNullOrEmpty(Resources.TagHelpers.SelectDropdownTagHelper.LocalizationUrl))
            {
                sb.AppendLine($"<script src=\"{Resources.TagHelpers.SelectDropdownTagHelper.LocalizationUrl}\" type=\"text/javascript\"></script>");
            }
        }

        private bool HasDefaultValue(object model)
        {
            var type = model.GetType();
            var typeInfo = type?.GetTypeInfo();
            if (typeInfo != null && typeInfo.IsValueType)
            {
                return model.Equals(Activator.CreateInstance(type));
            }
            return true;
        }
    }
}
