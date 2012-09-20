using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class MetricValue
    {
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
                join _c in dc.ViewnameMetricCategory on
                    new { InstanceId = (Guid?)m.InstanceId, m.MetricCategoryID } equals
                    new { _c.InstanceId, _c.MetricCategoryID } into __c
                from mv in __mv.DefaultIfEmpty()
                join _mvf in dc.Frequency on mv.FrequencyID equals _mvf.FrequencyID into __mvf
                join _mvt in dc.MetricDataType on mv.MetricDataTypeID equals _mvt.MetricDataTypeID into __mvt
                join _OrgLocName in dc.ViewnameOrgLocation on
                    new { InstanceId = (Guid?)LinqMicajahDataContext.InstanceId, OrgLocationID = (Guid?)OrgLocationID } equals new { _OrgLocName.InstanceId, _OrgLocName.OrgLocationID } into __OrgLocName
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
    }
}
