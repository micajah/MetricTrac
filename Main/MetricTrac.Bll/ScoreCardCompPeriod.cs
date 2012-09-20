using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class ScoreCardCompPeriod
    {
        public class Extended : ScoreCardCompPeriod
        {
            public string PeriodName { get; set; }
            public string PeriodValue { get; set; }
            public int? FrequencyId { get; set; }
            public bool LastPeriod { get; set; }
            public bool ToDate { get; set; }

            public string CompPeriodName { get; set; }
            public string CompPeriodValue { get; set; }
            public int? CompFrequencyId { get; set; }
            public bool CompLastPeriod { get; set; }
            public bool CompToDate { get; set; }
        }

        static List<ScoreCardCompPeriodType> mAllType;
        public static List<ScoreCardCompPeriodType> AllType
        {
            get
            {
                lock (typeof(ScoreCardCompPeriod))
                {
                    if (mAllType == null)
                    {
                        using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
                        {
                            mAllType = dc.ScoreCardCompPeriodType.ToList();
                        }
                    }
                }
                return mAllType;
            }
        }

        static int? GepPeriodTypeId(string t)
        {
            if (string.IsNullOrEmpty(t)) return null;
            if (t == "c") return null;
            var r = AllType.Where(at => at.Value == t).FirstOrNull();
            if (r == null) return null;
            return r.ScoreCardCompPeriodTypeId;
        }

        public static void SavePeriod(string PeriodType1, string PeriodType2, DateTime? Begin1, DateTime? End1, DateTime? Begin2, DateTime? End2)
        {
            if (LinqMicajahDataContext.LogedUserId == null) return;
            int? Id1 = GepPeriodTypeId(PeriodType1);
            int? Id2 = GepPeriodTypeId(PeriodType2);
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var cp = dc.ScoreCardCompPeriod.Where(p => p.UserId == (Guid)LinqMicajahDataContext.LogedUserId).FirstOrNull();
                bool IsNew = cp == null;
                if (IsNew)
                {
                    cp = new ScoreCardCompPeriod();
                    cp.ScoreCardCompPeriodId = Guid.NewGuid();
                }

                cp.UserId = (Guid)LinqMicajahDataContext.LogedUserId;
                cp.PeriodType1 = Id1;
                cp.PeriodType2 = Id2;
                cp.Begin1 = Begin1;
                cp.End1 = End1;
                cp.Begin2 = Begin2;
                cp.End2 = End2;

                if (IsNew) dc.ScoreCardCompPeriod.InsertOnSubmit(cp);
                dc.SubmitChanges();
            }
        }

        public static Extended GetPeriod()
        {
            if (LinqMicajahDataContext.LogedUserId == null) return null;
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var l = from p in dc.ScoreCardCompPeriod
                        join _t1 in dc.ScoreCardCompPeriodType on p.PeriodType1 equals (int?)_t1.ScoreCardCompPeriodTypeId into __t1
                        join _t2 in dc.ScoreCardCompPeriodType on p.PeriodType2 equals (int?)_t2.ScoreCardCompPeriodTypeId into __t2
                        from t1 in __t1.DefaultIfEmpty()
                        from t2 in __t2.DefaultIfEmpty()
                        where p.UserId == LinqMicajahDataContext.LogedUserId
                        select new Extended()
                        {
                            InstanceId = p.InstanceId,
                            ScoreCardCompPeriodId = p.ScoreCardCompPeriodId,
                            UserId = p.UserId,
                            PeriodType1 = p.PeriodType1,
                            PeriodType2 = p.PeriodType2,
                            Begin1 = p.Begin1,
                            End1 = p.End1,
                            Begin2 = p.Begin2,
                            End2 = p.End2,

                            PeriodName = t1.Name,
                            PeriodValue = t1.Value,
                            //FrequencyId = t1.FrequecyId,
                            //LastPeriod = t1.LastPeriod,
                            //ToDate = t1.ToDate,

                            CompPeriodName = t2.Name,
                            CompPeriodValue = t2.Value,
                            //CompFrequencyId = t2.FrequecyId,
                            //CompLastPeriod = t2.LastPeriod,
                            //CompToDate = t2.ToDate
                        };
                var d = l.FirstOrNull();
                return d;
            }
        }
    }
}
