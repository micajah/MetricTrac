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
    public partial class WhereListSelect : System.Web.UI.UserControl
    {
        protected const int RowCount = 15;

        protected class RepeiterData
        {
            public string Name { get; set; }
            public bool First { get; set; }
            public bool Last { get; set; }

            public bool HideManager { get; set; }
            public bool HideRow { get; set; }

            public RepeiterData(string name)
            {
                Name = name;
            }
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

        protected void phValue_Load(object sender, EventArgs e)
        {
            PlaceHolder phValue = (PlaceHolder)sender;
            if (phValue.Controls.Count > 0) return;
            System.Web.UI.HtmlControls.HtmlTableRow tr = (System.Web.UI.HtmlControls.HtmlTableRow)phValue.Parent.Parent.Parent;
            ColumnSelect csWhere = (ColumnSelect)tr.FindControl("csWhere");
            foreach (string SelectControl in csWhere.SelectControlList)
            {
                UserControl c = (UserControl)LoadControl(SelectControl);
                if (!(c is MetricTrac.Utils.IValueSelectControl)) continue;
                System.Reflection.PropertyInfo pi = c.GetType().GetProperty("Width");
                if (pi != null)
                {
                    pi.SetValue(c, new Unit(300, UnitType.Pixel), null);
                }

                Panel p = new Panel();
                p.ID = System.IO.Path.GetFileNameWithoutExtension(SelectControl);
                p.Style.Add("display", "none");
                phValue.Controls.Add(p);

                p.Controls.Add(c);
            }
        }

        Type[] NumericType = new Type[]
            {
                typeof(int),    typeof(long),    typeof(short), 
                typeof(int?),   typeof(long?),   typeof(short?), 
                typeof(Int16),  typeof(Int32),   typeof(Int64),
                typeof(Int16?), typeof(Int32?),  typeof(Int64?),
                typeof(uint),   typeof(ulong),   typeof(ushort), 
                typeof(uint?),  typeof(ulong?),  typeof(ushort?), 
                typeof(UInt16), typeof(UInt32),  typeof(UInt64),
                typeof(UInt16?),typeof(UInt32?), typeof(UInt64?),
                typeof(float),  typeof(decimal), typeof(double),
                typeof(float?), typeof(decimal?),typeof(double?),
                typeof(Single), typeof(Decimal), typeof(Double),
                typeof(Single?),typeof(Decimal?),typeof(Double?)
            };

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            Telerik.Web.UI.RadAjaxManager m = Telerik.Web.UI.RadAjaxManager.GetCurrent(Page);
            foreach (RepeaterItem it in rpWhere.Items)
            {
                ColumnSelect csWhere = (ColumnSelect)it.FindControl("csWhere");
                ConditionSelect Condition = (ConditionSelect)it.FindControl("Condition");
                PlaceHolder phValue = (PlaceHolder)it.FindControl("phValue");
                System.Web.UI.WebControls.TextBox tbValue = (System.Web.UI.WebControls.TextBox)it.FindControl("tbValue");
                Telerik.Web.UI.RadComboBox rcbValue = (Telerik.Web.UI.RadComboBox)it.FindControl("rcbValue");
                Telerik.Web.UI.RadNumericTextBox rnValue = (Telerik.Web.UI.RadNumericTextBox)it.FindControl("rnValue");

                rcbValue.Visible = false;
                rnValue.Visible = false;

                string Table;
                string Column;
                string TextColumn;
                string SelectControl;
                if (!csWhere.SelectedField(DataViewTypeID, out Table, out Column, out TextColumn, out SelectControl)) continue;
                tbValue.Visible = false;

                Type t = typeof(MetricTrac.Bll.LinqMicajahEntitybase).Assembly.GetType("MetricTrac.Bll." + Table);
                if (t == null) continue;

                System.Reflection.PropertyInfo pi = t.GetProperty(Column);
                if (pi == null) continue;

                if (SelectControl != null)
                {
                    Condition.Mode = ConditionSelect.ConditionViewMode.Equal;
                    foreach (Panel p in phValue.Controls)
                    {
                        if (p.ID != System.IO.Path.GetFileNameWithoutExtension(SelectControl)) continue;
                        p.Style["display"] = "block";
                    }

                    continue;
                }

                if (pi.PropertyType == typeof(string))
                {
                    Condition.Mode = ConditionSelect.ConditionViewMode.Like;
                    tbValue.Visible = true;
                    continue;
                }
                if (pi.PropertyType == typeof(Guid) || pi.PropertyType == typeof(Guid?))
                {
                    Condition.Mode = ConditionSelect.ConditionViewMode.Equal;
                    rcbValue.Visible = true;
                    rcbValue.DataValueField = Column;
                    rcbValue.DataTextField = TextColumn;
                    rcbValue.DataSource = MetricTrac.Bll.LinqMicajahDataContext.SelectGuid(Table, Column, TextColumn);
                    rcbValue.DataBind();
                    continue;
                }
                
                foreach (Type nt in NumericType)
                {
                    if (pi.PropertyType == nt)
                    {
                        Condition.Mode = ConditionSelect.ConditionViewMode.Compare;
                        rnValue.Visible = true;
                        continue;
                    }
                }

                Condition.Mode = ConditionSelect.ConditionViewMode.All;
                tbValue.Visible = true;

            }
        }

        protected void rpWhere_PreRender(object sender, EventArgs e)
        {
            Telerik.Web.UI.RadAjaxManager m = Telerik.Web.UI.RadAjaxManager.GetCurrent(Page);
            foreach (RepeaterItem it in rpWhere.Items)
            {
                Panel pValue = (Panel)it.FindControl("pValue");
                Panel pCondition = (Panel)it.FindControl("pCondition");
                ColumnSelect csWhere = (ColumnSelect)it.FindControl("csWhere");

                m.AjaxSettings.AddAjaxSetting(csWhere.ColumnSelectComboBox, pCondition, lpWhere);
                m.AjaxSettings.AddAjaxSetting(csWhere.ColumnSelectComboBox, pValue, lpWhere);
            }
        }

        private string GetValue(RepeaterItem it, string Table, string Column, string SelectControl)
        {
            ConditionSelect Condition = (ConditionSelect)it.FindControl("Condition");
            PlaceHolder phValue = (PlaceHolder)it.FindControl("phValue");
            System.Web.UI.WebControls.TextBox tbValue = (System.Web.UI.WebControls.TextBox)it.FindControl("tbValue");
            Telerik.Web.UI.RadComboBox rcbValue = (Telerik.Web.UI.RadComboBox)it.FindControl("rcbValue");
            Telerik.Web.UI.RadNumericTextBox rnValue = (Telerik.Web.UI.RadNumericTextBox)it.FindControl("rnValue");


            Type t = typeof(MetricTrac.Bll.LinqMicajahEntitybase).Assembly.GetType("MetricTrac.Bll." + Table);
            if (t == null) return null;

            System.Reflection.PropertyInfo pi = t.GetProperty(Column);
            if (pi == null) return null;

            if (SelectControl != null)
            {
                foreach (Panel p in phValue.Controls)
                {
                    if (p.ID != System.IO.Path.GetFileNameWithoutExtension(SelectControl)) continue;
                    if (p.Controls.Count < 1) return null;
                    if(!(p.Controls[0] is MetricTrac.Utils.IValueSelectControl)) return null;
                    MetricTrac.Utils.IValueSelectControl iv = (MetricTrac.Utils.IValueSelectControl)p.Controls[0];
                    if (!iv.IsValueSelected) return null;
                    return iv.SelectedValue.ToString();
                }

                return null;
            }

            if (pi.PropertyType == typeof(string))
            {
                if (tbValue.Text == string.Empty) return null;
                return tbValue.Text;
            }
            if (pi.PropertyType == typeof(Guid) || pi.PropertyType == typeof(Guid?))
            {
                if (string.IsNullOrEmpty(rcbValue.SelectedValue)) return null;
                return rcbValue.SelectedValue;
            }

            foreach (Type nt in NumericType)
            {
                if (pi.PropertyType == nt)
                {
                    if (rnValue.Value == null) return null;
                    return ((double)rnValue.Value).ToString();
                }
            }

            return null;
        }

        private bool IsPrepered;
        private void Prepare()
        {
            if (IsPrepered) return;
            IsPrepered = true;
            mWhereCriteria = new List<MetricTrac.Bll.DataViewWhere.Extend>();
            int ActiveRowsCount = 0;
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
                RepeaterItem it = rpWhere.Items[i];
                ColumnSelect csWhere = (ColumnSelect)it.FindControl("csWhere");
                string TableName;
                string CoulumnName;
                string TextColumnName;
                string SelectControl;
                if (!csWhere.SelectedField(DataViewTypeID, out TableName, out CoulumnName, out TextColumnName, out SelectControl)) continue;
                string v = GetValue(it, TableName, CoulumnName, SelectControl);

                ConditionSelect Condition = (ConditionSelect)it.FindControl("Condition");

                MetricTrac.Bll.DataViewWhere.Extend c = new MetricTrac.Bll.DataViewWhere.Extend()
                {
                    TableName = TableName,
                    ColumnName = CoulumnName,
                    CompareValue = v,
                    DataViewConditionTypeID = Condition.DataViewConditionTypeID
                };
                mWhereCriteria.Add(c);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                for (int i = 1; i < RowCount; i++)
                {
                    HideRow(i);
                }
                if (mWhereCriteriaUpdated && mWhereCriteria.Count > 0)
                {
                    for (int i = 0; i < mWhereCriteria.Count; i++)
                    {
                        var c = mWhereCriteria[i];
                        ShowRow(i);
                        if(i>0) HideListManager(i-1);

                        RepeaterItem it = rpWhere.Items[i];

                        ColumnSelect csWhere = (ColumnSelect)it.FindControl("csWhere");
                        ConditionSelect Condition = (ConditionSelect)it.FindControl("Condition");
                        PlaceHolder phValue = (PlaceHolder)it.FindControl("phValue");
                        System.Web.UI.WebControls.TextBox tbValue = (System.Web.UI.WebControls.TextBox)it.FindControl("tbValue");
                        Telerik.Web.UI.RadComboBox rcbValue = (Telerik.Web.UI.RadComboBox)it.FindControl("rcbValue");
                        Telerik.Web.UI.RadNumericTextBox rnValue = (Telerik.Web.UI.RadNumericTextBox)it.FindControl("rnValue");

                        tbValue.Visible = false;
                        rcbValue.Visible = false;
                        rnValue.Visible = false;

                        csWhere.Select(c.TableName, c.ColumnName);
                        string OutTable;
                        string OutColumn;
                        string TextColumn;
                        string SelectControl;
                        if (!csWhere.SelectedField(DataViewTypeID, out OutTable, out OutColumn, out TextColumn, out SelectControl)) continue;

                        Type t = typeof(MetricTrac.Bll.LinqMicajahEntitybase).Assembly.GetType("MetricTrac.Bll." + c.TableName);
                        if (t == null) continue;

                        System.Reflection.PropertyInfo pi = t.GetProperty(c.ColumnName);
                        if (pi == null) continue;

                        if (SelectControl != null)
                        {
                            Condition.Mode = ConditionSelect.ConditionViewMode.Equal;
                            foreach (Panel p in phValue.Controls)
                            {
                                if (p.ID != System.IO.Path.GetFileNameWithoutExtension(SelectControl)) continue;
                                if (!(p.Controls[0] is MetricTrac.Utils.IValueSelectControl)) continue;
                                MetricTrac.Utils.IValueSelectControl iv = (MetricTrac.Utils.IValueSelectControl)p.Controls[0];
                                iv.SelectedValue = c.CompareValue;
                                p.Style[HtmlTextWriterStyle.Display] = "block";
                                break;
                            }

                            continue;
                        }

                        if (pi.PropertyType == typeof(string))
                        {
                            Condition.Mode = ConditionSelect.ConditionViewMode.Like;
                            tbValue.Visible = true;
                            tbValue.Text = c.CompareValue;
                            continue;
                        }
                        if (pi.PropertyType == typeof(Guid) || pi.PropertyType == typeof(Guid?))
                        {
                            Condition.Mode = ConditionSelect.ConditionViewMode.Equal;
                            rcbValue.Visible = true;
                            rcbValue.DataValueField = c.ColumnName;
                            rcbValue.DataTextField = TextColumn;
                            rcbValue.DataSource = MetricTrac.Bll.LinqMicajahDataContext.SelectGuid(c.TableName, c.ColumnName, TextColumn);
                            rcbValue.DataBind();
                            rcbValue.SelectedValue = c.CompareValue;
                            continue;
                        }

                        foreach (Type nt in NumericType)
                        {
                            if (pi.PropertyType == nt)
                            {
                                Condition.Mode = ConditionSelect.ConditionViewMode.Compare;
                                rnValue.Visible = true;
                                double v;
                                if (!double.TryParse(c.CompareValue, out v)) continue;
                                rnValue.Value = v;
                                continue;
                            }
                        }

                        Condition.Mode = ConditionSelect.ConditionViewMode.All;
                        tbValue.Visible = true;
                    }
                    ShowListManager(mWhereCriteria.Count - 1);
                }
                else
                {
                    ShowRow(0);
                    ShowRow(1);
                    ShowRow(2);
                    HideListManager(0);
                    HideListManager(1);
                    ShowListManager(2);
                }
            }
        }

        public static ICollection GetWhereData ()
        {
            RepeiterData[] d = new RepeiterData[RowCount];
            for (int i = 0; i < RowCount; i++)
            {
                d[i] = new RepeiterData((i < 9 ? "&nbsp;" : "") + (i + 1).ToString());
            }
            d[0].First = true;
            d[RowCount - 1].Last = true;
            return d;
        }

        System.Web.UI.HtmlControls.HtmlTableRow GetTableRow(int n)
        {
            return (System.Web.UI.HtmlControls.HtmlTableRow)rpWhere.Items[n].FindControl("trWhere");
        }

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
            return (ListManager)rpWhere.Items[n].FindControl("lmWhere");
        }

        void HideListManager(int n)
        {
            GetListManager(n).HideButton = true;
        }

        void ShowListManager(int n)
        {
            GetListManager(n).HideButton = false;
        }

        List<MetricTrac.Bll.DataViewWhere.Extend> mWhereCriteria;
        bool mWhereCriteriaUpdated;
        public List<MetricTrac.Bll.DataViewWhere.Extend> WhereCriteria
        {
            get
            {
                Prepare();
                return mWhereCriteria;
            }
            set
            {
                mWhereCriteria = value;
                mWhereCriteriaUpdated = true;
            }
        }

        public string WhereCondition
        {
            get
            {
                return tbCondition.Text;
            }
            set
            {
                tbCondition.Text = value;
            }
        }
    }
}
