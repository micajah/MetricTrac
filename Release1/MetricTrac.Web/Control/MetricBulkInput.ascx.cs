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
using Telerik.Web.UI;
using MetricTrac.Bll;

namespace MetricTrac.Control
{    
    public partial class MetricBulkInput : System.Web.UI.UserControl
    {
        public enum Mode { Input, View, Approve }

        protected MetricTrac.Bll.FrequencyMetric LastFrequencyMetric;
        protected MetricTrac.Bll.MetricOrgValue LastMetricMetricValue;
     
        protected bool GroupByMetric { get { return MetricID != null; } }

        protected int FrequencyID
        {
            get
            {
                string s = Request.QueryString["FrequencyID"];
                if (string.IsNullOrEmpty(s)) return -1;
                int ID = -1;
                int.TryParse(s, out ID);                
                return ID;
            }
        }

        protected Guid? MetricID
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

        protected Guid? OrgLocationID
        {
            get
            {
                string s = Request.QueryString["OrgLocationID"];
                if (string.IsNullOrEmpty(s)) return null;
                Guid? ID = null;
                try { ID = new Guid(s); }
                catch { };
                return ID;
            }
        }

        public Mode DataMode { get; set;}
        
        DateTime CurrentDate = DateTime.Now;
        DateTime FrequencyFirstDate = DateTime.MinValue;
        readonly string FrequencyFirstDateViewState = "bFrequencyFirstDate";

        protected RadAjaxManager ramManager
        {
            get { return RadAjaxManager.GetCurrent(Page); }
        }

        DateTime DebugTime = DateTime.Now;
        string LogTime = String.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                phApproveButtons.Visible = !(phInputButtons.Visible = DataMode == Mode.Input);
            DebugTime = DateTime.Now;

            object o = ViewState[FrequencyFirstDateViewState];
            if (o is DateTime) FrequencyFirstDate = (DateTime)o;
            else FrequencyFirstDate = Bll.Frequency.GetNormalizedDate(FrequencyID, CurrentDate);
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/MetricInput.css");
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/BulkEdit.css");
            /*ramManager.UpdatePanelsRenderMode = UpdatePanelRenderMode.Inline;
            ramManager.ClientEvents.OnRequestStart = "OnRequestStart";
            ramManager.ClientEvents.OnResponseEnd = "OnResponseEnd";
            ramManager.RequestQueueSize = 1;*/
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            ReBind();
            PlaceHolder3.Visible = DataMode == Mode.Approve;
            ViewState[FrequencyFirstDateViewState] = FrequencyFirstDate;            
            /*foreach (RepeaterItem ri in rMetric.Items)
                if (ri.ItemType == ListItemType.Item || ri.ItemType == ListItemType.AlternatingItem)
                {
                    LinkButton hlNavL1 = (LinkButton)ri.FindControl("hlNavL1");
                    LinkButton hlNavR1 = (LinkButton)ri.FindControl("hlNavR1");
                    ramManager.AjaxSettings.AddAjaxSetting(hlNavL1, pnlUpdate, ralGrid);
                    ramManager.AjaxSettings.AddAjaxSetting(hlNavR1, pnlUpdate, ralGrid);
                }
            //ramManager.AjaxSettings.AddAjaxSetting(lbSave1, pnlUpdate, ralGrid);*/
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            //lbDebug.Text = "Debug Info: TotalMilliseconds=" + ((TimeSpan)(DateTime.Now - DebugTime)).TotalMilliseconds + "<br /><br /><br />" + LogTime;
        }

        void ReBind()
        {
            LastFrequencyMetric = GetData();
            foreach (MetricTrac.Bll.MetricOrgValue mmv in LastFrequencyMetric.Metrics)            
                mmv.OrgLocationFullName = MetricTrac.Bll.Mc_EntityNode.GetHtmlFullName(mmv.OrgLocationFullName);
            rMetric.DataSource = LastFrequencyMetric.Metrics;
            rMetric.DataBind();            
        }

        private FrequencyMetric GetData()
        {
            Guid? SelCollectorUseID = null;
            if (DataMode == Mode.Input)
                SelCollectorUseID = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;
            Guid? SelApproverUseID = null;
            if (DataMode == Mode.Approve)
                SelApproverUseID = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;
            return MetricTrac.Bll.MetricValue.BulkEditList(13, FrequencyID, FrequencyFirstDate, MetricID, OrgLocationID, SelCollectorUseID, SelApproverUseID, DataMode == Mode.Approve, DataMode != Mode.Approve);
        }

