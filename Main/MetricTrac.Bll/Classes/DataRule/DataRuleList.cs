using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class DataRule
    {
        public static List<Extend> List(int DataRuleTypeID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            List<Extend> rules = (from r in dc.DataRule
                        join _c in dc.ViewnameGroupCategoryAspect on new { InstanceId = (Guid?)r.InstanceId, r.GroupCategoryAspectID } equals new { _c.InstanceId, _c.GroupCategoryAspectID } into __c
                        join _u in dc.ViewnameUser on r.UserId equals _u.UserId into __u
                        join _g in dc.Mc_Group on new { r.GroupId, Deleted = false } equals new { GroupId = (Guid?)_g.GroupId, _g.Deleted } into __g

                        from c in __c.DefaultIfEmpty()
                        from u in __u.DefaultIfEmpty()
                        from g in __g.DefaultIfEmpty()

                        where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.DataRuleTypeID == DataRuleTypeID

                        orderby r.OrderNumber, r.Created

                        select new Extend
                        {
                            InstanceId = r.InstanceId,
                            DataRuleID = r.DataRuleID,
                            OrderNumber = r.OrderNumber,
                            GroupCategoryAspectID = r.GroupCategoryAspectID,
                            UserId = r.UserId,
                            GroupId = r.GroupId,
                            Status = r.Status,
                            Created = r.Created,
                            Updated = r.Updated,
                            Description = r.Description,
                            GroupCategoryAspectName = c.FullName,
                            UserName = u.Name,
                            GroupName = g.Name
                        }).ToList();
            var ruleOrgLocations = from drol in dc.DataRuleOrgLocation
                                join r in dc.DataRule on new { drol.InstanceId, drol.DataRuleID } equals new { r.InstanceId, r.DataRuleID }
                                join _n in dc.ViewnameOrgLocation on new { OrgLocationID = (Guid?)drol.OrgLocationID, InstanceId = (Guid?)drol.InstanceId } equals new { _n.OrgLocationID, _n.InstanceId } into __n
                                from n in __n.DefaultIfEmpty()
                                
                                where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.DataRuleTypeID == DataRuleTypeID
                                orderby r.OrderNumber, r.Created
                                select new RuleEntity
                                {                            
                                    DataRuleID = r.DataRuleID,
                                    EntityID = drol.OrgLocationID,
                                    EntityName = n.FullName
                                };
            var rulePerformanceIndicators = from drpi in dc.DataRulePerformanceIndicator
                                            join r in dc.DataRule on new { drpi.InstanceId, drpi.DataRuleID } equals new { r.InstanceId, r.DataRuleID }
                                            join _i in dc.PerformanceIndicator on new { drpi.PerformanceIndicatorID, drpi.InstanceId, Status = (bool?)true } equals new { _i.PerformanceIndicatorID, _i.InstanceId, _i.Status } into __i
                                            from i in __i.DefaultIfEmpty()
                        
                                            where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.DataRuleTypeID == DataRuleTypeID
                                            orderby r.OrderNumber, r.Created
                                            select new RuleEntity
                                            {                            
                                                DataRuleID = r.DataRuleID,
                                                EntityID = drpi.PerformanceIndicatorID,
                                                EntityName = i.Name
                                            };
            var ruleMetrics = from drm in dc.DataRuleMetric
                                            join r in dc.DataRule on new { drm.InstanceId, drm.DataRuleID } equals new { r.InstanceId, r.DataRuleID }
                                            join _m in dc.Metric on new { drm.MetricID, drm.InstanceId, Status = (bool?)true } equals new { _m.MetricID, _m.InstanceId, _m.Status } into __m
                                            from m in __m.DefaultIfEmpty()

                                            where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.DataRuleTypeID == DataRuleTypeID
                                            orderby r.OrderNumber, r.Created
                                            select new RuleEntity
                                            {
                                                DataRuleID = r.DataRuleID,
                                                EntityID = drm.MetricID,
                                                EntityName = m.Name
                                            };

            
            foreach (Extend dr in rules)
            {
                dr.OrgLocationName = EntityArrayName(dr.DataRuleID, ruleOrgLocations);
                dr.PerformanceIndicatorName = EntityArrayName(dr.DataRuleID, rulePerformanceIndicators);
                dr.MetricName = EntityArrayName(dr.DataRuleID, ruleMetrics);                
            }
            return rules.ToList();
        }

        private static string EntityArrayName(Guid DataRuleID, IQueryable<RuleEntity> Entities)
        {
            List<RuleEntity> rEntities = Entities.Where(r => r.DataRuleID == DataRuleID).ToList();
            string Name = String.Empty;
            foreach (RuleEntity re in rEntities)
                if (String.IsNullOrEmpty(Name))
                    Name = re.EntityName;
                else
                    Name += ", " + re.EntityName;
            return Name;
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
                                where _r1.OrderNumber < r0.OrderNumber &&
                                _r1.InstanceId == LinqMicajahDataContext.InstanceId &&
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
            mr.r1.OrderNumber = R0Number;            

            dc.SubmitChanges();
            UpdateD_DataRule(dc);
        }        
    }
}