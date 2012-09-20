using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class MetricFilter
    {
        public class Extend : MetricFilter
        {
            public Guid?[] FilterOrgLocation { get; set; }
            public Guid?[] FilterPI { get; set; }
            public Guid?[] FilterMetric { get; set; }
        }

        public static List<MetricFilter> List()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            List<MetricFilter> UserFilters = (from m in dc.MetricFilter
                              where m.InstanceID == LinqMicajahDataContext.InstanceId &&
                                   m.UserID == LinqMicajahDataContext.LogedUserId &&
                                   m.Status == true                                   
                              orderby m.Name
                              select m).ToList();
            UserFilters.Insert(0, new MetricFilter { MetricFilterID = Guid.Empty, Name = String.Empty });
            return UserFilters;
        }

        public static MetricFilter.Extend Get(Guid MetricFilterID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            MetricFilter.Extend MetricFilter = (from m in dc.MetricFilter
                   where m.InstanceID == LinqMicajahDataContext.InstanceId &&
                        m.MetricFilterID == MetricFilterID &&
                        m.Status == true
                   select new Extend
                   {
                       //InstanceId = m.InstanceId,
                       MetricFilterID = m.MetricFilterID,
                       Name = m.Name,
                       GroupCategoryAspectID = m.GroupCategoryAspectID,
                       DataCollectorID = m.DataCollectorID
                   }).FirstOrNull();
            if (MetricFilter != null)
            {
                MetricFilter.FilterOrgLocation = (from drol in dc.MetricFilterOrgLocation
                                                  where drol.InstanceId == LinqMicajahDataContext.InstanceId && drol.MetricFilterID == MetricFilter.MetricFilterID
                                                  select (Guid?)drol.OrgLocationID).ToArray();
                MetricFilter.FilterPI = (from drpi in dc.MetricFilterPerformanceIndicator
                                         where drpi.InstanceId == LinqMicajahDataContext.InstanceId && drpi.MetricFilterID == MetricFilter.MetricFilterID
                                         select (Guid?)drpi.PerformanceIndicatorID).ToArray();
                MetricFilter.FilterMetric = (from drm in dc.MetricFilterMetric
                                             where drm.InstanceId == LinqMicajahDataContext.InstanceId && drm.MetricFilterID == MetricFilter.MetricFilterID
                                             select (Guid?)drm.MetricID).ToArray();
            }
            return MetricFilter;
        }

        public static void InsertOrUpdate(Extend MetricFilter, bool InsertMode)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                if (!InsertMode)
                {
                    IQueryable<MetricFilterOrgLocation> OldMetricFilterOrgLocation
                        = from drol in dc.MetricFilterOrgLocation
                          where drol.InstanceId == LinqMicajahDataContext.InstanceId && drol.MetricFilterID == MetricFilter.MetricFilterID
                          select drol;

                    IQueryable<MetricFilterPerformanceIndicator> OldMetricFilterPerformanceIndicator
                        = from drpi in dc.MetricFilterPerformanceIndicator
                          where drpi.InstanceId == LinqMicajahDataContext.InstanceId && drpi.MetricFilterID == MetricFilter.MetricFilterID
                          select drpi;
                    IQueryable<MetricFilterMetric> OldMetricFilterMetric
                        = from drm in dc.MetricFilterMetric
                          where drm.InstanceId == LinqMicajahDataContext.InstanceId && drm.MetricFilterID == MetricFilter.MetricFilterID
                          select drm;
                    dc.MetricFilterOrgLocation.DeleteAllOnSubmit(OldMetricFilterOrgLocation);
                    dc.MetricFilterPerformanceIndicator.DeleteAllOnSubmit(OldMetricFilterPerformanceIndicator);
                    dc.MetricFilterMetric.DeleteAllOnSubmit(OldMetricFilterMetric);
                    dc.SubmitChanges();
                }
                foreach (Guid? NewOrgLocationID in MetricFilter.FilterOrgLocation)
                {
                    MetricFilterOrgLocation NewOrgLocation = new MetricFilterOrgLocation();
                    NewOrgLocation.InstanceId = LinqMicajahDataContext.InstanceId;
                    NewOrgLocation.MetricFilterID = MetricFilter.MetricFilterID;
                    NewOrgLocation.OrgLocationID = (Guid)NewOrgLocationID;
                    NewOrgLocation.MetricFilterOrgLocationID = Guid.NewGuid();
                    dc.MetricFilterOrgLocation.InsertOnSubmit(NewOrgLocation);
                }
                foreach (Guid? NewPIID in MetricFilter.FilterPI)
                {
                    MetricFilterPerformanceIndicator NewPI = new MetricFilterPerformanceIndicator();
                    NewPI.InstanceId = LinqMicajahDataContext.InstanceId;
                    NewPI.MetricFilterID = MetricFilter.MetricFilterID;
                    NewPI.PerformanceIndicatorID = (Guid)NewPIID;
                    NewPI.MetricFilterPerformanceIndicatorID = Guid.NewGuid();
                    dc.MetricFilterPerformanceIndicator.InsertOnSubmit(NewPI);
                }
                foreach (Guid? NewMetricID in MetricFilter.FilterMetric)
                {
                    MetricFilterMetric NewMetric = new MetricFilterMetric();
                    NewMetric.InstanceId = LinqMicajahDataContext.InstanceId;
                    NewMetric.MetricFilterID = MetricFilter.MetricFilterID;
                    NewMetric.MetricID = (Guid)NewMetricID;
                    NewMetric.MetricFilterMetricID = Guid.NewGuid();
                    dc.MetricFilterMetric.InsertOnSubmit(NewMetric);
                }
                dc.SubmitChanges();
            }
            return;
        }

        public override void OnInserting(LinqMicajahDataContext dc, ref bool Cancel)
        {
            this.InstanceID = LinqMicajahDataContext.InstanceId;
            this.UserID = (Guid)LinqMicajahDataContext.LogedUserId;
            base.OnInserting(dc, ref Cancel);
        }
    }
}