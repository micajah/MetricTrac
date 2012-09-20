using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class PerformanceIndicatorFormPerformanceIndicatorJunc
    {
        public static IQueryable<PerformanceIndicatorFormPerformanceIndicatorJunc> PIFPIJuncList()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IQueryable<PerformanceIndicatorFormPerformanceIndicatorJunc> junc =
                    from p in dc.PerformanceIndicatorFormPerformanceIndicatorJunc
                    join pif in dc.PerformanceIndicatorForm on
                    new { p.InstanceId, p.PerformanceIndicatorFormID } equals
                    new { pif.InstanceId, pif.PerformanceIndicatorFormID }
                    join pi in dc.PerformanceIndicator on
                    new { p.InstanceId, p.PerformanceIndicatorID } equals
                    new { pi.InstanceId, pi.PerformanceIndicatorID }
                    where p.InstanceId == LinqMicajahDataContext.InstanceId &&
                          p.Status == true &&
                          pif.Status == true &&
                          pi.Status == true
                    select p;
            return junc;
        }
    }
}



