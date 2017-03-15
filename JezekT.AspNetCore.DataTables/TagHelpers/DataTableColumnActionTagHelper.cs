using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace JezekT.AspNetCore.DataTables.TagHelpers
{
    [HtmlTargetElement("data-table-column-action", ParentTag = "data-table-column")]
    [HtmlTargetElement("data-table-column-action", Attributes = ActionUrlName)]
    [HtmlTargetElement("data-table-column-action", Attributes = ActionTitleName)]
    [HtmlTargetElement("data-table-column-action", Attributes = ActionAttributeName)]
    [HtmlTargetElement("data-table-column-action", Attributes = ControllerAttributeName)]
    [HtmlTargetElement("data-table-column-action", Attributes = ActionUrlName)]
    [HtmlTargetElement("data-table-column-action", Attributes = RouteValuesPrefix + "*")]
    public class DataTableColumnActionTagHelper : TagHelper
    {
        private const string ActionUrlName = "action-url";
        private const string ActionTitleName = "action-title";
        private const string RouteValuesPrefix = "asp-route-";
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string VisibleName = "visible";
        private const string CanExecutePropertyName = "can-execute-asp-for";


        private readonly IHtmlGenerator _htmlGenerator;
        private IDictionary<string, string> _routeValues;

        [HtmlAttributeName(ActionUrlName)]
        public string ActionUrl { get; set; }
        [HtmlAttributeName(ActionTitleName)]
        public string ActionTitle { get; set; }

        [HtmlAttributeName(VisibleName)]
        public bool Visible { get; set; } = true;
        [HtmlAttributeName(CanExecutePropertyName)]
        public ModelExpression CanExecuteProperty { get; set; }

        [HtmlAttributeName(ActionAttributeName)]
        public string Action { get; set; }
        [HtmlAttributeName(ControllerAttributeName)]
        public string Controller { get; set; }
        [HtmlAttributeName(RouteValuesDictionaryName, DictionaryAttributePrefix = RouteValuesPrefix)]
        public IDictionary<string, string> RouteValues
        {
            get
            {
                if (_routeValues == null)
                {
                    _routeValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                }
                return _routeValues;
            }
            set
            {
                _routeValues = value;
            }
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (Visible)
            {
                var dataTableContext = (DataTableContext)context.Items[typeof(DataTableTagHelper)];
                if (dataTableContext.ActionDataSet == null)
                {
                    dataTableContext.ActionDataSet = new List<ActionData>();
                }

                if (string.IsNullOrEmpty(ActionUrl))
                {
                    RouteValueDictionary routeValues = null;
                    if (_routeValues != null && _routeValues.Count > 0)
                    {
                        routeValues = new RouteValueDictionary(_routeValues);
                    }

                    var tagBuilder = _htmlGenerator.GenerateActionLink(
                                ViewContext,
                                linkText: string.Empty,
                                actionName: Action,
                                protocol: null,
                                hostname: null,
                                fragment: null,
                                controllerName: Controller,
                                routeValues: routeValues,
                                htmlAttributes: null);

                    if (tagBuilder != null)
                    {
                        ActionUrl = tagBuilder.Attributes["href"];
                    }
                }

                dataTableContext.ActionDataSet.Add(new ActionData { ActionTitle = ActionTitle, ActionUrl = ActionUrl, CanExecuteProperty = CanExecuteProperty?.Name?.ToLower()});
                output.SuppressOutput();
            }
        }


        public DataTableColumnActionTagHelper(IHtmlGenerator htmlGenerator)
        {
            if (htmlGenerator == null) throw new ArgumentNullException();
            Contract.EndContractBlock();

            _htmlGenerator = htmlGenerator;
        }
    }
}
