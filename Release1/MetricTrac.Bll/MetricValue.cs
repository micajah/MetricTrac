using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace MetricTrac.Bll
{
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
                    TotalValue += (InterpolateMissingDays?(MissCount + 1):1) * LastValue;
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


    public class MetricOrgValue : Metric.Extend
    {
        public Guid OrgLocationID { get; set; }        
        public string OrgLocationFullName {get;set;}
        public int GroupCount { get; set; }
        public int GroupNumber { get; set; }

        public Guid? CollectorUserID { get; set; }
        public string CollectorFullName { get; set; }

        public List<MetricValue.Extend> MetricValues {get;set;}

        public List<MetricTrac.Bll.GroupCategoryAspect.Extend> GCA { get; set; }
        public List<MetricTrac.Bll.PerformanceIndicator> Pi { get; set; }
        public List<MetricTrac.Bll.PerformanceIndicatorForm> PiForm { get; set; }

        public Guid? RelatedOrgLocationUoMRecordID { get; set; }
        public Guid? OrgLocationUnitOfMeasureID { get; set; }
        public string OrgLocationUnitOfMeasureName { get; set; }
        public string InputUnitOfMeasureNameTooltip { get; set; }         
                
        public Guid? RelatedOrgLocationNameRecordID { get; set; }
        public string MetricOrgLocationAlias { get; set; }
        public string MetricOrgLocationCode { get; set; }

        public MetricOrgValue()
        {
            MetricValues = new List<MetricValue.Extend>();
            GCA = new List<GroupCategoryAspect.Extend>();
            PiForm = new List<PerformanceIndicatorForm>();
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
        }

        private static void AddMetricValue(MetricOrgValue mmv, List<DateHeader> hl, MetricValue.Extend mv, CompositeDailyValues cdv, int FrequencyID, bool ViewMode)
        {
            if (mmv == null) return;

            DateTime NextDate;
            DateTime AddDate = mv == null ? DateTime.MinValue : mv.Date;

            while (mmv.MetricValues.Count < hl.Count && (NextDate = hl[mmv.MetricValues.Count].Date) > AddDate)
            {
                MetricValue.Extend CalculatedValue = new MetricValue.Extend();
                CalculatedValue.Verified = false;
                CalculatedValue.Date = NextDate;
                CalculatedValue.Approved = false;
                CalculatedValue.MetricDataTypeID = 1;
                CalculatedValue.IsCalculated = true;
                CalculatedValue.IsAbsent = true;
                CalculatedValue.MetricID = mmv.MetricID;
                CalculatedValue.InstanceId = mmv.InstanceId;
                CalculatedValue.CollectionStartDate = mmv.CollectionStartDate;
                CalculatedValue.CollectionEndDate = mmv.CollectionEndDate;
                CalculatedValue.CollectionEnabled = (ViewMode || IsCollectingDate(CalculatedValue.CollectionStartDate, CalculatedValue.CollectionEndDate, CalculatedValue.Date));
                
                if (cdv != null)
                {
                    decimal? v = cdv.GetCompositeValue(FrequencyID, NextDate, false);
                    if (v != null) CalculatedValue.Value  = ((decimal)v).ToString();
                }

                mmv.MetricValues.Add(CalculatedValue);
           }

            if (mv != null &&
                mmv.MetricValues.Count < hl.Count && 
                hl[mmv.MetricValues.Count].Date == mv.Date) 
                mmv.MetricValues.Add(mv);
        }

        private static List<DateHeader> GetDateHeader(int FrequencyID, DateTime FirstDate, int ValueCount)
        {
            List<DateHeader> hl = new List<DateHeader>();
            DateTime CurrentDate = Frequency.GetNormalizedDate(FrequencyID, FirstDate);
            for (int n = 0; n < ValueCount; n++)
            {
                DateTime d = Frequency.AddPeriod(CurrentDate, FrequencyID, -n);
                DateHeader h = new DateHeader();
                h.Date = d;
                h.sDate = Frequency.GetPeriodName(d, FrequencyID, true);

                hl.Add(h);
            }
            return hl;
        }

        private static List<DateHeader> GetDateHeader(int FrequencyID, DateTime BeginDate, DateTime EndDate)
        {
            List<DateHeader> hl = new List<DateHeader>();
            DateTime CurrentDate = EndDate;
            while (CurrentDate >= BeginDate)
            {
                DateHeader h = new DateHeader();
                h.Date = CurrentDate;
                h.sDate = Frequency.GetPeriodName(CurrentDate, FrequencyID, true);
                hl.Add(h);

                CurrentDate = Frequency.AddPeriod(CurrentDate, FrequencyID, -1);
            }
            return hl;
        }

        //-------- Bulk Edit List
        public static FrequencyMetric BulkEditList(int ValueCount, int FrequencyID, DateTime BaseDate, Guid? SelMetricID, Guid? SelOrgLocationID, Guid? @CollectorUserId, Guid? @ApproverUserId, bool ViewMode, bool IncludeApprovedValues)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            var fml = (from f in dc.Frequency
                       where f.FrequencyID == FrequencyID
                       select new FrequencyMetric
                       {
                           FrequencyID = f.FrequencyID,
                           Name = f.Name
                       }).FirstOrNull();
            if (fml != null)
            {
                fml.Date = GetDateHeader(FrequencyID, BaseDate, ValueCount);
                List<MetricOrgValue> l = List(ValueCount, BaseDate, FrequencyID, dc, fml.Date, null, SelMetricID, new Guid?[1] { SelOrgLocationID }, null, null, null, @CollectorUserId, ViewMode, SelMetricID != null, @ApproverUserId, IncludeApprovedValues);
                if (SelOrgLocationID != null) // this is temporary fix
                {
                    fml.Metrics = new List<MetricOrgValue>();
                    foreach (MetricOrgValue mov in l)
                        if (mov.OrgLocationID == SelOrgLocationID)
                            fml.Metrics.Add(mov);
                }
                else
                    fml.Metrics = l;
            }

            return fml;
        }
        //----------------------------------------------------------------------------------

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

        public static TimeSpan SqlQueryTime;
        public static TimeSpan TotalTime;

        public static List<FrequencyMetric> List(int ValueCount, DateTime BaseDate, Dictionary<int, DateTime> FrequencyFirstDate, Guid? SelectedMetricID, Guid? SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid? SelPiID, Guid? SelPifID, Guid? SelUserId, bool ViewMode, bool OrderByMetric, Guid? @ApproverUserId)
        {
            DateTime StartTime = DateTime.Now;
            SqlQueryTime = new TimeSpan(0);

            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            var fml = (from f in dc.Frequency
                       orderby f.FrequencyID
                       select new FrequencyMetric
                       {
                           FrequencyID = f.FrequencyID,
                           Name = f.Name
                       }).ToList();

            for (int i = 0; i < fml.Count();i++ )
            {
                var fm = fml[i];
                DateTime FirstDate;
                if (FrequencyFirstDate.Keys.Contains(fm.FrequencyID))
                {
                    FirstDate = FrequencyFirstDate[fm.FrequencyID];
                }
                else
                {
                    FirstDate = Frequency.GetNormalizedDate(fm.FrequencyID, BaseDate);
                    FrequencyFirstDate[fm.FrequencyID] = FirstDate;
                }

                fm.Date = GetDateHeader(fm.FrequencyID, FirstDate, ValueCount);
                fm.Metrics = List(ValueCount, FirstDate, fm.FrequencyID, dc, fm.Date, SelectedMetricID, SelMetricID, SelOrgLocationsID, SelGcaID, SelPiID, SelPifID, SelUserId, ViewMode, OrderByMetric, @ApproverUserId, true);
                if (fm.Metrics.Count < 1)
                {
                    fml.Remove(fm);
                    i--;
                }
            }

            TotalTime = StartTime - DateTime.Now;

            return fml;
        }

        public static List<MetricOrgValue> List(int ValueCount, DateTime BaseDate, int FrequencyID, Guid? SelectedMetricID, Guid? SelMetricID, Guid?[] SelOrgLocationID, Guid? SelGcaID, Guid? SelPiID, Guid? SelPifID, Guid? SelUserId, bool ViewMode, bool OrderByMetric, Guid? @ApproverUserId)
        {
            LinqMicajahDataContext dc  = new LinqMicajahDataContext();
            List<DateHeader> hl = GetDateHeader(FrequencyID, BaseDate, ValueCount);
            return List(ValueCount, BaseDate, FrequencyID, dc, hl, SelectedMetricID, SelMetricID, SelOrgLocationID, SelGcaID, SelPiID, SelPifID, SelUserId, ViewMode, OrderByMetric, @ApproverUserId, true);
        }

        private static void SetOrgLocationNumber(int GroupNumber, List<MetricOrgValue> MetricMetricValues)
        {
            if (GroupNumber > 0) MetricMetricValues[MetricMetricValues.Count - GroupNumber].GroupNumber = GroupNumber;
        }

        public static FrequencyMetric List(DateTime BeginDate, DateTime EndDate, int FrequencyID, Guid? SelectedMetricID, Guid? SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid? SelPiID, Guid? SelPifID, Guid? SelUserId, bool ViewMode, bool OrderByMetric, Guid? @ApproverUserId)
        {
            FrequencyMetric f = new FrequencyMetric();
            f.Date = GetDateHeader(FrequencyID, BeginDate, EndDate);
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                f.Metrics = List(BeginDate, EndDate, FrequencyID, dc, f.Date, SelectedMetricID, SelMetricID, SelOrgLocationsID, SelGcaID, SelPiID, SelPifID, SelUserId, ViewMode, OrderByMetric, @ApproverUserId, true);
            }
            return f;
        }
                
        public static List<MetricOrgValue> List(int ValueCount, DateTime NormalizedDate, int FrequencyID, LinqMicajahDataContext dc, List<DateHeader> hl, Guid? SelectedMetricID, Guid? SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid? SelPiID, Guid? SelPifID, Guid? SelUserId, bool ViewMode, bool OrderByMetric, Guid? @ApproverUserId, bool IncludeApprovedValues)
        {
            DateTime EndDate = Frequency.AddPeriod(NormalizedDate, FrequencyID, 1);
            DateTime BeginDate = Frequency.AddPeriod(EndDate, FrequencyID ,- ValueCount);
            return List(BeginDate, EndDate, FrequencyID, dc, hl, SelectedMetricID, SelMetricID, SelOrgLocationsID, SelGcaID, SelPiID, SelPifID, SelUserId, ViewMode, OrderByMetric, @ApproverUserId, IncludeApprovedValues);
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

        public static List<MetricOrgValue> List(DateTime BeginDate, DateTime EndDate, int FrequencyID, LinqMicajahDataContext dc, List<DateHeader> hl, Guid? SelectedMetricID, Guid? SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid? SelPiID, Guid? SelPifID, Guid? SelUserId, bool ViewMode, bool OrderByMetric, Guid? @ApproverUserId, bool IncludeApprovedValues)
        {
            DateTime StartTime = DateTime.Now;

            string OrgLocations = GetLocationsEncodedString(SelOrgLocationsID, ',');
            ISingleResult<Sp_SelectMetricValuesResult> V = dc.Sp_SelectMetricValues(LinqMicajahDataContext.InstanceId, FrequencyID, EndDate, BeginDate, SelectedMetricID, SelMetricID, null, SelGcaID, SelPiID, SelPifID, SelUserId, OrderByMetric, ViewMode, @ApproverUserId, OrgLocations, IncludeApprovedValues);

            List<MetricOrgValue> MetricMetricValues = new List<MetricOrgValue>();
            MetricOrgValue LastMetricMetricValue = null;
            CompositeDailyValues LastCompositeDailyValues = null;
            int GroupNumber = 0;
            int GroupCount = 0;

            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            bool FirstResult=true;

            foreach (var v in V)
            {
                if (FirstResult)
                {
                    FirstResult = false;
                    SqlQueryTime += DateTime.Now - StartTime;
                }
                
                if (LastMetricMetricValue == null || v.MetricID != LastMetricMetricValue.MetricID || v.OrgLocationID != LastMetricMetricValue.OrgLocationID)
                {
                    if (LastMetricMetricValue == null || v.OrgLocationID == LastMetricMetricValue.OrgLocationID)
                    {
                        GroupNumber++;
                    }
                    else
                    {
                        SetOrgLocationNumber(GroupNumber, MetricMetricValues);
                        GroupNumber = 1;
                        GroupCount++;
                    }

                    AddMetricValue(LastMetricMetricValue, hl, null, LastCompositeDailyValues, FrequencyID, ViewMode);

                    LastMetricMetricValue = new MetricOrgValue();
                    LastMetricMetricValue.GroupCount = GroupCount;

                    //Metric fields
                    LastMetricMetricValue.InstanceId = (Guid)v.InstanceId;
                    LastMetricMetricValue.MetricID = (Guid)v.MetricID;
                    LastMetricMetricValue.Name = v.MetricName;
                    LastMetricMetricValue.FrequencyID = (int)v.MetricFrequencyID;                    
                    LastMetricMetricValue.MetricCategoryID = v.MetricCategoryID;                    
                    LastMetricMetricValue.MetricDataTypeID = (int)v.MetricDataTypeID;
                    LastMetricMetricValue.NODecPlaces = v.NODecPlaces;
                    LastMetricMetricValue.NOMinValue = v.NOMinValue;
                    LastMetricMetricValue.NOMaxValue = v.NOMaxValue;
                    LastMetricMetricValue.MetricTypeID = (int)v.MetricTypeID;
                    LastMetricMetricValue.UnitOfMeasureID = v.MetricUnitOfMeasureID;
                    LastMetricMetricValue.InputUnitOfMeasureID = v.MetricInputUnitOfMeasureID;
                    LastMetricMetricValue.CollectionStartDate = v.CollectionStartDate;
                    LastMetricMetricValue.CollectionEndDate = v.CollectionEndDate;
                    LastMetricMetricValue.AllowCustomNames = (bool)v.AllowCustomNames;

                    //Extend fields                   
                    LastMetricMetricValue.FrequencyName = v.FrequencyName;
                    LastMetricMetricValue.UnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.MetricUnitOfMeasureID);
                    LastMetricMetricValue.InputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.MetricInputUnitOfMeasureID);

                    //MetricOrg fields                    
                    LastMetricMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                    LastMetricMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                    LastMetricMetricValue.CollectorUserID = v.CollectorUserId;
                    LastMetricMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                    LastMetricMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                    LastMetricMetricValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.OrgLocationUnitOfMeasureID);

                    LastMetricMetricValue.RelatedOrgLocationNameRecordID = v.MetricOrgLocationNameID;
                    LastMetricMetricValue.MetricOrgLocationAlias = v.CustomMetricAlias;
                    LastMetricMetricValue.MetricOrgLocationCode = v.CustomMetricCode;

                    // define actual measure unit
                    if (v.MetricOrgLocationUoMID != null)
                        LastMetricMetricValue.InputUnitOfMeasureName = LastMetricMetricValue.OrgLocationUnitOfMeasureName;
                    
                    if (ViewMode)
                    {

                        /*LastMetricMetricValue.GCA =
                            (
                                from pij in dc.PerformanceIndicatorMetricJunc
                                join pi in dc.PerformanceIndicator on new { pij.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { pi.PerformanceIndicatorID, pi.InstanceId, pi.Status }
                                join g in dc.GroupCategoryAspect on new { pi.GroupCategoryAspectID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { GroupCategoryAspectID = (Guid?)g.GroupCategoryAspectID, g.InstanceId, g.Status }
                                join gn in dc.GCAFullNameView on new { g.GroupCategoryAspectID, LinqMicajahDataContext.InstanceId } equals new { gn.GroupCategoryAspectID, gn.InstanceId }

                                join _pifj in dc.PerformanceIndicatorFormPerformanceIndicatorJunc on new { pij.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { _pifj.PerformanceIndicatorID, _pifj.InstanceId, _pifj.Status } into __pifj
                                from pifj in __pifj.DefaultIfEmpty()

                                join _pif in dc.PerformanceIndicatorForm on new { pifj.PerformanceIndicatorFormID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { _pif.PerformanceIndicatorFormID, _pif.InstanceId, _pif.Status } into __pif
                                from pif in __pif.DefaultIfEmpty()

                                join _onj in dc.PIFormOrgLocationJuncView on pif.PerformanceIndicatorFormID equals _onj.PerformanceIndicatorFormID into __onj
                                from onj in __onj.DefaultIfEmpty()

                                join _en in dc.Mc_EntityNode on new { OrgLocationID = onj.OrgLocationID, Deleted = false } equals new { OrgLocationID = _en.EntityNodeId, _en.Deleted } into __en
                                from en in __en.DefaultIfEmpty()

                                where pij.MetricID == LastMetricMetricValue.MetricID && pij.InstanceId == LinqMicajahDataContext.InstanceId && pij.Status == true &&
                                      (LastMetricMetricValue.OrgLocationID == null ? (en.EntityNodeId == null) : (LastMetricMetricValue.OrgLocationID == en.EntityNodeId))

                                select new GroupCategoryAspect.Extend
                                {
                                    GroupCategoryAspectID = g.GroupCategoryAspectID,
                                    InstanceId = g.InstanceId,
                                    Name = g.Name,
                                    ParentId = g.ParentId,

                                    Status = g.Status,
                                    Created = g.Created,
                                    Updated = g.Updated,

                                    FullName = gn.FullName,
                                }
                            ).Distinct().ToList();*/

                        /*LastMetricMetricValue.Pi =
                            (
                                from pij in dc.PerformanceIndicatorMetricJunc
                                join pi in dc.PerformanceIndicator on new { pij.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { pi.PerformanceIndicatorID, pi.InstanceId, pi.Status }
                                join pifj in dc.PerformanceIndicatorFormPerformanceIndicatorJunc on new { pij.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { pifj.PerformanceIndicatorID, pifj.InstanceId, pifj.Status }
                                join pif in dc.PerformanceIndicatorForm on new { pifj.PerformanceIndicatorFormID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { pif.PerformanceIndicatorFormID, pif.InstanceId, pif.Status }

                                join _onj in dc.PIFormOrgLocationJuncView on pif.PerformanceIndicatorFormID equals _onj.PerformanceIndicatorFormID into __onj
                                from onj in __onj.DefaultIfEmpty()

                                join _en in dc.Mc_EntityNode on new { OrgLocationID = onj.OrgLocationID, Deleted = false } equals new { OrgLocationID = _en.EntityNodeId, _en.Deleted } into __en
                                from en in __en.DefaultIfEmpty()

                                where pij.MetricID == LastMetricMetricValue.MetricID && pij.InstanceId == LinqMicajahDataContext.InstanceId && pij.Status == true &&
                                      (LastMetricMetricValue.OrgLocationID == null ? (en.EntityNodeId == null) : (LastMetricMetricValue.OrgLocationID == en.EntityNodeId))

                                select pi
                            ).Distinct().ToList();*/

                        /*LastMetricMetricValue.PiForm =
                            (
                                from pij in dc.PerformanceIndicatorMetricJunc
                                join pi in dc.PerformanceIndicator on new { pij.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { pi.PerformanceIndicatorID, pi.InstanceId, pi.Status }
                                join pifj in dc.PerformanceIndicatorFormPerformanceIndicatorJunc on new { pij.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { pifj.PerformanceIndicatorID, pifj.InstanceId, pifj.Status }
                                join pif in dc.PerformanceIndicatorForm on new { pifj.PerformanceIndicatorFormID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { pif.PerformanceIndicatorFormID, pif.InstanceId, pif.Status }

                                join _onj in dc.PIFormOrgLocationJuncView on pif.PerformanceIndicatorFormID equals _onj.PerformanceIndicatorFormID into __onj
                                from onj in __onj.DefaultIfEmpty()

                                join _en in dc.Mc_EntityNode on new { OrgLocationID = onj.OrgLocationID, Deleted = false } equals new { OrgLocationID = _en.EntityNodeId, _en.Deleted } into __en
                                from en in __en.DefaultIfEmpty()

                                where pij.MetricID == LastMetricMetricValue.MetricID && pij.InstanceId == LinqMicajahDataContext.InstanceId && pij.Status == true &&
                                      (LastMetricMetricValue.OrgLocationID == null ? (en.EntityNodeId == null) : (LastMetricMetricValue.OrgLocationID == en.EntityNodeId))

                                select pif
                            ).Distinct().ToList();*/

                    }


                    var vvv = (from pij in dc.PerformanceIndicatorMetricJunc
                               where pij.MetricID == LastMetricMetricValue.MetricID
                               select pij).ToList();

                    if (SelUserId != null)
                    {
                    }

                    MetricMetricValues.Add(LastMetricMetricValue);
                    LastCompositeDailyValues = null;
                }

                if (v.MetricValueID != null)
                {
                    MetricValue.Extend LastMetricValue = new MetricValue.Extend();
                    // Value fields
                    LastMetricValue.InstanceId = (Guid)v.InstanceId;
                    LastMetricValue.MetricValueID = (Guid)v.MetricValueID;
                    LastMetricValue.MetricID = (Guid)v.MetricID;
                    LastMetricValue.Date = (DateTime)v.Date;
                    LastMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                    LastMetricValue.FrequencyID = (int)v.ValueFrequencyID;
                    LastMetricValue.Value = v.Value;
                    LastMetricValue.Verified = (bool)v.Verified;
                    LastMetricValue.Approved = v.Approved;
                    LastMetricValue.FilesAttached = (bool)v.FilesAttached;
                    LastMetricValue.ReviewUpdated = (bool)v.ReviewUpdated;
                    LastMetricValue.MissedCalc = (bool)v.MissedCalc;
                    LastMetricValue.MetricDataTypeID = (int)v.ValueDataTypeID;
                    LastMetricValue.UnitOfMeasureID = v.ValueUnitOfMeasureID;
                    LastMetricValue.InputUnitOfMeasureID = v.ValueInputUnitOfMeasureID;
                    LastMetricValue.Notes = v.Notes;

                    // Extend fields
                        // extend - value reference
                    LastMetricValue.ValueFrequencyName = v.FrequencyName;
                    LastMetricValue.ValueUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.ValueUnitOfMeasureID);
                    LastMetricValue.ValueInputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.ValueInputUnitOfMeasureID);
                    LastMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                    LastMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                    LastMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                    LastMetricValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.OrgLocationUnitOfMeasureID);

                        // extend - metric fields
                    LastMetricValue.MetricName = v.MetricName;
                    LastMetricValue.MetricFrequencyID = (int)v.MetricFrequencyID;
                    LastMetricValue.ActualMetricDataTypeID = (int)v.MetricDataTypeID;
                    LastMetricValue.MetricCategoryID = v.MetricCategoryID;
                    LastMetricValue.MetricUnitOfMeasureID = v.MetricUnitOfMeasureID;
                    LastMetricValue.MetricInputUnitOfMeasureID = v.MetricInputUnitOfMeasureID;
                    LastMetricValue.NODecPlaces = v.NODecPlaces;
                    LastMetricValue.NOMinValue = v.NOMinValue;
                    LastMetricValue.NOMaxValue = v.NOMaxValue;
                    LastMetricValue.MetricInputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.MetricInputUnitOfMeasureID);
                    LastMetricValue.MetricUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.MetricUnitOfMeasureID);
                    LastMetricValue.IsCalculated = (int)v.MetricTypeID > 1;
                    LastMetricValue.CollectionStartDate = v.CollectionStartDate;
                    LastMetricValue.CollectionEndDate = v.CollectionEndDate;
                    LastMetricValue.CollectionEnabled = (ViewMode || IsCollectingDate(LastMetricValue.CollectionStartDate, LastMetricValue.CollectionEndDate, LastMetricValue.Date));

                    if (LastMetricValue.FrequencyID != LastMetricMetricValue.FrequencyID && SelectedMetricID==null)
                    {
                        if (LastMetricValue.MetricDataTypeID != 1) continue;
                        decimal Val;
                        if (!decimal.TryParse(LastMetricValue.Value, out Val)) continue;

                        if (LastCompositeDailyValues == null) LastCompositeDailyValues = new CompositeDailyValues();
                        //LastCompositeDailyValues.AddCompositeDailyValue((DateTime)v.InputDate, Val, (int)LastMetricValue.FrequencyID, LastMetricValue.Date);
                        
                        continue;
                    }

                    AddMetricValue(LastMetricMetricValue, hl, LastMetricValue, LastCompositeDailyValues, FrequencyID, ViewMode);
                }
            }

            SetOrgLocationNumber(GroupNumber, MetricMetricValues);
            AddMetricValue(LastMetricMetricValue, hl, null, LastCompositeDailyValues, FrequencyID, ViewMode);

            if (!ViewMode)
                for (int i = 0; i < MetricMetricValues.Count; i++)
                {
                    bool AllValuesCollectionDisabled = true;
                    MetricOrgValue mov = MetricMetricValues[i];
                    foreach (MetricValue.Extend me in mov.MetricValues)
                        if (me.CollectionEnabled)
                            AllValuesCollectionDisabled = false;
                    if (AllValuesCollectionDisabled)
                        MetricMetricValues.Remove(mov);
                }
            return MetricMetricValues;
        }

        //----------------------------------------------------------------------------------
        //- Alert Queue Select

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
                                    int OldMetricOrgValueIndex = fm.Metrics.FindIndex(n=>(n.FrequencyID == dmo.FrequencyID && n.MetricID == dmo.MetricID && n.OrgLocationID == dmo.OrgLocationID));
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
        //----------------------------------------------------------------------------------

        
        //- Missed Values Select
        public static List<DistinctFrequencyMetric> MissedQueueList(DateTime BaseDate, Dictionary<FrequencyMetricOrgLocationID, DateTime> FrequencyFirstDate, Guid? SelUserId, bool OrderByMetric)
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
                fm.Metrics = MissedQueueList(dc, FirstDate, FirstDate, fm.FrequencyID, null, null, SelUserId, OrderByMetric);

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
                            List<DistinctMetricOrgValue> r = MissedQueueList(dc, FirstDate, FrequencyFirstDate[fmo], fm.FrequencyID, fmo.MetricID, fmo.OrgLocationID, SelUserId, OrderByMetric);
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

        public static List<DistinctMetricOrgValue> MissedQueueList(LinqMicajahDataContext dc, DateTime BaseDate, DateTime NormalizedDate, int FrequencyID, Guid? MetricID, Guid? OrgLocationID, Guid? SelUserId, bool OrderByMetric)
        {
            DateTime EndDate = Frequency.AddPeriod(NormalizedDate, FrequencyID, 1);
            ISingleResult<Sp_SelectMissedInputMetricValuesResult> V = dc.Sp_SelectMissedInputMetricValues(LinqMicajahDataContext.InstanceId, BaseDate, EndDate, FrequencyID, MetricID, OrgLocationID, SelUserId, OrderByMetric);

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
                    LastMetricMetricValue.InstanceId = LinqMicajahDataContext.InstanceId;
                    LastMetricMetricValue.MetricID = (Guid)v.MetricID;
                    LastMetricMetricValue.Name = v.MetricName;
                    LastMetricMetricValue.FrequencyID = FrequencyID;
                    LastMetricMetricValue.MetricTypeID = 1;
                    LastMetricMetricValue.MetricDataTypeID = 1;
                    LastMetricMetricValue.NODecPlaces = v.NODecPlaces;
                    LastMetricMetricValue.InputUnitOfMeasureID = v.MetricInputUnitOfMeasureID;
                    LastMetricMetricValue.InputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.MetricInputUnitOfMeasureID);
                    LastMetricMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                    LastMetricMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                    LastMetricMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                    LastMetricMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                    LastMetricMetricValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, v.OrgLocationUnitOfMeasureID);
                    LastMetricMetricValue.CollectionStartDate = v.CollectionStartDate;
                    LastMetricMetricValue.CollectionEndDate = v.CollectionEndDate;
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
                LastMetricValue.InstanceId = LinqMicajahDataContext.InstanceId;
                LastMetricValue.MetricID = (Guid)v.MetricID;
                LastMetricValue.NODecPlaces = v.NODecPlaces;
                // value data
                LastMetricValue.MetricValueID = (Guid)v.MetricValueID;
                LastMetricValue.FrequencyID = FrequencyID;
                LastMetricValue.Date = (DateTime)v.Date;
                LastMetricValue.Value = v.Value;
                LastMetricValue.Approved = v.Approved;
                LastMetricValue.FilesAttached = (bool)v.FilesAttached;
                LastMetricValue.ReviewUpdated = (bool)v.ReviewUpdated;
                LastMetricValue.OrgLocationID = (Guid)v.OrgLocationID;
                LastMetricValue.MetricDataTypeID = 1;
                LastMetricValue.InputUnitOfMeasureID = v.ValueInputUnitOfMeasureID;
                LastMetricValue.OrgLocationFullName = v.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : v.OrgLocationFullName;
                LastMetricValue.RelatedOrgLocationUoMRecordID = v.MetricOrgLocationUoMID;
                LastMetricValue.OrgLocationUnitOfMeasureID = v.OrgLocationUnitOfMeasureID;
                LastMetricValue.IsCalculated = false;

                LastMetricValue.CollectionStartDate = v.CollectionStartDate;
                LastMetricValue.CollectionEndDate = v.CollectionEndDate;
                LastMetricValue.CollectionEnabled = IsCollectingDate(LastMetricValue.CollectionStartDate, LastMetricValue.CollectionEndDate, LastMetricValue.Date);
                
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

                        AnotherMetricValue.CollectionStartDate = LastMetricMetricValue.CollectionStartDate;
                        AnotherMetricValue.CollectionEndDate = LastMetricMetricValue.CollectionEndDate;
                        AnotherMetricValue.CollectionEnabled = false;//IsCollectingDate(AnotherMetricValue.CollectionStartDate, AnotherMetricValue.CollectionEndDate, AnotherMetricValue.Date);
                
                        DateHeader ah = new DateHeader();
                        ah.Date = DateTime.MinValue;
                        ah.sDate = "&nbsp;";
                        m.DatesHeader.Add(ah);
                        m.MetricValues.Add(AnotherMetricValue);
                    }
                }
            }

            for (int k = 0; k < MetricMetricValues.Count; k++)
            {
                bool AllValuesCollectionDisabled = true;
                DistinctMetricOrgValue mov = MetricMetricValues[k];
                foreach (MetricValue.Extend me in mov.MetricValues)
                    if (me.CollectionEnabled)
                        AllValuesCollectionDisabled = false;
                if (AllValuesCollectionDisabled && !mov.IsNextValues && !mov.IsPreviousValues)
                    MetricMetricValues.Remove(mov);
            }
            return MetricMetricValues;
        }

        //----------------------------------------------------------------------------------
        

        // Related Input Values
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
        //------------------------------------------------------

        //- Work List Select        
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
        //----------------------------------------------------------------------------------

        public static Extend Get(Guid MetricID, DateTime NormalizedDate, Guid OrgLocationID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            var MetricValue =
                from m in dc.Metric
                join f in dc.Frequency on m.FrequencyID equals f.FrequencyID
                join t in dc.MetricDataType on m.MetricDataTypeID equals t.MetricDataTypeID

                join _mv in dc.MetricValue on
                    new
                    {
                        m.MetricID,
                        LinqMicajahDataContext.InstanceId,
                        m.FrequencyID,
                        Date = NormalizedDate,
                        Status = (bool?)true,
                        OrgLocationID
                    }
                    equals new
                    {
                        _mv.MetricID,
                        _mv.InstanceId,
                        _mv.FrequencyID,
                        _mv.Date,
                        _mv.Status,
                        _mv.OrgLocationID
                    } into __mv
                join _c in dc.MetricCategoryFullNameView on
                    new { m.InstanceId, m.MetricCategoryID } equals
                    new { _c.InstanceId, MetricCategoryID = (Guid?)_c.MetricCategoryID } into __c
                from mv in __mv.DefaultIfEmpty()
                join _mvf in dc.Frequency on mv.FrequencyID equals _mvf.FrequencyID into __mvf
                join _mvt in dc.MetricDataType on mv.MetricDataTypeID equals _mvt.MetricDataTypeID into __mvt
                join _OrgLocName in dc.EntityNodeFullNameView on
                    new { IsNullInstance = true, OrgLocationID } equals new { IsNullInstance = _OrgLocName.InstanceId == null, OrgLocationID = _OrgLocName.EntityNodeId } into __OrgLocName
                from OrgLocName in __OrgLocName.DefaultIfEmpty()
                from mvf in __mvf.DefaultIfEmpty()
                from mvt in __mvt.DefaultIfEmpty()
                from c in __c.DefaultIfEmpty()
                join _mnuom in dc.MetricOrgLocationUoM on
                    new { m.InstanceId, m.MetricID, OrgLocationID }
                    equals new { _mnuom.InstanceId, _mnuom.MetricID, _mnuom.OrgLocationID } into __mnuom
                from mnuom in __mnuom.DefaultIfEmpty()
                join _mnname in dc.MetricOrgLocationName on
                    new { m.InstanceId, m.MetricID, OrgLocationID }
                    equals new { _mnname.InstanceId, _mnname.MetricID, _mnname.OrgLocationID } into __mnname
                from mnname in __mnname.DefaultIfEmpty()

                where
                        m.MetricID == MetricID &&
                        m.InstanceId == LinqMicajahDataContext.InstanceId &&
                        m.Status == true

                select new MetricValue.Extend
                {
                    // Metric solid fields
                    InstanceId = m.InstanceId,
                    MetricID = m.MetricID,

                    // Value fields                    
                    MetricValueID = mv.MetricValueID == null ? Guid.Empty : mv.MetricValueID,
                    OrgLocationID = mv.OrgLocationID == null ? OrgLocationID : mv.OrgLocationID,
                    InputUserId = mv.InputUserId,
                    ApproveUserId = mv.ApproveUserId,
                    FrequencyID = mv.FrequencyID == null ? m.FrequencyID : mv.FrequencyID,
                    Date = mv.Date == null ? FailureDate : mv.Date,
                    MetricDataTypeID = mv.MetricDataTypeID == null ? m.MetricDataTypeID : mv.MetricDataTypeID,
                    Value = mv.Value,
                    ConvertedValue = mv.ConvertedValue,
                    Notes = mv.Notes,
                    Verified = mv.Verified == null ? false : mv.Verified,
                    Approved = mv.MetricValueID == null ? false : mv.Approved,
                    FilesAttached = mv.FilesAttached == null ? false : mv.FilesAttached,
                    ReviewUpdated = mv.ReviewUpdated == null ? false : mv.ReviewUpdated,
                    InputUnitOfMeasureID = mv.InputUnitOfMeasureID,
                    UnitOfMeasureID = mv.UnitOfMeasureID,

                    // Value Reference fields
                    ValueFrequencyName = mvf.Name,
                    ValueDataTypeName = mvt.Name,
                    ApprovalStatus = mv.MetricValueID == null ? "Pending" : (mv.Approved == null ? "Under Review" : ((bool)mv.Approved ? "Approved" : "Pending")),
                    Period = Frequency.GetPeriodName(NormalizedDate, m.FrequencyID),
                    ValueInputUnitOfMeasureName = Metric.GetMeasureUnitName(OrgUoMs, mv.InputUnitOfMeasureID),
                    ValueUnitOfMeasureName = Metric.GetMeasureUnitName(OrgUoMs, mv.UnitOfMeasureID),
                    RelatedOrgLocationUoMRecordID = mnuom.MetricOrgLocationUoMID,
                    OrgLocationUnitOfMeasureID = mnuom.InputUnitOfMeasureID,
                    OrgLocationUnitOfMeasureName = Metric.GetMeasureUnitName(OrgUoMs, mnuom.InputUnitOfMeasureID),
                    OrgLocationFullName = OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : OrgLocName.FullName,

                    RelatedOrgLocationNameRecordID = mnname.MetricOrgLocationNameID,
                    MetricOrgLocationAlias = mnname.Alias,
                    MetricOrgLocationCode = mnname.Code,

                    // Metric fields
                    MetricName = m.Name,
                    MetricFrequencyID = m.FrequencyID,
                    ActualMetricDataTypeID = m.MetricDataTypeID,
                    MetricCategoryID = m.MetricCategoryID,
                    MetricInputUnitOfMeasureID = m.InputUnitOfMeasureID,
                    MetricUnitOfMeasureID = m.UnitOfMeasureID,
                    NODecPlaces = m.NODecPlaces,
                    NOMinValue = m.NOMinValue,
                    NOMaxValue = m.NOMaxValue,
                    FormulaCode = m.FormulaCode,
                    Variable = m.Variable,
                    Documentation = m.Documentation,
                    Description = m.Notes,
                    Definition = m.Definition,
                    References = m.MetricReferences,
                    AllowMetricCustomNames = m.AllowCustomNames,
                    
                    // Metric reference fields
                    MetricFrequencyName = f.Name,
                    MetricCategoryName = c.FullName,
                    MetricDataTypeName = t.Name,
                    MetricInputUnitOfMeasureName = Metric.GetMeasureUnitName(OrgUoMs, m.InputUnitOfMeasureID),
                    MetricUnitOfMeasureName = Metric.GetMeasureUnitName(OrgUoMs, m.UnitOfMeasureID),
                    IsCalculated = m.MetricTypeID == 2
                };
            return MetricValue.FirstOrNull();
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

        public static Guid InsertOrUpdate(Guid MetricID, DateTime Date, Guid OrgLocationID, bool IsFilesAttached, bool Approve, Guid? SelectedUoMID, string OldValue, string Value, bool? OldApproved, bool? Approved, Guid? UserId, string Notes, string CustomMetricAlias, string CustomMetricCode)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                Metric metric =
                    (from m in dc.Metric
                     where
                         m.MetricID == MetricID &&
                         m.InstanceId == LinqMicajahDataContext.InstanceId &&
                         m.Status == true
                     select m).FirstOrNull();
                Guid _ActualValueID = Guid.Empty;
                if (metric != null)
                {
                    MetricValue metricValue =
                        (from mv in dc.MetricValue
                         where
                             mv.MetricID == MetricID &&
                             mv.InstanceId == LinqMicajahDataContext.InstanceId &&
                             mv.Status == true &&
                             mv.Date == Date &&
                             mv.FrequencyID == metric.FrequencyID &&
                             mv.OrgLocationID == OrgLocationID
                         select mv).FirstOrNull();
                    MetricOrgLocationUoM MOUoM =
                        (from muom in dc.MetricOrgLocationUoM
                         where
                             muom.MetricID == MetricID &&
                             muom.InstanceId == LinqMicajahDataContext.InstanceId &&
                             muom.OrgLocationID == OrgLocationID
                         select muom).FirstOrNull();
                    if (MOUoM == null)
                    {
                        Bll.MetricOrgLocationUoM muom = new Bll.MetricOrgLocationUoM();
                        muom.MetricID = MetricID;
                        muom.OrgLocationID = OrgLocationID;
                        muom.InputUnitOfMeasureID = SelectedUoMID;
                        dc.MetricOrgLocationUoM.InsertOnSubmit(muom);
                    }
                    else                    
                        if (SelectedUoMID != MOUoM.InputUnitOfMeasureID)
                            MOUoM.InputUnitOfMeasureID = SelectedUoMID; // change org location uom

                    if (CustomMetricAlias != null && CustomMetricCode != null)
                    {
                        MetricOrgLocationName MOName =
                            (from mname in dc.MetricOrgLocationName
                             where
                                 mname.MetricID == MetricID &&
                                 mname.InstanceId == LinqMicajahDataContext.InstanceId &&
                                 mname.OrgLocationID == OrgLocationID
                             select mname).FirstOrNull();
                        if (MOName == null)
                        {
                            Bll.MetricOrgLocationName moname = new Bll.MetricOrgLocationName();
                            moname.MetricID = MetricID;
                            moname.OrgLocationID = OrgLocationID;
                            moname.Alias = CustomMetricAlias;
                            moname.Code = CustomMetricCode;
                            dc.MetricOrgLocationName.InsertOnSubmit(moname);
                        }
                        else
                        { // update org location specific metric names
                            MOName.Alias = CustomMetricAlias;
                            MOName.Code = CustomMetricCode;
                        }
                    }
                    dc.SubmitChanges();

                    string ConvertedValue = Value;
                    if (metric.UnitOfMeasureID != SelectedUoMID && metric.MetricDataTypeID == 1 && SelectedUoMID != null && metric.UnitOfMeasureID != null)
                        ConvertedValue = Mc_UnitsOfMeasure.ConvertValue(Value, (Guid)SelectedUoMID, (Guid)metric.UnitOfMeasureID);

                    if (metricValue == null)
                    {
                        Bll.MetricValue mv = new Bll.MetricValue();
                        mv.MetricValueID = Guid.NewGuid();
                        mv.MetricID = MetricID;
                        mv.FrequencyID = metric.FrequencyID;
                        mv.Date = Date;
                        mv.OrgLocationID = OrgLocationID;
                        mv.InputUnitOfMeasureID = SelectedUoMID;
                        mv.UnitOfMeasureID = metric.UnitOfMeasureID;
                        mv.MetricDataTypeID = metric.MetricDataTypeID;
                        mv.Value = Value;
                        mv.ConvertedValue = ConvertedValue;
                        mv.InputUserId = UserId;
                        mv.Approved = false;
                        mv.ReviewUpdated = false;
                        mv.ApproveUserId = null;
                        mv.Notes = Notes;
                        mv.FilesAttached = IsFilesAttached;
                        mv.IsCalc = true;
                        mv.InProcess = false;                        
                        dc.MetricValue.InsertOnSubmit(mv);
                        dc.SubmitChanges();
                        _ActualValueID = mv.MetricValueID;
                    }
                    else
                    {
                        metricValue.MetricDataTypeID = metric.MetricDataTypeID;
                        metricValue.InputUnitOfMeasureID = SelectedUoMID;
                        metricValue.UnitOfMeasureID = metric.UnitOfMeasureID;
                        metricValue.Value = Value;
                        metricValue.ConvertedValue = ConvertedValue;
                        metricValue.Approved = Approved;
                        metricValue.ReviewUpdated = (Approve) ? false : (OldApproved == null && Approved == null);
                        if (Approve)
                        {
                            if (OldApproved != Approved)
                                metricValue.ApproveUserId = UserId;
                            if (OldValue != Value)
                                metricValue.InputUserId = UserId;
                        }
                        else                        
                            metricValue.InputUserId = UserId;
                        metricValue.Notes = Notes;
                        metricValue.FilesAttached = IsFilesAttached;
                        metricValue.IsCalc = true;                        
                        dc.SubmitChanges();
                        _ActualValueID = metricValue.MetricValueID;
                    }                    
                }
                return _ActualValueID;
            }
        }

        public static void MakeAllInputsDirty()
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                IEnumerable<MetricValue> InputMetricValue =
                    (from m in dc.Metric
                     join mr in dc.MetricRelation
                        on new { m.InstanceId, m.MetricID } equals new { mr.InstanceId, mr.MetricID }
                    join mv in dc.MetricValue
                        on new { m.InstanceId, m.MetricID } equals new { mv.InstanceId, mv.MetricID }                    
                    where
                        m.MetricTypeID == 1
                        &&
                        m.MetricDataTypeID == 1
                        &&
                        mv.Status == true
                    select mv).Distinct();
                foreach (MetricValue mv in InputMetricValue)
                {
                    mv.IsCalc = true;
                    mv.InProcess = false;
                }
                try
                {
                    dc.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (ChangeConflictException)
                {
                    foreach (ObjectChangeConflict conflict in dc.ChangeConflicts)
                        foreach (MemberChangeConflict memberConflict in conflict.MemberConflicts)
                            if (memberConflict.Member.Name.Equals("IsCalc") || memberConflict.Member.Name.Equals("InProcess"))
                                memberConflict.Resolve(RefreshMode.KeepCurrentValues);
                            else
                                memberConflict.Resolve(RefreshMode.OverwriteCurrentValues);
                    dc.SubmitChanges(ConflictMode.ContinueOnConflict);
                }              
            }
        }

        public static void MakeFormulaRelatedInputsDirty(Guid OldMetricFormulaID, Guid NewFormulaID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {                
                List<MetricValue> InputMetricValue =
                    (from m in dc.Metric
                     join mr in dc.MetricRelation
                        on new { m.InstanceId, m.MetricID } equals new { mr.InstanceId, mr.MetricID }
                     join mv in dc.MetricValue
                         on new { m.InstanceId, m.MetricID } equals new { mv.InstanceId, mv.MetricID }
                     where
                         m.MetricTypeID == 1
                         &&
                         m.MetricDataTypeID == 1
                         &&
                         mv.Status == true
                         &&
                         (mr.MetricFormulaID == OldMetricFormulaID || mr.MetricFormulaID == NewFormulaID)
                     select mv).ToList();
                foreach (MetricValue mv in InputMetricValue)
                {
                    mv.IsCalc = true;
                    mv.InProcess = false;
                }
                try
                {
                    dc.SubmitChanges(ConflictMode.ContinueOnConflict);
                }
                catch (ChangeConflictException)
                {
                    foreach (ObjectChangeConflict conflict in dc.ChangeConflicts)
                        foreach (MemberChangeConflict memberConflict in conflict.MemberConflicts)
                            if (memberConflict.Member.Name.Equals("IsCalc") || memberConflict.Member.Name.Equals("InProcess"))
                                memberConflict.Resolve(RefreshMode.KeepCurrentValues);
                            else
                                memberConflict.Resolve(RefreshMode.OverwriteCurrentValues);
                    dc.SubmitChanges(ConflictMode.ContinueOnConflict);
                }     
            }
        }

        public static readonly DateTime FailureDate = new DateTime(1900, 1, 1);

        public static List<MetricValue.Extend> MetricValuesForCalculation(int ActGen, out List<Bll.MetricValue.Extend> _OutputValues)
        { 
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            //http://omaralzabir.com/linq_to_sql_solve_transaction_deadlock_and_query_timeout_problem_using_uncommitted_reads/
            // WITH (NOLOCK) 
            int OldTimeOut = dc.CommandTimeout;
            dc.CommandTimeout = 600;

            IQueryable<MetricValue> metricValue =
            from mv in dc.MetricValue
            join m in dc.Metric on
                new { mv.InstanceId, mv.MetricID } equals
                new { m.InstanceId, m.MetricID }
            where
                mv.Status == true &&
                mv.IsCalc == true &&
                m.Generation == (ActGen-1)
            select mv;

            foreach (MetricValue mv in metricValue)
            {
                mv.InProcess = true;
                mv.IsCalc = false;
            }
            dc.SubmitChanges();
           
            IMultipleResults results = dc.Sp_SelectOutputMetricValues(ActGen);
            _OutputValues = results.GetResult<Sp_SelectOutputMetricValuesResult1>()
                .Select(r => new Extend {
                            // metric
                            InstanceId = (Guid)r.InstanceId,
                            MetricID = (Guid)r.MetricID,
                            FrequencyID = (int)r.FrequencyID,
                            MetricInputUnitOfMeasureID = r.MetricInputUnitOfMeasureID,
                            MetricUnitOfMeasureID = r.MetricUnitOfMeasureID,
                            RelatedFormulaID = r.MetricFormulaID,
                            VariableFormula = r.VariableFormula,
                            // value
                            MetricValueID = (Guid)r.MetricValueID,
                            OrgLocationID = (Guid)r.OrgLocationID,
                            Date = (DateTime)r.Date
                        })//.OrderBy(r=>r.Date)
                        .ToList();           
             
            List<MetricValue.Extend> _InputValues = results.GetResult<Sp_SelectOutputMetricValuesResult2>()
                .Select(r => new Extend {
                            // metric
                            MetricID = (Guid)r.MetricID,
                            Variable = r.Variable,
                            MetricInputUnitOfMeasureID = r.MetricInputUnitOfMeasureID,
                            MetricUnitOfMeasureID = r.MetricUnitOfMeasureID,
                            RelatedFormulaID = r.MetricFormulaID,
                            // value
                            MetricValueID = (Guid)r.MetricValueID,                            
                            OrgLocationID = (Guid)r.OrgLocationID,
                            Date = (DateTime)r.Date,
                            Value = r.Value,
                            MissedCalc = (bool)r.MissedCalc,
                            ConvertedValue = r.ConvertedValue,
                            InputUnitOfMeasureID = r.ValueInputUnitOfMeasureID,
                            UnitOfMeasureID = r.ValueUnitOfMeasureID,
                            RelatedOrgLocationUoMRecordID = r.MetricOrgLocationUoMID,
                            OrgLocationUnitOfMeasureID = r.OrgLocationUnitOfMeasureID
                })//.OrderBy(r => r.Date) //???
                .ToList();
            dc.CommandTimeout = OldTimeOut;            
            return _InputValues;
            //=========================
            /*Guid? _CurMetricID = null;
            Guid? _CurOrgLocationID = null;
            bool start = true;
            foreach (MetricValue.Extend me in _OutputValues)
            {
                if (start)
                {
                    _CurMetricID = me.MetricID;
                    _CurOrgLocationID = me.OrgLocationID;
                }
                me.UnitOfMeasureID = me.MetricUnitOfMeasureID;
                Guid? DefUoM = me.RelatedOrgLocationUoMRecordID == null ? me.MetricInputUnitOfMeasureID : me.OrgLocationUnitOfMeasureID;
                List<MetricValue.Extend> _RelatedInputValues = _InputValues.FindAll(r => (r.RelatedFormulaID == me.RelatedFormulaID) && (r.Date == me.Date) && (r.OrgLocationID == me.OrgLocationID));
                if (_RelatedInputValues.Count == 0)
                    me.InputUnitOfMeasureID = DefUoM;
                bool IsSameInputUoMs = true;
                bool IsSameOutputUoMs = true;
                for (int i = 0; i < _RelatedInputValues.Count - 1; i++)
                {
                    Guid? CurInputUoM = _RelatedInputValues[i].InputUnitOfMeasureID;
                    Guid? CurOutputUoM = _RelatedInputValues[i].UnitOfMeasureID;
                    for (int j = i + 1; j < _RelatedInputValues.Count; j++)
                    {
                        if (CurInputUoM != _RelatedInputValues[j].InputUnitOfMeasureID)
                            IsSameInputUoMs = false;
                        if (CurOutputUoM != _RelatedInputValues[j].UnitOfMeasureID)
                            IsSameOutputUoMs = false;
                    }
                }                
                if (IsSameInputUoMs)
                    me.InputUnitOfMeasureID = _RelatedInputValues[0].InputUnitOfMeasureID;
                else
                    if (IsSameOutputUoMs)
                        me.InputUnitOfMeasureID = _RelatedInputValues[0].UnitOfMeasureID;
                    else
                        me.InputUnitOfMeasureID = DefUoM;

                if ((_CurMetricID != me.MetricID) || (_CurOrgLocationID != me.OrgLocationID) || start)
                {
                    UpdateMetricOrgLocationUoM(me.InstanceId, me.MetricID, me.OrgLocationID, me.InputUnitOfMeasureID);
                    _CurMetricID = me.MetricID;
                    _CurOrgLocationID = me.OrgLocationID;
                }     
                start = false;
            }            */            
        }

        /*private static void UpdateMetricOrgLocationUoM(Guid InstanceId, Guid MetricID, Guid OrgLocationID, Guid? InputUnitOfMeasureID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                MetricOrgLocationUoM monum =
                (from mon in dc.MetricOrgLocationUoM                
                where
                    mon.InstanceId == InstanceId &&
                    mon.MetricID == MetricID &&
                    mon.OrgLocationID == OrgLocationID
                select mon).FirstOrNull();

                if (monum != null)
                    monum.InputUnitOfMeasureID = InputUnitOfMeasureID;
                else
                {
                    Bll.MetricOrgLocationUoM muom = new Bll.MetricOrgLocationUoM();
                    muom.InstanceId = InstanceId;
                    muom.MetricID = MetricID;
                    muom.OrgLocationID = OrgLocationID;
                    muom.InputUnitOfMeasureID = InputUnitOfMeasureID;
                    dc.MetricOrgLocationUoM.InsertOnSubmit(muom);
                }
                dc.SubmitChanges();
            }
        }*/

        public static void SaveCalcValues(List<Extend> _InputMetricValues, List<Extend> _CalcMetricValues)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {                
                foreach (MetricValue.Extend mve in _CalcMetricValues)
                {
                    if (mve.MetricValueID == Guid.Empty && String.IsNullOrEmpty(mve.Value))
                        continue; // don't save uncalced values
                    mve.ConvertedValue = mve.Value;
                    if (mve.InputUnitOfMeasureID != mve.UnitOfMeasureID && mve.InputUnitOfMeasureID != null && mve.UnitOfMeasureID != null)
                        Mc_UnitsOfMeasure.ConvertValue(mve.Value, (Guid)mve.InputUnitOfMeasureID, (Guid)mve.UnitOfMeasureID);
                    MetricValue metricValue =
                        (from mv in dc.MetricValue
                         where
                             mv.MetricID == mve.MetricID &&
                             mv.InstanceId == mve.InstanceId &&
                             mv.Status == true &&
                             mv.Date == mve.Date &&
                             mv.FrequencyID == mve.FrequencyID &&
                             mv.OrgLocationID == mve.OrgLocationID
                         select mv).FirstOrNull();
                    if (metricValue == null)
                    { // insert
                        Bll.MetricValue mv = new Bll.MetricValue();
                        mv.InstanceId = mve.InstanceId;
                        mv.MetricID = mve.MetricID;
                        mv.FrequencyID = mve.FrequencyID;
                        mv.Date = mve.Date;                        
                        mv.InputUnitOfMeasureID = mve.InputUnitOfMeasureID;
                        mv.UnitOfMeasureID = mve.UnitOfMeasureID;
                        mv.MetricDataTypeID = 1;
                        mv.Value = mve.Value;
                        mv.ConvertedValue = mve.ConvertedValue;
                        mv.Notes = "Calculated";
                        mv.FilesAttached = false;
                        mv.IsCalc = true;
                        mv.InProcess = false;
                        mv.OrgLocationID = mve.OrgLocationID;
                        mv.Approved = false;
                        mv.InputUserId = null;
                        mv.MissedCalc = mve.MissedCalc;
                        dc.MetricValue.InsertOnSubmit(mv);                        
                    }
                    else
                    { // update                        
                        metricValue.MetricDataTypeID = 1;
                        metricValue.InputUnitOfMeasureID = mve.InputUnitOfMeasureID;
                        metricValue.UnitOfMeasureID = mve.UnitOfMeasureID;                        
                        metricValue.Value = mve.Value;
                        metricValue.ConvertedValue = mve.ConvertedValue;
                        metricValue.Notes = "Calculated";
                        metricValue.FilesAttached = false;
                        metricValue.IsCalc = true;
                        metricValue.OrgLocationID = mve.OrgLocationID;
                        metricValue.MissedCalc = mve.MissedCalc;
                        metricValue.InProcess = false;
                    }
                    dc.SubmitChanges();
                }
            }
        }

        public static void ClearInputValues(int maxgen)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                IQueryable<MetricValue> metricValue =
                from mv in dc.MetricValue
                join m in dc.Metric on
                    new { mv.InstanceId, mv.MetricID } equals
                    new { m.InstanceId, m.MetricID }
                where
                    mv.Status == true &&
                    mv.IsCalc == true &&
                    m.Generation == maxgen
                select mv;

                foreach (MetricValue mv in metricValue)
                {
                    mv.InProcess = true;
                    mv.IsCalc = false;
                }
                dc.SubmitChanges();

                IQueryable<MetricValue> _metricValue =
                from mv in dc.MetricValue                
                where                    
                    mv.InProcess == true
                select mv;

                foreach (MetricValue mv in _metricValue)                
                    mv.InProcess = false;
                dc.SubmitChanges();
            }
        }

        public static string GetMeasureUnitAbbvr(List<Micajah.Common.Bll.MeasureUnit> OrgUoMs, Guid? MeasureUnitID)
        {
            Micajah.Common.Bll.MeasureUnit mu = OrgUoMs.Find(u => u.MeasureUnitId == MeasureUnitID);
            return mu == null ? String.Empty : mu.SingularAbbreviation;
        }

        public static MetricOrgValue List(int ValueCount, DateTime BaseDate, Guid ScoreCardMetricID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var r = from cm in dc.ScoreCardMetric
                    join m in dc.Metric on new { LinqMicajahDataContext.InstanceId, cm.MetricID, Status=(bool?)true } equals new { m.InstanceId, m.MetricID, m.Status }
                    join o in dc.EntityNodeFullNameView on cm.OrgLocationID equals o.EntityNodeId
                    join _u in dc.MetricOrgLocationUoM on new { LinqMicajahDataContext.InstanceId, cm.MetricID, cm.OrgLocationID } equals new { _u.InstanceId, _u.MetricID, OrgLocationID=(Guid?)_u.OrgLocationID } into __u

                    from u in __u.DefaultIfEmpty()

                    where cm.InstanceId == LinqMicajahDataContext.InstanceId && cm.ScoreCardMetricID == ScoreCardMetricID && cm.Status==true
                    select new MetricOrgValue
                    {
                        Name = m.Name,
                        OrgLocationFullName = o.EntityNodeId == Guid.Empty ? LinqMicajahDataContext.OrganizationName : o.FullName,
                        MetricID = m.MetricID,
                        OrgLocationID = o.EntityNodeId,
                        FrequencyID = m.FrequencyID,
                        InputUnitOfMeasureID = u.MetricOrgLocationUoMID == null ? m.InputUnitOfMeasureID : u.InputUnitOfMeasureID,//u.InputUnitOfMeasureID == null ? m.InputUnitOfMeasureID : u.InputUnitOfMeasureID,
                        NODecPlaces = m.NODecPlaces
                    };
            var mo = r.FirstOrNull();
            if (mo == null) return null;

            DateTime EndDate = Frequency.GetNormalizedDate(mo.FrequencyID, BaseDate);
            

            ScoreCardMetric.Extend e = new ScoreCardMetric.Extend()
            {
                InstanceId = LinqMicajahDataContext.InstanceId,
                ScoreCardMetricID = ScoreCardMetricID,
                MetricID = mo.MetricID,
                OrgLocationID = mo.OrgLocationID,
                ScoreCardPeriodID = 1,
                MetricFrequencyID = mo.FrequencyID
            };
            
            mo.MetricValues = new List<Extend>();
            for (int i = ValueCount-1; i >= 0; i--)
            {
                DateTime dt = Frequency.AddPeriod(EndDate, mo.FrequencyID, -i);
                Guid? TotalUnitOfMessureID;
                double? CurrentValue;
                double? PreviousValue;
                ScoreCardMetric.Calculate(dc, e.MetricID, e.OrgLocationID, e.UomID, e.ScoreCardPeriodID, e.MetricFrequencyID, dt, LinqMicajahDataContext.OrganizationId, LinqMicajahDataContext.InstanceId, out TotalUnitOfMessureID, out CurrentValue, out PreviousValue);
                e.CurrentValue = CurrentValue;
                e.PreviousValue = PreviousValue;

                Extend mv = new Extend()
                {
                    InstanceId = mo.InstanceId,
                    MetricID = mo.MetricID,
                    FrequencyID = mo.FrequencyID,
                    Date = dt,
                    UnitOfMeasureID = TotalUnitOfMessureID,
                    InputUnitOfMeasureID = TotalUnitOfMessureID,
                    MetricDataTypeID = 1,
                    DValue = e.CurrentValue,
                    OrgLocationID = mo.OrgLocationID,
                    Period = Frequency.GetPeriodName(dt, mo.FrequencyID, true)
                };

                mo.MetricValues.Add(mv);
                mo.UnitOfMeasureID = TotalUnitOfMessureID;
            }
            return mo;
        }

        //********** static methods *********
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
