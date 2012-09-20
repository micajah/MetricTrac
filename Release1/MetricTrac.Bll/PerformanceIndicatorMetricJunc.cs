using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class PerformanceIndicatorMetricJunc
    {
        public static void Insert(Guid PerformanceIndicatorID, Guid MetricID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                PerformanceIndicatorMetricJunc PiMJ = 
                    (
                    from j in dc.PerformanceIndicatorMetricJunc 
                    where 
                        j.InstanceId == LinqMicajahDataContext.InstanceId &&
                        j.PerformanceIndicatorID == PerformanceIndicatorID && 
                        j.MetricID == MetricID 
                    select j
                    ).FirstOrNull();

                if (PiMJ == null)
                {
                    PiMJ = new PerformanceIndicatorMetricJunc();
                    PiMJ.PerformanceIndicatorID = PerformanceIndicatorID;
                    PiMJ.MetricID = MetricID;

                    dc.PerformanceIndicatorMetricJunc.InsertOnSubmit(PiMJ);
                }
                else
                {
                    PiMJ.Status = true;
                }

                dc.SubmitChanges();
            }
        }

        public static void Delete(Guid PerformanceIndicatorID, Guid MetricID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                PerformanceIndicatorMetricJunc PiMJ =
                    (
                    from j in dc.PerformanceIndicatorMetricJunc
                    where
                        j.InstanceId == LinqMicajahDataContext.InstanceId &&
                        j.PerformanceIndicatorID == PerformanceIndicatorID &&
                        j.MetricID == MetricID
                    select j
                    ).FirstOrNull();

                if (PiMJ != null)
                {
                    PiMJ.Status = false;
                    dc.SubmitChanges();
                }
            }
        }
    }
}
