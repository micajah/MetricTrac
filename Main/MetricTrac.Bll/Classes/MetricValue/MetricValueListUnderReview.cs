using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class MetricValue
    {
        public static List<DistinctFrequencyMetric> AlertQueueList(DateTime BaseDate, Dictionary<FrequencyMetricOrgLocationID, DateTime> FrequencyFirstDate, Guid? SelUserId, Guid? @ApproverUserId, bool ViewMode, bool OrderByMetric)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            List<DistinctFrequencyMetric> fml = (from f in dc.Frequency
                                                 orderby f.FrequencyID
                                                 select new DistinctFrequencyMetric
                                                 {
                                                     FrequencyID = f.FrequencyID,
                                                     Name = f.Name
                                                 }).ToList();

            for (int i = 0; i < fml.Count(); i++)
            {
                DistinctFrequencyMetric fm = fml[i];
                DateTime FirstDate = Frequency.GetNormalizedDate(fm.FrequencyID, BaseDate);
                fm.Metrics = AlertQueueList(dc, FirstDate, fm.FrequencyID, null, null, SelUserId, @ApproverUserId, ViewMode, OrderByMetric);

                if (fm.Metrics.Count < 1)
                {
                    fml.Remove(fm);
                    i--;
                }
                else
                {
                    foreach (FrequencyMetricOrgLocationID fmo in FrequencyFirstDate.Keys)
                        if (fmo.FrequencyID == fm.FrequencyID)
                        {
                            List<DistinctMetricOrgValue> r = AlertQueueList(dc, FrequencyFirstDate[fmo], fm.FrequencyID, fmo.MetricID, fmo.OrgLocationID, SelUserId, @ApproverUserId, ViewMode, OrderByMetric);
                            if (r != null)
                                foreach (DistinctMetricOrgValue dmo in r)
                                {
                                    int OldMetricOrgValueIndex = fm.Metrics.FindIndex(n => (n.FrequencyID == dmo.FrequencyID && n.MetricID == dmo.MetricID && n.OrgLocationID == dmo.OrgLocationID));
                                    if (OldMetricOrgValueIndex >= 0)
                                        fm.Metrics[OldMetricOrgValueIndex] = dmo;
                                }
                        }
                }
            }

            return fml;
        }

        public static List<DistinctMetricOrgValue> AlertQueueList(LinqMicajahDataContext dc, DateTime NormalizedDate, int FrequencyID, Guid? MetricID, Guid? OrgLocationID, Guid? SelUserId, Guid? @ApproverUserId, bool ViewMode, bool OrderByMetric)
        {
            DateTime EndDate = Frequency.AddPeriod(NormalizedDate, FrequencyID, 1);
            ISingleResult<Sp_SelectUnderReviewMetricValuesResult> V = dc.Sp_SelectUnderReviewMetricValues(LinqMicajahDataContext.InstanceId, EndDate, ViewMode, FrequencyID, MetricID, OrgLocationID, SelUserId, @ApproverUserId, OrderByMetric);

            List<DistinctMetricOrgValue> MetricMetricValues = new List<DistinctMetricOrgValue>();
            DistinctMetricOrgValue LastMetricMetricValue = null;

            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            int i = 0;
            int j = 0;
            DateTime NextDate = DateTime.MinValue;
            foreach (var v in V)
            {
                if (LastMetricMetricValue == null
                    || v.MetricID != LastMetricMetricValue.MetricID
                    || v.OrgLocationID != LastMetricMetricValue.OrgLocationID)
                {
                    i = 0;
                    j = 0;
                    LastMetricMetricValue = new DistinctMetricOrgValue();
                    // copy metric data
                    LastMetricMetricValue.InstanceId = (Guid)v.InstanceId;
                    LastMetricMetricValue.MetricID = (Guid)v.MetricID;
                    LastMetricMetricValue.Name = v.MetricName;
                    LastMetricMetricValue.FrequencyID = (int)v.MetricFrequencyID;
                    LastMetricMetricValue.MetricTypeID = (int)v.MetricTypeID;
                    LastMetricMetricValue.MetricDataTypeID = (int)v.MetricDataTypeID;
                    LastMetricMetricValue.NODecPlaces = v.NODecPlaces;
                    LastMetricMetricValue.InputUnitOfMeasureID = v.MetricInputUnitOfMeasureID;
                    LastMetricMetricValue.InputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.MetricInputUnitOfMeasureID);
                    LastMetricMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                    LastMetricMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                    LastMetricMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                    LastMetricMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                    LastMetricMetricValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.OrgLocationUnitOfMeasureID);
                    LastMetricMetricValue.AllowCustomNames = (bool)v.AllowCustomNames;
                    // find actual input measure unit for metric-org location pair
                    LastMetricMetricValue.InputUnitOfMeasureName = (LastMetricMetricValue.RelatedOrgLocationUoMRecordID == null) ? LastMetricMetricValue.InputUnitOfMeasureName : LastMetricMetricValue.OrgLocationUnitOfMeasureName;
                    LastMetricMetricValue.RelatedOrgLocationNameRecordID = v.MetricOrgLocationNameID;
                    LastMetricMetricValue.MetricOrgLocationAlias = v.CustomMetricAlias;
                    LastMetricMetricValue.MetricOrgLocationCode = v.CustomMetricCode;
                    LastMetricMetricValue.IsPreviousValues = false;
                    LastMetricMetricValue.IsNextValues = false;
                    LastMetricMetricValue.PreviousDate = DateTime.MinValue;
                    LastMetricMetricValue.NextDate = DateTime.MinValue;
                    LastMetricMetricValue.DatesHeader = new List<DateHeader>();
                    MetricMetricValues.Add(LastMetricMetricValue);
                }

                if (v.ValuePosType == "A") // check for existed left values
                {
                    j++;
                    if (j == 2)
                        LastMetricMetricValue.NextDate = (DateTime)v.Date;
                    continue;
                }

                if (LastMetricMetricValue.MetricValues.Count == 6) // check for existed right values
                {
                    LastMetricMetricValue.IsPreviousValues = true;
                    continue;
                }

                if (j == 2)
                    LastMetricMetricValue.IsNextValues = true;

                MetricValue.Extend LastMetricValue = new MetricValue.Extend();
                // metric data
                LastMetricValue.InstanceId = (Guid)v.InstanceId;
                LastMetricValue.MetricID = (Guid)v.MetricID;
                LastMetricValue.NODecPlaces = v.NODecPlaces;
                // value data
                LastMetricValue.MetricValueID = (Guid)v.MetricValueID;
                LastMetricValue.FrequencyID = (int)v.MetricFrequencyID;
                LastMetricValue.Date = (DateTime)v.Date;
                LastMetricValue.Value = v.Value;
                LastMetricValue.Approved = v.Approved;
                LastMetricValue.FilesAttached = (bool)v.FilesAttached;
                LastMetricValue.ReviewUpdated = (bool)v.ReviewUpdated;
                LastMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                LastMetricValue.MetricDataTypeID = (int)v.ValueDataTypeID;
                LastMetricValue.InputUnitOfMeasureID = v.ValueInputUnitOfMeasureID;
                LastMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                LastMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                LastMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                LastMetricValue.IsCalculated = v.MetricTypeID != 1;

                DateHeader h = new DateHeader();
                h.Date = (DateTime)v.Date;
                h.sDate = Frequency.GetPeriodName((DateTime)v.Date, FrequencyID, true);
                LastMetricMetricValue.DatesHeader.Add(h);

                if (i == 1)
                    LastMetricMetricValue.PreviousDate = LastMetricValue.Date;

                LastMetricMetricValue.MetricValues.Add(LastMetricValue);
                i++;
            }
            foreach (DistinctMetricOrgValue m in MetricMetricValues)
            {
                if (m.MetricValues.Count < 6)
                {
                    int count = 6 - m.MetricValues.Count;
                    for (int k = 1; k <= count; k++)
                    {
                        MetricValue.Extend AnotherMetricValue = new MetricValue.Extend();
                        // metric data
                        AnotherMetricValue.InstanceId = LastMetricMetricValue.InstanceId;
                        AnotherMetricValue.MetricID = LastMetricMetricValue.MetricID;
                        AnotherMetricValue.NODecPlaces = LastMetricMetricValue.NODecPlaces;
                        // value data
                        AnotherMetricValue.MetricValueID = Guid.Empty;
                        AnotherMetricValue.FrequencyID = FrequencyID;
                        AnotherMetricValue.Date = DateTime.MinValue;
                        AnotherMetricValue.Value = null;
                        AnotherMetricValue.Approved = false;
                        AnotherMetricValue.FilesAttached = false;
                        AnotherMetricValue.ReviewUpdated = false;
                        AnotherMetricValue.OrgLocationID = LastMetricMetricValue.OrgLocationID;
                        AnotherMetricValue.MetricDataTypeID = LastMetricMetricValue.MetricDataTypeID;
                        AnotherMetricValue.InputUnitOfMeasureID = LastMetricMetricValue.InputUnitOfMeasureID;
                        AnotherMetricValue.OrgLocationFullName = LastMetricMetricValue.OrgLocationFullName;
                        AnotherMetricValue.RelatedOrgLocationUoMRecordID = LastMetricMetricValue.RelatedOrgLocationUoMRecordID;
                        AnotherMetricValue.OrgLocationUnitOfMeasureID = LastMetricMetricValue.OrgLocationUnitOfMeasureID;
                        AnotherMetricValue.IsCalculated = LastMetricMetricValue.MetricTypeID != 1;
                        DateHeader ah = new DateHeader();
                        ah.Date = DateTime.MinValue;
                        ah.sDate = "&nbsp;";
                        m.DatesHeader.Add(ah);
                        m.MetricValues.Add(AnotherMetricValue);
                    }
                }
            }
            return MetricMetricValues;
        }
    }
}
