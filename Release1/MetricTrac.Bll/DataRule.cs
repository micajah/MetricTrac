using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class DataRule
    {
        public class Extend : DataRule
        {
            public string OrgLocationName { get; set; }
            public string GroupCategoryAspectName { get; set; }
            public string PIFormName { get; set; }
            public string PIName { get; set; }
            public string MetricName { get; set; }
            public string UserName { get; set; }
            public string GroupName { get; set; }
            public Guid?[] OrgLocationsID { get; set; }
            public string[] OrgLocationsName{ get; set; }
        }

        public static List<Extend> List(int DataRuleTypeID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var rules = from r in dc.DataRule
                        join _n in dc.EntityNodeFullNameView on new { r.OrgLocationID, InstanceId = (Guid?)null } equals new { OrgLocationID = (Guid?)_n.EntityNodeId, _n.InstanceId } into __n
                        join _c in dc.GCAFullNameView on new { LinqMicajahDataContext.InstanceId, r.GroupCategoryAspectID } equals new { _c.InstanceId, GroupCategoryAspectID = (Guid?)_c.GroupCategoryAspectID } into __c
                        join _f in dc.PerformanceIndicatorForm on new { r.PerformanceIndicatorFormID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { PerformanceIndicatorFormID = (Guid?)_f.PerformanceIndicatorFormID, _f.InstanceId, _f.Status } into __f
                        join _i in dc.PerformanceIndicator on new { r.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { PerformanceIndicatorID = (Guid?)_i.PerformanceIndicatorID, _i.InstanceId, _i.Status } into __i
                        join _m in dc.Metric on new { r.MetricID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { MetricID = (Guid?)_m.MetricID, _m.InstanceId, _m.Status } into __m
                        join _u in dc.Mc_User on new { r.UserId, Deleted = false } equals new { UserId = (Guid?)_u.UserId, _u.Deleted } into __u
                        join _g in dc.Mc_Group on new { r.GroupId, Deleted = false } equals new { GroupId=(Guid?)_g.GroupId, _g.Deleted } into __g

                        from n in __n.DefaultIfEmpty()
                        from c in __c.DefaultIfEmpty()
                        from f in __f.DefaultIfEmpty()
                        from i in __i.DefaultIfEmpty()
                        from m in __m.DefaultIfEmpty()
                        from u in __u.DefaultIfEmpty()
                        from g in __g.DefaultIfEmpty()

                        where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.DataRuleTypeID == DataRuleTypeID

                        orderby r.OrderNumber, r.Created

                        select new Extend
                        {
                            InstanceId = r.InstanceId,
                            DataRuleID = r.DataRuleID,
                            OrderNumber = r.OrderNumber,
                            OrgLocationID = r.OrgLocationID,
                            GroupCategoryAspectID = r.GroupCategoryAspectID,
                            PerformanceIndicatorFormID = r.PerformanceIndicatorFormID,
                            MetricID = r.MetricID,
                            UserId = r.UserId,
                            GroupId = r.GroupId,
                            Status = r.Status,
                            Created = r.Created,
                            Updated = r.Updated,
                            Description = r.Description,

                            OrgLocationName = r.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : n.FullName,
                            GroupCategoryAspectName = c.FullName,
                            PIFormName = f.Name,
                            PIName = i.Name,
                            MetricName = m.Name,
                            UserName = ((u.FirstName == null) ? "" : u.FirstName + " ") + ((u.LastName == null) ? "" : u.LastName),
                            GroupName = g.Name,

                            RuleClusterID = r.RuleClusterID
                        };

            List<Extend> AllRulesList = rules.ToList();
            List<Extend> ResultRulesList = new List<Extend>();
                        
            List<Guid?> CurOrgLocations = new List<Guid?>();
            for (int i = 0; i < AllRulesList.Count; i++)
            {
                Extend r = AllRulesList[i];
                if (r.RuleClusterID != null)
                {
                    Guid CurClusterID = (Guid)r.RuleClusterID;
                    string ClasterName = String.Empty;
                    Extend rs = r;
                    while (rs.RuleClusterID == CurClusterID)
                    {
                        if (String.IsNullOrEmpty(ClasterName))
                            ClasterName = rs.OrgLocationName;
                        else
                            ClasterName += ", " + rs.OrgLocationName;
                        CurOrgLocations.Add(rs.OrgLocationID);
                        i++;
                        if (i < AllRulesList.Count)
                            rs = AllRulesList[i];
                        else
                            break;
                    }
                    r.OrgLocationsID = CurOrgLocations.ToArray();
                    r.OrgLocationName = ClasterName;//" and " + (CurOrgLocations.Count - 1).ToString() + " other Org Locations";
                    i--;
                    CurOrgLocations.Clear();
                }
                ResultRulesList.Add(r);
            }
            return ResultRulesList;
        }

        private class MoveDataRule
        {
            public DataRule r0 { get; set; }
            public DataRule r1 { get; set; }
        }

        public override void OnInserting(LinqMicajahDataContext dc, ref bool Cancel)
        {
            base.OnInserting(dc, ref Cancel);

            int? Order = null;
            DataRule rule = null;
            if (this.RuleClusterID != null)
                rule = (from r in dc.DataRule
                                      where r.InstanceId == LinqMicajahDataContext.InstanceId
                                            && r.Status == true
                                            && r.RuleClusterID == this.RuleClusterID
                                            && r.DataRuleTypeID == this.DataRuleTypeID
                                      select r).FirstOrNull();

            if (rule != null)
                Order = rule.OrderNumber;
            else
            {
                Order = (
                    from r in dc.DataRule
                    where r.DataRuleTypeID == this.DataRuleTypeID && r.InstanceId == this.InstanceId
                    select (int?)r.OrderNumber
                ).Min();

                if (Order == null || Order < 1) Order = int.MaxValue / 2;
                else Order--;
            }

            this.OrderNumber = (int)Order;
        }

        public static void Move(Guid DataRuleID, bool Up, int DataRuleTypeID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            MoveDataRule mr;
            if (Up)
            {
                mr = (from r0 in dc.DataRule

                      where r0.DataRuleID == DataRuleID &&
                               r0.InstanceId == LinqMicajahDataContext.InstanceId &&
                               r0.Status == true && r0.DataRuleTypeID == DataRuleTypeID

                      select new MoveDataRule
                         {
                             r0 = r0,
                             r1 = (from _r1 in dc.DataRule
                                   orderby _r1.OrderNumber descending
                                   where _r1.OrderNumber<r0.OrderNumber &&
                                   _r1.InstanceId==LinqMicajahDataContext.InstanceId &&
                                   _r1.Status == true && _r1.DataRuleTypeID == DataRuleTypeID
                                    select _r1).FirstOrDefault()
                         }).FirstOrDefault();
            }
            else
            {
                mr = (from r0 in dc.DataRule

                      where r0.DataRuleID == DataRuleID &&
                            r0.InstanceId == LinqMicajahDataContext.InstanceId &&
                            r0.Status == true && r0.DataRuleTypeID == DataRuleTypeID

                      select new MoveDataRule
                      {
                          r0 = r0,
                          r1 = (from _r1 in dc.DataRule
                                orderby _r1.OrderNumber
                                where _r1.OrderNumber > r0.OrderNumber &&
                                _r1.InstanceId == LinqMicajahDataContext.InstanceId &&
                                _r1.Status == true && _r1.DataRuleTypeID == DataRuleTypeID
                                select _r1).FirstOrDefault()
                      }).FirstOrDefault();
            }

            if (mr == null || mr.r0 == null || mr.r1 == null) return;


            int R0Number = mr.r0.OrderNumber;
            int R1Number = mr.r1.OrderNumber;

            mr.r0.OrderNumber = R1Number;
            if (mr.r0.RuleClusterID != null)
            {
                IQueryable<DataRule> RelatedRules = from r in dc.DataRule
                                                    where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.RuleClusterID == mr.r0.RuleClusterID
                                                    select r;
                foreach (DataRule dr in RelatedRules)
                    dr.OrderNumber = R1Number;
            }

            mr.r1.OrderNumber = R0Number;
            if (mr.r1.RuleClusterID != null)
            {
                IQueryable<DataRule> RelatedRules = from r in dc.DataRule
                                                    where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.RuleClusterID == mr.r1.RuleClusterID
                                                    select r;
                foreach (DataRule dr in RelatedRules)
                    dr.OrderNumber = R0Number;
            }

            dc.SubmitChanges();
        }

        // Clusters
        public static Guid?[] OrgLocationsClusterList(Guid RuleClusterID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var OrgLocationsArray = (from r in dc.DataRule                        
                        where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.RuleClusterID == RuleClusterID
                        orderby r.OrderNumber
                        select r.OrgLocationID).ToArray();
            return OrgLocationsArray;
        }

        public static void InsertOrgLocationsClusterList(Guid RuleID, Guid?[] OrgLocationsID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                DataRule FirstRule = (from r in dc.DataRule
                                         where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.DataRuleID == RuleID
                                         select r).FirstOrNull();
                if (FirstRule != null && OrgLocationsID.Length > 1)
                {
                    for (int i = 1; i < OrgLocationsID.Length; i++)
                    {
                        DataRule OneMoreRule = new DataRule();
                        OneMoreRule.InstanceId = FirstRule.InstanceId;
                        OneMoreRule.DataRuleID = Guid.NewGuid();
                        OneMoreRule.DataRuleTypeID = FirstRule.DataRuleTypeID;
                        OneMoreRule.OrderNumber = FirstRule.OrderNumber;
                        OneMoreRule.OrgLocationID = OrgLocationsID[i];
                        OneMoreRule.GroupCategoryAspectID = FirstRule.GroupCategoryAspectID;
                        OneMoreRule.PerformanceIndicatorID = FirstRule.PerformanceIndicatorID;
                        OneMoreRule.PerformanceIndicatorFormID = FirstRule.PerformanceIndicatorFormID;
                        OneMoreRule.MetricID = FirstRule.MetricID;
                        OneMoreRule.UserId = FirstRule.UserId;
                        OneMoreRule.GroupId = FirstRule.GroupId;
                        OneMoreRule.Status = FirstRule.Status;
                        OneMoreRule.Created = FirstRule.Created;
                        OneMoreRule.Updated = FirstRule.Updated;
                        OneMoreRule.Description = FirstRule.Description;
                        OneMoreRule.RuleClusterID = FirstRule.RuleClusterID;                        
                        dc.DataRule.InsertOnSubmit(OneMoreRule);
                    }
                    dc.SubmitChanges();
                }
            }
        }

        public override void OnDeleted(LinqMicajahDataContext dc)
        {
            base.OnDeleted(dc); 
            Guid? RuleClusterID = this.RuleClusterID;
            if (RuleClusterID == null && this.DataRuleID != null)
            {
                DataRule dr = (from r in dc.DataRule
                              where r.InstanceId == LinqMicajahDataContext.InstanceId && r.DataRuleID == this.DataRuleID
                              select r).FirstOrNull();
                if (dr != null)
                    RuleClusterID = dr.RuleClusterID;
            }
            if (RuleClusterID != null)
                MetricTrac.Bll.DataRule.DeleteOrgLocationsClusterList((Guid)RuleClusterID, null);
        }

        public static void DeleteOrgLocationsClusterList(Guid RuleClusterID, Guid? FirstRuleID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                IQueryable<DataRule> RelatedRules = from r in dc.DataRule
                                                         where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.RuleClusterID == RuleClusterID
                                                         select r;
                foreach (DataRule dr in RelatedRules)
                    if (dr.DataRuleID != FirstRuleID || FirstRuleID == null)
                        dr.Status = false;
                dc.SubmitChanges();
            }
        }
    }
}
