using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class OrgPIFList : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            lbOrg.Text = MetricTrac.Bll.Mc_EntityNode.GetHtmlFullName(cPIFList.OrgLocationID);
        }
    }
}
