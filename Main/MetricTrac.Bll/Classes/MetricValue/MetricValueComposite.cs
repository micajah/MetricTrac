using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{   
    internal class CompositeDailyValues
    {
        private class CompositeInfo
        {
            public decimal Value { get; set; }
            public int FrequencyID { get; set; }
            public DateTime InputDate { get; set; }
        }

        Dictionary<DateTime, CompositeInfo> DailyValues;

        public CompositeDailyValues()
        {
            DailyValues = new Dictionary<DateTime, CompositeInfo>();
        }

        public void AddCompositeDailyValue(DateTime InputDate, decimal Value, int FrequencyID, DateTime date, int MetricFrequencyID)
        {
            try
            {
                if (DailyValues == null)
                {
                    DailyValues = new Dictionary<DateTime, CompositeInfo>();
                }

                DateTime BeginPeriod = Frequency.GetNormalizedDate(FrequencyID, date);
                DateTime EndPeripd = Frequency.AddPeriod(BeginPeriod, FrequencyID, 1);
                decimal v = Value / (EndPeripd - BeginPeriod).Days;
                for (DateTime Date = BeginPeriod; Date < EndPeripd; Date = Date.AddDays(1))
                {
                    CompositeInfo ci;
                    if (FrequencyID == MetricFrequencyID || !DailyValues.Keys.Contains(Date))
                    {
                        ci = new CompositeInfo();
                        DailyValues[Date] = ci;
                    }
                    else
                    {
                        ci = DailyValues[Date];
                    }

                    ci.FrequencyID = FrequencyID;
                    ci.Value = v;
                    ci.InputDate = InputDate;
                }
            }
            catch { }
        }

        public void AddCompositeDailyValue(DateTime InputDate, decimal Value, int FrequencyID, DateTime date)
        {
            AddCompositeDailyValue(InputDate, Value, FrequencyID, date, -1);
        }

        public decimal? GetCompositeValue(DateTime BeginPeriod, DateTime EndPeripd, bool InterpolateMissingDays)
        {
            try
            {
                if (DailyValues == null) return null;
                int MissCount = 0;
                decimal TotalValue = 0;
                decimal LastValue = 0;
                bool IsExist = false;

                for (DateTime Date = BeginPeriod; Date < EndPeripd; Date = Date.AddDays(1))
                {
                    if (!DailyValues.Keys.Contains(Date))
                    {
                        MissCount++;
                        continue;
                    }
                    CompositeInfo ci = DailyValues[Date];

                    LastValue = ci.Value;
                    TotalValue += (InterpolateMissingDays ? (MissCount + 1) : 1) * LastValue;
                    MissCount = 0;
                    IsExist = true;
                }
                if (!IsExist) return null;
                if (InterpolateMissingDays) TotalValue += MissCount * LastValue;
                return TotalValue;
            }
            catch
            {
                return null;
            }
        }

        public decimal? GetCompositeValue(int FrequencyID, DateTime date, bool InterpolateMissingDays)
        {
            DateTime BeginPeriod = Frequency.GetNormalizedDate(FrequencyID, date);
            DateTime EndPeripd = Frequency.AddPeriod(BeginPeriod, FrequencyID, 1);
            return GetCompositeValue(BeginPeriod, EndPeripd, InterpolateMissingDays);
        }
    }
    
}
