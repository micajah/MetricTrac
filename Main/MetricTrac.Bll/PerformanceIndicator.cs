using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.Text;
using System.Data;

// define bll nicks for sp results;
using PIMetricInfo = MetricTrac.Bll.Sp_SelectPIReportValuesResult1;
using PIMetricValue = MetricTrac.Bll.Sp_SelectPIReportValuesResult2;

namespace MetricTrac.Bll
{
    public enum PerformanceIndicatorListMode { List, FormJunc, FormSelect }
    public partial class PerformanceIndicator
    {
        public sealed class Extend : PerformanceIndicator
        {
            public string GCAName {get; set;}
            public string SectorName {get; set;}
            public string RequirementName {get; set;}
            public bool IsVirtual  {get; set;}

            public Guid? GroupID { get; set; }
            public Guid? CategoryID { get; set; }
            public Guid? AspectID { get; set; }
            public String GroupName { get; set; }
            public String CategoryName { get; set; }
            public String AspectName { get; set; }

            public Guid? PerformanceIndicatorFomulaID {get; set;}
            public string Formula {get; set;}
            public DateTime? FormulaBeginDate { get; set; }
            public DateTime? FormulaEndDate { get; set; }
            public DateTime? FormulaCreated { get; set; }
            public DateTime? FormulaUpdated { get; set; }
            public Guid? FormulaUpdatedBy { get; set; }
            public string FormulaComment { get; set; }
            public string VariableFormula {get; set;}

            public decimal FormulaValue { get; set; }
            public string UnitOfMessureName { get; set; }
            public string AltUnitOfMessureName { get; set; }
            public string FrequencyName { get; set; }
            public List<Guid> OrgLocationID { get; set; }
            public List<string> OrgLocationFullName { get; set; }

