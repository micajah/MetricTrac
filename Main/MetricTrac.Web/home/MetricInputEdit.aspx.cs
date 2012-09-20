using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;
using Micajah.FileService.WebControls;
using Telerik.Web.UI;
using System.Text;
using DataMode = MetricTrac.Control.MetricInputList.Mode; 

namespace MetricTrac
{
    public partial class MetricInputEdit : System.Web.UI.Page
    {   

        private Guid mMetricID;
        protected Guid MetricID
        {
            get
            {
                if (mMetricID != Guid.Empty) return mMetricID;
                string strMetricID = Request.QueryString["MetricID"];
                if (string.IsNullOrEmpty(strMetricID)) return mMetricID;
                try
                {
                    mMetricID = new Guid(strMetricID);
                }
                catch { }
                return mMetricID;
            }
        }

        private Guid? mOrgLocationID;
        protected Guid OrgLocationID
        {
            get
            {
                if (mOrgLocationID != null) return (Guid)mOrgLocationID;
                mOrgLocationID = Guid.Empty;
                string s = Request.QueryString["OrgLocationID"];
                if (!string.IsNullOrEmpty(s))
                {
                    try { mOrgLocationID = new Guid(s); }
                    catch { }
                }
                return (Guid)mOrgLocationID;
            }
        }


