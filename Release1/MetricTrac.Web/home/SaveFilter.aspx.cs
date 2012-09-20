using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;

namespace MetricTrac
{
    public partial class SaveFilter : System.Web.UI.Page
    {
        const string SesionName = "SaveFilter_FilterInfo";
        public static MetricTrac.Bll.MetricFilter.Extend Filter
        {
            get { return HttpContext.Current.Session[SesionName] as MetricTrac.Bll.MetricFilter.Extend; }
            set { HttpContext.Current.Session[SesionName] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
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

            if (Filter != null && Filter.MetricFilterID != Guid.Empty)
            {
                mfSaveFilter.ChangeMode(DetailsViewMode.Edit);
                dsMetricFilter.WhereParameters.Add("MetricFilterID", System.Data.DbType.Guid, Filter.MetricFilterID.ToString());
            }
        }

        protected void mfSaveFilter_PreRender(object sender, EventArgs e)
        {
            if (Filter != null && Filter.MetricFilterID == Guid.Empty && !IsPostBack)
            {                
                ((MetricTrac.Control.OrglocationMultipick)mfSaveFilter.FindControl("orgLocationSelect1")).OrgLocationsID = Filter.OrgLocationsID;
                ((MetricTrac.Control.GCASelect)mfSaveFilter.FindControl("cASelect")).GCAID = Filter.GroupCategoryAspectID;
                if (Filter.PerformanceIndicatorFormID != null) ((DropDownList)mfSaveFilter.FindFieldControl("PerformanceIndicatorFormID")).SelectedValue = Filter.PerformanceIndicatorFormID.ToString();
                if (Filter.PerformanceIndicatorID != null) ((DropDownList)mfSaveFilter.FindFieldControl("PerformanceIndicatorID")).SelectedValue = Filter.PerformanceIndicatorID.ToString();
                if (Filter.DataCollectorID != null) ((DropDownList)mfSaveFilter.FindFieldControl("DataCollectorID")).SelectedValue = Filter.DataCollectorID.ToString();
            }
        }

        private void CloseOnReload()
        {
            ScriptManager.RegisterStartupScript(this, typeof(MetricList), "_CorrectReturn_", "CloseOnReload(false);", true);
        }

        protected void mfSaveFilter_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            CloseOnReload();
        }

        protected void mfSaveFilter_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            CloseOnReload();
        }

        protected void mfSaveFilter_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            CloseOnReload();
        }

        protected void mfSaveFilter_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) CloseOnReload();
        }

        protected void dsMetricFilter_Inserting1(object sender, LinqDataSourceInsertEventArgs e)
        {
            Guid?[] SelectedOrgLocationsID = ((MetricTrac.Control.OrglocationMultipick)mfSaveFilter.FindControl("orgLocationSelect1")).OrgLocationsID;
            if (SelectedOrgLocationsID != null)
                if (SelectedOrgLocationsID.Length > 0)
                {
                    string LocationsId = MetricTrac.Bll.MetricValue.GetLocationsEncodedString(SelectedOrgLocationsID, ',');
                    if (!String.IsNullOrEmpty(LocationsId))
                        ((MetricTrac.Bll.MetricFilter)e.NewObject).OrgLocations = LocationsId;
                }
        }
    }
}