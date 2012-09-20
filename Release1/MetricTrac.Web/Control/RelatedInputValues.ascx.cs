using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;
using System.Data;
using System.Data.Linq;
using System.Collections;

namespace MetricTrac.Control
{
    public partial class RelatedInputValues : System.Web.UI.UserControl
    {
        public enum Mode { Input, View, Approve }

        MetricTrac.Bll.FrequencyMetric LastFrequencyMetric;
        MetricTrac.Bll.MetricOrgValue LastMetricMetricValue;

        protected Guid CalcMetricID
        {
            get
            {
                string s = Request.QueryString["MetricID"];
                Guid ID = Guid.Empty;
                try { ID = new Guid(s); }
                catch { };
                return ID;
            }
        }

        public Mode DataMode { get; set; }

        protected bool GroupByMetric
        {
            get
            {
                return MF.GroupByMetric;                
            }
        }

        protected DateTime CurrentDate = DateTime.Now;
        protected DateTime FrequencyFirstDate;
        readonly string FrequencyFirstDateViewState = "relFrequencyFirstDate";
        
        protected void Page_Load(object sender, EventArgs e)
        {   
            object o = ViewState[FrequencyFirstDateViewState];
            if (o is DateTime) FrequencyFirstDate = (DateTime)o;
            else FrequencyFirstDate = DateTime.MinValue;
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricInput.css");            
            switch (DataMode)
            {
                case Mode.Approve:
                    if (Micajah.Common.Security.UserContext.Breadcrumbs.Exists(r => r.Name.Contains("Approve Data"))) // TODO: change to redirect to parent
                    {
                        hlBack.NavigateUrl = "/home/ApproveDataList.aspx";
                        hlBack.Text = "Back to Approve Data";
                    }
                    else
                    {
                        hlBack.NavigateUrl = "/home/ApproveWorkList.aspx";
                        hlBack.Text = "Back to Data Validation & Approval";
                    }
                    break;
                case Mode.View:                    
                default:
                    hlBack.NavigateUrl = "/home/MetricDataList.aspx";
                    hlBack.Text = "Back to View Data";
                    break;
            }            
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            ReBind();
            ViewState[FrequencyFirstDateViewState] = FrequencyFirstDate;            
            if (rapFrequency.IsAjaxRequest)
                rapFrequency.ResponseScripts.Add("rapFrequency_ResponseEnd()");
        }

        void ReBind()
        {
            Guid? ApproverUserId = null;
            if (DataMode == Mode.Approve) ApproverUserId = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;
            int ValueCount = 6;
            LastFrequencyMetric = MetricTrac.Bll.MetricValue.RelatedValuesList(ValueCount, CurrentDate, FrequencyFirstDate, CalcMetricID, GroupByMetric, ApproverUserId);
            bool FilledFormulas = false;
            foreach (MetricTrac.Bll.MetricOrgValue mmv in LastFrequencyMetric.Metrics)
            {
                if (mmv.MetricID == CalcMetricID && !FilledFormulas)
                {
                    List<Bll.FormulaHeader> f = new List<Bll.FormulaHeader>();
                    FilledFormulas = true;
                    Guid? LastFormulaID = mmv.MetricValues[0].RelatedFormulaID;
                    string Formula = mmv.MetricValues[0].Formula;
                    int group = 1;
                    for (int i = 1; i < mmv.MetricValues.Count; i++)
                    {
                        if (LastFormulaID == mmv.MetricValues[i].RelatedFormulaID)
                            group++;
                        else
                        {
                            Bll.FormulaHeader fh = new MetricTrac.Bll.FormulaHeader();
                            fh.ColGroup = group;
                            fh.sFormula = Formula;
                            f.Add(fh);
                            LastFormulaID = mmv.MetricValues[i].RelatedFormulaID;
                            Formula = mmv.MetricValues[i].Formula;
                            group = 1;
                        }
                    }
                    Bll.FormulaHeader fhe = new MetricTrac.Bll.FormulaHeader();
                    fhe.ColGroup = group;
                    fhe.sFormula = Formula;
                    f.Add(fhe);
                    LastFrequencyMetric.Formulas = f;
                }
                mmv.OrgLocationFullName = MetricTrac.Bll.Mc_EntityNode.GetHtmlFullName(mmv.OrgLocationFullName);
                
                if (string.IsNullOrEmpty(mmv.InputUnitOfMeasureName)) continue;
                // try to trim uom name to 4 letters or 3 letters + '.'
                /*bool IsNameTooLong = false;
                if (mmv.InputUnitOfMeasureName.Length > 3)
                {
                    IsNameTooLong = true;
                    mmv.InputUnitOfMeasureNameTooltip = mmv.InputUnitOfMeasureName;
                    mmv.InputUnitOfMeasureName = mmv.InputUnitOfMeasureName.Substring(0, 2);
                }*/
                for (int i = mmv.InputUnitOfMeasureName.Length - 1; i > 0; i--)
                    mmv.InputUnitOfMeasureName = mmv.InputUnitOfMeasureName.Insert(i, "<br />");
                /*if (IsNameTooLong)
                    mmv.InputUnitOfMeasureName += "<div><img height=\"8\" width=\"8\" src=\"../images/More.gif\"></div>";*/
            }
            if (FrequencyFirstDate == DateTime.MinValue)
                FrequencyFirstDate = MetricTrac.Bll.Frequency.GetNormalizedDate(LastFrequencyMetric.FrequencyID, CurrentDate);
            lblFrequency.Text = LastFrequencyMetric.Name;           
            rFormula.DataSource = LastFrequencyMetric.Formulas;
            rFormula.DataBind();
            rMetric.DataSource = LastFrequencyMetric.Metrics;
            rMetric.DataBind();
            
        }

