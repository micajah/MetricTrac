using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac.Control
{
    public partial class MetricValueHistoryLog : System.Web.UI.UserControl
    {        
        public Guid MetricValueID {get; set;}        

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void ldsMetricValueChangeLog_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = Bll.MetricValueChangeLog.GetLog(MetricValueID);
        }
        //<mits:TextField DataField="Event" HeaderText="Event" />
    }
}