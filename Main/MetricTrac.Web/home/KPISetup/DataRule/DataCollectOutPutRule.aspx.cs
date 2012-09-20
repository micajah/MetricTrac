using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class DataCollectOutPutRule : System.Web.UI.Page
    {
        private Guid mRuleID;
        protected Guid RuleID
        {
            get
            {
                if (mRuleID != Guid.Empty) return mRuleID;
                string strID = Request.QueryString["RuleID"];
                if (string.IsNullOrEmpty(strID)) return mRuleID;
                try
                {
                    mRuleID = new Guid(strID);
                }
                catch { }
                return mRuleID;
            }
        }

        MetricTrac.Bll.DataCollectorRuleOut mRuleOut;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && RuleID != Guid.Empty)
            {
                Utils.MetricUtils.InitLinqDataSources(ldsDataCollectorRuleOut);
                ldsDataCollectorRuleOut.WhereParameters.Add("RuleId", System.Data.DbType.Guid, RuleID.ToString());

                mRuleOut = MetricTrac.Bll.DataCollectorRuleOut.Get(RuleID);
                if (mRuleOut != null)
                {
                    mfDataCollector.ChangeMode(DetailsViewMode.Edit);
                }
                else
                {
                }
            }

            ldsDataCollectorRuleOut.InsertParameters.Add("RuleId", System.Data.DbType.Guid, RuleID.ToString());
        }

        protected void ldsDataCollectorRuleOut_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (mRuleOut != null) e.Result = mRuleOut;
        }

        private void RedirectToGrid()
        {
            string RedirectUrl = "~/Resources.Micajah.Common/Pages/Admin/RulesEngine.aspx";
            if (RuleID != Guid.Empty)
            {
                try
                {
                    Micajah.Common.Bll.Rule rule = Micajah.Common.Bll.Rule.Create(RuleID);
                    Guid RuleEngineId = rule.RulesEngineId;
                    RedirectUrl = "~/Resources.Micajah.Common/Pages/Admin/InstanceRules.aspx?RuleEngineId=" + RuleEngineId;
                }
                catch { }
            }
            Response.Redirect(RedirectUrl);
        }

        protected void mfDataCollector_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfMDataCollector_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfDataCollector_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfDataCollector_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

    }
}