        private DateTime mOperationDate;
        protected DateTime OperationDate
        {
            get
            {
                string strDate = Request.QueryString["Date"];
                if (string.IsNullOrEmpty(strDate)) return DateTime.MinValue;
                else
                    try
                    {
                        mOperationDate = DateTime.ParseExact(strDate, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch { }
                return mOperationDate;
            }
        }

        protected DataMode Mode
        {
            get
            {
                switch (Request.QueryString["Mode"])
                {
                    case "View": return DataMode.View;
                    case "Approve": return DataMode.Approve;
                    case "Input":
                    default: return DataMode.Input;
                }
            }
        }

        protected bool IsBulkEdit
        { get { return (Request.QueryString["BulkEdit"] != null); } }
        
        // only for edit mode
        protected SimpleUpload FilesUpload
        {
            get { return (SimpleUpload)mfMetricValue.FindControl("suFiles"); }
        }

        protected RadNumericTextBox rntValue
        {
            get { return (RadNumericTextBox)mfMetricValue.FindControl("rntValue"); }
        }

        protected System.Web.UI.WebControls.TextBox tbValue
        {
            get { return (System.Web.UI.WebControls.TextBox)mfMetricValue.FindControl("tbValue"); }
        }        

        protected System.Web.UI.WebControls.CheckBox chbValue
        {
            get { return (System.Web.UI.WebControls.CheckBox)mfMetricValue.FindControl("chbValue"); }
        }

        protected RadDatePicker rdpDateValue
        {
            get { return (RadDatePicker)mfMetricValue.FindControl("rdpDateValue"); }
        }

        protected System.Web.UI.WebControls.Label lblOldValue
        {
            get { return (System.Web.UI.WebControls.Label)mfMetricValue.FindControl("lblOldValue"); }
        }

        protected System.Web.UI.WebControls.Label lblUoM
        {
            get { return (System.Web.UI.WebControls.Label)mfMetricValue.FindControl("lblUoM"); }
        }

        protected System.Web.UI.WebControls.Label lblRange
        {
            get { return (System.Web.UI.WebControls.Label)mfMetricValue.FindControl("lblRange"); }
        }

        protected DropDownList ddlInputUnitOfMeasure
        {
            get { return (DropDownList)mfMetricValue.FindControl("ddlInputUnitOfMeasure"); }
        }

        protected DropDownList ddlApprovalStatus
        {
            get { return (DropDownList)mfMetricValue.FindControl("ddlApprovalStatus"); }
        }

        protected System.Web.UI.WebControls.TextBox tbComments
        {
            get { return (System.Web.UI.WebControls.TextBox)mfMetricValue.FindControl("tbComments"); }
        }

        protected System.Web.UI.WebControls.TextBox tbAlias
        {
            get { return (System.Web.UI.WebControls.TextBox)mfMetricValue.FindControl("tbAlias"); }
        }

        protected System.Web.UI.WebControls.TextBox tbCode
        {
            get { return (System.Web.UI.WebControls.TextBox)mfMetricValue.FindControl("tbCode"); }
        }
        // -------------


        // only for readonly
        protected FileList flFiles
        {
            get 
            {                
                return (FileList)mfMetricValue.FindControl("flFiles");
            }
        }

        protected System.Web.UI.WebControls.Label lblValueView
        {
            get { return (System.Web.UI.WebControls.Label)mfMetricValue.FindControl("lblValueView"); }
        }

        protected System.Web.UI.WebControls.Label lblUoMView
        {
            get { return (System.Web.UI.WebControls.Label)mfMetricValue.FindControl("lblUoMView"); }
        }

        protected System.Web.UI.WebControls.Label lblConvertedValue
        {
            get { return (System.Web.UI.WebControls.Label)mfMetricValue.FindControl("lblConvertedValue"); }
        }

        protected System.Web.UI.WebControls.Label lblOutputUoMView
        {
            get { return (System.Web.UI.WebControls.Label)mfMetricValue.FindControl("lblOutputUoMView"); }
        }

        protected System.Web.UI.WebControls.Label lblAppStatus
        {
            get { return (System.Web.UI.WebControls.Label)mfMetricValue.FindControl("lblAppStatus"); }
        }

        void InitButtonSection()
        {            
            TableCell c = mfMetricValue.Rows[mfMetricValue.Rows.Count - 1].Cells[0];
            Button UpdateButton = (Button)c.Controls[0];            
            FilesUpload.UploadControlsUniqueId = new string[] { UpdateButton.UniqueID };
        }

        /* PIF 
        void InitButtonSection()
        {
            System.Web.UI.HtmlControls.HtmlTableCell c = (System.Web.UI.HtmlControls.HtmlTableCell)mfPerformanceIndicatorForm.Rows[mfPerformanceIndicatorForm.Rows.Count - 1].Cells[0].Controls[0].Controls[0].Controls[0];
            Button p = (Button)c.Controls[0];
            Button OrgLocButton = new Button();
            OrgLocButton.Text = "Create Performance Indicator Form and Assign to Org Location";
            OrgLocButton.OnClientClick = "__doPostBack('" + this.UniqueID + "', 'assign'); return false;";
            c.Controls.AddAt(1, OrgLocButton);
            c.Controls.AddAt(1, new LiteralControl("&nbsp; or &nbsp;"));
            //c.Controls.AddAt(1, new LiteralControl("<INPUT TYPE = submit name = " + this.UniqueID + " Value = 'Click Me' />"));
        }*/

        protected void flFiles_Init(object sender, EventArgs e)
        {
            if (MVS != null)
                flFiles.LocalObjectId = MVS.MetricValueID.ToString();
        }

        protected void suFiles_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                FilesUpload.LocalObjectId = null;
        }

        protected void Page_Load(object sender, EventArgs e)
        {   
            if ((MetricID == Guid.Empty) || (OperationDate == DateTime.MinValue)) RedirectToGrid(true);
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleApplicationLogo = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleBreadcrumbs = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleFooter = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleFooterLinks = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeader = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeaderLinks = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleLeftArea = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleMainMenu = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeaderLogo = false;            
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleSearchControl = false;                        
            pnlBtnClose.Visible = (Mode == DataMode.View);            
            ValueHistoryLog.MetricValueID = MVS.MetricValueID;
            mfMetricValue.Fields[11].Visible = (Mode != DataMode.Input);
            mfMetricValue.Fields[9].Visible = (Mode == DataMode.View);

            if (Mode == DataMode.View)
                mfMetricValue.ChangeMode(DetailsViewMode.ReadOnly);
            else
            {
                mfMetricValue.CloseButtonCaption = "Save Metric Value";
                InitButtonSection();
            }

            if (!IsPostBack && !MetricTrac.Utils.MetricUtils.IsPopupSupported(Request))
            {
                btnClose.OnClientClick = String.Empty;
                phHideBtn.Visible = false;
            }
        }

        private string ErrorMessage
        {
            set { ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage = value; }
            get { return ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage; }
        }

        protected void mfMetricValue_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel)
            {
                FilesUpload.RejectChanges();
                RedirectToGrid(true);
            }
        }

