using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class DataCollectorRuleOut
    {
        public static DataCollectorRuleOut Get(Guid RuleId)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            return (
                        from o in dc.DataCollectorRuleOut
                        where o.InstanceId == LinqMicajahDataContext.InstanceId && o.RuleId == RuleId && o.Status == true
                        select o
                   ).FirstOrNull();
        }
    }
}
