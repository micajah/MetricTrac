using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;
using Telerik.Web.UI;

namespace MetricTrac
{
    public partial class MetricFormulaEdit : System.Web.UI.Page
    {
        private Guid mMetricID;
        protected Guid MetricID
        {
            get
            {
                if (mMetricID != Guid.Empty) return mMetricID;
                string strMetricID = Request.QueryString["MetricID"];
                if (string.IsNullOrEmpty(strMetricID)) return mMetricID;
                try
                {
                    mMetricID = new Guid(strMetricID);
                }
                catch { }
                return mMetricID;
            }
        }
        

        protected void Page_Load(object sender, EventArgs e)
        {   
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleApplicationLogo = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleBreadcrumbs = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleFooter = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleFooterLinks = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeader = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeaderLinks = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleLeftArea = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleMainMenu = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeaderLogo = false;            
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleSearchControl = false;
            rdpBeginDate.DateInput.DateFormat = rdpBeginDate.Culture.DateTimeFormat.ShortDatePattern;
            rdpEndDate.DateInput.DateFormat = rdpEndDate.Culture.DateTimeFormat.ShortDatePattern;          
        }

        private string ErrorMessage
        {
            set { ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage = value; }
            get { return ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage; }
        }

        protected void cgvFormula_RowDataBound(object sender, GridViewRowEventArgs e)
        {            
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            Bll.MetricFormula.Extend rData = (Bll.MetricFormula.Extend)e.Row.DataItem;
            e.Row.Cells[0].Text = rData.BeginDate.ToShortDateString();
            if (rData.EndDate != null)
                e.Row.Cells[1].Text = ((DateTime)rData.EndDate).ToShortDateString();
            else e.Row.Cells[1].Text = "Non given";
            e.Row.Cells[4].Text = rData.ChangeDate.ToShortDateString();
        }

        protected void ldsMetricFormula_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = Bll.MetricFormula.GetFormulaHistory(MetricID);
        }
    }
}