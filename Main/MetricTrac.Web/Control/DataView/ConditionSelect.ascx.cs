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
    public partial class ConditionSelect : System.Web.UI.UserControl
    {
        public enum ConditionViewMode { All, Equal, Like, Compare };
        public ConditionViewMode Mode { get; set; }

        public int DataViewConditionTypeID
        {
            get
            {
                int id=0;
                if (string.IsNullOrEmpty(rcbCondition.SelectedValue)) return 0;
                int.TryParse(rcbCondition.SelectedValue, out id);
                return id;
            }
        }

        void ShowItem(string v)
        {
            foreach (Telerik.Web.UI.RadComboBoxItem i in rcbCondition.Items)
            {
                if (i.Value == v)
                {
                    if (!rcbCondition.Visible)
                    {
                        rcbCondition.Visible = true;
                        i.Selected = true;
                    }
                    i.Visible = true;
                }
            }
        }

        void SelectItem(string v)
        {
            foreach (Telerik.Web.UI.RadComboBoxItem i in rcbCondition.Items)
            {
                if (i.Value == v)
                {
                    i.Selected = true;
                    return;
                }
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            foreach (Telerik.Web.UI.RadComboBoxItem i in rcbCondition.Items) i.Visible = false;
            switch (Mode)
            {
                case ConditionViewMode.Equal:
                    ShowItem("3");
                    break;
                case ConditionViewMode.Like:
                    ShowItem("4");
                    break;
                case ConditionViewMode.Compare:
                    ShowItem("1");
                    ShowItem("2");
                    ShowItem("3");
                    ShowItem("5");
                    ShowItem("6");
                    break;
                default:
                    ShowItem("1");
                    ShowItem("2");
                    ShowItem("3");
                    ShowItem("4");
                    ShowItem("5");
                    ShowItem("6");
                    SelectItem("3");
                    break;
            }

            if (!rcbCondition.SelectedItem.Visible)
            {
                foreach (Telerik.Web.UI.RadComboBoxItem i in rcbCondition.Items)
                    if (i.Visible)
                    {
                        i.Selected = true;
                        break;
                    }
            }
        }
    }
}
