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
    public partial class MetricInputList : System.Web.UI.UserControl
    {
        public enum Mode { Input, View, Approve }

        MetricTrac.Bll.FrequencyMetric LastFrequencyMetric;
        MetricTrac.Bll.MetricOrgValue LastMetricMetricValue;

        protected readonly string RestrictWidth="75px";

        public Mode DataMode { get; set; }
        protected bool GroupByMetric
        {
            get
            {
                return MF.GroupByMetric;                
            }
        }

        protected Guid? SelectedMetricID
        {
            get
            {
                string s = Request.QueryString["MetricID"];
                if (string.IsNullOrEmpty(s)) return null;

                Guid? ID = null;
                try { ID = new Guid(s); }
                catch { };
                return ID;
            }
        }

        DateTime mCurrentDate = DateTime.Now;
        DateTime CurrentDate
        {
            get
            {
                if (DataMode == Mode.View && MF.BaseDate != null) return (DateTime)MF.BaseDate;
                return mCurrentDate; 
            }
        }

        Dictionary<int, DateTime> FrequencyFirstDate;

        readonly string FrequencyFirstDateViewState = "FrequencyFirstDate";
        DateTime StartTime;
        protected void Page_Load(object sender, EventArgs e)
        {
            StartTime = DateTime.Now;
            if (!IsPostBack) MF.BaseDate = DateTime.Now;
            MF.FilterSectionVisible = DataMode == Mode.View;
            pnlAlert.Visible = DataMode != Mode.View;            
            AlertQueue.DataMode = this.DataMode;
            AlertQueue.GroupByMetric = this.GroupByMetric;            
            object o = ViewState[FrequencyFirstDateViewState];
            if (o is Dictionary<int, DateTime>) FrequencyFirstDate = (Dictionary<int, DateTime>)o;
            else FrequencyFirstDate = new Dictionary<int, DateTime>();
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricInput.css");
        }

        void ReBind()
        {
            if (DataMode == Mode.Approve) ApproverUserId = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;
            if (DataMode == Mode.Input) SelCollectorUseID = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;


            List<MetricTrac.Bll.FrequencyMetric> fms = MetricTrac.Bll.MetricValue.List(6, CurrentDate, FrequencyFirstDate, SelectedMetricID, SelMetricID,
                SelOrgLocationsID, SelGcaID, SelPiID, SelPifID, SelCollectorUseID, DataMode == Mode.View || DataMode == Mode.Approve, GroupByMetric, ApproverUserId);
            ((MetricTrac.MasterPage)(Page.Master)).AddDebufInfo("SqlQueryTime", MetricTrac.Bll.MetricValue.SqlQueryTime.ToString());
            ((MetricTrac.MasterPage)(Page.Master)).AddDebufInfo("TotalTime", MetricTrac.Bll.MetricValue.TotalTime.ToString());

            foreach (MetricTrac.Bll.FrequencyMetric fm in fms)
            {
                foreach (MetricTrac.Bll.MetricOrgValue mmv in fm.Metrics)
                {
                    mmv.OrgLocationFullName = MetricTrac.Bll.Mc_EntityNode.GetHtmlFullName(mmv.OrgLocationFullName);
                }
            }
            rFrequency.DataSource = fms;
            rFrequency.DataBind();
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (!MF.IsInternalAjax && !AlertQueue.IsInternalAjax) ReBind();
            ViewState[FrequencyFirstDateViewState] = FrequencyFirstDate;
            lbMode.Visible = SelectedMetricID != null;
            if (rapFrequency.IsAjaxRequest) rapFrequency.ResponseScripts.Add("rapFrequency_ResponseEnd()");
            lbDebugInfo.Text = "Debug Info: TotalMilliseconds=" + ((TimeSpan)(DateTime.Now - StartTime)).TotalMilliseconds;
        }


        protected void rdiDateToday_TextChanged(object sender, EventArgs e)
        {
        }

        private void BindRepiter(string rID, object DS, RepeaterItemEventArgs e)
        {
            Repeater r = (Repeater)e.Item.FindControl(rID);
            r.DataSource = DS;
            r.DataBind();
        }

        void DisableNavigationButton(RepeaterItemEventArgs e, string IDSuffix)
        {
            Image i = (Image)e.Item.FindControl("iNav" + IDSuffix);
            i.ImageUrl = i.ImageUrl.Replace(".png", "-disabled.png");
            HyperLink hl = (HyperLink)e.Item.FindControl("hlNav" + IDSuffix);
            hl.NavigateUrl = string.Empty;
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

        void SetNavigationButton(RepeaterItemEventArgs e, int FrequencyID)
        {
            DateTime L0Date = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID, CurrentDate);
            DateTime L1Date = GetNavData(FrequencyID, '1', 1);
            if (L1Date > L0Date) DisableNavigationButton(e, "L1");
        }

        protected void rFrequency_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
            LastFrequencyMetric = (MetricTrac.Bll.FrequencyMetric)e.Item.DataItem;

            foreach (MetricTrac.Bll.MetricOrgValue mmv in LastFrequencyMetric.Metrics)
            {
                if(string.IsNullOrEmpty(mmv.InputUnitOfMeasureName)) continue;
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
            BindRepiter("rMetric", LastFrequencyMetric.Metrics, e);            
        }

        private DateTime GetNavData(int FrequencyID, char NavIndex, int step)
        {
            DateTime nd = FrequencyFirstDate[FrequencyID];
            int year = nd.Year;
            if (FrequencyID == (int)MetricTrac.Bll.Frequency.FrequencyName.FiscalAnnual ||
                FrequencyID == (int)MetricTrac.Bll.Frequency.FrequencyName.FiscalBiAnnual)
            {
                MetricTrac.Bll.Frequency.GetFiscalYear(nd, out year);
            }
            DateTime d = DateTime.MinValue;

            switch (NavIndex)
            {
                case '0':
                    d = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID, CurrentDate);
                    break;

                case '1':
                    d = MetricTrac.Bll.Frequency.AddPeriod(nd, FrequencyID, step);
                    break;

                case '2':
                    switch ((MetricTrac.Bll.Frequency.FrequencyName)FrequencyID)
                    {
                        case MetricTrac.Bll.Frequency.FrequencyName.BiAnnual:
                        case MetricTrac.Bll.Frequency.FrequencyName.FiscalBiAnnual:
                            d = nd.AddYears(((year - 1) % 4 == 0) ? 4 * step : 2 * step);
                            break;

                        default:
                            d = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID + 1, nd);
                            d = MetricTrac.Bll.Frequency.AddPeriod(d, FrequencyID + 1, step + 1);
                            d = d.AddDays(-1);
                            d = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID, d);
                            break;
                    }
                    break;

                case '3':
                    switch ((MetricTrac.Bll.Frequency.FrequencyName)FrequencyID)
                    {
                        case MetricTrac.Bll.Frequency.FrequencyName.Annual:
                        case MetricTrac.Bll.Frequency.FrequencyName.FiscalAnnual:
                            d = nd.AddYears(((year - 1) % 4) * (-Math.Sign(step)));
                            d = d.AddYears(4 * step);
                            break;

                        case MetricTrac.Bll.Frequency.FrequencyName.BiAnnual:
                        case MetricTrac.Bll.Frequency.FrequencyName.FiscalBiAnnual:
                            d = nd.AddYears(((year - 1) % 8 == 0) ? 8 * step : 4 * step);
                            break;

                        default:
                            d = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID + 2, nd);
                            d = MetricTrac.Bll.Frequency.AddPeriod(d, FrequencyID + 2, step + 1);
                            d = d.AddDays(-1);
                            d = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID, d);
                            break;
                    }
                    break;
            }
            return d;
        }

        protected void rapFrequency_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            string Command = e.Argument;
            if (string.IsNullOrEmpty(Command) || Command.Length < 3) return;

            if (Command[0] != 'L' && Command[0] != 'R') return;
            int step = Command[0] == 'L' ? 1 : -1;

            int FrequencyID;
            if (!int.TryParse(Command.Substring(2), out FrequencyID)) return;

            DateTime d = GetNavData(FrequencyID, Command[1], step);
            FrequencyFirstDate[FrequencyID] = d;
        }

        MetricTrac.Bll.MetricOrgValue GetMetric(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            return ri.DataItem as MetricTrac.Bll.MetricOrgValue;
        }

        protected bool IsNewGroup(object container)
        {
            MetricTrac.Bll.MetricOrgValue mmv = GetMetric(container);
            if(LastMetricMetricValue == null) return true;
            if (LastMetricMetricValue.FrequencyID != mmv.FrequencyID) return true;
            if (GroupByMetric) return  LastMetricMetricValue.MetricID != mmv.MetricID;
            return LastMetricMetricValue.OrgLocationID != mmv.OrgLocationID;
        }

        System.Web.UI.HtmlControls.HtmlTableCell [] LastTdUnit = new System.Web.UI.HtmlControls.HtmlTableCell[2];
        int LocationCount;
        protected void rpMetric_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;

            MetricTrac.Bll.MetricOrgValue NewMMV = (MetricTrac.Bll.MetricOrgValue)e.Item.DataItem;
            // !!! just remove rowspan for now
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

        protected void rDateTop_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
        }

        protected void rDate_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
        }

        protected void rMericValue_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
        }

        private string GetGrayColor(int g)
        {
            string s = g.ToString("X2");
            return "#" + s + s + s;
        }


        protected void ddlColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        protected string GetRelatedUrl(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            MetricTrac.Bll.MetricOrgValue m = ri.DataItem as MetricTrac.Bll.MetricOrgValue;
            return (DataMode == Mode.View ? "RelatedInputValues" : "RelatedInputValuesA") + ".aspx?MetricID=" + m.MetricID;
        }


        MetricTrac.Bll.MetricValue.Extend GetValue(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            return ri.DataItem as MetricTrac.Bll.MetricValue.Extend;
        }

        protected string GetEditUrl(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            int BackPageIndex = 4;
            switch (DataMode)
            {
                case Mode.Input:
                    BackPageIndex = 3;
                    break;
                case Mode.View:
                    BackPageIndex = 4;
                    break;
                case Mode.Approve:
                    BackPageIndex = 5;
                    break;
                default:
                    BackPageIndex = 4;
                    break;
            }
            return "MetricInputEdit.aspx?MetricID=" + val.MetricID +
                "&Date=" + val.Date.ToString("MM-dd-yyyy") +
                "&OrgLocationID=" + LastMetricMetricValue.OrgLocationID +
                "&Mode=" + DataMode.ToString() +
                "&BackPage=" + BackPageIndex.ToString();
        }

        protected string GetValueTitle(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            string title = String.Empty;
            if ((val.Value != null) && !(DataMode == Mode.Approve && val.Approved == true))
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

        protected string GetChartUrl(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            MetricTrac.Bll.MetricOrgValue v = (MetricTrac.Bll.MetricOrgValue)ri.DataItem;
            string FreqDate = FrequencyFirstDate[v.FrequencyID].ToString("MM-dd-yyyy");
            return "DataValueChart.aspx?MetricID=" + v.MetricID + "&OrgLocationID=" + v.OrgLocationID + "&Date=" + FreqDate;
        }

        protected bool IsEditValue(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            if (DataMode == Mode.Input)
                return val.CollectionEnabled;
            else
                return (val.Value != null && !(val.Approved == true && DataMode == Mode.Approve));
        }

        protected bool IsEmptyValue(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            return (val.Value == null || (DataMode == Mode.Approve && val.Approved == true));
        }

        protected bool IsCalculatedValue(object container)
        {
            return GetValue(container).IsCalculated==true;
        }

        protected bool IsApprovedValue(object container)
        {
            return GetValue(container).Approved == true;
        }

        protected bool IsFilesAttachedValue(object container)
        {
            return GetValue(container).FilesAttached;
        }

        protected string GetCalcStatusStyle(object container)
        {
            string CalcStyle = String.Empty;
            if (IsCalculatedValue(container))
                CalcStyle = GetValue(container).MissedCalc ? "style=\"color:Red;\"" : "style=\"color:Olive\"";
            return CalcStyle;
        }

        protected string GetValueCss(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);                        
            string ret = "class='";
            if (DataMode == Mode.Approve && val.Approved == true)
                return "class='empty-css'";
            if (val.IsCalculated == true && val.MissedCalc)
                ret += "rick";
            else
            {
                if ((val.Approved == false) && !val.FilesAttached)
                    return "class='empty-css'";
                if (val.Approved == true)
                    ret += "tick";
                else
                    if (val.Approved == null)
                    {
                        ret += "rick";
                        if (val.ReviewUpdated)
                            ret += "-up";
                    }
            }
            if (val.FilesAttached)
            {
                if (val.Approved != false) ret += "-";
                ret += "clip";
            }
            ret += "'";
            return ret;
        }

        protected string GetOnClickHandler(object container)
        {
            string OnClickHandler = String.Empty;
            if (MetricTrac.Utils.MetricUtils.IsPopupSupported(Request))
                OnClickHandler = "onclick=\"openRadWindow('" + GetEditUrl(container) + "');return false;\"";
            return OnClickHandler;
        }

        protected string GetValueCell(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            return Bll.MetricValue.FormatValue(val, (DataMode == Mode.Approve && val.Approved == true));
        }

        private Guid? SelMetricID
        {
            get { return (Guid?)ViewState["SelMetricID"]; }
            set { ViewState["SelMetricID"] = value; }
        }
        Guid? SelPiID
        {
            get { return (Guid?)ViewState["SelPiID"]; }
            set { ViewState["SelPiID"] = value; }
        }
        Guid? SelPifID
        {
            get { return (Guid?)ViewState["SelPifID"]; }
            set { ViewState["SelPifID"] = value; }
        }
        Guid? SelCollectorUseID
        {
            get { return (Guid?)ViewState["SelCollectorUseID"]; }
            set { ViewState["SelCollectorUseID"] = value; }
        }
        Guid? SelGcaID
        {
            get { return (Guid?)ViewState["SelGcaID"]; }
            set { ViewState["SelGcaID"] = value; }
        }
        Guid? SelOrgLocationID
        {
            get { return (Guid?)ViewState["SelOrgLocationID"]; }
            set { ViewState["SelOrgLocationID"] = value; }
        }

        Guid?[] SelOrgLocationsID
        {
            get { return (Guid?[])ViewState["SelOrgLocationsID"]; }
            set { ViewState["SelOrgLocationsID"] = value; }
        }
        Guid? ApproverUserId
        {
            get { return (Guid?)ViewState["ApproverUserId"]; }
            set { ViewState["ApproverUserId"] = value; }
        }
        protected void MF_Use(object sender, EventArgs e)
        {
            SelPifID = MF.PIFormID;
            SelPiID = MF.PIID;
            SelCollectorUseID = MF.DataCollectorID;
            SelMetricID = MF.MetricID;
            SelGcaID = MF.GCAID;
            SelOrgLocationsID = MF.SelectOrgLocationsID;
            FrequencyFirstDate.Clear();
        }

        protected void MF_GroupChanged(object sender, EventArgs e)
        {

        }
    }
}
