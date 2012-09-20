using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac.Control
{
    public partial class SelectWeek : System.Web.UI.UserControl
    {
        void AddDdlWeek(string t, string v)
        {
            ddlWeek.Items.Add(new ListItem(t, v));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                for (int w = 1; w <= 54; w++)
                {
                    AddDdlWeek("W"+w.ToString(),w.ToString());
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

        public int SelectedWeek
        {
            get
            {
                string s = ddlWeek.SelectedValue;
                int w = 0;
                int.TryParse(s, out w);
                return w;
            }
            set
            {
                foreach (ListItem i in ddlWeek.Items)
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
                if (SelectedYear <= 0 || SelectedWeek <= 0) return DateTime.MinValue;
                DateTime dt = MetricTrac.Bll.Frequency.GetFirstYearSunday(SelectedYear);
                dt.AddDays((SelectedWeek - 1) * 7);
                return dt;
            }
            set
            {
                int year = value.Year;
                DateTime dt = MetricTrac.Bll.Frequency.GetFirstYearSunday(year);
                if(dt<value) year--;
                SelectedWeek = (int)((value - dt).TotalDays / 7);
                SelectedYear = year;
            }
        }
    }
}