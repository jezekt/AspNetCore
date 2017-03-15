using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JezekT.AspNetCore.DataTables.Settings;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace JezekT.AspNetCore.DataTables.TagHelpers
{
    [HtmlTargetElement("data-table")]
    [RestrictChildren("data-table-columns")]
    public class DataTableTagHelper : TagHelper
    {
        private const string DataUrlName = "url-data";
        private const string ProcessingName = "processing";
        private const string ServerSideName = "server-side";
        private const string SearchDelayName = "search-delay";
        private const string QueryIdsName = "query-ids";
        private const string ModelTypeName = "model-type";
        private const string TableIdName = "table-id";

        private readonly IModelMetadataProvider _modelMetadataProvider;

        [HtmlAttributeName(DataUrlName)]
        public string DataUrl { get; set; }
        [HtmlAttributeName(ProcessingName)]
        public bool Processing { get; set; } = true;
        [HtmlAttributeName(ServerSideName)]
        public bool ServerSide { get; set; } = true;
        [HtmlAttributeName(SearchDelayName)]
        public int SearchDelay { get; set; } = 1000;
        [HtmlAttributeName(QueryIdsName)]
        public int[] QueryIds { get; set; }
        [HtmlAttributeName(ModelTypeName)]
        public Type ModelType { get; set; }
        [HtmlAttributeName(TableIdName)]
        public string TableId { get; set; } = "dataTable";


        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (context == null || output == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            output.TagName = "";

            var tableContext = await GetTableContextAsync(context, output);

            var sb = new StringBuilder();
            AppendTableHtml(sb, tableContext, context);
            


            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine($"$('#{TableId}').DataTable({{");
            if (!string.IsNullOrEmpty(DataTableSettings.LocalizationUrl))
            {
                sb.AppendLine($"language: {{url: \"{DataTableSettings.LocalizationUrl}\"}},");
            }

            if (ServerSide)
            {
                sb.AppendLine("serverSide: true,");
                sb.AppendLine($"processing: {Processing.ToString().ToLower()},");
                sb.AppendLine($"searchDelay: {SearchDelay},");
                sb.AppendLine("ajax: {");
                sb.AppendLine("contentType: \"json\",");
                sb.AppendLine($"url:\"{DataUrl}\",");
                sb.AppendLine("data: function (params) {");
                sb.AppendLine("return {");
                sb.AppendLine("draw: params.draw,");
                sb.AppendLine("start: params.start,");
                sb.AppendLine("pageSize: params.length,");
                sb.AppendLine("term: params.search.value,");
                sb.AppendLine("orderField: params.columns[params.order[0].column].data,");
                sb.AppendLine("orderDirection: params.order[0].dir,");
                if (QueryIds != null)
                {
                    sb.AppendLine($"queryIds: \"{GetQueryIdsString(QueryIds)}\",");
                }
                sb.AppendLine("};},},");
                sb.AppendLine("columns: [");
                if (tableContext.ColumnsProperties != null)
                {
                    foreach (var columnProperty in tableContext.ColumnsProperties)
                    {
                        if (columnProperty.Metadata.ModelType == typeof(DateTime) || columnProperty.Metadata.ModelType == typeof(DateTime?))
                        {
                            sb.AppendLine($"{{ \"data\": \"{columnProperty.Name.ToLower()}\",");
                            sb.AppendLine("\"render\": function(data){");
                            sb.AppendLine("if (data == null) return data;");
                            sb.AppendLine("var d = new Date(data);");
                            sb.AppendLine("return d.toLocaleString();");
                            sb.AppendLine("}},");
                        }
                        else if (columnProperty.Metadata.ModelType == typeof(decimal) || columnProperty.Metadata.ModelType == typeof(decimal?))
                        {
                            sb.AppendLine($"{{ \"data\": \"{columnProperty.Name.ToLower()}\",");
                            sb.AppendLine("\"render\": function(data){");
                            sb.AppendLine("return data.toLocaleString();");
                            sb.AppendLine("}},");
                        }
                        else
                        {
                            sb.AppendLine($"{{ \"data\": \"{columnProperty.Name.ToLower()}\" }},");
                        }
                    }
                }
                if (tableContext.ActionDataSet != null && tableContext.ActionDataSet.Any())
                {
                    sb.AppendLine(GetColumnActionContent(tableContext.ActionDataSet));
                }
                sb.AppendLine("],");
            }
            else
            {
                throw new NotImplementedException();
            }

            sb.AppendLine("});");
            sb.AppendLine("</script>");

            output.Content.AppendHtml(sb.ToString());
        }


        public DataTableTagHelper(IModelMetadataProvider modelMetadataProvider)
        {
            if (modelMetadataProvider == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _modelMetadataProvider = modelMetadataProvider;
        }


        private async Task<DataTableContext> GetTableContextAsync(TagHelperContext context, TagHelperOutput output)
        {
            Contract.Requires(context != null && output != null);

            var tableContext = new DataTableContext { ModelExplorer = GetModelExplorer(ModelType) };
            context.Items.Add(typeof(DataTableTagHelper), tableContext);

            await output.GetChildContentAsync();

            return tableContext;
        }

        private void AppendTableHtml(StringBuilder sb, DataTableContext tableContext, TagHelperContext context)
        {
            Contract.Requires(sb != null && tableContext != null && context != null);

            var classes = "";
            if (context.AllAttributes.TryGetAttribute("class", out TagHelperAttribute classTag))
            {
                classes = classTag.Value.ToString();
            }

            sb.AppendLine($"<table id=\"{TableId}\" class=\"{classes}\" cellspacing=\"0\" width=\"100%\">");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            if (tableContext.ColumnsProperties != null)
            {
                foreach (var columnProperty in tableContext.ColumnsProperties)
                {
                    sb.AppendLine($"<th>{columnProperty.Metadata?.DisplayName}</th>");
                }
            }
            if (tableContext.ActionDataSet != null && tableContext.ActionDataSet.Any())
            {
                sb.AppendLine("<th></th>");
            }
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("</table>");
        }

        private string GetColumnActionContent(List<ActionData> actionDataSet)
        {
            Contract.Requires(actionDataSet != null);

            var sb = new StringBuilder();
            sb.AppendLine("{ \"sortable\" : false,");
            sb.AppendLine("\"render\" : function(data, type, row){");
            sb.AppendLine("var action = ''");

            foreach (var actionData in actionDataSet)
            {
                if (actionData != null)
                {
                    if (!string.IsNullOrEmpty(actionData.CanExecuteProperty))
                    {
                        sb.Append($"if(row['{actionData.CanExecuteProperty}'] == true){{ action = action + '");
                    }
                    else
                    {
                        sb.Append("{ action = action + '");
                    }

                    if (actionData.ActionUrl.Contains("id"))
                    {
                        var url = actionData.ActionUrl.Replace("id", "' + row['id'] + '");
                        sb.Append($"<a href={url}>{actionData.ActionTitle}</a>");
                    }
                    else
                    {
                        sb.Append($"<a href={actionData.ActionUrl}/' + row['id'] + '>{actionData.ActionTitle}</a>");
                    }

                    if (actionDataSet.IndexOf(actionData) < actionDataSet.Count - 1)
                    {
                        sb.Append(" | ");
                    }

                    sb.Append("';}");
                }
            }
            sb.Append("return action;");
            sb.Append("}}");
            return sb.ToString();
        }

        private string GetQueryIdsString(int[] filterIds)
        {
            Contract.Requires(filterIds != null);

            var sb = new StringBuilder();

            if (filterIds.Any())
            {
                foreach (var filterId in filterIds)
                {
                    sb.Append($"{filterId},");
                }
                return sb.Remove(sb.Length - 1, 1).ToString();
            }
            return null;
        }

        private ModelExplorer GetModelExplorer(Type modelType)
        {
            if (modelType != null)
            {
                return _modelMetadataProvider.GetModelExplorerForType(ModelType, null);
            }
            return null;
        }
    }
}


