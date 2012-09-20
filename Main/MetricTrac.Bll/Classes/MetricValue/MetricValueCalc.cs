using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class MetricValue
    {
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
                m.Generation == (ActGen - 1)
            select mv;

            foreach (MetricValue mv in metricValue)
            {
                mv.InProcess = true;
                mv.IsCalc = false;
            }
            dc.SubmitChanges();

            IMultipleResults results = dc.Sp_SelectOutputMetricValues(ActGen);
            _OutputValues = results.GetResult<Sp_SelectOutputMetricValuesResult1>()
                .Select(r => new Extend
                {
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
                .Select(r => new Extend
                {
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
                         /*m.MetricTypeID == 1
                         &&*/
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
    }
}