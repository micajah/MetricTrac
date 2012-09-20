using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Micajah.Common.WebControls;

namespace MetricTrac
{
    public partial class PerformanceIndicatorEdit : System.Web.UI.Page
    {
        private Guid? mPerformanceIndicatorID;
        protected Guid PerformanceIndicatorID
        {
            get
            {
                if (mPerformanceIndicatorID != null) return (Guid)mPerformanceIndicatorID;
                mPerformanceIndicatorID = Guid.Empty;
                string strPerformanceIndicatorID = Request.QueryString["PerformanceIndicatorID"];
                if (string.IsNullOrEmpty(strPerformanceIndicatorID)) return Guid.Empty;
                try
                {
                    mPerformanceIndicatorID = new Guid(strPerformanceIndicatorID);
                }
                catch { }
                return (Guid)mPerformanceIndicatorID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (PerformanceIndicatorID != Guid.Empty)
            {
                ldsPerformanceIndicator.WhereParameters.Add("PerformanceIndicatorID", System.Data.DbType.Guid, PerformanceIndicatorID.ToString());
                mfPerformanceIndicator.ChangeMode(DetailsViewMode.Edit);
            }
        }

        protected void mfPerformanceIndicator_DataBound(object sender, EventArgs e)
        {
        }

        protected void mfPerformanceIndicator_DataBinding(object sender, EventArgs e)
        {
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (rapMetric.IsAjaxRequest) cMetricList.Rebind();
        }

        private void RedirectToGrid()
        {
            Response.Redirect("PerformanceIndicatorList.aspx");
        }

        protected void mfPerformanceIndicator_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfPerformanceIndicator_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            cMetricList.SaveInsert(InsertedPerformanceIndicator.PerformanceIndicatorID);
            RedirectToGrid();
        }

        protected void mfPerformanceIndicator_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfPerformanceIndicator_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        MetricTrac.Bll.PerformanceIndicator InsertedPerformanceIndicator;
        protected void ldsPerformanceIndicator_Inserting(object sender, LinqDataSourceInsertEventArgs e)
        {
            InsertedPerformanceIndicator = (MetricTrac.Bll.PerformanceIndicator)e.NewObject;
        }

        protected void mfPerformanceIndicator_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values["SortCode"] = GetSortCode(e.Values["Code"].ToString());
        }

        protected void mfPerformanceIndicator_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            e.NewValues["SortCode"] = GetSortCode(e.NewValues["Code"].ToString());
        }

        private string GetSortCode(string Code)
        {
            int CastLength = 25;
            string result = String.Empty;
            if (!String.IsNullOrEmpty(Code))
            {
                int i = Code.Length - 1;
                while (Code[i] >= '0' && Code[i] <= '9' && i >= 0)
                {
                    i--;
                    if (i < 0) break;
                }
                if (i < Code.Length - 1)
                {
                    int InsertLength = CastLength - Code.Length;
                    result = Code.Substring(0, i + 1);
                    for (int k = 1; k <= InsertLength; k++)
                        result += "0";
                    result += Code.Substring(i + 1, Code.Length - i - 1);
                }
            }
            return result;
        }
    }
}
