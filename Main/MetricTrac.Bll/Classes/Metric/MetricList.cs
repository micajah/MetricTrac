using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Linq.SqlClient;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class Metric
    {
        // Methods used buy MetricList control only
        public static List<Extend> List(Guid? MetricCategoryID, string NameDescriptionPart)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            List<Extend> Metrics =
                (from m in dc.Metric
                join f in dc.Frequency on m.FrequencyID equals f.FrequencyID
                join mt in dc.MetricType on m.MetricTypeID equals mt.MetricTypeID
                join mdt in dc.MetricDataType on m.MetricDataTypeID equals mdt.MetricDataTypeID
                join _mc in dc.ViewnameMetricCategory on new { InstanceId = (Guid?)m.InstanceId, m.MetricCategoryID } equals new { _mc.InstanceId, _mc.MetricCategoryID } into __mc
                join _mch in dc.ViewHierarchyMetricCategory on new { InstanceId = (Guid?)m.InstanceId, m.MetricCategoryID } equals new { _mch.InstanceId, MetricCategoryID = _mch.SubMetricCategoryID } into __mch
                from mc in __mc.DefaultIfEmpty()
                from mch in __mch.DefaultIfEmpty()
                where
                    m.InstanceId == LinqMicajahDataContext.InstanceId
                    &&
                    m.Status == true
                    &&
                    (MetricCategoryID == null || mch.MetricCategoryID == MetricCategoryID)
                    &&
                    (String.IsNullOrEmpty(NameDescriptionPart) || SqlMethods.Like(m.Name, '%' + NameDescriptionPart + '%') || SqlMethods.Like(m.Notes, '%' + NameDescriptionPart + '%'))
                orderby
                    mc.FullName, m.MetricCategoryID, m.Name, m.MetricID
                select new Extend
                {
                    InstanceId = m.InstanceId,
                    MetricID = m.MetricID,
                    Name = m.Name/*.TrimEnd()*/,
                    Notes = m.Notes,
                    FrequencyID = m.FrequencyID,
                    Status = m.Status,
                    Created = m.Created,
                    Updated = m.Updated,
                    MetricCategoryID = m.MetricCategoryID,
                    FrequencyName = f.Name,
                    MetricCategoryName = mc.FullName,
                    UnitOfMeasureID = m.UnitOfMeasureID,
                    InputUnitOfMeasureID = m.InputUnitOfMeasureID,
                    UnitOfMeasureName = GetMeasureUnitName(OrgUoMs, m.UnitOfMeasureID),
                    InputUnitOfMeasureName = GetMeasureUnitName(OrgUoMs, m.InputUnitOfMeasureID),
                    MetricTypeID = m.MetricTypeID,
                    MetricDataTypeID = m.MetricDataTypeID,
                    MetricDataTypeName = mdt.Name,
                    MetricTypeName = mt.Name
                }).Distinct().OrderBy(k => k.MetricCategoryName).ThenBy(k => k.Name).ToList();

            
            List<MetricOrgLocation> Locations =
                (from m in Metrics
                join x in dc.ViewPathExtended on new { InstanceId = (Guid?)m.InstanceId, MetricID = (Guid?)m.MetricID } equals new { x.InstanceId, x.MetricID }               
                join o in dc.ViewnameOrgLocation on new { x.InstanceId, x.OrgLocationID } equals new { o.InstanceId, o.OrgLocationID }
                select new MetricOrgLocation
                         {
                             MetricID = m.MetricID,
                             OrgLocationID = (Guid)x.OrgLocationID,
                             Name = o.FullName,
                             IsVirtual = x.IsVirtual == 1
                         }).Distinct().ToList();

            List<Extend> lME = new List<Extend>();
            foreach (Extend m in Metrics)
            {
                m.Name = m.Name.TrimEnd();
                m.AssignedOrgLocations =
                        (from x in Locations                         
                         where x.MetricID == m.MetricID
                         select x).ToList();

                if (m.AssignedOrgLocations.Count == 0)
                {
                    m.AssignedLocationsNames = String.Empty;
                    m.AssignedLocationName = String.Empty;
                }
                else if (m.AssignedOrgLocations.Count == 1)
                {
                    m.AssignedLocationsNames = String.Empty;
                    m.AssignedLocationName = m.AssignedOrgLocations[0].Name + (m.AssignedOrgLocations[0].IsVirtual ? " (related input)" : String.Empty);
                }
                else if (m.AssignedOrgLocations.Count > 1)
                {
                    StringBuilder LocationNames = new StringBuilder();
                    foreach (MetricOrgLocation mol in m.AssignedOrgLocations)
                    {
                        if (LocationNames.Length > 0)
                            LocationNames.Append("<br />");
                        LocationNames.Append(mol.Name + (mol.IsVirtual ? " (related input)" : String.Empty));
                    }
                    m.AssignedLocationsNames = LocationNames.ToString();
                    m.AssignedLocationName = "Multiple Org Locations";
                }

                lME.Add(m);
            }



            /*ISingleResult<Sp_SelectFilterMetricsResult> result = dc.Sp_SelectFilterMetrics(LinqMicajahDataContext.InstanceId, MetricCategoryID, String.IsNullOrEmpty(NameDescriptionPart) ? null : NameDescriptionPart);
            
            
            foreach (Sp_SelectFilterMetricsResult me in result)
            {
                Extend m = new Extend();
                m.InstanceId = (Guid)me.InstanceId;
                m.MetricID = (Guid)me.MetricID;
                m.Name = me.Name.TrimEnd();
                m.Notes = me.Notes;
                m.FrequencyID = (int)me.FrequencyID;
                m.Status = me.Status;
                m.Created = (DateTime)me.Created;
                m.Updated = me.Updated;
                m.MetricCategoryID = me.MetricCategoryID == null ? null : (me.Status == true ? me.MetricCategoryID : null);
                m.FrequencyName = me.FrequencyName;
                m.MetricCategoryName = me.MetricCategoryName;

                m.UnitOfMeasureID = me.UnitOfMeasureID;
                m.InputUnitOfMeasureID = me.InputUnitOfMeasureID;
                m.UnitOfMeasureName = GetMeasureUnitName(OrgUoMs, me.UnitOfMeasureID);
                m.InputUnitOfMeasureName = GetMeasureUnitName(OrgUoMs, me.InputUnitOfMeasureID);
                m.MetricTypeID = (int)me.MetricTypeID;
                m.MetricDataTypeID = (int)me.MetricDataTypeID;
                m.MetricDataTypeName = me.MetricDataTypeName;
                m.MetricTypeName = me.MetricTypeName;

                m.AssignedOrgLocations =
                        (from x in dc.ViewPathExtended
                         join oln in dc.ViewnameOrgLocation on new { x.InstanceId, x.OrgLocationID } equals new { oln.InstanceId, oln.OrgLocationID }
                         where x.InstanceId == m.InstanceId && x.MetricID == m.MetricID
                         
                         select new MetricOrgLocation
                         {
                             Name = oln.FullName,
                             IsVirtual = x.IsVirtual == 1
                         }).ToList();
                if (m.AssignedOrgLocations.Count == 0)
                {
                    m.AssignedLocationsNames = String.Empty;
                    m.AssignedLocationName = String.Empty;
                }
                else if (m.AssignedOrgLocations.Count == 1)
                {
                    m.AssignedLocationsNames = String.Empty;
                    m.AssignedLocationName = m.AssignedOrgLocations[0].Name + (m.AssignedOrgLocations[0].IsVirtual ? " (Virtual)" : String.Empty);
                }
                else if (m.AssignedOrgLocations.Count > 1)
                {
                    StringBuilder LocationNames = new StringBuilder();
                    foreach (MetricOrgLocation mol in m.AssignedOrgLocations)
                    {
                        if (LocationNames.Length > 0)
                            LocationNames.Append("<br />");
                        LocationNames.Append(mol.Name + (mol.IsVirtual ? " (Virtual)" : String.Empty));
                    }
                    m.AssignedLocationsNames = LocationNames.ToString();
                    m.AssignedLocationName = "Multiple Org Locations";
                }

                lME.Add(m);
            }*/
            return lME;
        }

        public static void Delete(Guid MetricID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                /*var M = (from m in dc.Metric where m.MetricID == MetricID select m).FirstOrNull();
                if (M != null)
                {
                    dc.Metric.DeleteOnSubmit(M); 
                    dc.SubmitChanges();
                }*/
                var PIMecricJuncs = from j in dc.PerformanceIndicatorMetricJunc
                                    where j.MetricID == MetricID && j.Status == true &&
                                          j.InstanceId == LinqMicajahDataContext.InstanceId
                                    select j;

                foreach (PerformanceIndicatorMetricJunc j in PIMecricJuncs)
                    j.Status = false;

                Metric ret =
                            (from m in dc.Metric
                             where (m.InstanceId == LinqMicajahDataContext.InstanceId) && (m.MetricID == MetricID)
                             select m).FirstOrNull();
                if (ret != null) ret.Status = false;
                dc.SubmitChanges();
            }
        }
    }
}