            public Extend()
            {
                OrgLocationID = new List<Guid>();
                OrgLocationFullName = new List<string>();
            }
        }
        public static char[] aChars = { ' ', '+', '-', '*', '/', '(', ')', '.', ',' };
        public static string[] SimpleFunctions = new string[] {"Sum", "Average", "RMS"};
        public static string[] Functions = new string[] {
            "ReportSub_Sum(", "ReportSub_Average(", "ReportSub_RMS(",
            "ReportTop_Sum(","ReportTop_Average(","ReportTop_RMS(",
            "SelectedSub_Sum(","SelectedSub_Average(","SelectedSub_RMS(",
            "SelectedTop_Sum(","SelectedTop_Average(","SelectedTop_RMS("
        };
        public static void ParseFormulaCodes(string Expression, out List<string> Metrics, out List<string> Locations)
        {
            // allowed characters - all codes should start from letter or 'M'            
            Metrics = new List<string>();
            Locations = new List<string>();
            if (String.IsNullOrEmpty(Expression)) return;

            int Pos = 0;
            while (true)
            {
                int n = int.MaxValue;
                string f="";                

                for (int i = 0; i < Functions.Length; i++)
                {
                    int N = Expression.IndexOf(Functions[i], Pos);
                    if (N >= 0 && N < n)
                    {
                        n = N;
                        f = Functions[i];
                    }
                }
                if (n == int.MaxValue) break;
                int end = Expression.IndexOf(")", n + 1);
                Pos = end;
                if (end < 0) break;
                string arg = Expression.Substring(n + f.Length, end - f.Length - n).Trim();
                int CommaPos = arg.IndexOf(",");
                if (CommaPos >= 0)
                {
                    string location = arg.Substring(0, CommaPos).Trim();
                    if (!Locations.Contains(location)) Locations.Add(location);
                    string metric = arg.Substring(CommaPos + 1).Trim();
                    if (!Metrics.Contains(metric)) Metrics.Add(metric);
                }
                else
                {
                    if (!Metrics.Contains(arg)) Metrics.Add(arg);
                }
            }
        }
        public static string GetGuidAsNumber(Guid guid)
        {
            string r = guid.ToString();
            r = r.Replace("-", "");
            return r;
        }
        public static Guid ParseGuidAsNumber(string g)
        {
            //{01234567-8901-2345-6789-012345678901}
            if (string.IsNullOrEmpty(g) || g.Length != 32) return Guid.Empty;
            string s = g.Substring(0, 8) + "-" + g.Substring(8, 4) + "-" + g.Substring(12, 4) + "-" + g.Substring(16, 4) + "-" + g.Substring(20);
            return new Guid(s);
        }
        public static Guid ParseGuidAsNumber(string g, string prefix)
        {
            if (string.IsNullOrEmpty(g) || !g.StartsWith(prefix)) return Guid.Empty;
            g = g.Substring(prefix.Length);
            return ParseGuidAsNumber(g);
        }
        private static string ReplaceCodeInFormula(string Expression, string OldCode, string NewCode)
        {
            // allowed characters - all codes should start from letter or 'M'            
            if (String.IsNullOrEmpty(Expression)) return String.Empty;
            string exs = " " + Expression + " ";
            for (int i = 1; i < exs.Length; i++)
                if (aChars.Contains<char>(exs[i - 1]) && !aChars.Contains<char>(exs[i]) && !((exs[i] >= '1') && (exs[i] <= '9')))
                {
                    int s = i;
                    string code = String.Empty;
                    while ((i < exs.Length) && !aChars.Contains<char>(exs[i]))
                    {
                        code += exs[i];
                        i++;
                    }
                    if (code == OldCode)
                    {
                        exs = exs.Remove(s, i - s); // check
                        exs = exs.Insert(s, NewCode);
                    }
                }
            exs = exs.TrimStart(' ').TrimEnd(' ');
            return exs;
        }
        public static Guid UpdatePIFormulaRelations(LinqMicajahDataContext dc, Guid PIID, Guid OldPIFormulaID, string NewFormula, DateTime BeginDate, DateTime? EndDate, string FormulaComment)
        {
            Guid NewFormulaID = OldPIFormulaID;
            bool UpdateFlag = false;
            DateTime DTToday = DateTime.Today;

            if (OldPIFormulaID != Guid.Empty)
            {
                Bll.PerformanceIndicatorFormula pifo =
                    (from pif in dc.PerformanceIndicatorFormula
                     where pif.InstanceId == LinqMicajahDataContext.InstanceId &&
                         pif.PerformanceIndicatorFomulaID == OldPIFormulaID
                     select pif).FirstOrNull();
                if (pifo.Formula == NewFormula)
                { // update formula with new dates
                    pifo.BeginDate = BeginDate;
                    pifo.EndDate = EndDate;
                    pifo.Comment = FormulaComment;
                    pifo.UpdatedBy = LinqMicajahDataContext.LogedUserId;
                    pifo.Updated = DateTime.Now;
                    UpdateFlag = true;
                    dc.SubmitChanges();
                }
            }

            if (!UpdateFlag && (NewFormula==null || NewFormula.Trim()=="") && BeginDate==DateTime.MinValue)
            {
                Bll.PerformanceIndicatorFormula pifo =
                    (from pif in dc.PerformanceIndicatorFormula
                     where pif.InstanceId == LinqMicajahDataContext.InstanceId &&
                         DTToday >= pif.BeginDate && (pif.EndDate == null || DTToday<=pif.EndDate)
                     select pif).FirstOrNull();
                if (pifo == null) UpdateFlag = true;
            }

            if (!UpdateFlag)
            {
                List<string> Metrics;
                List<string> Locations;
                ParseFormulaCodes(NewFormula, out Metrics, out Locations);
                List<Metric> ms = null;
                string VariableFormula = NewFormula;

                ms = dc.Metric.Where(m => (Metrics.Contains(m.FormulaCode) && m.InstanceId == LinqMicajahDataContext.InstanceId && m.Status == true)).ToList();
                foreach (Metric m in ms)
                {
                    string var = GetGuidAsNumber(m.MetricID);
                    VariableFormula = ReplaceCodeInFormula(VariableFormula, m.FormulaCode, "m" + var);
                }

                var loc = (
                    from v in dc.ViewnameOrgLocation 
                    where v.InstanceId==LinqMicajahDataContext.InstanceId &&
                        Locations.Contains(v.FullName.Replace("->", "_").Replace("+", "_").Replace("-", "_").Replace("/", "_").Replace("*", "_").Replace("(", "_").Replace(")", "_").Replace(".", "_").Replace(",", "_").Replace(" ", "_")) 
                    select v
                    ).ToList();

                foreach (var l in loc)
                {
                    string var = "l" + GetGuidAsNumber((Guid)l.OrgLocationID);
                    string LocAlias = l.FullName.Replace("->", "_").Replace("+", "_").Replace("-", "_").Replace("/", "_").Replace("*", "_").Replace("(", "_").Replace(")", "_").Replace(".", "_").Replace(",", "_").Replace(" ", "_");
                    VariableFormula = ReplaceCodeInFormula(VariableFormula, LocAlias, var);
                }

                Bll.PerformanceIndicatorFormula pif = new Bll.PerformanceIndicatorFormula();
                pif.InstanceId = LinqMicajahDataContext.InstanceId;
                pif.PerformanceIndicatorID = PIID;
                pif.Formula = NewFormula;
                pif.VariableFormula = VariableFormula;
                pif.BeginDate = BeginDate;
                pif.EndDate = EndDate;
                pif.Comment = FormulaComment;
                pif.UpdatedBy = LinqMicajahDataContext.LogedUserId;
                pif.Updated = DateTime.Now;
                pif.PerformanceIndicatorFomulaID = Guid.NewGuid();

                dc.PerformanceIndicatorFormula.InsertOnSubmit(pif);
                dc.SubmitChanges();

                NewFormulaID = pif.PerformanceIndicatorFomulaID;

                foreach (Metric m in ms)
                {
                    Bll.PerformanceIndicatorFormulaRelation r = new PerformanceIndicatorFormulaRelation();
                    r.InstanceId = LinqMicajahDataContext.InstanceId;
                    r.PerformanceIndicatorFormulaRelationID = Guid.NewGuid();
                    r.PerformanceIndicatorFormulaID = NewFormulaID;
                    r.MetricID = m.MetricID;
                    r.OrgLocationID = null;
                    dc.PerformanceIndicatorFormulaRelation.InsertOnSubmit(r);
                }

                foreach (var l in loc)
                {
                    Bll.PerformanceIndicatorFormulaRelation r = new PerformanceIndicatorFormulaRelation();
                    r.InstanceId = LinqMicajahDataContext.InstanceId;
                    r.PerformanceIndicatorFormulaRelationID = Guid.NewGuid();
                    r.PerformanceIndicatorFormulaID = NewFormulaID;
                    r.OrgLocationID = l.OrgLocationID;
                    r.MetricID = null;
                    dc.PerformanceIndicatorFormulaRelation.InsertOnSubmit(r);
                }
                dc.SubmitChanges();
            }


            return NewFormulaID;
        } 

        public static string Save(Extend pi)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                DateTime DTNow = DateTime.Now;
                PerformanceIndicator NewPi;

                Guid ExistPIID = (
                    from p in dc.PerformanceIndicator 
                    where p.InstanceId == LinqMicajahDataContext.InstanceId && 
                        (pi.PerformanceIndicatorID==Guid.Empty || p.PerformanceIndicatorID != pi.PerformanceIndicatorID) &&
                        p.Name==pi.Name
                    select p.PerformanceIndicatorID
                    ).Take(1).FirstOrDefault();

                if (ExistPIID != Guid.Empty) return "\"" + pi.Name + "\" Performance Indicator already exist.";

                if (!string.IsNullOrEmpty(pi.Formula) && pi.FormulaBeginDate != null && pi.FormulaEndDate != null && pi.FormulaEndDate < pi.FormulaEndDate)
                    return "Formula start date should be less than end one.";


                if (!string.IsNullOrEmpty(pi.Formula) && pi.FormulaBeginDate == null)
                    return "Start Date is required for calculated Performance Indicator.";



