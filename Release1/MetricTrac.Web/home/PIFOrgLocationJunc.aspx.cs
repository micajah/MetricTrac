using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class PIFOrgLocationJunc : System.Web.UI.Page
    {
        private Guid mPIFID;
        protected Guid PerformanceIndicatorFormID
        {
            get
            {
                if (mPIFID != Guid.Empty) return mPIFID;
                try
                {
                    mPIFID = new Guid(Request.QueryString["PerformanceIndicatorFormID"]);
                }
                catch
                {
                    mPIFID = Guid.Empty;
                }
                return mPIFID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;
                OrgTree.CustomRootNodeText = user.SelectedOrganization.Name;
                OrgTree.EntityNodeId = PerformanceIndicatorFormID;
                OrgTree.LoadTree();
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            OrgTree.SaveTree();
            RedirectToEditPage();
        }

        protected void lnkCancel_Click(object sender, EventArgs e)
        {            
            RedirectToEditPage();
        }

        private void RedirectToEditPage()
        {
            Response.Redirect(String.Format("PIFEdit.aspx?PerformanceIndicatorFormID={0}", PerformanceIndicatorFormID.ToString()));
        }

    }
}
