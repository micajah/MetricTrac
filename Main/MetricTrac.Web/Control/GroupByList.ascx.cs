using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace MetricTrac.Control
{
    public partial class GroupByList : System.Web.UI.UserControl
    {   
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (_ExtendedMode)
                {
                    ListItem li = new ListItem("Metric Category", "MetricCategory");
                    ddlGroup.Items.Add(li);
                }
                HttpCookie GroupBy = Request.Cookies["__MetricsAndGroupBy"];
                if (GroupBy != null)
                {
                    ListItem li = ddlGroup.Items.FindByValue(GroupBy.Value);
                    if (li != null)
                        ddlGroup.SelectedValue = GroupBy.Value;
                }
            }            
            ddlGroup.Attributes.Add("onchange",                 
        	    "var ddlGroup = document.getElementById('" + ddlGroup.ClientID + "');" +
        	    "if (ddlGroup != null) document.cookie = '__MetricsAndGroupBy=' + ddlGroup.options[ddlGroup.selectedIndex].value;");
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricFilter.css");
        }

        public bool GroupByMetric
        {
            get { return ddlGroup.SelectedValue != "Location"; }
            /*set { ddlGroup.SelectedValue = value ? "Metric" : "Location"; }*/
        }

        private bool _ExtendedMode = false;
        public bool ExtendedMode
        {

            set { _ExtendedMode = value; }
        }

        public Bll.GroupByMode SelectedGroupByMode
        {
            get
            {
                Bll.GroupByMode _mode = Bll.GroupByMode.Location;
                switch (ddlGroup.SelectedValue)
                {
                    case "Location":
                        _mode = Bll.GroupByMode.Location;
                        break;
                    case "Metric":
                        _mode = Bll.GroupByMode.Metric;
                        break;
                    case "MetricCategory":
                        _mode = Bll.GroupByMode.MetricCategory;
                        break;
                }
                return _mode; 
            }
        }
    }
}