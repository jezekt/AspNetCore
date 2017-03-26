using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JezekT.AspNetCore.Select2.Settings;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;

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
        private const string MultipleName = "multiple";
        private const string RestrictIdsName = "restrict-ids";

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
        [HtmlAttributeName(MultipleName)]
        public bool Multiple { get; set; }
        [HtmlAttributeName(RestrictIdsName)]
        public string RestrictIds { get; set; }


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
            var elementName = $"select2-dropdown-{modelExpression.Name.ToLower()}";
            var currentValues = _generator.GetCurrentValues(ViewContext, modelExpression.ModelExplorer, modelExpression.Name, false);
            var selectBuilder = _generator.GenerateSelect(ViewContext, modelExpression.ModelExplorer, null, modelExpression.Name, Enumerable.Empty<SelectListItem>(), currentValues, false, null);

            if (selectBuilder != null)
            {
                var classes = elementName;
                TagHelperAttribute classTag;
                if (context.AllAttributes.TryGetAttribute("class", out classTag))
                {
                    classes = $"{classes} {classTag.Value}";
                }

                selectBuilder.Attributes.Add("class", classes);
                output.Content.AppendHtml(selectBuilder);
            }

            output.Content.AppendLine();

            output.TagName = "";

            var sb = new StringBuilder();
            if (SelectedValueProperty != null)
            {
                string dataString = null;
                if (selectDropdownContext.OptionDataSet != null)
                {
                    var elements = selectDropdownContext.OptionDataSet.Select(x => $"{{id:'{x.Value}', text:'{x.Text}'}}").ToArray();
                    dataString = string.Join(",", elements);
                }
                sb.AppendLine("<script type=\"text/javascript\">");
                    sb.AppendLine("var $select = $(\"." + elementName + "\").select2({");
                        sb.AppendLine("data: ["+ dataString +"]");
                    sb.AppendLine("});");
                sb.AppendLine("</script>");
            }
            else if (SelectedIdProperty != null)
            {
                sb.AppendLine("<script type=\"text/javascript\">");
                    sb.AppendLine("var $select = $(\"." + elementName + "\").select2({");
                        sb.AppendLine("ajax: {");
                            sb.AppendLine("url: \"" + AllDataUrl + "\",");
                            sb.AppendLine("dataType: 'json',");
                            sb.AppendLine("delay: " + Delay + ",");
                            sb.AppendLine("cache: " + Cache.ToString().ToLower() + ",");
                            sb.AppendLine("data: function(params) { return {");
                                sb.AppendLine("term: params.term || \"\",");
                                sb.AppendLine("page: params.page || 1,");
                                if (!string.IsNullOrEmpty(RestrictIds))
                                {
                                    sb.AppendLine("restrictIds: '" + RestrictIds + "',");
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
                        if (!string.IsNullOrEmpty(SelectDropdownSettings.LanguageCode))
                        {
                            sb.AppendLine("language: '" + SelectDropdownSettings.LanguageCode + "',");
                        }

                        if (Multiple)
                        {
                            sb.AppendLine("multiple: \"multiple\",");
                        }
                        if (!string.IsNullOrEmpty(Theme))
                        {
                            sb.AppendLine("theme: \"" + Theme + "\",");
                        }
                        sb.AppendLine("templateResult: formatResult, templateSelection: formatSelection");
                    sb.AppendLine("});");
                    sb.AppendLine("function formatResult(item) { if (item.loading) return \"" + SelectDropdownSettings.Loading + "\"; return item.text; }");
                    sb.AppendLine("function formatSelection(item) { return item.text; }");

                if (!Multiple && SelectedIdProperty?.Model != null)
                {
                    sb.AppendLine("var $option = $('<option selected>" + SelectDropdownSettings.Loading + "</option>').val(" + SelectedIdProperty.Model + ");");
                    sb.AppendLine("$select.append($option).trigger('change');");
                    sb.AppendLine("$.ajax({ type: 'GET', url: '" + SingleDataUrl + "/" + SelectedIdProperty.Model +
                                  "', dataType: 'json'}).then(function (data) { $option.text(data.text).val(data.id); $option.removeData();$select.trigger('change');});");
                }

                if (Multiple)
                {

                    sb.AppendLine("let defaults = $select.select2('data');");
                    sb.AppendLine("defaults.forEach(obj=>{");
                    sb.AppendLine("let order = $select.data('preserved-order') || [];");
                    sb.AppendLine("order[ order.length ] = obj.id;");
                    sb.AppendLine("$select.data('preserved-order', order)");
                    sb.AppendLine("});");

                    sb.AppendLine("function select_render($select,text){");
                    sb.AppendLine("const order = $select.data('preserved-order') || [];");
                    sb.AppendLine("const $container = $select.next('.select2-container');");
                    sb.AppendLine("const $tags = $container.find('li.select2-selection__choice');");
                    sb.AppendLine("const $input = $tags.last().next();");
                    sb.AppendLine("order.forEach(function(currentValue, index, arr){");
                    sb.AppendLine("let newVal = index + 1 + \". \" + currentValue;");
                    sb.AppendLine("let $el = $tags.filter(`[title=\"${ currentValue}\"]`);");
                    sb.AppendLine("if ($el[0] && $el[0] !== null) { $el[0].innerHTML = $el[0].innerHTML.replace(currentValue, newVal);}");
                    sb.AppendLine("$input.before( $el );");
                    sb.AppendLine("});");
                    sb.AppendLine("}");

                    sb.AppendLine("function select2SelectHandler(e){");
                    sb.AppendLine("selectionHandler(e.params.data.text, e.type);");
                    sb.AppendLine("}");

                    sb.AppendLine("function selectionHandler(text, type){");
                    sb.AppendLine("const order = $select.data('preserved-order') || [];");
                    sb.AppendLine("if (type == 'select2:select'){");
                    sb.AppendLine("order[ order.length ] = text;");
                    sb.AppendLine("} else if ( type == 'select2:unselect'){");
                    sb.AppendLine("let found_index = order.indexOf(text);");
                    sb.AppendLine("if(found_index >= 0)");
                    sb.AppendLine("order.splice(found_index,1);");
                    sb.AppendLine("}");
                    sb.AppendLine("$select.data('preserved-order', order);");
                    sb.AppendLine("select_render($select,text);");
                    sb.AppendLine("}");


                    sb.AppendLine("$select.on('select2:select', select2SelectHandler).on('select2:unselect', select2SelectHandler);");

                    if (SelectedIdProperty?.Model != null)
                    {
                        var idsString = "?ids=" + string.Join("&ids=", (int[])SelectedIdProperty.Model);
                        sb.AppendLine("$.ajax({ type: 'GET', url: '" + SingleDataUrl + idsString + "', dataType: 'json'}).then(function (data) { ");
                        sb.AppendLine("data.items.forEach(x => {");
                        sb.AppendLine("$select.append('<option value=\"' + x.id + '\" selected=\"selected\">' + x.text + '</option>');");
                        sb.AppendLine("$select.trigger('change');");
                        sb.AppendLine("selectionHandler(x.text, \"select2:select\");");
                        sb.AppendLine("});});");
                    }

                }

                sb.AppendLine("</script>");

            }
            output.Content.AppendHtml(sb.ToString());
        }


        public SelectDropdownTagHelper(IHtmlGenerator generator)
        {
            if (generator == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _generator = generator;
        }

    }
}
