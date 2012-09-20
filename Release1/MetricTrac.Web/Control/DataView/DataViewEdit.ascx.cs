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
    public partial class DataViewEdit : System.Web.UI.UserControl
    {
        public string RedirectListUrl
        {
            get { return (string)ViewState["RedirectListUrl"]; }
            set { ViewState["RedirectListUrl"] = value; }
        }

        public Guid DataViewTypeID
        {
            get
            {
                object o = ViewState["DataViewTypeID"];
                if(!(o is Guid)) return Guid.Empty;
                return (Guid)o;
            }
            set
            {
                ViewState["DataViewTypeID"] = value;
            }
        }

        Guid? mDataViewListID;
        Guid DataViewListID
        {
            get
            {
                if (mDataViewListID != null) return (Guid)mDataViewListID;
                mDataViewListID = Guid.Empty;
                string s = HttpContext.Current.Request.QueryString["DataViewListID"];
                if (string.IsNullOrEmpty(s)) return Guid.Empty;
                try { mDataViewListID = new Guid(s); }
                catch { }
                return (Guid)mDataViewListID;
            }
        }

        MetricTrac.Utils.DataViewConfig mDVC;
        public MetricTrac.Utils.DataViewConfig DVC
        {
            get
            {
                if (mDVC != null) return mDVC;
                mDVC = MetricTrac.Utils.DataViewConfig.Get(DataViewTypeID);
                return mDVC;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (DataViewListID != Guid.Empty)
            {
                dsDataView.WhereParameters.Add("DataViewListID", System.Data.DbType.Guid, DataViewListID.ToString());
                mfDataView.ChangeMode(DetailsViewMode.Edit);
            }
        }

        public class ColumnInfo
        {
            public Guid DataViewTypeID { get; set; }
        }
        public System.Collections.ICollection dsColumn_Select()
        {
            return DVC.ReferenceEntityList;
        }

        protected void mfDataView_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            e.Values.Add("DataViewTypeID", DataViewTypeID);
        }

        protected void mfDataView_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
        }

        private void RedirectToGrid()
        {
            //string url = "DataViewList.aspx?DataViewTypeID=" + DataViewTypeID;
            Response.Redirect(RedirectListUrl);
        }

        protected void mfDataView_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfMDataView_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfDataView_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfDataView_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        private void ClearList(Guid DataViewListID, int DataViewColumnListTypeID)
        {
            List<string> EmptyList = new List<string>();
            MetricTrac.Bll.DataViewList.SaveColumns(DataViewTypeID, EmptyList, DataViewColumnListTypeID, DataViewListID);
        }

        private void SaveList(Guid DataViewListID, string id, int DataViewColumnListTypeID)
        {
            MetricTrac.Control.DataView.ColumnListSelect vs = (MetricTrac.Control.DataView.ColumnListSelect)mfDataView.FindControl(id);
            MetricTrac.Bll.DataViewList.SaveColumns(DataViewTypeID, vs.TableColumns, DataViewColumnListTypeID, DataViewListID);
        }

        private void SaveWhere(Guid DataViewListID, string id, int DataViewColumnListTypeID)
        {
            var wsWhere = (MetricTrac.Control.DataView.WhereListSelect)mfDataView.FindControl(id);
            var l = wsWhere.WhereCriteria;
            MetricTrac.Bll.DataViewWhere.SaveWhere(DataViewTypeID, l, wsWhere.WhereCondition, DataViewColumnListTypeID, DataViewListID);
        }

        private void SaveLists(Guid DataViewListID, bool Slave)
        {
            SaveList(DataViewListID, "vsSelect", 1);
            SaveList(DataViewListID, "vsGroupBy", 3);
            SaveList(DataViewListID, "vsOrderBy", 4);

            if (Slave)
            {
                SaveList(DataViewListID, "vsSelectSlave", 11);
                SaveList(DataViewListID, "vsGroupBySlave", 13);
                SaveList(DataViewListID, "vsOrderBySlave", 14);
            }
            else
            {
                ClearList(DataViewListID, 11);
                ClearList(DataViewListID, 13);
                ClearList(DataViewListID, 14);
            }

            SaveWhere(DataViewListID, "wsWhere", 2);
        }

        protected void dsDataView_Updated(object sender, LinqDataSourceStatusEventArgs e)
        {
            MetricTrac.Bll.DataViewList dv = (MetricTrac.Bll.DataViewList)e.Result;
            SaveLists(dv.DataViewListID, dv.Slave);
        }

        protected void dsDataView_Inserted(object sender, LinqDataSourceStatusEventArgs e)
        {
            MetricTrac.Bll.DataViewList dv = (MetricTrac.Bll.DataViewList)e.Result;
            SaveLists(dv.DataViewListID, dv.Slave);
        }

        void BindColumnList(string id, List<string> d)
        {
            MetricTrac.Control.DataView.ColumnListSelect vs = (MetricTrac.Control.DataView.ColumnListSelect)mfDataView.FindControl(id);
            vs.TableColumns = d;
        }

        void BindWhere(string id, List<MetricTrac.Bll.DataViewWhere.Extend> w, string WhereCondition)
        {
            MetricTrac.Control.DataView.WhereListSelect wsWhere = (MetricTrac.Control.DataView.WhereListSelect)mfDataView.FindControl(id);
            wsWhere.WhereCriteria = w;
            wsWhere.WhereCondition = WhereCondition;
        }

        void HideList(string id)
        {
            var c = mfDataView.FindControl(id);
            var tr = (TableRow)c.Parent.Parent;
            tr.Style.Add(HtmlTextWriterStyle.Display, "none");
        }

        private void HideSlave()
        {
            HideList("vsSelectSlave");
            HideList("vsGroupBySlave");
            HideList("vsOrderBySlave");
        }

        protected void mfDataView_DataBound(object sender, EventArgs e)
        {
            var r = MetricTrac.Bll.DataViewList.Get(DataViewListID);
            if (r == null)
            {
                HideSlave();
                return;
            }

            BindColumnList("vsSelect", r.SelectList);
            BindColumnList("vsGroupBy", r.GroupByList);
            BindColumnList("vsOrderBy", r.OrderByList);
            BindWhere("wsWhere", r.WhereList, r.WhereCondition);

            if (r.Slave)
            {
                BindColumnList("vsSelectSlave", r.SelectSlaveList);
                BindColumnList("vsGroupBySlave", r.GroupBySlaveList);
                BindColumnList("vsOrderBySlave", r.OrderBySlaveList);
            }
            else
            {
                HideSlave();
            }
        }
    }
}
