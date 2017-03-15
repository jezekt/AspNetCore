using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace JezekT.AspNetCore.DataTables.TagHelpers
{
    public class DataTableContext
    {
        public ModelExplorer ModelExplorer { get; set; }
        public List<ModelExpression> ColumnsProperties { get; set; }
        public List<ActionData> ActionDataSet { get; set; }
    }
}
