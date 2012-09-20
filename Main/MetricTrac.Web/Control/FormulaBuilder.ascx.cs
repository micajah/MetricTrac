using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace MetricTrac.Control
{
    public partial class FormulaBuilder : System.Web.UI.UserControl
    {
        public string Expression 
        { 
            get
            {
                return reExpression.Content;
            }
            set 
            {
                reExpression.Content = value;
            }
        }        

        public string InputClientID
        {
            get
            {
                return reExpression.ClientID;
            }

        }

        private Guid? mMetricCategoryID;
        protected Guid? MetricCategoryID
        {
            get
            {
                mMetricCategoryID = MetricTrac.Utils.MetricUtils.GetMetricCategoryFromFullName(Request.QueryString["MetricCategory"], MetricTrac.Bll.MetricCategory.SelectAll());                
                return mMetricCategoryID;
            }
        }

        private int mFrequencyID = -1;
        protected int FrequencyID
        {
            get
            {
                if (mFrequencyID != -1) return mFrequencyID;
                string strFrequencyID = Request.QueryString["FrequencyID"];
                if (string.IsNullOrEmpty(strFrequencyID)) return mFrequencyID;
                if (!int.TryParse(strFrequencyID, out mFrequencyID))
                    mFrequencyID = -1;
                return mFrequencyID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //Operations
            EditorToolGroup main = new EditorToolGroup();
            reExpression.Tools.Add(main);
            EditorTool opAdd = new EditorTool();
            opAdd.Name = "Add";
            opAdd.ShowIcon = false;
            opAdd.ShowText = true;
            opAdd.Text = "&nbsp;+&nbsp;";
            main.Tools.Add(opAdd);
            EditorTool opSub = new EditorTool();
            opSub.Name = "Sub";
            opSub.ShowIcon = false;
            opSub.ShowText = true;
            opSub.Text = "&nbsp;-&nbsp;";
            main.Tools.Add(opSub);
            EditorTool opMult = new EditorTool();
            opMult.Name = "Multiple";
            opMult.ShowIcon = false;
            opMult.ShowText = true;
            opMult.Text = "&nbsp;*&nbsp;";
            main.Tools.Add(opMult);
            EditorTool opDiv = new EditorTool();
            opDiv.Name = "Divide";
            opDiv.ShowIcon = false;
            opDiv.ShowText = true;
            opDiv.Text = "&nbsp;/&nbsp;";
            main.Tools.Add(opDiv);
            // ()
            EditorToolGroup scopes = new EditorToolGroup();            
            reExpression.Tools.Add(scopes);
            EditorTool scOpen = new EditorTool();
            scOpen.Name = "scOpen";
            scOpen.ShowIcon = false;
            scOpen.ShowText = true;
            scOpen.Text = "&nbsp;(&nbsp;";
            scopes.Tools.Add(scOpen);
            EditorTool scClose = new EditorTool();
            scClose.Name = "scClose";
            scClose.ShowIcon = false;
            scClose.ShowText = true;
            scClose.Text = "&nbsp;)&nbsp;";
            scopes.Tools.Add(scClose);
            EditorTool scDot = new EditorTool();
            scDot.Name = "scDot";
            scDot.ShowIcon = false;
            scDot.ShowText = true;
            scDot.Text = "&nbsp;.&nbsp;";
            scopes.Tools.Add(scDot);
            //Metrics
            EditorToolGroup dynamicToolbar = new EditorToolGroup();            
            reExpression.Tools.Add(dynamicToolbar);
            EditorDropDown ddn = new EditorDropDown("MetricDropdown");
            ddn.Text = "Available Metrics";
            ddn.Attributes["width"] = "500px";
            ddn.Attributes["popupwidth"] = "500px";
            ddn.Attributes["popupheight"] = "250px";
            List<Bll.Metric.Extend> _Metrics = Bll.Metric.GetBaseMetrics(MetricCategoryID, FrequencyID);
            foreach (Bll.Metric.Extend m in _Metrics)
                ddn.Items.Add(m.Name, m.FormulaCode);
            dynamicToolbar.Tools.Add(ddn);


            ddn = new EditorDropDown("OrgLocationDropdown");
            ddn.Text = "Org Location Mode";
            ddn.Attributes["width"] = "450px";
            ddn.Attributes["popupwidth"] = "450px";
            ddn.Attributes["popupheight"] = "250px";
            ddn.Items.Add("Sum", "Sum(");
            ddn.Items.Add("Quantity", "Quantity(");
            ddn.Items.Add("Average", "Average(");
            ddn.Items.Add("RMS sum", "RMS_Sum(");
            ddn.Items.Add("RMS average", "RMS_average(");
            ddn.Items.Add("Org Location", "");
            dynamicToolbar.Tools.Add(ddn);
        }
    }
}