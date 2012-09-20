using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Micajah.Common.WebControls;

namespace MetricTrac.MTControls
{
    public class MTGridView : CommonGridView
    {
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
        public DataControlField FindField(string DataField_HeaderText)
        {
            foreach(DataControlField f in Columns)
            {
                if (IsPropValue(f, "DataField", DataField_HeaderText)) return f;
            }
            foreach(DataControlField f in Columns)
            {
                if (IsPropValue(f, "HeaderText", DataField_HeaderText)) return f;
            }
            return null;
        }

        protected override void Render(HtmlTextWriter writer)
        {
            base.Render(writer);
        }
    }
}
