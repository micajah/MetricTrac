using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Globalization;
using Micajah.Math.Parser;

namespace MetricTrac
{
    public static class PerformanceIndicatorCalc
    {
        private static decimal GetValue(Bll.PerformanceIndicator.Extend pi, DateTime StartDate, DateTime EndDate)
        {
            if (string.IsNullOrEmpty(pi.VariableFormula)) return 0; ;
            string Formula = pi.VariableFormula.Replace(" ","");
            List<string> m;
            List<string> l;
            int n; string s;
            MetricTrac.Bll.PerformanceIndicator.ParseFormulaCodes(pi.VariableFormula, out m, out l);

            foreach (string lid in l)
            {
                Guid OrgLocationID = Guid.Empty;
                if (!string.IsNullOrEmpty(lid) && lid.StartsWith("l"))
                {
                    OrgLocationID = MetricTrac.Bll.PerformanceIndicator.ParseGuidAsNumber(lid.Substring(1));
                }
                while((n=Formula.IndexOf(lid))>0)
                {
                    string func = MetricTrac.Bll.PerformanceIndicator.Functions[0];
                    s = Formula.Substring(n - func.Length, func.Length);
                    if (s != func) return 0;
                    int mn = Formula.IndexOf(",", n);
                    if (mn < 0) return 0;
                    if (mn - n != 17) return 0;
                    mn++;
                    int dn = Formula.IndexOf(")", mn);
                    if (dn < 0) return 0;
                    if (dn - mn != 17) return 0;
                    s = Formula.Substring(mn, dn - mn);
                    Guid MetricID = MetricTrac.Bll.PerformanceIndicator.ParseGuidAsNumber(s);
                    if (MetricID == Guid.Empty) return 0;
                }
            }

            foreach (string mid in m)
            {
                Guid MetricID = Guid.Empty;
                if (!string.IsNullOrEmpty(mid) && mid.StartsWith("m"))
                {
                    MetricID = MetricTrac.Bll.PerformanceIndicator.ParseGuidAsNumber(mid.Substring(1));
                }
            }

            return 0;
        }
        public static void RunProcess(List<Bll.PerformanceIndicator.Extend> PIs, DateTime StartDate, DateTime EndDate)
        {
            MathParser parser = new MathParser();
            foreach (Bll.PerformanceIndicator.Extend pi in PIs)
            {
                pi.FormulaValue=0;
                
            }
            /*int maxgen = Bll.Metric.GenerationCount();
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
                    bool IsMissedInputs = false;
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
                        mv.MissedCalc = IsMissedInputs;
                    }
                    else
                        mv.Value = "-";
                }

                foreach (Bll.MetricValue.Extend mv in _CalcMetricValues)
                    result += "MetricName: " + mv.MetricName + " Date: " + mv.Date.ToShortDateString() + " CalcValue: " + mv.Value + "<br />";

                Bll.MetricValue.SaveCalcValues(_InputMetricValues, _CalcMetricValues);
            }
            Bll.MetricValue.ClearInputValues(maxgen);

            return String.IsNullOrEmpty(result) && String.IsNullOrEmpty(er) ? String.Empty : (result + "<br /><br />" + er);*/
        }


        static MathParser parser;
        public static double CalcStringFormula(List<string> Formulas)
        {
            double val = 0;
            if (Formulas != null)
            {
                if (parser == null) parser = new MathParser();
                foreach (string f in Formulas)
                {
                    if (string.IsNullOrEmpty(f)) continue;
                    string F = f.Replace("\n", "").Replace("\r", "").Replace("\t", "");
                    double v = parser.Parse(F, new System.Collections.Hashtable());
                    if (double.IsInfinity(v) || double.IsNaN(v)) v = 0;
                    val += v;
                }
            }
            return val;
        }
    }
}