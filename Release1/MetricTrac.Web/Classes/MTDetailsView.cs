using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Micajah.Common.WebControls;

namespace MetricTrac.MTControls
{
    public class MTDetailsView : MagicForm
    {
        public MTDetailsView()
        {
        }

        private bool IsPropValue(DataControlField f, string PropName, string PropValue)
        {
            try
            {
                Type t = f.GetType();
                System.Reflection.PropertyInfo pi = t.GetProperty(PropName);
                if (pi == null) return false;
                if (pi.PropertyType.FullName != "System.String") return false;
                string s = pi.GetValue(f, null).ToString();
                return s == PropValue;
            }
            catch
            {
                return false;
            }
        }

        private System.Web.UI.Control GetControl(DataControlField f)
        {
            if (Rows[Fields.IndexOf(f)].Cells[1].Controls.Count > 1)
                return Rows[Fields.IndexOf(f)].Cells[1].Controls[1];
            else
                return Rows[Fields.IndexOf(f)].Cells[1].Controls[0];            
        }

        public System.Web.UI.Control FindFieldControl(string DataField_HeaderText)
        {
            DataControlField f = FindField(DataField_HeaderText);
            if (f == null) return null;
            return GetControl(f);
        }

        public DataControlField FindField(string DataField_HeaderText)
        {
            foreach (DataControlField f in Fields)
            {
                if (IsPropValue(f, "DataField", DataField_HeaderText)) return f;
            }
            foreach (DataControlField f in Fields)
            {
                if (IsPropValue(f, "HeaderText", DataField_HeaderText)) return f;
            }
            return null;
        }

        //protected object DataSourceItem { get; private set; }

        public delegate void ExtractRowValuesDelegate(System.Collections.Specialized.IOrderedDictionary fieldValues, bool includeReadOnlyFields, bool includeKeys);
        public event ExtractRowValuesDelegate ExtractRowValuesEvent;
        protected override void ExtractRowValues(System.Collections.Specialized.IOrderedDictionary fieldValues, bool includeReadOnlyFields, bool includeKeys)
        {
            base.ExtractRowValues(fieldValues, includeReadOnlyFields, includeKeys);
            if (ExtractRowValuesEvent != null) ExtractRowValuesEvent(fieldValues, includeReadOnlyFields, includeKeys);
        }

        public string CloseButtonCaption { get; set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (!String.IsNullOrEmpty(CloseButtonCaption) && this.CloseButton != null)
            {
                ((Button)this.CloseButton).Text = CloseButtonCaption;
                ((Button)this.CloseButton).Attributes.Add("style", "padding: 7px 15px 7px 15px; font-size:9pt;");
            }
        }

    }
}
