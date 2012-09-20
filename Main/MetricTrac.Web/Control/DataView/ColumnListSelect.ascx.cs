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
    public partial class ColumnListSelect : System.Web.UI.UserControl
    {
        const int RowCount=15;

        Guid? mDataViewTypeID;
        protected Guid DataViewTypeID
        {
            get
            {
                if (mDataViewTypeID != null) return (Guid)mDataViewTypeID;
                mDataViewTypeID = Guid.Empty;
                string s = HttpContext.Current.Request.QueryString["DataViewTypeID"];
                if (string.IsNullOrEmpty(s))
                {
                    MetricTrac.Bll.DataViewList dvl = MetricTrac.Bll.DataViewList.Get(DataViewListID);
                    if (dvl == null || dvl.DataViewTypeID == Guid.Empty) return Guid.Empty;
                    mDataViewTypeID = dvl.DataViewTypeID;
                    return dvl.DataViewTypeID;
                }
                try { mDataViewTypeID = new Guid(s); }
                catch { }
                return (Guid)mDataViewTypeID;
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

        public List<string> TableColumns { get; set; }

        void HideRow(int n)
        {
            GetListManager(n).HideRow = true;
        }
        void ShowRow(int n)
        {
            GetListManager(n).HideRow = false;
        }

        ListManager GetListManager(int n)
        {
            return (ListManager)this.FindControl("lmColumn"+n);
        }

        void HideListManager(int n)
        {
            GetListManager(n).HideButton = true;
        }

        void ShowListManager(int n)
        {
            GetListManager(n).HideButton = false;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                TableColumns = new List<string>();
                int ActiveRowsCount=0;
                for (int i = RowCount - 1; i >= 0; i--)
                {
                    ListManager lm = GetListManager(i);
                    if (!lm.HideButton && !lm.HideRow)
                    {
                        ActiveRowsCount = i + 1;
                        break;
                    }
                }
                for (int i = 0; i < ActiveRowsCount; i++)
                {
                    ColumnSelect cs = (ColumnSelect)FindControl("ColumnSelect" + i);
                    string TableName;
                    string CoulumnName;
                    string TextColumnName;
                    string SelectControl;
                    if (!cs.SelectedField(DataViewTypeID, out TableName, out CoulumnName, out TextColumnName, out SelectControl)) continue;

                    TableColumns.Add(TableName + "," + CoulumnName);
                }
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (TableColumns == null || TableColumns.Count == 0)
            {
                for (int i = 3; i < RowCount; i++) HideRow(i);
                HideListManager(0);
                HideListManager(1);
                ShowListManager(2);
            }
            else
            {
                for (int i = TableColumns.Count; i < RowCount; i++)
                {
                    HideRow(i);
                }
                for (int i = 0; i < TableColumns.Count-1; i++)
                {
                    HideListManager(i);
                }
                ShowListManager(TableColumns.Count - 1);

                for (int i = 0; i < TableColumns.Count; i++)
                {
                    ColumnSelect cs = (ColumnSelect)this.FindControl("ColumnSelect" + i);
                    cs.SelectedValue = TableColumns[i];
                }
            }
        }
    }
}
