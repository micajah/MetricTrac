using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Threading;
using Micajah.Math.Parser;
using System.Globalization;

namespace MetricTrac
{
    public static class MetricValuesCalc
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
        static public void ProcessLoop()
        {
            while (true)
            {                
                try
                {
                    MetricValuesCalc.ProcessCalc();
                }
                catch { }
                SetLowestPriority();
                System.Threading.Thread.Sleep(18000);
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

                ForceTimer = new System.Timers.Timer(60000 * 33);
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

                if (LastProcessTime == DateTime.MinValue || (DTNow - LastProcessTime).TotalMinutes > 33)
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




        //=============================
        public static void ProcessCalc()
        {
            string CalcResult = String.Empty;
            try
            {
                CalcResult = RunProcess();
            }
            catch (Exception e)
            {
                CalcResult = e.Message + "| Stack: " + e.StackTrace;
            }
            if (!String.IsNullOrEmpty(CalcResult))
                Bll.ApplicationLog.LogAppMessage("Calc process", CalcResult);
        }

        private static string RunProcess()
        {
            Bll.Metric.CheckVariablesAndFormulas();

            int maxgen = Bll.Metric.GenerationCount();
            MathParser parser = new MathParser();            
            string er = String.Empty;
            string result = String.Empty;
            for (int i = 1; i <= maxgen; i++)
            {
                List<Bll.MetricValue.Extend> _CalcMetricValues = null;
                List<Bll.MetricValue.Extend> _InputMetricValues = Bll.MetricValue.MetricValuesForCalculation(i, out _CalcMetricValues);
                List<Bll.Mc_Instance> Instances = MetricTrac.Bll.Mc_Instance.List();
                                
                foreach (Bll.MetricValue.Extend mv in _CalcMetricValues)
                {
                    Guid? CalcOutputUoM = mv.UnitOfMeasureID = mv.MetricUnitOfMeasureID;
                    mv.InputUnitOfMeasureID = mv.MetricInputUnitOfMeasureID;
                    Guid OrganizationId = Instances.Where(r => r.InstanceId == mv.InstanceId).SingleOrDefault().OrganizationId;
                    List<Bll.MetricValue.Extend> ActualInputMetricValues = _InputMetricValues.Where(r => (r.RelatedFormulaID == mv.RelatedFormulaID) && (r.Date == mv.Date) && (r.OrgLocationID == mv.OrgLocationID)).ToList();
                    Hashtable h = new Hashtable();
                    /*bool IsMissedInputs = false;
                    bool IsConverted = true;
                    foreach (Bll.MetricValue.Extend mvi in ActualInputMetricValues)
                    {
                        Guid? CurOutputUoM = mvi.UnitOfMeasureID;
                        if (CalcOutputUoM != CurOutputUoM)
                        {
                            if (CalcOutputUoM != null && CurOutputUoM != null)
                            {
                                List<Micajah.Common.Bll.MeasureUnit> l = Bll.Mc_UnitsOfMeasure.GetConvertedUoMs(CalcOutputUoM, OrganizationId);
                                Micajah.Common.Bll.MeasureUnit mu = Micajah.Common.Bll.MeasureUnit.Create((Guid)CurOutputUoM, OrganizationId);
                                if (!l.Contains(mu))
                                    IsConverted = false;
                            }
                            else
                                IsConverted = false;
                        }
                        if (IsConverted)
                        {
                            string var = mvi.Variable;
                            if (String.IsNullOrEmpty(var))
                                var = Bll.Metric.GetGuidAsNumber(mvi.MetricID);

                            string PreValue = "0";
                            if (mvi.ConvertedValue != "-" && !String.IsNullOrEmpty(mvi.ConvertedValue))
                            {
                                PreValue = mvi.ConvertedValue;
                                if (CalcOutputUoM != CurOutputUoM)
                                    PreValue = Bll.Mc_UnitsOfMeasure.ConvertValue(PreValue, (Guid)CurOutputUoM, (Guid)CalcOutputUoM, OrganizationId);
                            }
                            else
                                IsMissedInputs = true;
                            h.Add("v" + var, PreValue);
                            if (mvi.MetricValueID == Guid.Empty)
                                IsMissedInputs = true;
                        }
                    }*/
                    bool IsInputsWithSubMissed = false;
                    bool IsMissedInputs = false;
                    bool IsConverted = true;
                    foreach (Bll.MetricValue.Extend mvi in ActualInputMetricValues)
                    {
                        Guid? CurInputUoM = mvi.InputUnitOfMeasureID;
                        if (CalcOutputUoM != CurInputUoM)
                        {
                            if (CalcOutputUoM != null && CurInputUoM != null)
                            {
                                List<Micajah.Common.Bll.MeasureUnit> l = Bll.Mc_UnitsOfMeasure.GetConvertedUoMs(CalcOutputUoM, OrganizationId);
                                Micajah.Common.Bll.MeasureUnit mu = Micajah.Common.Bll.MeasureUnit.Create((Guid)CurInputUoM, OrganizationId);
                                if (!l.Contains(mu))
                                    IsConverted = false;
                            }
                            else
                                IsConverted = false;
                        }
                        if (IsConverted)
                        {
                            string var = mvi.Variable;
                            if (String.IsNullOrEmpty(var))
                                var = Bll.Metric.GetGuidAsNumber(mvi.MetricID);

                            string PreValue = "0";
                            if (mvi.Value != "-" && !String.IsNullOrEmpty(mvi.Value))
                            {
                                PreValue = mvi.Value;
                                if (CalcOutputUoM != CurInputUoM && CalcOutputUoM != null && CurInputUoM != null)
                                    PreValue = Bll.Mc_UnitsOfMeasure.ConvertValue(PreValue, (Guid)CurInputUoM, (Guid)CalcOutputUoM, OrganizationId);
                            }
                            else
                                IsMissedInputs = true;
                            h.Add("v" + var, PreValue);
                            if (mvi.MetricValueID == Guid.Empty)
                                IsMissedInputs = true;
                            if (mvi.MissedCalc)
                                IsInputsWithSubMissed = true;
                        }
                    }

                    if (IsConverted)
                    {
                        string StartValue = null;
                        try
                        {
                            StartValue = parser.Parse(mv.VariableFormula.ToLower().Trim(), h).ToString();
                        }
                        catch (Exception e)
                        {
                            er += e.Message + "<br />";
                            StartValue = "-";
                        }
                        mv.Value = StartValue == "Infinity" ? "-" : StartValue;                        
                        mv.MissedCalc = IsMissedInputs || IsInputsWithSubMissed;
                    }
                    else
                        mv.Value = "-";
                }
                                
                foreach (Bll.MetricValue.Extend mv in _CalcMetricValues)
                    result += "MetricName: " + mv.MetricName + " Date: " + mv.Date.ToShortDateString() + " CalcValue: " + mv.Value + "<br />";

                Bll.MetricValue.SaveCalcValues(_InputMetricValues, _CalcMetricValues);
            }
            Bll.MetricValue.ClearInputValues(maxgen);

            return String.IsNullOrEmpty(result) && String.IsNullOrEmpty(er) ? String.Empty : (result + "<br /><br />" + er);
        }
    }
}
