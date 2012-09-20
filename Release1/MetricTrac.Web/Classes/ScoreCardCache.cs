using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Globalization;

namespace MetricTrac.Utils
{
    public static class ScoreCardCache
    {
        static System.Threading.Thread ProcessLoopThread;
        static System.Timers.Timer ForceTimer;
        static DateTime LastForceTime;
        static DateTime LastProcessTime;

        static List<MetricTrac.Bll.Mc_Instance> Instance;
        //static Guid [] OrganizationId;

        static void SetLowestPriority()
        {
            ProcessLoopThread.Priority = System.Threading.ThreadPriority.Lowest;
        }
        static public void ProcessScoreCardMetric(Guid ScoreCardMetricID)
        {
            ProcessScoreCardMetric(ScoreCardMetricID, MetricTrac.Bll.LinqMicajahDataContext.InstanceId, MetricTrac.Bll.LinqMicajahDataContext.OrganizationId);
        }
        static public void ProcessScoreCardMetric(Guid ScoreCardMetricID, Guid InstanceId, Guid OrganizationId)
        {
            var m = MetricTrac.Bll.ScoreCardMetric.Get(ScoreCardMetricID, InstanceId);
            if (m == null) return;

            double? oldCurrentValue = m.CurrentValue;
            double? oldPreviousValue = m.PreviousValue;
            Guid? oldTotalUnitOfMessureID = m.UomID;

            Guid? TotalUnitOfMessureID;
            double? CurrentValue;
            double? PreviousValue;

            MetricTrac.Bll.ScoreCardMetric.Calculate(m.MetricID, m.OrgLocationID, m.UomID, m.ScoreCardPeriodID, m.MetricFrequencyID, OrganizationId, InstanceId, out TotalUnitOfMessureID, out CurrentValue, out PreviousValue);
            m.CurrentValue = CurrentValue;
            m.PreviousValue = PreviousValue;
            if (oldCurrentValue == m.CurrentValue &&
                oldPreviousValue == m.PreviousValue &&
                oldTotalUnitOfMessureID == TotalUnitOfMessureID) return;

            MetricTrac.Bll.ScoreCardValue.Update(m.ScoreCardMetricID, m.CurrentValue, m.PreviousValue, TotalUnitOfMessureID, InstanceId);
        }
        static public void ProcessScoreCard(Guid ScoreCardID, bool NeedDelete)
        {
            ProcessScoreCard(ScoreCardID, NeedDelete, MetricTrac.Bll.LinqMicajahDataContext.InstanceId, MetricTrac.Bll.LinqMicajahDataContext.OrganizationId);
        }
        static public void ProcessScoreCard(Guid ScoreCardID, bool NeedDelete, Guid InstanceId, Guid OrganizationId)
        {
            var scms = MetricTrac.Bll.ScoreCardMetric.List(ScoreCardID, InstanceId);
            foreach (var m in scms)
            {
                ProcessScoreCardMetric(m.ScoreCardMetricID, InstanceId, OrganizationId);
            }
            //if (NeedDelete) MetricTrac.Bll.ScoreCardValue.DeleteUnused(InstanceId);
        }

        static public void ProcessAll()
        {
            foreach (var inst in Instance)
            {
                var scs = MetricTrac.Bll.ScoreCard.List(inst.InstanceId);
                foreach (var sc in scs)
                {
                    ProcessScoreCard(sc.ScoreCardID, false, inst.InstanceId, inst.OrganizationId);
                }
            }
            LastProcessTime = DateTime.Now;
            //MetricTrac.Bll.ScoreCardValue.DeleteUnused(InstanceId);
            
        }
        static public void ProcessLoop()
        {
            while (true)
            {
                try
                {
                    ProcessAll();                    
                }
                catch { }
                try
                {
                    MetricValuesCalc.ProcessCalc();
                }
                catch { }
                SetLowestPriority();
                System.Threading.Thread.Sleep(3333);
            }
        }
        static public void Start(CultureInfo Culture, CultureInfo UICulture)
        {
            try
            {
                if (ProcessLoopThread != null && ProcessLoopThread.IsAlive) return;
                Instance = MetricTrac.Bll.Mc_Instance.List();
                ProcessLoopThread = new System.Threading.Thread(ProcessLoop);
                ProcessLoopThread.CurrentCulture = Culture;
                ProcessLoopThread.CurrentUICulture = UICulture;
                SetLowestPriority();
                ProcessLoopThread.Start();

                ForceTimer = new System.Timers.Timer(60000*33);
                ForceTimer.Elapsed += new System.Timers.ElapsedEventHandler(ForceTimer_Elapsed);
            }
            catch { }
        }

        static void ForceTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                DateTime DTNow = DateTime.Now;
                if (LastForceTime == DateTime.MinValue)
                {
                    LastForceTime = DTNow;
                    return;
                }

                DateTime DTForce;
                if (DTNow.Hour < 4) DTForce = DTNow.Date.AddDays(-1);
                else DTForce = DTNow.Date;
                DTForce = DTForce.AddHours(4);
                if (LastForceTime > DTForce) return;

                if (LastProcessTime == DateTime.MinValue || (DTNow-LastProcessTime).TotalMinutes>33)
                    ProcessLoopThread.Priority = System.Threading.ThreadPriority.Normal;
            }
            catch { }
        }

        static public void Stop()
        {
            try
            {
                if (ProcessLoopThread == null || !ProcessLoopThread.IsAlive) return;
                ProcessLoopThread.Abort();
                ProcessLoopThread = null;
            }
            catch { }
        }
    }
}