        System.Web.UI.HtmlControls.HtmlTableCell[] LastTdUnit = new System.Web.UI.HtmlControls.HtmlTableCell[2];
        int LocationCount;
        protected void rpMetric_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;

            MetricTrac.Bll.MetricOrgValue NewMMV = (MetricTrac.Bll.MetricOrgValue)e.Item.DataItem;
            bool IsNewUnit = true;//LastMetricMetricValue == null || IsNewGroup(e.Item) || LastMetricMetricValue.InputUnitOfMeasureName != NewMMV.InputUnitOfMeasureName;
            LastMetricMetricValue = NewMMV;

            if (IsNewUnit)
            {
                if (LastTdUnit[0] != null)
                {
                    LastTdUnit[0].RowSpan = LocationCount;
                    LastTdUnit[1].RowSpan = LocationCount;
                }
                LastTdUnit[0] = e.Item.FindControl("tdUnitLeft") as System.Web.UI.HtmlControls.HtmlTableCell;
                LastTdUnit[1] = e.Item.FindControl("tdUnitRight") as System.Web.UI.HtmlControls.HtmlTableCell;
                LastTdUnit[0].Visible = true;
                LastTdUnit[1].Visible = true;
                LocationCount = 1;

                BindRepiter("rDate", LastFrequencyMetric.Date, e);                
                SetNavigationButton(e, LastFrequencyMetric.FrequencyID);
            }
            else
            {
                LocationCount++;
                LastTdUnit[0].RowSpan = LocationCount;
                LastTdUnit[1].RowSpan = LocationCount;
            }
            BindRepiter("rMericValue", NewMMV.MetricValues, e);
        }

        protected bool IsNewGroup(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            MetricTrac.Bll.MetricOrgValue mmv = ri.DataItem as MetricTrac.Bll.MetricOrgValue;
            if (LastMetricMetricValue == null) return true;
            if (LastMetricMetricValue.FrequencyID != mmv.FrequencyID) return true;
            if (GroupByMetric) return LastMetricMetricValue.MetricID != mmv.MetricID;
            return LastMetricMetricValue.OrgLocationID != mmv.OrgLocationID;
        }

        private void BindRepiter(string rID, object DS, RepeaterItemEventArgs e)
        {
            Repeater r = (Repeater)e.Item.FindControl(rID);
            r.DataSource = DS;
            r.DataBind();
        }

