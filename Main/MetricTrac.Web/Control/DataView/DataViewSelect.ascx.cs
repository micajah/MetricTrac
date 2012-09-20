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
    public partial class DataViewSelect : System.Web.UI.UserControl
    {
        public event EventHandler Apply;

        public string EditUrl
        {
            get { return (string)ViewState["EditUrl"]; }
            set { ViewState["EditUrl"] = value; }
        }
        public string ListUrl
        {
            get { return (string)ViewState["ListUrl"]; }
            set { ViewState["ListUrl"] = value; }
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

        Guid? mDataViewListID;
        bool DataViewListIDSelected;
        public Guid? DataViewListID
        {
            get
            {
                return mDataViewListID;
            }
            set
            {
                mDataViewListID = value;
                DataViewListIDSelected = true;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string s = ddlDataView.SelectedValue;
            if (!DataViewListIDSelected && !string.IsNullOrEmpty(s))
            {
                mDataViewListID = new Guid(s);
            }
        }
        protected void Page_Prerender(object sender, EventArgs e)
        {
            var l = MetricTrac.Bll.DataViewList.List(DataViewTypeID);
            ddlDataView.DataSource = l;
            ddlDataView.DataBind();
            ddlDataView.Items.Insert(0, string.Empty);

            string v = mDataViewListID==null?string.Empty:mDataViewListID.ToString();
            foreach (ListItem li in ddlDataView.Items)
            {
                li.Selected = li.Value == v;
            }
        }

        protected void bApply_Click(object sender, EventArgs e)
        {
            if (Apply != null) Apply(this, new EventArgs());
        }

        protected void lbEdit_Click(object sender, EventArgs e)
        {
            if (DataViewListID == null) lbAdd_Click(null, new EventArgs());
            string s = EditUrl;
            s+=EditUrl.Contains('?')?"&":"?";
            s+="DataViewListID="+DataViewListID;
            Response.Redirect(s);
        }

        protected void lbAdd_Click(object sender, EventArgs e)
        {
            Response.Redirect(EditUrl);
        }

        /*protected void lbManage_Click(object sender, EventArgs e)
        {
            Response.Redirect(ListUrl);
        }*/
    }
}
