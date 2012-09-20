using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public enum GroupByMode { Location = 0, Metric, MetricCategory }

    public class DateHeader
    {
        public DateTime Date { get; set; }
        public string sDate { get; set; }
    }

    public class FormulaHeader
    {
        public int ColGroup { get; set; }
        public string sFormula { get; set; }
    }

    public class FrequencyMetric : Frequency
    {
        public List<MetricOrgValue> Metrics {get;set;}
        public List<DateHeader> Date { get; set; }
        public List<FormulaHeader> Formulas { get; set; }
    }

    public class MetricOrgValue : Metric.Extend
    {
        public Guid OrgLocationID { get; set; }
        public Guid? PerformanceIndicatorID { get; set; }
        public string OrgLocationFullName {get;set;}
        public int GroupCount { get; set; }
        public int GroupNumber { get; set; }

        public Guid? CollectorUserID { get; set; }
        public string CollectorFullName { get; set; }

        public List<MetricValue.Extend> MetricValues {get;set;}

        public List<MetricTrac.Bll.GroupCategoryAspect.Extend> GCA { get; set; }
        public List<MetricTrac.Bll.PerformanceIndicator> Pi { get; set; }        

        public Guid? RelatedOrgLocationUoMRecordID { get; set; }
        public Guid? OrgLocationUnitOfMeasureID { get; set; }
        public string OrgLocationUnitOfMeasureName { get; set; }
        public string InputUnitOfMeasureNameTooltip { get; set; }         
                
        public Guid? RelatedOrgLocationNameRecordID { get; set; }
        public string MetricOrgLocationAlias { get; set; }
        public string MetricOrgLocationCode { get; set; }
        public bool IsTotalAgg { get; set; }  

        public List<DateHeader> Date { get; set; }

        public MetricOrgValue()
        {
            MetricValues = new List<MetricValue.Extend>();
            GCA = new List<GroupCategoryAspect.Extend>();
            Pi = new List<PerformanceIndicator>();
        }
    }

    public class DistinctMetricOrgValue : MetricOrgValue
    {     
        public List<DateHeader> DatesHeader { get; set; }
        public bool IsPreviousValues { get; set; }
        public bool IsNextValues { get; set; }
        public DateTime NextDate { get; set; }
        public DateTime PreviousDate { get; set; }
    }

    public class DistinctFrequencyMetric : Frequency
    {
        public List<DistinctMetricOrgValue> Metrics { get; set; }        
    }
        
    public class EntityValue
    {
        public int? FrequencyID { get; set; }
        public Guid EntityID { get; set; }
        public string EntityName { get; set; }
        public int PageCount { get; set; }
        public bool IsMoreValues { get; set; }
        public int MetricTypeID { get; set; }
        public List<MetricValue.Extend> EntityValues { get; set; }

        public EntityValue()
        {
            EntityValues = new List<MetricValue.Extend>();            
        }
    }
        
    public class PageEntityID
    {        
        public Guid EntityID;
        public int PageNumber;
    }

    [Serializable]
    public struct FrequencyMetricOrgLocationID
    {
        public int FrequencyID;
        public Guid MetricID;
        public Guid OrgLocationID;
    }

    public partial class MetricValue
    {
        public sealed class Extend : MetricValue
        {
            // Value reference fields
            public string ValueFrequencyName { get; set; }            
            public string ValueDataTypeName { get; set; }            
            public string ValueInputUnitOfMeasureName { get; set; }
            public string ValueUnitOfMeasureName { get; set; }
            public string Period { get; set; }
            public string ApprovalStatus { get; set; }
            public string OrgLocationFullName { get; set; }
            public Guid? RelatedOrgLocationUoMRecordID { get; set; }
            public Guid? OrgLocationUnitOfMeasureID { get; set; }
            public string OrgLocationUnitOfMeasureName { get; set; }
            public double? DValue { get; set; }

            // Metric Fields
            public string MetricName { get; set; }            
            public int MetricFrequencyID { get; set; }
            public int ActualMetricDataTypeID { get; set; }
            public Guid? MetricCategoryID { get; set; }
            public Guid? MetricUnitOfMeasureID { get; set; }
            public Guid? MetricInputUnitOfMeasureID { get; set; }
            public int? NODecPlaces { get; set; }
            public decimal? NOMinValue { get; set; }
            public decimal? NOMaxValue { get; set; }
            public string FormulaCode { get; set; }
            public string Variable { get; set; }            
            public string Description { get; set; }
            public string Documentation { get; set; }
            public string Definition { get; set; }
            public string References { get; set; }
            public DateTime? CollectionStartDate { get; set; }
            public DateTime? CollectionEndDate { get; set; }
            public bool CollectionEnabled { get; set; }

            public bool AllowMetricCustomNames { get; set; }
            public Guid? RelatedOrgLocationNameRecordID { get; set; }            
            public string MetricOrgLocationAlias { get; set; }
            public string MetricOrgLocationCode { get; set; }

            // Metric reference fields
            public string MetricFrequencyName { get; set; }
            public string MetricCategoryName { get; set; }
            public Guid? RelatedFormulaID { get; set; }
            public string Formula { get; set; }
            public string VariableFormula { get; set; }
            public string MetricUnitOfMeasureName { get; set; }
            public string MetricInputUnitOfMeasureName { get; set; }
            public string MetricDataTypeName { get; set; }

            public bool? IsCalculated { get; set; }
            public bool IsAbsent { get; set; }

            public bool IsTotalAgg { get; set; }        
        }


        private static bool IsCollectingDate(DateTime? StartDate, DateTime? EndDate, DateTime ValueDate)
        {
            bool IsCollectingDate = true;
            if (StartDate != null || EndDate != null)
            { // input mode
                if (StartDate != null && EndDate == null)
                {
                    IsCollectingDate = ValueDate >= (DateTime)StartDate;
                }
                else if (StartDate == null && EndDate != null)
                {
                    IsCollectingDate = ValueDate <= (DateTime)EndDate;
                }
                else // both are not null
                {
                    IsCollectingDate = (ValueDate >= (DateTime)StartDate && ValueDate <= (DateTime)EndDate);
                }
            }
            return IsCollectingDate;
        }

        public static string GetLocationsEncodedString(Guid?[] SelectedLocations, char Divider)
        {
            String sLocations = String.Empty;
            if (SelectedLocations != null)
                if (SelectedLocations.Length > 0)
                    foreach (Guid? g in SelectedLocations)
                        if (g != null)
                            sLocations += g.ToString() + Divider;
            if (!String.IsNullOrEmpty(sLocations))
                sLocations = sLocations.TrimEnd(Divider);
            else
                sLocations = null;
            return sLocations;
        }

        public static void GetFullDateRange(out DateTime? From, out DateTime? To)
        {
            From = To = DateTime.Now.Date;
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                try{
                    From = (from v in dc.MetricValue where v.InstanceId == LinqMicajahDataContext.InstanceId && v.Status == true select v.Date).Min();
                }catch { }
                try{
                    To = (from v in dc.MetricValue where v.InstanceId == LinqMicajahDataContext.InstanceId && v.Status == true select v.Date).Max();
                }catch { }
            }
        }        

        public static readonly DateTime FailureDate = new DateTime(1900, 1, 1);        

        public static string GetMeasureUnitAbbvr(List<Micajah.Common.Bll.MeasureUnit> OrgUoMs, Guid? MeasureUnitID)
        {
            Micajah.Common.Bll.MeasureUnit mu = OrgUoMs.Find(u => u.MeasureUnitId == MeasureUnitID);
            return mu == null ? String.Empty : mu.SingularAbbreviation;
        }
        
        public static string FormatValue(MetricValue.Extend Val)
        {
            return FormatValue(Val, false);
        }

        public static string FormatValue(MetricValue.Extend Val, bool BeEmpty)
        {
            string FormatString = string.Empty;

            if (String.IsNullOrEmpty(Val.Value) || BeEmpty)
                FormatString = "&nbsp";
            else
            {
                FormatString = Val.Value.ToString();
                if (Val.MetricDataTypeID == 1 && Val.NODecPlaces != null)
                    try
                    {
                        decimal dv = Decimal.Parse(Val.Value, System.Globalization.NumberStyles.Any);
                        FormatString = dv.ToString("N" + ((int)Val.NODecPlaces));
                    }
                    catch { }
            }                
            return FormatString;
        }
    }
}