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
        static public List<Extend> List(DateTime? Date)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var r = from d in dc.ScoreCardDashboard
                    join cm in dc.ScoreCardMetric on new { d.InstanceId, d.ScoreCardMetricID, Status = (bool?)true } equals new { cm.InstanceId, cm.ScoreCardMetricID, cm.Status }
                    join p in dc.ScoreCardPeriod on cm.ScoreCardPeriodID equals p.ScoreCardPeriodID
                    join o in dc.EntityNodeFullNameView on cm.OrgLocationID equals o.EntityNodeId
                    join _m in dc.Metric on new { LinqMicajahDataContext.InstanceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, _m.MetricID, _m.Status } into __m
                    join _v in dc.ScoreCardValue on new { LinqMicajahDataContext.InstanceId, cm.ScoreCardMetricID, Status = (bool?)true } equals new { _v.InstanceId, _v.ScoreCardMetricID, _v.Status } into __v

                    from m in __m.DefaultIfEmpty()

                    from v in __v.DefaultIfEmpty()

                    where d.UserId == LinqMicajahDataContext.LogedUserId && d.InstanceId == LinqMicajahDataContext.InstanceId && d.Status == true

                    select new Extend
                    {
                        InstanceId = cm.InstanceId,
                        ScoreCardDashboardID = d.ScoreCardDashboardID,
                        ScoreCardMetricID = cm.ScoreCardMetricID,
                        MetricID = cm.MetricID,
                        OrgLocationID = cm.OrgLocationID,
                        ScoreCardPeriodID = cm.ScoreCardPeriodID,
                        Status = d.Status,
                        Created = d.Created,
                        Updated = d.Updated,

                        MinValue = cm.MinValue,
                        MaxValue = cm.MaxValue,
                        BaselineValue = cm.BaselineValue,
                        Breakpoint1Value = cm.Breakpoint1Value,
                        Breakpoint2Value = cm.Breakpoint2Value,

                        MetricName = m.Name,
                        MetricFrequencyID = m.FrequencyID == null ? 0 : m.FrequencyID,
                        MetricNODecPlaces = m.NODecPlaces,

                        CurrentValue = v.CurrentValue,
                        PreviousValue = v.PreviousValue,
                        UomID = v.UnitsOfMeasureId,

                        ScoreCardPeriodName = p.Name,
                        OrgLocationName = cm.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : o.FullName
                    };
            var l = r.ToList();

            foreach (var v in l)
            {
                ScoreCardMetric.Fill(v);
            }

            return l;
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
