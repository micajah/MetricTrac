using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using Micajah.Common.WebControls;
using MetricTrac.Utils;

namespace MetricTrac.Control
{
    public partial class MetricFilter : System.Web.UI.UserControl
    {
        public event EventHandler Use;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<Bll.DataRule.Entity> dr = Bll.PerformanceIndicator.List().Select(m => new Bll.DataRule.Entity(m.PerformanceIndicatorID, m.Name)).ToList();
                if (AddUnassigned)
                    dr.Insert(0, new Bll.DataRule.Entity(Guid.Empty, "<<< Not Assigned to Any PI >>>"));
                mslPI.LoadEntities(dr, null);
            }
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricFilter.css");
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            Guid?[] sMetrics = multiMetric.SelectedValues;
            List<Bll.DataRule.Entity> Metrics =
                MetricTrac.Bll.Metric.List(SelectedOrgLocations, SelectedGroupCategoryAspect, SelectedPerformanceIndicators, SelectedDataCollector)
                .Select(m => new Bll.DataRule.Entity(m.MetricID, m.Name))
                .ToList();
            multiMetric.LoadEntities(Metrics, null);
            multiMetric.SelectedValues = sMetrics;            
        }

        // Apply, Clear, Save handlers
        protected void lFilter_Click(object sender, EventArgs e)
        {  
            if (Use != null) Use(sender, e);
        }

        protected void lbClear_Click(object sender, EventArgs e)
        {
            ClearFilters();
            rcbSaved.SelectedValue = Guid.Empty.ToString();
            if (Use != null) Use(sender, e);
        }

        private void ClearFilters()
        {
            SelectedOrgLocations = null;
            SelectedGroupCategoryAspect = null;
            SelectedPerformanceIndicators = null;
            SelectedMetrics = null;
            SelectedDataCollector = null;
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            Guid ID = Guid.Empty;
            try { ID = new Guid(rcbSaved.SelectedValue); }
            catch { };
            MetricTrac.Bll.MetricFilter.Extend fi = new MetricTrac.Bll.MetricFilter.Extend
            {
                GroupCategoryAspectID = SelectedGroupCategoryAspect,
                DataCollectorID = SelectedDataCollector,
                FilterOrgLocation = SelectedOrgLocations,
                FilterPI = SelectedPerformanceIndicators,
                FilterMetric = SelectedMetrics,
                MetricFilterID = ID,
                Name = rcbSaved.Text// check it
            };
            SaveFilter.Filter = fi;

            Telerik.Web.UI.RadAjaxManager am = Telerik.Web.UI.RadAjaxManager.GetCurrent(this.Page);
            string s = "OpenDialogWindow();";
            if (IsDialog) s = "setTimeout('" + s + "', 333);";
            am.ResponseScripts.Add(s);
        }

        // select some saved filter
        protected void rcbSaved_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        { // move this to client side
            string FilterID = e.Value;
            rcbSaved.DataBind();
            Telerik.Web.UI.RadComboBoxItem item = rcbSaved.FindItemByValue(FilterID);
            if (item != null)
                item.Selected = true;
            else
                rcbSaved.SelectedValue = Guid.Empty.ToString();
            //---------
            Guid ID = Guid.Empty;
            try { ID = new Guid(e.Value); }
            catch { };
            if (ID != Guid.Empty)
            {
                Bll.MetricFilter.Extend mf = MetricTrac.Bll.MetricFilter.Get(ID);
                if (mf != null)
                {
                    SelectedOrgLocations = mf.FilterOrgLocation;
                    SelectedGroupCategoryAspect = mf.GroupCategoryAspectID;
                    SelectedPerformanceIndicators = mf.FilterPI;
                    SelectedMetrics = mf.FilterMetric;
                    SelectedDataCollector = mf.DataCollectorID;
                }
                else
                    ClearFilters();
            }
            else
                ClearFilters();
            if (Use != null) Use(this, new EventArgs());
        }

        protected void lbExport_Click(object sender, EventArgs e)
        {
            Guid ID = Guid.Empty;
            try { ID = new Guid(rcbSaved.SelectedValue); }
            catch { };
            MetricTrac.Bll.MetricFilter.Extend fi = new MetricTrac.Bll.MetricFilter.Extend
            {
                GroupCategoryAspectID = SelectedGroupCategoryAspect,
                DataCollectorID = SelectedDataCollector,
                FilterOrgLocation = SelectedOrgLocations,
                FilterPI = SelectedPerformanceIndicators,
                FilterMetric = SelectedMetrics,
                MetricFilterID = ID,
                Name = rcbSaved.Text// check it
            };
            MetricTrac.home.ExportExcel.Filter = fi;
            Response.Redirect("ExportExcel.aspx");
        }

        #region Public Control Properties

        public Button AllyButton // for ScoreCardMetricEdit ajax only
        {
            get { return lFilter; }
        }

        public DateTime BaseDate // used for View Data only
        {
            get { return dpBaseDate.SelectedDate == null ? DateTime.MinValue : (DateTime)dpBaseDate.SelectedDate; }
            set { dpBaseDate.SelectedDate = value; }
        }

        public bool IsInternalAjax // used for View Data only
        {
            get { return rapMetricSelect.IsAjaxRequest; }
        }
        
        // Visible section
        public bool ExportVisible // default false, is set to true on View Data only
        {            
            set { tdExport.Visible = value; }
        }

        public bool BaseDateVisible // default false, is set to true on View Data only
        {
            set { tdBaseDate.Visible = value; }
        }

        public bool MetricVisible // default = true, set to false only on MetricList and ScoreCardMetricEdit
        {            
            set { tdMetric.Visible = value; }
        }

        public bool TwoRowMode // default = false, set to true only on MetricList and ScoreCardMetricEdit
        {
            set { phTwoRowMode.Visible = value; }
        }

        // Metric List only
        public bool IsDialog { get; set; }
        public bool AddUnassigned { get; set; }
        #endregion
        
        #region Public Filter Values
        public Guid?[] SelectedOrgLocations
        {
            get { return multiOrgLocationSelect.OrgLocationsID; }
            set { multiOrgLocationSelect.OrgLocationsID = value; }
        }

        public Guid? SelectedGroupCategoryAspect
        {
            get { return sGCA.GCAID; }
            set { sGCA.GCAID = value; }
        }

        public Guid?[] SelectedPerformanceIndicators
        {
            get { return mslPI.SelectedValues; }
            set { mslPI.SelectedValues = value; }
        }

        public Guid?[] SelectedMetrics
        {
            get { return multiMetric.SelectedValues; }
            set { multiMetric.SelectedValues = value; }
        }

        public Guid? SelectedDataCollector
        {
            get
            {
                if (string.IsNullOrEmpty(rcbUser.SelectedValue)) return null;
                try { return new Guid(rcbUser.SelectedValue); }
                catch { return null; }
            }
            set { rcbUser.SelectedValue = (value == null || value == Guid.Empty) ? null : value.ToString(); }
        }
        #endregion        
    }
}