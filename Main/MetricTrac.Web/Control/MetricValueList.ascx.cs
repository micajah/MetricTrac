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
using MetricTrac.Bll;
using Telerik.Web.UI;

namespace MetricTrac.Control
{
    public partial class MetricValueList : System.Web.UI.UserControl
    {
        public enum Mode { Input, View, Approve }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                MF.BaseDate = DateTime.Now;
            ShiftDates = ShiftDatesSave;
            pnlAlert.Visible = !(MF.Visible = DataMode == Mode.View);
            AlertQueue.DataMode = MetricInputList.Mode.View; //this.DataMode;
            AlertQueue.GroupByMetric = this.GroupByMetric;            
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricInput.css");
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {            
            if (!MF.IsInternalAjax && !AlertQueue.IsInternalAjax)
                ReBind();
            ShiftDatesSave = ShiftDates;
            if (rapFrequency.IsAjaxRequest) rapFrequency.ResponseScripts.Add("rapFrequency_ResponseEnd()");
            lbDebugInfo.Text = "Debug Info: TotalMilliseconds=" + ((TimeSpan)(DateTime.Now - StartTime)).TotalMilliseconds;
        }

        private void ReBind()
        {
            if (DataMode == Mode.Approve) ApproverUserId = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;
            if (DataMode == Mode.Input) SelCollectorUseID = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;


            List<MetricTrac.Bll.FrequencyMetric> fms = MetricTrac.Bll.MetricValue.exList(6, CurrentDate, ShiftDates, SelMetricID,
                SelOrgLocationsID, SelGcaID, SelPiID, DataMode == Mode.View || DataMode == Mode.Approve, GroupByMetric, SelCollectorUseID, ApproverUserId);
            ((MetricTrac.MasterPage)(Page.Master)).AddDebufInfo("SqlQueryTime", MetricTrac.Bll.MetricValue.SqlQueryTime.ToString());
            ((MetricTrac.MasterPage)(Page.Master)).AddDebufInfo("TotalTime", MetricTrac.Bll.MetricValue.TotalTime.ToString());

            foreach (MetricTrac.Bll.FrequencyMetric fm in fms)
                foreach (MetricTrac.Bll.MetricOrgValue mmv in fm.Metrics)
                    mmv.OrgLocationFullName = MetricTrac.Bll.Mc_EntityNode.GetHtmlFullName(mmv.OrgLocationFullName);
            rFrequency.DataSource = fms;
            rFrequency.DataBind();
        }

        #region Handlers
        protected void rFrequency_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
            LastFrequencyMetric = (MetricTrac.Bll.FrequencyMetric)e.Item.DataItem;

            foreach (MetricTrac.Bll.MetricOrgValue mmv in LastFrequencyMetric.Metrics)
            {
                if (string.IsNullOrEmpty(mmv.InputUnitOfMeasureName)) continue;
                for (int i = mmv.InputUnitOfMeasureName.Length - 1; i > 0; i--)
                    mmv.InputUnitOfMeasureName = mmv.InputUnitOfMeasureName.Insert(i, "<br />");
            }
            BindRepiter("rMetric", LastFrequencyMetric.Metrics, e);
        }

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

                BindRepiter("rDate", /*LastFrequencyMetric*/LastMetricMetricValue.Date, e);

                DateTime L0Date = MetricTrac.Bll.Frequency.GetNormalizedDate(LastFrequencyMetric.FrequencyID, CurrentDate);

