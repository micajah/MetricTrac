using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Micajah.Common.WebControls;

namespace MetricTrac.MTControls
{
    public class MTHtmlEditorField : HtmlEditorField
    {
        public string ToolsFile { get; set; }
        public EditModes EditModes { get; set; }
                       
        public MTHtmlEditorField()
        {
            EditModes = EditModes.Design;            
        }

        private HtmlEditor GetFieldControl(DataControlFieldCell cell)
        {
            foreach (WebControl c in cell.Controls)
            {
                if (c is HtmlEditor) return (HtmlEditor)c;
            }
            return null;
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            base.InitializeDataCell(cell, rowState);
            HtmlEditor control = GetFieldControl(cell);
            if (base.Visible && control != null)
            {
                if(!String.IsNullOrEmpty(ToolsFile)) control.ToolsFile = ToolsFile;
                control.EditModes = EditModes;                
            }
        }

        protected override object ExtractControlValue(System.Web.UI.Control control)
        {
            object v = base.ExtractControlValue(control);
            return v;
        }
    }
}
