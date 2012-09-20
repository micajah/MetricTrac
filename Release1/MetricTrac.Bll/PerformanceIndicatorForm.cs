using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Collections;

namespace MetricTrac.Bll
{
    public partial class PerformanceIndicatorForm
    {
        public class PIFOrgLocation
        {
            public string Name {get; set; }
            public bool Child { get; set; }
        }

        public class Extend : PerformanceIndicatorForm
        {
            public List<PIFOrgLocation> OrgLocations;
            public bool IsVirtual { get; set; }
        }
        
        // === PerformanceIndicatorForm List - Main Methods for PIFList page ===
        public static IQueryable<PerformanceIndicatorForm> List()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IQueryable<PerformanceIndicatorForm> result =
                from pif in dc.PerformanceIndicatorForm
                where (pif.InstanceId == LinqMicajahDataContext.InstanceId) && (pif.Status == true)
                select pif;
            return result;
        }

        public static List<PerformanceIndicatorForm.Extend> List(Guid OrgLocationID, bool SubLocations)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            List<PerformanceIndicatorForm.Extend> r;
            if (OrgLocationID == Guid.Empty && SubLocations)
            {
                r = (
                        from j in dc.PIFormOrgLocationJuncView
                        join f in dc.PerformanceIndicatorForm on new { LinqMicajahDataContext.InstanceId, Status = (bool?)true, PerformanceIndicatorFormID = j.PerformanceIndicatorFormID } equals new { f.InstanceId, f.Status, f.PerformanceIndicatorFormID }
                        select new Extend{
                            Name = f.Name,
                            PerformanceIndicatorFormID = f.PerformanceIndicatorFormID,

                            Status = f.Status,
                            Created = f.Created,
                            Updated = f.Updated
                        }
                    ).Distinct().ToList();
            }
            else
            {
                r = (
                        from j in dc.PIFormOrgLocationJuncView
                        join f in dc.PerformanceIndicatorForm on new { LinqMicajahDataContext.InstanceId, Status = (bool?)true, PerformanceIndicatorFormID = j.PerformanceIndicatorFormID } equals new { f.InstanceId, f.Status, f.PerformanceIndicatorFormID }
                        join n in dc.EntityNodeFullNameView on new { j.OrgLocationID } equals new { OrgLocationID = n.EntityNodeId }
                        where j.OrgLocationID == OrgLocationID || (SubLocations && (n.Parent1 == OrgLocationID || n.Parent2 == OrgLocationID || n.Parent3 == OrgLocationID || n.Parent4 == OrgLocationID || n.Parent5 == OrgLocationID))
                        select new Extend
                        {
                            Name = f.Name,
                            PerformanceIndicatorFormID = f.PerformanceIndicatorFormID,

                            Status = f.Status,
                            Created = f.Created,
                            Updated = f.Updated
                        }
                    ).Distinct().ToList();
            }

            foreach (PerformanceIndicatorForm.Extend pif in r)
            {
                Guid? []p = new Guid?[5];
                var OrgLocationFullInfo = (from f in dc.EntityNodeFullNameView where f.EntityNodeId == OrgLocationID select f).FirstOrNull();
                p[0] = OrgLocationFullInfo.Parent1;
                p[1] = OrgLocationFullInfo.Parent2;
                p[2] = OrgLocationFullInfo.Parent3;
                p[3] = OrgLocationFullInfo.Parent4;
                p[4] = OrgLocationFullInfo.Parent5;

                pif.OrgLocations = (
                                from j in dc.PIFormOrgLocationJuncView
                                //join mc in dc.Mc_EntityNodesRelatedEntityNodes on j.EntityNodesRelatedEntityNodesId equals mc.EntityNodesRelatedEntityNodesId
                                join f in dc.EntityNodeFullNameView on j.OrgLocationID equals f.EntityNodeId

                                where j.PerformanceIndicatorFormID == pif.PerformanceIndicatorFormID && 
                                        (
                                            (
                                                j.OrgLocationID == OrgLocationID || 
                                                (
                                                    SubLocations && 
                                                    (
                                                        f.Parent1 == OrgLocationID || 
                                                        f.Parent2 == OrgLocationID || 
                                                        f.Parent3 == OrgLocationID || 
                                                        f.Parent4 == OrgLocationID || 
                                                        f.Parent5 == OrgLocationID
                                                    )
                                                )
                                            )
                                        )

                                select new PIFOrgLocation
                                {
                                    Name = f.FullName == "Organization Location" ? LinqMicajahDataContext.OrganizationName : f.FullName
                                    //Child = mc.RelationType == 2
                                }
                               ).Distinct().ToList();
            }

