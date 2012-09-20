using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Math.Parser;

namespace MetricTrac
{
    public partial class CalculateMetric : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            lblResult.Text = Bll.ApplicationLog.GetLastAppMessage("Calc process");
        }

        protected void btnCalculate_Click(object sender, EventArgs e)
        {
            lblResult.Text = "Sorry, calculation process now is running only in separate thread and is called by timer.<br/> By default, this page shows time and result of previous running of it.";            
        }

        protected void btnRecalc_Click(object sender, EventArgs e)
        {
            Bll.MetricValue.MakeAllInputsDirty();
            lblResult.Text = "All values are marked as need recalc. They will be calculated by separate thread. <br /> Manual run of calc process is disabled.";            
        }
    }
}