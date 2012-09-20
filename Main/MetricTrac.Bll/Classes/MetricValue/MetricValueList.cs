using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class MetricValue
    {
        [Serializable]
        public class ShiftDate
        {
            public int FrequencyID { get; set; }
            public Guid EntityID { get; set; }
            public DateTime StartDate { get; set; }
        }

        // ========= Experimental Metric Input List Select  =========
        public static List<FrequencyMetric> exList(int ValueCount, DateTime BaseDate, /*Dictionary<Guid, DateTime>*/List<ShiftDate> StartDates, Guid?[] SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid?[] SelPiID, bool ViewMode, bool OrderByMetric, Guid? CollectorId, Guid? ApproverId)
        {
            bool IncludeApprovedValues = true; // for bulk approve page = false
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            string OrgLocations = GetLocationsEncodedString(SelOrgLocationsID, ',');
            string PerformanceIndicators = GetLocationsEncodedString(SelPiID, ',');
            string Metrics = GetLocationsEncodedString(SelMetricID, ',');
            string DateFilter = String.Empty;
            if (StartDates != null)
                foreach (ShiftDate d in StartDates)
                    DateFilter += "<X><Entity>" + d.EntityID.ToString() + "</Entity><FrequencyID>" + d.FrequencyID + "</FrequencyID><EndDate>" + /*d.StartDate*/Frequency.AddPeriod(d.StartDate, d.FrequencyID, 1).ToShortDateString() + "</EndDate></X>";
            ISingleResult<Sp_exSelectMetricValuesResult> SV = dc.Sp_exSelectMetricValues(LinqMicajahDataContext.InstanceId,
                ValueCount, BaseDate, DateFilter,
                OrgLocations, PerformanceIndicators, Metrics, SelGcaID,
                CollectorId, ApproverId,
                OrderByMetric, ViewMode, IncludeApprovedValues);
            List<Sp_exSelectMetricValuesResult> Vt = SV.ToList();
            


            var fml = (from f in dc.Frequency
                       orderby f.FrequencyID
                       select new FrequencyMetric
                       {
                           FrequencyID = f.FrequencyID,
                           Name = f.Name
                       }).ToList();

            for (int i = 0; i < fml.Count(); i++)
            {
                var fm = fml[i];
                DateTime FirstDate;
                /*if (StartDates.Keys.Contains(fm.FrequencyID))
                {
                    FirstDate = FrequencyFirstDate[fm.FrequencyID];
                }
                else*/
                {
                    FirstDate = Frequency.GetNormalizedDate(fm.FrequencyID, BaseDate);                    
                }

                fm.Date = GetDateHeader(fm.FrequencyID, FirstDate, ValueCount);
                
                DateTime StartTime = DateTime.Now;


                List<Sp_exSelectMetricValuesResult> V = Vt.Where(r => r.MetricFrequencyID == fm.FrequencyID).ToList();

                List<MetricOrgValue> MetricMetricValues = new List<MetricOrgValue>();
                MetricOrgValue LastMetricMetricValue = null;
                CompositeDailyValues LastCompositeDailyValues = null;
                int GroupNumber = 0;
                int GroupCount = 0;

                List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
                bool FirstResult = true;

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

                        AddMetricValue(LastMetricMetricValue, /*fm*/LastMetricMetricValue == null ? null : LastMetricMetricValue.Date, null, LastCompositeDailyValues, fm.FrequencyID, ViewMode);//!!!

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


                        DateTime CurFirstDate = MetricTrac.Bll.Frequency.GetNormalizedDate(fm.FrequencyID, BaseDate);
                        foreach (ShiftDate sd in StartDates)
                            if (sd.FrequencyID == fm.FrequencyID && sd.EntityID == (OrderByMetric ? LastMetricMetricValue.MetricID : LastMetricMetricValue.OrgLocationID))
                                CurFirstDate = sd.StartDate;
                        LastMetricMetricValue.Date = GetDateHeader(fm.FrequencyID, CurFirstDate, ValueCount);

                        // define actual measure unit
                        if (v.MetricOrgLocationUoMID != null)
                            LastMetricMetricValue.InputUnitOfMeasureName = LastMetricMetricValue.OrgLocationUnitOfMeasureName;



                        var vvv = (from pij in dc.PerformanceIndicatorMetricJunc
                                   where pij.MetricID == LastMetricMetricValue.MetricID
                                   select pij).ToList();

                        

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

                        if (LastMetricValue.FrequencyID != LastMetricMetricValue.FrequencyID)
                        {
                            if (LastMetricValue.MetricDataTypeID != 1) continue;
                            decimal Val;
                            if (!decimal.TryParse(LastMetricValue.Value, out Val)) continue;

                            if (LastCompositeDailyValues == null) LastCompositeDailyValues = new CompositeDailyValues();
                            //LastCompositeDailyValues.AddCompositeDailyValue((DateTime)v.InputDate, Val, (int)LastMetricValue.FrequencyID, LastMetricValue.Date);

                            continue;
                        }

                        AddMetricValue(LastMetricMetricValue, /*fm*/LastMetricMetricValue == null ? null : LastMetricMetricValue.Date, LastMetricValue, LastCompositeDailyValues, fm.FrequencyID, ViewMode); //!!!
                    }
                }

                SetOrgLocationNumber(GroupNumber, MetricMetricValues);
                AddMetricValue(LastMetricMetricValue, /*fm*/LastMetricMetricValue == null ? null : LastMetricMetricValue.Date, null, LastCompositeDailyValues, fm.FrequencyID, ViewMode); //!!!


                /*Guid? ActualValueUoM = 
                    (LastMetricValue.RelatedOrgLocationUoMRecordID != null) ?
                    v.OrgLocationUnitOfMeasureID
                    :
                    v.MetricInputUnitOfMeasureID*/
                if (!ViewMode)
                    for (int ii = 0; ii < MetricMetricValues.Count; ii++)
                    {
                        bool AllValuesCollectionDisabled = true;
                        MetricOrgValue mov = MetricMetricValues[ii];
                        foreach (MetricValue.Extend me in mov.MetricValues)
                            if (me.CollectionEnabled)
                                AllValuesCollectionDisabled = false;
                        if (AllValuesCollectionDisabled)
                            MetricMetricValues.Remove(mov);
                    }
                else if (OrderByMetric && MetricMetricValues.Count > 0) // values vertical aggregation
                {   
                    Dictionary<Guid, List<string>> MetricTotals = new Dictionary<Guid, List<string>>();
                    Dictionary<Guid, Guid?> ActualUoM = new Dictionary<Guid, Guid?>();

                    Guid MetricID = MetricMetricValues[0].MetricID;
                    List<List<string>> Inputs = new List<List<string>>();
                    Guid? FirstUoM = Guid.Empty;
                    bool SameUoM = true;

                    for (int j = 0; j < MetricMetricValues.Count; j++)
                    {
                        if (MetricMetricValues[j].MetricID == MetricID)
                        {
                            List<string> OrgLocValues = new List<string>();
                            foreach (MetricValue.Extend mv in MetricMetricValues[j].MetricValues)
                            {
                                if (FirstUoM == Guid.Empty)
                                {
                                    if (mv.RelatedOrgLocationUoMRecordID != null)
                                        FirstUoM = mv.OrgLocationUnitOfMeasureID;
                                }
                                else 
                                {
                                    if (mv.RelatedOrgLocationUoMRecordID != null)
                                        if (FirstUoM != mv.OrgLocationUnitOfMeasureID)
                                            SameUoM = false;
                                }
                                OrgLocValues.Add(mv.Value);
                            }
                            Inputs.Add(OrgLocValues);
                        }
                        else
                        {
                            if (SameUoM && Inputs.Count > 1)
                            {
                                List<string> TotalValues = SumValues(Inputs);
                                MetricTotals.Add(MetricID, TotalValues);
                                ActualUoM.Add(MetricID, 
                                    FirstUoM == Guid.Empty ? MetricMetricValues[j].InputUnitOfMeasureID : FirstUoM);
                            }

                            if (j < MetricMetricValues.Count - 1)
                            {
                                MetricID = MetricMetricValues[j].MetricID;
                                FirstUoM = Guid.Empty;
                                SameUoM = true;
                                Inputs = new List<List<string>>();
                                j--;
                            }
                        }
                    }
                    if (SameUoM && Inputs.Count > 1)
                    {
                        List<string> TotalValues = SumValues(Inputs);
                        MetricTotals.Add(MetricID, TotalValues);
                        ActualUoM.Add(MetricID,
                                    FirstUoM == Guid.Empty ? MetricMetricValues[MetricMetricValues.Count - 1].InputUnitOfMeasureID : FirstUoM);
                    }

                    foreach (Guid m in MetricTotals.Keys)
                    {                        
                        Guid? MeasureUnit = ActualUoM[m];

                        int Index = MetricMetricValues.FindLastIndex(s => s.MetricID == m);
                        MetricOrgValue Example = MetricMetricValues[Index];
                        // Create and add new metricorgvalue 
                        //================
                        MetricOrgValue TotalMetricOrgValue = new MetricOrgValue();
                        TotalMetricOrgValue.GroupCount = Example.GroupCount;

                        //Metric fields
                        TotalMetricOrgValue.InstanceId = Example.InstanceId;
                        TotalMetricOrgValue.MetricID = Example.MetricID;
                        TotalMetricOrgValue.Name = Example.Name;
                        TotalMetricOrgValue.FrequencyID = Example.FrequencyID;
                        TotalMetricOrgValue.MetricCategoryID = Example.MetricCategoryID;
                        TotalMetricOrgValue.MetricDataTypeID = Example.MetricDataTypeID;
                        TotalMetricOrgValue.NODecPlaces = Example.NODecPlaces;
                        TotalMetricOrgValue.NOMinValue = Example.NOMinValue;
                        TotalMetricOrgValue.NOMaxValue = Example.NOMaxValue;
                        TotalMetricOrgValue.MetricTypeID = Example.MetricTypeID;
                        TotalMetricOrgValue.UnitOfMeasureID = Example.UnitOfMeasureID;
                        TotalMetricOrgValue.InputUnitOfMeasureID = MeasureUnit;
                        TotalMetricOrgValue.CollectionStartDate = Example.CollectionStartDate;
                        TotalMetricOrgValue.CollectionEndDate = Example.CollectionEndDate;
                        TotalMetricOrgValue.AllowCustomNames = Example.AllowCustomNames;

                        //Extend fields                   
                        TotalMetricOrgValue.FrequencyName = Example.FrequencyName;
                        TotalMetricOrgValue.UnitOfMeasureName = Example.UnitOfMeasureName;
                        TotalMetricOrgValue.InputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, MeasureUnit);

                        //MetricOrg fields                    
                        TotalMetricOrgValue.OrgLocationID = Example.OrgLocationID;
                        TotalMetricOrgValue.OrgLocationFullName = "Total";
                        TotalMetricOrgValue.CollectorUserID = Example.CollectorUserID;
                        TotalMetricOrgValue.RelatedOrgLocationUoMRecordID = Example.RelatedOrgLocationUoMRecordID;
                        TotalMetricOrgValue.OrgLocationUnitOfMeasureID = MeasureUnit;
                        TotalMetricOrgValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, MeasureUnit);

                        TotalMetricOrgValue.RelatedOrgLocationNameRecordID = Example.RelatedOrgLocationNameRecordID;
                        TotalMetricOrgValue.MetricOrgLocationAlias = Example.MetricOrgLocationAlias;
                        TotalMetricOrgValue.MetricOrgLocationCode = Example.MetricOrgLocationCode;
                        TotalMetricOrgValue.Date = Example.Date;
                        TotalMetricOrgValue.IsTotalAgg = true;

                        // define actual measure unit                            
                        TotalMetricOrgValue.MetricValues = new List<Extend>();

                        //Values                       
                        List<string> sTotalValues = MetricTotals[m];
                        if (sTotalValues.Count == Example.MetricValues.Count)
                        {
                            for (int k = 0; k < sTotalValues.Count; k++)
                            {
                                MetricValue.Extend ExampleValue = Example.MetricValues[k];
                                string sTotalValue = sTotalValues[k];

                                MetricValue.Extend TotalValue = new MetricValue.Extend();
                                // Value fields                            
                                TotalValue.MetricID = ExampleValue.MetricID;
                                TotalValue.Date = ExampleValue.Date;
                                TotalValue.OrgLocationID = Guid.Empty;
                                TotalValue.FrequencyID = ExampleValue.FrequencyID;
                                TotalValue.Value = sTotalValue;
                                TotalValue.Verified = false;
                                TotalValue.Approved = false;
                                TotalValue.FilesAttached = false;
                                TotalValue.ReviewUpdated = false;
                                TotalValue.MissedCalc = false;
                                TotalValue.MetricDataTypeID = ExampleValue.MetricDataTypeID;
                                TotalValue.UnitOfMeasureID = ExampleValue.UnitOfMeasureID;
                                TotalValue.InputUnitOfMeasureID = MeasureUnit;
                                TotalValue.Notes = String.Empty;

                                // Extend fields
                                // extend - value reference
                                TotalValue.ValueFrequencyName = ExampleValue.ValueFrequencyName;
                                TotalValue.ValueUnitOfMeasureName = ExampleValue.ValueUnitOfMeasureName;
                                TotalValue.ValueInputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, MeasureUnit);
                                TotalValue.OrgLocationFullName = "<b>Total</b>";
                                TotalValue.RelatedOrgLocationUoMRecordID = ExampleValue.RelatedOrgLocationUoMRecordID;
                                TotalValue.OrgLocationUnitOfMeasureID = MeasureUnit;
                                TotalValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, MeasureUnit);

                                // extend - metric fields
                                TotalValue.MetricName = ExampleValue.MetricName;
                                TotalValue.MetricFrequencyID = ExampleValue.MetricFrequencyID;
                                TotalValue.ActualMetricDataTypeID = ExampleValue.ActualMetricDataTypeID;
                                TotalValue.MetricCategoryID = ExampleValue.MetricCategoryID;
                                TotalValue.MetricUnitOfMeasureID = ExampleValue.MetricUnitOfMeasureID;
                                TotalValue.MetricInputUnitOfMeasureID = ExampleValue.MetricInputUnitOfMeasureID;
                                TotalValue.NODecPlaces = TotalMetricOrgValue.NODecPlaces;
                                TotalValue.NOMinValue = TotalMetricOrgValue.NOMinValue;
                                TotalValue.NOMaxValue = TotalMetricOrgValue.NOMaxValue;
                                TotalValue.MetricInputUnitOfMeasureName = ExampleValue.MetricInputUnitOfMeasureName;
                                TotalValue.MetricUnitOfMeasureName = ExampleValue.MetricUnitOfMeasureName;
                                TotalValue.IsCalculated = true;
                                TotalValue.CollectionStartDate = DateTime.MinValue;
                                TotalValue.CollectionEndDate = DateTime.MaxValue;
                                TotalValue.CollectionEnabled = true;
                                TotalValue.IsTotalAgg = true;

                                TotalMetricOrgValue.MetricValues.Add(TotalValue);
                            }
                        }
                        
                        //================
                        //INSERT
                        MetricMetricValues.Insert(Index + 1, TotalMetricOrgValue);
                    }                    
                }
                fm.Metrics = MetricMetricValues;
                
                if (fm.Metrics.Count < 1)
                {
                    fml.Remove(fm);
                    i--;
                }
            }
            return fml;
        }

        private static List<string> SumValues(List<List<string>> Inputs)
        {
            List<string> Result = new List<string>();
            if (Inputs.Count > 0)
                for (int j = 0; j < Inputs[0].Count; j++)
                {
                    double sum = 0;
                    bool IsValue = false;
                    for (int i = 0; i < Inputs.Count; i++)
                    {
                        double x = 0;
                        if (double.TryParse(Inputs[i][j], out x))
                        {
                            IsValue = true;
                            sum += x;
                        }
                    }
                    string res= String.Empty;
                    if (IsValue)
                        res = sum.ToString();
                    Result.Add(res);
                }
            return Result;
        }
        // ==========================================================


        public static TimeSpan SqlQueryTime;
        public static TimeSpan TotalTime;

        // ========= Metric Input List  =========
        public static List<FrequencyMetric> List(int ValueCount, DateTime BaseDate, Dictionary<int, DateTime> FrequencyFirstDate, Guid?[] SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid?[] SelPiID, Guid? SelUserId, bool ViewMode, /*bool OrderByMetric*/GroupByMode GroupBy, Guid? @ApproverUserId)
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

            for (int i = 0; i < fml.Count(); i++)
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
                fm.Metrics = CommonList(ValueCount, FirstDate, fm.FrequencyID, dc, fm.Date, SelMetricID, SelOrgLocationsID, SelGcaID, SelPiID, SelUserId, ViewMode, GroupBy, @ApproverUserId, true);
                if (fm.Metrics.Count < 1)
                {
                    fml.Remove(fm);
                    i--;
                }
            }


            if (ViewMode)
            {
            }

            TotalTime = StartTime - DateTime.Now;

            return fml;
        }

        // ========= Metric Bulk Edit List  =========
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
                List<MetricOrgValue> l = CommonList(ValueCount, BaseDate, FrequencyID, dc, fml.Date, new Guid?[1] { SelMetricID }, new Guid?[1] { SelOrgLocationID }, null, null, @CollectorUserId, ViewMode, (SelMetricID != null ? GroupByMode.Metric : GroupByMode.Location), @ApproverUserId, IncludeApprovedValues);
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

        // ========= Intermediate List for Input and Bulk methods  =========
        public static List<MetricOrgValue> CommonList(int ValueCount, DateTime NormalizedDate, int FrequencyID, LinqMicajahDataContext dc, List<DateHeader> hl, Guid?[] SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid?[] SelPiID, Guid? SelUserId, bool ViewMode, /*bool OrderByMetric*/GroupByMode GroupBy, Guid? @ApproverUserId, bool IncludeApprovedValues)
        {
            DateTime EndDate = Frequency.AddPeriod(NormalizedDate, FrequencyID, 1);
            DateTime BeginDate = Frequency.AddPeriod(EndDate, FrequencyID, -ValueCount);
            return List(BeginDate, EndDate, FrequencyID, dc, hl, SelMetricID, SelOrgLocationsID, SelGcaID, SelPiID, SelUserId, ViewMode, GroupBy, @ApproverUserId, IncludeApprovedValues);
        }

        // ========= Export Excel List  =========
        public static FrequencyMetric ExportList(DateTime BeginDate, DateTime EndDate, int FrequencyID, Guid?[] SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid?[] SelPiID, Guid? SelUserId, bool ViewMode, bool OrderByMetric, Guid? @ApproverUserId)
        {
            FrequencyMetric f = new FrequencyMetric();
            f.Date = GetDateHeader(FrequencyID, BeginDate, EndDate);
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                f.Metrics = List(BeginDate, EndDate, FrequencyID, dc, f.Date, SelMetricID, SelOrgLocationsID, SelGcaID, SelPiID, SelUserId, ViewMode, OrderByMetric ? GroupByMode.Metric : GroupByMode.Location, @ApproverUserId, true);
            }
            return f;
        }
                
        public static List<MetricOrgValue> List(DateTime BeginDate, DateTime EndDate, int FrequencyID, LinqMicajahDataContext dc, List<DateHeader> hl, Guid?[] SelMetricID, Guid?[] SelOrgLocationsID, Guid? SelGcaID, Guid?[] SelPiID, Guid? SelUserId, bool ViewMode, GroupByMode GroupBy, Guid? @ApproverUserId, bool IncludeApprovedValues)
        {
            DateTime StartTime = DateTime.Now;

            string OrgLocations = GetLocationsEncodedString(SelOrgLocationsID, ',');
            string PerformanceIndicators = GetLocationsEncodedString(SelPiID, ',');
            string Metrics = GetLocationsEncodedString(SelMetricID, ',');
            ISingleResult<Sp_SelectMetricValuesResult> V = dc.Sp_SelectMetricValues(LinqMicajahDataContext.InstanceId, FrequencyID, EndDate, BeginDate,
                OrgLocations, PerformanceIndicators, Metrics, SelGcaID, SelUserId, (int)GroupBy, ViewMode, @ApproverUserId, IncludeApprovedValues);
            List<Sp_SelectMetricValuesResult> vl = V.ToList();
            List<MetricOrgValue> MetricMetricValues = new List<MetricOrgValue>();
            MetricOrgValue LastMetricMetricValue = null;
            CompositeDailyValues LastCompositeDailyValues = null;
            int GroupNumber = 0;
            int GroupCount = 0;

            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            bool FirstResult = true;

            foreach (var v in vl)
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
                    LastMetricMetricValue.MetricCategoryName = v.MetricCategoryName;
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
                    LastMetricValue.MetricCategoryName = v.MetricCategoryName;
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

                    if (LastMetricValue.FrequencyID != LastMetricMetricValue.FrequencyID)
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

            /*Guid? ActualValueUoM = 
                    (LastMetricValue.RelatedOrgLocationUoMRecordID != null) ?
                    v.OrgLocationUnitOfMeasureID
                    :
                    v.MetricInputUnitOfMeasureID*/
            if (!ViewMode)
                for (int ii = 0; ii < MetricMetricValues.Count; ii++)
                {
                    bool AllValuesCollectionDisabled = true;
                    MetricOrgValue mov = MetricMetricValues[ii];
                    foreach (MetricValue.Extend me in mov.MetricValues)
                        if (me.CollectionEnabled)
                            AllValuesCollectionDisabled = false;
                    if (AllValuesCollectionDisabled)
                        MetricMetricValues.Remove(mov);
                }
            else if (GroupBy != GroupByMode.Location && MetricMetricValues.Count > 0) // values vertical aggregation
            {
                Dictionary<Guid, List<string>> MetricTotals = new Dictionary<Guid, List<string>>();
                Dictionary<Guid, Guid?> ActualUoM = new Dictionary<Guid, Guid?>();

                Guid MetricID = MetricMetricValues[0].MetricID;
                List<List<string>> Inputs = new List<List<string>>();
                Guid? FirstUoM = Guid.Empty;
                bool SameUoM = true;

                for (int j = 0; j < MetricMetricValues.Count; j++)
                {
                    if (MetricMetricValues[j].MetricID == MetricID)
                    {
                        List<string> OrgLocValues = new List<string>();
                        foreach (MetricValue.Extend mv in MetricMetricValues[j].MetricValues)
                        {
                            if (FirstUoM == Guid.Empty)
                            {
                                if (mv.RelatedOrgLocationUoMRecordID != null)
                                    FirstUoM = mv.OrgLocationUnitOfMeasureID;
                            }
                            else
                            {
                                if (mv.RelatedOrgLocationUoMRecordID != null)
                                    if (FirstUoM != mv.OrgLocationUnitOfMeasureID)
                                        SameUoM = false;
                            }
                            OrgLocValues.Add(mv.Value);
                        }
                        Inputs.Add(OrgLocValues);
                    }
                    else
                    {
                        if (SameUoM && Inputs.Count > 1)
                        {
                            List<string> TotalValues = SumValues(Inputs);
                            MetricTotals.Add(MetricID, TotalValues);
                            ActualUoM.Add(MetricID,
                                FirstUoM == Guid.Empty ? MetricMetricValues[j].InputUnitOfMeasureID : FirstUoM);
                        }

                        if (j < MetricMetricValues.Count - 1)
                        {
                            MetricID = MetricMetricValues[j].MetricID;
                            FirstUoM = Guid.Empty;
                            SameUoM = true;
                            Inputs = new List<List<string>>();
                            j--;
                        }
                    }
                }
                if (SameUoM && Inputs.Count > 1)
                {
                    List<string> TotalValues = SumValues(Inputs);
                    MetricTotals.Add(MetricID, TotalValues);
                    ActualUoM.Add(MetricID,
                                FirstUoM == Guid.Empty ? MetricMetricValues[MetricMetricValues.Count - 1].InputUnitOfMeasureID : FirstUoM);
                }

                foreach (Guid m in MetricTotals.Keys)
                {
                    Guid? MeasureUnit = ActualUoM[m];

                    int Index = MetricMetricValues.FindLastIndex(s => s.MetricID == m);
                    MetricOrgValue Example = MetricMetricValues[Index];
                    // Create and add new metricorgvalue 
                    //================
                    MetricOrgValue TotalMetricOrgValue = new MetricOrgValue();
                    TotalMetricOrgValue.GroupCount = Example.GroupCount;

                    //Metric fields
                    TotalMetricOrgValue.InstanceId = Example.InstanceId;
                    TotalMetricOrgValue.MetricID = Example.MetricID;
                    TotalMetricOrgValue.Name = Example.Name;
                    TotalMetricOrgValue.FrequencyID = Example.FrequencyID;
                    TotalMetricOrgValue.MetricCategoryID = Example.MetricCategoryID;
                    TotalMetricOrgValue.MetricCategoryName = Example.MetricCategoryName;
                    TotalMetricOrgValue.MetricDataTypeID = Example.MetricDataTypeID;
                    TotalMetricOrgValue.NODecPlaces = Example.NODecPlaces;
                    TotalMetricOrgValue.NOMinValue = Example.NOMinValue;
                    TotalMetricOrgValue.NOMaxValue = Example.NOMaxValue;
                    TotalMetricOrgValue.MetricTypeID = Example.MetricTypeID;
                    TotalMetricOrgValue.UnitOfMeasureID = Example.UnitOfMeasureID;
                    TotalMetricOrgValue.InputUnitOfMeasureID = MeasureUnit;
                    TotalMetricOrgValue.CollectionStartDate = Example.CollectionStartDate;
                    TotalMetricOrgValue.CollectionEndDate = Example.CollectionEndDate;
                    TotalMetricOrgValue.AllowCustomNames = Example.AllowCustomNames;

                    //Extend fields                   
                    TotalMetricOrgValue.FrequencyName = Example.FrequencyName;
                    TotalMetricOrgValue.UnitOfMeasureName = Example.UnitOfMeasureName;
                    TotalMetricOrgValue.InputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, MeasureUnit);

                    //MetricOrg fields                    
                    TotalMetricOrgValue.OrgLocationID = Example.OrgLocationID;
                    TotalMetricOrgValue.OrgLocationFullName = "Total";
                    TotalMetricOrgValue.CollectorUserID = Example.CollectorUserID;
                    TotalMetricOrgValue.RelatedOrgLocationUoMRecordID = Example.RelatedOrgLocationUoMRecordID;
                    TotalMetricOrgValue.OrgLocationUnitOfMeasureID = MeasureUnit;
                    TotalMetricOrgValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, MeasureUnit);

                    TotalMetricOrgValue.RelatedOrgLocationNameRecordID = Example.RelatedOrgLocationNameRecordID;
                    TotalMetricOrgValue.MetricOrgLocationAlias = Example.MetricOrgLocationAlias;
                    TotalMetricOrgValue.MetricOrgLocationCode = Example.MetricOrgLocationCode;
                    TotalMetricOrgValue.Date = Example.Date;
                    TotalMetricOrgValue.IsTotalAgg = true;

                    // define actual measure unit                            
                    TotalMetricOrgValue.MetricValues = new List<Extend>();

                    //Values                       
                    List<string> sTotalValues = MetricTotals[m];
                    if (sTotalValues.Count == Example.MetricValues.Count)
                    {
                        for (int k = 0; k < sTotalValues.Count; k++)
                        {
                            MetricValue.Extend ExampleValue = Example.MetricValues[k];
                            string sTotalValue = sTotalValues[k];

                            MetricValue.Extend TotalValue = new MetricValue.Extend();
                            // Value fields                            
                            TotalValue.MetricID = ExampleValue.MetricID;
                            TotalValue.Date = ExampleValue.Date;
                            TotalValue.OrgLocationID = Guid.Empty;
                            TotalValue.FrequencyID = ExampleValue.FrequencyID;
                            TotalValue.Value = sTotalValue;
                            TotalValue.Verified = false;
                            TotalValue.Approved = false;
                            TotalValue.FilesAttached = false;
                            TotalValue.ReviewUpdated = false;
                            TotalValue.MissedCalc = false;
                            TotalValue.MetricDataTypeID = ExampleValue.MetricDataTypeID;
                            TotalValue.UnitOfMeasureID = ExampleValue.UnitOfMeasureID;
                            TotalValue.InputUnitOfMeasureID = MeasureUnit;
                            TotalValue.Notes = String.Empty;

                            // Extend fields
                            // extend - value reference
                            TotalValue.ValueFrequencyName = ExampleValue.ValueFrequencyName;
                            TotalValue.ValueUnitOfMeasureName = ExampleValue.ValueUnitOfMeasureName;
                            TotalValue.ValueInputUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, MeasureUnit);
                            TotalValue.OrgLocationFullName = "<b>Total</b>";
                            TotalValue.RelatedOrgLocationUoMRecordID = ExampleValue.RelatedOrgLocationUoMRecordID;
                            TotalValue.OrgLocationUnitOfMeasureID = MeasureUnit;
                            TotalValue.OrgLocationUnitOfMeasureName = GetMeasureUnitAbbvr(OrgUoMs, MeasureUnit);

                            // extend - metric fields
                            TotalValue.MetricName = ExampleValue.MetricName;
                            TotalValue.MetricFrequencyID = ExampleValue.MetricFrequencyID;
                            TotalValue.ActualMetricDataTypeID = ExampleValue.ActualMetricDataTypeID;
                            TotalValue.MetricCategoryID = ExampleValue.MetricCategoryID;
                            TotalValue.MetricCategoryName = ExampleValue.MetricCategoryName;
                            TotalValue.MetricUnitOfMeasureID = ExampleValue.MetricUnitOfMeasureID;
                            TotalValue.MetricInputUnitOfMeasureID = ExampleValue.MetricInputUnitOfMeasureID;
                            TotalValue.NODecPlaces = TotalMetricOrgValue.NODecPlaces;
                            TotalValue.NOMinValue = TotalMetricOrgValue.NOMinValue;
                            TotalValue.NOMaxValue = TotalMetricOrgValue.NOMaxValue;
                            TotalValue.MetricInputUnitOfMeasureName = ExampleValue.MetricInputUnitOfMeasureName;
                            TotalValue.MetricUnitOfMeasureName = ExampleValue.MetricUnitOfMeasureName;
                            TotalValue.IsCalculated = true;
                            TotalValue.CollectionStartDate = DateTime.MinValue;
                            TotalValue.CollectionEndDate = DateTime.MaxValue;
                            TotalValue.CollectionEnabled = true;
                            TotalValue.IsTotalAgg = true;

                            TotalMetricOrgValue.MetricValues.Add(TotalValue);
                        }
                    }

                    //================
                    //INSERT
                    MetricMetricValues.Insert(Index + 1, TotalMetricOrgValue);
                }
            }

            return MetricMetricValues;
        }

        //=========
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
                    if (v != null) CalculatedValue.Value = ((decimal)v).ToString();
                }

                mmv.MetricValues.Add(CalculatedValue);
            }

            if (mv != null &&
                mmv.MetricValues.Count < hl.Count &&
                hl[mmv.MetricValues.Count].Date == mv.Date)
                mmv.MetricValues.Add(mv);
        }

        public static List<DateHeader> GetDateHeader(int FrequencyID, DateTime FirstDate, int ValueCount)
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
            DateTime CurrentDate = Frequency.AddPeriod(EndDate, FrequencyID, -1);
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

        private static void SetOrgLocationNumber(int GroupNumber, List<MetricOrgValue> MetricMetricValues)
        {
            if (GroupNumber > 0) MetricMetricValues[MetricMetricValues.Count - GroupNumber].GroupNumber = GroupNumber;
        }
    }
}
