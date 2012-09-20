using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class Mc_EntityNode
    {
        public class Extend : Mc_EntityNode
        {
            public string FullName { get; set; }
        }

        public static List<Extend> ListOrgLocations()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;            

            var org =(from n in dc.Mc_EntityNode
                    join _f in dc.EntityNodeFullNameView on /*new { InstanceId=(Guid?)LinqMicajahDataContext.InstanceId, */n.EntityNodeId/* }*/ equals /*new {InstanceId=(Guid?)null, */_f.EntityNodeId /*}*/ into __f
                    from f in __f.DefaultIfEmpty()

                      orderby n.Name //n.OrderNumber

                    where n.Deleted == false && n.InstanceId == null
                        && (n.EntityId == new Guid("4cda22f3-4f01-4768-8608-938dc6a06825") || n.EntityNodeId==Guid.Empty)
                        && (n.OrganizationId == user.SelectedOrganization.OrganizationId || n.EntityNodeId==Guid.Empty)
                    select new Extend
                    {
                        EntityNodeId = n.EntityNodeId,
                        ParentEntityNodeId = n.ParentEntityNodeId,
                        Name = n.EntityNodeId == Guid.Empty ? LinqMicajahDataContext.OrganizationName : n.Name,
                        OrderNumber = n.OrderNumber,
                        OrganizationId = n.OrganizationId,
                        InstanceId = n.InstanceId,
                        EntityId = n.EntityId,
                        EntityNodeTypeId = n.EntityNodeTypeId,
                        SubEntityId = n.SubEntityId,
                        SubEntityLocalId = n.SubEntityLocalId,
                        Deleted = n.Deleted,

                        FullName = n.EntityNodeId == Guid.Empty ? LinqMicajahDataContext.OrganizationName : f.FullName
                    }).ToList();

            foreach (MetricTrac.Bll.Mc_EntityNode.Extend n in org) if (n.ParentEntityNodeId == null && n.EntityNodeId!=Guid.Empty) n.ParentEntityNodeId = Guid.Empty;
            return org;
        }

        public static List<D_EntityNode> D_ListOrgLocations()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;
            var org = (from n in dc.D_EntityNode                       
                       where n.OrganizationId == user.SelectedOrganization.OrganizationId
                       select n).ToList();            
            return org;
        }

        public static string GetFullName(Guid OrgLocationID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            return OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : (from e in dc.EntityNodeFullNameView where e.InstanceId == null && e.EntityNodeId == OrgLocationID select e.FullName).FirstOrDefault();
        }

        public static string GetHtmlFullName(Guid OrgLocationID)
        {
            string FullName = GetFullName(OrgLocationID);
            return GetHtmlFullName(FullName);
        }
        public static string GetHtmlFullName(string FullName)
        {
            if (!String.IsNullOrEmpty(FullName))
            {
                int n = FullName.LastIndexOf(">");
                if (n < 0) n = 0;
                FullName = FullName.Insert(n, "<b>") + "</b>";
                return FullName;
            }
            else
                return "Deleted Org Location";
        }

        // junc with org locations
        public class PIFOrgLocationJunc
        {
            public Guid PerformanceIndicatorFormID { get; set; }
            public Guid OrgLocationID { get; set; }         
        }
        
        public static IQueryable<PIFOrgLocationJunc> PIFOrgLocationsJuncList()// ask Yura what is relation type
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var org = from r in dc.PIFormOrgLocationJuncView
                      where r.InstanceId == LinqMicajahDataContext.InstanceId
                       select new PIFOrgLocationJunc
                       {
                           PerformanceIndicatorFormID = r.PerformanceIndicatorFormID,
                           OrgLocationID = r.OrgLocationID                           
                       };
            return org;
        }
    }
}
