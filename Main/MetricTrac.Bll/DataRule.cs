using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{       
    public partial class DataRule
    {
        public class RuleEntity
        {
            public Guid DataRuleID { get; set; }
            public Guid EntityID { get; set; }            
            public string EntityName { get; set; }
        }

        public class Entity
        {
            public Guid ID { get; set; }
            public string Name { get; set; }

            public Entity(Guid sID, string sName)
            {
                ID = sID;
                Name = sName;
            }
        }

        public class Extend : DataRule
        {
            public string UserName { get; set; }
            public string GroupName { get; set; }
            public string GroupCategoryAspectName { get; set; }

            public string OrgLocationName { get; set; }
            public string PerformanceIndicatorName { get; set; }
            public string MetricName { get; set; }
            
            // edit page
            public Guid?[] DataRuleOrgLocation { get; set; }
            public Guid?[] DataRulePI { get; set; }
            public Guid?[] DataRuleMetric { get; set; }
        }

        private class MoveDataRule
        {
            public DataRule r0 { get; set; }
            public DataRule r1 { get; set; }
        }
    }
}