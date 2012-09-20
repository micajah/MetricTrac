using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac.Control
{
    public partial class SelectMonth : System.Web.UI.UserControl
    {
        void AddDdlMonth(string t, string v)
        {
            ddlMonth.Items.Add(new ListItem(t,v));
        }
        string[] MonthName = new string[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                for (int w = 1; w <= 12; w++)
                {
                    AddDdlMonth(MonthName[w-1], w.ToString());
                }
            }
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

        public int SelectedMonth
        {
            get
            {
                string s = ddlMonth.SelectedValue;
                int w = 0;
                int.TryParse(s, out w);
                return w;
            }
            set
            {
                foreach (ListItem i in ddlMonth.Items)
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
                if (SelectedYear <= 0 || SelectedMonth <= 0) return DateTime.MinValue;
                DateTime dt = new DateTime(SelectedYear, SelectedMonth, 1);
                return dt;
            }
            set
            {
                SelectedYear = value.Year;
                SelectedMonth = value.Month;
            }
        }
    }
}