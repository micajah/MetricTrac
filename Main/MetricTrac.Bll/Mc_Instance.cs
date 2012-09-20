using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class Mc_Instance
    {
        public static List<Mc_Instance> List()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var r = (from i in dc.Mc_Instance where !i.Deleted select i).ToList();
            return r;
        }
    }
}
