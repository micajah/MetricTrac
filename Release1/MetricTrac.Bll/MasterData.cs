using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MetricTrac.Bll
{
    public class MasterData
    {
        public Guid OrgLocationID { get; set; }
        public Guid? FormID { get; set; }
        public Guid? PerformanceIndicatorID { get; set; }
        public Guid? GcaID { get; set; }
        public Guid MetricID { get; set; }
        public Guid? MetricCategoryID { get; set; }
        public bool IsVirtual { get; set; }

        public static IQueryable<MasterData> GetMasterData()
        {               
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IQueryable<MasterData> data =
                    from path in dc.D_MetricOrgLocationPath                    
                    join m in dc.Metric on
                        new { path.InstanceId, path.MetricID } equals
                        new { m.InstanceId, m.MetricID }                    
                    where path.InstanceId == LinqMicajahDataContext.InstanceId                        
                    select new MasterData
                    {
                        OrgLocationID = path.OrgLocationID,
                        FormID = path.PerformanceIndicatorFormID,
                        PerformanceIndicatorID = path.PerformanceIndicatorID,
                        GcaID = path.GroupCategoryAspectID,
                        MetricID = path.MetricID,
                        MetricCategoryID = m.MetricCategoryID,
                        IsVirtual = path.IsVirtual
                    };
            return data;
        }
    }
}
