using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Micajah.Common.WebControls;

namespace MetricTrac.MTControls
{
    public class MTDatePickerField : DatePickerField
    {
        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            base.InitializeDataCell(cell, rowState);
            DatePicker datePicker = null;
            foreach (System.Web.UI.Control c in cell.Controls)
                if (c is Micajah.Common.WebControls.DatePicker)
                    datePicker = (Micajah.Common.WebControls.DatePicker)c;
            if (base.Visible && datePicker != null)
            {
                datePicker.DateFormat = this.DateFormat; // MC don't set this property for DatePicker
                datePicker.DataBinding += new EventHandler(this.OnBindingField);
            }
        }

        private void OnBindingField(object sender, EventArgs e)
        {
            DatePicker datePicker = sender as DatePicker;
            if (datePicker.IsEmpty)
                datePicker.Clear();
        }

        protected override object ExtractControlValue(System.Web.UI.Control control)
        {
            DatePicker datePicker = control as DatePicker;
            return datePicker.IsEmpty ? null : base.ExtractControlValue(control);
        }
    }
}