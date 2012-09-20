using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class PerformanceIndicatorList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            MainPerformanceIndicatorList.GCAID = GCAFilter.GCAID == null ? Guid.Empty : (Guid)GCAFilter.GCAID;
            MainPerformanceIndicatorList.SectorID = String.IsNullOrEmpty(ddlSector.SelectedValue) ? -1 : int.Parse(ddlSector.SelectedValue);
        }
    }
}