        protected void hlNavR1_Command(object sender, CommandEventArgs e)
        {
            string Command = e.CommandArgument.ToString();
            if (string.IsNullOrEmpty(Command)) return;
            if (Command[0] != 'L' && Command[0] != 'R') return;
            int step = Command[0] == 'L' ? 1 : -1;
            FrequencyFirstDate = MetricTrac.Bll.Frequency.AddPeriod(FrequencyFirstDate, FrequencyID, step);
        }
        
        protected void rpMetric_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
            MetricTrac.Bll.MetricOrgValue mov = (MetricTrac.Bll.MetricOrgValue)e.Item.DataItem;            
            if (LastMetricMetricValue == null)
            {
                BindRepiter("rDate", LastFrequencyMetric.Date, e);
                SetNavigationButton(e, FrequencyID);
            }
            LastMetricMetricValue = mov;

            System.Web.UI.WebControls.DropDownList ddlInputUnitOfMeasure = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlInputUnitOfMeasure");
            System.Web.UI.WebControls.Label lblUoM = (System.Web.UI.WebControls.Label)e.Item.FindControl("lblUoM");
            ddlInputUnitOfMeasure.Items.Clear();
            if (mov.RelatedOrgLocationUoMRecordID == null)
            { // no previous records exists                
                ddlInputUnitOfMeasure.Visible = true;
                lblUoM.Visible = false;
                if (mov.InputUnitOfMeasureID != null)
                {
                    List<Micajah.Common.Bll.MeasureUnit> l = Bll.Mc_UnitsOfMeasure.GetConvertedUoMs(mov.InputUnitOfMeasureID);
                    ddlInputUnitOfMeasure.DataSource = l;
                    ddlInputUnitOfMeasure.DataBind();
                    ddlInputUnitOfMeasure.SelectedValue = mov.InputUnitOfMeasureID.ToString();
                }
                else
                    ddlInputUnitOfMeasure.Items.Add(new ListItem("", ""));
            }
            else
            {// there are defined uom
                ddlInputUnitOfMeasure.Visible = false;                
                lblUoM.Visible = true;
            }
            BindRepiter("rMericValue", mov.MetricValues, e);
        }

        protected string ListsStringArray { set; get; }

        protected void rpMetricValue_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
            MetricTrac.Bll.MetricValue.Extend MVS = (MetricTrac.Bll.MetricValue.Extend)e.Item.DataItem;            
            Telerik.Web.UI.RadNumericTextBox rntValue = (Telerik.Web.UI.RadNumericTextBox)e.Item.FindControl("rntValue");
            System.Web.UI.WebControls.TextBox tbValue = (System.Web.UI.WebControls.TextBox)e.Item.FindControl("tbValue");
            System.Web.UI.WebControls.CheckBox chbValue = (System.Web.UI.WebControls.CheckBox)e.Item.FindControl("chbValue");
            Telerik.Web.UI.RadDatePicker rdpDateValue = (Telerik.Web.UI.RadDatePicker)e.Item.FindControl("rdpDateValue");
            System.Web.UI.WebControls.DropDownList ddlApprovalStatus = (System.Web.UI.WebControls.DropDownList)e.Item.FindControl("ddlApprovalStatus");            
            ddlApprovalStatus.Attributes.Add("onchange", "valueChanged();return false;");
            if ((DataMode == Mode.Approve) && (MVS.Value != null))
                ListsStringArray += ddlApprovalStatus.ClientID + "|";
            bool IsDataTypeChanged = false;
            bool IsNewRecord = false;
            if (MVS.MetricValueID == Guid.Empty)
                IsNewRecord = true;            
            else
                if (MVS.MetricDataTypeID != MVS.ActualMetricDataTypeID) 
                    IsDataTypeChanged = true;
            switch (LastMetricMetricValue.MetricDataTypeID)
            {
                case 2://Text
                    tbValue.Visible = true;
                    rntValue.Visible = chbValue.Visible = rdpDateValue.Visible = false;
                    tbValue.Text = String.Empty;
                    if (!IsDataTypeChanged)
                        tbValue.Text = MVS.Value;
                    break;
                case 3://Bool - checkbox
                    chbValue.Visible = true;
                    rntValue.Visible = tbValue.Visible = rdpDateValue.Visible = false;
                    chbValue.Checked = false;
                    if (!IsDataTypeChanged)
                        chbValue.Checked = MVS.Value == bool.TrueString;
                    break;
                case 4://Date
                    rdpDateValue.Visible = true;
                    rntValue.Visible = tbValue.Visible = chbValue.Visible = false;
                    rdpDateValue.SelectedDate = DateTime.Now;
                    DateTime _dt = DateTime.Now;
                    if (!IsDataTypeChanged)
                        if (DateTime.TryParse(MVS.Value, out _dt))
                            rdpDateValue.SelectedDate = _dt;
                    break;
                case 1://Numeric 
                default:
                    rntValue.Visible = true;
                    rntValue.ReadOnly = LastMetricMetricValue.MetricTypeID > 1;
                    tbValue.Visible = chbValue.Visible = rdpDateValue.Visible = false;
                    rntValue.Text = String.Empty;
                    if ((!IsDataTypeChanged) && ((MVS.UnitOfMeasureID == MVS.MetricUnitOfMeasureID) || IsNewRecord))
                    {
                        double _Value = double.NaN;
                        if (double.TryParse(MVS.Value, out _Value))
                            rntValue.Value = _Value;
                    }

                    // Range label
                    if ((MVS.NOMinValue != null) || (MVS.NOMaxValue != null) || (MVS.NODecPlaces != null))
                    {                        
                        int defPlaces = 2;
                        if (MVS.NODecPlaces != null)
                        {
                            rntValue.NumberFormat.DecimalDigits = (int)MVS.NODecPlaces;
                            defPlaces = (int)MVS.NODecPlaces;                     
                        }
                        if (MVS.NOMinValue != null)                        
                            rntValue.MinValue = decimal.ToDouble((decimal)MVS.NOMinValue);
                        if (MVS.NOMaxValue != null)                        
                            rntValue.MaxValue = decimal.ToDouble((decimal)MVS.NOMaxValue);
                    }                                        
                    break;
            }
        }

        private void BindRepiter(string rID, object DS, RepeaterItemEventArgs e)
        {
            Repeater r = (Repeater)e.Item.FindControl(rID);
            r.DataSource = DS;
            r.DataBind();
        }
        
        protected string GetEditUrl(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;            
            MetricTrac.Bll.MetricValue.Extend val = ri.DataItem as MetricTrac.Bll.MetricValue.Extend;
            int BackPageIndex = 1;
            switch (DataMode)
            {
                case Mode.Input:
                    BackPageIndex = 1;
                    break;
                case Mode.Approve:
                    BackPageIndex = 2;
                    break;
                default:
                    BackPageIndex = 1;
                    break;
            }
            return "MetricInputEdit.aspx?MetricID=" + val.MetricID +
                "&Date=" + val.Date.ToString("MM-dd-yyyy") +
                "&OrgLocationID=" + LastMetricMetricValue.OrgLocationID +
                "&Mode=" + DataMode.ToString() +
                "&BulkEdit=True" +
                "&BackPage=" + BackPageIndex.ToString();
        }

        

        protected string GetOnClickHandler(object container)
        {
            string OnClickHandler = String.Empty;
            if (MetricTrac.Utils.MetricUtils.IsPopupSupported(Request))
                OnClickHandler = "onclick=\"openRadWindow('" + GetEditUrl(container) + "'); activeEditElement = '" + GetClientId(container) + "'; activeSelectElement = '" + GetSelectClientId(container) + "'; activeEditElementType = '" + GetElementType(container) + "'; return false;\"";
            return OnClickHandler;
        }

        protected bool IsEditValue(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            MetricTrac.Bll.MetricValue.Extend val = ri.DataItem as MetricTrac.Bll.MetricValue.Extend;
            return ((DataMode == Mode.Input) ? val.CollectionEnabled : (val.Value != null));
        }

        protected string GetClientId(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            MetricTrac.Bll.MetricValue.Extend val = ri.DataItem as MetricTrac.Bll.MetricValue.Extend;
            string ClientId = String.Empty;
            switch (val.MetricDataTypeID)
            {
                case 2:
                    System.Web.UI.WebControls.TextBox tbValue = (System.Web.UI.WebControls.TextBox)ri.FindControl("tbValue");
                    ClientId = tbValue.ClientID;
                    break;
                case 3:
                    System.Web.UI.WebControls.CheckBox chbValue = (System.Web.UI.WebControls.CheckBox)ri.FindControl("chbValue");
                    ClientId = chbValue.ClientID;
                    break;
                case 4:
                    Telerik.Web.UI.RadDatePicker rdpDateValue = (Telerik.Web.UI.RadDatePicker)ri.FindControl("rdpDateValue");
                    ClientId = rdpDateValue.ClientID;
                    break;
                case 1:
                default:
                    Telerik.Web.UI.RadNumericTextBox rntValue = (Telerik.Web.UI.RadNumericTextBox)ri.FindControl("rntValue");
                    ClientId = rntValue.ClientID;
                    break;
            }
            return ClientId;
        }

        protected string GetSelectClientId(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            System.Web.UI.WebControls.DropDownList ddlApprovalStatus = (System.Web.UI.WebControls.DropDownList)ri.FindControl("ddlApprovalStatus");
            return ddlApprovalStatus.ClientID;
        }

        protected int GetElementType(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            MetricTrac.Bll.MetricValue.Extend val = ri.DataItem as MetricTrac.Bll.MetricValue.Extend;
            return val.MetricDataTypeID;
        }
        //--------------

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
            LinkButton hl = (LinkButton)e.Item.FindControl("hlNav" + IDSuffix);
            hl.Enabled = false;            
        }
        
        protected string GetTitle(string CommandName)
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

        /*protected void lbSave_Click(object sender, EventArgs e)
        {
            int i = 0;
            LastFrequencyMetric = GetData();
            foreach (RepeaterItem ri in rMetric.Items)
            {
                MetricOrgValue mov = LastFrequencyMetric.Metrics[i];                
                DateTime iTime = DateTime.Now;
                DropDownList ddlInputUnitOfMeasure = (DropDownList)ri.FindControl("ddlInputUnitOfMeasure");
                HiddenField hfUoM = (HiddenField)ri.FindControl("hfUoM");
                HiddenField hfMetric = (HiddenField)ri.FindControl("hfMetric");
                HiddenField hfOrgLocation = (HiddenField)ri.FindControl("hfOrgLocation");
                Repeater r = (Repeater)ri.FindControl("rMericValue");

                Guid? ActualUoMID = null;
                if (ddlInputUnitOfMeasure.Visible)
                {
                    if (!String.IsNullOrEmpty(ddlInputUnitOfMeasure.SelectedValue))
                        ActualUoMID = new Guid(ddlInputUnitOfMeasure.SelectedValue);
                }
                else
                {
                    if (!String.IsNullOrEmpty(hfUoM.Value))
                        ActualUoMID = new Guid(hfUoM.Value);
                }
                Guid oMetricID = new Guid(hfMetric.Value);
                Guid oOrgLocationID = new Guid(hfOrgLocation.Value);
                int j = 0;                
                foreach (RepeaterItem rri in r.Items)
                {                    
                    DateTime jTime = DateTime.Now;
                    HiddenField hfDate = (HiddenField)rri.FindControl("hfDate"); 
                    DateTime oDate = DateTime.Parse(hfDate.Value);                    
                    Telerik.Web.UI.RadNumericTextBox rntValue = (Telerik.Web.UI.RadNumericTextBox)rri.FindControl("rntValue");
                    System.Web.UI.WebControls.TextBox tbValue = (System.Web.UI.WebControls.TextBox)rri.FindControl("tbValue");
                    System.Web.UI.WebControls.CheckBox chbValue = (System.Web.UI.WebControls.CheckBox)rri.FindControl("chbValue");
                    Telerik.Web.UI.RadDatePicker rdpDateValue = (Telerik.Web.UI.RadDatePicker)rri.FindControl("rdpDateValue");
                    System.Web.UI.WebControls.DropDownList ddlApprovalStatus = (System.Web.UI.WebControls.DropDownList)rri.FindControl("ddlApprovalStatus");

                    Bll.MetricValue.Extend OldMetricValue = mov.MetricValues[j];                    
                    string Value = String.Empty;
                    if (rntValue.Visible)
                        Value = rntValue.Value.ToString();
                    else
                        if (tbValue.Visible)
                            Value = tbValue.Text;
                        else
                            if (chbValue.Visible)
                                Value = chbValue.Checked ? bool.TrueString : bool.FalseString;
                            else
                                if (rdpDateValue.Visible)
                                    Value = rdpDateValue.SelectedDate.ToString();

                    bool? Approved = OldMetricValue.Approved;
                    if (DataMode == Mode.Approve)
                        switch (ddlApprovalStatus.SelectedValue)
                        {
                            case "":
                                Approved = null;
                                break;
                            case "True":
                                Approved = true;
                                break;
                            case "False":
                            default:
                                Approved = false;
                                break;
                        }

                    if (((!String.IsNullOrEmpty(Value) || (OldMetricValue.MetricValueID != Guid.Empty && String.IsNullOrEmpty(Value))) && Value != OldMetricValue.Value)
                        || 
                        (DataMode == Mode.Approve && Approved != OldMetricValue.Approved))
                    {                        
                        Guid CurentUserId = Micajah.Common.Security.UserContext.Current.UserId;                        
                        Guid _ValueID = Bll.MetricValue.InsertOrUpdate(
                            oMetricID,
                            oDate,
                            oOrgLocationID,
                            OldMetricValue.FilesAttached,
                            false,
                            ActualUoMID,
                            OldMetricValue.Value,
                            Value,
                            OldMetricValue.Approved,
                            Approved,
                            CurentUserId,
                            OldMetricValue.Notes,
                            null,
                            null);

                        Bll.MetricValue.Extend NewMetricValue = Bll.MetricValue.Get(oMetricID, oDate, oOrgLocationID);
                        Bll.Mc_User.Extend mue = Bll.Mc_User.GetValueInputUser(OldMetricValue.MetricValueID);
                        // build mail to data collector if status or comment were changed
                        if ((DataMode == Mode.Approve) && (OldMetricValue.Approved != NewMetricValue.Approved))
                        {
                            Bll.MetricValueChangeLog.LogChange(NewMetricValue.MetricValueID,
                                Bll.MetricValueChangeTypeEnum.StatusChanged,
                                OldMetricValue.ApprovalStatus,
                                NewMetricValue.ApprovalStatus,
                                Utils.Mail.BuildLogMessageBody(OldMetricValue, NewMetricValue, String.Empty, Micajah.Common.Security.UserContext.Current, mue, Bll.MetricValueChangeTypeEnum.StatusChanged));                            
                            if (NewMetricValue.Approved == null)
                                Utils.Mail.Send(mue.Email, mue.FullName, "MetricTrac - Value Status is changed", Utils.Mail.BuildEmailBody(OldMetricValue, NewMetricValue, String.Empty, Micajah.Common.Security.UserContext.Current));
                        }
                        // record in change log
                        if (OldMetricValue.MetricValueID == Guid.Empty)
                            Bll.MetricValueChangeLog.LogChange(NewMetricValue.MetricValueID,
                                MetricTrac.Bll.MetricValueChangeTypeEnum.ValueEntered,
                                String.Empty,
                                NewMetricValue.Value,
                                Utils.Mail.BuildLogMessageBody(OldMetricValue, NewMetricValue, "Bulk Edit", Micajah.Common.Security.UserContext.Current, mue, MetricTrac.Bll.MetricValueChangeTypeEnum.ValueEntered));
                        else
                            if (OldMetricValue.Value != NewMetricValue.Value)
                                Bll.MetricValueChangeLog.LogChange(OldMetricValue.MetricValueID,
                                    MetricTrac.Bll.MetricValueChangeTypeEnum.ValueChanged,
                                    OldMetricValue.Value,
                                    NewMetricValue.Value,
                                    Utils.Mail.BuildLogMessageBody(OldMetricValue, NewMetricValue, "Bulk Edit", Micajah.Common.Security.UserContext.Current, mue, MetricTrac.Bll.MetricValueChangeTypeEnum.ValueChanged));                        
                    }
                    LogTime += "    value " + j.ToString() + " time  " + ((TimeSpan)(DateTime.Now - jTime)).TotalMilliseconds;
                    j++;
                }
                LogTime += "Row " + i.ToString() + " time  " + ((TimeSpan)(DateTime.Now - iTime)).TotalMilliseconds + "<br />";
                i++;
            }
            if (DataMode == Mode.Input)
                Response.Redirect("MetricInputList.aspx");
            else
            {                
                if (Micajah.Common.Security.UserContext.Breadcrumbs.Exists(r => r.Name.Contains("Approve Data"))) // TODO: change to redirect to parent
                    Response.Redirect("ApproveDataList.aspx");
                else
                    Response.Redirect("ApproveWorkList.aspx");
            }
        }*/

        protected void btnSwitch_Click(object sender, EventArgs e)
        {
            DateTime CurrentDateNormalized = Bll.Frequency.GetNormalizedDate(FrequencyID, CurrentDate);
            if (hfSwitch.Value.Equals("0"))
            {

                hfSwitch.Value = "1";
                DateTime ZeroDate = new DateTime(2000, 1, 1);
                if (MetricID != null)
                {
                    Metric.Extend metric = Metric.Get((Guid)MetricID);
                    if (metric.CollectionStartDate != null)
                        ZeroDate = (DateTime)metric.CollectionStartDate;
                    FrequencyFirstDate = Bll.Frequency.GetNormalizedDate(FrequencyID, ZeroDate);
                }
                else if (OrgLocationID != null)
                {
                    LastFrequencyMetric = GetData();
                    DateTime MetricMinDate = DateTime.MaxValue;
                    foreach (MetricOrgValue mov in LastFrequencyMetric.Metrics)
                        if (mov.CollectionStartDate != null)
                            if (MetricMinDate > mov.CollectionStartDate)
                                MetricMinDate = (DateTime)mov.CollectionStartDate;
                    if (MetricMinDate == DateTime.MaxValue)
                        MetricMinDate = ZeroDate;
                    FrequencyFirstDate = Bll.Frequency.GetNormalizedDate(FrequencyID, MetricMinDate);
                }
                FrequencyFirstDate = MetricTrac.Bll.Frequency.AddPeriod(FrequencyFirstDate, FrequencyID, 12);
                if (FrequencyFirstDate > CurrentDateNormalized)
                    FrequencyFirstDate = CurrentDateNormalized;
            }
            else if (hfSwitch.Value.Equals("1"))
            {
                hfSwitch.Value = "0";
                FrequencyFirstDate = CurrentDateNormalized;
            }
        }

        protected string GetSwitchTitle()
        {
            string SwitchTitle = String.Empty;
            if (hfSwitch.Value.Equals("0"))
                SwitchTitle = "Switch to oldest period";
            else if (hfSwitch.Value.Equals("1"))
                SwitchTitle = "Switch to most current period";
            return SwitchTitle;
        }

        protected void lbSave1_Click(object sender, EventArgs e)
        {
            SaveBulkValues();
            if (DataMode == Mode.Input)
                Response.Redirect("MetricInputList.aspx");
            else
            {
                if (Micajah.Common.Security.UserContext.Breadcrumbs.Exists(r => r.Name.Contains("Approve Data"))) // TODO: change to redirect to parent
                    Response.Redirect("ApproveDataList.aspx");
                else
                    Response.Redirect("ApproveWorkList.aspx");
            }
        }

        protected void lbSave2_Click(object sender, EventArgs e)
        {
            SaveBulkValues();
        }

        private void SaveBulkValues()
        {
            int i = 0;
            LastFrequencyMetric = GetData();
            List<Bll.MetricValue.Extend> NewValues = new List<MetricValue.Extend>();
            List<Bll.MetricValue.Extend> OldValues = new List<MetricValue.Extend>();
            List<Bll.MetricOrgLocationUoM> MetricOrgLocationUoMList = new List<Bll.MetricOrgLocationUoM>();
            foreach (RepeaterItem ri in rMetric.Items)
            {
                MetricOrgValue mov = LastFrequencyMetric.Metrics[i];
                DateTime iTime = DateTime.Now;
                DropDownList ddlInputUnitOfMeasure = (DropDownList)ri.FindControl("ddlInputUnitOfMeasure");
                HiddenField hfUoM = (HiddenField)ri.FindControl("hfUoM");
                HiddenField hfMetric = (HiddenField)ri.FindControl("hfMetric");
                HiddenField hfOrgLocation = (HiddenField)ri.FindControl("hfOrgLocation");
                Repeater r = (Repeater)ri.FindControl("rMericValue");

                Guid? ActualUoMID = null;
                if (ddlInputUnitOfMeasure.Visible)
                {
                    if (!String.IsNullOrEmpty(ddlInputUnitOfMeasure.SelectedValue))
                        ActualUoMID = new Guid(ddlInputUnitOfMeasure.SelectedValue);
                }
                else
                {
                    if (!String.IsNullOrEmpty(hfUoM.Value))
                        ActualUoMID = new Guid(hfUoM.Value);
                }
                Guid oMetricID = new Guid(hfMetric.Value);
                Guid oOrgLocationID = new Guid(hfOrgLocation.Value);

                Bll.MetricOrgLocationUoM muom = new Bll.MetricOrgLocationUoM();
                muom.MetricID = oMetricID;
                muom.OrgLocationID = oOrgLocationID;
                muom.InputUnitOfMeasureID = ActualUoMID;
                MetricOrgLocationUoMList.Add(muom);

                int j = 0;
                foreach (RepeaterItem rri in r.Items)
                {
                    DateTime jTime = DateTime.Now;
                    HiddenField hfDate = (HiddenField)rri.FindControl("hfDate");
                    DateTime oDate = DateTime.Parse(hfDate.Value);
                    Telerik.Web.UI.RadNumericTextBox rntValue = (Telerik.Web.UI.RadNumericTextBox)rri.FindControl("rntValue");
                    System.Web.UI.WebControls.TextBox tbValue = (System.Web.UI.WebControls.TextBox)rri.FindControl("tbValue");
                    System.Web.UI.WebControls.CheckBox chbValue = (System.Web.UI.WebControls.CheckBox)rri.FindControl("chbValue");
                    Telerik.Web.UI.RadDatePicker rdpDateValue = (Telerik.Web.UI.RadDatePicker)rri.FindControl("rdpDateValue");
                    System.Web.UI.WebControls.DropDownList ddlApprovalStatus = (System.Web.UI.WebControls.DropDownList)rri.FindControl("ddlApprovalStatus");

                    Bll.MetricValue.Extend OldMetricValue = mov.MetricValues[j];
                    string Value = String.Empty;
                    if (rntValue.Visible)
                        Value = rntValue.Value.ToString();
                    else
                        if (tbValue.Visible)
                            Value = tbValue.Text;
                        else
                            if (chbValue.Visible)
                                Value = chbValue.Checked ? bool.TrueString : bool.FalseString;
                            else
                                if (rdpDateValue.Visible)
                                    Value = rdpDateValue.SelectedDate.ToString();

                    bool? Approved = OldMetricValue.Approved;
                    if (DataMode == Mode.Approve)
                        switch (ddlApprovalStatus.SelectedValue)
                        {
                            case "":
                                Approved = null;
                                break;
                            case "True":
                                Approved = true;
                                break;
                            case "False":
                            default:
                                Approved = false;
                                break;
                        }

                    if (
                        (
                            (
                                !String.IsNullOrEmpty(Value)
                                ||
                                (OldMetricValue.MetricValueID != Guid.Empty && String.IsNullOrEmpty(Value))
                            )
                            && Value != OldMetricValue.Value
                        )
                        ||
                        (DataMode == Mode.Approve && Approved != OldMetricValue.Approved))
                    {
                        MetricValue.Extend nv = new MetricValue.Extend();
                        nv.MetricID = oMetricID;
                        nv.Date = oDate;
                        nv.OrgLocationID = oOrgLocationID;
                        nv.InputUnitOfMeasureID = ActualUoMID;
                        nv.Value = Value;
                        nv.Approved = Approved;
                        // additional fields
                        nv.Period = Frequency.GetPeriodName(oDate, FrequencyID);
                        nv.MetricName = mov.Name;
                        nv.OrgLocationFullName = mov.OrgLocationFullName;
                        nv.Notes = OldMetricValue.Notes;
                        nv.ValueInputUnitOfMeasureName = mov.OrgLocationUnitOfMeasureName;
                        NewValues.Add(nv);

                        OldValues.Add(OldMetricValue);
                    }
                    LogTime += "    value " + j.ToString() + " time  " + ((TimeSpan)(DateTime.Now - jTime)).TotalMilliseconds;
                    j++;
                }
                LogTime += "Row " + i.ToString() + " time  " + ((TimeSpan)(DateTime.Now - iTime)).TotalMilliseconds + "<br />";
                i++;
            }
            Bll.MetricValue.SaveBulkValues(MetricOrgLocationUoMList, NewValues, OldValues, Micajah.Common.Security.UserContext.Current, DataMode == Mode.Input);
        } 
    }
}



            
            