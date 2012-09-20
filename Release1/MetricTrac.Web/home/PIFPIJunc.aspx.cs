using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;

namespace MetricTrac
{
    public partial class PIFPIJunc : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            mainPerformanceIndicatorList.GCAID = GCAFilter.GCAID == null ? Guid.Empty : (Guid)GCAFilter.GCAID;
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            string _AddedPerformanceIndicators = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] != null)
                _AddedPerformanceIndicators = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs];              
            if (String.IsNullOrEmpty(_AddedPerformanceIndicators))
                _AddedPerformanceIndicators = mainPerformanceIndicatorList.SelectedIndicators;
            else 
                _AddedPerformanceIndicators += "|" + mainPerformanceIndicatorList.SelectedIndicators;
            Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] = _AddedPerformanceIndicators;
            RedirectToEditPage();
        }

        protected void lnkCancel_Click(object sender, EventArgs e)
        {            
            RedirectToEditPage();
        }

        private void RedirectToEditPage()
        {
            Response.Redirect(String.Format("PIFEdit.aspx?PerformanceIndicatorFormID={0}", mainPerformanceIndicatorList.PerformanceIndicatorFormID.ToString()));
        }
    }
}