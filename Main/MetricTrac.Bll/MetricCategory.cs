using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class MetricCategory
    {
        public static IQueryable<MetricCategory> SelectAll()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            IQueryable<MetricCategory> ret =

                from _MetricCategory in dc.MetricCategory
                orderby _MetricCategory.ParentId, _MetricCategory.Name
                where (_MetricCategory.InstanceId == LinqMicajahDataContext.InstanceId && (_MetricCategory.Status == true))
                select _MetricCategory;

            return ret;
        }

        public static IQueryable<ViewHierarchyMetricCategory> SelectAllIncludes()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            IQueryable<ViewHierarchyMetricCategory> ret =

                from _MetricCategory in dc.ViewHierarchyMetricCategory 
                where _MetricCategory.InstanceId == LinqMicajahDataContext.InstanceId
                select _MetricCategory;

            return ret;
        }

        public static Guid Insert(MetricCategory newMetricCategory)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {   
                dc.MetricCategory.InsertOnSubmit(newMetricCategory);
                dc.SubmitChanges();
                return newMetricCategory.MetricCategoryID;
            }
        }

        public static void Delete(Guid itemID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                MetricCategory ret =
                    (from _MetricCategory in dc.MetricCategory
                     where (_MetricCategory.InstanceId == LinqMicajahDataContext.InstanceId) && (_MetricCategory.MetricCategoryID == itemID)
                     select _MetricCategory).FirstOrNull();
                if (ret != null)
                {
                    ret.Status = false;                    
                    DeleteChildItems(dc, itemID);
                    dc.SubmitChanges();
                }
            }
        }

        private static void DeleteChildItems(LinqMicajahDataContext dc, Guid _ParentID)
        {
            IQueryable<MetricCategory> rets =
                from _MetricCategory in dc.MetricCategory
                where ((_MetricCategory.InstanceId == LinqMicajahDataContext.InstanceId) && (_MetricCategory.Status == true) && (_MetricCategory.ParentId == _ParentID))
                select _MetricCategory;
            foreach (MetricCategory metriccategory in rets)
            {
                metriccategory.Status = false;                
                DeleteChildItems(dc, metriccategory.MetricCategoryID);
            }
        }

        public static void Update(Guid itemID, string text)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                MetricCategory ret =
                    (from _MetricCategory in dc.MetricCategory
                     orderby _MetricCategory.Name
                     where (_MetricCategory.InstanceId == LinqMicajahDataContext.InstanceId) && (_MetricCategory.MetricCategoryID == itemID)
                     select _MetricCategory).FirstOrNull();

                if (ret != null)
                {
                    ret.Name = text;                    
                    dc.SubmitChanges();
                }
            }
        }

        // Custom features
        private class GuidPair
        {
            public Guid SourceGuid = Guid.Empty;
            public Guid DestGuid = Guid.Empty;
        }

        public static void Merge(Guid SourceID, Guid DestID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                MergeMC(dc, SourceID, DestID);
            }
        }

        private static void MergeMC(LinqMicajahDataContext dc, Guid SourceID, Guid DestID)
        {
            IQueryable<GuidPair> Pairs =
                 from mc1 in dc.MetricCategory
                 join mc2 in dc.MetricCategory
                    on mc1.Name equals mc2.Name 
                 where
                    (mc1.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (mc2.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (mc1.ParentId == SourceID)
                    &&
                    (mc2.ParentId == DestID)
                    &&
                    mc1.Status == true
                    &&
                    mc2.Status == true
                 select new GuidPair
                 {
                     SourceGuid = mc1.MetricCategoryID,
                     DestGuid = mc2.MetricCategoryID
                 };
            foreach (GuidPair gp in Pairs)
                MergeMC(dc, gp.SourceGuid, gp.DestGuid);

            // main merge
            IQueryable<MetricCategory> SourceMCs =
                 from mc in dc.MetricCategory
                 where                    
                    (mc.InstanceId == LinqMicajahDataContext.InstanceId) && (mc.ParentId == SourceID)
                 select mc;
            foreach (MetricCategory _mc in SourceMCs)
                _mc.ParentId = DestID;

            ChangeMetricsCategory(dc, SourceID, DestID);

            MetricCategory SourceMC =
                 (from mc in dc.MetricCategory
                  where
                     (mc.InstanceId == LinqMicajahDataContext.InstanceId) && (mc.MetricCategoryID == SourceID)
                  select mc).FirstOrNull();
            if (SourceMC != null)
                SourceMC.Status = false;
            dc.SubmitChanges();
        }

        private static void ChangeMetricsCategory(LinqMicajahDataContext dc, Guid SourceID, Guid DestID)
        {
            IQueryable<Metric> ms =
                from m in dc.Metric
                where
                (m.InstanceId == LinqMicajahDataContext.InstanceId) &&
                (m.MetricCategoryID == SourceID) &&
                (m.Status == true)
                select m;
            foreach (Metric m in ms)
                m.MetricCategoryID = DestID;
            dc.SubmitChanges();
        }

        public static void Copy(Guid SourceID, Guid DestID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                CopyMC(dc, SourceID, DestID);
            }
        }

        private static void CopyMC(LinqMicajahDataContext dc, Guid SourceID, Guid DestID)
        {
            MetricCategory SourceMC =
                 (from mc in dc.MetricCategory
                  where
                     (mc.InstanceId == LinqMicajahDataContext.InstanceId)
                     &&  
                     (mc.MetricCategoryID == SourceID)
                     &&
                     (mc.Status == true)
                  select mc).FirstOrNull();
            if (SourceMC != null)
            {
                MetricCategory cSource = new MetricCategory();
                cSource.CopyFrom(SourceMC);
                cSource.ParentId = DestID;                
                dc.MetricCategory.InsertOnSubmit(cSource);
                dc.SubmitChanges();

                IQueryable<MetricCategory> SourceMCs =
                 from mc in dc.MetricCategory
                 where
                    (mc.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (mc.ParentId == SourceID)
                    &&
                    (mc.Status == true)
                 select mc;
                foreach (MetricCategory mc in SourceMCs)
                    CopyMC(dc, mc.MetricCategoryID, cSource.MetricCategoryID);
            }
        }

        public static void Move(Guid SourceID, Guid DestID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                GuidPair Pair =
                 (from mc1 in dc.MetricCategory
                  join mc2 in dc.MetricCategory
                     on mc1.Name equals mc2.Name
                  where
                     (mc1.InstanceId == LinqMicajahDataContext.InstanceId)
                     &&
                     (mc2.InstanceId == LinqMicajahDataContext.InstanceId)
                     &&
                     (mc1.MetricCategoryID == SourceID)
                     &&
                     (mc2.ParentId == DestID)
                     &&
                     mc1.Status == true
                     &&
                     mc2.Status == true
                  select new GuidPair
                  {
                      SourceGuid = mc1.MetricCategoryID,
                      DestGuid = mc2.MetricCategoryID
                  }).FirstOrNull();
                if (Pair == null)
                {
                    // move
                    MetricCategory SourceMC =
                     (from mc in dc.MetricCategory
                      where
                         (mc.InstanceId == LinqMicajahDataContext.InstanceId)   
                         &&
                         (mc.MetricCategoryID == SourceID)
                      select mc).FirstOrNull();
                    if (SourceMC != null)
                        SourceMC.ParentId = DestID;
                    dc.SubmitChanges();
                }
                else
                    if (SourceID != Pair.DestGuid)
                        MergeMC(dc, SourceID, Pair.DestGuid);// merge                
            }
        }
    }
}
