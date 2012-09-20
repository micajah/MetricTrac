using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac.Control.Select
{
    public partial class SelectSemiYear : System.Web.UI.UserControl
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

        public int SelectedSemi
        {
            get
            {
                string s = ddlSemi.SelectedValue;
                int v = 0;
                int.TryParse(s, out v);
                return v;
            }
            set
            {
                foreach (ListItem i in ddlSemi.Items)
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
                if (SelectedYear <= 0 || SelectedSemi <= 0) return DateTime.MinValue;
                DateTime dt = cSelectYear.SelectedDate;
                dt.AddMonths((SelectedSemi - 1) * 6);
                return dt;
            }
            set
            {
                SelectedYear = value.Year;
                SelectedSemi = value.Month < 6 ? 1 : 2;
            }
        }
    }
}