using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Web.UI;

namespace MetricTrac.Control
{
    public partial class OrglocationMultipick : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitButtonHandlers();
                LoadOrgLocations();
            }
        }

        private void InitButtonHandlers()
        {
            Button btnFinish = (Button)ddlOrgLocation.Items[0].FindControl("btnFinish");
            Button btnClear = (Button)ddlOrgLocation.Items[0].FindControl("btnClear");
            if (btnFinish != null)
                btnFinish.OnClientClick = "return FinishSelect_" + this.ClientID + "();";
            if (btnFinish != null)
                btnClear.OnClientClick = "return ClearSelect_" + this.ClientID + "();";
        }
        
        private void LoadOrgLocations()
        {   
            tvOrgLocations.DataSource = MetricTrac.Bll.Mc_EntityNode.ListOrgLocations();
            tvOrgLocations.DataBind();
        }

        protected void tvOrgLocations_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            RadTreeNode node = e.Node;
            node.ExpandMode = TreeNodeExpandMode.ClientSide;
            node.ImageUrl = "~/images/GCATree/OrgLocation.gif";
            string FullPath = node.Text;
            RadTreeNode Parent = node;
            while ((Parent = Parent.ParentNode) != null)
                FullPath = Parent.Text + " > " + FullPath;
            node.Attributes.Add("FullPath", FullPath);
        }

        protected void ddlOrgLocation_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {           
            if (SelectedOrgLocationChanged != null)
                SelectedOrgLocationChanged(OrgLocationID);
            if (SelectedOrgLocationsChanged != null)
                SelectedOrgLocationsChanged(OrgLocationsID);
        }

        public Guid? OrgLocationID
        {
            get
            {
                Guid? SelectedNodeValue = null;
                IList<RadTreeNode> selectedNodes = tvOrgLocations.SelectedNodes;
                if (selectedNodes.Count > 0)
                    SelectedNodeValue = ParseStringToGuid(selectedNodes[0].Value);
                return SelectedNodeValue;
            }
            set
            {   
                tvOrgLocations.ClearSelectedNodes();
                RadTreeNode SelectNode = tvOrgLocations.FindNodeByValue((ShowNullAsRootOrgLocation && value == null) ? Guid.Empty.ToString() : value.ToString());
                if (SelectNode != null)
                    SelectNode.Expanded = SelectNode.Selected = true;
            }
        }

        public Guid?[] OrgLocationsID
        {
            get
            {
                IList<RadTreeNode> selectedNodes = tvOrgLocations.SelectedNodes;
                Guid?[] g = selectedNodes.Select(r => ParseStringToGuid(r.Value)).ToArray();                
                return g;
            }
            set
            {
                tvOrgLocations.ClearSelectedNodes();
                if (value != null)
                    foreach (Guid? g in value)
                    {   
                        RadTreeNode node = tvOrgLocations.FindNodeByValue((ShowNullAsRootOrgLocation && g == null) ? Guid.Empty.ToString() : g.ToString());
                        if (node != null)
                            node.Expanded = node.Selected = true;
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

        protected RadTreeView tvOrgLocations
        {
            get
            {
                return (RadTreeView)ddlOrgLocation.Items[0].FindControl("tvOrgLocations");
            }
        }

        #region Public event, called when org location changing
        public delegate void OnSelectedOrgLocationChanged(Guid? OrgLocationID);
        public event OnSelectedOrgLocationChanged SelectedOrgLocationChanged;

        public delegate void OnSelectedOrgLocationsChanged(Guid?[] OrgLocationsID);
        public event OnSelectedOrgLocationsChanged SelectedOrgLocationsChanged;
        #endregion

        #region Public Control Properties

        /// <summary>
        /// OrgLocationID returns NULL if "Org Location Root" select
        /// Currently not used, Root Location ID always equal to Guid.Empty
        /// </summary>
        public bool RootOrgLocationEqualToNull { get; set; }

        /// <summary>
        /// Show selectted "Org Location Root" if OrgLocationID == NULL
        /// </summary>
        public bool ShowNullAsRootOrgLocation { get; set; }

        protected string mOnClientOrgLocationChange;
        public string OnClientOrgLocationChange { set { mOnClientOrgLocationChange = value; } }

        public bool AutoPostBack { get; set; }

        public Unit Width
        {
            get { return ddlOrgLocation.Width; }
            set { ddlOrgLocation.Width = value; }
        }

        public Unit DropDownWidth
        {
            get { return ddlOrgLocation.DropDownWidth; }
            set { ddlOrgLocation.DropDownWidth = value; }
        }

        public string EmptyMessage
        {
            get { return ddlOrgLocation.EmptyMessage;}
            set { ddlOrgLocation.EmptyMessage = value; }
        }

        public string ToolTip
        {
            get { return ddlOrgLocation.ToolTip; }
            set { ddlOrgLocation.ToolTip = value; }
        }
        #endregion
    }
}