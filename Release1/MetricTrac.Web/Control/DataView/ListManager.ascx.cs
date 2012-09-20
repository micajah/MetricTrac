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
    public partial class ListManager : System.Web.UI.UserControl
    {
        public bool First
        {
            get
            {
                object o = ViewState["ListManagerFirstRow"];
                return (o is bool) ? (bool)o : false;
            }
            set
            {
                ViewState["ListManagerFirstRow"] = value;
            }
        }

        public bool Last
        {
            get
            {
                object o = ViewState["ListManagerLastRow"];
                return (o is bool) ? (bool)o : false;
            }
            set
            {
                ViewState["ListManagerLastRow"] = value;
            }
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
        }

        private void SetHidenValue(int n, bool v)
        {
            bool v2 = GetHidenValue(n == 0 ? 1 : 0);
            if (n == 0) hfListManager.Value = (v ? "1" : "0") + "," + (v2 ? "1" : "0");
            else hfListManager.Value = (v2 ? "1" : "0") + "," + (v ? "1" : "0");
        }

        private bool GetHidenValue(int n)
        {
            string s = hfListManager.Value;
            if (string.IsNullOrEmpty(s)) return false;
            string [] ss = hfListManager.Value.Split(',');
            if (ss == null || ss.Length != 2) return false;
            return ss[n] == "1";
        }

        public bool HideButton
        {
            get
            {
                return GetHidenValue(1);
            }
            set
            {
                SetHidenValue(1, value);
            }
        }
        public bool HideRow
        {
            get
            {
                return GetHidenValue(0);
            }
            set
            {
                SetHidenValue(0, value);
            }
        }
        protected string GetStyle()
        {
            return HideButton ? "display:none" : "";
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            System.Web.UI.Control c = this;
            while (c != null)
            {
                if (c is TableRow)
                {
                    ((TableRow)c).Style[HtmlTextWriterStyle.Display] = HideRow ? "none" : "";
                    break;
                }
                if (c is System.Web.UI.HtmlControls.HtmlTableRow)
                {
                    ((System.Web.UI.HtmlControls.HtmlTableRow)c).Style[HtmlTextWriterStyle.Display] = HideRow ? "none" : "";
                    break;
                }
                c = c.Parent;
            }
        }
    }
}
