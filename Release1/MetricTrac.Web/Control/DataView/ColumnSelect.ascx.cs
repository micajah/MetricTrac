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
    public partial class ColumnSelect : System.Web.UI.UserControl
    {
        public Telerik.Web.UI.RadComboBox ColumnSelectComboBox
        {
            get { return rcbColumn; }
        }

        private Guid mDataViewTypeID = Guid.Empty;
        public Guid DataViewTypeID
        {
            get
            {
                if (mDataViewTypeID != Guid.Empty) return mDataViewTypeID;
                System.Web.UI.Control c = this;
                while (c != null)
                {
                    c = c.Parent;
                    if (!(c is MetricTrac.Control.DataView.DataViewEdit)) continue;
                    MetricTrac.Control.DataView.DataViewEdit e = (MetricTrac.Control.DataView.DataViewEdit)c;
                    mDataViewTypeID = e.DataViewTypeID;
                    return mDataViewTypeID;
                }
                throw new Exception("Can not parse DataViewTypeID");
            }
        }

        private MetricTrac.Utils.DataViewConfig data { get {
            return MetricTrac.Utils.DataViewConfig.Get(DataViewTypeID);
        } }

        List<string> mSelectControlList = new List<string>();
        public List<string> SelectControlList { get { return mSelectControlList; } }

        private string mSelectedValue;
        public string SelectedValue
        {
            get
            {
                return mSelectedValue;
            }
            set
            {
                mSelectedValue = value;
                foreach (Telerik.Web.UI.RadComboBoxItem i in rcbColumn.Items)
                {
                    i.Selected = i.Value == value;
                }
            }
        }
        public void Select(string TableName, string ColumnName)
        {
            SelectedValue = TableName+","+ColumnName;
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            if (data == null) lbTest.Text = "null";
            else lbTest.Text = data.DataViewTypeID.ToString();

            string sv = rcbColumn.SelectedValue;
            if (!IsPostBack && mSelectedValue != null) sv = mSelectedValue;

            List<ColumnInfo> ds = new List<ColumnInfo>();
            ds.Add(new ColumnInfo());
            mSelectControlList.Clear();

            foreach (var r in data.ReferenceEntityList)
            {
                foreach (var c in r.UseFieldList)
                {
                    bool HideIdentifier = HideNameColumn && c.ReadableName=="Identifier";
                    ColumnInfo ci = new ColumnInfo()
                    {
                        ReadebleTable = r.ReadableName,
                        ReadebleColumn = HideIdentifier ? string.Empty : c.ReadableName,
                        SqlTable = r.SqlName,
                        SqlColumn = c.SqlName,
                        id = r.SqlName + "," + c.SqlName,
                        text = r.ReadableName.Replace("&nbsp;", " ") + (HideIdentifier ? string.Empty : (": " + c.ReadableName.Replace("&nbsp;", " ")))                        
                    };
                    ds.Add(ci);
                    if (!string.IsNullOrEmpty(c.SelectControl) && !mSelectControlList.Contains(c.SelectControl))
                        mSelectControlList.Add(c.SelectControl);
                }
            }

            rcbColumn.DataSource = ds;
            rcbColumn.DataBind();
            if(sv!=null)
                foreach (Telerik.Web.UI.RadComboBoxItem i in rcbColumn.Items)
                {
                    if (i.Value == sv)
                    {
                        i.Selected = true;
                        break;
                    }
                }
        }

        public class ColumnInfo
        {
            public string ReadebleTable { get; set; }
            public string ReadebleColumn { get; set; }
            public string SqlTable { get; set; }
            public string SqlColumn { get; set; }
            public string id { get; set; }
            public string text { get; set; }
        }

        public bool AutoPostBack
        {
            get { return rcbColumn.AutoPostBack; }
            set { rcbColumn.AutoPostBack = value; }
        }

        public bool HideNameColumn
        {
            get
            {
                object o = ViewState["HideNameColumn"];
                if(!(o is bool)) return false;
                return (bool)o;
            }
            set
            {
                ViewState["HideNameColumn"] = value;
            }
        }


        public bool SelectedField(Guid DataViewTypeID, out string TableName, out string CoulumnName, out string TextColumnName, out string SelectControl)
        {
            TableName = null;
            CoulumnName = null;
            TextColumnName = null;
            SelectControl = null;

            string s = rcbColumn.SelectedValue;
            if (string.IsNullOrEmpty(s)) return false;

            string[] ss = s.Split(',');
            if (ss.Length != 2) return false;

            TableName = ss[0];
            CoulumnName = ss[1];

            MetricTrac.Utils.DataViewConfig c= MetricTrac.Utils.DataViewConfig.Get(DataViewTypeID);

            foreach(var t in c.ReferenceEntityList)
            {
                if (t.SqlName != TableName) continue;
                foreach (var f in t.UseFieldList)
                {
                    if(f.SqlName!=CoulumnName) continue;
                    TextColumnName = t.SqlNameFieldName;
                    SelectControl = f.SelectControl;
                    break;
                }
            }


            return true;
        }

        public event Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler SelectedIndexChanged;
        protected void  rcbColumn_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            SelectedValue = e.Value;
            if (SelectedIndexChanged != null) SelectedIndexChanged(o,e); 
        }
    }
}
