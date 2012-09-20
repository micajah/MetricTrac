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
    public partial class UnderReviewQueue : System.Web.UI.UserControl
    {
        MetricTrac.Bll.DistinctFrequencyMetric LastFrequencyMetric;
        MetricTrac.Bll.DistinctMetricOrgValue LastMetricOrgValue;

        public MetricInputList.Mode DataMode { get; set; }

        DateTime CurrentDate = DateTime.Now;

        Dictionary<Bll.FrequencyMetricOrgLocationID, DateTime> FrequencyFirstDate;

        readonly string FrequencyFirstDateViewState = "qFrequencyFirstDate";

        public bool GroupByMetric { get; set; }        
        
        protected void Page_Load(object sender, EventArgs e)
        {            
            object o = ViewState[FrequencyFirstDateViewState];
            if (o is Dictionary<Bll.FrequencyMetricOrgLocationID, DateTime>) FrequencyFirstDate = (Dictionary<Bll.FrequencyMetricOrgLocationID, DateTime>)o;
            else FrequencyFirstDate = new Dictionary<Bll.FrequencyMetricOrgLocationID, DateTime>();
            //((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricInput.css");
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/UnderReview.css");
        }       

        protected void Page_Prerender(object sender, EventArgs e)
        {
            ReBind();
            ViewState[FrequencyFirstDateViewState] = FrequencyFirstDate;            
            if (rapFrequency.IsAjaxRequest) rapFrequency.ResponseScripts.Add("qrapFrequency_ResponseEnd()");
            if (LastFrequencyMetric == null) lblAlert.Visible = false;
        }

        void ReBind()
        {
            Guid? ApproverUserId = null;
            Guid? SelCollectorUseID = null;
            if (DataMode == MetricInputList.Mode.Approve) ApproverUserId = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;
            if (DataMode == MetricInputList.Mode.Input) SelCollectorUseID = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;

            List<MetricTrac.Bll.DistinctFrequencyMetric> fms = MetricTrac.Bll.MetricValue.AlertQueueList(CurrentDate, FrequencyFirstDate, SelCollectorUseID, ApproverUserId, DataMode == MetricInputList.Mode.View || DataMode == MetricInputList.Mode.Approve, GroupByMetric);
            foreach (MetricTrac.Bll.DistinctFrequencyMetric fm in fms)
                foreach (MetricTrac.Bll.DistinctMetricOrgValue mmv in fm.Metrics)
                    mmv.OrgLocationFullName = MetricTrac.Bll.Mc_EntityNode.GetHtmlFullName(mmv.OrgLocationFullName);
            rFrequency.DataSource = fms;
            rFrequency.DataBind();
        }

        protected void rFrequency_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
            LastFrequencyMetric = (MetricTrac.Bll.DistinctFrequencyMetric)e.Item.DataItem;

            foreach (MetricTrac.Bll.DistinctMetricOrgValue mmv in LastFrequencyMetric.Metrics)
            {
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
            BindRepiter("rMetric", LastFrequencyMetric.Metrics, e);
        }
        
        protected void rpMetric_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
            LastMetricOrgValue = (MetricTrac.Bll.DistinctMetricOrgValue)e.Item.DataItem;
            BindRepiter("rDate", LastMetricOrgValue.DatesHeader, e);
            Bll.FrequencyMetricOrgLocationID curFrequencyMetricOrgLocationID = new MetricTrac.Bll.FrequencyMetricOrgLocationID();
            curFrequencyMetricOrgLocationID.FrequencyID = LastFrequencyMetric.FrequencyID;
            curFrequencyMetricOrgLocationID.MetricID = LastMetricOrgValue.MetricID;
            curFrequencyMetricOrgLocationID.OrgLocationID = LastMetricOrgValue.OrgLocationID;
            SetNavigationButton(e, curFrequencyMetricOrgLocationID);
            BindRepiter("rMericValue", LastMetricOrgValue.MetricValues, e);
        }

        private void BindRepiter(string rID, object DS, RepeaterItemEventArgs e)
        {
            Repeater r = (Repeater)e.Item.FindControl(rID);
            r.DataSource = DS;
            r.DataBind();
        }

        // === End Bind section ===

        // --- Navigation ---
        void SetNavigationButton(RepeaterItemEventArgs e, Bll.FrequencyMetricOrgLocationID curFrequencyMetricOrgLocationID)
        {
            if (!FrequencyFirstDate.Keys.Contains(curFrequencyMetricOrgLocationID))
                DisableNavigationButton(e, "L1");
            if (!LastMetricOrgValue.IsPreviousValues)
                DisableNavigationButton(e, "R1");        
        }

        void DisableNavigationButton(RepeaterItemEventArgs e, string IDSuffix)
        {
            Image i = (Image)e.Item.FindControl("iNav" + IDSuffix);
            i.ImageUrl = i.ImageUrl.Replace(".png", "-disabled.png");
            HyperLink hl = (HyperLink)e.Item.FindControl("hlNav" + IDSuffix);
            hl.NavigateUrl = string.Empty;
        }
       
        // ---- AJAX request --------------------

        protected void rapFrequency_AjaxRequest(object sender, Telerik.Web.UI.AjaxRequestEventArgs e)
        {
            string Command = e.Argument;
            if (string.IsNullOrEmpty(Command) || Command.Length < 3) return;

            string[] ArgArray = Command.Split('|');
            if (ArgArray.Length < 7) return;

            if (Command[0] != 'L' && Command[0] != 'R') return;
            int step = Command[0] == 'L' ? 1 : -1;
            
            int FrequencyID;
            if (!int.TryParse(ArgArray[1], out FrequencyID)) return;

            Bll.FrequencyMetricOrgLocationID CurTriplet = new MetricTrac.Bll.FrequencyMetricOrgLocationID();
            CurTriplet.FrequencyID = FrequencyID;

            try
            {
                Guid MetricID = new Guid(ArgArray[2]);

                Guid OrgLocationID = new Guid(ArgArray[3]);

                CurTriplet.MetricID = MetricID;
                CurTriplet.OrgLocationID = OrgLocationID;

                DateTime PreviousDate = DateTime.Parse(ArgArray[4]);
                DateTime NextDate = DateTime.Parse(ArgArray[5]);

                bool IsNextValues = bool.Parse(ArgArray[6]);

                DateTime NewDate = step > 0 ? NextDate : PreviousDate;

                if (!IsNextValues && (step > 0))
                {
                    if (FrequencyFirstDate.Keys.Contains(CurTriplet))
                        FrequencyFirstDate.Remove(CurTriplet);
                }
                else
                {
                    if (FrequencyFirstDate.Keys.Contains(CurTriplet))
                        FrequencyFirstDate[CurTriplet] = NewDate;
                    else
                        FrequencyFirstDate.Add(CurTriplet, NewDate);
                }
            }
            catch { return; }    
            
        }

        //-------- ASCX inline methods -------------
            // auxiliary methods
        MetricTrac.Bll.MetricValue.Extend GetValue(object container) 
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            return ri.DataItem as MetricTrac.Bll.MetricValue.Extend;
        }
            // ------------

        protected bool IsEmptyValue(object container)
        {
            return GetValue(container).Value == null;
        }

        protected string GetEditUrl(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            int BackPageIndex = 4;
            switch (DataMode)
            {
                case MetricInputList.Mode.Input:
                    BackPageIndex = 3;
                    break;
                case MetricInputList.Mode.View:
                    BackPageIndex = 4;
                    break;
                case MetricInputList.Mode.Approve:
                    BackPageIndex = 5;
                    break;
                default:
                    BackPageIndex = 4;
                    break;
            }
            return "MetricInputEdit.aspx?MetricID=" + val.MetricID +
                "&Date=" + val.Date.ToString("MM-dd-yyyy") +
                "&OrgLocationID=" + LastMetricOrgValue.OrgLocationID +
                "&Mode=" + DataMode.ToString() +
                "&BackPage=" + BackPageIndex.ToString();
        }

        protected string GetOnClickHandler(object container)
        {
            string OnClickHandler = String.Empty;
            if (MetricTrac.Utils.MetricUtils.IsPopupSupported(Request))
                OnClickHandler = "onclick=\"qopenRadWindow('" + GetEditUrl(container) + "');return false;\"";
            return OnClickHandler;
        }

        protected bool IsEditValue(object container)
        {            
            return GetValue(container).Value != null;
        }

        protected bool IsCalculatedValue(object container)
        {
            return GetValue(container).IsCalculated==true;
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
            if (val.IsCalculated == true && val.MissedCalc)
                ret += "rick";
            else
            {
                if ((val.Approved == false) && !val.FilesAttached) return string.Empty;

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

        protected string GetValueCell(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            return Bll.MetricValue.FormatValue(val);
        }
        // navigation
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

        public bool IsInternalAjax
        {
            get { return rapFrequency.IsAjaxRequest; }
        }
    }
}