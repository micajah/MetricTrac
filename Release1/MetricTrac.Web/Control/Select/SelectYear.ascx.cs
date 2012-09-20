using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac.Control
{
    public partial class SelectYear : System.Web.UI.UserControl
    {
        public bool Fiscal { get; set; }
        public bool BiAnual { get; set; }

        void AddDdlYear(string y)
        {
            ddlYear.Items.Add(new ListItem(y, y));
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int c = 0;
                for (int y = DateTime.Now.Year; y >= 2000; y--)
                {
                    c++;
                    if (BiAnual && (y & 1) == 1) continue;
                    AddDdlYear(y.ToString());
                }
            }
        }
        public int SelectedYear
        {
            get
            {
                string s = ddlYear.SelectedValue;
                int y = 0;
                int.TryParse(s, out y);
                return y;
            }
            set
            {
                int year = value;
                if (BiAnual) year = year - (year % 2);
                foreach (ListItem i in ddlYear.Items)
                {
                    if (i.Value == year.ToString())
                    {
                        i.Selected = true;
                        break;
                    }
                }
            }
        }

        public string DdlClientID
        {
            get { return ddlYear.ClientID; }
        }

        

        public DateTime SelectedDate
        {
            get
            {
                if (SelectedYear <= 0) return DateTime.MinValue;
                DateTime dt;
                if (Fiscal)
                {
                    dt = MetricTrac.Bll.Frequency.GetFiscalYearStart(SelectedYear);
                }
                else
                {
                    dt = new DateTime(SelectedYear, 1, 1);
                }
                return dt;
            }
            set
            {
                SelectedYear = value.Year;
            }
        }
    }
}