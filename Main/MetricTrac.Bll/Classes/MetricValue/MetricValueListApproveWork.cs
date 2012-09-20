using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class MetricValue
    {
        public static List<EntityValue> WorkList(int ValueCount, PageEntityID EntityPageInfo, Guid? @ApproverUserId, bool OrderByMetric)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            Guid? EntityID = null;
            int SkipCount = 0;
            if (EntityPageInfo != null)
            {
                EntityID = EntityPageInfo.EntityID;
                SkipCount = EntityPageInfo.PageNumber * ValueCount;
            }

            ISingleResult<Sp_SelectApproverWorkListResult> V = dc.Sp_SelectApproverWorkList(LinqMicajahDataContext.InstanceId, ValueCount, SkipCount, EntityID, OrderByMetric, @ApproverUserId);

            List<EntityValue> MetricMetricValues = new List<EntityValue>();
            EntityValue LastMetricMetricValue = null;
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            int i = 0;
            foreach (var v in V)
            {
                if (LastMetricMetricValue == null
                    || (OrderByMetric && v.MetricID != LastMetricMetricValue.EntityID)
                    || (!OrderByMetric && v.OrgLocationID != LastMetricMetricValue.EntityID))
                {
                    i = 0;
                    LastMetricMetricValue = new EntityValue();
                    // copy metric data
                    LastMetricMetricValue.FrequencyID = OrderByMetric ? v.MetricFrequencyID : null;
                    LastMetricMetricValue.PageCount = (SkipCount / ValueCount) + 1;
                    LastMetricMetricValue.EntityID = OrderByMetric ? (Guid)v.MetricID : (Guid)v.OrgLocationID;
                    LastMetricMetricValue.EntityName = OrderByMetric ? v.MetricName : v.OrgLocationFullName;
                    LastMetricMetricValue.IsMoreValues = false;
                    LastMetricMetricValue.MetricTypeID = (int)v.MetricTypeID;
                    MetricMetricValues.Add(LastMetricMetricValue);
                }
                if (i == ValueCount)
                {
                    LastMetricMetricValue.IsMoreValues = true;
                    continue;
                }
                MetricValue.Extend LastMetricValue = new MetricValue.Extend();
                // metric data
                LastMetricValue.InstanceId = (Guid)v.InstanceId;
                LastMetricValue.MetricID = (Guid)v.MetricID;
                LastMetricValue.NODecPlaces = v.NODecPlaces;
                LastMetricValue.MetricName = v.MetricName;
                LastMetricValue.MetricCategoryName = v.MetricCategoryFullName;
                // value data
                LastMetricValue.MetricValueID = (Guid)v.MetricValueID;
                LastMetricValue.FrequencyID = (int)v.ValueFrequencyID;
                LastMetricValue.ValueFrequencyName = v.ValueFrequencyName;
                LastMetricValue.Date = (DateTime)v.Date;
                LastMetricValue.Period = Frequency.GetPeriodName((DateTime)v.Date, LastMetricValue.FrequencyID, true);
                LastMetricValue.Value = v.Value;
                LastMetricValue.Approved = v.Approved;
                LastMetricValue.ReviewUpdated = (bool)v.ReviewUpdated;
                LastMetricValue.MissedCalc = (bool)v.MissedCalc;
                string title = String.Empty;
                if (v.MetricValueID == null || v.MetricValueID == Guid.Empty || String.IsNullOrEmpty(v.Value)/* || v.Status == false*/) //!!! add after fix stored procedure
                {
                    LastMetricValue.ApprovalStatus = "Missing&nbsp;Value";
                }
                else
                {
                    if (v.MetricTypeID != 1)
                    {
                        title = "Calc&nbsp;value";
                        if (v.MissedCalc == true)
                            title += " | Some&nbsp;input&nbsp;values&nbsp;missed";
                    }
                    else
                        title = "Input&nbsp;value";
                    LastMetricValue.ApprovalStatus = (v.Approved == null ? (v.ReviewUpdated == true ? "Under&nbsp;Review | Updated&nbsp;by&nbsp;Collector" : "Under&nbsp;Review") : ((bool)v.Approved ? "Approved" : "Pending"));
                }
                title = String.IsNullOrEmpty(title) ? String.Empty : "(" + title + ") ";
                LastMetricValue.Notes = title + v.Notes;
                LastMetricValue.FilesAttached = (bool)v.FilesAttached;
                LastMetricValue.ReviewUpdated = (bool)v.ReviewUpdated;
                LastMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                LastMetricValue.MetricDataTypeID = (int)v.ValueDataTypeID;
                LastMetricValue.InputUnitOfMeasureID = v.ValueInputUnitOfMeasureID;
                LastMetricValue.ValueInputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, LastMetricValue.InputUnitOfMeasureID);
                LastMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                LastMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                LastMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                LastMetricValue.IsCalculated = v.MetricTypeID != 1;
                LastMetricMetricValue.EntityValues.Add(LastMetricValue);
                i++;
            }
            return MetricMetricValues;
        }
    }
}
