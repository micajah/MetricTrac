using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Micajah.Common.WebControls;

namespace MetricTrac.MTControls
{
    public class MTComboBoxField : ComboBoxField
    {
        public bool AddEmptyItem { get; set; }
        public string OnClientSelectedIndexChanged { get; set; }

        private string m_SelectedValue = string.Empty;
        private void OnBindingField(object sender, EventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            if (comboBox != null)
            {
                m_SelectedValue = this.LookupStringValue(comboBox);
                this.DataBound += new EventHandler(MTComboBoxField_DataBound);
            }
        }

        void MTComboBoxField_DataBound(object sender, EventArgs e)
        {
            if (AddEmptyItem)
            {
                ComboBox comboBox = sender as ComboBox;
                RadComboBoxItem it = new RadComboBoxItem();
                comboBox.Items.Insert(0,it);
                if (string.IsNullOrEmpty(m_SelectedValue)) it.Selected = true;
            }
        }
        private ComboBox GetComboBox(DataControlFieldCell cell)
        {
            foreach (WebControl c in cell.Controls)
            {
                if (c is ComboBox) return (ComboBox)c;
            }
            return null;
        }
        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            base.InitializeDataCell(cell, rowState);
            ComboBox control = GetComboBox(cell);
            if (base.Visible)
            {
                control.DataBinding += new EventHandler(this.OnBindingField);
                if (!String.IsNullOrEmpty(this.OnClientSelectedIndexChanged))
                    control.OnClientSelectedIndexChanged = this.OnClientSelectedIndexChanged;
            }
        }

        protected override object ExtractControlValue(System.Web.UI.Control control)
        {
            ComboBox comboBox = control as ComboBox;
            if (AddEmptyItem && comboBox != null && string.IsNullOrEmpty(comboBox.SelectedValue)) return null;
            return base.ExtractControlValue(control);
        }


    }
}
