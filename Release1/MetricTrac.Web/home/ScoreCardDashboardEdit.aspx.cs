using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class ScoreCardDashboardEdit : System.Web.UI.Page
    {
        private Guid? mScoreCardID;
        protected Guid ScoreCardID
        {
            get
            {
                if (mScoreCardID != null) return (Guid)mScoreCardID;
                mScoreCardID = Guid.Empty;
                string s = Request.QueryString["ScoreCardID"];
                if (!string.IsNullOrEmpty(s))
                {
                    try
                    {
                        mScoreCardID = new Guid(s);
                    }
                    catch { }
                }
                return (Guid)mScoreCardID;
            }
        }

        private Guid? mScoreCardDashboardID;
        protected Guid ScoreCardDashboardID
        {
            get
            {
                if (mScoreCardDashboardID != null) return (Guid)mScoreCardDashboardID;
                mScoreCardDashboardID = Guid.Empty;
                string s = Request.QueryString["ScoreCardDashboardID"];
                if (string.IsNullOrEmpty(s)) return Guid.Empty;
                try
                {
                    mScoreCardDashboardID = new Guid(s);
                }
                catch { }
                return (Guid)mScoreCardDashboardID;
            }
        }

        DropDownList ddlScoreCard
        {
            get { return (DropDownList)mfScoreCardDashboard.FindControl("ddlScoreCard"); }
        }

        Telerik.Web.UI.RadComboBox rcbMetricOrg
        {
            get { return (Telerik.Web.UI.RadComboBox)mfScoreCardDashboard.FindControl("rcbMetricOrg"); }
        }

        protected void mfScoreCardDashboard_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCardDashboard_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCardDashboard_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCardDashboard_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        private void RedirectToGrid()
        {
            Response.Redirect("DashboardEdit.aspx");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ScoreCardDashboardID != Guid.Empty)
            {
                dsScoreCardDashboard.WhereParameters.Add("ScoreCardDashboardID", System.Data.DbType.Guid, ScoreCardDashboardID.ToString());
                mfScoreCardDashboard.ChangeMode(DetailsViewMode.Edit);
            }
            else
            {
                Parameter p = new Parameter("UserId", System.Data.DbType.Guid, ((Guid)MetricTrac.Bll.LinqMicajahDataContext.LogedUserId).ToString());
                dsScoreCardDashboard.InsertParameters.Add(p);
            }
        }
        protected void Page_Prerender(object sender, EventArgs e)
        {
            Guid ScoreCardID = new Guid(ddlScoreCard.SelectedValue);
            string ScoreCardMetricID = rcbMetricOrg.SelectedValue;

            if(!IsPostBack && mfScoreCardDashboard.DataItem!=null)
            {
                MetricTrac.Bll.ScoreCardDashboard d = (MetricTrac.Bll.ScoreCardDashboard)mfScoreCardDashboard.DataItem;
                var scm = MetricTrac.Bll.ScoreCardMetric.Get(d.ScoreCardMetricID);
                ScoreCardMetricID = d.ScoreCardMetricID.ToString();
                ScoreCardID = scm.ScoreCardID;
            }

            var mo = MetricTrac.Bll.ScoreCardMetric.ListUnusedDashboard(ScoreCardID, ScoreCardDashboardID == Guid.Empty ? null : (Guid?)ScoreCardDashboardID);
            rcbMetricOrg.DataSource = mo;
            rcbMetricOrg.DataBind();
            if (mo.Count < 1) rcbMetricOrg.EmptyMessage = "This Score Card has not more Metric, Please select another Score Card";

            for(int i=0;i<mo.Count();i++)
            {
                Telerik.Web.UI.RadComboBoxItem it = rcbMetricOrg.Items[i];
                MetricTrac.Bll.ScoreCardMetric.Extend d = mo[i];
                string t = d.MetricName+" / "+d.OrgLocationName + " / " + d.ScoreCardPeriodName;
                it.Text = t;
            }

            foreach(Telerik.Web.UI.RadComboBoxItem i in rcbMetricOrg.Items)
            {
                i.Selected = i.Value == ScoreCardMetricID;
            }

            foreach (ListItem i in ddlScoreCard.Items)
            {
                i.Selected = i.Value == ScoreCardID.ToString();
            }

            Telerik.Web.UI.RadAjaxManager ram = Telerik.Web.UI.RadAjaxManager.GetCurrent(this);
            ram.UpdatePanelsRenderMode = UpdatePanelRenderMode.Inline;
            ram.AjaxSettings.AddAjaxSetting(mfScoreCardDashboard.FindControl("ddlScoreCard"), mfScoreCardDashboard.FindControl("rapDashboard"), (Telerik.Web.UI.RadAjaxLoadingPanel)mfScoreCardDashboard.FindControl("ralpDashboard"));
        }

        protected void mfScoreCardDashboard_ExtractRowValues(System.Collections.Specialized.IOrderedDictionary fieldValues, bool includeReadOnlyFields, bool includeKeys)
        {
            string v = rcbMetricOrg.SelectedValue;
            if (!string.IsNullOrEmpty(v))
            {
                Guid ScoreCardMetricID = new Guid(v);
                fieldValues.Add("ScoreCardMetricID", ScoreCardMetricID);
            }
        }

        protected void dsScoreCardMetric_Inserted(object sender, LinqDataSourceStatusEventArgs e)
        {
        }

        protected void dsScoreCardMetric_Inserting(object sender, LinqDataSourceInsertEventArgs e)
        {
        }


    }
}