        private void RedirectToGrid(bool IsCancel)
        {
            if (!MetricTrac.Utils.MetricUtils.IsPopupSupported(Request) && !String.IsNullOrEmpty(Request.QueryString["BackPage"]))
            {                
                switch (Request.QueryString["BackPage"])
                {
                    case "1":
                        Response.Redirect("MetricInputList.aspx");
                        //Response.Redirect("MetricBulkInput.aspx");
                        break;
                    case "2":
                        Response.Redirect("ApproveDataList.aspx");
                        //Response.Redirect("MetricBulkApprove.aspx");
                        break;
                    case "3":
                        Response.Redirect("MetricInputList.aspx");
                        break;
                    case "4":
                        Response.Redirect("MetricDataList.aspx");
                        break;
                    case "5":
                        Response.Redirect("ApproveDataList.aspx");
                        break;
                    case "6":
                        Response.Redirect("MetricMissedValuesList.aspx");
                        break;
                    case "7":
                        Response.Redirect("MetricDataList.aspx");
                        //Response.Redirect("RelatedInputValues.aspx");
                        break;
                    case "8":
                        Response.Redirect("ApproveDataList.aspx");
                        //Response.Redirect("RelatedInputValuesA.aspx");
                        break;
                    case "9":
                        Response.Redirect("ApproveWorkList.aspx");
                        break;                    
                    default:
                        Response.Redirect("MetricDataList.aspx");
                        break;
                }                
            }
            else
                if (String.IsNullOrEmpty(this.ErrorMessage))
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "_CorrectReturn_", IsCancel ? "CloseOnReload(true, null);" : (IsBulkEdit ? "CloseOnReload(true, '" + ValueArgument.ToString() + "');" : "CloseOnReload(false, null);"), true);                            
        }

        private Bll.MetricValue.Extend mMVS = null;
        protected Bll.MetricValue.Extend MVS
        {
            get
            {
                if (mMVS == null)
                    mMVS = Bll.MetricValue.Get(MetricID, OperationDate, OrgLocationID);
                return mMVS;
            }
        }

        protected void ldsMetricValue_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (IsPostBack && String.IsNullOrEmpty(this.ErrorMessage))
            {
                e.Cancel = true;
                return;
            }            
            e.Result = MVS;
            phDescription.Visible = !String.IsNullOrEmpty(MVS.Description);
            lblDescription.Text = String.IsNullOrEmpty(MVS.Description) ? "No description" : MVS.Description;
            phDefinition.Visible = !String.IsNullOrEmpty(MVS.Definition);
            lblDefinition.Text = String.IsNullOrEmpty(MVS.Definition) ? "No definition" : MVS.Definition;
            phDocumentation.Visible = !String.IsNullOrEmpty(MVS.Documentation);
            lblDocumentation.Text = String.IsNullOrEmpty(MVS.Documentation) ? "No documentation" : MVS.Documentation;
            phReferences.Visible = !String.IsNullOrEmpty(MVS.References);
            lblReferences.Text = String.IsNullOrEmpty(MVS.References) ? "No references" : MVS.References;

            hr1.Visible = phDescription.Visible && (phDefinition.Visible || phDocumentation.Visible || lblReferences.Visible);
            hr2.Visible = phDefinition.Visible && (phDocumentation.Visible || lblReferences.Visible);
            hr3.Visible = phDocumentation.Visible && lblReferences.Visible;
        }

        protected void ldsInputUnitOfMeasure_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ddlInputUnitOfMeasure.Items.Clear();
            if (MVS.IsCalculated == true)
            {
                e.Cancel = true;
                ddlInputUnitOfMeasure.Items.Add(new ListItem(MVS.ValueUnitOfMeasureName, MVS.UnitOfMeasureID.ToString()));
                ddlInputUnitOfMeasure.Enabled = false;
            }
            else
            {
                if (MVS.RelatedOrgLocationUoMRecordID == null)
                { // no previous records exists
                    if (MVS.MetricValueID == Guid.Empty)
                    { // +insert value
                        if (MVS.MetricInputUnitOfMeasureID == null)
                        { // ++metric have no uom
                            e.Cancel = true;
                            ddlInputUnitOfMeasure.Items.Add(new ListItem(String.Empty, String.Empty));
                        }
                        else
                        { // ++ metric have some default uom
                            List<Micajah.Common.Bll.MeasureUnit> l = Bll.Mc_UnitsOfMeasure.GetConvertedUoMs(MVS.MetricInputUnitOfMeasureID);
                            e.Result = l;
                            ddlInputUnitOfMeasure.SelectedValue = MVS.MetricInputUnitOfMeasureID.ToString();
                        }
                    }
                    else
                    { // +update value
                        if (MVS.InputUnitOfMeasureID == null)
                        {
                            if (MVS.MetricInputUnitOfMeasureID == null)
                            { // ++metric have no uom
                                e.Cancel = true;
                                ddlInputUnitOfMeasure.Items.Add(new ListItem(String.Empty, String.Empty));
                            }
                            else
                            { // ++ metric have some default uom
                                List<Micajah.Common.Bll.MeasureUnit> l = Bll.Mc_UnitsOfMeasure.GetConvertedUoMs(MVS.MetricInputUnitOfMeasureID);
                                e.Result = l;
                                ddlInputUnitOfMeasure.SelectedValue = MVS.MetricInputUnitOfMeasureID.ToString();
                            }
                        }
                        else
                        {
                            if (MVS.MetricInputUnitOfMeasureID == null)
                            { // ++metric have no uom
                                e.Cancel = true;
                                ddlInputUnitOfMeasure.Items.Add(new ListItem(String.Empty, String.Empty));
                            }
                            else
                            { // ++ metric have some default uom
                                List<Micajah.Common.Bll.MeasureUnit> l = Bll.Mc_UnitsOfMeasure.GetConvertedUoMs(MVS.MetricInputUnitOfMeasureID);
                                e.Result = l;
                                if (l.Select(u => u.MeasureUnitId == MVS.InputUnitOfMeasureID).Count() > 0)
                                    ddlInputUnitOfMeasure.SelectedValue = MVS.InputUnitOfMeasureID.ToString();
                                else
                                    ddlInputUnitOfMeasure.SelectedValue = MVS.MetricInputUnitOfMeasureID.ToString();
                            }
                        }
                    }
                }
                else
                {// there are defined uom for org location
                    e.Cancel = true;
                    ddlInputUnitOfMeasure.Items.Add(new ListItem(MVS.OrgLocationUnitOfMeasureName, MVS.OrgLocationUnitOfMeasureID.ToString()));
                    ddlInputUnitOfMeasure.Enabled = false;
                }
            }
        }
        
        protected void mfMetricValue_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack && String.IsNullOrEmpty(this.ErrorMessage)) return;
            mfMetricValue.Fields[3].Visible = mfMetricValue.Fields[4].Visible = (MVS.AllowMetricCustomNames && (Mode != DataMode.Approve));

            /*lblConvertedValue.Text = MVS.ConvertedValue;
            if (MVS.MetricDataTypeID == 1)
                lblOutputUoMView.Text = MVS.ValueUnitOfMeasureName + (String.IsNullOrEmpty(MVS.ValueUnitOfMeasureName) ? String.Empty : "(s)");*/
            
            string PreValue = MVS.Value;
            bool IsConverted = true;
            Guid? CurInputUoM = MVS.InputUnitOfMeasureID;
            Guid? CalcOutputUoM = MVS.MetricUnitOfMeasureID;
            if (CalcOutputUoM != CurInputUoM)
            {
                if (CalcOutputUoM != null && CurInputUoM != null)
                {
                    List<Micajah.Common.Bll.MeasureUnit> l = Bll.Mc_UnitsOfMeasure.GetConvertedUoMs(CalcOutputUoM);
                    Micajah.Common.Bll.MeasureUnit mu = Micajah.Common.Bll.MeasureUnit.Create((Guid)CurInputUoM, Bll.LinqMicajahDataContext.OrganizationId);
                    if (!l.Contains(mu))
                        IsConverted = false;
                }
                else
                    IsConverted = false;
            }
            if (IsConverted && PreValue != "-" && !String.IsNullOrEmpty(PreValue) && CalcOutputUoM != CurInputUoM && CalcOutputUoM != null && CurInputUoM != null)
                PreValue = Bll.Mc_UnitsOfMeasure.ConvertValue(PreValue, (Guid)CurInputUoM, (Guid)CalcOutputUoM);

            lblConvertedValue.Text = PreValue;            
            if (MVS.MetricDataTypeID == 1)                
                lblOutputUoMView.Text = MVS.MetricUnitOfMeasureName + (String.IsNullOrEmpty(MVS.MetricUnitOfMeasureName) ? String.Empty : "(s)");//MVS.ValueUnitOfMeasureName + (String.IsNullOrEmpty(MVS.ValueUnitOfMeasureName) ? String.Empty : "(s)");

            if (Mode == DataMode.View)
            {                
                flFiles.LocalObjectId = MVS.MetricValueID.ToString();
                lblValueView.Text = MVS.Value;
                //lblConvertedValue.Text = MVS.ConvertedValue;
                if (MVS.MetricDataTypeID == 1)
                {
                    lblUoMView.Text = MVS.ValueInputUnitOfMeasureName + (String.IsNullOrEmpty(MVS.ValueInputUnitOfMeasureName) ? String.Empty : "(s)");
                    //lblOutputUoMView.Text = MVS.ValueUnitOfMeasureName + (String.IsNullOrEmpty(MVS.ValueUnitOfMeasureName) ? String.Empty : "(s)");
                }
            }
            else
            {
                ddlApprovalStatus.Enabled = Mode == DataMode.Approve;
                bool IsDataTypeChanged = false;
                bool IsNewRecord = false;
                if (MVS.MetricValueID == Guid.Empty)
                {
                    lblOldValue.Visible = false;
                    IsNewRecord = true;
                }
                else
                {
                    if (MVS.MetricDataTypeID != MVS.ActualMetricDataTypeID)
                    {// Data type was changed for this metric
                        IsDataTypeChanged = true;
                        string sOldValue = "Old value for this period is " + MVS.Value;
                        if (MVS.MetricDataTypeID == 1)
                            sOldValue += " " + MVS.ValueInputUnitOfMeasureName;
                        lblOldValue.Text = sOldValue;
                        lblOldValue.Visible = true;
                    }
                }
                FilesUpload.LocalObjectId = MVS.MetricValueID == Guid.Empty ? null : MVS.MetricValueID.ToString();
                switch (MVS.ActualMetricDataTypeID)
                {
                    case 1://Numeric 
                        rntValue.Visible = lblUoM.Visible = ddlInputUnitOfMeasure.Visible = true;
                        if (MVS.IsCalculated==true)
                        {
                            mfMetricValue.Fields[8].HeaderText = "Calculated&nbsp;value";
                            rntValue.ReadOnly = true;
                            rntValue.BorderStyle = BorderStyle.None;
                            //
                        }
                        tbValue.Visible = chbValue.Visible = rdpDateValue.Visible = false;
                        rntValue.Text = String.Empty;                        

                        if (!IsDataTypeChanged)
                            if ((MVS.MetricUnitOfMeasureID != MVS.UnitOfMeasureID) && !IsNewRecord)
                            {
                                lblOldValue.Text = "Old value for this period is " + MVS.Value + " " + MVS.ValueInputUnitOfMeasureName;
                                lblOldValue.Visible = true;
                            }
                            else
                            {
                                double _Value = double.NaN;
                                if (double.TryParse(MVS.Value, out _Value))
                                    rntValue.Value = _Value;
                            }

                        // Range label
                        if ((MVS.NOMinValue != null) || (MVS.NOMaxValue != null) || (MVS.NODecPlaces != null))
                        {
                            //lblRange.Visible = true;                        
                            lblRange.Text = String.Empty;
                            int defPlaces = 2;
                            if (MVS.NODecPlaces != null)
                            {
                                rntValue.NumberFormat.DecimalDigits = (int)MVS.NODecPlaces;
                                defPlaces = (int)MVS.NODecPlaces;
                                lblRange.Text += "DecPlaces = " + MVS.NODecPlaces.ToString() + ";";
                            }
                            if (MVS.NOMinValue != null)
                            {
                                rntValue.MinValue = decimal.ToDouble((decimal)MVS.NOMinValue);
                                lblRange.Text += "MinValue = " + ((decimal)MVS.NOMinValue).ToString("F" + defPlaces.ToString()) + "; "; // how many digits it may show ???
                            }
                            if (MVS.NOMaxValue != null)
                            {
                                rntValue.MaxValue = decimal.ToDouble((decimal)MVS.NOMaxValue);
                                lblRange.Text += "MaxValue = " + ((decimal)MVS.NOMaxValue).ToString("F" + defPlaces.ToString()) + "; ";
                            }

                        }
                        else
                            lblRange.Visible = false;
                        break;
                    case 2://Text
                        tbValue.Visible = true;
                        rntValue.Visible = lblUoM.Visible = ddlInputUnitOfMeasure.Visible = lblRange.Visible = chbValue.Visible = rdpDateValue.Visible = false;
                        tbValue.Text = String.Empty;
                        if (!IsDataTypeChanged)
                            tbValue.Text = MVS.Value;
                        break;
                    case 3://Bool - checkbox
                        chbValue.Visible = true;
                        rntValue.Visible = lblUoM.Visible = ddlInputUnitOfMeasure.Visible = lblRange.Visible = tbValue.Visible = rdpDateValue.Visible = false;
                        chbValue.Checked = false;
                        if (!IsDataTypeChanged)
                            chbValue.Checked = MVS.Value == bool.TrueString;
                        break;
                    case 4://Date
                        rdpDateValue.Visible = true;
                        rntValue.Visible = lblUoM.Visible = ddlInputUnitOfMeasure.Visible = lblRange.Visible = tbValue.Visible = chbValue.Visible = false;
                        rdpDateValue.SelectedDate = DateTime.Now;
                        DateTime _dt = DateTime.Now;
                        if (!IsDataTypeChanged)
                            if (DateTime.TryParse(MVS.Value, out _dt))
                                rdpDateValue.SelectedDate = _dt;
                        break;
                    default:
                        rntValue.Visible = lblUoM.Visible = ddlInputUnitOfMeasure.Visible = lblRange.Visible = tbValue.Visible = chbValue.Visible = rdpDateValue.Visible = false;
                        break;
                }
            }
        }

        private string ValueArgument = String.Empty;
        protected void ldsMetricValue_Updating(object sender, LinqDataSourceUpdateEventArgs e)
        {
            // save previous value
            MetricTrac.Bll.MetricValue.Extend OldMetricValue = MVS;
            // get new data
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
            ValueArgument = Value;
            Guid? ActualUoMID = null;
            if (!String.IsNullOrEmpty(ddlInputUnitOfMeasure.SelectedValue))
                ActualUoMID = new Guid(ddlInputUnitOfMeasure.SelectedValue);
            string CustomMetricAlias = null; // if pass null to Isert/Update - nothing changed. It's possible if custom names disabled or bulk edit
            string CustomMetricCode = null;
            if (MVS.AllowMetricCustomNames)
            {
                CustomMetricAlias = tbAlias.Text;
                CustomMetricCode = tbCode.Text;
            }
            
            bool? Approved = false;
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
            ValueArgument = Value + "|" + Approved.ToString();
            string comments = tbComments.Text;
            string Notes = ((Micajah.Common.WebControls.TextBox)mfMetricValue.FindFieldControl("Notes")).Text;            
            Guid CurentUserId = Micajah.Common.Security.UserContext.Current.UserId;            
            //------------------------------
                                    
            Guid _ValueID = Bll.MetricValue.InsertOrUpdate(
                MetricID,
                OperationDate,
                OrgLocationID,
                !FilesUpload.IsEmpty,
                Mode == DataMode.Approve,
                ActualUoMID,
                OldMetricValue.Value,
                Value,
                OldMetricValue.Approved,
                Approved,
                CurentUserId,
                Notes,
                CustomMetricAlias,
                CustomMetricCode);

            if (_ValueID != Guid.Empty)
            {
                FilesUpload.LocalObjectId = _ValueID.ToString();
                if (!FilesUpload.AcceptChanges())
                    if (FilesUpload.ErrorOccurred)
                    {
                        string _errorMessage = String.Empty;
                        foreach (string s in FilesUpload.ErrorMessages)
                            _errorMessage += s + "\n";
                        this.ErrorMessage = _errorMessage;
                    }
            }
            else // change this error handler after adding central error tracker
                this.ErrorMessage = "Unable to save changes. Please, try again later.";
            Bll.MetricValue.Extend NewMetricValue = Bll.MetricValue.Get(MetricID, OperationDate, OrgLocationID);
            Bll.Mc_User.Extend mue = Bll.Mc_User.GetValueInputUser(OldMetricValue.MetricValueID);
            // build mail to data collector if status or comment were changed
            if ((Mode == DataMode.Approve) && ((!String.IsNullOrEmpty(comments)) || (OldMetricValue.Approved != NewMetricValue.Approved)))
            {
                string MailCaption = OldMetricValue.Approved != NewMetricValue.Approved ? "MetricTrac - Value Status is changed" : "SustainApp - Value has new comment from Data Approver";
                if (OldMetricValue.Approved != NewMetricValue.Approved)                
                    Bll.MetricValueChangeLog.LogChange(NewMetricValue.MetricValueID,
                    Bll.MetricValueChangeTypeEnum.StatusChanged,
                    OldMetricValue.ApprovalStatus,
                    NewMetricValue.ApprovalStatus,
                    Utils.Mail.BuildLogMessageBody(OldMetricValue, NewMetricValue, comments, Micajah.Common.Security.UserContext.Current, mue, Bll.MetricValueChangeTypeEnum.StatusChanged));
                else
                    Bll.MetricValueChangeLog.LogChange(NewMetricValue.MetricValueID,
                    Bll.MetricValueChangeTypeEnum.CommentToDataCollector,
                    null,
                    comments,
                    Utils.Mail.BuildLogMessageBody(OldMetricValue, NewMetricValue, comments, Micajah.Common.Security.UserContext.Current, mue, Bll.MetricValueChangeTypeEnum.CommentToDataCollector));
                if (NewMetricValue.Approved == null && mue != null)
                    Utils.Mail.Send(mue.Email, mue.FullName, MailCaption, Utils.Mail.BuildEmailBody(OldMetricValue, NewMetricValue, comments, Micajah.Common.Security.UserContext.Current));
            }


            // record in change log
              // first time value entered
            if (OldMetricValue.MetricValueID == Guid.Empty)
                Bll.MetricValueChangeLog.LogChange(NewMetricValue.MetricValueID,
                    MetricTrac.Bll.MetricValueChangeTypeEnum.ValueEntered,
                    String.Empty,
                    NewMetricValue.Value,
                    Utils.Mail.BuildLogMessageBody(OldMetricValue, NewMetricValue, Notes, Micajah.Common.Security.UserContext.Current, mue, MetricTrac.Bll.MetricValueChangeTypeEnum.ValueEntered));
            else
            {
                // value changed
                if (OldMetricValue.Value != NewMetricValue.Value)
                    Bll.MetricValueChangeLog.LogChange(MVS.MetricValueID,
                        MetricTrac.Bll.MetricValueChangeTypeEnum.ValueChanged,
                        OldMetricValue.Value,
                        NewMetricValue.Value,
                        Utils.Mail.BuildLogMessageBody(OldMetricValue, NewMetricValue, Notes, Micajah.Common.Security.UserContext.Current, mue, MetricTrac.Bll.MetricValueChangeTypeEnum.ValueChanged));
                // notes changed
                if (OldMetricValue.Notes != NewMetricValue.Notes)
                    Bll.MetricValueChangeLog.LogChange(MVS.MetricValueID,
                        MetricTrac.Bll.MetricValueChangeTypeEnum.NoteChanged,
                        OldMetricValue.Notes,
                        NewMetricValue.Notes,
                        Utils.Mail.BuildLogMessageBody(OldMetricValue, NewMetricValue, Notes, Micajah.Common.Security.UserContext.Current, mue, MetricTrac.Bll.MetricValueChangeTypeEnum.NoteChanged));
            }
            
            e.Cancel = true;
        }

        protected void mfMetricValue_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {            
            RedirectToGrid(false);
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            RedirectToGrid(true);
        }
    }
}