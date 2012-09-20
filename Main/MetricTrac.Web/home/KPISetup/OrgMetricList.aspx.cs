using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class OrgMetricList : System.Web.UI.Page
    {
        Guid? mOrgLocationID;
        public Guid OrgLocationID
        {
            get
            {                
                if (mOrgLocationID!=null) return (Guid)mOrgLocationID;
                mOrgLocationID = Guid.Empty;
                string s = Request.QueryString["OrgLocationID"];
                if (string.IsNullOrEmpty(s)) return Guid.Empty;
                try { mOrgLocationID = new Guid(s); }
                catch { }
                return (Guid)mOrgLocationID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.MetricUtils.InitLinqDataSources(ldsMetric);
        }

        protected void cgvMetric_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            MetricTrac.Bll.Metric.Extend m = e.Row.DataItem as MetricTrac.Bll.Metric.Extend;
            if (m != null)
            {
                Repeater rpLocation = e.Row.FindControl("rpLocation") as Repeater;
                rpLocation.DataSource = m.AssignedOrgLocations;
                rpLocation.DataBind();
            }
        }

        protected void ldsMetric_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = Bll.Metric.ListLocations(OrgLocationID);
        }
    }
}