using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;

namespace MetricTrac
{
    public partial class DataValueChart : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Micajah.Common.Pages.MasterPage mp = Page.Master as Micajah.Common.Pages.MasterPage;
            mp.VisibleApplicationLogo = false;
            mp.VisibleBreadcrumbs = false;
            mp.VisibleFooter = false;
            mp.VisibleFooterLinks = false;
            mp.VisibleHeader = false;
            mp.VisibleHeaderLinks = false;
            mp.VisibleLeftArea = false;
            mp.VisibleMainMenu = false;
            mp.VisibleHeaderLogo = false;
            mp.VisibleSearchControl = false;
        }
    }
}
