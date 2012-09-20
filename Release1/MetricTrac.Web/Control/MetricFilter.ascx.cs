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
        public bool IsDialog { get; set; }
        public event EventHandler Use;
        public bool BaseDateVisible
        {
            get { return tdBaseDate.Visible; }
            set { tdBaseDate.Visible = value; }
        }

        public bool AddUnassigned { get; set; }
        public bool GroupByVisible { get; set; }
        public bool FilterSectionVisible { get; set; }
        public bool GroupByMetric
        {   get { return ddlGroup.SelectedValue == "True";}
            set { ddlGroup.SelectedValue = value.ToString(); }}

        public bool MetricVisible
        {
            get { return tdMetric.Visible; }
            set { tdMetric.Visible = value; }
        }

        public bool TwoRowMode { get; set; }

        public Button AllyButton
        {
            get { return lFilter; }
        }

        private Guid? SelectedMetricID;

        protected void Page_Init(object sender, EventArgs e)
        {
            rwSaveFilter.Behaviors = Telerik.Web.UI.WindowBehaviors.Close | Telerik.Web.UI.WindowBehaviors.Move | Telerik.Web.UI.WindowBehaviors.Resize;
            if (!IsPostBack)
            {
                HttpCookie GroupBy = Request.Cookies["__MetricsAndGroupBy"];
                if (GroupBy != null) ddlGroup.SelectedValue = GroupBy.Value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            dpBaseDate.DateInput.EnableEmbeddedBaseStylesheet = false;
            dpBaseDate.DateInput.EnableEmbeddedSkins = false;
            phTwoRow1.Visible = TwoRowMode;
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricFilter.css");
            SelectedMetricID = MetricID;            
        }

        protected void rcbSaved_DataBound(object sender, EventArgs e)
        {
            rcbSaved.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem());            
        }
       
        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (AddUnassigned) rcbPI.Items[1].Visible = true;
            phGroup.Visible = GroupByVisible;
            phFilter.Visible = FilterSectionVisible;
            List<MetricTrac.Bll.Metric> ds = MetricTrac.Bll.Metric.List(SelectOrgLocationsID, GCAID, PIID, PIFormID, DataCollectorID);
            rcbMetric.DataSource = ds;
            rcbMetric.DataBind();
            rcbMetric.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem());
            if (ds.Where(mm => (mm.MetricID == SelectedMetricID)).Count() > 0) rcbMetric.SelectedValue = SelectedMetricID.ToString();
            ddlGroup.Attributes.Add("onchange", "SaveOnClient()");
            InitExportLink();
        }

        private Guid? GetSelectedGuid(Telerik.Web.UI.RadComboBox ddl)
        {
            if (string.IsNullOrEmpty(ddl.SelectedValue)) return null;
            try { return new Guid(ddl.SelectedValue); }
            catch { return null; }
        }

        public DateTime BaseDate
        {
            get { return dpBaseDate.SelectedDate == null ? dpBaseDate.MinDate : (DateTime)dpBaseDate.SelectedDate; }
            set { dpBaseDate.SelectedDate = value; }
        }

        public Guid? PIID
        {
            get { return GetSelectedGuid(rcbPI); }
        }
        public Guid? PIFormID
        {
            get { return GetSelectedGuid(rcbPIForm); }
        }
        public Guid? GCAID
        {
            get { return sGCA.GCAID; }
        }

        public Guid?[] SelectOrgLocationsID
        {
            get
            {
                return multiOrgLocationSelect.OrgLocationsID;
            }
            set
            {
                multiOrgLocationSelect.OrgLocationsID = value;
            }
        }

        public Guid? DataCollectorID
        {
            get { return GetSelectedGuid(rcbUser); }
        }
        public Guid? MetricID
        {
            get { return GetSelectedGuid(rcbMetric); }
        }

        protected void rapSaved_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            rcbSaved.DataBind();
        }

        static string GetStr(Guid? g)
        {
            if (g == null || g == Guid.Empty) return null;
            return g.Value.ToString();
        }
        protected void lFilter_Click(object sender, EventArgs e)
        {
            if (hfSaved.Value == "1")
            {
                string sId = rcbSaved.SelectedValue;
                Guid ID = Guid.Empty;
                try { ID = new Guid(sId);} catch{};
                if(ID==Guid.Empty)
                {
                    SelectOrgLocationsID = null;
                    sGCA.GCAID=null;
                    rcbPIForm.SelectedValue=null;
                    rcbPI.SelectedValue=null;
                    rcbMetric.SelectedValue=null;
                    rcbUser.SelectedValue=null;
                }
                else
                {
                    var l = MetricTrac.Bll.MetricFilter.List();
                    var mf = l.Where(f => f.MetricFilterID == ID).FirstOrNull();

                    SelectOrgLocationsID = MetricTrac.Bll.MetricFilter.GetDecodedLocations(mf.OrgLocations);
                    sGCA.GCAID = mf.GroupCategoryAspectID;
                    rcbPIForm.SelectedValue = GetStr(mf.PerformanceIndicatorFormID);
                    rcbPI.SelectedValue = GetStr(mf.PerformanceIndicatorID);
                    rcbMetric.SelectedValue = GetStr(mf.MetricID);
                    rcbUser.SelectedValue = GetStr(mf.DataCollectorID);
                }
            }
            if (Use != null) Use(sender, e);
            hfSaved.Value = null;
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {   
            MetricTrac.Bll.MetricFilter.Extend fi = new MetricTrac.Bll.MetricFilter.Extend
            {
                OrgLocationID = SelectOrgLocationsID.Length > 0 ? SelectOrgLocationsID[0] : null,
                GroupCategoryAspectID = GCAID,
                PerformanceIndicatorFormID = PIFormID,
                PerformanceIndicatorID = PIID,
                MetricID = MetricID,
                DataCollectorID = DataCollectorID,
                OrgLocationsID = SelectOrgLocationsID
            };
            SaveFilter.Filter = fi;

            Telerik.Web.UI.RadAjaxManager am = Telerik.Web.UI.RadAjaxManager.GetCurrent(this.Page);
            string s = "OpenDialogWindow();";
            if (IsDialog) s = "setTimeout('" + s + "', 333);";
            am.ResponseScripts.Add(s);
        }        

        protected string AjaxManagerClientID
        {
            get { return Telerik.Web.UI.RadAjaxManager.GetCurrent(Page).ClientID;}
        }

        public event EventHandler GroupChanged;
        protected void ddlGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (GroupChanged != null) GroupChanged(sender, e);
        }

        public bool IsInternalAjax
        {
            get { return rapMetricSelect.IsAjaxRequest; }
        }

        protected void lbClear_Click(object sender, EventArgs e)
        {
            rcbPI.SelectedValue=null;
            rcbPIForm.SelectedValue=null;
            sGCA.GCAID=null;
            SelectOrgLocationsID=null;
            rcbUser.SelectedValue=null;
            rcbMetric.SelectedValue=null;
            rcbSaved.SelectedValue = null;
            if (Use != null) Use(sender, e); 
        }

        

        protected void InitExportLink()
        {
            string url = string.Empty;
            if (!string.IsNullOrEmpty(rcbSaved.SelectedValue) && rcbSaved.SelectedValue!=Guid.Empty.ToString()) 
                url += "&saved=" + HttpUtility.UrlEncode(rcbSaved.SelectedValue);
            string LocationsId = MetricTrac.Bll.MetricValue.GetLocationsEncodedString(SelectOrgLocationsID, ',');            
            if (!String.IsNullOrEmpty(LocationsId))
                url += "&location=" + LocationsId;
            if (sGCA.GCAID != null && sGCA.GCAID != Guid.Empty) 
                url += "&gca=" + sGCA.GCAID;
            if (!string.IsNullOrEmpty(rcbPIForm.SelectedValue) && rcbPIForm.SelectedValue!=Guid.Empty.ToString()) 
                url += "&piform=" + rcbPIForm.SelectedValue;
            if (!string.IsNullOrEmpty(rcbMetric.SelectedValue) && rcbMetric.SelectedValue!=Guid.Empty.ToString()) 
                url+="&metric="+rcbMetric.SelectedValue;
            if (!string.IsNullOrEmpty(rcbUser.SelectedValue) && rcbUser.SelectedValue != Guid.Empty.ToString())
                url+="&collector="+rcbUser.SelectedValue;

            if (url.StartsWith("&")) url="?"+url.Substring(1);

            url = "~/home/ExportExcel.aspx" + url;
            hlExport.NavigateUrl = url;
        }

        public bool ExportVisible
        {
            get { return tdExport.Visible; }
            set { tdExport.Visible = value; }
        }
    }
}
