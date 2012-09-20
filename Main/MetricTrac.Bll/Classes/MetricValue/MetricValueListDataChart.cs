using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class MetricValue
    {
        public static MetricOrgValue List(int ValueCount, DateTime BaseDate, Guid ScoreCardMetricID, MetricTrac.Bll.ScoreCardMetric.CalcStringFormula Calculator)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var r = from cm in dc.ScoreCardMetric
                    join _m in dc.Metric on new { LinqMicajahDataContext.InstanceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, MetricID = (Guid?)_m.MetricID, _m.Status } into __m
                    join o in dc.ViewnameOrgLocation on new { InstanceId = (Guid?)cm.InstanceId, cm.OrgLocationID } equals new { o.InstanceId, o.OrgLocationID }
                    join _u in dc.MetricOrgLocationUoM on new { LinqMicajahDataContext.InstanceId, cm.MetricID, cm.OrgLocationID } equals new { _u.InstanceId, MetricID = (Guid?)_u.MetricID, OrgLocationID = (Guid?)_u.OrgLocationID } into __u
                    join _pi in dc.PerformanceIndicator on new { LinqMicajahDataContext.InstanceId, cm.PerformanceIndicatorId, Status = (bool?)true } equals new { _pi.InstanceId, PerformanceIndicatorId = (Guid?)_pi.PerformanceIndicatorID, _pi.Status } into __pi

                    from m in __m.DefaultIfEmpty()
                    from u in __u.DefaultIfEmpty()
                    from pi in __pi.DefaultIfEmpty()

                    where cm.InstanceId == LinqMicajahDataContext.InstanceId && cm.ScoreCardMetricID == ScoreCardMetricID && cm.Status == true
                    select new MetricOrgValue
                    {
                        Name = cm.MetricID == null ? pi.Name : m.Name,
                        OrgLocationFullName = o.FullName,
                        MetricID = cm.MetricID == null ? Guid.Empty : (Guid)cm.MetricID,
                        OrgLocationID = cm.OrgLocationID == null ? Guid.Empty : (Guid)cm.OrgLocationID,//o.EntityNodeId, !!!! TODO fix this
                        FrequencyID = m.MetricID == null ? 3 : m.FrequencyID,
                        InputUnitOfMeasureID = u.MetricOrgLocationUoMID == null ? m.InputUnitOfMeasureID : u.InputUnitOfMeasureID,//u.InputUnitOfMeasureID == null ? m.InputUnitOfMeasureID : u.InputUnitOfMeasureID,
                        UnitOfMeasureID = cm.MetricID == null ? pi.UnitOfMeasureID : m.UnitOfMeasureID,
                        NODecPlaces = m.NODecPlaces,
                        PerformanceIndicatorID = cm.PerformanceIndicatorId == null || pi.PerformanceIndicatorID == Guid.Empty ? null : (Guid?)pi.PerformanceIndicatorID
                    };
            var mo = r.FirstOrNull();
            if (mo == null) return null;

            DateTime EndDate = Frequency.GetNormalizedDate(mo.FrequencyID, BaseDate);


            ScoreCardMetric.Extend e = new ScoreCardMetric.Extend()
            {
                InstanceId = LinqMicajahDataContext.InstanceId,
                ScoreCardMetricID = ScoreCardMetricID,
                MetricID = mo.MetricID,
                OrgLocationID = mo.OrgLocationID,
                ScoreCardPeriodID = 1,
                MetricFrequencyID = mo.FrequencyID,
                PerformanceIndicatorId = mo.PerformanceIndicatorID,
                UomID = mo.UnitOfMeasureID
            };

            mo.MetricValues = new List<Extend>();
            for (int i = ValueCount - 1; i >= 0; i--)
            {
                DateTime dtBegin = Frequency.AddPeriod(EndDate, mo.FrequencyID, -i);
                DateTime dtEnd = Frequency.AddPeriod(dtBegin, mo.FrequencyID, 1);
                if (e.MetricID != null && e.MetricID!=Guid.Empty)
                {
                    e.CurrentValue = ScoreCardMetric.CalculateTotalValue(dc, LinqMicajahDataContext.OrganizationId, LinqMicajahDataContext.InstanceId, (Guid)e.MetricID, e.OrgLocationID, e.UomID, dtBegin, dtEnd, true, ScoreCardMetric.enTotalValueType.Sum, false);
                }
                else if (e.PerformanceIndicatorId!=null)
                {
                    var f = PerformanceIndicator.GetFormulasWithRealValues(dc, dtBegin, dtEnd, (new Guid[] { e.OrgLocationID == null ? Guid.Empty : (Guid)e.OrgLocationID }).ToList(), (Guid)e.PerformanceIndicatorId, e.UomID==null?Guid.Empty:(Guid)e.UomID);
                    double v = Calculator(f);
                    if (v != 0) e.CurrentValue = v;
                    //e.PerformanceIndicatorId;
                }
       
                Extend mv = new Extend()
                {
                    InstanceId = mo.InstanceId,
                    MetricID = mo.MetricID,
                    FrequencyID = mo.FrequencyID,
                    Date = dtBegin,
                    UnitOfMeasureID = e.UomID,
                    InputUnitOfMeasureID = e.UomID,
                    MetricDataTypeID = 1,
                    DValue = e.CurrentValue,
                    OrgLocationID = mo.OrgLocationID,
                    Period = Frequency.GetPeriodName(dtBegin, mo.FrequencyID, true)
                };

                mo.MetricValues.Add(mv);
                mo.UnitOfMeasureID = e.UomID;
            }
            return mo;
        }
    }
}
