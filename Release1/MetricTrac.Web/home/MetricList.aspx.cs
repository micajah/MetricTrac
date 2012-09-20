using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;

namespace MetricTrac
{
    public partial class MetricList : System.Web.UI.Page
    {
        protected bool AddMode
        {
            get { return PerformanceIndicatorID != null; }
        }

        private Guid? mPerformanceIndicatorID;
        private bool mPerformanceIndicatorIDReady;
        public Guid? PerformanceIndicatorID
        {
            get
            {
                if (mPerformanceIndicatorIDReady) return mPerformanceIndicatorID;
                mPerformanceIndicatorIDReady = true;
                if (Request.QueryString["IsNew"] == "True")
                {
                    mPerformanceIndicatorID = Guid.Empty;
                    return Guid.Empty;
                }

                string s = Request.QueryString["PerformanceIndicatorID"];
                if (string.IsNullOrEmpty(s)) return null;

                try { mPerformanceIndicatorID = new Guid(s); }
                catch { }
                return mPerformanceIndicatorID;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            if (AddMode)
            {
                Micajah.Common.Pages.MasterPage mp = Page.Master as Micajah.Common.Pages.MasterPage;
                mp.VisibleApplicationLogo = false;
                mp.VisibleBreadcrumbs = false;
                mp.VisibleFooter = false;
                mp.VisibleFooterLinks = false;
                mp.VisibleHeader = false;
                mp.VisibleHeaderLinks = false;
                mp.VisibleLeftArea = false;
                mp.VisibleMainMenu = false;
                mp.VisibleHeaderLogo = false;                
                mp.VisibleSearchControl = false;

                cMetricList.Mode = MetricTrac.Control.MetricList.enMode.PIAdd;
                btSave.Text = "Add Selected Metric to Performance Indicator & Close";
                trButtons.Visible = true;
                trFilter.Visible = true;
                divScroll.Attributes.Add("style", "height:375px;overflow:scroll");

                cMetricList.Width = 720;
           }            
            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AddMode)
            {
                cMetricList.FilterPIID = cMericFilter.PIID;
                cMetricList.FilterPIFormID = cMericFilter.PIFormID;
                cMetricList.FilterGCAID = cMericFilter.GCAID;
                cMetricList.FilterOrgLocationsID = cMericFilter.SelectOrgLocationsID;
                cMetricList.FilterDataCollectorID = cMericFilter.DataCollectorID;
                cMetricList.FilterMetricID = cMericFilter.MetricID;
            }
            else
            {
                cMetricList.FilterMetricCategoryID = mcs.MetricCategoryID;
                cMetricList.FilterNameDescription = txtSearch.Text;
                cMetricList.Rebind();
            }
            pnlFilter.Visible = !AddMode;
            cMericFilter.IsDialog = AddMode;
        }

        protected void btSave_Click(object sender, EventArgs e)
        {
            cMetricList.Add();
            ScriptManager.RegisterStartupScript(this, typeof(MetricList), "_CorrectReturn_", "CloseOnReload(false);", true);
        }

        protected void cMericFilter_Use(object sender, EventArgs e)
        {
            cMetricList.Rebind();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {            
            
        }
    }
}
