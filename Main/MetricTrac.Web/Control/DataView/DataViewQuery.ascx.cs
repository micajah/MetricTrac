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
    public partial class DataViewQuery : System.Web.UI.UserControl
    {
        public Guid DataViewTypeID
        {
            get { return (Guid)ViewState["DataViewTypeID"]; }
            set { ViewState["DataViewTypeID"] = value; }
        }
        private bool RepeaterIsFilled;
        public Guid? DataViewListID
        {
            get { return (Guid?)ViewState["DataViewListID"]; }
            set
            {
                ViewState["DataViewListID"] = value;
                RepeaterIsFilled = true;
                if (value != null) FillRepeaters();
            }
        }
        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (!RepeaterIsFilled && DataViewListID != null)
            {
                FillRepeaters();
            }
        }

        private Repeater BindRepeater(RepeaterItem it, string ID, object ds)
        {
            Repeater r = (Repeater)it.FindControl(ID);
            r.DataSource = ds;
            r.DataBind();
            return r;
        }

        private void FillRepeaters()
        {
            MetricTrac.Bll.DataViewList.Extend dv;
            MetricTrac.Utils.DataViewConfig c = MetricTrac.Utils.DataViewConfig.Get(DataViewTypeID);
            List<MetricTrac.Utils.DataViewConfig.MasterGroup> MasterGroupList;
            DataSet ds = c.GenerateSQL((Guid)DataViewListID, out dv, out MasterGroupList);

            rMasterGroup.DataSource = MasterGroupList;
            rMasterGroup.DataBind();
        }

        protected void rMasterGroup_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            MetricTrac.Utils.DataViewConfig.MasterGroup mg = (MetricTrac.Utils.DataViewConfig.MasterGroup)e.Item.DataItem;
            if (mg == null) return;
            BindRepeater(e.Item, "rMasterGroupTree", mg.GroupTree);
            BindRepeater(e.Item, "rMasterRecordHeader", mg.MasterHeaderList);
            BindRepeater(e.Item, "rMasterRecord", mg.MasterRecordList);
            BindRepeater(e.Item, "rSlaveGroup", mg.SlaveGroupList);
        }

        protected void rMasterRecord_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            MetricTrac.Utils.DataViewConfig.MasterRecord mr = (MetricTrac.Utils.DataViewConfig.MasterRecord)e.Item.DataItem;
            if (mr == null) return;
            BindRepeater(e.Item, "rMasterValue", mr.MasterValueList);
            BindRepeater(e.Item, "rSlaveRecord", mr.SlaveRecordList);
        }

        protected void rSlaveRecord_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            MetricTrac.Utils.DataViewConfig.SlaveRecord sr = (MetricTrac.Utils.DataViewConfig.SlaveRecord)e.Item.DataItem;
            if (sr == null) return;
            BindRepeater(e.Item, "rSlaveVlue", sr.SlaveValueList);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricInput.css");
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/UnderReview.css");
        }  


        
    }
}
