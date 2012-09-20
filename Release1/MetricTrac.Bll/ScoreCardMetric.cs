using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class ScoreCardMetric
    {
        public class Extend : ScoreCardMetric
        {
            public string MetricName { get; set; }
            public int MetricFrequencyID { get; set; }
            public int? MetricNODecPlaces { get; set; }
            public bool? GrowUpIsGood { get; set; }

            public string OrgLocationName { get; set; }

            public double? CurrentValue { get; set; }
            public string CurrentValueStr { get; set; }
            public double? PreviousValue { get; set; }
            public string PreviousValueStr { get; set; }
            public double? ChangeValue { get; set; }
            public string ChangeValueStr { get; set; }

            public Guid? UomID { get; set; }

            public string ScoreCardPeriodName { get; set; }
        }

        class PeriodDate
        {
            public DateTime Begin;
            public DateTime End;
        }

        static PeriodDate GetPeriod(int FrequencyID, bool prev, DateTime DTNow)
        {
            PeriodDate ret = new PeriodDate();
            ret.Begin = Frequency.GetNormalizedDate(FrequencyID, DTNow);
            if (prev) ret.Begin = Frequency.AddPeriod(ret.Begin, FrequencyID, -1);
            ret.End = Frequency.AddPeriod(ret.Begin, FrequencyID, 1);
            return ret;
        }

        static PeriodDate GetYearAgo(PeriodDate p)
        {
            PeriodDate ret = new PeriodDate();
            ret.Begin = p.Begin.AddYears(-1);
            ret.End = p.End.AddYears(-1);
            return ret;
        }

        static void GetPeriodDate(int ScoreCardPeriodID, int MetricFrequencyID, DateTime DTNow, out PeriodDate CurentPeriod, out PeriodDate PrevPeriod)
        {
            CurentPeriod = null;
            PrevPeriod = null;
            switch (ScoreCardPeriodID)
            {
                case  1://Current Period
                    CurentPeriod = GetPeriod(MetricFrequencyID, false, DTNow);
                    break;
                case 2://Current Month
                    CurentPeriod = GetPeriod(3, false, DTNow);
                    break;
                case 3://Current Quarter
                    CurentPeriod = GetPeriod(4, false, DTNow);
                    break;
                case 4://Current Year
                    CurentPeriod = GetPeriod(6, false, DTNow);
                    break;
                case 5://Previous Period
                    PrevPeriod = GetPeriod(MetricFrequencyID, true, DTNow);
                    break;
                case 6://Previous Month
                    PrevPeriod = GetPeriod(3, true, DTNow);
                    break;
                case 7://Previous Quarter
                    PrevPeriod = GetPeriod(4, true, DTNow);
                    break;
                case 8://Previous Year
                    PrevPeriod = GetPeriod(6, true, DTNow);
                    break;
                case 9://Current Period Versus Previous
                    CurentPeriod = GetPeriod(MetricFrequencyID, false, DTNow);
                    PrevPeriod = GetPeriod(MetricFrequencyID, true, DTNow);
                    break;
                case 10://Current Period Versus Period 1 Year Ago
                    CurentPeriod = GetPeriod(MetricFrequencyID, false, DTNow);
                    PrevPeriod = GetYearAgo(CurentPeriod);
                    break;
                case 11://Current Month Versus Prevous Month
                    CurentPeriod = GetPeriod(3, false, DTNow);
                    PrevPeriod = GetPeriod(3, true, DTNow);
                    break;
                case 12://Current Month Versus Month 1 Year Ago
                    CurentPeriod = GetPeriod(3, false, DTNow);
                    PrevPeriod = GetYearAgo(CurentPeriod);
                    break;
                case 13://Current Quarter Versus Pevious Quarter
                    CurentPeriod = GetPeriod(4, false, DTNow);
                    PrevPeriod = GetPeriod(4, true, DTNow);
                    break;
                case 14://Current Quarter Versus Quarter 1 Year Ago
                    CurentPeriod = GetPeriod(4, false, DTNow);
                    PrevPeriod = GetYearAgo(CurentPeriod);
                    break;
                case 15://Current Year Versus Pervious Year
                    CurentPeriod = GetPeriod(6, false, DTNow);
                    PrevPeriod = GetPeriod(6, true, DTNow);
                    break;
                case 16://Current Period YTD vs Previous Period
                    CurentPeriod = new PeriodDate();
                    CurentPeriod.End = DTNow.Date;
                    CurentPeriod.Begin = new DateTime(CurentPeriod.End.Year, 1, 1);
                    break;
                case 17://Current YTD Rolling Average vs Previous YTD
                    CurentPeriod = new PeriodDate();
                    CurentPeriod.End = DTNow.Date;
                    CurentPeriod.Begin = new DateTime(CurentPeriod.End.Year, 1, 1);
                    break;
            }
        }

        static double? GetValue(LinqMicajahDataContext dc, Guid MetricID, Guid? OrgLocationID, Guid? UomID, PeriodDate d, Guid OrganizationId, Guid InstanceId, out Guid? TotalUnitOfMessureID)
        {
            var values = (from m in dc.Metric
                          join v in dc.MetricValue on new { MetricID, m.FrequencyID, Status=(bool?)true } equals new { v.MetricID, v.FrequencyID, v.Status }
                          join o in dc.EntityNodeFullNameView on v.OrgLocationID equals (Guid?)o.EntityNodeId
                          join r in dc.D_MetricOrgLocationRule on new { InstanceId, v.MetricID, v.OrgLocationID } equals new { r.InstanceId, r.MetricID, r.OrgLocationID }

                          where (
                                    OrgLocationID == null || OrgLocationID == Guid.Empty || OrgLocationID == v.OrgLocationID ||
                                    OrgLocationID == o.Parent1 || OrgLocationID == o.Parent2 || OrgLocationID == o.Parent3 || OrgLocationID == o.Parent4 || OrgLocationID == o.Parent5
                                ) &&
                                m.MetricID == MetricID && m.Status==true &&
                                v.Date >= d.Begin && v.Date < d.End
                          select v).Distinct().ToList();

            TotalUnitOfMessureID = null;
            if(values.Count<1) return null;

            double Sum = 0;
            bool ValueExist = false;

            TotalUnitOfMessureID = values[0].InputUnitOfMeasureID;
            for (int i = 1; i < values.Count;i++ )
            {
                Guid? UID = values[i].InputUnitOfMeasureID;
                if (UID != TotalUnitOfMessureID)
                {
                    TotalUnitOfMessureID = UomID;
                    break;
                }
            }
            foreach (var v in values)
            {
                if (string.IsNullOrEmpty(v.Value)) continue;
                double SingleValue;

                try { SingleValue = double.Parse(v.Value, System.Globalization.NumberStyles.Any); }
                catch { continue; }

                if (TotalUnitOfMessureID != v.InputUnitOfMeasureID)
                {
                    if (TotalUnitOfMessureID == null || v.InputUnitOfMeasureID == null) continue;
                    double? ConvertedVal = Mc_UnitsOfMeasure.ConvertValue(SingleValue, (Guid)v.InputUnitOfMeasureID, (Guid)TotalUnitOfMessureID, OrganizationId);
                    if (ConvertedVal == null) continue;
                    SingleValue = (double)ConvertedVal;
                }

                ValueExist = true;
                Sum += SingleValue;
            }
            if (!ValueExist) return null;

            return Sum;            
        }

        static public void Calculate(Guid MetricID, Guid? OrgLocationID, Guid? UomID, int ScoreCardPeriodID, int MetricFrequencyID, Guid OrganizationId, Guid InstanceId, out Guid? TotalUnitOfMessureID, out double? CurrentValue, out double? PreviousValue)
        {
            Calculate(new LinqMicajahDataContext(), MetricID, OrgLocationID, UomID, ScoreCardPeriodID, MetricFrequencyID, DateTime.Now, OrganizationId, InstanceId, out TotalUnitOfMessureID, out CurrentValue, out PreviousValue);
        }

        static public void Calculate(LinqMicajahDataContext dc, Guid MetricID, Guid? OrgLocationID, Guid? UomID, int ScoreCardPeriodID, int MetricFrequencyID, DateTime DTNow, Guid OrganizationId, Guid InstanceId, out Guid? TotalUnitOfMessureID, out double? CurrentValue, out double? PreviousValue)
        {
            CurrentValue = null;
            PreviousValue = null;
            PeriodDate PDCurrent;
            PeriodDate PDPrev;
            TotalUnitOfMessureID = UomID;
            GetPeriodDate(ScoreCardPeriodID, MetricFrequencyID, DTNow, out PDCurrent, out PDPrev);

            if (PDCurrent != null)
            {
                CurrentValue = GetValue(dc, MetricID, OrgLocationID, UomID, PDCurrent, OrganizationId, InstanceId, out TotalUnitOfMessureID);
            }
            if (PDPrev != null)
            {
                PreviousValue = GetValue(dc, MetricID, OrgLocationID, UomID, PDPrev, OrganizationId, InstanceId, out TotalUnitOfMessureID);
            }

            /*if (CurrentValue == null && PDCurrent!=null) // replace empty current value with previous for dashboard
                CurrentValue = PreviousValue;*/
        }

        static public List<Extend> List(Guid ScoreCardID)
        {
            return List(ScoreCardID, LinqMicajahDataContext.InstanceId);
        }

        static public List<Extend> ListUnusedDashboard(Guid ScoreCardID, Guid? ScoreCardDashboardID)
        {
            DateTime DTNow = DateTime.Now;
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            Guid? ScoreCardMetricID = null;
            if(ScoreCardDashboardID!=null)
            {
                ScoreCardMetricID = (
                                        from ud in dc.ScoreCardDashboard 
                                        where ud.ScoreCardDashboardID==ScoreCardDashboardID && 
                                            ud.InstanceId==LinqMicajahDataContext.InstanceId && 
                                            ud.Status==true 
                                        select (Guid?)ud.ScoreCardMetricID
                                       ).FirstOrDefault();
            }

            var r = (from cm in dc.ScoreCardMetric
                     join p in dc.ScoreCardPeriod on cm.ScoreCardPeriodID equals p.ScoreCardPeriodID
                     join o in dc.EntityNodeFullNameView on cm.OrgLocationID equals o.EntityNodeId
                     join _m in dc.Metric on new { InstanceId = LinqMicajahDataContext.InstanceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, _m.MetricID, _m.Status } into __m
                     join _v in dc.ScoreCardValue on new { InstanceId = LinqMicajahDataContext.InstanceId, cm.ScoreCardMetricID, Status = (bool?)true } equals new { _v.InstanceId, _v.ScoreCardMetricID, _v.Status } into __v
                     join _d in dc.ScoreCardDashboard on new { InstanceId = LinqMicajahDataContext.InstanceId, cm.ScoreCardMetricID, Status = (bool?)true, UserId=(Guid)LinqMicajahDataContext.LogedUserId } equals new { _d.InstanceId, _d.ScoreCardMetricID, _d.Status, _d.UserId } into __d

                     from m in __m.DefaultIfEmpty()

                     from v in __v.DefaultIfEmpty()

                     from d in __d.DefaultIfEmpty()

                     where cm.InstanceId == LinqMicajahDataContext.InstanceId && cm.Status == true && cm.ScoreCardID == ScoreCardID &&
                            (d.ScoreCardMetricID == null || (ScoreCardDashboardID != null && cm.ScoreCardMetricID == ScoreCardMetricID))

                     select new Extend
                     {
                         InstanceId = cm.InstanceId,
                         ScoreCardMetricID = cm.ScoreCardMetricID,
                         ScoreCardID = cm.ScoreCardID,
                         MetricID = cm.MetricID,
                         OrgLocationID = cm.OrgLocationID,
                         ScoreCardPeriodID = cm.ScoreCardPeriodID,
                         Status = cm.Status,
                         Created = cm.Created,
                         Updated = cm.Updated,

                         MinValue = cm.MinValue,
                         MaxValue = cm.MaxValue,
                         BaselineValue = cm.BaselineValue,
                         Breakpoint1Value = cm.Breakpoint1Value,
                         Breakpoint2Value = cm.Breakpoint2Value,

                         MetricName = m.Name,
                         MetricFrequencyID = m.FrequencyID == null ? 0 : m.FrequencyID,
                         MetricNODecPlaces = m.NODecPlaces,
                         GrowUpIsGood = m.GrowUpIsGood,

                         CurrentValue = v.CurrentValue,
                         PreviousValue = v.PreviousValue,
                         UomID = v.UnitsOfMeasureId,

                         ScoreCardPeriodName = p.Name,
                         OrgLocationName = cm.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : o.FullName
                     }).ToList();

            foreach (var m in r)
            {
                Fill(m);
            }
            return r;
        }

        static public List<Extend> List(Guid ScoreCardID, Guid InstanceId)
        {
            DateTime DTNow = DateTime.Now;
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            return List(ScoreCardID, null, dc, InstanceId);
        }

        public static string GetValueStr(double? v, int? NODecPlaces, string UnitsOfMeasureName)
        {
            if(v==null) return null;
            double V = (double)v;
            int n = NODecPlaces == null ? 2 : (int)NODecPlaces;
            /*string FormatString = "#0" + ((n > 0) ? "." : "");
            for (int i = 0; i < n; i++) FormatString += "0";
            string ValStr = V.ToString(FormatString);*/
            string ValStr = V.ToString("N" + NODecPlaces.ToString());
            if (!string.IsNullOrEmpty(UnitsOfMeasureName)) ValStr += "&nbsp;" + UnitsOfMeasureName;
            return ValStr.Replace(" ", "&nbsp;");
        }

        static public void Fill(Extend m)
        {
            string UomName = null;
            if (m.UomID != null) UomName = Micajah.Common.Bll.MeasureUnit.GetMeasureUnitName((Guid)m.UomID, Micajah.Common.Bll.MeasureUnitName.SingularAbbreviation);
            m.CurrentValueStr = GetValueStr(m.CurrentValue, m.MetricNODecPlaces, UomName);
            m.PreviousValueStr = GetValueStr(m.PreviousValue, m.MetricNODecPlaces, UomName);

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
                        m.ChangeValue =  (m.CurrentValue - m.PreviousValue) / m.PreviousValue * 100;//  (1 - m.CurrentValue / m.PreviousValue) * 100;
                        m.ChangeValueStr = ((double)m.ChangeValue).ToString("#0");
                        m.ChangeValueStr += "%";
                    }
                    catch
                    {
                        m.ChangeValueStr = "~";
                    }
                }
            }
        }        

        static public List<Extend> List(Guid ScoreCardID, DateTime? DTNow, LinqMicajahDataContext dc, Guid ScoreCardInstaceId)
        {

            var r = (from cm in dc.ScoreCardMetric
                     join i in dc.Mc_Instance on new { cm.InstanceId, Deleted = false } equals new { i.InstanceId, i.Deleted }
                     join org in dc.Mc_Organization on new { i.OrganizationId, Deleted=false } equals new { org.OrganizationId, org.Deleted }
                     join p in dc.ScoreCardPeriod on cm.ScoreCardPeriodID equals p.ScoreCardPeriodID
                     join o in dc.EntityNodeFullNameView on cm.OrgLocationID == null ? Guid.Empty : (Guid)cm.OrgLocationID equals o.EntityNodeId
                     join _m in dc.Metric on new { InstanceId = ScoreCardInstaceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, _m.MetricID, _m.Status } into __m
                     join _v in dc.ScoreCardValue on new { InstanceId = ScoreCardInstaceId, cm.ScoreCardMetricID, Status = (bool?)true } equals new { _v.InstanceId, _v.ScoreCardMetricID, _v.Status } into __v

                     from m in __m.DefaultIfEmpty()

                     from v in __v.DefaultIfEmpty()

                     where cm.InstanceId == ScoreCardInstaceId && cm.Status == true && cm.ScoreCardID == ScoreCardID

                     select new Extend
                     {
                         InstanceId = cm.InstanceId,
                         ScoreCardMetricID = cm.ScoreCardMetricID,
                         ScoreCardID = cm.ScoreCardID,
                         MetricID = cm.MetricID,
                         OrgLocationID = cm.OrgLocationID,
                         ScoreCardPeriodID = cm.ScoreCardPeriodID,
                         Status = cm.Status,
                         Created = cm.Created,
                         Updated = cm.Updated,

                         MinValue = cm.MinValue,
                         MaxValue = cm.MaxValue,
                         BaselineValue = cm.BaselineValue,
                         Breakpoint1Value = cm.Breakpoint1Value,
                         Breakpoint2Value = cm.Breakpoint2Value,
                         BaselineValueLabel = cm.BaselineValueLabel,
                         Breakpoint1ValueLabel = cm.Breakpoint1ValueLabel,
                         Breakpoint2ValueLabel = cm.Breakpoint2ValueLabel,

                         MetricName = m.Name,
                         MetricFrequencyID = m.FrequencyID == null ? 0 : m.FrequencyID,
                         MetricNODecPlaces = m.NODecPlaces,
                         GrowUpIsGood = m.GrowUpIsGood,

                         CurrentValue = v.CurrentValue,
                         PreviousValue = v.PreviousValue,
                         UomID = v.UnitsOfMeasureId,

                         ScoreCardPeriodName = p.Name,
                         OrgLocationName = (cm.OrgLocationID == null || cm.OrgLocationID == Guid.Empty) ? org.Name : o.FullName
                     }).ToList();

            foreach (var m in r)
            {
                Fill(m);
                /*if (DTNow != null)
                {
                    Guid? BaseUom = Micajah.Common.Bll.MeasureUnit.
                    Calculate(m.MetricID, m.OrgLocationID, Micajah.Common.Bll.MeasureUnit.GetMeasureUnitName(
                }*/
            }
            return r;
        }


        static public Extend Get(Guid ScoreCardMetricID, Guid CardInstanceId)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            Extend mv = (
                from cm in dc.ScoreCardMetric
                join p in dc.ScoreCardPeriod on cm.ScoreCardPeriodID equals p.ScoreCardPeriodID
                join m in dc.Metric on new { InstanceId = CardInstanceId, cm.MetricID, Status = (bool?)true } equals new { m.InstanceId, m.MetricID, m.Status }
                join o in dc.EntityNodeFullNameView on cm.OrgLocationID equals o.EntityNodeId 
                join _v in dc.ScoreCardValue on new { InstanceId = CardInstanceId, cm.ScoreCardMetricID, Status = (bool?)true } equals new { _v.InstanceId, _v.ScoreCardMetricID, _v.Status } into __v
 
                from v in __v.DefaultIfEmpty()

                where cm.InstanceId == CardInstanceId && cm.ScoreCardMetricID == ScoreCardMetricID && cm.Status == true
                select new Extend
                {
                    InstanceId = CardInstanceId,
                    ScoreCardMetricID = cm.ScoreCardMetricID,
                    ScoreCardID = cm.ScoreCardID,
                    MetricID = cm.MetricID,
                    OrgLocationID = cm.OrgLocationID,
                    ScoreCardPeriodID = cm.ScoreCardPeriodID,
                    Status = cm.Status,
                    Created = cm.Created,
                    Updated = cm.Updated,

                    MinValue = cm.MinValue,
                    MaxValue = cm.MaxValue,
                    BaselineValue = cm.BaselineValue,
                    Breakpoint1Value = cm.Breakpoint1Value,
                    Breakpoint2Value = cm.Breakpoint2Value,
                    BaselineValueLabel = cm.BaselineValueLabel,
                    Breakpoint1ValueLabel = cm.Breakpoint1ValueLabel,
                    Breakpoint2ValueLabel = cm.Breakpoint2ValueLabel,

                    MetricName = m.Name,
                    MetricFrequencyID = m.FrequencyID == null ? 0 : m.FrequencyID,
                    MetricNODecPlaces = m.NODecPlaces,
                    GrowUpIsGood = m.GrowUpIsGood,

                    CurrentValue = v.CurrentValue,
                    PreviousValue = v.PreviousValue,
                    UomID = v.UnitsOfMeasureId,

                    ScoreCardPeriodName = p.Name,
                    OrgLocationName = cm.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : o.FullName
                }
                ).FirstOrNull();
            if (mv != null) Fill(mv);
            return mv;
        }

        static public Extend Get(Guid ScoreCardMetricID)
        {
            return Get(ScoreCardMetricID, LinqMicajahDataContext.InstanceId);
        }
    }
}
