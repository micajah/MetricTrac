using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac.Control.Select
{
    public partial class SelectPI : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                dsPI.WhereParameters.Add("InstanceId", System.Data.DbType.Guid, Bll.LinqMicajahDataContext.InstanceId.ToString());
            }
        }

        public Unit Width
        {
            get { return ddlPI.Width; }
            set { ddlPI.Width = value; }
        }

        public Guid? SelectedPIId
        {
            get 
            {
                string s = ddlPI.SelectedValue;
                if (string.IsNullOrEmpty(s)) return null;
                Guid g;
                try { g = new Guid(s); }
                catch { return null; }
                return g;
            }
            set
            {
                ddlPI.SelectedItem.Selected = false;
                if (value != null && value != Guid.Empty)
                {
                    foreach (ListItem it in ddlPI.Items)
                    {
                        if (it.Value == value.ToString())
                        {
                            it.Selected = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}