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
            public Guid? AltUomID { get; set; }

            public string ScoreCardPeriodName { get; set; }
        }

        public class PeriodDate
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

        public static void GetPeriodDate(int ScoreCardPeriodID, int MetricFrequencyID, DateTime DTNow, out PeriodDate CurentPeriod, out PeriodDate PrevPeriod)
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

        static List<MetricValue> GetValues(LinqMicajahDataContext dc, Guid InstanceId, Guid MetricID, Guid? OrgLocationID, DateTime BeginDate, DateTime? EndDate)
        {
            if (OrgLocationID == null) OrgLocationID = Guid.Empty;

            var values = (from h in dc.ViewHierarchyOrgLocation
                          join mo in dc.ViewMetricOrgLocation on new { InstanceId = (Guid?)InstanceId, MetricID = (Guid?)MetricID, OrgLocationID=(Guid?)h.SubOrgLocationID } equals new { mo.InstanceId, mo.MetricID, mo.OrgLocationID }
                          join v in dc.MetricValue on new { MetricID, OrgLocationID = (Guid?)h.SubOrgLocationID, Status = (bool?)true } equals new { v.MetricID, OrgLocationID = (Guid?)v.OrgLocationID, v.Status }
                          where h.InstanceId == InstanceId && h.OrgLocationID == OrgLocationID &&
                                ( dc.FxGetNextPeriodDate(v.FrequencyID, v.Date)>BeginDate &&
                                    (
                                        (EndDate==null && v.Date<=BeginDate )
                                        || 
                                        (EndDate != null && v.Date<EndDate)
                                    )
                                )
                          select v)
                          .ToList();

            return values;
        }

        static List<MetricValue> GetValue(LinqMicajahDataContext dc, Guid InstanceId, Guid MetricID, Guid? OrgLocationID, DateTime BeginDate, DateTime? EndDate)
        {
            Guid _OrgLocationID = Guid.Empty;
            if (OrgLocationID != null) _OrgLocationID=(Guid)OrgLocationID;

            var values = (from mo in dc.ViewMetricOrgLocation
                          join v in dc.MetricValue on new { MetricID, OrgLocationID=_OrgLocationID, Status = (bool?)true } equals new { v.MetricID, v.OrgLocationID, v.Status }
                          where mo.InstanceId == InstanceId && mo.OrgLocationID == OrgLocationID && mo.MetricID==MetricID &&
                                (dc.FxGetNextPeriodDate(v.FrequencyID, v.Date) > BeginDate &&
                                    (
                                        (EndDate == null && v.Date <= BeginDate)
                                        ||
                                        (EndDate != null && v.Date < EndDate)
                                    )
                                )
                          select v)
                          .ToList();

            return values;
        }

        public enum enTotalValueType { Sum=0, Average=1, RMS=2}

        public static double? CalculateTotalValue(LinqMicajahDataContext dc, Guid OrganizationId, Guid InstanceId, Guid MetricID, Guid? OrgLocationID, Guid? UomID, DateTime BeginDate, DateTime? EndDate, bool IncludeSubLocation, enTotalValueType TotalValueType, bool IgnoreUnitOfMessure)
        {
            DateTime d2000=new DateTime(2000,1,1);
            int Begin2000 = (int)(BeginDate - d2000).TotalDays;
            int End2000 = EndDate == null ? int.MaxValue : (int)((((DateTime)EndDate)) - d2000).TotalDays;
            var mvs = dc.Sp_CalcMetricValue(InstanceId, MetricID, OrgLocationID, Begin2000, End2000);

            var m = Bll.Metric.Get(MetricID);
            if (m == null) return null;
            if (IgnoreUnitOfMessure) UomID = m.UnitOfMeasureID;

            double v = 0;
            bool ValueExist = false;
            foreach (var mv in mvs)
            {
                if (mv.Val == null || mv.Val == 0) continue;
                ValueExist = true;
                if (mv.UnitOfMeasureID != UomID && mv.UnitOfMeasureID != null && UomID != null)
                {
                    double? v0 = Mc_UnitsOfMeasure.ConvertValue((double)((decimal)(mv.Val)), (Guid)mv.UnitOfMeasureID, (Guid)UomID, OrganizationId);
                    if (v0 != null) v += (double)v0;
                }
                else
                {
                    v += (double)mv.Val;
                }
            }

            return ValueExist ? v : (double?)null;
        }

        public static double? CalculateTotalValue3(LinqMicajahDataContext dc, Guid OrganizationId, Guid InstanceId, Guid MetricID, Guid? OrgLocationID, Guid? UomID, DateTime BeginDate, DateTime? EndDate, bool IncludeSubLocation, enTotalValueType TotalValueType)
        {
            double sum = 0;
            bool ValExist = false;
            List<MetricValue> values;
            if(IncludeSubLocation) values = GetValues(dc, InstanceId, MetricID, OrgLocationID, BeginDate, EndDate);
            else values = GetValue(dc, InstanceId, MetricID, OrgLocationID, BeginDate, EndDate);

            foreach (var v in values)
            {
                double dv;
                if(!double.TryParse(v.Value, out dv)) continue;
                if (v.InputUnitOfMeasureID != UomID && v.InputUnitOfMeasureID != null && UomID!=null)
                {
                    double? ConvertedValue = Mc_UnitsOfMeasure.ConvertValue(dv, (Guid)v.InputUnitOfMeasureID, (Guid)UomID, OrganizationId);
                    if (ConvertedValue == null) continue;
                    dv = (double)ConvertedValue;
                }

                if (EndDate != null)
                {
                    DateTime ValEndDate = MetricTrac.Bll.Frequency.AddPeriod(v.Date, v.FrequencyID, 1).AddDays(-1);
                    if (v.Date < BeginDate || ValEndDate > EndDate)
                    {
                        double ValDays = ((ValEndDate > EndDate ? (DateTime)EndDate : ValEndDate) - (v.Date < BeginDate ? BeginDate : v.Date)).TotalDays+1;
                        dv = dv * ValDays / ((ValEndDate - v.Date).TotalDays+1);
                    }
                }

                sum += dv;
                ValExist = true;
            }
            if (!ValExist) return null;
            return sum;
        }

        public static string CalculateTotalString(LinqMicajahDataContext dc, Guid OrganizationId, Guid InstanceId, Guid MetricID, Guid? OrgLocationID, Guid? UomID, DateTime BeginDate, DateTime EndDate)
        {
            var values = GetValues(dc, InstanceId, MetricID, OrgLocationID, BeginDate, EndDate);
            List<string> ExistValues = new List<string>();
            foreach (var v in values)
            {
                string sv = v.Value;
                if (string.IsNullOrEmpty(sv)) continue;
                sv = sv.Trim();
                if (ExistValues.Contains(sv)) continue;
                ExistValues.Add(sv);
            }
            if (ExistValues.Count < 1) return null;
            return string.Join(",", ExistValues.ToArray());
        }

        static double? _GetValue(LinqMicajahDataContext dc, Guid MetricID, Guid? OrgLocationID, Guid? UomID, PeriodDate d, Guid OrganizationId, Guid InstanceId, out Guid? TotalUnitOfMessureID)
        {
            var values = (from m in dc.Metric
                          join v in dc.MetricValue on new { MetricID, m.FrequencyID, Status = (bool?)true } equals new { v.MetricID, v.FrequencyID, v.Status }
                          //join o in dc.EntityNodeFullNameView on v.OrgLocationID equals (Guid?)o.EntityNodeId
                          join r in dc.FxGetRule(LinqMicajahDataContext.InstanceId) on new { InstanceId = (Guid?)v.InstanceId, MetricID = (Guid?)v.MetricID, OrgLocationID = (Guid?)v.OrgLocationID } equals new { r.InstanceId, r.MetricID, r.OrgLocationID }
                          join l in dc.ViewHierarchyOrgLocation on new { InstanceId = (Guid?)v.InstanceId, OrgLocationID = (Guid?)v.OrgLocationID } equals new { l.InstanceId, OrgLocationID = l.SubOrgLocationID }
                          join o in dc.ViewnameOrgLocation on new { InstanceId = (Guid?)v.InstanceId, OrgLocationID = (Guid?)v.OrgLocationID } equals new { o.InstanceId, o.OrgLocationID }
                          
                          where (
                                    OrgLocationID == null || OrgLocationID == Guid.Empty || OrgLocationID == l.OrgLocationID
                                    //OrgLocationID == v.OrgLocationID || OrgLocationID == o.Parent1 || OrgLocationID == o.Parent2 || OrgLocationID == o.Parent3 || OrgLocationID == o.Parent4 || OrgLocationID == o.Parent5
                                ) &&
                                m.MetricID == MetricID && m.Status == true &&
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
                //CurrentValue = GetValue(dc, MetricID, OrgLocationID, UomID, PDCurrent, OrganizationId, InstanceId, out TotalUnitOfMessureID);
                CurrentValue = CalculateTotalValue(dc, OrganizationId, InstanceId, MetricID, OrgLocationID, UomID, PDCurrent.Begin, PDCurrent.End, true, ScoreCardMetric.enTotalValueType.Sum, false);
            }
            if (PDPrev != null)
            {
                //PreviousValue = GetValue(dc, MetricID, OrgLocationID, UomID, PDPrev, OrganizationId, InstanceId, out TotalUnitOfMessureID);
                PreviousValue = CalculateTotalValue(dc, OrganizationId, InstanceId, MetricID, OrgLocationID, UomID, PDPrev.Begin, PDPrev.End, true, ScoreCardMetric.enTotalValueType.Sum, false);
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
                     //join o in dc.EntityNodeFullNameView on cm.OrgLocationID equals o.EntityNodeId
                     join o in dc.ViewnameOrgLocation on new { InstanceId = (Guid?)cm.InstanceId, OrgLocationID = (Guid?)cm.OrgLocationID } equals new { o.InstanceId, o.OrgLocationID }
                     join _m in dc.Metric on new { LinqMicajahDataContext.InstanceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, MetricID = (Guid?)_m.MetricID, _m.Status } into __m
                     join _i in dc.PerformanceIndicator on new { LinqMicajahDataContext.InstanceId, cm.PerformanceIndicatorId, Status = (bool?)true } equals new { _i.InstanceId, PerformanceIndicatorId = (Guid?)_i.PerformanceIndicatorID, _i.Status } into __i
                     join _v in dc.ScoreCardValue on new { InstanceId = LinqMicajahDataContext.InstanceId, cm.ScoreCardMetricID, Status = (bool?)true } equals new { _v.InstanceId, _v.ScoreCardMetricID, _v.Status } into __v
                     join _d in dc.ScoreCardDashboard on new { InstanceId = LinqMicajahDataContext.InstanceId, cm.ScoreCardMetricID, Status = (bool?)true, UserId = (Guid)LinqMicajahDataContext.LogedUserId } equals new { _d.InstanceId, _d.ScoreCardMetricID, _d.Status, _d.UserId } into __d

                     from m in __m.DefaultIfEmpty()
                     from i in __i.DefaultIfEmpty()
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
            return List(ScoreCardID, null, null, null, null, dc, InstanceId, null);
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

            if (m.UomID!=null && m.AltUomID != null)
            {
                UomName = Micajah.Common.Bll.MeasureUnit.GetMeasureUnitName((Guid)m.AltUomID, Micajah.Common.Bll.MeasureUnitName.SingularAbbreviation);
                double? AltVal;
                string AltStr;
                if(m.CurrentValue!=null)
                {
                    AltVal = MetricTrac.Bll.Mc_UnitsOfMeasure.ConvertValue((double)m.CurrentValue, (Guid)m.UomID, (Guid)m.AltUomID, LinqMicajahDataContext.OrganizationId);
                    AltStr = GetValueStr(AltVal, m.MetricNODecPlaces, UomName);
                    if (!string.IsNullOrEmpty(AltStr))
                    {
                        if (!string.IsNullOrEmpty(m.CurrentValueStr)) m.CurrentValueStr += " = ";
                        m.CurrentValueStr += AltStr;
                    }
                }
                if (m.PreviousValue != null)
                {
                    AltVal = MetricTrac.Bll.Mc_UnitsOfMeasure.ConvertValue((double)m.PreviousValue, (Guid)m.UomID, (Guid)m.AltUomID, LinqMicajahDataContext.OrganizationId);
                    AltStr = GetValueStr(AltVal, m.MetricNODecPlaces, UomName);
                    if (!string.IsNullOrEmpty(AltStr))
                    {
                        if (!string.IsNullOrEmpty(m.PreviousValueStr)) m.PreviousValueStr += " = ";
                        m.PreviousValueStr += AltStr;
                    }
                }
            }

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

        public delegate double CalcStringFormula(List<string> Formulas);

        static public List<Extend> List(Guid ScoreCardID, DateTime? StartDate, DateTime? EndDate, DateTime? StartCompDate, DateTime? EndCompDate, LinqMicajahDataContext dc, Guid ScoreCardInstaceId, CalcStringFormula csf)
        {

            var r = (from cm in dc.ScoreCardMetric
                     join o in dc.ViewnameOrgLocation on new { InstanceId = (Guid?)cm.InstanceId, OrgLocationID = (Guid?)(cm.OrgLocationID == null ? Guid.Empty : cm.OrgLocationID) } equals new { o.InstanceId, o.OrgLocationID }
                     join _m in dc.Metric on new { InstanceId = ScoreCardInstaceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, MetricID = (Guid?)_m.MetricID, _m.Status } into __m
                     join _i in dc.PerformanceIndicator on new { LinqMicajahDataContext.InstanceId, cm.PerformanceIndicatorId, Status = (bool?)true } equals new { _i.InstanceId, PerformanceIndicatorId = (Guid?)_i.PerformanceIndicatorID, _i.Status } into __i

                     from m in __m.DefaultIfEmpty()
                     from i in __i.DefaultIfEmpty()

                     where cm.InstanceId == ScoreCardInstaceId && cm.Status == true && cm.ScoreCardID == ScoreCardID

                     select new Extend
                     {
                         InstanceId = cm.InstanceId,
                         ScoreCardMetricID = cm.ScoreCardMetricID,
                         PerformanceIndicatorId = cm.PerformanceIndicatorId,

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

                         MetricName = m.MetricID!=null ? m.Name : "PI: "+i.Name,
                         MetricFrequencyID = m.FrequencyID == null ? 0 : m.FrequencyID,
                         MetricNODecPlaces = m.MetricID==null ? (int)i.DecimalPlaces: m.NODecPlaces,
                         GrowUpIsGood = m.GrowUpIsGood,

                         UomID = cm.MetricID==null?(i.UnitOfMeasureID):(m.UnitOfMeasureID == null ? m.InputUnitOfMeasureID : m.UnitOfMeasureID),
                         AltUomID = cm.MetricID==null?i.AltUnitOfMeasureID:null,

                         OrgLocationName = o.FullName
                     }).ToList();

            foreach (var m in r)
            {
                if (m.MetricID != null)
                {
                    if (StartDate != null) m.CurrentValue = CalculateTotalValue(dc, LinqMicajahDataContext.OrganizationId, LinqMicajahDataContext.InstanceId, (Guid)m.MetricID, m.OrgLocationID, m.UomID, (DateTime)StartDate, EndDate, true, ScoreCardMetric.enTotalValueType.Sum, false);
                    if (StartCompDate != null) m.PreviousValue = CalculateTotalValue(dc, LinqMicajahDataContext.OrganizationId, LinqMicajahDataContext.InstanceId, (Guid)m.MetricID, m.OrgLocationID, m.UomID, (DateTime)StartCompDate, EndCompDate, true, ScoreCardMetric.enTotalValueType.Sum, false);
                }
                else if (m.PerformanceIndicatorId!=null)
                {
                    if (csf != null)
                    {
                        List<Guid> ogl = new List<Guid>();
                        ogl.Add(m.OrgLocationID == null ? Guid.Empty : (Guid)m.OrgLocationID);
                        if (StartDate != null)
                        {
                            List<string> ResultFormula = Bll.PerformanceIndicator.GetFormulasWithRealValues(dc, (DateTime)StartDate, EndDate == null ? DateTime.MaxValue : (DateTime)EndDate, ogl, (Guid)m.PerformanceIndicatorId, m.UomID == null ? Guid.Empty : (Guid)m.UomID);
                            m.CurrentValue = csf(ResultFormula);
                            if (m.CurrentValue == 0) m.CurrentValue = null;
                        }
                        if (StartCompDate != null)
                        {
                            List<string> ResultFormula = Bll.PerformanceIndicator.GetFormulasWithRealValues(dc, (DateTime)StartCompDate, EndCompDate == null ? DateTime.MaxValue : (DateTime)EndCompDate, ogl, (Guid)m.PerformanceIndicatorId, m.UomID == null ? Guid.Empty : (Guid)m.UomID);
                            m.PreviousValue = csf(ResultFormula);
                            if (m.PreviousValue == null) m.PreviousValue = null;
                        }
                    }
                }
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
                join _m in dc.Metric on new { InstanceId = CardInstanceId, cm.MetricID, Status = (bool?)true } equals new { _m.InstanceId, MetricID=(Guid?)_m.MetricID, _m.Status } into __m
                //join o in dc.EntityNodeFullNameView on cm.OrgLocationID equals o.EntityNodeId
                join o in dc.ViewnameOrgLocation on new { InstanceId = (Guid?)cm.InstanceId, cm.OrgLocationID } equals new { o.InstanceId, o.OrgLocationID }
                join _v in dc.ScoreCardValue on new { InstanceId = CardInstanceId, cm.ScoreCardMetricID, Status = (bool?)true } equals new { _v.InstanceId, _v.ScoreCardMetricID, _v.Status } into __v
 
                from m in __m.DefaultIfEmpty()
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

        /*static public List<Extend> ListWithPI()
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var  l = dc.ScoreCardMetric
            }
        }*/
    }
}