        void SetNavigationButton(RepeaterItemEventArgs e, int FrequencyID)
        {
            DateTime L0Date = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID, CurrentDate);
            DateTime L1Date = MetricTrac.Bll.Frequency.AddPeriod(FrequencyFirstDate, FrequencyID, 1);
            if (L1Date > L0Date) DisableNavigationButton(e, "L1");
        }

        void DisableNavigationButton(RepeaterItemEventArgs e, string IDSuffix)
        {
            Image i = (Image)e.Item.FindControl("iNav" + IDSuffix);
            i.ImageUrl = i.ImageUrl.Replace(".png", "-disabled.png");
            HyperLink hl = (HyperLink)e.Item.FindControl("hlNav" + IDSuffix);
            hl.NavigateUrl = string.Empty;
        }

        protected void rapFrequency_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            string Command = e.Argument;
            if (string.IsNullOrEmpty(Command) || Command.Length < 3) return;

            if (Command[0] != 'L' && Command[0] != 'R') return;
            int step = Command[0] == 'L' ? 1 : -1;

            int FrequencyID;
            if (!int.TryParse(Command.Substring(2), out FrequencyID)) return;
            FrequencyFirstDate = MetricTrac.Bll.Frequency.AddPeriod(FrequencyFirstDate, FrequencyID, step);            
        }

        //---- render methods
        private string GetGrayColor(int g)
        {
            string s = g.ToString("X2");
            return "#" + s + s + s;
        }

        MetricTrac.Bll.MetricValue.Extend GetValue(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            return ri.DataItem as MetricTrac.Bll.MetricValue.Extend;
        }

        protected string GetEditUrl(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            int BackPageIndex = 7;
            switch (DataMode)
            {
                case Mode.Input:
                    BackPageIndex = 7;
                    break;
                case Mode.Approve:
                    BackPageIndex = 8;
                    break;
                default:
                    BackPageIndex = 7;
                    break;
            }
            return "MetricInputEdit.aspx?MetricID=" + val.MetricID +
                "&Date=" + val.Date.ToString("MM-dd-yyyy") +
                "&OrgLocationID=" + LastMetricMetricValue.OrgLocationID +
                "&Mode=" + DataMode.ToString() +
                "&BackPage=" + BackPageIndex.ToString();
        }

        protected string GetOnClickHandler(object container)
        {
            string OnClickHandler = String.Empty;
            if (MetricTrac.Utils.MetricUtils.IsPopupSupported(Request))
                OnClickHandler = "onclick=\"openRadWindow('" + GetEditUrl(container) + "');return false;\"";
            return OnClickHandler;
        }

        protected bool IsEditValue(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            if (DataMode == Mode.Input)
                return !val.IsAbsent;
            else
                return val.Value != null;
        }

        protected string GetChartUrl(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            MetricTrac.Bll.MetricOrgValue v = (MetricTrac.Bll.MetricOrgValue)ri.DataItem;
            string FreqDate = FrequencyFirstDate.ToString("MM-dd-yyyy");
            return "DataValueChart.aspx?MetricID=" + v.MetricID + "&OrgLocationID=" + v.OrgLocationID + "&Date=" + FreqDate;
        }

        protected string GetEmptyStyle(object container)
        {
            string style = String.Empty;
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            if (val.IsAbsent)
                style = " absent";
            else
                if (val.Value == null)
                    style = " empty";            
            return style;
        }

        protected bool IsCalculatedValue(object container)
        {
            return GetValue(container).IsCalculated == true;
        }

        protected string GetCalcStatusStyle(object container)
        {
            string CalcStyle = String.Empty;
            if (IsCalculatedValue(container))
                CalcStyle = GetValue(container).MissedCalc ? "style=\"color:Red;\"" : "style=\"color:Olive\"";
            return CalcStyle;
        }

        protected string GetValueTitle(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            string title = String.Empty;
            if (val.Value != null)
            {
                if (val.IsCalculated == true)
                {
                    title = "Calc value";
                    if (val.MissedCalc)
                        title += " | Some input values missed";
                }
                else
                    title = "Input value";
                switch (val.Approved)
                {
                    case null:
                        title += (val.ReviewUpdated) ? " | Under Review (Updated)" : " | Under Review";
                        break;
                    case true:
                        title += " | Approved";
                        break;
                    case false:
                    default:
                        title += " | Pending";
                        break;
                }
            }
            return title;
        }

        protected string GetValueCss(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            string ret = "class='";
            if (val.IsCalculated == true && val.MissedCalc)
                ret += "rick";
            else
            {
                if ((val.Approved == false) && !val.FilesAttached) return "class='empty-css'";
                if (val.Approved == true)
                    ret += "tick";
                else
                    if (val.Approved == null)
                    {
                        ret += "rick";
                        if (val.ReviewUpdated)
                            ret += "-up";
                    }
                if (val.FilesAttached)
                {
                    if (val.Approved != false) ret += "-";
                    ret += "clip";
                }
            }
            ret += "'";
            return ret;
        }

        protected string GetValueCell(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            return Bll.MetricValue.FormatValue(val);
        }

        protected string GetTitle(int FrequencyID, string CommandName)
        {
            string Title = (CommandName[0] == 'L') ? "Next " : "Previous ";
            switch (CommandName[1])
            {
                case '0':
                    Title = "Current";
                    break;
                case '1':
                    Title += MetricTrac.Bll.Frequency.GetFrequencyName(FrequencyID);
                    break;
                case '2':
                    switch ((MetricTrac.Bll.Frequency.FrequencyName)FrequencyID)
                    {
                        case MetricTrac.Bll.Frequency.FrequencyName.BiAnnual:
                            Title += "FourAnnual";
                            break;
                        case MetricTrac.Bll.Frequency.FrequencyName.FiscalBiAnnual:
                            Title += "FiscalFourAnnual";
                            break;
                        default:
                            Title += MetricTrac.Bll.Frequency.GetFrequencyName(FrequencyID + 1);
                            break;
                    }
                    break;
                case '3':
                    switch ((MetricTrac.Bll.Frequency.FrequencyName)FrequencyID)
                    {
                        case MetricTrac.Bll.Frequency.FrequencyName.Annual:
                            Title += "FourAnnual";
                            break;
                        case MetricTrac.Bll.Frequency.FrequencyName.BiAnnual:
                            Title += "EightAnnual";
                            break;
                        case MetricTrac.Bll.Frequency.FrequencyName.FiscalAnnual:
                            Title += "FiscalFourAnnual";
                            break;
                        case MetricTrac.Bll.Frequency.FrequencyName.FiscalBiAnnual:
                            Title += "FiscalEightAnnual";
                            break;
                        default:
                            Title += MetricTrac.Bll.Frequency.GetFrequencyName(FrequencyID + 2);
                            break;
                    }
                    break;
            }
            return "To " + Title;
        }
    }
}
