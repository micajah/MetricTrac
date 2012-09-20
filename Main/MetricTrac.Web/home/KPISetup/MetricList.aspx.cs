using System;
using System.Web.UI;
using Telerik.Web.UI;

namespace MetricTrac
{
    public partial class MetricList : System.Web.UI.Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            Micajah.Common.Pages.MasterPage mp = Page.Master as Micajah.Common.Pages.MasterPage;            
            rapMetricList.LoadingPanelID = ((MetricTrac.MasterPage)mp).ralpLoadingPanel1ID;

            if (AddMode)
            {
                // ====================
                // this will be replaced by window.type = popup
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
                // ====================

                cMetricList.Mode = MetricTrac.Control.MetricList.enMode.PIAdd;
                btSave.Text = "Add Selected Metric to Performance Indicator & Close";
                trButtons.Visible = trFilter.Visible = true;
                divScroll.Attributes.Add("style", "height:375px;overflow:scroll");
                cMetricList.Width = 720;
            }
            pnlFilter.Visible = !(cMericFilter.IsDialog = AddMode);
            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (AddMode)
            {
                cMetricList.FilterPIID = cMericFilter.SelectedPerformanceIndicators;
                cMetricList.FilterGCAID = cMericFilter.SelectedGroupCategoryAspect;
                cMetricList.FilterOrgLocationsID = cMericFilter.SelectedOrgLocations;
                cMetricList.FilterDataCollectorID = cMericFilter.SelectedDataCollector;
                cMetricList.FilterMetricID = cMericFilter.SelectedMetrics;
            }
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
            // for now change to no postback from category select, otherwise do this in page load on postback
            cMetricList.FilterMetricCategoryID = mcs.MetricCategoryID;
            cMetricList.FilterNameDescription = txtSearch.Text;
            cMetricList.Rebind();
        }

        #region Protected Properties: AddMode, ramManager
        protected bool AddMode
        {
            get { return (Request.QueryString["IsNew"] == "True") || !String.IsNullOrEmpty(Request.QueryString["PerformanceIndicatorID"]); }
        }


        protected RadAjaxManager ramManager
        {
            get { return RadAjaxManager.GetCurrent(Page); }
        }
        #endregion
    }
}
