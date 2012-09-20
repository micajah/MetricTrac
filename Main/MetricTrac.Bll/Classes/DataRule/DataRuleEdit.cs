using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class DataRule
    {
        public static Extend Get(Guid DataRuleID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            Extend rule = (from r in dc.DataRule
                        join _c in dc.ViewnameGroupCategoryAspect on new { InstanceId = (Guid?)r.InstanceId, r.GroupCategoryAspectID } equals new { _c.InstanceId, _c.GroupCategoryAspectID } into __c
                        join _u in dc.ViewnameUser on r.UserId equals _u.UserId into __u
                        join _g in dc.Mc_Group on new { r.GroupId, Deleted = false } equals new { GroupId = (Guid?)_g.GroupId, _g.Deleted } into __g

                        from c in __c.DefaultIfEmpty()
                        from u in __u.DefaultIfEmpty()
                        from g in __g.DefaultIfEmpty()

                        where r.InstanceId == LinqMicajahDataContext.InstanceId && r.Status == true && r.DataRuleID == DataRuleID

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
                        }).FirstOrNull();
            if (rule != null)
            {
                rule.DataRuleOrgLocation = (from drol in dc.DataRuleOrgLocation
                                            where drol.InstanceId == LinqMicajahDataContext.InstanceId && drol.DataRuleID == DataRuleID
                                            select (Guid?)drol.OrgLocationID).ToArray();
                rule.DataRulePI = (from drpi in dc.DataRulePerformanceIndicator
                                                where drpi.InstanceId == LinqMicajahDataContext.InstanceId && drpi.DataRuleID == DataRuleID
                                                 select (Guid?)drpi.PerformanceIndicatorID).ToArray();
                rule.DataRuleMetric = (from drm in dc.DataRuleMetric
                                   where drm.InstanceId == LinqMicajahDataContext.InstanceId && drm.DataRuleID == DataRuleID
                                   select (Guid?)drm.MetricID).ToArray();
            }
            return rule;
        }

        public static void InsertOrUpdate(Extend DataRule)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                DataRule dr =(from r in dc.DataRule
                             where
                                r.InstanceId == LinqMicajahDataContext.InstanceId &&
                                r.DataRuleID == DataRule.DataRuleID
                             select r).FirstOrNull();
                bool InsertMode = (dr == null);
                Guid DataRuleID = DataRule.DataRuleID;
                if (dr == null)
                {
                    DataRule NewRule = new DataRule();
                    NewRule.DataRuleID = DataRuleID = Guid.NewGuid();
                    NewRule.Description = DataRule.Description;
                    NewRule.UserId = DataRule.UserId;
                    NewRule.GroupId = DataRule.GroupId;
                    NewRule.DataRuleTypeID = DataRule.DataRuleTypeID;
                    NewRule.GroupCategoryAspectID = DataRule.GroupCategoryAspectID;
                    dc.DataRule.InsertOnSubmit(NewRule);
                }
                else
                {                    
                    dr.Description = DataRule.Description;
                    dr.UserId = DataRule.UserId;
                    dr.GroupId = DataRule.GroupId;
                    dr.GroupCategoryAspectID = DataRule.GroupCategoryAspectID;
                }
                dc.SubmitChanges();
                // save related entities
                if (!InsertMode)
                {
                    IQueryable<DataRuleOrgLocation> OldDataRuleOrgLocation
                        = from drol in dc.DataRuleOrgLocation
                          where drol.InstanceId == LinqMicajahDataContext.InstanceId && drol.DataRuleID == DataRuleID
                          select drol;

                    IQueryable<DataRulePerformanceIndicator> OldDataRulePerformanceIndicator
                        = from drpi in dc.DataRulePerformanceIndicator
                          where drpi.InstanceId == LinqMicajahDataContext.InstanceId && drpi.DataRuleID == DataRuleID
                          select drpi;
                    IQueryable<DataRuleMetric> OldDataRuleMetric
                        = from drm in dc.DataRuleMetric
                          where drm.InstanceId == LinqMicajahDataContext.InstanceId && drm.DataRuleID == DataRuleID
                          select drm;
                    dc.DataRuleOrgLocation.DeleteAllOnSubmit(OldDataRuleOrgLocation);
                    dc.DataRulePerformanceIndicator.DeleteAllOnSubmit(OldDataRulePerformanceIndicator);
                    dc.DataRuleMetric.DeleteAllOnSubmit(OldDataRuleMetric);
                    dc.SubmitChanges();
                }
                foreach (Guid? NewOrgLocationID in DataRule.DataRuleOrgLocation)
                {
                    DataRuleOrgLocation NewOrgLocation = new DataRuleOrgLocation();
                    NewOrgLocation.InstanceId = LinqMicajahDataContext.InstanceId;
                    NewOrgLocation.DataRuleID = DataRuleID;
                    NewOrgLocation.OrgLocationID = (Guid)NewOrgLocationID;
                    NewOrgLocation.DataRuleOrgLocationID = Guid.NewGuid();
                    dc.DataRuleOrgLocation.InsertOnSubmit(NewOrgLocation);
                }
                foreach (Guid? NewPIID in DataRule.DataRulePI)
                {
                    DataRulePerformanceIndicator NewOrgLocation = new DataRulePerformanceIndicator();
                    NewOrgLocation.InstanceId = LinqMicajahDataContext.InstanceId;
                    NewOrgLocation.DataRuleID = DataRuleID;
                    NewOrgLocation.PerformanceIndicatorID = (Guid)NewPIID;
                    NewOrgLocation.DataRulePerformanceIndicatorID = Guid.NewGuid();
                    dc.DataRulePerformanceIndicator.InsertOnSubmit(NewOrgLocation);
                }
                foreach (Guid? NewMetricID in DataRule.DataRuleMetric)
                {
                    DataRuleMetric NewOrgLocation = new DataRuleMetric();
                    NewOrgLocation.InstanceId = LinqMicajahDataContext.InstanceId;
                    NewOrgLocation.DataRuleID = DataRuleID;
                    NewOrgLocation.MetricID = (Guid)NewMetricID;
                    NewOrgLocation.DataRuleMetricID = Guid.NewGuid();
                    dc.DataRuleMetric.InsertOnSubmit(NewOrgLocation);
                }
                dc.SubmitChanges();
                UpdateD_DataRule(dc);
            }
            return;
        }

        public override void OnInserting(LinqMicajahDataContext dc, ref bool Cancel)
        {
            base.OnInserting(dc, ref Cancel);
            int? Order = (
                from r in dc.DataRule
                where r.DataRuleTypeID == this.DataRuleTypeID && r.InstanceId == this.InstanceId
                select (int?)r.OrderNumber
            ).Min();
            if (Order == null || Order < 1)
                Order = int.MaxValue / 2;
            else
                Order--;
            this.OrderNumber = (int)Order;
        }

        public static void Delete(Guid DataRuleID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {   
                DataRule rule = (from r in dc.DataRule
                                    where  
                                          r.InstanceId == LinqMicajahDataContext.InstanceId
                                          &&
                                          r.DataRuleID == DataRuleID
                                          &&
                                          r.Status == true
                                    select r).FirstOrNull();
                if (rule != null)
                {
                    rule.Status = false;
                    rule.Updated = DateTime.Now;
                    dc.SubmitChanges();
                }
                UpdateD_DataRule(dc);
            }
        }

        public static void UpdateD_DataRule(LinqMicajahDataContext dc)
        {
            dc.Sp_d_DataRule();
            dc.Sp_d_MetricOrgLocationRule();
        }
    }
}