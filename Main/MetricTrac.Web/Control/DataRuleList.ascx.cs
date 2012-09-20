using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;
using System.Data;
using System.Data.Linq;
using System.Collections;

namespace MetricTrac.Control
{    
    public partial class DataRuleList : System.Web.UI.UserControl
    {
        public int DataRuleTypeID { get; set; }
        public string RedirectEditUrl { get; set; }

        protected int DataSourceCount;

        protected void Page_Load(object sender, EventArgs e)
        {
            ((Micajah.Common.WebControls.HyperLinkField)cgvDataRule.Columns[0]).DataNavigateUrlFormatString = RedirectEditUrl + "?DataRuleID={0}";
        }

        protected void ldsCollectorRule_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            List<MetricTrac.Bll.DataRule.Extend> r = MetricTrac.Bll.DataRule.List(DataRuleTypeID);
            DataSourceCount = r.Count;
            e.Result = r;
        }

        protected void cgvDataRule_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (e.NewEditIndex < 0 || e.NewEditIndex >= cgvDataRule.DataKeys.Count) return;
            Response.Redirect(RedirectEditUrl + "?DataRuleID=" + cgvDataRule.DataKeys[e.NewEditIndex].Value.ToString());
        }

        protected void cgvDataRule_Action(object sender, Micajah.Common.WebControls.CommonGridViewActionEventArgs e)
        {
            if (e.Action != Micajah.Common.WebControls.CommandActions.Add) return;
            Response.Redirect(RedirectEditUrl);
        }

        protected void ibMove_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandArgument == null) return;
            string s = e.CommandArgument.ToString().ToLower();
            if (s == string.Empty) return; ;

            Guid ID;
            try { ID = new Guid(s); }
            catch { return; }
            MetricTrac.Bll.DataRule.Move(ID, e.CommandName.ToString().ToLower() == "up", DataRuleTypeID);

            cgvDataRule.DataBind();
        }

        protected string GetImgUrl(object o)
        {
            return "~/images/Navigation/up.gif";
        }

        protected void ldsCollectorRule_Deleting(object sender, LinqDataSourceDeleteEventArgs e)
        {
            Bll.DataRule.Delete(((Bll.DataRule)e.OriginalObject).DataRuleID);
            e.Cancel = true;
        }
    }
}