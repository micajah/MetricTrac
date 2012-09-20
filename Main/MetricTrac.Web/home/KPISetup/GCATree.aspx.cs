using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.SessionState;
using System.Web.UI.HtmlControls;
using Telerik.Web.UI;
using System.Data.Linq;

namespace MetricTrac
{
    public partial class GCATree : System.Web.UI.Page
    {
        protected enum enTreeLevel { Root = 0, Group = 1, Category = 2, Aspect = 3 };
        
        private void GetExpandedNodes(RadTreeNodeCollection nodes, System.Collections.Generic.List<string> ExpandedNodes)
        {
            foreach (RadTreeNode n in nodes)
            {
                if (n.Expanded) ExpandedNodes.Add(n.Level + " " + n.Value);
                if (n.Nodes.Count > 0 && n.Level<5) GetExpandedNodes(n.Nodes, ExpandedNodes);
            }
        }

        private void SetExpandedNode(RadTreeNode n, System.Collections.Generic.List<string> ExpandedNodes)
        {
            n.Expanded = ExpandedNodes.Contains(n.Level + " " + n.Value);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Micajah.Common.Pages.MasterPage mp = Page.Master as Micajah.Common.Pages.MasterPage;
            rapTreeView.LoadingPanelID = ((MetricTrac.MasterPage)mp).ralpLoadingPanel1ID;
            ((Micajah.Common.Pages.MasterPage)Page.Master).AutoGenerateBreadcrumbs = false;
            Micajah.Common.Security.UserContext.Breadcrumbs.Add("Org Settings", "Resources.Micajah.Common/Pages/Admin/Configuration.aspx", String.Empty);
            Micajah.Common.Security.UserContext.Breadcrumbs.Add("Trees", "Resources.Micajah.Common/Pages/Admin/OrganizationEntities.aspx", String.Empty);
            Micajah.Common.Security.UserContext.Breadcrumbs.Add("Group Category Aspect Tree", String.Empty, String.Empty);         
            if (!IsPostBack) GenerateTreeView();
        }

        private void GenerateTreeView()
        {
            System.Collections.Generic.List<string> ExpandedNodes = new System.Collections.Generic.List<string>();
            GetExpandedNodes(rtvGCA.Nodes, ExpandedNodes);

            rtvGCA.Nodes.Clear();

            IQueryable<Bll.GroupCategoryAspect> Gca = Bll.GroupCategoryAspect.SelectAll();
            List<Bll.PerformanceIndicator.Extend> PI = Bll.PerformanceIndicator.List();
            //List<Bll.PerformanceIndicator> PI = lPI.Cast<Bll.PerformanceIndicator>();
            IQueryable<Bll.Metric.MetricPIJunc> JuncMetrics =  Bll.PerformanceIndicator.PIMetricJuncList();

            
            RadTreeNode RootNode = new RadTreeNode("Group Category Aspect Tree", "0");
            RootNode.Expanded = true;
            RootNode.AllowDrag = false;
            RootNode.AllowDrop = false;
            RootNode.AllowEdit = false;
            RootNode.ContextMenuID = "RootContextMenu";

            rtvGCA.Nodes.Add(RootNode);
            RadTreeNode GroupNode = null;

            IQueryable<Bll.GroupCategoryAspect> GcaLevel0 = Gca.Where(p => p.ParentId == null);

            foreach (Bll.GroupCategoryAspect gca in GcaLevel0)            
            {                
                GroupNode = new RadTreeNode(gca.Name, gca.GroupCategoryAspectID.ToString());                
                GroupNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("box.png", Micajah.Common.WebControls.IconSize.Smaller, true);//"~/images/GCATree/category16.gif"
                GroupNode.ContextMenuID = "GroupContextMenu";
                RootNode.Nodes.Add(GroupNode);
                SetExpandedNode(GroupNode, ExpandedNodes);
                BuildSubTree(Gca, Gca.Where(p => p.ParentId == gca.GroupCategoryAspectID), GroupNode, gca.GroupCategoryAspectID, 0, ExpandedNodes, PI, JuncMetrics);
                AppendPINodes(GroupNode, PI.Where(pi => pi.GroupCategoryAspectID == gca.GroupCategoryAspectID), ExpandedNodes, JuncMetrics);
            }
            if (PI.Count(pi => pi.GroupCategoryAspectID == null) > 0)
            {
                RadTreeNode EmptyNode = new RadTreeNode("Other Performance Indicators", "PIM");
                EmptyNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                EmptyNode.EnableContextMenu = false;
                EmptyNode.AllowDrag = false;
                EmptyNode.AllowDrop = false;
                EmptyNode.AllowEdit = false;
                RootNode.Nodes.Add(EmptyNode);
                SetExpandedNode(EmptyNode, ExpandedNodes);
                AppendPINodes(EmptyNode, PI.Where(pi => pi.GroupCategoryAspectID == null), ExpandedNodes, JuncMetrics);                
            }
        }

