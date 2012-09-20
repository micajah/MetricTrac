using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class DataMinerList : System.Web.UI.Page
    {
        protected void Page_Prerender(object sender, EventArgs e)
        {
            DataViewQuery1.DataViewListID = DataViewSelect1.DataViewListID;
        }
    }
}
