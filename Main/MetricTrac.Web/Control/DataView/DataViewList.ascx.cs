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

namespace MetricTrac.Control.DataView
{    
    public partial class DataViewList : System.Web.UI.UserControl
    {
        public string RedirectEditUrl
        {
            get
            {
                object o = ViewState["RedirectEditUrl"];
                return (string)o;
            }
            set
            {
                ViewState["RedirectEditUrl"] = value;
            }
        }

        public Guid DataViewTypeID
        {
            get
            {
                object o = ViewState["DataViewTypeID"];
                if (!(o is Guid)) return Guid.Empty;
                return (Guid)o;
            }
            set
            {
                ViewState["DataViewTypeID"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MetricTrac.Utils.DataViewConfig c = MetricTrac.Utils.DataViewConfig.Get(DataViewTypeID);
            if (c == null)
            {
                phError.Visible = true;
                phGrid.Visible = false;
                return;
            }

            phError.Visible = false;
            phGrid.Visible = true;
            lbTitle.Text = c.Name;

        }

        protected void dsDataViewList_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = MetricTrac.Bll.DataViewList.List(DataViewTypeID);
        }

        protected void cgvDataViewList_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (e.NewEditIndex < 0 || e.NewEditIndex >= cgvDataViewList.DataKeys.Count) return;
            Response.Redirect(RedirectEditUrl + "?DataViewListID=" + cgvDataViewList.DataKeys[e.NewEditIndex].Value.ToString());
        }

        protected void cgvDataViewList_Action(object sender, Micajah.Common.WebControls.CommonGridViewActionEventArgs e)
        {
            if (e.Action != Micajah.Common.WebControls.CommandActions.Add) return;
            Response.Redirect(RedirectEditUrl + "?DataViewTypeID=" + DataViewTypeID);
        }
    }
}
