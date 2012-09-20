using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace MetricTrac.Utils
{
    public class MetricCache
    {       

        Guid PerformanceIndicatorID;
        List<MetricTrac.Bll.Metric.Extend> NewMetrics;

        List<MetricTrac.Bll.Metric.Extend> mOldMetrics;
        List<MetricTrac.Bll.Metric.Extend> OldMetrics
        {
            get
            {
                if (mOldMetrics != null) return mOldMetrics;
                if (PerformanceIndicatorID == Guid.Empty) mOldMetrics = new List<MetricTrac.Bll.Metric.Extend>();
                else mOldMetrics = MetricTrac.Bll.Metric.List(PerformanceIndicatorID);
                return mOldMetrics;
            }
        }
        List<MetricTrac.Bll.Metric.Extend> mAllMetric;
        List<MetricTrac.Bll.Metric.Extend> AllMetric
        {
            get
            {
                if (mAllMetric != null) return mAllMetric;
                mAllMetric = MetricTrac.Bll.Metric.List().ToList();
                return mAllMetric;
            }
        }

        string StateObjectName
        {
            get { return "MetricOf" + PerformanceIndicatorID + "PerformanceIndicator"; }
        }

        public MetricCache(Guid PerformanceIndicatorID, System.Web.SessionState.HttpSessionState vs, bool IsContinue)
        {
            this.PerformanceIndicatorID = PerformanceIndicatorID;

            object o = vs[StateObjectName];
            if (IsContinue && o != null && o is Guid[])
            {
                Guid[] MetricIds = (Guid[])o;
                NewMetrics = new List<MetricTrac.Bll.Metric.Extend>();
                foreach (Guid MetricId in MetricIds)
                {
                    MetricTrac.Bll.Metric.Extend k = MetricTrac.Bll.Metric.Get(MetricId);
                    if (k != null) NewMetrics.Add(k);
                }
            }
            else
            {
                NewMetrics = new List<MetricTrac.Bll.Metric.Extend>();
                foreach (MetricTrac.Bll.Metric.Extend k in OldMetrics) NewMetrics.Add(k);
            }
        }

        public void WriteSesion(System.Web.SessionState.HttpSessionState vs)
        {
            List<Guid> MetricIds = new List<Guid>();
            foreach (MetricTrac.Bll.Metric.Extend k in NewMetrics)
            {
                if (k.MetricID == Guid.Empty) continue;
                MetricIds.Add(k.MetricID);
            }
            vs[StateObjectName] = MetricIds.ToArray();
        }

        public void Add(Guid MetricID)
        {
            if (NewMetrics.Where(k => k.MetricID == MetricID).Count() > 0) return;
            var ks = AllMetric.Where(k => k.MetricID == MetricID);
            if (ks.Count() < 1) return;
            NewMetrics.Add(ks.First());
        }

        public void Delete(Guid MetricID)
        {
            var ks = NewMetrics.Where(k => k.MetricID == MetricID);
            if (ks.Count() < 1) return;
            NewMetrics.Remove(ks.First());
        }

        public void Rename(Guid OldMetricID, Guid NewMetricID)
        {
            Delete(OldMetricID);
            Add(NewMetricID);
        }

        private bool Contains(List<MetricTrac.Bll.Metric.Extend> l, MetricTrac.Bll.Metric.Extend k)
        {
            foreach (MetricTrac.Bll.Metric.Extend lk in l)
            {
                if (lk.MetricID == k.MetricID) return true;
            }
            return false;
        }

        public void Save()
        {
            foreach (MetricTrac.Bll.Metric.Extend k in NewMetrics)
            {
                if (Contains(OldMetrics, k)) continue;
                if (k.MetricID == Guid.Empty) continue;
                MetricTrac.Bll.PerformanceIndicatorMetricJunc.Insert(PerformanceIndicatorID, k.MetricID);
            }
            foreach (MetricTrac.Bll.Metric.Extend k in OldMetrics)
            {
                if (Contains(NewMetrics, k)) continue;
                if (k.MetricID == Guid.Empty) continue;
                MetricTrac.Bll.PerformanceIndicatorMetricJunc.Delete(PerformanceIndicatorID, k.MetricID);
            }
        }

        public void Save(Guid PerformanceIndicatorID)
        {
            this.PerformanceIndicatorID = PerformanceIndicatorID;
            Save();
        }

        public List<MetricTrac.Bll.Metric.Extend> List()
        {
            List<MetricTrac.Bll.Metric.Extend> ret = new List<MetricTrac.Bll.Metric.Extend>();
            foreach (MetricTrac.Bll.Metric.Extend k in NewMetrics) ret.Add(k);
            return ret;
        }

        public List<MetricTrac.Bll.Metric.Extend> ListUnused(Guid?[] PIID, Guid? GCAID, Guid?[] OrgLocationsID, Guid? DataCollectorID)
        {
            List<MetricTrac.Bll.Metric.Extend> u;
            if (PIID != null || GCAID != null || OrgLocationsID != null || DataCollectorID != null)
            {
                List<Guid> MetricIDs = MetricTrac.Bll.Metric.ListUnused(Guid.Empty,PIID,GCAID,OrgLocationsID,DataCollectorID).ToList().Select(um=>(Guid)um.MetricID).ToList();
                u = AllMetric.Where(m => MetricIDs.Contains(m.MetricID)).ToList();
            }
            else{
                u = AllMetric;
            }
            List<Guid> NewMetricsID = NewMetrics.Select(nm=>nm.MetricID).ToList();
            u = u.Where(m => !NewMetricsID.Contains(m.MetricID)).ToList();
            return u.ToList();
         }

        public List<MetricTrac.Bll.Metric.Extend> ListReferenced()
        {
            List<Guid> NewMetricID = new List<Guid>();
            return MetricTrac.Bll.Metric.ListReferenced((from m in NewMetrics select m.MetricID).ToList());
        }
    }
}