        private void AppendPINodes(RadTreeNode RootNode, IEnumerable<MetricTrac.Bll.PerformanceIndicator.Extend> PI, System.Collections.Generic.List<string> ExpandedNodes, IQueryable<Bll.Metric.MetricPIJunc> JuncMetrics)
        {
            foreach (Bll.PerformanceIndicator pi in PI)
            {
                RadTreeNode PINode = new RadTreeNode(pi.Name, "PIM");
                PINode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("book.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                PINode.EnableContextMenu = false;
                PINode.AllowDrag = false;
                PINode.AllowDrop = false;
                PINode.AllowEdit = false;
                RootNode.Nodes.Add(PINode);
                SetExpandedNode(PINode, ExpandedNodes);
                AppendMetricNodes(PINode, JuncMetrics.Where(m => m.PerformanceIndicatorID == pi.PerformanceIndicatorID), ExpandedNodes);
            }
        }

        private void AppendMetricNodes(RadTreeNode RootNode, IQueryable<Bll.Metric.MetricPIJunc> JuncMetrics, System.Collections.Generic.List<string> ExpandedNodes)
        {
            foreach (Bll.Metric.MetricPIJunc mj in JuncMetrics)
            {
                RadTreeNode MeetricNode = new RadTreeNode(mj.Name, "PIM");
                MeetricNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("book_open.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                MeetricNode.EnableContextMenu = false;
                MeetricNode.AllowDrag = false;
                MeetricNode.AllowDrop = false;
                MeetricNode.AllowEdit = false;
                RootNode.Nodes.Add(MeetricNode);
                SetExpandedNode(MeetricNode, ExpandedNodes);
            }
        }

        private void BuildSubTree(IQueryable<MetricTrac.Bll.GroupCategoryAspect> AllGca, IQueryable<MetricTrac.Bll.GroupCategoryAspect> Gca, RadTreeNode RootNode, Guid guid, int level, System.Collections.Generic.List<string> ExpandedNodes, List<Bll.PerformanceIndicator.Extend> PI, IQueryable<Bll.Metric.MetricPIJunc> JuncMetrics)
        {
            foreach (MetricTrac.Bll.GroupCategoryAspect gca in Gca)
            {
                RadTreeNode SomeNode = new RadTreeNode(gca.Name, gca.GroupCategoryAspectID.ToString());
                if (level == 0)
                {                    
                    SomeNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("folder.png", Micajah.Common.WebControls.IconSize.Smaller, true);//"~/images/GCATree/group16.gif";
                    SomeNode.ContextMenuID = "CategoryContextMenu";
                }
                else
                {
                    SomeNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("note.png", Micajah.Common.WebControls.IconSize.Smaller, true);//"~/images/GCATree/aspect16.gif";
                    SomeNode.ContextMenuID = "AspectContextMenu";                        
                }
                RootNode.Nodes.Add(SomeNode);
                SetExpandedNode(SomeNode, ExpandedNodes);
                BuildSubTree(AllGca, AllGca.Where(p => p.ParentId == gca.GroupCategoryAspectID), SomeNode, gca.GroupCategoryAspectID, 1, ExpandedNodes, PI, JuncMetrics);
                AppendPINodes(SomeNode, PI.Where(pi => pi.GroupCategoryAspectID == gca.GroupCategoryAspectID), ExpandedNodes, JuncMetrics);
            }
        }

        // treeview event handlers
        protected void rtvGCA_ContextMenuItemClick(object sender, RadTreeViewContextMenuEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Node.Value)) return;
            Guid ID = new Guid(e.Node.Value);            
            if (string.IsNullOrEmpty(e.MenuItem.Value)) return;

            string[] command = e.MenuItem.Value.Split(' ');
            if (command.Length != 2) return;

            if (command[0] == "Delete")
            {
                Bll.GroupCategoryAspect.Delete(ID);
                GenerateTreeView();
            }
        }

        protected void rtvGCA_NodeEdit(object sender, RadTreeNodeEditEventArgs e)
        {            
            if (string.IsNullOrEmpty(e.Node.Value))
            {
                Guid? ParentID = null;
                if (e.Node.ParentNode.Value != "0") ParentID = new Guid(e.Node.ParentNode.Value);
                Bll.GroupCategoryAspect gca = new Bll.GroupCategoryAspect();                
                gca.ParentId = ParentID;
                gca.Name = e.Text;                
                Guid _InsertedItemID = MetricTrac.Bll.GroupCategoryAspect.Insert(gca);
                e.Node.Expanded = true;
            }
            else
            {                
                if (!String.IsNullOrEmpty(e.Node.Value) && e.Node.Value != "0")
                {
                    Guid ID = new Guid(e.Node.Value);
                    Bll.GroupCategoryAspect.Update(ID, e.Text);
                }
            }
            GenerateTreeView();
        }

        protected void rtvGCA_NodeDrop(object sender, RadTreeNodeDragDropEventArgs e)
        {
            string ErrorMessage = String.Empty;

            if (e.SourceDragNode == null || e.DestDragNode == null) return;
            bool IsMerge = e.SourceDragNode.Level == e.DestDragNode.Level;
            if (e.SourceDragNode.Level - 1 != e.DestDragNode.Level && !IsMerge) return;
            if (e.SourceDragNode.Level < 1) return;

            Guid SourceID = new Guid(e.SourceDragNode.Value);
            Guid DestID = new Guid(e.DestDragNode.Value);
            
            if (IsMerge)            
                Bll.GroupCategoryAspect.Merge(SourceID, DestID);
            else
            {
                bool isCopy = Request.Params["CtrlKeyField"] == "True";
                if (isCopy)
                    Bll.GroupCategoryAspect.Copy(SourceID, DestID);
                else Bll.GroupCategoryAspect.Move(SourceID, DestID);
            }
            if (!String.IsNullOrEmpty(ErrorMessage))
                rapTreeView.ResponseScripts.Add("alert('" + ErrorMessage.Replace("'", "\\'") + "');");                        
            GenerateTreeView();
        }
    }
}