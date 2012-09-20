using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Data;

// define bll nicks for sp results;
using PIMetricInfo = MetricTrac.Bll.Sp_SelectPIReportValuesResult1;
using PIMetricValue = MetricTrac.Bll.Sp_SelectPIReportValuesResult2;

namespace MetricTrac.Bll
{
    public enum PerformanceIndicatorListMode { List, FormJunc, FormSelect }
    public partial class PerformanceIndicator
    {
        public sealed class Extend : PerformanceIndicator
        {
            public string GCAName {get; set;}
            public string SectorName {get; set;}
            public string RequirementName {get; set;}
            public bool IsVirtual  {get; set;}
        }
       

        public static List<Extend> List()
        {
            return List(Guid.Empty, -1);
        }

        public static List<Extend> List(Guid GcaFilter, int SectorFilter)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            var result =
                from pi in dc.PerformanceIndicator
                join _s in dc.Sector on pi.SectorID equals _s.SectorID into __s
                join _g in dc.GCAFullNameView on new { pi.InstanceId, pi.GroupCategoryAspectID } equals new { _g.InstanceId, GroupCategoryAspectID = (Guid?)_g.GroupCategoryAspectID } into __g
                join _r in dc.Requirement on pi.RequirementID equals _r.RequirementID into __r

                from s in __s.DefaultIfEmpty()

                from g in __g.DefaultIfEmpty()

                from r in __r.DefaultIfEmpty()

                where
                    (pi.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (pi.Status == true)
                    &&
                    // new criterias
                    (SectorFilter == -1 || SectorFilter == pi.SectorID)
                    &&
                    (GcaFilter == Guid.Empty || (g.GroupCategoryAspectID == GcaFilter || g.Parent1 == GcaFilter || g.Parent2 == GcaFilter))

                orderby pi.SortCode, pi.Code, g.FullName, g.GroupCategoryAspectID, pi.Name

                select new Extend
                {
                    InstanceId = pi.InstanceId,
                    PerformanceIndicatorID = pi.PerformanceIndicatorID,
                    Name = pi.Name.TrimEnd(),
                    Description = pi.Description,
                    Code = pi.Code,

                    GroupCategoryAspectID = g.GroupCategoryAspectID,
                    SectorID = pi.SectorID,
                    RequirementID = pi.RequirementID,

                    GCAName = g.FullName,
                    SectorName = s.Name,
                    RequirementName = r.Name,

                    Created = pi.Created,
                    Updated = pi.Updated,
                    Status = pi.Status
                };

            return result.ToList();
        }

        public static Extend Get(Guid PerformanceIndicatorID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            var result =
                (from pi in dc.PerformanceIndicator
                join _s in dc.Sector on pi.SectorID equals _s.SectorID into __s
                join _g1 in dc.GroupCategoryAspect on new { pi.InstanceId, pi.GroupCategoryAspectID } equals new { _g1.InstanceId, GroupCategoryAspectID = (Guid?)_g1.GroupCategoryAspectID } into __g1
                join _r in dc.Requirement on pi.RequirementID equals _r.RequirementID into __r

                from s in __s.DefaultIfEmpty()

                from g1 in __g1.DefaultIfEmpty()
                join _g2 in dc.GroupCategoryAspect on new { g1.InstanceId, g1.ParentId } equals new { _g2.InstanceId, ParentId = (Guid?)_g2.GroupCategoryAspectID } into __g2

                from g2 in __g2.DefaultIfEmpty()
                join _g3 in dc.GroupCategoryAspect on new { g2.InstanceId, g2.ParentId } equals new { _g3.InstanceId, ParentId = (Guid?)_g3.GroupCategoryAspectID } into __g3

                from g3 in __g3.DefaultIfEmpty()

                from r in __r.DefaultIfEmpty()

                where
                    (pi.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (pi.PerformanceIndicatorID == PerformanceIndicatorID)                    
                select new Extend
                {
                    InstanceId = pi.InstanceId,
                    PerformanceIndicatorID = pi.PerformanceIndicatorID,
                    Name = pi.Name,
                    Description = pi.Description,
                    Help = pi.Help,
                    Code = pi.Code,

                    GroupCategoryAspectID = pi.GroupCategoryAspectID != null ? (g1.Status == true ? pi.GroupCategoryAspectID : null) : null,
                    SectorID = pi.SectorID,
                    RequirementID = pi.RequirementID,

                    GCAName = ((g3.Name == null) ? "" : g3.Name + "&nbsp;>&nbsp;") + ((g2.Name == null) ? "" : g2.Name + "&nbsp;>&nbsp;") + ((g1.Name == null) ? "" : g1.Name),
                    SectorName = s.Name,
                    RequirementName = r.Name,

                    Created = pi.Created,
                    Updated = pi.Updated,
                    Status = pi.Status
                }).FirstOrNull();

            return result;
        }

        public override void OnDeleting(LinqMicajahDataContext c, ref bool Cancel)
        {
            base.OnDeleting(c, ref Cancel);
            Cancel = true;

            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var js = from j in dc.PerformanceIndicatorMetricJunc
                         where j.InstanceId == LinqMicajahDataContext.InstanceId &&
                               j.Status == true &&
                               j.PerformanceIndicatorID == PerformanceIndicatorID
                         select j;
                foreach (var j in js) j.Status = false;

                var rets =
                    from pifj in dc.PerformanceIndicatorFormPerformanceIndicatorJunc
                    where (pifj.InstanceId == LinqMicajahDataContext.InstanceId) && (pifj.PerformanceIndicatorID == PerformanceIndicatorID)
                    select pifj;
                foreach (var pifj in rets) pifj.Status = false;

                PerformanceIndicator ret =
                         (from pi in dc.PerformanceIndicator
                          where (pi.InstanceId == LinqMicajahDataContext.InstanceId) && (pi.PerformanceIndicatorID == PerformanceIndicatorID)
                          select pi).FirstOrNull();
                if (ret != null) ret.Status = false;

                dc.SubmitChanges();
            }
        }

