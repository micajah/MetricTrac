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
    public partial class MetricCategoryTree : System.Web.UI.Page
    {
        private string MetricSuffix = "PIM";
        private string EmptyNodeID = "ENS";
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
            Micajah.Common.Security.UserContext.Breadcrumbs.Add("Metric Categories", String.Empty, String.Empty);         
            if (!IsPostBack) GenerateTreeView();
        }

        private void GenerateTreeView()
        {
            System.Collections.Generic.List<string> ExpandedNodes = new System.Collections.Generic.List<string>();
            GetExpandedNodes(rtvMetricCategory.Nodes, ExpandedNodes);

            rtvMetricCategory.Nodes.Clear();

            IQueryable<Bll.MetricCategory> _MetricCategories = Bll.MetricCategory.SelectAll();
            IQueryable<Bll.Metric.Extend> _Metrics = Bll.Metric.List();
            
            RadTreeNode RootNode = new RadTreeNode("Metric Categories Tree", "0");
            RootNode.Expanded = true;
            RootNode.AllowDrag = false;
            RootNode.AllowDrop = false;
            RootNode.AllowEdit = false;
            RootNode.ContextMenuID = "RootContextMenu";

            rtvMetricCategory.Nodes.Add(RootNode);
            RadTreeNode _Node = null;

            IQueryable<Bll.MetricCategory> MCLevel0 = _MetricCategories.Where(p => p.ParentId == null);

            foreach (MetricTrac.Bll.MetricCategory metriccategory in MCLevel0)
            {
                _Node = new RadTreeNode(metriccategory.Name, metriccategory.MetricCategoryID.ToString());                    
                _Node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("application.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                _Node.ContextMenuID = "MiddleContextMenu";
                _Node.AllowDrag = true;
                _Node.AllowDrop = true;
                RootNode.Nodes.Add(_Node);
                SetExpandedNode(_Node, ExpandedNodes);
                BuildSubTree(_MetricCategories, _MetricCategories.Where(mc => mc.ParentId == metriccategory.MetricCategoryID), _Node, metriccategory.MetricCategoryID, 0, ExpandedNodes, _Metrics);
                AppendMetrics(_Node, _Metrics.Where(m => m.MetricCategoryID == metriccategory.MetricCategoryID), ExpandedNodes);
            }            
            if (_Metrics.Count(m => m.MetricCategoryID == null) > 0)
            {
                RadTreeNode EmptyNode = new RadTreeNode("Other Metrics", EmptyNodeID);
                EmptyNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                EmptyNode.EnableContextMenu = false;
                EmptyNode.AllowDrag = false;
                EmptyNode.AllowDrop = false;
                EmptyNode.AllowEdit = false;
                RootNode.Nodes.Add(EmptyNode);
                SetExpandedNode(EmptyNode, ExpandedNodes);
                AppendMetrics(EmptyNode, _Metrics.Where(m => m.MetricCategoryID == null), ExpandedNodes);
            }
        }

        private void AppendMetrics(RadTreeNode RootNode, IQueryable<MetricTrac.Bll.Metric.Extend> Metrics, System.Collections.Generic.List<string> ExpandedNodes)
        {
            foreach (Bll.Metric.Extend mj in Metrics)
            {
                RadTreeNode MetricNode = new RadTreeNode(mj.Name, mj.MetricID.ToString() + "|" + MetricSuffix);
                MetricNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("book_open.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                MetricNode.EnableContextMenu = false;
                MetricNode.AllowDrag = true;
                MetricNode.AllowDrop = false;
                MetricNode.AllowEdit = false;
                RootNode.Nodes.Add(MetricNode);
                SetExpandedNode(MetricNode, ExpandedNodes);
            }
        }
        
        private void BuildSubTree(IQueryable<MetricTrac.Bll.MetricCategory> AllMetricCategories, IQueryable<MetricTrac.Bll.MetricCategory> _MetricCategories, RadTreeNode RootNode, Guid guid, int level, System.Collections.Generic.List<string> ExpandedNodes, IQueryable<MetricTrac.Bll.Metric.Extend> Metrics)
        {
            foreach (MetricTrac.Bll.MetricCategory MetricCategory in _MetricCategories)
            {  
                RadTreeNode SomeNode = new RadTreeNode(MetricCategory.Name, MetricCategory.MetricCategoryID.ToString());
                if (level == 0)
                {                        
                    SomeNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("application.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                    SomeNode.ContextMenuID = "MiddleContextMenu";
                }
                else
                {                        
                    SomeNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("application.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                    SomeNode.ContextMenuID = "LeafContextMenu";                        
                }
                SomeNode.AllowDrag = true;
                SomeNode.AllowDrop = true;
                RootNode.Nodes.Add(SomeNode);
                SetExpandedNode(SomeNode, ExpandedNodes);
                BuildSubTree(AllMetricCategories, AllMetricCategories.Where(mc => mc.ParentId == MetricCategory.MetricCategoryID), SomeNode, MetricCategory.MetricCategoryID, 1, ExpandedNodes, Metrics);
                AppendMetrics(SomeNode, Metrics.Where(m => m.MetricCategoryID == MetricCategory.MetricCategoryID), ExpandedNodes);
            }
        }

        // treeview event handlers
        protected void rtvMetricCategory_ContextMenuItemClick(object sender, RadTreeViewContextMenuEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Node.Value)) return;
            Guid ID = new Guid(e.Node.Value);            
            if (string.IsNullOrEmpty(e.MenuItem.Value)) return;
            string[] command = e.MenuItem.Value.Split(' ');
            if (command[0] == "Delete")
            {
                MetricTrac.Bll.MetricCategory.Delete(ID);
                GenerateTreeView();
            }
        }

        protected void rtvMetricCategory_NodeEdit(object sender, RadTreeNodeEditEventArgs e)
        {            
            if (string.IsNullOrEmpty(e.Node.Value))
            {
                Guid? ParentID = null;
                if (e.Node.ParentNode.Value != "0") ParentID = new Guid(e.Node.ParentNode.Value);
                Bll.MetricCategory MetricCategory = new Bll.MetricCategory();
                MetricCategory.ParentId = ParentID;
                MetricCategory.Name = e.Text;
                MetricCategory.InstanceId = Bll.LinqMicajahDataContext.InstanceId;
                Guid _InsertedItemID = Bll.MetricCategory.Insert(MetricCategory);
                e.Node.Expanded = true;
            }
            else
            {                
                if (!String.IsNullOrEmpty(e.Node.Value) && e.Node.Value != "0")
                {
                    Guid ID = new Guid(e.Node.Value);
                    Bll.MetricCategory.Update(ID, e.Text);
                }
            }
            GenerateTreeView();
        }

        protected void rtvMetricCategory_NodeDrop(object sender, RadTreeNodeDragDropEventArgs e)
        {
            string ErrorMessage = String.Empty;
            if (e.SourceDragNode == null || e.DestDragNode == null) return;
            int SourceLevel = e.SourceDragNode.Level;
            string SourceValue = e.SourceDragNode.Value;
            int DestLevel = e.DestDragNode.Level;
            string DestValue = e.DestDragNode.Value;

            if (SourceLevel > 0 &&
                SourceValue != EmptyNodeID &&
                DestLevel > 0 &&
                DestLevel <= 3 &&
                DestValue != SourceValue)
            {
                if (SourceValue.Contains(MetricSuffix))
                {
                    if (!DestValue.Contains(MetricSuffix) || DestValue == EmptyNodeID)
                    {
                        string[] ComplexID = SourceValue.Split('|');
                        Guid MetricID = new Guid(ComplexID[0]);
                        Guid? DestID = null;
                        if (DestValue != EmptyNodeID)
                            DestID = new Guid(e.DestDragNode.Value);
                        Bll.Metric.ChangeMetricCategory(MetricID, DestID);
                    }
                }
                else
                {
                    if (SourceLevel <= 3 && !DestValue.Contains(MetricSuffix) && DestValue != EmptyNodeID && (DestLevel == SourceLevel - 1 || DestLevel == SourceLevel))
                    {
                        bool IsMerge = SourceLevel == DestLevel;                        
                        Guid SourceID = new Guid(SourceValue);
                        Guid DestID = new Guid(DestValue);
                        if (IsMerge)
                            Bll.MetricCategory.Merge(SourceID, DestID);
                        else
                        {
                            bool isCopy = Request.Params["CtrlKeyField"] == "True";
                            if (isCopy)
                                Bll.MetricCategory.Copy(SourceID, DestID);
                            else
                                Bll.MetricCategory.Move(SourceID, DestID);
                        }
                    }
                }
            }
            if (!String.IsNullOrEmpty(ErrorMessage))
                rapTreeView.ResponseScripts.Add("alert('" + ErrorMessage.Replace("'", "\\'") + "');");
            GenerateTreeView();
        }
    }
}