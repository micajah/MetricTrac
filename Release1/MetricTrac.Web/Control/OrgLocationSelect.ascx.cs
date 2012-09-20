using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace MetricTrac.Control
{
    public partial class OrgLocationSelect : System.Web.UI.UserControl, MetricTrac.Utils.IValueSelectControl
    {
        public delegate void OnSelectedOrgLocationChanged(Guid OrgLocationID);
        public event OnSelectedOrgLocationChanged SelectedOrgLocationChanged;

        protected string mOnClientOrgLocationChange;
        public string OnClientOrgLocationChange { set { mOnClientOrgLocationChange = value; } }

        public bool AutoPostBack { get; set; }

        /// <summary>
        /// OrgLocationID returns NULL if "Org Location Root" select
        /// </summary>
        public bool RootOrgLocationEqualToNull {  get; set; }

        /// <summary>
        /// Show selectted "Org Location Root" if OrgLocationID == NULL
        /// </summary>
        public bool ShowNullAsRooOrgLocation {  get; set; }

        private List<MetricTrac.Bll.Mc_EntityNode.Extend> GetDataSource()
        {
            List<MetricTrac.Bll.Mc_EntityNode.Extend> org = MetricTrac.Bll.Mc_EntityNode.ListOrgLocations();            
            return org;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ddlOrgLocation_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string v = ddlOrgLocation.SelectedValue;
            Guid OrgLocationID = Guid.Empty;
            if (!string.IsNullOrEmpty(v))
            {
                try { OrgLocationID = new Guid(v); }
                catch { }
            }
            if (SelectedOrgLocationChanged != null) SelectedOrgLocationChanged(OrgLocationID);
        }

        private bool IsFilled;
        private void FillTreeView(Guid? SelectedOrgLocationId, bool ClearText)
        {
            if (ShowNullAsRooOrgLocation && SelectedOrgLocationId == null) SelectedOrgLocationId = Guid.Empty;
            if (ClearText) ddlOrgLocation.Text = string.Empty;
            Telerik.Web.UI.RadTreeView tvOrgLocations = (Telerik.Web.UI.RadTreeView)ddlOrgLocation.Items[0].FindControl("tvOrgLocations");

            if (!IsFilled)
            {
                tvOrgLocations.DataSource = GetDataSource();
                tvOrgLocations.DataTextField = "Name";
                tvOrgLocations.DataFieldParentID = "ParentEntityNodeId";
                tvOrgLocations.DataValueField = "EntityNodeId";
                tvOrgLocations.DataFieldID = "EntityNodeId";
                tvOrgLocations.DataBind();
            }

            System.Collections.Generic.IList<Telerik.Web.UI.RadTreeNode> AllNodes = tvOrgLocations.GetAllNodes();
            foreach (Telerik.Web.UI.RadTreeNode node in AllNodes)
            {
                string FullPath = node.Text;
                Telerik.Web.UI.RadTreeNode Parent = node;
                while ((Parent = Parent.ParentNode) != null)
                {
                    FullPath = Parent.Text + " > " + FullPath;
                }

                if (SelectedOrgLocationId != null && node.Value == SelectedOrgLocationId.ToString()) ddlOrgLocation.Text = FullPath;
                node.Attributes.Add("FullPath", FullPath);

                node.ImageUrl = "~/images/GCATree/OrgLocation.gif";
            }

            IsFilled = true;
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (!IsPostBack) FillTreeView(null, false);
        }

        public Guid? OrgLocationID
        {
            get
            {
                if (string.IsNullOrEmpty(OrgLocationFullName)) return null;
                MetricTrac.Bll.Mc_EntityNode[] t = GetDataSource().ToArray();
                string[] OrgLocations = OrgLocationFullName.Split('>');

                Guid? OrgLocationID = null;
                string OrgLocation;

                for (int i = 0; i < OrgLocations.Length; i++)
                {
                    OrgLocation = HttpUtility.HtmlEncode(OrgLocations[i].Trim());

                    MetricTrac.Bll.Mc_EntityNode[] c;

                    if (i == OrgLocations.Length - 1) c = t.Where(on => on.Name.ToLower().StartsWith(OrgLocation.ToLower())).ToArray();
                    else c = t.Where(on => on.Name.ToLower() == OrgLocation.ToLower()).ToArray();

                    if (i==0) c = c.Where(on => on.ParentEntityNodeId == null).ToArray();
                    else c = c.Where(on => on.ParentEntityNodeId == OrgLocationID).ToArray();

                    if (c.Length > 0)
                    {
                        OrgLocationID = c[0].EntityNodeId;
                    }
                    else
                    {
                        break;
                    }
                }

                if (RootOrgLocationEqualToNull && OrgLocationID == Guid.Empty) return null;
                return OrgLocationID;
            }
            set
            {
                FillTreeView(value, true);
            }
        }

        public string OrgLocationFullName
        {
            get
            {
                return ddlOrgLocation.Text;
            }
        }

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

        #region IValueSelectControl Members

        public object SelectedValue
        {
            get { return OrgLocationID; }
            set { OrgLocationID = new Guid(value.ToString()); }
        }

        public bool IsValueSelected
        {
            get { return OrgLocationID!=null; }
        }

        #endregion
    }
}