        // Performance Indicator Report section
        public static IList<PIMetricInfo> GetPIReportValues(Guid? OrgLocationID, Guid GroupID, int FrequencyID, Guid? OrgLocationTypeID, DateTime BeginDate, DateTime EndDate)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IMultipleResults results = dc.Sp_SelectPIReportValues(LinqMicajahDataContext.InstanceId, OrgLocationID, GroupID, FrequencyID, OrgLocationTypeID, BeginDate, EndDate);
            IList<PIMetricInfo> MetricInfo = results.GetResult<PIMetricInfo>().ToList();
            IList<PIMetricValue> MetricValues = results.GetResult<PIMetricValue>().ToList();
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();           
            foreach (PIMetricInfo mi in MetricInfo)
            {
                mi.LocationFullName = (OrgLocationID == Guid.Empty && !String.IsNullOrEmpty(mi.LocationFullName)) ? LinqMicajahDataContext.OrganizationName : mi.LocationFullName;

                mi.InputUoMName = GetMeasureUnitPluralName(OrgUoMs, mi.InputUnitOfMeasureID);
                mi.UoMName = GetMeasureUnitPluralName(OrgUoMs, mi.UnitOfMeasureID);
                CompositeDailyValues cdv = new CompositeDailyValues();
                foreach (PIMetricValue mv in MetricValues)
                {
                    decimal Val = 0;
                    if (mv.MetricID == mi.MetricID && mv.OrgLocationID == mi.OrgLocationID) // add one more linq select
                        if ((mv.ValueMetricDataTypeID == 1) && (mv.UnitOfMeasureID == mv.MetricUnitOfMeasureID)) 
                            // !!! for now we ignore values when its output uom != metric output uom
                            if (decimal.TryParse(mv.ConvertedValue, out Val))
                                cdv.AddCompositeDailyValue((DateTime)mv.InputDate, Val, (int)mv.FrequencyID, (DateTime)mv.Date, (int)mi.FrequencyID);
                        
                }
                decimal? res = cdv.GetCompositeValue(BeginDate, EndDate, false);
                mi.SumValue = res == null ? "0" : res.ToString();                        
            }
            return MetricInfo;
        }

        public static IQueryable<Metric.MetricPIJunc> PIMetricJuncList()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IQueryable<Metric.MetricPIJunc> metrics =
                    from pimj in dc.PerformanceIndicatorMetricJunc
                    join m in dc.Metric on                    
                    new { pimj.InstanceId, pimj.MetricID } equals
                    new { m.InstanceId, m.MetricID }
                    join pi in dc.PerformanceIndicator on
                    new { pimj.InstanceId, pimj.PerformanceIndicatorID } equals
                    new { pi.InstanceId, pi.PerformanceIndicatorID }
                    where m.InstanceId == LinqMicajahDataContext.InstanceId &&
                          m.Status == true &&
                          pimj.Status == true &&
                          pi.Status == true
                    select new Metric.MetricPIJunc
                    {
                        MetricID = pimj.MetricID,
                        Name = m.Name,
                        PerformanceIndicatorID = pimj.PerformanceIndicatorID
                    };
            return metrics;
        }

        public static string GetMeasureUnitPluralName(List<Micajah.Common.Bll.MeasureUnit> OrgUoMs, Guid? MeasureUnitID)
        {
            Micajah.Common.Bll.MeasureUnit mu = OrgUoMs.Find(u => u.MeasureUnitId == MeasureUnitID);
            return mu == null ? String.Empty : mu.PluralFullName;
        }

        public static DataView GetOrgLocationTypes()
        {
            Micajah.Common.Dal.OrganizationDataSet.EntityNodeTypeDataTable dt = Micajah.Common.Bll.Providers.EntityNodeProvider.GetCustomEntityNodeTypesByEntityId(LinqMicajahDataContext.OrganizationId, null, new Guid("4cda22f3-4f01-4768-8608-938dc6a06825"));
            dt.DefaultView.Sort = "OrderNumber";
            return dt.DefaultView;    
        }
    }    
}
