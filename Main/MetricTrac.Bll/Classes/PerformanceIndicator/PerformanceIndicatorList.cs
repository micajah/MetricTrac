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
    public partial class PerformanceIndicator
    {
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
                //join _g in dc.GCAFullNameView on new { pi.InstanceId, pi.GroupCategoryAspectID } equals new { _g.InstanceId, GroupCategoryAspectID = (Guid?)_g.GroupCategoryAspectID } into __g
                join _gn in dc.ViewnameGroupCategoryAspect on new { InstanceId = (Guid?)pi.InstanceId, pi.GroupCategoryAspectID } equals new { _gn.InstanceId, _gn.GroupCategoryAspectID } into __gn
                join _gh in dc.ViewHierarchyGroupCategoryAspect on new { InstanceId = (Guid?)pi.InstanceId, pi.GroupCategoryAspectID } equals new { _gh.InstanceId, GroupCategoryAspectID = (Guid?)_gh.SubGroupCategoryAspectID } into __gh
                join _r in dc.Requirement on pi.RequirementID equals _r.RequirementID into __r

                from s in __s.DefaultIfEmpty()

                from gn in __gn.DefaultIfEmpty()
                from gh in __gh.DefaultIfEmpty()

                from r in __r.DefaultIfEmpty()

                where
                    (pi.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (pi.Status == true)
                    &&
                    // new criterias
                    (SectorFilter == -1 || SectorFilter == pi.SectorID)
                    &&
                    (GcaFilter == Guid.Empty || (Guid)gh.GroupCategoryAspectID == GcaFilter)// (g.GroupCategoryAspectID == GcaFilter || g.Parent1 == GcaFilter || g.Parent2 == GcaFilter))

                orderby pi.SortCode, pi.Code, gn.FullName, pi.GroupCategoryAspectID, pi.Name

                select new Extend
                {
                    InstanceId = pi.InstanceId,
                    PerformanceIndicatorID = pi.PerformanceIndicatorID,
                    Name = pi.Name.TrimEnd(),
                    Description = pi.Description,
                    Code = pi.Code,

                    GroupCategoryAspectID = pi.GroupCategoryAspectID,
                    SectorID = pi.SectorID,
                    RequirementID = pi.RequirementID,

                    GCAName = gn.FullName,
                    SectorName = s.Name,
                    RequirementName = r.Name,

                    Created = pi.Created,
                    Updated = pi.Updated,
                    Status = pi.Status
                };

            return result.Distinct().ToList();
        }
    }
}