                if (pi.PerformanceIndicatorID == Guid.Empty)
                {
                    NewPi = new PerformanceIndicator()
                    {
                        Alias = pi.Alias,
                        Code = pi.Code,
                        Created = DateTime.Now,
                        Description = pi.Description,
                        GroupCategoryAspectID = pi.GroupCategoryAspectID,
                        Help = pi.Help,
                        InstanceId = LinqMicajahDataContext.InstanceId,
                        Name = pi.Name,
                        RequirementID = pi.RequirementID,
                        SectorID = pi.SectorID,
                        SortCode = pi.SortCode,
                        Status = true,
                        UnitOfMeasureID=pi.UnitOfMeasureID,
                        AltUnitOfMeasureID = pi.AltUnitOfMeasureID,
                        DecimalPlaces = pi.DecimalPlaces
                    };
                    dc.PerformanceIndicator.InsertOnSubmit(NewPi);
                    dc.SubmitChanges();
                    pi.PerformanceIndicatorID=NewPi.PerformanceIndicatorID;
                }
                else
                {
                    NewPi = dc.PerformanceIndicator.Where(PI => PI.InstanceId == LinqMicajahDataContext.InstanceId && PI.PerformanceIndicatorID == pi.PerformanceIndicatorID).FirstOrNull();
                    if (NewPi == null) return "Can not find this Performance Indicator for update. Maybe this Performance Indicator was deleted.";
                    NewPi.Alias = pi.Alias;
                    NewPi.Code = pi.Code;
                    NewPi.Description = pi.Description;
                    NewPi.GroupCategoryAspectID = pi.GroupCategoryAspectID;
                    NewPi.Help = pi.Help;
                    NewPi.Name = pi.Name;
                    NewPi.RequirementID = pi.RequirementID;
                    NewPi.SectorID = pi.SectorID;
                    NewPi.SortCode = pi.SortCode;
                    NewPi.UnitOfMeasureID = pi.UnitOfMeasureID;
                    NewPi.AltUnitOfMeasureID = pi.AltUnitOfMeasureID;
                    NewPi.DecimalPlaces = pi.DecimalPlaces;
                    dc.SubmitChanges();





                    /*
                    if (e.NewValues["MetricTypeID"].ToString() == "2")
                    {
                        // update related metric values
                        Bll.MetricValue.MakeFormulaRelatedInputsDirty(OldFormulaID, NewFormulaID);
                        // run calc process
                        //MetricValuesCalc.ProcessCalc();
                    }

                     */

                }

