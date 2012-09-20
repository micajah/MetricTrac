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
            MetricTrac.Control.OrglocationMultipick orgLocationSelect = ((MetricTrac.Control.OrglocationMultipick)mfSaveFilter.FindControl("orgLocationSelect1"));
            MetricTrac.Control.GCASelect cASelect = ((MetricTrac.Control.GCASelect)mfSaveFilter.FindControl("cASelect"));
            MetricTrac.Control.MultiSelectList multiPI = ((MetricTrac.Control.MultiSelectList)mfSaveFilter.FindControl("multiPI"));
            MetricTrac.Control.MultiSelectList multiMetric = ((MetricTrac.Control.MultiSelectList)mfSaveFilter.FindControl("multiMetric"));
            DropDownList DataCollectorList = ((DropDownList)mfSaveFilter.FindFieldControl("DataCollectorID"));
            if (!IsPostBack)
            {
                List<Bll.DataRule.Entity> dr = Bll.PerformanceIndicator.List().Select(m => new Bll.DataRule.Entity(m.PerformanceIndicatorID, m.Name)).ToList();
                multiPI.LoadEntities(dr, null);
                
                if (Filter != null)
                {
                    orgLocationSelect.OrgLocationsID = Filter.FilterOrgLocation;
                    cASelect.GCAID = Filter.GroupCategoryAspectID;
                    multiPI.SelectedValues = Filter.FilterPI;
                    DataCollectorList.SelectedValue = Filter.DataCollectorID.ToString();
                }
            }
            List<Bll.DataRule.Entity> Metrics =
                MetricTrac.Bll.Metric.List(orgLocationSelect.OrgLocationsID, cASelect.GCAID, multiPI.SelectedValues, null)
                .Select(m => new Bll.DataRule.Entity(m.MetricID, m.Name))
                .ToList();
            multiMetric.LoadEntities(Metrics, null);
            if (!IsPostBack && Filter != null)
                multiMetric.SelectedValues = Filter.FilterMetric;
        }
        

        protected void mfSaveFilter_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {

            Bll.MetricFilter.Extend UpdateFilter = CreateFilterFields((Guid)e.Keys["MetricFilterID"]);
            Bll.MetricFilter.InsertOrUpdate(UpdateFilter, false);
            CloseOnReload(e.Keys["MetricFilterID"].ToString());
        }

        protected void dsMetricFilter_Inserted(object sender, LinqDataSourceStatusEventArgs e)
        {
            string ResultID = String.Empty;
            if (e.Result != null)
            {
                Bll.MetricFilter InsertedFilter = (Bll.MetricFilter)e.Result;
                Bll.MetricFilter.Extend UpdateFilter = CreateFilterFields(InsertedFilter.MetricFilterID);
                Bll.MetricFilter.InsertOrUpdate(UpdateFilter, true);
                ResultID = InsertedFilter.MetricFilterID.ToString();
            }
            CloseOnReload(ResultID);
        }

        protected void mfSaveFilter_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            mfSaveFilter.ChangeMode(DetailsViewMode.Insert);
            CloseOnReload(e.Keys["MetricFilterID"].ToString());
        }

        protected void mfSaveFilter_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) CloseOnReload(String.Empty);
        }

        private void CloseOnReload(string Argument)
        {
            ScriptManager.RegisterStartupScript(this, typeof(MetricList), "_CorrectReturn_", "CloseOnReload('" + Argument + "');", true);
        }

        private Bll.MetricFilter.Extend CreateFilterFields(Guid FilterID)
        {
            MetricTrac.Control.OrglocationMultipick orgLocationSelect = ((MetricTrac.Control.OrglocationMultipick)mfSaveFilter.FindControl("orgLocationSelect1"));            
            MetricTrac.Control.MultiSelectList multiPI = ((MetricTrac.Control.MultiSelectList)mfSaveFilter.FindControl("multiPI"));
            MetricTrac.Control.MultiSelectList multiMetric = ((MetricTrac.Control.MultiSelectList)mfSaveFilter.FindControl("multiMetric"));

            Bll.MetricFilter.Extend filter = new MetricTrac.Bll.MetricFilter.Extend();
            filter.MetricFilterID = FilterID;
            filter.FilterOrgLocation = orgLocationSelect.OrgLocationsID;
            filter.FilterPI = multiPI.SelectedValues;
            filter.FilterMetric = multiMetric.SelectedValues;
            return filter;
        }
    }
}