            return r;
        }
        public override void OnDeleting(LinqMicajahDataContext c, ref bool Cancel)
        {
            base.OnDeleting(c, ref Cancel);            
            Cancel = true;

            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                IQueryable rets =
                    from pifpij in dc.PerformanceIndicatorFormPerformanceIndicatorJunc
                    where (pifpij.InstanceId == LinqMicajahDataContext.InstanceId) 
                            &&
                          (pifpij.PerformanceIndicatorFormID == PerformanceIndicatorFormID)
                            &&
                          (pifpij.Status == true)
                    select pifpij;

                foreach (PerformanceIndicatorFormPerformanceIndicatorJunc pifpij in rets)
                    if (pifpij != null)                    
                        pifpij.Status = false;

                PerformanceIndicatorForm ret =
                     (from pif in dc.PerformanceIndicatorForm
                      where (pif.InstanceId == LinqMicajahDataContext.InstanceId) && (pif.PerformanceIndicatorFormID == PerformanceIndicatorFormID)
                      select pif).FirstOrNull();
                if (ret != null)                
                    ret.Status = false;
                dc.SubmitChanges();
            }            
        }

        public override void OnInserted(LinqMicajahDataContext dc)
        {
            base.OnInserted(dc);
            /*Mc_EntityNodesRelatedEntityNodes r = new Mc_EntityNodesRelatedEntityNodes{ 
                RelationType=1, 
                EntityNodesRelatedEntityNodesId = Guid.NewGuid(),
                RelatedEntityNodeId=Guid.Empty,
                EntityNodeId=this.PerformanceIndicatorFormID,
                EntityId = new Guid("4cda22f3-4f01-4768-8608-938dc6a06825")};
            dc.Mc_EntityNodesRelatedEntityNodes.InsertOnSubmit(r);
            dc.SubmitChanges();*/
        }

        // ==========================================

        // === PerformanceIndicatorForm Edit - Main Methods for PIFEdit page
        public static void AddPerformanceIndicatorsToForm(Guid PIFID, Guid[] PerfomanceIndicators)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                foreach (Guid pi in PerfomanceIndicators)
                {
                    PerformanceIndicatorFormPerformanceIndicatorJunc ret =
                    (from pifj in dc.PerformanceIndicatorFormPerformanceIndicatorJunc
                     where (pifj.InstanceId == LinqMicajahDataContext.InstanceId) && (pifj.PerformanceIndicatorFormID == PIFID) && (pifj.PerformanceIndicatorID == pi)
                     select pifj).FirstOrNull();
                    if (ret != null)
                        ret.Status = true;                    
                    else
                    {
                        PerformanceIndicatorFormPerformanceIndicatorJunc newPIFJ = new PerformanceIndicatorFormPerformanceIndicatorJunc();
                        newPIFJ.InstanceId = LinqMicajahDataContext.InstanceId;
                        newPIFJ.PerformanceIndicatorFormID = PIFID;
                        newPIFJ.PerformanceIndicatorID = pi;                        
                        dc.PerformanceIndicatorFormPerformanceIndicatorJunc.InsertOnSubmit(newPIFJ);
                    }
                }
                dc.SubmitChanges();
            }
        }

        public static void RemovePerformanceIndicator(Guid PIFID, Guid PerformanceIndicatorID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                PerformanceIndicatorFormPerformanceIndicatorJunc ret =
                    (from pifj in dc.PerformanceIndicatorFormPerformanceIndicatorJunc
                     where (pifj.InstanceId == LinqMicajahDataContext.InstanceId) && (pifj.PerformanceIndicatorFormID == PIFID) && (pifj.PerformanceIndicatorID == PerformanceIndicatorID)
                     select pifj).FirstOrNull();
                if (ret != null)                
                    ret.Status = false;
                dc.SubmitChanges();
            }
        }
        //=====================================

        // PerformanceIndicatorForm: unassigned Performance Indicator select
        public static List<PerformanceIndicator.Extend> UnassignedPerformanceIndicatorList(Guid PIFID, Guid GCAItemID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            Guid? refGCAId = null;
            if (GCAItemID != Guid.Empty) refGCAId = GCAItemID;
            ISingleResult<Sp_SelectUnassignedPerformanceIndicatorResult> result = dc.Sp_SelectUnassignedPerformanceIndicator(LinqMicajahDataContext.InstanceId, PIFID, refGCAId);
            List<PerformanceIndicator.Extend> lPI = new List<PerformanceIndicator.Extend>();
            foreach (Sp_SelectUnassignedPerformanceIndicatorResult i in result)
            {
                PerformanceIndicator.Extend pi = new PerformanceIndicator.Extend();
                pi.InstanceId = i.InstanceId== null ? Guid.Empty : (Guid)i.InstanceId;
                pi.PerformanceIndicatorID = i.PerformanceIndicatorID == null ? Guid.Empty : (Guid)i.PerformanceIndicatorID;
                pi.Name = i.Name.TrimEnd();                
                pi.Description = i.Description;
                pi.Code = i.Code;
                pi.GroupCategoryAspectID = i.GroupCategoryAspectID;
                pi.Status = i.Status;
                pi.Created = (DateTime)i.Created;
                pi.Updated = i.Updated;
                pi.SectorID = i.SectorID;
                pi.Help = i.Help;
                pi.RequirementID = i.RequirementID;
                pi.SectorName = i.Sector;
                pi.RequirementName = i.Requirement;
                pi.GCAName = i.GCA;
                lPI.Add(pi);
            }
            return lPI;
        }

        // Select actual Form's Performance Indicators from db
        public static IQueryable<PerformanceIndicator.Extend> AssignedPerformanceIndicatorsList(Guid PIFID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            var result =
                from pifj in dc.PerformanceIndicatorFormPerformanceIndicatorJunc
                join pi in dc.PerformanceIndicator on pifj.PerformanceIndicatorID equals pi.PerformanceIndicatorID
                join _s in dc.Sector on pi.SectorID equals _s.SectorID into __s
                join _g in dc.GCAFullNameView on new { pi.InstanceId, pi.GroupCategoryAspectID } equals new { _g.InstanceId, GroupCategoryAspectID = (Guid?)_g.GroupCategoryAspectID } into __g
                join _r in dc.Requirement on pi.RequirementID equals _r.RequirementID into __r

                from s in __s.DefaultIfEmpty()

                from g in __g.DefaultIfEmpty()

                from r in __r.DefaultIfEmpty()

                where pi.InstanceId == LinqMicajahDataContext.InstanceId && pifj.PerformanceIndicatorFormID == PIFID && pifj.Status == true
                orderby pi.SortCode, pi.Code, g.FullName, g.GroupCategoryAspectID, pi.Name
                select new PerformanceIndicator.Extend
                {
                    InstanceId = pi.InstanceId,
                    PerformanceIndicatorID = pi.PerformanceIndicatorID,
                    Name = pi.Name.TrimEnd(),
                    Description = pi.Description,
                    Code = pi.Code,
                    GroupCategoryAspectID = pi.GroupCategoryAspectID,
                    SectorID = pi.SectorID,
                    RequirementID = pi.RequirementID,

                    GCAName = g.FullName,
                    SectorName = s.Name,
                    RequirementName = r.Name,

                    Created = pi.Created,
                    Updated = pi.Updated,
                    Status = pi.Status
                };

            return result;
        }

        public static List<PerformanceIndicator.Extend> PerformanceIndicatorsList(Guid[] gSelectedPI)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            List<PerformanceIndicator.Extend> l = new List<PerformanceIndicator.Extend>();

            foreach (Guid piid in gSelectedPI)
            {
                var result =
                (from pi in dc.PerformanceIndicator
                 join _s in dc.Sector on pi.SectorID equals _s.SectorID into __s
                 join _g in dc.GCAFullNameView on new { pi.InstanceId, pi.GroupCategoryAspectID } equals new { _g.InstanceId, GroupCategoryAspectID = (Guid?)_g.GroupCategoryAspectID } into __g
                 join _r in dc.Requirement on pi.RequirementID equals _r.RequirementID into __r

                 from s in __s.DefaultIfEmpty()

                 from g in __g.DefaultIfEmpty()

                 from r in __r.DefaultIfEmpty()

                 where (pi.InstanceId == LinqMicajahDataContext.InstanceId) && (pi.PerformanceIndicatorID == piid)

                 orderby pi.SortCode, pi.Code, g.FullName, g.GroupCategoryAspectID, pi.Name

                 select new PerformanceIndicator.Extend
                    {
                        InstanceId = pi.InstanceId,
                        PerformanceIndicatorID = pi.PerformanceIndicatorID,
                        Name = pi.Name.TrimEnd(),
                        Description = pi.Description,
                        Code = pi.Code,
                        GroupCategoryAspectID = pi.GroupCategoryAspectID,
                        SectorID = pi.SectorID,
                        RequirementID = pi.RequirementID,

                        GCAName = g.FullName,
                        SectorName = s.Name,
                        RequirementName = r.Name,

                        Created = pi.Created,
                        Updated = pi.Updated,
                        Status = pi.Status
                    }
                 ).FirstOrNull();
                if (result != null) l.Add(result);
            }
            return l;
        }
        
     }    
}
