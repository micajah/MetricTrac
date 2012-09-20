using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class Frequency
    {
        public enum FrequencyName
        {
            Daily = 1,
            Weekly,
            Monthly,
            Qtrly,
            SemiAnnual,
            Annual,
            BiAnnual,
            FiscalQtr,
            FiscalSemiAnnual,
            FiscalAnnual,
            FiscalBiAnnual
        }

        public static string GetFrequencyName(int FrequencyID)
        {
            switch(FrequencyID)
            {
                case 1: return "Daily";
                case 2: return "Weekly";
                case 3: return "Monthly";
                case 4: return "Qtrly";
                case 5: return "SemiAnnual";
                case 6: return "Annual";
                case 7: return "BiAnnual";
                case 8: return "FiscalQtr";
                case 9: return "FiscalSemiAnnual";
                case 10: return "FiscalYear";
                case 11: return "FiscalBiAnnual";
            }
            return string.Empty;
        }

        public static DateTime AddPeriod(DateTime NormalizedDate, int FrequencyID, int PeriodCount)
        {
            if (PeriodCount == 0) return NormalizedDate;

            switch (FrequencyID)
            {
                case 1://Daily
                    return NormalizedDate.AddDays(PeriodCount);
                case 2://Weekly
                    return NormalizedDate.AddDays(PeriodCount * 7);
                case 3://Monthly
                    return NormalizedDate.AddMonths(PeriodCount);
                case 4://Qtrly
                    return NormalizedDate.AddMonths(PeriodCount * 3);
                case 5://SemiAnnual
                    return NormalizedDate.AddMonths(PeriodCount * 6);
                case 6://Annual
                    return NormalizedDate.AddYears(PeriodCount);
                case 7://BiAnnual
                    return NormalizedDate.AddYears(PeriodCount * 2);
                case 8://FiscalQtr
                    return NormalizedDate.AddMonths(PeriodCount * 3);
                case 9://FiscalSemiAnnual
                    return NormalizedDate.AddMonths(PeriodCount * 6);
                case 10://FiscalYear
                    return NormalizedDate.AddYears(PeriodCount);
                case 11://FiscalBiAnnual
                    return NormalizedDate.AddYears(PeriodCount * 2);
            }
            return DateTime.MinValue;
        }

        public static DateTime GetNormalizedDate(int FrequencyID, DateTime BaseDate)
        {
            DateTime d = BaseDate.Date;
            int fy = d.Month >= 10 ? d.Year + 1 : d.Year;
            switch (FrequencyID)
            {
                case 1://Daily
                    return d;
                case 2://Weekly
                    return d.AddDays(-((int)(d.DayOfWeek)));
                case 3://Monthly
                    return new DateTime(d.Year, d.Month, 1);
                case 4://Qtrly
                    return new DateTime(d.Year, d.Month - ((d.Month-1) % 3), 1);
                case 5://SemiAnnual
                    return new DateTime(d.Year, d.Month - ((d.Month-1) % 6), 1);
                case 6://Annual
                    return new DateTime(d.Year, 1, 1);
                case 7://BiAnnual
                    return new DateTime(d.Year - d.Year%2, 1, 1);
                case 8://FiscalQtr
                    return new DateTime(d.Year, d.Month - ((d.Month - 1) % 3), 1);
                case 9://FiscalSemiAnnual
                    if (d.Month < 4) return new DateTime(d.Year-1, 10, 1);
                    if (d.Month < 10) return new DateTime(d.Year, 4, 1);
                    return new DateTime(d.Year, 10, 1);
                case 10://FiscalYear
                    return new DateTime(fy - 1, 10, 1);
                case 11://FiscalBiAnnual
                    fy = fy - fy % 2;
                    return new DateTime(fy-1, 10, 1);
            }
            return DateTime.MinValue;
        }

        public static void GetFiscalYear(DateTime BaseDate, out int FiscalYear)
        {
            FiscalYear = BaseDate.Month >= 10 ? BaseDate.Year + 1 : BaseDate.Year;
        }

        public static DateTime GetFiscalYearStart(int FiscalYear)
        {
            return new DateTime(FiscalYear - 1, 10, 1);
        }

        public static void GetFiscalSemiYear(DateTime BaseDate, out int FiscalYear, out int FiscalSemiYear)
        {
            FiscalYear = BaseDate.Month >= 10 ? BaseDate.Year + 1 : BaseDate.Year;
            FiscalSemiYear = BaseDate.Month >= 10 ? 1 : 2;
        }
        public static void GetFiscalQtr(DateTime BaseDate, out int FiscalYear, out int FiscalQtr)
        {
            FiscalYear = BaseDate.Month >= 10 ? BaseDate.Year + 1 : BaseDate.Year;
            FiscalQtr = ((((BaseDate.Month + 5) / 3)-1) % 4)+1;
        }

        public static string GetPeriodName(DateTime NormalizedDate, int FrequencyID)
        {
            return GetPeriodName(NormalizedDate, FrequencyID, false);
        }

        public static string [] GetPeriodTwoRowName(DateTime NormalizedDate, int FrequencyID)
        {
            string Date = GetPeriodName(NormalizedDate, FrequencyID, true);
            return Date.Split('\n');
        }

        public static string GetPeriodName(DateTime NormalizedDate, int FrequencyID, bool ShortString)
        {            
            DateTime ShiftDate = NormalizedDate.AddMonths(3);
            string OutFormat;
            switch (FrequencyID)
            {
                case 1://Daily
                    if (ShortString) return NormalizedDate.ToString("MMM\ndd yy");
                    else return NormalizedDate.ToString("MMMM dd, yyyy - dddd");
                case 2://Weekly
                    DateTime FirstYearSunday = GetFirstYearSunday(NormalizedDate.Year);
                    TimeSpan ts = NormalizedDate.Subtract(FirstYearSunday);
                    int week = (int)(ts.TotalDays) / 7;
                    OutFormat = ShortString ? "W{0}\n{1}" : "Week {0}, {1}";
                    return String.Format(OutFormat,week, NormalizedDate.Year);
                case 3://Monthly
                    if (ShortString) return NormalizedDate.ToString("MMM\nyy");
                    return NormalizedDate.ToString("MMMM yyyy");
                case 4://Qtrly
                    int qtrlyNo = (NormalizedDate.Month + 2) / 3;
                    OutFormat = "Q{0}\n{1}";
                    return String.Format(OutFormat, qtrlyNo, NormalizedDate.Year);
                case 5://SemiAnnual
                    int semiannualNo = (NormalizedDate.Month + 5) / 6;
                    OutFormat = "S{0}\n{1}";
                    return String.Format(OutFormat, semiannualNo, NormalizedDate.Year);
                case 6://Annual
                    return NormalizedDate.Year.ToString();
                case 7://BiAnnual
                    return String.Format("{0}\n{1}", NormalizedDate.Year.ToString(), (NormalizedDate.Year + 1).ToString());
                case 8://FiscalQtr                    
                    int fqtrlyNo = (ShiftDate.Month + 2) / 3;
                    OutFormat = "FQ{0}\n{1}";
                    return String.Format(OutFormat, fqtrlyNo.ToString(), ShiftDate.Year.ToString());
                case 9://FiscalSemiAnnual                    
                    int fsemiannualNo = (ShiftDate.Month + 5) / 6;
                    OutFormat = "S{0}\n{1}";
                    return String.Format(OutFormat, fsemiannualNo.ToString(), ShiftDate.Year.ToString());
                case 10://FiscalYear
                    return ShiftDate.Year.ToString();
                case 11://FiscalBiAnnual
                    return String.Format("{0}\n{1}", ShiftDate.Year.ToString(), (ShiftDate.Year + 1).ToString());
            }
            return String.Empty;
        }

        public static DateTime GetFirstYearSunday(int Year)
        {
            DateTime FirstDay = new DateTime(Year, 1, 1);
            return FirstDay.AddDays(7-((int)(FirstDay.DayOfWeek)));
        }

        public static string GetMonthName(int Month)
        {
            switch (Month)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
                default:
                    return String.Empty;
            }
        }

        public static void FillPeriodDate()
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                dc.ExecuteCommand("delete from D_PeriodDate");
                for (int FrequencyID = 1; FrequencyID <= 11; FrequencyID++)
                {
                    DateTime b = new DateTime(2000, 1, 1);
                    b = GetNormalizedDate(FrequencyID, b);
                    DateTime dt;
                    DateTime EndDt = new DateTime(2075,1,1);
                    int PeriodNumber=0;

                    do
                    {
                        dt = AddPeriod(b, FrequencyID, PeriodNumber);
                        D_PeriodDate r = new D_PeriodDate()
                        {
                            FrequencyID = FrequencyID,
                            Date = dt,
                            PeriodNumber = PeriodNumber,
                            ShortName = GetPeriodName(dt, FrequencyID, true),
                            LongName = GetPeriodName(dt, FrequencyID, false)
                        };
                        dc.D_PeriodDate.InsertOnSubmit(r);
                        dc.SubmitChanges();
                        PeriodNumber++;
                    }
                    while (dt <= EndDt);
                }
            }
        }

        public static List<int> ListUsed()
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var FId = (from m in dc.MetricValue where m.Status == true && m.InstanceId == LinqMicajahDataContext.InstanceId select m.FrequencyID).Distinct<int>();
                return FId.ToList();
            }
        }
    }
}



