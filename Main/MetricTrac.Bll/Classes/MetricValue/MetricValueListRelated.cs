using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class MetricValue
    {
        public static FrequencyMetric RelatedValuesList(int ValueCount, DateTime BaseDate, DateTime FirstDate, Guid CalcMetricID, bool GroupByMetric, Guid? @ApproveUserId)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            var fm = (from f in dc.Frequency
                      join m in dc.Metric on f.FrequencyID equals m.FrequencyID
                      where m.MetricID == CalcMetricID
                      select new FrequencyMetric
                      {
                          FrequencyID = f.FrequencyID,
                          Name = f.Name
                      }).FirstOrNull();
            if (fm != null)
            {
                DateTime NormalizedDate = FirstDate;
                if (FirstDate == DateTime.MinValue)
                    NormalizedDate = Frequency.GetNormalizedDate(fm.FrequencyID, BaseDate);
                fm.Date = GetDateHeader(fm.FrequencyID, NormalizedDate, ValueCount);
                fm.Metrics = RelatedValuesList(ValueCount, NormalizedDate, fm.FrequencyID, dc, fm.Date, CalcMetricID, GroupByMetric, @ApproveUserId);
            }
            return fm;
        }

        public static List<MetricOrgValue> RelatedValuesList(int ValueCount, DateTime NormalizedDate, int FrequencyID, LinqMicajahDataContext dc, List<DateHeader> hl, Guid? CalcMetricID, bool OrderByMetric, Guid? @ApproverUserId)
        {
            DateTime EndDate = Frequency.AddPeriod(NormalizedDate, FrequencyID, 1);
            DateTime BeginDate = Frequency.AddPeriod(EndDate, FrequencyID, -ValueCount);
            ISingleResult<Sp_SelectMetricRelatedValuesResult> V = dc.Sp_SelectMetricRelatedValues(LinqMicajahDataContext.InstanceId, FrequencyID, CalcMetricID, BeginDate, EndDate, OrderByMetric, @ApproverUserId);
            List<MetricOrgValue> MetricMetricValues = new List<MetricOrgValue>();
            MetricOrgValue LastMetricMetricValue = null;
            int GroupNumber = 0;
            int GroupCount = 0;
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            foreach (var v in V)
            {
                if (LastMetricMetricValue == null || v.MetricID != LastMetricMetricValue.MetricID || v.OrgLocationID != LastMetricMetricValue.OrgLocationID)
                {
                    if (LastMetricMetricValue == null || v.OrgLocationID == LastMetricMetricValue.OrgLocationID)
                        GroupNumber++;
                    else
                    {
                        SetOrgLocationNumber(GroupNumber, MetricMetricValues);
                        GroupNumber = 1;
                        GroupCount++;
                    }
                    PushMetricValue(LastMetricMetricValue, null, hl);
                    LastMetricMetricValue = new MetricOrgValue();
                    LastMetricMetricValue.GroupCount = GroupCount;

                    // Common fields
                    LastMetricMetricValue.InstanceId = (Guid)v.InstanceId;
                    LastMetricMetricValue.FrequencyID = FrequencyID;
                    LastMetricMetricValue.FrequencyName = v.FrequencyName;

                    //Metric fields                    
                    LastMetricMetricValue.MetricID = (Guid)v.MetricID;
                    LastMetricMetricValue.Name = v.MetricName;
                    LastMetricMetricValue.MetricTypeID = (int)v.MetricTypeID;
                    LastMetricMetricValue.MetricDataTypeID = 1;
                    LastMetricMetricValue.NODecPlaces = v.NODecPlaces;
                    LastMetricMetricValue.InputUnitOfMeasureID = v.MetricInputUnitOfMeasureID;
                    LastMetricMetricValue.InputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.MetricInputUnitOfMeasureID);

                    //MetricOrg fields                    
                    LastMetricMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                    LastMetricMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                    LastMetricMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                    LastMetricMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                    LastMetricMetricValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.OrgLocationUnitOfMeasureID);

                    // define actual measure unit
                    if (v.MetricOrgLocationUoMID != null)
                        LastMetricMetricValue.InputUnitOfMeasureName = LastMetricMetricValue.OrgLocationUnitOfMeasureName;
                    MetricMetricValues.Add(LastMetricMetricValue);
                }

                MetricValue.Extend LastMetricValue = new MetricValue.Extend();
                // Value fields
                LastMetricValue.InstanceId = (Guid)v.InstanceId;
                LastMetricValue.MetricValueID = (Guid)v.MetricValueID;
                LastMetricValue.MetricID = (Guid)v.MetricID;
                LastMetricValue.Date = (DateTime)v.Date;
                LastMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                LastMetricValue.FrequencyID = FrequencyID;
                LastMetricValue.Value = v.Value;
                LastMetricValue.Verified = v.Verified == null ? false : (bool)v.Verified;
                LastMetricValue.Approved = v.MetricValueID == Guid.Empty ? false : v.Approved;
                LastMetricValue.FilesAttached = v.FilesAttached == null ? false : (bool)v.FilesAttached;
                LastMetricValue.ReviewUpdated = v.ReviewUpdated == null ? false : (bool)v.ReviewUpdated;
                LastMetricValue.MissedCalc = v.MissedCalc == null ? false : (bool)v.MissedCalc;
                LastMetricValue.MetricDataTypeID = 1;
                LastMetricValue.InputUnitOfMeasureID = v.ValueInputUnitOfMeasureID;

                // Extend fields
                // extend - value reference
                LastMetricValue.ValueFrequencyName = v.FrequencyName;
                LastMetricValue.ValueInputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.ValueInputUnitOfMeasureID);
                LastMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                LastMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                LastMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                LastMetricValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.OrgLocationUnitOfMeasureID);

                // extend - metric fields
                LastMetricValue.MetricName = v.MetricName;
                LastMetricValue.MetricFrequencyID = FrequencyID;
                LastMetricValue.ActualMetricDataTypeID = 1;
                LastMetricValue.MetricInputUnitOfMeasureID = v.MetricInputUnitOfMeasureID;
                LastMetricValue.NODecPlaces = v.NODecPlaces;
                LastMetricValue.MetricInputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.MetricInputUnitOfMeasureID);
                LastMetricValue.IsCalculated = (int)v.MetricTypeID > 1;
                LastMetricValue.IsAbsent = false;
                LastMetricValue.RelatedFormulaID = v.MetricFormulaID;
                LastMetricValue.Formula = v.Formula;

                PushMetricValue(LastMetricMetricValue, LastMetricValue, hl);
            }
            SetOrgLocationNumber(GroupNumber, MetricMetricValues);
            PushMetricValue(LastMetricMetricValue, null, hl);
            return MetricMetricValues;
        }

        private static void PushMetricValue(MetricOrgValue mmv, MetricValue.Extend mv, List<DateHeader> hl)
        {
            if (mmv == null) return;
            DateTime NextDate = DateTime.MinValue;
            DateTime AddDate = mv == null ? DateTime.MinValue : mv.Date;
            while (mmv.MetricValues.Count < hl.Count && (NextDate = hl[mmv.MetricValues.Count].Date) > AddDate)
            {
                MetricValue.Extend CalculatedValue = new MetricValue.Extend();
                CalculatedValue.InstanceId = mmv.InstanceId;
                CalculatedValue.MetricValueID = Guid.Empty;
                CalculatedValue.MetricID = mmv.MetricID;
                CalculatedValue.Date = NextDate;
                CalculatedValue.OrgLocationID = mmv.OrgLocationID;
                CalculatedValue.FrequencyID = mmv.FrequencyID;
                CalculatedValue.Value = null;
                CalculatedValue.Verified = false;
                CalculatedValue.Approved = false;
                CalculatedValue.FilesAttached = false;
                CalculatedValue.ReviewUpdated = false;
                CalculatedValue.MissedCalc = false;
                CalculatedValue.MetricDataTypeID = 1;
                CalculatedValue.InputUnitOfMeasureID = mmv.InputUnitOfMeasureID;
                CalculatedValue.IsCalculated = (int)mmv.MetricTypeID > 1;
                CalculatedValue.IsAbsent = true;
                mmv.MetricValues.Add(CalculatedValue);
            }

            if (mv != null && mmv.MetricValues.Count < hl.Count && hl[mmv.MetricValues.Count].Date == mv.Date)
                mmv.MetricValues.Add(mv);
        }
    }
}
