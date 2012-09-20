using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;
using System.Data;
using System.Data.Linq;
using System.Collections;

namespace MetricTrac.Control
{    
    public partial class ScoreCardEdit : System.Web.UI.UserControl
    {
        public bool MyDashboardMode { get; set; }

        private Guid? mScoreCardID;
        protected Guid ScoreCardID
        {
            get
            {
                if (mScoreCardID != null) return (Guid)mScoreCardID;
                try
                {
                    mScoreCardID = new Guid(Request.QueryString["ScoreCardID"]);
                }
                catch
                {
                    mScoreCardID = Guid.Empty;
                }
                return (Guid)mScoreCardID;
            }
        }

        void HideColumn(string DataField)
        {
            foreach (DataControlField f in cgvScoreCardMetric.Columns)
            {
                if (!(f is BoundField)) continue;
                BoundField bf = (BoundField)f;
                if (bf.DataField == DataField) bf.Visible = false;
            }
        }

        protected void cgvScoreCardMetric_Init(object sender, EventArgs e)
        {
            CommonGridView cgvScoreCardMetric = (CommonGridView)sender;
            if (MyDashboardMode && cgvScoreCardMetric.DataSourceID != "dsScoreCardDashboard")
            {
                cgvScoreCardMetric.DataKeyNames = new string[] { "ScoreCardDashboardID" };
                cgvScoreCardMetric.DataSourceID = "dsScoreCardDashboard";                
            }

            if (!MyDashboardMode)
            {
                HideColumn("ScoreCardPeriodName");
                HideColumn("MinValue");
                HideColumn("MaxValue");
                HideColumn("BaselineValue");
                HideColumn("BaselineValueLabel");
                HideColumn("Breakpoint1Value");
                HideColumn("Breakpoint1ValueLabel");
                HideColumn("Breakpoint2Value");
                HideColumn("Breakpoint2ValueLabel");
            }
        }

        protected void dsScoreCardDashboard_Deleting(object sender, LinqDataSourceDeleteEventArgs e)
        {
            if (MyDashboardMode)
            {
                MetricTrac.Bll.ScoreCardDashboard d = (MetricTrac.Bll.ScoreCardDashboard)e.OriginalObject;
                d.InstanceId = MetricTrac.Bll.LinqMicajahDataContext.InstanceId;
                d.UserId = (Guid)MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            trScoreCard.Visible = !MyDashboardMode;

            if (ScoreCardID != Guid.Empty)
            {
                dsScoreCard.WhereParameters.Add("ScoreCardID", System.Data.DbType.Guid, ScoreCardID.ToString());
                mfScoreCard.ChangeMode(DetailsViewMode.Edit);
                trMetricTitle.Visible = true;
                trMetricGrid.Visible = true;
            }
            else
            {
                trMetricTitle.Visible = MyDashboardMode;
                trMetricGrid.Visible = MyDashboardMode;
            }
        }

        protected void dsScoreCardMetric_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = MetricTrac.Bll.ScoreCardMetric.List(ScoreCardID); ;
        }

        protected void dsScoreCardDashboard_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = MetricTrac.Bll.ScoreCardDashboard.List(null, MetricTrac.PerformanceIndicatorCalc.CalcStringFormula);
        }

        protected void mfScoreCard_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCard_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCard_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfScoreCard_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        private void RedirectToGrid()
        {
            Response.Redirect("ScoreCardList.aspx");
        }

        protected void cgvScoreCardMetric_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (e.NewEditIndex < 0 || e.NewEditIndex >= cgvScoreCardMetric.DataKeys.Count) return;
            string redir;
            if (MyDashboardMode)
            {
                redir = "ScoreCardDashboardEdit.aspx?ScoreCardDashboardID=" + cgvScoreCardMetric.DataKeys[e.NewEditIndex].Value.ToString();
            }
            else
            {
                redir = "ScoreCardMetricEdit.aspx?ScoreCardMetricID=" + cgvScoreCardMetric.DataKeys[e.NewEditIndex].Value.ToString();
            }
            Response.Redirect(redir);
        }

        protected void cgvScoreCardMetric_Action(object sender, Micajah.Common.WebControls.CommonGridViewActionEventArgs e)
        {
            if (e.Action != Micajah.Common.WebControls.CommandActions.Add) return;
            string redir;
            if (MyDashboardMode) redir = "ScoreCardDashboardEdit.aspx";
            else redir = "ScoreCardMetricEdit.aspx?ScoreCardID=" + ScoreCardID;
            Response.Redirect(redir);
        }

        protected void mfScoreCard_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;
            e.Values["UserId"] = user.UserId;
        }
    }
}
