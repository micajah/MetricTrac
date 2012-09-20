using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac.Control
{
    public partial class MetricCategorySelect : System.Web.UI.UserControl
    {
        public delegate void OnSelectedMetricCategoryChanged(Guid MetricCategoryID);
        public event OnSelectedMetricCategoryChanged SelectedMetricCategoryChanged;

        protected string mOnClientMetricCategoryChange;
        public string OnClientMetricCategoryChange { set { mOnClientMetricCategoryChange = value; } }

        private bool mAutoPostBack;
        public bool AutoPostBack
        {
            set { mAutoPostBack = value; }
            get { return mAutoPostBack; }
        }

        protected string mOnClientTextChange;
        public string OnClientTextChange
        {
            set
            {
                mOnClientTextChange = value;                
            }
        }

        private IQueryable<MetricTrac.Bll.MetricCategory> GetDataSource()
        {
            return MetricTrac.Bll.MetricCategory.SelectAll();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ddlMetricCategory_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string v = ddlMetricCategory.SelectedValue;
            Guid MetricCategoryID = Guid.Empty;
            if (!string.IsNullOrEmpty(v))
            {
                try { MetricCategoryID = new Guid(v); }
                catch { }
            }
            if (SelectedMetricCategoryChanged != null) SelectedMetricCategoryChanged(MetricCategoryID);
        }

        private bool IsFilled;
        private void FillTreeView(Guid? SelectedMetricCategoryId, bool ClearText)
        {
            if (ClearText) ddlMetricCategory.Text = string.Empty;
            Telerik.Web.UI.RadTreeView tvMetricCategorys = (Telerik.Web.UI.RadTreeView)ddlMetricCategory.Items[0].FindControl("tvMetricCategorys");

            if (!IsFilled)
            {
                tvMetricCategorys.DataSource = GetDataSource();
                tvMetricCategorys.DataTextField = "Name";
                tvMetricCategorys.DataFieldParentID = "ParentId";
                tvMetricCategorys.DataValueField = "MetricCategoryID";
                tvMetricCategorys.DataFieldID = "MetricCategoryID";
                tvMetricCategorys.DataBind();
            }

            System.Collections.Generic.IList<Telerik.Web.UI.RadTreeNode> AllNodes = tvMetricCategorys.GetAllNodes();
            foreach (Telerik.Web.UI.RadTreeNode node in AllNodes)
            {
                string FullPath = node.Text;
                Telerik.Web.UI.RadTreeNode Parent = node;
                while ((Parent = Parent.ParentNode) != null)
                {
                    FullPath = Parent.Text + " > " + FullPath;
                }

                if (SelectedMetricCategoryId != null && SelectedMetricCategoryId != Guid.Empty && node.Value == SelectedMetricCategoryId.ToString()) ddlMetricCategory.Text = FullPath;
                node.Attributes.Add("FullPath", FullPath);

                node.ImageUrl = "~/images/GCATree/";
                switch (node.Level)
                {
                    case 0:
                        node.ImageUrl += "group16.gif";
                        break;
                    case 1:
                        node.ImageUrl += "category16.gif";
                        break;
                    default:
                        node.ImageUrl += "aspect16.gif";
                        break;
                }
            }

            IsFilled = true;
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (!IsPostBack) FillTreeView(Guid.Empty, false);
            Page.ClientScript.RegisterStartupScript(this.GetType(), "_startupscript", "InitStartMCValue('" + ddlMetricCategory.Text + "');", true);
        }

        public Guid? MetricCategoryID
        {
            get
            {
                Guid? MetricCategoryID = MetricTrac.Utils.MetricUtils.GetMetricCategoryFromFullName(MetricCategoryFullName, GetDataSource());                
                return MetricCategoryID;
            }
            set
            {
                FillTreeView(value, true);
            }
        }

        public string MetricCategoryFullName
        {
            get
            {
                return ddlMetricCategory.Text;
            }
        }

        public Unit Width
        {
            get { return ddlMetricCategory.Width; }
            set { ddlMetricCategory.Width = value; }
        }

        public string ddlMetricCategoryClientID
        {
            get
            {
                return ddlMetricCategory.ClientID;
            }
        }
    }
}