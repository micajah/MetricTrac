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
    public partial class MasterDataTree : System.Web.UI.Page
    {        
        // path
        List<Bll.MasterData> Data = null;
        // junc
        List<Bll.Metric.MetricPIJunc> JuncMetrics = null;
        List<Bll.PerformanceIndicatorFormPerformanceIndicatorJunc> JuncPIFPI = null;
        List<Bll.Mc_EntityNode.PIFOrgLocationJunc> JuncPIFOrgLocation = null;
        // entities
        List<Bll.PerformanceIndicator.Extend> PI = null;
        List<Bll.PerformanceIndicatorForm> PIF = null;
        List<Bll.Metric.Extend> M = null;
        List<Bll.GroupCategoryAspect> GCA = null;
        List<Bll.D_GroupCategoryAspect> D_GCA = null;
        List<Bll.MetricCategory> MC = null;
        List<Bll.D_MetricCategory> D_MC = null;
        List<Bll.Mc_EntityNode.Extend> OrgLocations = null;
        List<Bll.D_EntityNode> D_OrgLocations = null;

        private const string PINodeValue = "PerformanceIndicatorList.aspx";
        private const string PIFormNodeValue = "PIFList.aspx";
        private const string GCANodeValue = "GCATree.aspx";
        private const string MCNodeValue = "MetricCategoryTree.aspx";
        private const string OrgNodeValue = "/Resources.Micajah.Common/Pages/Admin/OrganizationEntity.aspx?EntityId=4cda22f34f0147688608938dc6a06825";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadRootNodes();
        }

        private void LoadRootNodes()
        {
            rtvMaster.Nodes.Clear();
            Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;
            RadTreeNode RootNode = new RadTreeNode(user.SelectedOrganization.Name, "");
            RootNode.Expanded = true;
            rtvMaster.Nodes.Add(RootNode);
            RadTreeNode RootPINode = new RadTreeNode("Performance Indicators", PINodeValue);
            RootPINode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;            
            RootNode.Nodes.Add(RootPINode);
            RadTreeNode RootPIFormNode = new RadTreeNode("Performance Indicator Forms", PIFormNodeValue);
            RootPIFormNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
            RootNode.Nodes.Add(RootPIFormNode);
            RadTreeNode RootGCANode = new RadTreeNode("GCA Subtree", GCANodeValue);
            RootGCANode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
            RootNode.Nodes.Add(RootGCANode);
            RadTreeNode RootMCNode = new RadTreeNode("Metric Categories Subtree", MCNodeValue);
            RootMCNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
            RootNode.Nodes.Add(RootMCNode);
            RadTreeNode RootOrgNode = new RadTreeNode("Org Location Subtree", OrgNodeValue);
            RootOrgNode.ExpandMode = TreeNodeExpandMode.ServerSideCallBack;
            RootNode.Nodes.Add(RootOrgNode);           
        }


        protected void rtvMaster_NodeExpand(object sender, RadTreeNodeEventArgs e)
        {
            Data = Bll.MasterData.GetMasterData().ToList(); // with virtual
            JuncMetrics = Bll.PerformanceIndicator.PIMetricJuncList().ToList();
            JuncPIFPI = Bll.PerformanceIndicatorFormPerformanceIndicatorJunc.PIFPIJuncList().ToList();
            JuncPIFOrgLocation = Bll.Mc_EntityNode.PIFOrgLocationsJuncList().ToList();
            PI = Bll.PerformanceIndicator.List().ToList();// active pi
            PIF = Bll.PerformanceIndicatorForm.List().ToList(); // active pif
            M = Bll.Metric.List().ToList(); // active metrics
            GCA = Bll.GroupCategoryAspect.SelectAll().ToList(); // active gca
            D_GCA = Bll.GroupCategoryAspect.D_SelectAll().ToList(); // active gca                
            MC = Bll.MetricCategory.SelectAll().ToList(); // active mc
            D_MC = Bll.MetricCategory.D_SelectAll().ToList(); // active mc
            OrgLocations = Bll.Mc_EntityNode.ListOrgLocations();// active locations
            D_OrgLocations = Bll.Mc_EntityNode.D_ListOrgLocations();// active locations
            switch (e.Node.Value)
            {
                case PINodeValue:                    
                    BuildPISubTree(e.Node);
                    break;
                case PIFormNodeValue:                    
                    BuildPIFormSubTree(e.Node);
                    break;
                case GCANodeValue:
                    BuildGCASubTree(e.Node);
                    break;
                case MCNodeValue:
                    BuildMCSubTree(e.Node);
                    break;
                case OrgNodeValue:
                    BuildLocationSubTree(e.Node);
                    break;
            }
            e.Node.Expanded = true;
            e.Node.ExpandMode = TreeNodeExpandMode.ClientSide;
        }

        private void BuildPISubTree(RadTreeNode RootNode)
        {            
            RadTreeNode node = null;
            foreach (Bll.PerformanceIndicator.Extend pi in PI)
            {
                node = new RadTreeNode(pi.Name, "PerformanceIndicatorEdit.aspx?PerformanceIndicatorID=" + pi.PerformanceIndicatorID.ToString());
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("book.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(node);
                //AppendMetricsByCategory(pi.PerformanceIndicatorID, node);
                RadTreeNode Mnode = new RadTreeNode("Metrics", String.Empty);
                node.Nodes.Add(Mnode);
                List<Guid> PIMetricsID = JuncMetrics.Where(d => d.PerformanceIndicatorID == pi.PerformanceIndicatorID).Select(r => r.MetricID).Distinct().ToList();
                AppendMetrics(Mnode, M.Where(m => PIMetricsID.Contains(m.MetricID)).ToList());
                AppendPILocations(pi.PerformanceIndicatorID, node);
            }            
        }

        private void AppendMetricsByCategory(Guid PI, RadTreeNode PINode)
        {
            RadTreeNode RootMNode = new RadTreeNode("Metrics Grouped By Categories", "MetricList.aspx");
            PINode.Nodes.Add(RootMNode);
            List<Guid> PIMetricsID = JuncMetrics.Where(d => d.PerformanceIndicatorID == PI).Select(r => r.MetricID).Distinct().ToList();
            List<Bll.Metric.Extend> PIMetrics = M.Where(m => PIMetricsID.Contains(m.MetricID)).ToList();
            List<Guid> UsedMCID = PIMetrics.Where(m => m.MetricCategoryID != null).Select(m => (Guid)m.MetricCategoryID).Distinct().ToList();
            List<Guid> NecesseryMCID = D_MC.Where(dmc => UsedMCID.Contains(dmc.IncludedID)).Select(dmc => dmc.MetricCategoryID).Distinct().ToList();
            List<Bll.MetricCategory> NecesseryMC = MC.Where(mc => NecesseryMCID.Contains(mc.MetricCategoryID)).ToList();
            BuildMByCSubTree(RootMNode, null, NecesseryMC, PIMetrics);
            List<MetricTrac.Bll.Metric.Extend> NoMCMetrics = PIMetrics.Where(m => m.MetricCategoryID == null).ToList();
            if (NoMCMetrics.Count > 0)
            {
                RadTreeNode EmptyNode = new RadTreeNode("No Category Metrics", String.Empty);
                EmptyNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootMNode.Nodes.Add(EmptyNode);
                AppendMetrics(EmptyNode, NoMCMetrics);
            }
        }

        private void BuildMByCSubTree(RadTreeNode RootNode, Guid? ParentID, List<Bll.MetricCategory> NecesseryMC, List<Bll.Metric.Extend> PIMetrics)
        {
            RadTreeNode node = null;
            List<Bll.MetricCategory> MCLevel = NecesseryMC.Where(p => p.ParentId == ParentID).ToList(); // first time - null
            foreach (MetricTrac.Bll.MetricCategory CurMC in MCLevel)
            {
                node = new RadTreeNode(CurMC.Name, "MetricCategoryTree.aspx");
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("application.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(node);
                BuildMByCSubTree(node, CurMC.MetricCategoryID, NecesseryMC, PIMetrics);
                AppendMetrics(node, PIMetrics.Where(m => m.MetricCategoryID == CurMC.MetricCategoryID).ToList());
            }
        }

        private void AppendMetrics(RadTreeNode RootNode, List<MetricTrac.Bll.Metric.Extend> Metrics)
        {
            RadTreeNode node = null;
            foreach (Bll.Metric.Extend m in Metrics)
            {
                node = new RadTreeNode(m.Name, "MetricEdit.aspx?MetricID=" + m.MetricID.ToString());
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("book_open.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(node);              
            }
        }

        private void AppendPILocations(Guid PI, RadTreeNode PINode)
        {
            List<Guid> PIFArray = JuncPIFPI.Where(p => p.PerformanceIndicatorID == PI).Select(p => p.PerformanceIndicatorFormID).Distinct().ToList();
            List<Guid> LocArray = JuncPIFOrgLocation.Where(p => PIFArray.Contains(p.PerformanceIndicatorFormID)).Select(p => p.OrgLocationID).Distinct().ToList();
            List<Bll.Mc_EntityNode.Extend> AssignedOrgLocations = OrgLocations.Where(o => LocArray.Contains(o.EntityNodeId)).Distinct().ToList();
            RadTreeNode node = null;
            RadTreeNode RootOLNode = new RadTreeNode("Assigned Org Locations", String.Empty);
            PINode.Nodes.Add(RootOLNode);
            foreach (Bll.Mc_EntityNode.Extend ol in AssignedOrgLocations)
            {
                node = new RadTreeNode(ol.FullName, String.Empty);
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("world.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootOLNode.Nodes.Add(node);
            }
        }
        //===========================================

        private void BuildPIFormSubTree(RadTreeNode RootNode)
        {            
            RadTreeNode node = null;
            foreach (Bll.PerformanceIndicatorForm pif in PIF)
            {
                node = new RadTreeNode(pif.Name, "PIFEdit.aspx?PerformanceIndicatorFormID=" + pif.PerformanceIndicatorFormID.ToString());
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("disk.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(node);
                AppendPIByGCA(pif.PerformanceIndicatorFormID, node);
                AppendPIFLocations(pif.PerformanceIndicatorFormID, node);
                AppendPIFMetricsByCategory(pif.PerformanceIndicatorFormID, node);
            }
        }

        private void AppendPIByGCA(Guid PIF, RadTreeNode PIFNode)
        {
            RadTreeNode RootPINode = new RadTreeNode("Performance Indicators Grouped By GCA", "PerformanceIndicatorList.aspx");
            PIFNode.Nodes.Add(RootPINode);
            List<Guid> PIFPIID = JuncPIFPI.Where(d => d.PerformanceIndicatorFormID == PIF).Select(r => r.PerformanceIndicatorID).Distinct().ToList();
            List<Bll.PerformanceIndicator.Extend> PIFPI = PI.Where(pi => PIFPIID.Contains(pi.PerformanceIndicatorID)).ToList();
            List<Guid> UsedGCAID = PIFPI.Where(pi => pi.GroupCategoryAspectID != null).Select(pi => (Guid)pi.GroupCategoryAspectID).Distinct().ToList();
            List<Guid> NecesseryGCAID = D_GCA.Where(dgca => UsedGCAID.Contains(dgca.IncludedID)).Select(dgca => dgca.GroupCategoryAspectID).Distinct().ToList();
            List<Bll.GroupCategoryAspect> NecesseryGCA = GCA.Where(gca => NecesseryGCAID.Contains(gca.GroupCategoryAspectID)).ToList();
            BuildSubGCAPITree(RootPINode, null, NecesseryGCA, PIFPI, 0);
            List<MetricTrac.Bll.PerformanceIndicator.Extend> NoGCAPI = PIFPI.Where(m => m.GroupCategoryAspectID == null).ToList();
            if (NoGCAPI.Count > 0)
            {
                RadTreeNode EmptyNode = new RadTreeNode("Performance Indicators without GCA", String.Empty);
                EmptyNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootPINode.Nodes.Add(EmptyNode);
                AppendPI(EmptyNode, NoGCAPI);
            }
        }

        private void BuildSubGCAPITree(RadTreeNode RootNode, Guid? ParentID, List<Bll.GroupCategoryAspect> NecesseryGCA, List<Bll.PerformanceIndicator.Extend> PIFPI, int level)
        {
            RadTreeNode node = null;
            List<Bll.GroupCategoryAspect> GCALevel = NecesseryGCA.Where(p => p.ParentId == ParentID).ToList(); // first time - null
            foreach (MetricTrac.Bll.GroupCategoryAspect CurGCA in GCALevel)
            {
                node = new RadTreeNode(CurGCA.Name, "GCATree.aspx");
                string GCAImage = String.Empty;
                switch (level)
                {
                    case 1:
                        GCAImage = "folder.png";
                        break;
                    case 2:
                        GCAImage = "note.png";
                        break;
                    case 0:
                    default:
                        GCAImage = "box.png";
                        break;
                }
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl(GCAImage, Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(node);
                BuildSubGCAPITree(node, CurGCA.GroupCategoryAspectID, NecesseryGCA, PIFPI, level + 1);
                AppendPI(node, PIFPI.Where(m => m.GroupCategoryAspectID == CurGCA.GroupCategoryAspectID).ToList());
            }
        }

        private void AppendPI(RadTreeNode RootNode, List<Bll.PerformanceIndicator.Extend> PI)
        {
            RadTreeNode node = null;
            foreach (Bll.PerformanceIndicator.Extend pi in PI)
            {
                node = new RadTreeNode(pi.Name, "PerformanceIndicatorEdit.aspx?PerformanceIndicatorID=" + pi.PerformanceIndicatorID.ToString());
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("book.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(node);
                AppendMetricsByCategory(pi.PerformanceIndicatorID, node);
                AppendPILocations(pi.PerformanceIndicatorID, node);
            }
        }

        private void AppendPIFLocations(Guid PIF, RadTreeNode PIFNode)
        {            
            List<Guid> LocArray = JuncPIFOrgLocation.Where(p => p.PerformanceIndicatorFormID == PIF).Select(p => p.OrgLocationID).Distinct().ToList();
            List<Bll.Mc_EntityNode.Extend> AssignedOrgLocations = OrgLocations.Where(o => LocArray.Contains(o.EntityNodeId)).Distinct().ToList();
            RadTreeNode node = null;
            RadTreeNode RootOLNode = new RadTreeNode("Assigned Org Locations", String.Empty);
            PIFNode.Nodes.Add(RootOLNode);
            foreach (Bll.Mc_EntityNode.Extend ol in AssignedOrgLocations)
            {
                node = new RadTreeNode(ol.FullName, String.Empty);
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("world.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootOLNode.Nodes.Add(node);
            }
        }

        private void AppendPIFMetricsByCategory(Guid PIF, RadTreeNode PIFNode)
        {
            RadTreeNode RootMNode = new RadTreeNode("Metrics Grouped By Categories", "MetricList.aspx");
            PIFNode.Nodes.Add(RootMNode);
            List<Guid> PIFPIID = JuncPIFPI.Where(d => d.PerformanceIndicatorFormID == PIF).Select(r => r.PerformanceIndicatorID).Distinct().ToList();
            List<Guid> PIFMetricsID = JuncMetrics.Where(d => PIFPIID.Contains(d.PerformanceIndicatorID)).Select(r => r.MetricID).Distinct().ToList();
            List<Bll.Metric.Extend> PIFMetrics = M.Where(m => PIFMetricsID.Contains(m.MetricID)).ToList();
            List<Guid> UsedMCID = PIFMetrics.Where(m => m.MetricCategoryID != null).Select(m => (Guid)m.MetricCategoryID).Distinct().ToList();
            List<Guid> NecesseryMCID = D_MC.Where(dmc => UsedMCID.Contains(dmc.IncludedID)).Select(dmc => dmc.MetricCategoryID).Distinct().ToList();
            List<Bll.MetricCategory> NecesseryMC = MC.Where(mc => NecesseryMCID.Contains(mc.MetricCategoryID)).ToList();
            BuildMByCSubTree(RootMNode, null, NecesseryMC, PIFMetrics);
            List<MetricTrac.Bll.Metric.Extend> NoMCMetrics = PIFMetrics.Where(m => m.MetricCategoryID == null).ToList();
            if (NoMCMetrics.Count > 0)
            {
                RadTreeNode EmptyNode = new RadTreeNode("No Category Metrics", String.Empty);
                EmptyNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootMNode.Nodes.Add(EmptyNode);
                AppendMetrics(EmptyNode, NoMCMetrics);
            }
        }        
        //===========================================

        private void BuildGCASubTree(RadTreeNode RootNode)
        {
            BuildSubGCAPITree(RootNode, null, GCA, PI, 0);
            List<MetricTrac.Bll.PerformanceIndicator.Extend> NoGCAPI = PI.Where(m => m.GroupCategoryAspectID == null).ToList();
            if (NoGCAPI.Count > 0)
            {
                RadTreeNode EmptyNode = new RadTreeNode("Performance Indicators without GCA", String.Empty);
                EmptyNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(EmptyNode);
                AppendPI(EmptyNode, NoGCAPI);
            }
        }       
        //===========================================

        private void BuildMCSubTree(RadTreeNode RootNode)
        {
            BuildMByCSubTree(RootNode, null, MC, M);
            List<MetricTrac.Bll.Metric.Extend> NoMCMetrics = M.Where(m => m.MetricCategoryID == null).ToList();
            if (NoMCMetrics.Count > 0)
            {
                RadTreeNode EmptyNode = new RadTreeNode("No Category Metrics", String.Empty);
                EmptyNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(EmptyNode);
                AppendMetrics(EmptyNode, NoMCMetrics);
            }
        }

        //===========================================

        private void BuildLocationSubTree(RadTreeNode RootNode)
        {
            BuildOrgLocationSubTree(RootNode, null, OrgLocations);
            // PIF with no locs
            List<Guid> PIFArray = JuncPIFOrgLocation.Select(p => p.PerformanceIndicatorFormID).Distinct().ToList();
            List<Bll.PerformanceIndicatorForm> PIFWithNoOrg = PIF.Where(pif => !PIFArray.Contains(pif.PerformanceIndicatorFormID)).Distinct().ToList();                        
            if (PIFWithNoOrg.Count > 0)
            {
                RadTreeNode PIFNoLocsNode = new RadTreeNode("Performance Indicator Forms without assigned Org Locations", "");
                PIFNoLocsNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(PIFNoLocsNode);
                RadTreeNode node = null;
                foreach (Bll.PerformanceIndicatorForm pif in PIFWithNoOrg)
                {
                    node = new RadTreeNode(pif.Name, "PIFEdit.aspx?PerformanceIndicatorFormID=" + pif.PerformanceIndicatorFormID.ToString());
                    node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("disk.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                    PIFNoLocsNode.Nodes.Add(node);
                }                
            }
            // PI with no locs
            List<Guid> OrgPIFID = JuncPIFOrgLocation.Select(r => r.PerformanceIndicatorFormID).Distinct().ToList();
            List<Guid> OrgPIID = JuncPIFPI.Where(d => OrgPIFID.Contains(d.PerformanceIndicatorFormID)).Select(r => r.PerformanceIndicatorID).Distinct().ToList();
            List<Bll.PerformanceIndicator.Extend> PIWithNoOrg = PI.Where(m => !OrgPIID.Contains(m.PerformanceIndicatorID)).ToList();
            if (PIWithNoOrg.Count > 0)
            {
                RadTreeNode PINoLocsNode = new RadTreeNode("Performance Indicators without assigned Org Locations", "");
                PINoLocsNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(PINoLocsNode);
                RadTreeNode node = null;
                foreach (Bll.PerformanceIndicator.Extend pi in PIWithNoOrg)
                {
                    node = new RadTreeNode(pi.Name, "PerformanceIndicatorEdit.aspx?PerformanceIndicatorID=" + pi.PerformanceIndicatorID.ToString());
                    node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("book.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                    PINoLocsNode.Nodes.Add(node);
                }
            }
            // metrics with no locs
            List<Guid> OrgLocMetricID = Data.Where(d => !d.IsVirtual).Select(d => d.MetricID).Distinct().ToList();
            List<Bll.Metric.Extend> MetricsWithNoOrgLoc = M.Where(m => !OrgLocMetricID.Contains(m.MetricID)).ToList();
            if (MetricsWithNoOrgLoc.Count > 0)
            {
                RadTreeNode EmptyNode = new RadTreeNode("Metrics without assigned Org Locations", String.Empty);
                EmptyNode.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("stop.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(EmptyNode);
                AppendMetrics(EmptyNode, MetricsWithNoOrgLoc);
            }
        }

        private void BuildOrgLocationSubTree(RadTreeNode RootNode, Guid? ParentID, List<Bll.Mc_EntityNode.Extend> OrgLocs)
        {
            RadTreeNode node = null;
            List<Bll.Mc_EntityNode.Extend> OrgLevel = OrgLocs.Where(p => p.ParentEntityNodeId == ParentID).ToList(); // first time - null
            foreach (Bll.Mc_EntityNode.Extend Loc in OrgLevel)
            {
                node = new RadTreeNode(Loc.Name, "/Resources.Micajah.Common/Pages/Admin/OrganizationEntity.aspx?EntityId=4cda22f34f0147688608938dc6a06825");
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("world.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootNode.Nodes.Add(node);
                BuildOrgLocationSubTree(node, Loc.EntityNodeId, OrgLocs);                
                AppendOrgPIF(Loc.EntityNodeId, node);
                AppendOrgPI(Loc.EntityNodeId, node);
                AppendOrgLocationMetrics(Loc.EntityNodeId, node);
            }            
        }

        private void AppendOrgPIF(Guid OrgLocationID, RadTreeNode RootNode)
        {
            List<Guid> PIFArray = JuncPIFOrgLocation.Where(p => p.OrgLocationID == OrgLocationID).Select(p => p.PerformanceIndicatorFormID).Distinct().ToList();
            List<Bll.PerformanceIndicatorForm> OrgPIF = PIF.Where(pif => PIFArray.Contains(pif.PerformanceIndicatorFormID)).Distinct().ToList();
            RadTreeNode node = null;
            RadTreeNode RootPIFNode = new RadTreeNode("Assigned Performance Indicator Forms", String.Empty);
            RootNode.Nodes.Add(RootPIFNode);
            foreach (Bll.PerformanceIndicatorForm pif in OrgPIF)
            {
                node = new RadTreeNode(pif.Name, "PIFEdit.aspx?PerformanceIndicatorFormID=" + pif.PerformanceIndicatorFormID.ToString());
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("disk.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootPIFNode.Nodes.Add(node);
                AppendPIByGCA(pif.PerformanceIndicatorFormID, node);
                AppendPIFLocations(pif.PerformanceIndicatorFormID, node);
                AppendPIFMetricsByCategory(pif.PerformanceIndicatorFormID, node);
            }
        }

        private void AppendOrgPI(Guid OrgLocationID, RadTreeNode RootNode)
        {
            RadTreeNode RootPINode = new RadTreeNode("Assigned Performance Indicators", String.Empty);
            RootNode.Nodes.Add(RootPINode);
            RadTreeNode node = null;

            List<Guid> OrgPIFID = JuncPIFOrgLocation.Where(d => d.OrgLocationID == OrgLocationID).Select(r => r.PerformanceIndicatorFormID).Distinct().ToList();
            List<Guid> OrgPIID = JuncPIFPI.Where(d => OrgPIFID.Contains(d.PerformanceIndicatorFormID)).Select(r => r.PerformanceIndicatorID).Distinct().ToList();
            List<Bll.PerformanceIndicator.Extend> OrgPI = PI.Where(m => OrgPIID.Contains(m.PerformanceIndicatorID)).ToList();
            foreach (Bll.PerformanceIndicator.Extend pi in OrgPI)
            {
                node = new RadTreeNode(pi.Name, "PerformanceIndicatorEdit.aspx?PerformanceIndicatorID=" + pi.PerformanceIndicatorID.ToString());
                node.ImageUrl = Micajah.Common.Bll.Providers.ResourceProvider.GetIconImageUrl("book.png", Micajah.Common.WebControls.IconSize.Smaller, true);
                RootPINode.Nodes.Add(node);
                AppendMetricsByCategory(pi.PerformanceIndicatorID, node);
                AppendPILocations(pi.PerformanceIndicatorID, node);
            }
        }

        private void AppendOrgLocationMetrics(Guid OrgLocID, RadTreeNode OrgLocNode)
        {
            RadTreeNode RootMNode = new RadTreeNode("Metrics Grouped By Categories", "MetricList.aspx");
            OrgLocNode.Nodes.Add(RootMNode);
            List<Guid> OrgLocMetricID = Data.Where(d => ((d.OrgLocationID == OrgLocID) && (!d.IsVirtual))).Select(d => d.MetricID).Distinct().ToList();
            List<Bll.Metric.Extend> OrgLocMetrics = M.Where(m => OrgLocMetricID.Contains(m.MetricID)).ToList();
            List<Guid> UsedMCID = OrgLocMetrics.Where(m => m.MetricCategoryID != null).Select(m => (Guid)m.MetricCategoryID).Distinct().ToList();
            List<Guid> NecesseryMCID = D_MC.Where(dmc => UsedMCID.Contains(dmc.IncludedID)).Select(dmc => dmc.MetricCategoryID).Distinct().ToList();
            List<Bll.MetricCategory> NecesseryMC = MC.Where(mc => NecesseryMCID.Contains(mc.MetricCategoryID)).ToList();
            BuildMByCSubTree(RootMNode, null, NecesseryMC, OrgLocMetrics);            
        }
    }
}