                DateTime L1Date = MetricTrac.Bll.Frequency.GetNormalizedDate(LastFrequencyMetric.FrequencyID, CurrentDate);
                foreach (MetricValue.ShiftDate s in ShiftDates)
                    if (s.FrequencyID == LastFrequencyMetric.FrequencyID && s.EntityID == (GroupByMetric ? LastMetricMetricValue.MetricID : LastMetricMetricValue.OrgLocationID))
                    {
                        L1Date = s.StartDate;
                        break;
                    }
                L1Date = MetricTrac.Bll.Frequency.AddPeriod(L1Date, LastFrequencyMetric.FrequencyID, 1);
                if (L1Date > L0Date)
                {
                    Image i = (Image)e.Item.FindControl("iNav" + "L1");
                    i.ImageUrl = i.ImageUrl.Replace(".png", "-disabled.png");
                    HyperLink hl = (HyperLink)e.Item.FindControl("hlNav" + "L1");
                    hl.NavigateUrl = string.Empty;
                }
            }
            else
            {
                LocationCount++;
                LastTdUnit[0].RowSpan = LocationCount;
                LastTdUnit[1].RowSpan = LocationCount;
            }

            BindRepiter("rMericValue", NewMMV.MetricValues, e);
        }

        protected void rapFrequency_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            string Command = e.Argument;
            if (string.IsNullOrEmpty(Command) || Command.Length < 3) return;

            if (Command[0] != 'L' && Command[0] != 'R') return;

            string[] array = Command.Split('|');
            if (array != null)
                if (array.Length == 3)
                {
                    int step = array[0] == "L" ? 1 : -1;

                    int FrequencyID;
                    if (!int.TryParse(array[1], out FrequencyID)) return;

                    Guid EntityID = Guid.Empty;
                    try { EntityID = new Guid(array[2]); }
                    catch { } 
                                       
                    MetricValue.ShiftDate sd = new MetricValue.ShiftDate
                    {
                        FrequencyID = FrequencyID,
                        StartDate = MetricTrac.Bll.Frequency.AddPeriod(Frequency.GetNormalizedDate(FrequencyID, CurrentDate), FrequencyID, step),
                        EntityID = EntityID
                    };
                    bool ExistShift = false;
                    foreach (MetricValue.ShiftDate s in ShiftDates)
                        if (s.FrequencyID == sd.FrequencyID && s.EntityID == sd.EntityID)
                        {
                            ExistShift = true;
                            s.StartDate = MetricTrac.Bll.Frequency.AddPeriod(s.StartDate, FrequencyID, step);
                            break;
                        }
                    if (!ExistShift)
                        ShiftDates.Add(sd);
                }
        }
        
        protected void MF_Use(object sender, EventArgs e)
        {
            SelPiID = MF.SelectedPerformanceIndicators;
            SelCollectorUseID = MF.SelectedDataCollector;
            SelMetricID = MF.SelectedMetrics;
            SelGcaID = MF.SelectedGroupCategoryAspect;
            SelOrgLocationsID = MF.SelectedOrgLocations;
            ShiftDates.Clear();
        }
        #endregion

        #region Private Methods
        private void BindRepiter(string rID, object DS, RepeaterItemEventArgs e)
        {
            Repeater r = (Repeater)e.Item.FindControl(rID);
            r.DataSource = DS;
            r.DataBind();
        }
        #endregion

        #region Rendering methods
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
            if (DataMode == Mode.View && val.IsTotalAgg)
            {
                title = "Total value";
            }
            else if ((val.Value != null) && !(DataMode == Mode.Approve && val.Approved == true))
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
            string FreqDate = "cskckj";//FrequencyFirstDate[v.FrequencyID].ToString("MM-dd-yyyy");
            return "DataValueChart.aspx?MetricID=" + v.MetricID + "&OrgLocationID=" + v.OrgLocationID + "&Date=" + FreqDate;
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

        protected bool IsEditValue(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            if (DataMode == Mode.Input)
                return val.CollectionEnabled;
            else
                return (val.Value != null && !(val.Approved == true && DataMode == Mode.Approve) && !(val.IsTotalAgg == true && DataMode == Mode.View));
        }

        protected bool IsTotalMetric(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            MetricTrac.Bll.MetricOrgValue mmv = ri.DataItem as MetricTrac.Bll.MetricOrgValue;
            return mmv.IsTotalAgg;            
        }

        protected bool IsTotalValue(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            return val.IsTotalAgg;
        }

        protected bool IsEmptyValue(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            return (val.Value == null || (DataMode == Mode.Approve && val.Approved == true));
        }

        protected bool IsCalculatedValue(object container)
        {
            return GetValue(container).IsCalculated == true;
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
        #endregion

        #region Filter Properties
        private List<MetricValue.ShiftDate> ShiftDates = null;
        private List<MetricValue.ShiftDate> ShiftDatesSave
        {
            get
            {  
                object o = ViewState["FrequencyFirstDate"];
                return o != null ? (List<MetricValue.ShiftDate>)ViewState["FrequencyFirstDate"] : new List<MetricValue.ShiftDate>(); ;
            }
            set { ViewState["FrequencyFirstDate"] = value; }
        }

        private Guid?[] SelMetricID
        {
            get { return (Guid?[])ViewState["SelMetricID"]; }
            set { ViewState["SelMetricID"] = value; }
        }
        private Guid?[] SelPiID
        {
            get { return (Guid?[])ViewState["SelPiID"]; }
            set { ViewState["SelPiID"] = value; }
        }
        private Guid? SelCollectorUseID
        {
            get { return (Guid?)ViewState["SelCollectorUseID"]; }
            set { ViewState["SelCollectorUseID"] = value; }
        }
        private Guid? SelGcaID
        {
            get { return (Guid?)ViewState["SelGcaID"]; }
            set { ViewState["SelGcaID"] = value; }
        }
        private Guid?[] SelOrgLocationsID
        {
            get { return (Guid?[])ViewState["SelOrgLocationsID"]; }
            set { ViewState["SelOrgLocationsID"] = value; }
        }
        private Guid? ApproverUserId
        {
            get { return (Guid?)ViewState["ApproverUserId"]; }
            set { ViewState["ApproverUserId"] = value; }
        }
        #endregion
        
        #region Private/Protected Properties
        protected RadAjaxManager AjaxManager
        {
            get { return RadAjaxManager.GetCurrent(this.Page); }
        }

        protected bool GroupByMetric
        {
            get { return GroupBy.GroupByMetric; }
        }

        private DateTime CurrentDate
        {
            get { return (DataMode == Mode.View && MF.BaseDate != DateTime.MinValue) ? MF.BaseDate : DateTime.Now; }
        }
        private DateTime StartTime = DateTime.Now;

        private MetricTrac.Bll.FrequencyMetric LastFrequencyMetric = null;
        private MetricTrac.Bll.MetricOrgValue LastMetricMetricValue = null;

        private System.Web.UI.HtmlControls.HtmlTableCell[] LastTdUnit = new System.Web.UI.HtmlControls.HtmlTableCell[2];
        private int LocationCount = 0;
        #endregion

        #region Public properties
        public Mode DataMode { get; set; }
        #endregion
    }
}