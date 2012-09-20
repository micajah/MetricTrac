using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac.Control.Select
{
    public partial class SelectQuarter : System.Web.UI.UserControl
    {
        public bool Fiscal
        {
            get { return cSelectYear.Fiscal; }
            set { cSelectYear.Fiscal = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public int SelectedYear
        {
            get
            {
                return cSelectYear.SelectedYear;
            }
            set
            {
                cSelectYear.SelectedYear = value;
            }
        }

        public int SelectedQuarter
        {
            get
            {
                string s = ddlQuarter.SelectedValue;
                int v = 0;
                int.TryParse(s, out v);
                return v;
            }
            set
            {
                foreach (ListItem i in ddlQuarter.Items)
                {
                    if (i.Value == value.ToString())
                    {
                        i.Selected = true;
                        break;
                    }
                }
            }
        }

        public DateTime SelectedDate
        {
            get
            {
                if (SelectedYear <= 0 || SelectedQuarter <= 0) return DateTime.MinValue;
                DateTime dt = cSelectYear.SelectedDate;
                dt.AddMonths((SelectedQuarter - 1) * 3);
                return dt;
            }
            set
            {
                SelectedYear = value.Year;
                SelectedQuarter = (value.Month - 1) % 3 + 1;
            }
        }
    }
}