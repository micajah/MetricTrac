using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace MetricTrac.Control
{
    public partial class MultiSelectList : System.Web.UI.UserControl
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!IsPostBack)
                InitButtonHandlers();
            lbEntities.Attributes.Add(Request.Browser.Browser == "IE" ? "onclick" : "onmouseup", "ListBoxSelect_" + this.ClientID + "(null);");
        }

        private void InitButtonHandlers()
        {
            Button btnFinish = (Button)ddlEntities.Items[0].FindControl("btnFinish");
            Button btnClear = (Button)ddlEntities.Items[0].FindControl("btnClear");
            if (btnFinish != null)
                btnFinish.OnClientClick = "return FinishSelect_" + this.ClientID + "();";
            if (btnFinish != null)
                btnClear.OnClientClick = "return ClearSelect_" + this.ClientID + "();";
            ddlEntities.OnClientLoad = "ListBoxSelect_" + this.ClientID;            
            ddlEntities.OnClientDropDownClosed = "comboClientDropDownClosed_" + this.ClientID;
        }

        public void LoadEntities(List<Bll.DataRule.Entity> lEntity, Guid? SelectedId)
        {            
            lbEntities.Items.Clear();
            foreach (Bll.DataRule.Entity g in lEntity)
            {   
                ListItem li = new ListItem(g.Name, g.ID.ToString());                
                li.Selected = g.ID == SelectedId;
                lbEntities.Items.Add(li);
            }
        }

        public Guid?[] SelectedValues
        {
            get
            {
                List<string> SelectedItems = new List<string>();
                foreach (ListItem li in lbEntities.Items)
                    if (li.Selected)
                        SelectedItems.Add(li.Value);                
                Guid?[] g = SelectedItems.Select(r => ParseStringToGuid(r)).ToArray();
                return g;                
            }
            set
            {
                if (value != null)
                {//lbEntities.ClearSelection();
                    List<string> SelectedItems = value.Select(v => v.ToString()).ToList();
                    foreach (ListItem li in lbEntities.Items)
                        li.Selected = SelectedItems.Contains(li.Value);
                }
            }
        }

        private Guid? ParseStringToGuid(string value)
        {
            Guid? result = null;
            if (!String.IsNullOrEmpty(value))
            {
                try { result = new Guid(value); }
                catch { }
            }
            return result;
        }

        public string EntitiesName { get; set; }

        public int Width
        {
            set { ddlEntities.Width = Unit.Pixel(value); }
        }

        public string ToolTip
        {
            get { return ddlEntities.ToolTip; }
            set { ddlEntities.ToolTip = value; }
        }

        public string OnClientSelectedIndexChanged { get; set; }

        public bool AutoPostBack { get; set; }

        protected ListBox lbEntities
        {
            get { return (ListBox)ddlEntities.Items[0].FindControl("lbEntities"); }
        }

        public string EmptyMessage
        {
            get { return ddlEntities.EmptyMessage; }
            set { ddlEntities.EmptyMessage = value; }
        }
    }
}