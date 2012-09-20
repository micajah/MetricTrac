using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;
using Micajah.FileService.WebControls;
using Telerik.Web.UI;

namespace MetricTrac
{
    public partial class PerformanceIndicatorInfo : System.Web.UI.Page
    {
        private Guid mPerformanceIndicatorID;
        protected Guid PerformanceIndicatorID
        {
            get
            {
                if (mPerformanceIndicatorID != Guid.Empty) return mPerformanceIndicatorID;
                string strPerformanceIndicatorID = Request.QueryString["PerformanceIndicatorID"];
                if (string.IsNullOrEmpty(strPerformanceIndicatorID)) return mPerformanceIndicatorID;
                try
                {
                    mPerformanceIndicatorID = new Guid(strPerformanceIndicatorID);
                }
                catch { }
                return mPerformanceIndicatorID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // change to popup window mode
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
            //----------------------------
            Bll.PerformanceIndicator.Extend pi = Bll.PerformanceIndicator.Get(PerformanceIndicatorID);
            lblName.Text = String.IsNullOrEmpty(pi.Name) ? "---" : pi.Name;
            lblCode.Text = String.IsNullOrEmpty(pi.Code) ? "---" : pi.Code;
            lblAlias.Text = String.IsNullOrEmpty(pi.Alias) ? "---" : pi.Alias;
            lblGCA.Text = String.IsNullOrEmpty(pi.GCAName) ? "---" : pi.GCAName;
            lblSector.Text = String.IsNullOrEmpty(pi.SectorName) ? "---" : pi.SectorName;
            lblRequirement.Text = String.IsNullOrEmpty(pi.RequirementName) ? "---" : pi.RequirementName;
            lblDescription.Text = String.IsNullOrEmpty(pi.Description) ? "---" : pi.Description;
            lblHelp.Text = String.IsNullOrEmpty(pi.Help) ? "---" : pi.Help;            
        }
    }
}
