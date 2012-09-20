using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class MetricValue
    {
        public static void SaveBulkValues(List<Bll.MetricOrgLocationUoM> MetricOrgLocationUoMList, List<Bll.MetricValue.Extend> NewValues, List<Bll.MetricValue.Extend> OldValues, Micajah.Common.Security.UserContext CurrentUser, bool InputMode)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                foreach (MetricOrgLocationUoM uom in MetricOrgLocationUoMList)
                {
                    MetricOrgLocationUoM MOUoM =
                            (from muom in dc.MetricOrgLocationUoM
                             where
                                 muom.MetricID == uom.MetricID &&
                                 muom.InstanceId == LinqMicajahDataContext.InstanceId &&
                                 muom.OrgLocationID == uom.OrgLocationID
                             select muom).FirstOrNull();
                        if (MOUoM == null)
                        {
                            Bll.MetricOrgLocationUoM muom = new Bll.MetricOrgLocationUoM();
                            muom.MetricID = uom.MetricID;
                            muom.OrgLocationID = uom.OrgLocationID;
                            muom.InputUnitOfMeasureID = uom.InputUnitOfMeasureID;
                            dc.MetricOrgLocationUoM.InsertOnSubmit(muom);
                        }
                        else
                            if (uom.InputUnitOfMeasureID != MOUoM.InputUnitOfMeasureID)
                                MOUoM.InputUnitOfMeasureID = uom.InputUnitOfMeasureID;
                }
                dc.SubmitChanges();
                Metric metric = null;
                for (int i = 0; i < NewValues.Count; i++)
                {
                    MetricValue.Extend NewValue = NewValues[i];
                    MetricValue.Extend OldValue = OldValues[i];

                    bool UpdateMetric = false;
                    if (metric == null)
                        UpdateMetric = true;
                    else if (metric.MetricID != NewValue.MetricID)
                        UpdateMetric = true;
                    if (UpdateMetric)
                        metric =
                            (from m in dc.Metric
                             where
                                 m.MetricID == NewValue.MetricID &&
                                 m.InstanceId == LinqMicajahDataContext.InstanceId &&
                                 m.Status == true
                             select m).FirstOrNull();
                    if (metric != null)
                    {
                        MetricValue metricValue =
                            (from mv in dc.MetricValue
                             where
                                 mv.MetricID == NewValue.MetricID &&
                                 mv.InstanceId == LinqMicajahDataContext.InstanceId &&
                                 mv.Status == true &&
                                 mv.Date == NewValue.Date &&
                                 mv.FrequencyID == metric.FrequencyID &&
                                 mv.OrgLocationID == NewValue.OrgLocationID
                             select mv).FirstOrNull();

                        string ConvertedValue = NewValue.Value;
                        if (metric.UnitOfMeasureID != NewValue.InputUnitOfMeasureID && metric.MetricDataTypeID == 1 && NewValue.InputUnitOfMeasureID != null && metric.UnitOfMeasureID != null)
                            ConvertedValue = Mc_UnitsOfMeasure.ConvertValue(NewValue.Value, (Guid)NewValue.InputUnitOfMeasureID, (Guid)metric.UnitOfMeasureID);

                        if (metricValue == null)
                        {
                            Bll.MetricValue mv = new Bll.MetricValue();
                            mv.MetricValueID = NewValue.MetricValueID = Guid.NewGuid();
                            mv.FrequencyID = metric.FrequencyID;
                            mv.UnitOfMeasureID = metric.UnitOfMeasureID;
                            mv.MetricDataTypeID = metric.MetricDataTypeID;
                            mv.ConvertedValue = ConvertedValue;
                            mv.InputUserId = CurrentUser.UserId;
                            mv.Approved = false;
                            mv.ReviewUpdated = false;
                            mv.ApproveUserId = null;
                            mv.Notes = null;
                            mv.FilesAttached = false;
                            mv.IsCalc = true;
                            mv.InProcess = false;

                            mv.MetricID = NewValue.MetricID;
                            mv.OrgLocationID = NewValue.OrgLocationID;
                            mv.Date = NewValue.Date;
                            mv.InputUnitOfMeasureID = NewValue.InputUnitOfMeasureID;
                            mv.Value = NewValue.Value;
                            dc.MetricValue.InsertOnSubmit(mv);
                        }
                        else
                        {
                            metricValue.MetricDataTypeID = metric.MetricDataTypeID;
                            metricValue.InputUnitOfMeasureID = NewValue.InputUnitOfMeasureID;
                            metricValue.UnitOfMeasureID = metric.UnitOfMeasureID;
                            metricValue.ConvertedValue = ConvertedValue;
                            metricValue.Approved = NewValue.Approved;
                            metricValue.ReviewUpdated = (!InputMode) ? false : (metricValue.Approved == null && NewValue.Approved == null);
                            if (!InputMode)
                            {
                                if (metricValue.Approved != NewValue.Approved)
                                    metricValue.ApproveUserId = CurrentUser.UserId;
                                if (metricValue.Value != NewValue.Value)
                                    metricValue.InputUserId = CurrentUser.UserId;
                            }
                            else
                                metricValue.InputUserId = CurrentUser.UserId;
                            metricValue.Value = NewValue.Value;
                            metricValue.IsCalc = true;
                        }
                    }
                }
                dc.SubmitChanges();
                for (int i = 0; i < NewValues.Count; i++)
                {
                    MetricValue.Extend NewValue = NewValues[i];
                    MetricValue.Extend OldValue = OldValues[i];
                    Bll.Mc_User.Extend mue = Bll.Mc_User.GetValueInputUser(dc, OldValue.MetricValueID);
                    // build mail to data collector if status or comment were changed
                    if ((!InputMode) && (OldValue.Approved != NewValue.Approved))
                    {
                        Bll.MetricValueChangeLog.LogChange(dc,
                            OldValue.MetricValueID == Guid.Empty ? NewValue.MetricValueID : OldValue.MetricValueID,
                            Bll.MetricValueChangeTypeEnum.StatusChanged,
                            OldValue.MetricValueID == Guid.Empty ? "Pending" : (OldValue.Approved == null ? "Under Review" : ((bool)OldValue.Approved ? "Approved" : "Pending")),
                            NewValue.Approved == null ? "Under Review" : ((bool)NewValue.Approved ? "Approved" : "Pending"),
                            Utils.Mail.BuildLogMessageBody(OldValue, NewValue, String.Empty, CurrentUser, mue, Bll.MetricValueChangeTypeEnum.StatusChanged));

                        if (NewValue.Approved == null && mue != null)
                            Utils.Mail.Send(mue.Email, mue.FullName, "MetricTrac - Value Status is changed", Utils.Mail.BuildEmailBody(OldValue, NewValue, String.Empty, CurrentUser));
                    }
                    // record in change log
                    if (OldValue.MetricValueID == Guid.Empty)
                        Bll.MetricValueChangeLog.LogChange(NewValue.MetricValueID,
                            MetricTrac.Bll.MetricValueChangeTypeEnum.ValueEntered,
                            String.Empty,
                            NewValue.Value,
                            Utils.Mail.BuildLogMessageBody(OldValue, NewValue, "Bulk Edit", CurrentUser, mue, MetricTrac.Bll.MetricValueChangeTypeEnum.ValueEntered));
                    else
                        if (OldValue.Value != NewValue.Value)
                            Bll.MetricValueChangeLog.LogChange(OldValue.MetricValueID,
                                MetricTrac.Bll.MetricValueChangeTypeEnum.ValueChanged,
                                OldValue.Value,
                                NewValue.Value,
                                Utils.Mail.BuildLogMessageBody(OldValue, NewValue, "Bulk Edit", CurrentUser, mue, MetricTrac.Bll.MetricValueChangeTypeEnum.ValueChanged));

                }
                dc.SubmitChanges();
            }
        }
    }
}