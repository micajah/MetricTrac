using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class MetricFilter
    {
        public class Extend : MetricFilter
        {
            public Guid?[] OrgLocationsID { get; set; }
        }

        public static IQueryable<MetricFilter> List()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            return from m in dc.MetricFilter 
                   where m.InstanceID==LinqMicajahDataContext.InstanceId && 
                        m.UserID==LinqMicajahDataContext.LogedUserId
                   orderby m.Name
                   select m;
        }

        public override void OnInserting(LinqMicajahDataContext dc, ref bool Cancel)
        {
            this.InstanceID = LinqMicajahDataContext.InstanceId;
            this.UserID = (Guid)LinqMicajahDataContext.LogedUserId;
            base.OnInserting(dc, ref Cancel);
        }

        public static Guid?[] GetDecodedLocations(string sLocations)
        {
            return GetDecodedLocations(sLocations, ',');
        }

        public static Guid?[] GetDecodedLocations(string sLocations, char Divider)
        {
            List<Guid?> Result = new List<Guid?>();
            if (!String.IsNullOrEmpty(sLocations))
            {
                string[] sLocationsArray = sLocations.Split(Divider);
                foreach (string s in sLocationsArray)
                    if (!String.IsNullOrEmpty(s))
                    {
                        try
                        {
                            Guid g = new Guid(s);
                            Result.Add((Guid?)g);
                        }
                        catch { }
                    }
            }
            return Result.Count == 0 ? null : Result.ToArray();
        }
    }
}