                DateTime BeginDate = DateTime.MinValue;
                if (pi.FormulaBeginDate != null) BeginDate = (DateTime)pi.FormulaBeginDate;
                UpdatePIFormulaRelations(dc, pi.PerformanceIndicatorID, pi.PerformanceIndicatorFomulaID == null ? Guid.Empty : (Guid)pi.PerformanceIndicatorFomulaID, pi.Formula, BeginDate, pi.FormulaEndDate, pi.FormulaComment);
            }
            return null;
        }

        public static void Delete(Guid PerformanceIndicatorID)
        {
        }

        public static Extend Get(Guid PerformanceIndicatorID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                DateTime DTNow = DateTime.Now;
                DateTime Today = DateTime.Today;

                var result =
                    (from pi in dc.PerformanceIndicator
                     join pif in dc.Frequency on pi.FrequencyID equals pif.FrequencyID
                     join _s in dc.Sector on pi.SectorID equals _s.SectorID into __s
                     join _g1 in dc.GroupCategoryAspect on new { pi.InstanceId, pi.GroupCategoryAspectID } equals new { _g1.InstanceId, GroupCategoryAspectID = (Guid?)_g1.GroupCategoryAspectID } into __g1
                     join _r in dc.Requirement on pi.RequirementID equals _r.RequirementID into __r
                     join _f in dc.PerformanceIndicatorFormula on new { pi.InstanceId, pi.PerformanceIndicatorID, Status = true } equals new { _f.InstanceId, _f.PerformanceIndicatorID, Status = _f.Status == true && DTNow >= _f.BeginDate && (_f.EndDate == null || DTNow < _f.EndDate) } into __f
                     join _u in dc.Mc_UnitsOfMeasure on pi.UnitOfMeasureID equals _u.UnitsOfMeasureId into __u
                     join _au in dc.Mc_UnitsOfMeasure on pi.UnitOfMeasureID equals _au.UnitsOfMeasureId into __au

                     from s in __s.DefaultIfEmpty()

                     from g1 in __g1.DefaultIfEmpty()
                     join _g2 in dc.GroupCategoryAspect on new { g1.InstanceId, g1.ParentId } equals new { _g2.InstanceId, ParentId = (Guid?)_g2.GroupCategoryAspectID } into __g2

                     from g2 in __g2.DefaultIfEmpty()
                     join _g3 in dc.GroupCategoryAspect on new { g2.InstanceId, g2.ParentId } equals new { _g3.InstanceId, ParentId = (Guid?)_g3.GroupCategoryAspectID } into __g3

                     from g3 in __g3.DefaultIfEmpty()

                     from r in __r.DefaultIfEmpty()

                     from f in __f.DefaultIfEmpty()
                     from u in __u.DefaultIfEmpty()
                     from au in __au.DefaultIfEmpty()

                     where
                         (pi.InstanceId == LinqMicajahDataContext.InstanceId)
                         &&
                         (pi.PerformanceIndicatorID == PerformanceIndicatorID)

                     orderby f.Updated descending

                     select new Extend
                     {
                         InstanceId = pi.InstanceId,
                         PerformanceIndicatorID = pi.PerformanceIndicatorID,
                         Name = pi.Name,
                         Description = pi.Description,
                         Help = pi.Help,
                         Code = pi.Code,

                         GroupCategoryAspectID = pi.GroupCategoryAspectID != null ? (g1.Status == true ? pi.GroupCategoryAspectID : null) : null,
                         FrequencyID = pi.FrequencyID,
                         SectorID = pi.SectorID,
                         RequirementID = pi.RequirementID,

                         GCAName = ((g3.Name == null) ? "" : g3.Name + "&nbsp;>&nbsp;") + ((g2.Name == null) ? "" : g2.Name + "&nbsp;>&nbsp;") + ((g1.Name == null) ? "" : g1.Name),
                         SectorName = s.Name,
                         RequirementName = r.Name,

                         Created = pi.Created,
                         Updated = pi.Updated,
                         Status = pi.Status,
                         UnitOfMeasureID = pi.UnitOfMeasureID,
                         AltUnitOfMeasureID = pi.AltUnitOfMeasureID,
                         FrequencyName = pif.Name,
                         DecimalPlaces = pi.DecimalPlaces,


                         PerformanceIndicatorFomulaID = f.PerformanceIndicatorFomulaID,
                         Formula = f.Formula,
                         FormulaBeginDate = f.BeginDate,
                         FormulaEndDate = f.EndDate,
                         FormulaCreated = f.Created,
                         FormulaUpdated = f.Updated,
                         FormulaUpdatedBy = f.UpdatedBy,
                         FormulaComment = f.Comment,
                         VariableFormula = f.VariableFormula

                     }).ToList();
                var ret = result.FirstOrNull();
                if(ret==null) return null;
                var ols = (from ol in dc.ViewPIOrgLocation join n in dc.ViewnameOrgLocation on new { ol.InstanceId, ol.OrgLocationID } equals new { n.InstanceId, n.OrgLocationID } where ol.PerformanceIndicatorID == PerformanceIndicatorID select n).ToList();
                //var piol =(from ol in dc.ViewPIOrgLocation where ol.PerformanceIndicatorID==PerformanceIndicatorID select ol.OrgLocationID==null?Guid.Empty:(Guid)ol.OrgLocationID ).ToList();
                ret.OrgLocationID = ols.Select(ol => ol.OrgLocationID == null ? Guid.Empty : (Guid)ol.OrgLocationID).ToList();
                ret.OrgLocationFullName = ols.Select(ol => ol.FullName).ToList();
                return ret;
            }
        }

        public override void OnDeleting(LinqMicajahDataContext c, ref bool Cancel)
        {
            base.OnDeleting(c, ref Cancel);
            Cancel = true;

            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var js = from j in dc.PerformanceIndicatorMetricJunc
                         where j.InstanceId == LinqMicajahDataContext.InstanceId &&
                               j.Status == true &&
                               j.PerformanceIndicatorID == PerformanceIndicatorID
                         select j;
                foreach (var j in js)
                    j.Status = false;

                PerformanceIndicator ret =
                         (from pi in dc.PerformanceIndicator
                          where (pi.InstanceId == LinqMicajahDataContext.InstanceId) && (pi.PerformanceIndicatorID == PerformanceIndicatorID)
                          select pi).FirstOrNull();
                if (ret != null)
                    ret.Status = false;

                dc.SubmitChanges();
            }
        }

        // Performance Indicator Report section
        public static IList<PIMetricInfo> GetPIReportValues(Guid? OrgLocationID, Guid GroupID, int FrequencyID, Guid? OrgLocationTypeID, DateTime BeginDate, DateTime EndDate)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IMultipleResults results = dc.Sp_SelectPIReportValues(LinqMicajahDataContext.InstanceId, OrgLocationID, GroupID, FrequencyID, OrgLocationTypeID, BeginDate, EndDate);
            IList<PIMetricInfo> MetricInfo = results.GetResult<PIMetricInfo>().ToList();
            IList<PIMetricValue> MetricValues = results.GetResult<PIMetricValue>().ToList();
            List<Micajah.Common.Bll.MeasureUnit> OrgUoMs = Mc_UnitsOfMeasure.GetOrganizationUoMs();           
            foreach (PIMetricInfo mi in MetricInfo)
            {
                mi.LocationFullName = (OrgLocationID == Guid.Empty && !String.IsNullOrEmpty(mi.LocationFullName)) ? LinqMicajahDataContext.OrganizationName : mi.LocationFullName;

                mi.InputUoMName = GetMeasureUnitPluralName(OrgUoMs, mi.InputUnitOfMeasureID);
                mi.UoMName = GetMeasureUnitPluralName(OrgUoMs, mi.UnitOfMeasureID);
                CompositeDailyValues cdv = new CompositeDailyValues();
                foreach (PIMetricValue mv in MetricValues)
                {
                    decimal Val = 0;
                    if (mv.MetricID == mi.MetricID && mv.OrgLocationID == mi.OrgLocationID) // add one more linq select
                        if ((mv.ValueMetricDataTypeID == 1) && (mv.UnitOfMeasureID == mv.MetricUnitOfMeasureID)) 
                            // !!! for now we ignore values when its output uom != metric output uom
                            if (decimal.TryParse(mv.ConvertedValue, out Val))
                                cdv.AddCompositeDailyValue((DateTime)mv.InputDate, Val, (int)mv.FrequencyID, (DateTime)mv.Date, (int)mi.FrequencyID);
                        
                }
                decimal? res = cdv.GetCompositeValue(BeginDate, EndDate, false);
                mi.SumValue = res == null ? "0" : res.ToString();                        
            }
            return MetricInfo;
        }

        public static IQueryable<Metric.MetricPIJunc> PIMetricJuncList()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IQueryable<Metric.MetricPIJunc> metrics =
                    from pimj in dc.PerformanceIndicatorMetricJunc
                    join m in dc.Metric on                    
                    new { pimj.InstanceId, pimj.MetricID } equals
                    new { m.InstanceId, m.MetricID }
                    join pi in dc.PerformanceIndicator on
                    new { pimj.InstanceId, pimj.PerformanceIndicatorID } equals
                    new { pi.InstanceId, pi.PerformanceIndicatorID }
                    where m.InstanceId == LinqMicajahDataContext.InstanceId &&
                          m.Status == true &&
                          pimj.Status == true &&
                          pi.Status == true
                    select new Metric.MetricPIJunc
                    {
                        MetricID = pimj.MetricID,
                        Name = m.Name,
                        PerformanceIndicatorID = pimj.PerformanceIndicatorID
                    };
            return metrics;
        }

        public static string GetMeasureUnitPluralName(List<Micajah.Common.Bll.MeasureUnit> OrgUoMs, Guid? MeasureUnitID)
        {
            Micajah.Common.Bll.MeasureUnit mu = OrgUoMs.Find(u => u.MeasureUnitId == MeasureUnitID);
            return mu == null ? String.Empty : mu.PluralFullName;
        }

        public static DataView GetOrgLocationTypes()
        {
            Micajah.Common.Dal.OrganizationDataSet.EntityNodeTypeDataTable dt = Micajah.Common.Bll.Providers.EntityNodeProvider.GetCustomEntityNodeTypesByEntityId(LinqMicajahDataContext.OrganizationId, null, new Guid("4cda22f3-4f01-4768-8608-938dc6a06825"));
            dt.DefaultView.Sort = "OrderNumber";
            return dt.DefaultView;    
        }

        class FormulaInfo{
            public Guid MetricID {get;set;}
            public Guid? OrgLocationID {get;set;}
            public string Function { get; set; }
            public double Value { get; set; }
        }

        static void AddAverage(ref double TotalVal, ref int TotalDays, double? NextVal, DateTime BeginDate, DateTime EndDate)
        {
            if (NextVal == null) NextVal = 0;
            int NextDays = (int)((BeginDate - EndDate).TotalDays) + 1;
            TotalVal = (TotalVal * TotalDays + (double)NextVal * NextDays) / (TotalDays + NextDays);
            TotalDays += NextDays;
        }

        static void AddRMS(ref double TotalVal, ref int TotalDays, double? NextVal, DateTime BeginDate, DateTime EndDate)
        {
            if (NextVal == null) NextVal = 0;
            int NextDays = (int)((BeginDate - EndDate).TotalDays) + 1;
            TotalVal = Math.Sqrt((TotalVal * TotalVal * TotalDays + (double)NextVal * (double)NextVal * NextDays) / (TotalDays + NextDays));
            TotalDays += NextDays;
        }

        static double GetValue(LinqMicajahDataContext dc, /*Extend pi*/Guid UnitOfMeasureID, ScoreCardMetric.enTotalValueType func, List<Guid> MetricIDs, List<Guid> OrgLocationIDs, DateTime BeginDate, DateTime EndDate, bool IncludeSubLocation)
        {
            double TotalVal = 0;
            double? NextVal;
            int TotalDays = 0;
            foreach(Guid MetricID in MetricIDs)
            {
                foreach (Guid OrgLocationID in OrgLocationIDs)
                {
                    NextVal = Bll.ScoreCardMetric.CalculateTotalValue(dc, LinqMicajahDataContext.OrganizationId, LinqMicajahDataContext.InstanceId, MetricID, OrgLocationID, UnitOfMeasureID, BeginDate, EndDate, IncludeSubLocation, func, true);
                    switch (func)
                    {
                        case Bll.ScoreCardMetric.enTotalValueType.Sum:
                            TotalVal += NextVal == null ? 0 : (double)NextVal;
                            break;
                        case Bll.ScoreCardMetric.enTotalValueType.Average:
                            AddAverage(ref TotalVal, ref TotalDays, NextVal, BeginDate, EndDate);
                            break;
                        case Bll.ScoreCardMetric.enTotalValueType.RMS:
                            AddRMS(ref TotalVal, ref TotalDays, NextVal, BeginDate, EndDate);
                            break;
                    }
                }
            }
            return TotalVal;
        }

        static ScoreCardMetric.enTotalValueType GetTotalValueType(string func)
        {
            if (func == Functions[1]) return Bll.ScoreCardMetric.enTotalValueType.Average;
            if (func == Functions[2]) return Bll.ScoreCardMetric.enTotalValueType.RMS;
            return Bll.ScoreCardMetric.enTotalValueType.Sum;
        }

        static string GetSimpleFormulasWithRealValues(LinqMicajahDataContext dc, /*Extend pi*/ Guid PiId, Guid PiUnitOfMessureId, PerformanceIndicatorFormula cf, List<Guid> OrgLocationIDs)
        {
            for (int sfi = 0; sfi < SimpleFunctions.Length; sfi++ )
            {
                string sf = SimpleFunctions[sfi];
                if (cf.Formula == sf)
                {
                    List<Guid> pimid = (
                        from j in dc.PerformanceIndicatorMetricJunc
                        join jm in dc.Metric on new {LinqMicajahDataContext.InstanceId, j.MetricID, Status=(bool?)true} equals new {jm.InstanceId, jm.MetricID, jm.Status}
                        where j.InstanceId == LinqMicajahDataContext.InstanceId && j.PerformanceIndicatorID == PiId && j.Status == true
                        select jm.MetricID
                        ).ToList();

                    ScoreCardMetric.enTotalValueType ft = GetTotalValueType(sf);
                    double TotalVal = GetValue(dc, PiUnitOfMessureId, ft, pimid, OrgLocationIDs, cf.BeginDate, (DateTime)cf.EndDate, true);
                    return TotalVal.ToString();
                }
            }
            return null;
        }

        static string GetFunction(ref string Formula, int ArgBegin, out Guid? arg2, out int FuncPos, out bool SelectedOrgLocation, out bool IncludeSubLocation, out ScoreCardMetric.enTotalValueType FuncType)
        {
            arg2 = null;
            FuncPos = 0;
            SelectedOrgLocation = true;
            IncludeSubLocation = true;
            FuncType = Bll.ScoreCardMetric.enTotalValueType.Sum;

            string arg1 = null;
            arg2 = null;
            foreach (string f in Functions)
            {
                int FuncBegin = ArgBegin - f.Length;
                if (FuncBegin < 0) continue;
                if (Formula.Substring(FuncBegin, f.Length) != f) continue;

                int FuncEnd = Formula.IndexOf(")", ArgBegin);
                if (FuncEnd < 0) return null;
                arg1 = Formula.Substring(ArgBegin, FuncEnd - ArgBegin).Trim();
                int CommaPos = arg1.IndexOf(",");
                if (CommaPos > 0)
                {
                    string s2 = arg1.Substring(CommaPos + 1).Trim();
                    arg1 = arg1.Substring(0, CommaPos).Trim();
                    arg2 = ParseGuidAsNumber(s2, "m");
                    if (arg2 == Guid.Empty) return null;
                }
                Formula = Formula.Substring(0, FuncBegin) + Formula.Substring(FuncEnd + 1);
                FuncPos = FuncBegin;

                string Selected = "Selected";
                string Report = "Report";
                SelectedOrgLocation = f.StartsWith(Selected);
                string pf = f.Substring(SelectedOrgLocation ? Selected.Length : Report.Length);

                if(string.IsNullOrEmpty(pf)) return null;
                IncludeSubLocation = pf.StartsWith("Sub");
                pf = pf.Substring(4);
                if (string.IsNullOrEmpty(pf)) return null;
                pf = pf.Substring(0, pf.Length-1);
                FuncType = GetTotalValueType(pf);
                return f;

            }
            return null;
        }

        static string GetFullFormulasWithRealValues(LinqMicajahDataContext dc, /*Extend pi*/Guid PIId, Guid FormulaUnitOfMesureId, PerformanceIndicatorFormula cf, List<Guid> OrgLocationIDs)
        {
            try
            {
                string Formula = cf.VariableFormula;
                List<string> m;
                List<string> l;
                int n;
                bool SelectedOrgLocation;
                bool IncludeSubLocation;
                ScoreCardMetric.enTotalValueType FuncType;
                List<Guid> MetricIDs = new List<Guid>();

                List<FormulaInfo> lFormulaInfo = new List<FormulaInfo>();
                MetricTrac.Bll.PerformanceIndicator.ParseFormulaCodes(cf.VariableFormula, out m, out l);

                foreach (string lid in l)
                {
                    Guid OrgLocationID = ParseGuidAsNumber(lid, "l");

                    while ((n = Formula.IndexOf(lid)) > 0)
                    {
                        int FuncPos;
                        Guid? MetricID;
                        string func = GetFunction(ref Formula, n, out MetricID, out FuncPos, out SelectedOrgLocation, out IncludeSubLocation, out FuncType);
                        if (string.IsNullOrEmpty(func)) return null;
                        if (MetricID == null) return null;
                        if (!SelectedOrgLocation) return null;

                        MetricIDs.Clear();
                        MetricIDs.Add((Guid)MetricID);

                        double v;
                        FormulaInfo fi = lFormulaInfo.Where(i => i.Function == func && i.MetricID == MetricID && i.OrgLocationID == OrgLocationID).FirstOrNull();
                        if (fi == null)
                        {
                            v = GetValue(dc, FormulaUnitOfMesureId, FuncType, MetricIDs, OrgLocationIDs, cf.BeginDate, (DateTime)cf.EndDate, IncludeSubLocation);
                            lFormulaInfo.Add(new FormulaInfo() { Function = func, MetricID = (Guid)MetricID, OrgLocationID = OrgLocationID, Value = v });
                        }
                        else
                        {
                            v = fi.Value;
                        }
                        Formula = Formula.Insert(FuncPos, v.ToString());
                    }
                }

                foreach (string mid in m)
                {
                    Guid MetricID = ParseGuidAsNumber(mid, "m");
                    if (MetricID == Guid.Empty) return null;
                    Guid? arg2;

                    while ((n = Formula.IndexOf(mid)) > 0)
                    {
                        int FuncPos;
                        string func = GetFunction(ref Formula, n, out arg2, out FuncPos, out SelectedOrgLocation, out IncludeSubLocation, out FuncType);

                        double v = 0;
                        FormulaInfo fi = lFormulaInfo.Where(i => i.Function == func && i.MetricID == MetricID && i.OrgLocationID == null).FirstOrNull();
                        if (fi == null)
                        {
                            MetricIDs.Clear();
                            MetricIDs.Add((Guid)MetricID);
                            v = GetValue(dc, FormulaUnitOfMesureId, FuncType, MetricIDs, OrgLocationIDs, cf.BeginDate, (DateTime)cf.EndDate, IncludeSubLocation);
                            lFormulaInfo.Add(new FormulaInfo() { Function = func, MetricID = (Guid)MetricID, OrgLocationID = null, Value = v });
                        }
                        else
                        {
                            v = fi.Value;
                        }

                        Formula = Formula.Insert(FuncPos, v.ToString());
                    }
                }
                return Formula;
            }
            catch
            {
                return null;
            }
        }

        static string GetFormulasWithRealValues(LinqMicajahDataContext dc, /*Extend pi*/Guid PIId, Guid UnitOfMessureId, PerformanceIndicatorFormula cf, List<Guid> OrgLocationIDs)
        {
            string SimpleValue = GetSimpleFormulasWithRealValues(dc, PIId, UnitOfMessureId, cf, OrgLocationIDs);
            if (SimpleValue != null) return SimpleValue;

            string FullFormulaValue = GetFullFormulasWithRealValues(dc, PIId, UnitOfMessureId, cf, OrgLocationIDs);
            return FullFormulaValue;
        }

        /*public static List<string> GetFormulasWithRealValues(LinqMicajahDataContext dc, DateTime StartDate, DateTime EndDate, List<Guid> OrgLocationIDs, Extend pi)
        {
            List<string> FormulasWithRealValues = new List<string>();
            var Fs =
                        (
                            from f in dc.PerformanceIndicatorFormula
                            where f.InstanceId == LinqMicajahDataContext.InstanceId && f.PerformanceIndicatorID == pi.PerformanceIndicatorID &&
                                StartDate >= f.BeginDate && (f.EndDate == null || EndDate <= f.EndDate)
                            orderby f.Updated == null ? f.Created : f.Updated descending
                            select f
                        ).ToList();
            if (Fs == null || Fs.Count < 1) return FormulasWithRealValues;

            List<PerformanceIndicatorFormula> CalcFormulas = new List<PerformanceIndicatorFormula>();
            PerformanceIndicatorFormula CurrentFormula = null;
            PerformanceIndicatorFormula ReturnCurrentFormula=null;

            for (DateTime day = StartDate; day <= EndDate; day = day.AddDays(1))
            {
                var AcceptableFormulas = Fs.Where(pif => day >= pif.BeginDate && (pif.EndDate == null || day <= pif.EndDate)).ToList();
                if (AcceptableFormulas.Count > 1)
                {
                    AcceptableFormulas = AcceptableFormulas
                        .OrderByDescending(pif => pif.Updated == null ? pif.Created : pif.Updated)
                        .OrderByDescending(pif => pif.BeginDate)
                        .OrderByDescending(pif => pif.Formula)
                        .ToList();
                }
                PerformanceIndicatorFormula NextFourmula = AcceptableFormulas.FirstOrDefault();
                if (NextFourmula == null || NextFourmula.PerformanceIndicatorFomulaID == Guid.Empty)
                {
                    CurrentFormula = null;
                    continue;
                }
                if (CurrentFormula == null || 
                        (
                            CurrentFormula.PerformanceIndicatorFomulaID != NextFourmula.PerformanceIndicatorFomulaID && 
                            (
                                CurrentFormula.Formula != NextFourmula.Formula ||
                                CurrentFormula.VariableFormula != NextFourmula.VariableFormula ||
                                CurrentFormula.BeginDate != NextFourmula.BeginDate ||
                                CurrentFormula.EndDate != NextFourmula.EndDate
                            )
                        )
                    )
                {
                    CurrentFormula = NextFourmula;
                    ReturnCurrentFormula = new PerformanceIndicatorFormula();
                    ReturnCurrentFormula.Formula = CurrentFormula.Formula;
                    ReturnCurrentFormula.VariableFormula = CurrentFormula.VariableFormula;
                    ReturnCurrentFormula.BeginDate = day;
                    ReturnCurrentFormula.EndDate = day;
                    CalcFormulas.Add(ReturnCurrentFormula);
                }
                else
                {
                    ReturnCurrentFormula.EndDate = day;
                }
            }

            for (int i = 0; i < CalcFormulas.Count; i++)
            {
                PerformanceIndicatorFormula cf = CalcFormulas[i];
                string f = GetFormulasWithRealValues(dc, pi, cf, OrgLocationIDs);
                if (!string.IsNullOrEmpty(f)) FormulasWithRealValues.Add(f);
            }
            return FormulasWithRealValues;
        }*/

        

        public static List<string> GetFormulasWithRealValues(LinqMicajahDataContext dc, DateTime StartDate, DateTime EndDate, List<Guid> OrgLocationIDs, Guid PIId, Guid ResultUnitOfMesureId)
        {
            List<string> FormulasWithRealValues = new List<string>();
            var Fs =
                        (
                            from f in dc.PerformanceIndicatorFormula
                            where f.InstanceId == LinqMicajahDataContext.InstanceId && f.PerformanceIndicatorID == PIId &&
                                StartDate >= f.BeginDate && (f.EndDate == null || EndDate <= f.EndDate)
                            orderby f.Updated == null ? f.Created : f.Updated descending
                            select f
                        ).ToList();
            if (Fs == null || Fs.Count < 1) return FormulasWithRealValues;

            List<PerformanceIndicatorFormula> CalcFormulas = new List<PerformanceIndicatorFormula>();
            PerformanceIndicatorFormula CurrentFormula = null;
            PerformanceIndicatorFormula ReturnCurrentFormula = null;

            for (DateTime day = StartDate; day <= EndDate; day = day.AddDays(1))
            {
                var AcceptableFormulas = Fs.Where(pif => day >= pif.BeginDate && (pif.EndDate == null || day <= pif.EndDate)).ToList();
                if (AcceptableFormulas.Count > 1)
                {
                    AcceptableFormulas = AcceptableFormulas
                        .OrderByDescending(pif => pif.Formula)
                        .OrderByDescending(pif => pif.BeginDate)
                        .OrderByDescending(pif => pif.Updated == null ? pif.Created : pif.Updated)
                        .ToList();
                }
                PerformanceIndicatorFormula NextFourmula = AcceptableFormulas.FirstOrDefault();
                if (NextFourmula == null || NextFourmula.PerformanceIndicatorFomulaID == Guid.Empty)
                {
                    CurrentFormula = null;
                    continue;
                }
                if (CurrentFormula == null ||
                        (
                            CurrentFormula.PerformanceIndicatorFomulaID != NextFourmula.PerformanceIndicatorFomulaID &&
                            (
                                CurrentFormula.Formula != NextFourmula.Formula ||
                                CurrentFormula.VariableFormula != NextFourmula.VariableFormula ||
                                CurrentFormula.BeginDate != NextFourmula.BeginDate ||
                                CurrentFormula.EndDate != NextFourmula.EndDate
                            )
                        )
                    )
                {
                    CurrentFormula = NextFourmula;
                    ReturnCurrentFormula = new PerformanceIndicatorFormula();
                    ReturnCurrentFormula.Formula = CurrentFormula.Formula;
                    ReturnCurrentFormula.VariableFormula = CurrentFormula.VariableFormula;
                    ReturnCurrentFormula.BeginDate = day;
                    ReturnCurrentFormula.EndDate = day;
                    CalcFormulas.Add(ReturnCurrentFormula);
                }
                else
                {
                    ReturnCurrentFormula.EndDate = day;
                }
            }

            for (int i = 0; i < CalcFormulas.Count; i++)
            {
                PerformanceIndicatorFormula cf = CalcFormulas[i];
                string f = GetFormulasWithRealValues(dc, PIId, ResultUnitOfMesureId, cf, OrgLocationIDs);
                if (!string.IsNullOrEmpty(f)) FormulasWithRealValues.Add(f);
            }
            return FormulasWithRealValues;
        }

        public static List<FrequencyMetric> GetValues(DateTime StartDate, int ValueCount, Guid?[] PerfomanceIndicatorIDs, MetricTrac.Bll.ScoreCardMetric.CalcStringFormula Calculator)
        {
            List<FrequencyMetric> lf = new List<FrequencyMetric>();
            foreach (Guid? PerfomanceIndicatorID in PerfomanceIndicatorIDs)
            {
                if (PerfomanceIndicatorID == null) continue;
                var pi = Get((Guid)PerfomanceIndicatorID);
                if (pi == null) continue;
                if (pi.OrgLocationID == null || pi.OrgLocationID.Count < 1) continue;

                FrequencyMetric freq = lf.Where(f => f.FrequencyID == pi.FrequencyID).FirstOrNull();
                if (freq == null)
                {
                    freq = new FrequencyMetric()
                    {
                        Date = MetricValue.GetDateHeader(pi.FrequencyID, StartDate, ValueCount),
                        FrequencyID = pi.FrequencyID,
                        Metrics = new List<MetricOrgValue>(),
                        Name = pi.FrequencyName
                    };
                    lf.Add(freq);
                }

                using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
                {
                    for (int li = 0; li < pi.OrgLocationID.Count; li++)
                    {
                        Guid OLID = pi.OrgLocationID[li];
                        MetricOrgValue mov = new MetricOrgValue();
                        mov.MetricValues = new List<MetricValue.Extend>();
                        for (int i = 0; i < ValueCount; i++) mov.MetricValues.Add(null);
                        mov.Date = MetricValue.GetDateHeader(pi.FrequencyID, StartDate, ValueCount);

                        /*FrequencyMetric f = new FrequencyMetric();
                        f.Date = 
                        f.Metric = new EntitySet<Metric>();
                        Metric m = new Metric();
                        f.Metric.Add(m);*/

                        mov.FrequencyID = pi.FrequencyID;
                        mov.Alias = pi.Alias;
                        mov.Code = pi.Code;
                        mov.Created = pi.Created;
                        mov.FrequencyID = pi.FrequencyID;
                        mov.InputUnitOfMeasureID = pi.UnitOfMeasureID;
                        mov.UnitOfMeasureID = pi.AltUnitOfMeasureID;
                        mov.InstanceId = pi.InstanceId;
                        mov.MetricID = pi.PerformanceIndicatorID;
                        mov.Name = "PI: " + pi.Name;
                        mov.NODecPlaces = pi.DecimalPlaces;
                        mov.Status = pi.Status;
                        mov.Updated = pi.Updated;
                        mov.OrgLocationFullName = pi.OrgLocationFullName[li];

                        DateTime sd = Frequency.GetNormalizedDate(pi.FrequencyID,StartDate);
                        DateTime ed;
                        for (int i = 0; i < mov.Date.Count; i++)
                        {
                            ed = Frequency.AddPeriod(sd, pi.FrequencyID, -1);
                            var Formulas = GetFormulasWithRealValues(dc, ed, sd, new List<Guid>(new Guid[] { OLID }), pi.PerformanceIndicatorID, mov.InputUnitOfMeasureID == null ? Guid.Empty : (Guid)mov.InputUnitOfMeasureID);
                            double val = Calculator(Formulas);
                            MetricValue.Extend NewVal;
                            if (val != 0)
                            {
                                string strVal = val.ToString();
                                try
                                {
                                    strVal = val.ToString("N" + ((int)pi.DecimalPlaces));
                                }
                                catch { }
                                NewVal = new MetricValue.Extend()
                                {
                                    Approved = true,
                                    Date = mov.Date[i].Date,
                                    DecimalValue = (decimal)val,
                                    FrequencyID = pi.FrequencyID,
                                    InProcess = false,
                                    InputUnitOfMeasureID = pi.UnitOfMeasureID,
                                    InstanceId = pi.InstanceId,
                                    IsCalc = true,
                                    MetricDataTypeID = 1,
                                    OrgLocationID = OLID,
                                    Status = true,
                                    Value = strVal
                                };
                            }
                            else
                            {
                                NewVal = new MetricValue.Extend();
                            }
                            mov.MetricValues[i] = NewVal;
                            sd = ed;
                        }
                        freq.Metrics.Add(mov);
                    }
                }
            }
            return lf;
        }
        public static void GetFormulasWithRealValues(DateTime StartDate, DateTime EndDate, List<Guid> OrgLocationIDs, out List<Extend> outPI, out List<List<string>> outFormulas, Guid? GroupID)
        {
            outPI = new List<Extend>();
            outFormulas = new List<List<string>>();
            using(LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                List<Extend> PIs = (
                    from pi in dc.PerformanceIndicator
                    join _gca1 in dc.GroupCategoryAspect on new { LinqMicajahDataContext.InstanceId, pi.GroupCategoryAspectID } equals new { _gca1.InstanceId, GroupCategoryAspectID = (Guid?)_gca1.GroupCategoryAspectID } into __gca1

                    from gca1 in __gca1.DefaultIfEmpty()
                    join _gca2 in dc.GroupCategoryAspect on new { LinqMicajahDataContext.InstanceId, gca1.ParentId } equals new { _gca2.InstanceId, ParentId=(Guid?)_gca2.GroupCategoryAspectID } into __gca2

                    from gca2 in __gca2.DefaultIfEmpty()
                    join _gca3 in dc.GroupCategoryAspect on new { LinqMicajahDataContext.InstanceId, gca2.ParentId } equals new { _gca3.InstanceId, ParentId=(Guid?)_gca3.GroupCategoryAspectID } into __gca3

                    from gca3 in __gca3.DefaultIfEmpty()

                    where pi.InstanceId == LinqMicajahDataContext.InstanceId &&
                        pi.Status == true &&
                        (GroupID ==null || pi.GroupCategoryAspectID == GroupID || gca1.ParentId == GroupID || gca2.ParentId == GroupID)
                    orderby pi.Name
                    select new Extend()
                    {
                        Alias = pi.Alias,
                        Code = pi.Code,
                        Created = pi.Created,
                        Description = pi.Description,
                        GroupCategoryAspectID = pi.GroupCategoryAspectID,
                        Help = pi.Help,
                        InstanceId = LinqMicajahDataContext.InstanceId,
                        Name = pi.Name,
                        PerformanceIndicatorID = pi.PerformanceIndicatorID,
                        RequirementID = pi.RequirementID,
                        SectorID = pi.SectorID,
                        SortCode = pi.SortCode,
                        Status = true,
                        UnitOfMeasureID = pi.UnitOfMeasureID,
                        AltUnitOfMeasureID = pi.AltUnitOfMeasureID,

                        GroupID = gca2.ParentId == null ? (gca1.ParentId == null ? pi.GroupCategoryAspectID : gca1.ParentId) : gca2.ParentId,
                        GroupName = gca2.ParentId == null ? (gca1.ParentId == null ? gca1.Name : gca2.Name) : gca3.Name,
                        CategoryID = gca2.ParentId == null ? (gca1.ParentId == null ? null : pi.GroupCategoryAspectID) : gca1.ParentId,
                        CategoryName = gca2.ParentId == null ? (gca1.ParentId == null ? null : gca1.Name) : gca2.Name,
                        AspectID = gca2.ParentId == null ? null : pi.GroupCategoryAspectID,
                        AspectName = gca2.ParentId == null ? null : gca1.Name,
                    }
                    ).OrderBy(o => o.GroupName).OrderBy(o => o.GroupID)
                    .OrderBy(o => o.CategoryName).OrderBy(o => o.CategoryID)
                    .OrderBy(o => o.AspectName).OrderBy(o => o.AspectID)
                    .ToList();

                foreach(Extend pi in PIs)
                {
                    if (pi.GroupID == null)
                    {
                        if (pi.CategoryID == null)
                        {
                            pi.GroupID = pi.AspectID;
                            pi.GroupName = pi.AspectName;
                        }
                        else
                        {
                            pi.GroupID = pi.CategoryID;
                            pi.GroupName = pi.CategoryName;
                            pi.CategoryID = pi.AspectID;
                            pi.CategoryName = pi.AspectName;
                        }
                    }

                    List<string> FormulasWithRealValues = GetFormulasWithRealValues(dc, StartDate, EndDate, OrgLocationIDs, pi.PerformanceIndicatorID, pi.UnitOfMeasureID == null ? Guid.Empty : (Guid)pi.UnitOfMeasureID);
                    outPI.Add(pi);
                    outFormulas.Add(FormulasWithRealValues);
                }
            }
        }
    }    
}
