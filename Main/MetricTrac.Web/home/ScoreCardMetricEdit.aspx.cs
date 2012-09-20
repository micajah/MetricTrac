using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class ScoreCardMetricEdit : System.Web.UI.Page
    {
        private Guid? mScoreCardID;
        protected Guid ScoreCardID
        {
            get
            {
                if (mScoreCardID != null) return (Guid)mScoreCardID;
                try
                {
                    string s = Request.QueryString["ScoreCardID"];
                    if (string.IsNullOrEmpty(s))
                    {
                        mScoreCardID = ViewState["ScoreCardID"] as Guid?;
                    }
                    else
                    {
                        mScoreCardID = new Guid(s);
                    }
                }
                catch
                {
                    mScoreCardID = Guid.Empty;
                }
                return (Guid)mScoreCardID;
            }
            set
            {
                ViewState["ScoreCardID"] = value;
                mScoreCardID = value;
            }
        }

        private Guid? mScoreCardMetricID;
        protected Guid ScoreCardMetricID
        {
            get
            {
                if (mScoreCardMetricID != null) return (Guid)mScoreCardMetricID;
                try
                {
                    mScoreCardMetricID = new Guid(Request.QueryString["ScoreCardMetricID"]);
                }
                catch
                {
                    mScoreCardMetricID = Guid.Empty;
                }
                return (Guid)mScoreCardMetricID;
            }
        }

        DropDownList ddlMetric { get { return (DropDownList)mfScoreCardMetric.FindControl("ddlMetric"); } }
        Control.Select.SelectPI cSelectPI { get { return (Control.Select.SelectPI)mfScoreCardMetric.FindControl("cSelectPI"); } }

        RadioButton rbMetric { get { return (RadioButton)mfScoreCardMetric.FindControl("rbMetric"); } }
        RadioButton rbPI { get { return (RadioButton)mfScoreCardMetric.FindControl("rbPI"); } }

        MetricTrac.Control.OrgLocationSelect orgLocationSelect { get { return (MetricTrac.Control.OrgLocationSelect)mfScoreCardMetric.FindControl("orgLocationSelect"); } }

        protected void mfScoreCardMetric_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCardMetric_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCardMetric_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCardMetric_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        private void RedirectToGrid()
        {            
            string url = "ScoreCardEdit.aspx";
            if (ScoreCardID != null)
                url += "?ScoreCardID=" + ScoreCardID;
            Response.Redirect(url);
        }

        Guid?[] SelOrgLocationsID
        {
            get { return ViewState["OrgLocationID"] as Guid?[]; }
            set { ViewState["OrgLocationID"] = value; }
        }

        Guid? SelGCAID
        {
            get { return ViewState["GCAID"] as Guid?; }
            set { ViewState["GCAID"] = value; }
        }

        Guid?[] SelPIID
        {
            get { return ViewState["PIID"] as Guid?[]; }
            set { ViewState["PIID"] = value; }
        }

        Guid? SelDataCollectorID
        {
            get { return ViewState["DataCollectorID"] as Guid?; }
            set { ViewState["DataCollectorID"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ScoreCardMetricID != Guid.Empty)
            {
                dsScoreCardMetric.WhereParameters.Add("ScoreCardMetricID", System.Data.DbType.Guid, ScoreCardMetricID.ToString());
                mfScoreCardMetric.ChangeMode(DetailsViewMode.Edit);
            }
            else
            {
                Parameter p = new Parameter("ScoreCardID", System.Data.DbType.Guid, ScoreCardID.ToString());
                dsScoreCardMetric.InsertParameters.Add(p);
            }
        }
        protected void Page_Prerender(object sender, EventArgs e)
        {

            string sm = ddlMetric.SelectedValue;
            var m = MetricTrac.Bll.Metric.List(SelOrgLocationsID, SelGCAID, SelPIID, SelDataCollectorID);
            ddlMetric.DataSource = m;
            ddlMetric.DataBind();

            if (!IsPostBack && mfScoreCardMetric.DataItem != null)
            {
                MetricTrac.Bll.ScoreCardMetric scm = (MetricTrac.Bll.ScoreCardMetric)mfScoreCardMetric.DataItem;
                ScoreCardID = scm.ScoreCardID;
                sm = scm.MetricID.ToString();
                orgLocationSelect.OrgLocationID = scm.OrgLocationID;
                if (mfScoreCardMetric.CurrentMode == DetailsViewMode.Insert)
                {
                }
                if (scm.PerformanceIndicatorId != null)
                {
                    cSelectPI.SelectedPIId = scm.PerformanceIndicatorId;
                }
            }
            
            if (!string.IsNullOrEmpty(sm))
            {
                foreach (ListItem li in ddlMetric.Items)
                {
                    if (li.Value == sm)
                    {
                        li.Selected = true;
                        break;
                    }
                }
            }
        }

        protected void mfScoreCardMetric_ExtractRowValues(System.Collections.Specialized.IOrderedDictionary fieldValues, bool includeReadOnlyFields, bool includeKeys)
        {
            if (rbMetric.Checked && !string.IsNullOrEmpty(ddlMetric.SelectedValue))
            {
                Guid MetricID = new Guid(ddlMetric.SelectedValue);
                fieldValues.Add("MetricID", MetricID);
            }
            if (rbPI.Checked && cSelectPI.SelectedPIId != null)
            {
                fieldValues.Add("PerformanceIndicatorId", (Guid)cSelectPI.SelectedPIId);
            }
            string v = (orgLocationSelect.OrgLocationID == null)?null:orgLocationSelect.OrgLocationID.ToString();
            Guid? OrgLocationID = null;
            try { OrgLocationID = new Guid(v); }
            catch { }
            if (OrgLocationID == Guid.Empty) OrgLocationID = null;
            fieldValues.Add("OrgLocationID", OrgLocationID);
            fieldValues["ScoreCardPeriodID"] = 1;
            fieldValues["InstanceId"] = MetricTrac.Bll.LinqMicajahDataContext.InstanceId;

        }        

        protected void mfScoreCardMetric_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
        }

        protected void mfScoreCardMetric_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
        }

        protected void mfScoreCardMetric_PreRender(object sender, EventArgs e)
        {
        }

        protected void ddlMetric_SelectedIndexChanged(object sender, EventArgs e)
        {
            string s = ddlMetric.SelectedValue;
            if (string.IsNullOrEmpty(s)) return;

            Guid MetricID;
            try { MetricID = new Guid(s); }
            catch { return; }

            MetricTrac.Bll.Metric m = MetricTrac.Bll.Metric.Get(MetricID);
            if (m == null) return;
        }

    }
}
