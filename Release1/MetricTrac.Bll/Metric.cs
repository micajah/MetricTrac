using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class Metric
    {
        public class Extend : Metric
        {
            public string FrequencyName { get; set; }
            public string MetricCategoryName { get; set; }
            public string UnitOfMeasureName { get; set; }
            public string InputUnitOfMeasureName { get; set; }
            public string MetricDataTypeName { get; set; }
            public string MetricTypeName { get; set; }            
        }

        public class MetricPIJunc : Metric
        {
            public Guid PerformanceIndicatorID { get; set; }
        }

        public class MetricOrgDataRulesResult
        {
            public Guid? MetricID { get; set; }
            public string MetricName { get; set; }

            public Guid? OrgLocationID { get; set; }
            public string OrgLocationName { get; set; }

            public Guid? CollectorUserID { get; set; }
            public Guid? CollectorGroupID { get; set; }
            public string CollectorName { get; set; }

            public Guid? ApproverUserID { get; set; }
            public Guid? ApproverGroupID { get; set; }
            public string ApproverName { get; set; }

            public string PINames { get; set; }
            public string GCANames { get; set; }
            public string PIFNames { get; set; }
        }
                
        class MetricComparer : IEqualityComparer<Metric>
        {
            public bool Equals(Metric x, Metric y)
            {                
                if (Object.ReferenceEquals(x, y))
                    return true;
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;
                return x.MetricID == y.MetricID;
            }

            public int GetHashCode(Metric product)
            {                
                if (Object.ReferenceEquals(product, null)) return 0;
                int hashMetricID = product.MetricID == null ? 0 : product.MetricID.GetHashCode();
                return hashMetricID;
            }
        }

        public static List<Metric> List(Guid? OrgLocationID, Guid? GroupCategoryAspectID, Guid? PerformanceIndicatorID, Guid? PerformanceIndicatorFormID, Guid? CollectorUserId)
        {            
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            ISingleResult<Sp_SelectMetricListResult> result = dc.Sp_SelectMetricList(LinqMicajahDataContext.InstanceId, CollectorUserId, OrgLocationID, GroupCategoryAspectID, PerformanceIndicatorID, PerformanceIndicatorFormID);
            List<Metric> resultListD = new List<Metric>();            
            foreach (Sp_SelectMetricListResult me in result)
            {
                Metric m = new Metric();
                m.InstanceId = (Guid)me.InstanceId;
                m.MetricID = (Guid)me.MetricID;
                m.Name = me.Name;
                m.Alias = me.Alias;
                m.Code = me.Code;
                m.Status = me.Status;
                m.Created = (DateTime)me.Created;
                m.Updated = me.Updated;
                m.MetricCategoryID = me.MetricCategoryID;
                m.UnitOfMeasureID = me.UnitOfMeasureID;
                m.InputUnitOfMeasureID = me.InputUnitOfMeasureID;                
                m.MetricTypeID = (int)me.MetricTypeID;
                m.MetricDataTypeID = (int)me.MetricDataTypeID;
                m.NODecPlaces = me.NODecPlaces;
                m.NOMaxValue = me.NOMaxValue;
                m.NOMinValue = me.NOMinValue;
                m.Definition = me.Definition;
                m.Documentation = me.Documentation;
                m.MetricReferences = me.MetricReferences;
                m.FormulaCode = me.FormulaCode;
                m.Variable = me.Variable;
                m.GrowUpIsGood = me.GrowUpIsGood;

                resultListD.Add(m);
            }

            List<Metric> resultList = resultListD/*.Distinct(new MetricComparer())*/.ToList();
            /*IQueryable<Metric> result = 

                (
                    from m in dc.Metric
                    join _r in dc.D_MetricOrgLocationRule on new { m.MetricID, LinqMicajahDataContext.InstanceId, CollectorUserId } equals new { _r.MetricID, _r.InstanceId, _r.CollectorUserId } into __r
                    join _ij in dc.PerformanceIndicatorMetricJunc on new { m.MetricID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { _ij.MetricID, _ij.InstanceId, _ij.Status } into __ij

                    from r in __r.DefaultIfEmpty()

                    from ij in __ij.DefaultIfEmpty()
                    join _i in dc.PerformanceIndicator on new { ij.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { _i.PerformanceIndicatorID, _i.InstanceId, _i.Status } into __i

                    from i in __i.DefaultIfEmpty()
                    join _fj in dc.PerformanceIndicatorFormPerformanceIndicatorJunc on new { i.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { _fj.PerformanceIndicatorID, _fj.InstanceId, _fj.Status } into __fj
                    join _g in dc.GCAFullNameView on new { i.GroupCategoryAspectID, LinqMicajahDataContext.InstanceId } equals new { GroupCategoryAspectID=(Guid?)_g.GroupCategoryAspectID, _g.InstanceId } into __g

                    from g in __g.DefaultIfEmpty()

                    from fj in __fj.DefaultIfEmpty()
                    join _f in dc.PerformanceIndicatorForm on new { fj.PerformanceIndicatorFormID, LinqMicajahDataContext.InstanceId, Status = (bool?)true } equals new { _f.PerformanceIndicatorFormID, _f.InstanceId, _f.Status } into __f

                    from f in __f.DefaultIfEmpty()
                    join _nj in dc.PIFormOrgLocationJuncView on f.PerformanceIndicatorFormID equals _nj.PerformanceIndicatorFormID into __nj

                    from nj in __nj.DefaultIfEmpty()
                    join _n in dc.EntityNodeFullNameView on new { OrgLocationID = nj.OrgLocationID, InstanceId = (Guid?)null } equals new { OrgLocationID = _n.EntityNodeId, _n.InstanceId } into __n

                    from n in __n.DefaultIfEmpty()

                    where m.InstanceId == LinqMicajahDataContext.InstanceId &&
                          m.Status == true &&
                          (OrgLocationID == null || n.EntityNodeId == OrgLocationID || n.Parent1 == OrgLocationID || n.Parent2 == OrgLocationID || n.Parent3 == OrgLocationID || n.Parent4 == OrgLocationID || n.Parent5 == OrgLocationID) &&
                          (GroupCategoryAspectID == null || g.GroupCategoryAspectID == GroupCategoryAspectID || g.Parent1 == GroupCategoryAspectID || g.Parent2 == GroupCategoryAspectID) &&
                          (PerformanceIndicatorID == null || i.PerformanceIndicatorID == PerformanceIndicatorID) &&
                          (PerformanceIndicatorFormID == null || f.PerformanceIndicatorFormID == PerformanceIndicatorFormID) &&
                          (CollectorUserId == null || r.CollectorUserId == CollectorUserId)

                    select m

                );
            List<Metric> resultToList = result.ToList();
            List<Metric> resultList = resultToList.Distinct(new MetricComparer()).ToList();
            // Alternative
            //myCustomerList.GroupBy(cust => cust.CustomerId).Select(grp => grp.First());*/

            for (int n = 0; n < resultList.Count; n++)
            {
                var Cm = resultList[n];
                if (Cm.MetricTypeID != 2) continue;

                List<Metric> i = (
                    from m in dc.Metric
                    join r in dc.MetricRelation on new { m.InstanceId, m.MetricID } equals new { r.InstanceId, r.MetricID }
                    where r.ReferenceMetricID == Cm.MetricID
                    select m
                    ).ToList();

                foreach(var im in i)
                {
                    if (resultList.Exists(mm => mm.MetricID == im.MetricID)) continue;
                    resultList.Add(im);
                }
            }

            return resultList.OrderBy(m => m.Name).ToList();
        }

        // several OrgLocations
        public static List<Metric> List(Guid?[] OrgLocationsID, Guid? GroupCategoryAspectID, Guid? PerformanceIndicatorID, Guid? PerformanceIndicatorFormID, Guid? CollectorUserId)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            List<Metric> resultListD = new List<Metric>();
            if (OrgLocationsID != null && OrgLocationsID.Length > 0)
            {
                foreach (Guid? location in OrgLocationsID)
                {
                    ISingleResult<Sp_SelectMetricListResult> result = dc.Sp_SelectMetricList(LinqMicajahDataContext.InstanceId, CollectorUserId, location, GroupCategoryAspectID, PerformanceIndicatorID, PerformanceIndicatorFormID);
                    ParseMetricList(resultListD, result);
                }
            }
            else
            {
                ISingleResult<Sp_SelectMetricListResult> result = dc.Sp_SelectMetricList(LinqMicajahDataContext.InstanceId, CollectorUserId, null, GroupCategoryAspectID, PerformanceIndicatorID, PerformanceIndicatorFormID);
                ParseMetricList(resultListD, result);
            }


            List<Metric> resultList = resultListD.Distinct(new MetricComparer()).ToList();
            List<Metric> resultList2 = new List<Metric>();
            
            for (int n = 0; n < resultList.Count; n++)
            {
                var Cm = resultList[n];
                if (!resultList2.Exists(mm => mm.MetricID == Cm.MetricID))
                    resultList2.Add(Cm);
                if (Cm.MetricTypeID != 2) continue;

                List<Metric> i = (
                    from m in dc.Metric
                    join r in dc.MetricRelation on new { m.InstanceId, m.MetricID } equals new { r.InstanceId, r.MetricID }
                    where r.ReferenceMetricID == Cm.MetricID && m.Status == true
                    select m
                    ).ToList();

                foreach (var im in i)
                    if (!resultList2.Exists(mm => mm.MetricID == im.MetricID))
                        resultList2.Add(im);
            }

            return resultList2.OrderBy(m => m.Name).ToList();
        }

        private static void ParseMetricList(List<Metric> resultListD, ISingleResult<Sp_SelectMetricListResult> result)
        {
            foreach (Sp_SelectMetricListResult me in result)
            {
                Metric m = new Metric();
                m.InstanceId = (Guid)me.InstanceId;
                m.MetricID = (Guid)me.MetricID;
                m.Name = me.Name;
                m.Alias = me.Alias;
                m.Code = me.Code;
                m.Status = me.Status;
                m.Created = (DateTime)me.Created;
                m.Updated = me.Updated;
                m.MetricCategoryID = me.MetricCategoryID;
                m.UnitOfMeasureID = me.UnitOfMeasureID;
                m.InputUnitOfMeasureID = me.InputUnitOfMeasureID;
                m.MetricTypeID = (int)me.MetricTypeID;
                m.MetricDataTypeID = (int)me.MetricDataTypeID;
                m.NODecPlaces = me.NODecPlaces;
                m.NOMaxValue = me.NOMaxValue;
                m.NOMinValue = me.NOMinValue;
                m.Definition = me.Definition;
                m.Documentation = me.Documentation;
                m.MetricReferences = me.MetricReferences;
                m.FormulaCode = me.FormulaCode;
                m.Variable = me.Variable;
                m.GrowUpIsGood = me.GrowUpIsGood;

                resultListD.Add(m);
            }
            return;
        }

        public static List<Extend> List(Guid? MetricCategoryID, string NameDescriptionPart)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            ISingleResult<Sp_SelectFilterMetricsResult> result = dc.Sp_SelectFilterMetrics(LinqMicajahDataContext.InstanceId, MetricCategoryID, String.IsNullOrEmpty(NameDescriptionPart) ? null : NameDescriptionPart);
            List<Extend> lME = new List<Extend>();
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            foreach (Sp_SelectFilterMetricsResult me in result)
            {
                Extend m = new Extend();
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
                lME.Add(m);
            }
            return lME;
        }

        public static IQueryable<Extend> List()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();

            var Metrics =
                from m in dc.Metric
                join _f in dc.Frequency on m.FrequencyID equals _f.FrequencyID into __f
                join _c in dc.MetricCategoryFullNameView on
                    new { m.InstanceId, m.MetricCategoryID } equals
                    new { _c.InstanceId, MetricCategoryID = (Guid?)_c.MetricCategoryID } into __c

                join _mc in dc.MetricCategory on
                    new { m.InstanceId, m.MetricCategoryID } equals
                    new { _mc.InstanceId, MetricCategoryID = (Guid?)_mc.MetricCategoryID } into __mc

                join t in dc.MetricDataType on m.MetricDataTypeID equals t.MetricDataTypeID
                join mt in dc.MetricType on m.MetricTypeID equals mt.MetricTypeID

                from f in __f.DefaultIfEmpty()
                from c in __c.DefaultIfEmpty()
                from mc in __mc.DefaultIfEmpty()

                where m.InstanceId == LinqMicajahDataContext.InstanceId &&
                    m.Status == true
                orderby c.FullName, c.MetricCategoryID, m.Name, m.MetricID
                select new Metric.Extend
                {
                    MetricID = m.MetricID,
                    Name = m.Name,
                    Notes = m.Notes,
                    FrequencyID = m.FrequencyID,                    
                    Status = m.Status,
                    Created = m.Created,
                    Updated = m.Updated,

                    MetricCategoryID = m.MetricCategoryID == null ? null : (mc.Status == true ? m.MetricCategoryID : null),

                    FrequencyName = f.Name,

                    MetricCategoryName = c.FullName,

                    UnitOfMeasureID = m.UnitOfMeasureID,
                    UnitOfMeasureName = GetMeasureUnitName(OrgUoMs, m.UnitOfMeasureID),
                    InputUnitOfMeasureID = m.InputUnitOfMeasureID,
                    InputUnitOfMeasureName = GetMeasureUnitName(OrgUoMs, m.InputUnitOfMeasureID),
                    MetricTypeID = m.MetricTypeID,
                    MetricDataTypeID = t.MetricDataTypeID,
                    MetricDataTypeName = t.Name,
                    MetricTypeName = mt.Name,

                    NODecPlaces = m.NODecPlaces,
                    NOMaxValue = m.NOMaxValue,
                    NOMinValue = m.NOMinValue,

                    Definition = m.Definition,
                    Documentation = m.Documentation,
                    MetricReferences = m.MetricReferences
                };            
            return Metrics;
        }
        /*
        Program p = new Program();
        Program.SearchCompanies("test", "test2");
        var pr = from pi in  dataContext.Companies.Where(Program.SearchCompanies("test", "test2")) select pi;

        public static Expression<Func<Company, bool>> SearchMetrics(string keyword)
        {
            var predicate = PredicateBuilder.False<Company>();
            return predicate.Or(p => p.Name.Contains(temp));            
        }

        public static class PredicateBuilder
        {
            public static Expression<Func<T, bool>> True<T>() { return f => true; }
            public static Expression<Func<T, bool>> False<T>() { return f => false; }

            public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expr1,
                                                                Expression<Func<T, bool>> expr2)
            {
                var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
                return Expression.Lambda<Func<T, bool>>
                      (Expression.OrElse(expr1.Body, invokedExpr), expr1.Parameters);
            }

            public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expr1,
                                                                 Expression<Func<T, bool>> expr2)
            {
                var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
                return Expression.Lambda<Func<T, bool>>
                      (Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
            }
        }*/


        public static List<Metric.Extend> List(Guid PerformanceIndicatorID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();


            var Metrics =
                from j in dc.PerformanceIndicatorMetricJunc
                join m in dc.Metric on new { j.InstanceId, j.MetricID } equals new { m.InstanceId, m.MetricID }
                join _f in dc.Frequency on m.FrequencyID equals _f.FrequencyID into __f
                join _c in dc.MetricCategoryFullNameView on
                    new { m.InstanceId, m.MetricCategoryID } equals
                    new { _c.InstanceId, MetricCategoryID = (Guid?)_c.MetricCategoryID } into __c
                join t in dc.MetricDataType on m.MetricDataTypeID equals t.MetricDataTypeID
                join mt in dc.MetricType on m.MetricTypeID equals mt.MetricTypeID

                from f in __f.DefaultIfEmpty()

                from c in __c.DefaultIfEmpty()

                where
                    j.InstanceId == LinqMicajahDataContext.InstanceId &&
                    j.PerformanceIndicatorID == PerformanceIndicatorID &&
                    j.Status == true &&
                    m.Status == true
                orderby c.FullName, c.MetricCategoryID, m.Name, m.MetricID
                select new Metric.Extend
                {
                    MetricID = m.MetricID,
                    Name = m.Name.TrimEnd(),
                    Notes = m.Notes,
                    FrequencyID = m.FrequencyID,                    
                    Status = m.Status,
                    Created = m.Created,
                    Updated = m.Updated,

                    FrequencyName = f.Name,

                    MetricCategoryName = c.FullName,


                    UnitOfMeasureID = m.UnitOfMeasureID,
                    InputUnitOfMeasureID = m.InputUnitOfMeasureID,
                    UnitOfMeasureName = GetMeasureUnitName(OrgUoMs, m.UnitOfMeasureID),
                    InputUnitOfMeasureName = GetMeasureUnitName(OrgUoMs, m.InputUnitOfMeasureID),

                    MetricTypeID = m.MetricTypeID,
                    MetricDataTypeID = t.MetricDataTypeID,
                    MetricDataTypeName = t.Name,
                    MetricTypeName = mt.Name,

                    NODecPlaces = m.NODecPlaces,
                    NOMaxValue = m.NOMaxValue,
                    NOMinValue = m.NOMinValue,

                    Definition = m.Definition,
                    Documentation = m.Documentation,
                    MetricReferences = m.MetricReferences
                };

            return Metrics.ToList();
        }
               
        public static List<Sp_SelectUnassignedMetricResult> ListUnused(Guid PerformanceIndicatorID, Guid? PIID, Guid? PIFormID, Guid? GCAID, Guid?[] OrgLocationsID, Guid? DataCollectorID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            string OrgLocations = MetricTrac.Bll.MetricValue.GetLocationsEncodedString(OrgLocationsID, ',');
            var rv = dc.Sp_SelectUnassignedMetric(LinqMicajahDataContext.InstanceId, PerformanceIndicatorID, DataCollectorID, PIID, GCAID, PIFormID, null, OrgLocations);
            List<Sp_SelectUnassignedMetricResult> r = rv.ToList();
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            foreach (Sp_SelectUnassignedMetricResult res in r)
            {
                res.InputUnitOfMeasureName = GetMeasureUnitName(OrgUoMs, res.InputUnitOfMeasureID);
                res.UnitOfMeasureName = GetMeasureUnitName(OrgUoMs, res.UnitOfMeasureID);
                res.Name = res.Name.TrimEnd();
            }
            return r;
        }        

        private static List<Metric.Extend> ListReferenced(LinqMicajahDataContext dc, List<Guid> MetricIDs)
        {
            List<Guid> NewMetricIDs = MetricIDs;
            List<Guid> AddMetricIDs = new List<Guid>();
            List<Metric.Extend> result = new List<Extend>();

            foreach (Guid MetricID in MetricIDs)
            {
                Extend m = Get(MetricID);
                m.Name = m.Name.TrimEnd();
                result.Add(m);
            }

            while (NewMetricIDs.Count > 0)
            {
                for(int i=0;i<NewMetricIDs.Count;i++)
                {
                    Guid MetricID = NewMetricIDs[i];
                    var IDs = from r in dc.MetricRelation
                              where r.InstanceId == LinqMicajahDataContext.InstanceId && r.ReferenceMetricID == MetricID
                              select r.ReferenceMetricID;
                    foreach (Guid AddMetricID in IDs)
                    {
                        if (MetricIDs.Contains(AddMetricID)) continue;
                        AddMetricIDs.Add(AddMetricID);
                        MetricIDs.Add(AddMetricID);
                        Metric.Extend m = Get(AddMetricID);
                        if(m.MetricTypeID!=2) result.Add(m);
                    }
                }
                NewMetricIDs = AddMetricIDs;
            }
            return result;
        }

        public static List<Metric.Extend> ListReferenced(List<Guid> MetricIDs)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            MetricIDs = (from r in dc.MetricRelation where r.InstanceId == LinqMicajahDataContext.InstanceId && MetricIDs.Contains(r.MetricID) select r.ReferenceMetricID).ToList();
            return ListReferenced(dc, MetricIDs);
        }

        public static List<Metric.Extend> ListReferenced(Guid PerformanceIndicatorID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var MetricIDs = (from j in dc.PerformanceIndicatorMetricJunc
                             join r in dc.MetricRelation on new { LinqMicajahDataContext.InstanceId, j.MetricID } equals new { r.InstanceId, MetricID=r.ReferenceMetricID }
                             join m in dc.Metric on new { LinqMicajahDataContext.InstanceId, r.MetricID } equals new { m.InstanceId, m.MetricID }
                    where j.InstanceId == LinqMicajahDataContext.InstanceId && j.PerformanceIndicatorID == PerformanceIndicatorID && j.Status == true && m.Status == true
                    select r.MetricID).Distinct().ToList();
            return ListReferenced(dc, MetricIDs);            
        }

        public static Extend Get(Guid MetricID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();

            IQueryable<Extend> Metrics =
                from m in dc.Metric
                join _f in dc.Frequency on m.FrequencyID equals _f.FrequencyID into __f
                join _c in dc.MetricCategoryFullNameView on
                    new { m.InstanceId, m.MetricCategoryID } equals
                    new { _c.InstanceId, MetricCategoryID = (Guid?)_c.MetricCategoryID } into __c
                join t in dc.MetricDataType on m.MetricDataTypeID equals t.MetricDataTypeID
                join mt in dc.MetricType on m.MetricTypeID equals mt.MetricTypeID

                from f in __f.DefaultIfEmpty()
                from c in __c.DefaultIfEmpty()

                where m.InstanceId == LinqMicajahDataContext.InstanceId &&
                    m.MetricID == MetricID &&
                    m.Status == true
                orderby c.FullName, c.MetricCategoryID, m.Name, m.MetricID
                select new Metric.Extend
                {
                    MetricID = m.MetricID,
                    Name = m.Name,
                    Alias = m.Alias,
                    Code = m.Code,
                    Notes = m.Notes,
                    FrequencyID = m.FrequencyID,                    
                    Status = m.Status,
                    Created = m.Created,
                    Updated = m.Updated,
                    CollectionStartDate = m.CollectionStartDate,
                    CollectionEndDate = m.CollectionEndDate,
                    FrequencyName = f.Name,

                    MetricCategoryID = m.MetricCategoryID,


                    MetricCategoryName = c.FullName,

                    UnitOfMeasureID = m.UnitOfMeasureID,
                    InputUnitOfMeasureID = m.InputUnitOfMeasureID,

                    UnitOfMeasureName = GetMeasureUnitName(OrgUoMs, m.UnitOfMeasureID),
                    InputUnitOfMeasureName = GetMeasureUnitName(OrgUoMs, m.InputUnitOfMeasureID),

                    MetricDataTypeID = t.MetricDataTypeID,
                    MetricDataTypeName = t.Name,
                    MetricTypeName = mt.Name,

                    NODecPlaces = m.NODecPlaces,
                    NOMaxValue = m.NOMaxValue,
                    NOMinValue = m.NOMinValue,

                    Definition = m.Definition,
                    Documentation = m.Documentation,
                    MetricReferences = m.MetricReferences,

                    MetricTypeID = m.MetricTypeID,
                    FormulaCode = m.FormulaCode,
                    Variable = m.Variable,

                    GrowUpIsGood = m.GrowUpIsGood

                };
            Extend me = Metrics.FirstOrNull();            
            return me;
        }

        public override void OnDeleting(LinqMicajahDataContext dc, ref bool Cancel)
        {
            base.OnDeleting(dc, ref Cancel);
            var PIMecricJuncs = from j in dc.PerformanceIndicatorMetricJunc
                                where j.MetricID == MetricID && j.Status == true &&
                                      j.InstanceId == LinqMicajahDataContext.InstanceId
                                select j;

            foreach (PerformanceIndicatorMetricJunc j in PIMecricJuncs) j.Status = false;
        }

        public static void Delete(Guid MetricID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var M = (from m in dc.Metric where m.MetricID == MetricID select m).FirstOrNull();
                if (M != null)
                {
                    dc.Metric.DeleteOnSubmit(M); 
                    dc.SubmitChanges();
                }
            }
        }

        public static List<MetricOrgDataRulesResult> ListRulesResult()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            var q = from mo in dc.D_MetricOrgLocationRule
                    join m in dc.Metric on new { LinqMicajahDataContext.InstanceId, mo.MetricID } equals new { m.InstanceId, m.MetricID }
                    join o in dc.EntityNodeFullNameView on mo.OrgLocationID equals (Guid?)o.EntityNodeId

                    join _cu in dc.UserFullNameView on new { LinqMicajahDataContext.InstanceId, mo.CollectorUserId } equals new { _cu.InstanceId, CollectorUserId = (Guid?)_cu.UserId } into __cu
                    join _cg in dc.Mc_Group on new { mo.CollectorGroupId, Deleted = false } equals new { CollectorGroupId = (Guid?)_cg.GroupId, _cg.Deleted } into __cg
                    join _au in dc.UserFullNameView on new { LinqMicajahDataContext.InstanceId, mo.ApproverUserId } equals new { _au.InstanceId, ApproverUserId = (Guid?)_au.UserId } into __au
                    join _ag in dc.Mc_Group on new { mo.ApproverGroupId, Deleted = false } equals new { ApproverGroupId = (Guid?)_ag.GroupId, _ag.Deleted } into __ag

                    from cu in __cu.DefaultIfEmpty()
                    from cg in __cg.DefaultIfEmpty()
                    from au in __au.DefaultIfEmpty()
                    from ag in __ag.DefaultIfEmpty()

                    where mo.InstanceId == LinqMicajahDataContext.InstanceId

                    orderby (o.EntityNodeId==Guid.Empty?null:o.FullName) , mo.OrgLocationID, m.Name, mo.MetricID

                    select new MetricOrgDataRulesResult
                    {
                        MetricID = mo.MetricID,
                        MetricName = m.Name,

                        OrgLocationID = mo.OrgLocationID,
                        OrgLocationName = mo.OrgLocationID == Guid.Empty ? LinqMicajahDataContext.OrganizationName : o.FullName,

                        CollectorUserID = mo.CollectorUserId,
                        CollectorGroupID = mo.CollectorGroupId,
                        CollectorName = (mo.CollectorUserId == null) ? ("<b>Group:</b> " + cg.Name) : ("<b>User:</b> " + cu.FullName),

                        ApproverUserID = mo.ApproverUserId,
                        ApproverGroupID = mo.ApproverGroupId,
                        ApproverName = (mo.ApproverUserId == null) ? ("<b>Group:</b> " + ag.Name) : ("<b>User:</b> " + au.FullName),
                    };

            var l = q.ToList();

            foreach (MetricOrgDataRulesResult r in l)
            {
                var GCAs = (
                    from p in dc.D_MetricOrgLocationPath
                    join g in dc.GCAFullNameView on new { p.GroupCategoryAspectID, LinqMicajahDataContext.InstanceId } equals new { GroupCategoryAspectID=(Guid?)g.GroupCategoryAspectID, g.InstanceId }
                    where p.InstanceId==LinqMicajahDataContext.InstanceId && p.MetricID==r.MetricID && p.OrgLocationID==r.OrgLocationID
                    orderby g.FullName
                    select new GroupCategoryAspect.Extend
                    {
                        GroupCategoryAspectID = g.GroupCategoryAspectID,
                        InstanceId = g.InstanceId,
                        FullName = g.FullName,
                        IsVirtual = p.IsVirtual
                    }
                ).Distinct().ToList();

                foreach (var GCA in GCAs)
                {
                    r.GCANames += GCA.FullName + (GCA.IsVirtual?" (Virtual)":"") + "<br>";
                }

                var PIs = (
                    from p in dc.D_MetricOrgLocationPath
                    join pi in dc.PerformanceIndicator on new { p.PerformanceIndicatorID, LinqMicajahDataContext.InstanceId } equals new { PerformanceIndicatorID=(Guid?)pi.PerformanceIndicatorID, pi.InstanceId }
                    where p.InstanceId == LinqMicajahDataContext.InstanceId && p.MetricID == r.MetricID && p.OrgLocationID == r.OrgLocationID
                    orderby pi.Name
                    select new PerformanceIndicator.Extend
                    {
                        InstanceId = pi.InstanceId,
                        PerformanceIndicatorID = pi.PerformanceIndicatorID,
                        Name = pi.Name,
                        IsVirtual = p.IsVirtual
                    }
                ).Distinct().ToList();

                foreach (var PI in PIs)
                {
                    r.PINames += PI.Name + (PI.IsVirtual?" (virtual)":"") + "<br>";
                }

                var PIFs = (
                    from p in dc.D_MetricOrgLocationPath
                    join pif in dc.PerformanceIndicatorForm on new { p.PerformanceIndicatorFormID, LinqMicajahDataContext.InstanceId } equals new { PerformanceIndicatorFormID=(Guid?)pif.PerformanceIndicatorFormID, pif.InstanceId }
                    where p.InstanceId == LinqMicajahDataContext.InstanceId && p.MetricID == r.MetricID && p.OrgLocationID == r.OrgLocationID
                    orderby pif.Name
                    select new PerformanceIndicatorForm.Extend
                    {
                        InstanceId = pif.InstanceId,
                        PerformanceIndicatorFormID = pif.PerformanceIndicatorFormID,
                        Name = pif.Name,
                        IsVirtual = p.IsVirtual
                    }
                 ).Distinct().ToList();

                foreach (var PIF in PIFs)
                {
                    r.PIFNames += PIF.Name + (PIF.IsVirtual?" (virtual)":"") + "<br>";
                }
            }

            return l;
        }

        //==========================

        
        // Calc metrics and formulas
        public static char[] aChars = { ' ', '+', '-', '*', '/', '(', ')', '.' };

        public static bool CheckUniqueName(string Name, Guid MetricID)
        {
            bool result = false;
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var mName = (from j in dc.Metric
                                    where j.Name == Name && j.Status == true &&
                                          j.InstanceId == LinqMicajahDataContext.InstanceId
                                          &&
                                          j.MetricID != MetricID
                                    select j).FirstOrNull();
                result = mName != null;
            }
            return result;
        }

        public static List<Metric.Extend> GetBaseMetrics(Guid? MetricCategoryID, int FrequencyID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            //List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();
            Guid? ParentMetricCategoryID = MetricCategoryID;
            if (MetricCategoryID != null && MetricCategoryID != Guid.Empty)
            {
                ParentMetricCategoryID =
                    (from mc1 in dc.MetricCategory
                     join _mc2 in dc.MetricCategory on
                         new { mc1.InstanceId, mc1.ParentId } equals
                         new { _mc2.InstanceId, ParentId = (Guid?)_mc2.MetricCategoryID } into __mc2
                     from mc2 in __mc2.DefaultIfEmpty()
                     join _mc3 in dc.MetricCategory on
                         new { mc2.InstanceId, mc2.ParentId } equals
                         new { _mc3.InstanceId, ParentId = (Guid?)_mc3.MetricCategoryID } into __mc3
                     from mc3 in __mc3.DefaultIfEmpty()
                     where
                         mc1.InstanceId == LinqMicajahDataContext.InstanceId &&
                         mc1.Status == true &&
                         mc1.MetricCategoryID == MetricCategoryID
                     select mc3 == null ? (mc2 == null ? mc1.MetricCategoryID : mc2.MetricCategoryID) : mc3.MetricCategoryID).First();
            }

            var Metrics =
                from m in dc.Metric
                join _d in dc.D_MetricCategory on
                    new { m.InstanceId, m.MetricCategoryID } equals
                    new { _d.InstanceId, MetricCategoryID = (Guid?)_d.IncludedID } into __d
                from d in __d.DefaultIfEmpty()
                where
                    m.InstanceId == LinqMicajahDataContext.InstanceId &&
                    m.Status == true &&
                    m.MetricDataTypeID == 1 &&

                    ((m.FrequencyID == FrequencyID) || (FrequencyID == -1)) &&

                    ((ParentMetricCategoryID == Guid.Empty)
                        || (m.MetricCategoryID == null && ParentMetricCategoryID == null)
                        || (d.MetricCategoryID == ParentMetricCategoryID))
                orderby m.Name
                select new Metric.Extend
                {
                    MetricID = m.MetricID,
                    Name = m.Name,
                    FormulaCode = m.FormulaCode
                };

            return Metrics.ToList();
        }

        // Main formula method
        public static Guid UpdateMetricFormulaRelations(Guid MetricID, Guid OldMetricFormulaID, string NewFormula, DateTime BeginDate, DateTime? EndDate, string FormulaComment)
        {
            Guid NewFormulaID = OldMetricFormulaID;
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {                
                bool UpdateFlag = false;
                if (OldMetricFormulaID != Guid.Empty)
                {
                    Bll.MetricFormula mfo =
                        (from mf in dc.MetricFormula
                         where mf.InstanceId == LinqMicajahDataContext.InstanceId &&
                             mf.MetricFormulaID == OldMetricFormulaID
                         select mf).FirstOrNull();
                    if (mfo.Formula == NewFormula)
                    { // update formula with new dates
                        mfo.BeginDate = BeginDate;
                        mfo.EndDate = EndDate;
                        mfo.Comment = FormulaComment;
                        mfo.UpdatedBy = LinqMicajahDataContext.LogedUserId;
                        UpdateFlag = true;
                    }
                }

                if (!UpdateFlag)
                {
                    List<string> codes = ParseFormulaCodes(NewFormula);
                    IQueryable<Metric> ms = null;
                    string VariableFormula = NewFormula;
                    if (codes != null)
                    {
                        ms = dc.Metric.Where( m => (codes.Contains(m.FormulaCode) && m.InstanceId == LinqMicajahDataContext.InstanceId));
                        if (ms != null)                        
                            foreach (Metric m in ms)
                            {
                                string var = m.Variable;
                                if (String.IsNullOrEmpty(var))
                                    var = GetGuidAsNumber(m.MetricID);
                                VariableFormula = ReplaceCodeInFormula(VariableFormula, m.FormulaCode, "v" + var);
                            }
                    }

                    Bll.MetricFormula mf = new Bll.MetricFormula();
                    mf.InstanceId = LinqMicajahDataContext.InstanceId;
                    mf.MetricID = MetricID;
                    mf.Formula = NewFormula;
                    mf.VariableFormula = VariableFormula;
                    mf.BeginDate = BeginDate;
                    mf.EndDate = EndDate;
                    mf.Comment = FormulaComment;
                    mf.UpdatedBy = LinqMicajahDataContext.LogedUserId;
                    dc.MetricFormula.InsertOnSubmit(mf);
                    dc.SubmitChanges();

                    NewFormulaID = mf.MetricFormulaID;
                    if (ms != null)
                        foreach (Metric m in ms)
                        {
                            Bll.MetricRelation mr = new Bll.MetricRelation();
                            mr.InstanceId = LinqMicajahDataContext.InstanceId;
                            mr.MetricID = m.MetricID;
                            mr.ReferenceMetricID = MetricID;
                            mr.MetricFormulaID = NewFormulaID;
                            dc.MetricRelation.InsertOnSubmit(mr);
                        }
                    
                }
                dc.SubmitChanges();

                // update metric generation status
                Metric mg =
                    (from m in dc.Metric
                     where
                        m.MetricID == MetricID &&
                        m.InstanceId == LinqMicajahDataContext.InstanceId
                     select m).FirstOrNull();

                int? maxgen =
                     (from mr in dc.MetricRelation
                     join m in dc.Metric on
                        new { mr.InstanceId, mr.MetricID } equals
                        new { m.InstanceId, m.MetricID }
                     where
                         mr.ReferenceMetricID == MetricID &&
                         mr.InstanceId == LinqMicajahDataContext.InstanceId
                     select (int?)m.Generation).Max();

                mg.Generation = maxgen == null ? 0 : ((int)maxgen)+1;
                dc.SubmitChanges();
            }
            return NewFormulaID;
        }        

        // parse forlmulas
        private static List<string> ParseFormulaCodes(string Expression)
        {
            // allowed characters - all codes should start from letter or 'M'            
            if (String.IsNullOrEmpty(Expression)) return null;
            string exs = " " + Expression + " ";
            List<string> l = new List<string>();
            for (int i = 1; i < exs.Length; i++)
                if (aChars.Contains<char>(exs[i - 1]) && !aChars.Contains<char>(exs[i]) && !((exs[i] >= '1') && (exs[i] <= '9')))
                {
                    string code = String.Empty;
                    while ((i < exs.Length) && !aChars.Contains<char>(exs[i]))
                    {
                        code += exs[i];
                        i++;
                    }
                    l.Add(code);
                }
            return l;
        }

        public static void ChangeRelatedFormulas(Guid MetricID, string OldCode, string NewCode)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                IQueryable<MetricFormula> mfs =
                    from mr in dc.MetricRelation
                    join md in dc.Metric on
                        new { mr.InstanceId, MetricID = mr.ReferenceMetricID } equals
                        new { md.InstanceId, md.MetricID }
                    join mf in dc.MetricFormula on
                        new { md.InstanceId, md.MetricID } equals
                        new { mf.InstanceId, mf.MetricID }
                    where
                        mr.MetricID == MetricID &&
                        mr.InstanceId == LinqMicajahDataContext.InstanceId
                    select mf;
                
                foreach (MetricFormula mf in mfs)
                    mf.Formula = ReplaceCodeInFormula(mf.Formula, OldCode, NewCode);
                dc.SubmitChanges();
            }
        }        

        private static string ReplaceCodeInFormula(string Expression, string OldCode, string NewCode)
        {
            // allowed characters - all codes should start from letter or 'M'            
            if (String.IsNullOrEmpty(Expression)) return String.Empty;
            string exs = " " + Expression + " ";            
            for (int i = 1; i < exs.Length; i++)
                if (aChars.Contains<char>(exs[i - 1]) && !aChars.Contains<char>(exs[i]) && !((exs[i] >= '1') && (exs[i] <= '9')))
                {
                    int s = i;
                    string code = String.Empty;
                    while ((i < exs.Length) && !aChars.Contains<char>(exs[i]))
                    {
                        code += exs[i];
                        i++;
                    }
                    if (code == OldCode)
                    {
                        exs = exs.Remove(s, i - s); // check
                        exs = exs.Insert(s, NewCode);
                    }                    
                }
            exs = exs.TrimStart(' ').TrimEnd(' ');
            return exs;
        }

        public static int GenerationCount()
        {
            int? maxgen = null;
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                maxgen =
                 (from m in dc.Metric
                  where
                      m.Status == true
                  select (int?)m.Generation).Max();
            }
            return maxgen == null ? 0 : (int)maxgen;
        }

        // Actual Metric Formula
        public static Bll.MetricFormula GetMetricFormula(Guid MetricID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            Bll.MetricFormula MetricFormula =
                (from mf in dc.MetricFormula
                where mf.InstanceId == LinqMicajahDataContext.InstanceId &&
                    mf.MetricID == MetricID &&
                    mf.Status == true
                 orderby mf.EndDate, mf.BeginDate descending, mf.Created descending, mf.Updated descending
                select mf).Take(1).FirstOrNull();
            if (MetricFormula == null)
                MetricFormula = new MetricFormula
                {
                    MetricFormulaID = Guid.Empty,
                    Formula = String.Empty,
                    VariableFormula = String.Empty,
                    BeginDate = DateTime.Now,
                    EndDate = null
                };
            return MetricFormula;
        }

        public static void ChangeMetricCategory(Guid MetricID, Guid? MCID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                Metric m =
                 (from me in dc.Metric                  
                  where
                     (me.InstanceId == LinqMicajahDataContext.InstanceId)
                     &&
                     (me.MetricID == MetricID)
                     &&
                     (me.Status == true)
                  select me).FirstOrNull();
                if (m != null)
                {
                    m.MetricCategoryID = MCID;
                    dc.SubmitChanges();
                }                
            }
        }

        public static string GetMeasureUnitName(List<Micajah.Common.Bll.MeasureUnit> OrgUoMs, Guid? MeasureUnitID)
        {
            Micajah.Common.Bll.MeasureUnit mu = OrgUoMs.Find(u => u.MeasureUnitId == MeasureUnitID);
            return mu == null ? String.Empty : mu.SingularFullName;
        }

        public static string GetGuidAsNumber(Guid guid)
        {
            byte[] s = guid.ToByteArray();
            ulong u1 = 0;
            ulong u2 = 0;
            for (int i = 0; i <= 7; i++)
                u1 += ((ulong)s[i]) << (8 * (7 - i));
            for ( int i = 8; i <= 15; i++)
                u2 += ((ulong)s[i])<<(8*(15-i));
            string result = u1.ToString() + u2.ToString();
            return result;
        }

        public static void CheckVariablesAndFormulas()
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                IQueryable<Metric> ms =
                    from me in dc.Metric
                    select me;
                foreach (Metric m in ms)
                    if (String.IsNullOrEmpty(m.Variable))
                        m.Variable = GetGuidAsNumber(m.MetricID);
                                
                
                IQueryable<MetricFormula> mfs =
                from mf in dc.MetricFormula                
                select mf;
                
                foreach (MetricFormula mf in mfs)
                    if (String.IsNullOrEmpty(mf.VariableFormula))
                    {
                        string VarFormula = mf.Formula;
                        List<string> codes = ParseFormulaCodes(mf.Formula);
                        if (codes != null)
                        {
                            IQueryable<Metric> mes = dc.Metric.Where(m => codes.Contains(m.FormulaCode));
                            if (mes != null)
                                foreach (Metric m in mes)
                                {
                                    string var = m.Variable;
                                    if (String.IsNullOrEmpty(var))
                                        var = GetGuidAsNumber(m.MetricID);
                                    VarFormula = ReplaceCodeInFormula(VarFormula, m.FormulaCode, "v" + var);
                                }
                        }
                        mf.VariableFormula = VarFormula;
                    }
                dc.SubmitChanges();                
            }
        }

        public static Guid? GetUom(Guid InstanceID ,Guid MetricID, Guid OrgLocatioID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            Guid? UomID = 
                (
                  from u in dc.MetricOrgLocationUoM
                  where u.InstanceId==InstanceID && u.MetricID == MetricID &&
                        (OrgLocatioID == Guid.Empty ? u.OrgLocationID == null : u.OrgLocationID == OrgLocatioID)
                  select u.InputUnitOfMeasureID
                ).FirstOrDefault();
            if (UomID == null)
            {
                UomID = (
                    from m in dc.Metric
                    where m.InstanceId == InstanceID && m.MetricID == MetricID && m.Status == true
                    select m.InputUnitOfMeasureID
                    ).FirstOrDefault();
            }
            return UomID;
        }
    }
}
