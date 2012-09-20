using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace MetricTrac.Control
{
    public partial class GCASelect : System.Web.UI.UserControl
    {
        public delegate void OnSelectedGCAChanged(Guid GCAID);
        public event OnSelectedGCAChanged SelectedGCAChanged;

        protected string mOnClientGCAChange;
        public string OnClientGCAChange { set { mOnClientGCAChange = value; } }

        private bool mAutoPostBack;
        public bool AutoPostBack
        {
            set { mAutoPostBack = value; }
            get { return mAutoPostBack; }
        }

        private IQueryable<MetricTrac.Bll.GroupCategoryAspect> GetDataSource()
        {
            return MetricTrac.Bll.GroupCategoryAspect.SelectAll();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void ddlGCA_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            string v = ddlGCA.SelectedValue;
            Guid GCAID = Guid.Empty;
            if (!string.IsNullOrEmpty(v))
            {
                try { GCAID = new Guid(v); }
                catch { }
            }
            if (SelectedGCAChanged != null) SelectedGCAChanged(GCAID);
        }

        private bool IsFilled;
        private void FillTreeView(Guid? SelectedGCAId, bool ClearText)
        {
            if (ClearText) ddlGCA.Text = string.Empty;
            Telerik.Web.UI.RadTreeView tvGCAs = (Telerik.Web.UI.RadTreeView)ddlGCA.Items[0].FindControl("tvGCAs");

            if (!IsFilled)
            {
                tvGCAs.DataSource = GetDataSource();
                tvGCAs.DataTextField = "Name";
                tvGCAs.DataFieldParentID = "ParentId";
                tvGCAs.DataValueField = "GroupCategoryAspectID";
                tvGCAs.DataFieldID = "GroupCategoryAspectID";
                tvGCAs.DataBind();
            }

            System.Collections.Generic.IList<Telerik.Web.UI.RadTreeNode> AllNodes = tvGCAs.GetAllNodes();
            foreach (Telerik.Web.UI.RadTreeNode node in AllNodes)
            {
                string FullPath = node.Text;
                Telerik.Web.UI.RadTreeNode Parent = node;
                while ((Parent = Parent.ParentNode) != null)
                {
                    FullPath = Parent.Text + " > " + FullPath;
                }

                if (SelectedGCAId!=null && SelectedGCAId != Guid.Empty && node.Value == SelectedGCAId.ToString()) ddlGCA.Text = FullPath;
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
        }

        public Guid? GCAID
        {
            get
            {
                if (string.IsNullOrEmpty(GCAFullName)) return null;
                MetricTrac.Bll.GroupCategoryAspect [] t = GetDataSource().ToArray();
                string[] GCAs = GCAFullName.Split('>');

                Guid GCAID = Guid.Empty;
                string GCA;

                for (int i = 0; i < GCAs.Length; i++)
                {
                    GCA = HttpUtility.HtmlEncode(GCAs[i].Trim());

                    MetricTrac.Bll.GroupCategoryAspect[] c;

                    if (i == GCAs.Length - 1) c = t.Where(gca => gca.Name.ToLower().StartsWith(GCA.ToLower())).ToArray();
                    else c = t.Where(gca => gca.Name.ToLower() == GCA.ToLower()).ToArray();

                    if (GCAID != Guid.Empty) c = c.Where(gca => gca.ParentId == GCAID).ToArray();
                    else c = c.Where(gca => gca.ParentId == null).ToArray();

                    if (c.Length>0)
                    {
                        GCAID = c[0].GroupCategoryAspectID;
                    }
                    else return GCAID;
                }
                return GCAID;
            }
            set
            {
                FillTreeView(value, true);
            }
        }

        public string GCAFullName
        {
            get
            {
                return ddlGCA.Text;
            }
        }

        public Unit Width
        {
            get { return ddlGCA.Width; }
            set { ddlGCA.Width = value; }
        }
        public Unit DropDownWidth
        {
            get { return ddlGCA.DropDownWidth; }
            set { ddlGCA.DropDownWidth = value; }
        }
        public string ComboBoxClientID
        {
            get { return ddlGCA.ClientID; }
        }

        public string EmptyMessage
        {
            get { return ddlGCA.EmptyMessage; }
            set { ddlGCA.EmptyMessage = value; }
        }

        public string ToolTip
        {
            get { return ddlGCA.ToolTip; }
            set { ddlGCA.ToolTip = value; }
        }
    }
}