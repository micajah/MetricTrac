using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class ScoreCardValue
    {
        public static void Update(Guid ScoreCardMetricID, double? newCurrentValue, double? newPreviousValue, Guid? newUnitsOfMeasureId, Guid newInstaceId)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            ScoreCardValue Val = (from v in dc.ScoreCardValue
                                  where v.InstanceId == newInstaceId && v.ScoreCardMetricID == ScoreCardMetricID
                      select v).FirstOrNull();
            if (Val == null)
            {
                Val = new ScoreCardValue();
                dc.ScoreCardValue.InsertOnSubmit(Val);
                Val.ScoreCardMetricID = ScoreCardMetricID;
            }
            Val.InstanceId = newInstaceId;
            Val.CurrentValue = newCurrentValue;
            Val.PreviousValue = newPreviousValue;
            Val.UnitsOfMeasureId = newUnitsOfMeasureId;
            dc.SubmitChanges();
        }

        public static void DeleteUnused(Guid InstanceId)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var dv = from v in dc.ScoreCardValue
                         join _m in dc.ScoreCardMetric on new { InstanceId, v.ScoreCardMetricID, Status = (bool?)true } equals new { _m.InstanceId, _m.ScoreCardMetricID, _m.Status } into __m

                         from m in __m.DefaultIfEmpty()

                         where v.InstanceId == InstanceId && v.Status == true

                         select v;

                bool DelExist = false;
                foreach (var v in dv)
                {
                    v.Status = false;
                }
                if (DelExist) dc.SubmitChanges();
            }
        }
    }
}
