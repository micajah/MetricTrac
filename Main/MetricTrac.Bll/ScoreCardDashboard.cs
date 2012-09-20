using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class ScoreCardDashboard
    {
        public class Extend : ScoreCardMetric.Extend
        {
            public Guid ScoreCardDashboardID { get; set; }
            public Guid UserId { get; set; }
        }
        static public ScoreCardDashboard Get(Guid DasboardID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var cd = dc.ScoreCardDashboard.Where(d => d.ScoreCardDashboardID == DasboardID && d.InstanceId == LinqMicajahDataContext.InstanceId).FirstOrNull();
                return cd;
            }
        }
        static public List<Extend> List(DateTime? Date, Bll.ScoreCardMetric.CalcStringFormula csf)
        {
            using(LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
            var r = from d in dc.ScoreCardDashboard
                    join cm in dc.ScoreCardMetric on new { d.InstanceId, d.ScoreCardMetricID, Status = (bool?)true } equals new { cm.InstanceId, cm.ScoreCardMetricID, cm.Status }
                    join p in dc.ScoreCardPeriod on d.ScoreCardPeriodID equals p.ScoreCardPeriodID
                    join o in dc.ViewnameOrgLocation on new { InstanceId = (Guid?)cm.InstanceId, cm.OrgLocationID } equals new { o.InstanceId, o.OrgLocationID }
                    join _m in dc.Metric on new { LinqMicajahDataContext.InstanceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, MetricID = (Guid?)_m.MetricID, _m.Status } into __m
                    join _i in dc.PerformanceIndicator on new { LinqMicajahDataContext.InstanceId, cm.PerformanceIndicatorId, Status = (bool?)true } equals new { _i.InstanceId, PerformanceIndicatorId = (Guid?)_i.PerformanceIndicatorID, _i.Status } into __i
                    //join _m in dc.Metric on new { LinqMicajahDataContext.InstanceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, _m.MetricID, _m.Status } into __m
                    //join _v in dc.ScoreCardValue on new { LinqMicajahDataContext.InstanceId, cm.ScoreCardMetricID, Status = (bool?)true } equals new { _v.InstanceId, _v.ScoreCardMetricID, _v.Status } into __v

                    from m in __m.DefaultIfEmpty()
                    from i in __i.DefaultIfEmpty()
                    //from v in __v.DefaultIfEmpty()

                    where d.UserId == LinqMicajahDataContext.LogedUserId && d.InstanceId == LinqMicajahDataContext.InstanceId && d.Status == true

                    select new Extend
                    {
                        InstanceId = d.InstanceId,
                        ScoreCardDashboardID = d.ScoreCardDashboardID,
                        ScoreCardMetricID = d.ScoreCardMetricID,
                        MetricID = cm.MetricID,
                        PerformanceIndicatorId = cm.PerformanceIndicatorId,

                        UomID = cm.MetricID == null ? (i.UnitOfMeasureID) : (m.UnitOfMeasureID == null ? m.InputUnitOfMeasureID : m.UnitOfMeasureID),
                        AltUomID = cm.MetricID == null ? i.AltUnitOfMeasureID : null,

                        OrgLocationID = cm.OrgLocationID,
                        ScoreCardPeriodID = d.ScoreCardPeriodID == null ? 1 : (int)d.ScoreCardPeriodID,
                        Status = d.Status,
                        Created = d.Created,
                        Updated = d.Updated,

                        MinValue = d.MinValue,
                        MaxValue = d.MaxValue,
                        BaselineValue = d.BaselineValue,
                        Breakpoint1Value = d.Breakpoint1Value,
                        Breakpoint2Value = d.Breakpoint2Value,
                        BaselineValueLabel = d.BaselineValueLabel,
                        Breakpoint1ValueLabel = d.Breakpoint1ValueLabel,
                        Breakpoint2ValueLabel = d.Breakpoint2ValueLabel,

                        MetricName = m.MetricID != null ? m.Name : "PI: " + i.Name,
                        MetricFrequencyID = m.FrequencyID == null ? 0 : m.FrequencyID,
                        MetricNODecPlaces = cm.MetricID == null ? (int)i.DecimalPlaces :m.NODecPlaces,
                        GrowUpIsGood = m.GrowUpIsGood,

                        //CurrentValue = v.CurrentValue,
                        //PreviousValue = v.PreviousValue,

                        ScoreCardPeriodName = p.Name,
                        OrgLocationName = o.FullName
                    };

            DateTime BaseDate = Date == null ? DateTime.Today : (DateTime)Date;
            var l = r.ToList();
            foreach (var v in l)
            {
                if (v.ScoreCardPeriodID == null) continue;
                ScoreCardMetric.PeriodDate p1;
                ScoreCardMetric.PeriodDate p2;

                ScoreCardMetric.GetPeriodDate((int)v.ScoreCardPeriodID, v.MetricFrequencyID, BaseDate, out p1, out p2);
                if (v.MetricID != null)
                {
                    v.CurrentValue = ScoreCardMetric.CalculateTotalValue(dc, LinqMicajahDataContext.OrganizationId, LinqMicajahDataContext.InstanceId, (Guid)v.MetricID, v.OrgLocationID, v.UomID, p1.Begin, p1.End, true, ScoreCardMetric.enTotalValueType.Sum, false);
                    if (p2 != null) v.PreviousValue = ScoreCardMetric.CalculateTotalValue(dc, LinqMicajahDataContext.OrganizationId, LinqMicajahDataContext.InstanceId, (Guid)v.MetricID, v.OrgLocationID, v.UomID, p2.Begin, p2.End, true, ScoreCardMetric.enTotalValueType.Sum, false);
                }
                else
                {
                    if (csf != null)
                    {
                        List<Guid> ogl = new List<Guid>();
                        ogl.Add(v.OrgLocationID == null ? Guid.Empty : (Guid)v.OrgLocationID);

                        List<string> ResultFormula = Bll.PerformanceIndicator.GetFormulasWithRealValues(dc, p1.Begin, p1.End, ogl, (Guid)v.PerformanceIndicatorId, v.UomID == null ? Guid.Empty : (Guid)v.UomID);
                        v.CurrentValue = csf(ResultFormula);
                        if (v.CurrentValue == 0) v.CurrentValue = null;

                        if (p2 != null)
                        {
                            ResultFormula = Bll.PerformanceIndicator.GetFormulasWithRealValues(dc, p2.Begin, p2.End, ogl, (Guid)v.PerformanceIndicatorId, v.UomID == null ? Guid.Empty : (Guid)v.UomID);
                            v.PreviousValue = csf(ResultFormula);
                            if (v.PreviousValue == null) v.PreviousValue = null;
                        }
                    }
                }
                ScoreCardMetric.Fill(v);
            }

            return l;
            }
        }


        /*static void Fill(Extend m)
        {
            string UomName = null;
            if (m.UomID != null) UomName = Micajah.Common.Bll.MeasureUnit.GetMeasureUnitName((Guid)m.UomID, Micajah.Common.Bll.MeasureUnitName.SingularAbbreviation);
            m.CurrentValueStr = ScoreCardMetric.GetValueStr(m.CurrentValue, m.MetricNODecPlaces, UomName);
            m.PreviousValueStr = ScoreCardMetric.GetValueStr(m.PreviousValue, m.MetricNODecPlaces, UomName);

            if (m.CurrentValue != null && m.PreviousValue != null)
            {
                if (m.PreviousValue == 0)
                {
                    m.ChangeValue = double.PositiveInfinity;
                    m.ChangeValueStr = "~";
                }
                else
                {
                    try
                    {
                        m.ChangeValue = (1 - m.CurrentValue / m.PreviousValue) * 100;
                        m.ChangeValueStr = ((double)m.ChangeValue).ToString("#0");
                        m.ChangeValueStr += "%";
                        if (m.ChangeValue < 100) m.ChangeValueStr = "(" + m.ChangeValueStr + ")";
                    }
                    catch
                    {
                        m.ChangeValueStr = "~";
                    }
                }
            }
        }*/

        /*public override void OnInserting(LinqMicajahDataContext dc, ref bool Cancel)
        {
            LinqMicajahDataContext NewDc = new LinqMicajahDataContext();
            ScoreCardDashboard scd = (from d in NewDc.ScoreCardDashboard
                                      where d.ScoreCardMetricID == ScoreCardMetricID && d.InstanceId == InstanceId && d.UserId == UserId && d.Status == false
                                      select d).FirstOrNull();
            if (scd != null)
            {
                scd.Status = true;
                NewDc.SubmitChanges();
                Cancel = true;
            }
            base.OnInserting(dc, ref Cancel);

        }
        public override void OnUpdating(LinqMicajahDataContext dc, ref bool Cancel)
        {
        }*/
    }
}
