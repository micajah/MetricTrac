using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace MetricTrac
{
    public partial class MetricEdit : System.Web.UI.Page, IPostBackEventHandler
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
                
        protected string PerformanceIndicatorID { get { return Request.QueryString["PI"]; } }

        private Guid mCopyMetricID = Guid.Empty;
        protected Guid CopyMetricID
        {
            get
            {
                if (mCopyMetricID != Guid.Empty) return mCopyMetricID;
                string strCopyMetricID = Request.QueryString["CopyMetricID"];
                if (string.IsNullOrEmpty(strCopyMetricID)) return mCopyMetricID;
                try
                {
                    mCopyMetricID = new Guid(strCopyMetricID);
                }
                catch { }
                return mCopyMetricID;
            }
        }

        private Micajah.Common.WebControls.TextBox heExpression
        {
            get
            {
                return (Micajah.Common.WebControls.TextBox)mfMetric.FindControl("heExpression");
            }
        }

        private HiddenField hfFormulaID
        {
            get
            {
                return (HiddenField)mfMetric.FindControl("hfFormulaID");
            }
        }

        private HiddenField hfBeginDate
        {
            get
            {
                return (HiddenField)mfMetric.FindControl("hfBeginDate");
            }
        }

        private HiddenField hfEndDate
        {
            get
            {
                return (HiddenField)mfMetric.FindControl("hfEndDate");
            }
        }

        private Label lblBeginDate
        {
            get
            {
                return (Label)mfMetric.FindControl("lblBeginDate");
            }
        }

        private Label lblEndDate
        {
            get
            {
                return (Label)mfMetric.FindControl("lblEndDate");
            }
        }

        public string FormulaInputClientID
        {
            get
            {                
                return heExpression != null ? heExpression.ClientID + "_txt" : String.Empty;
            }
        }

        public string BeginDateClientID
        {
            get
            {
                return hfBeginDate != null ? hfBeginDate.ClientID : String.Empty;
            }
        }

        public string EndDateClientID
        {
            get
            {
                return hfEndDate != null ? hfEndDate.ClientID : String.Empty;
            }
        }

        public string lblBeginDateClientID
        {
            get
            {
                return lblBeginDate != null ? lblBeginDate.ClientID : String.Empty;
            }
        }

        public string lblEndDateClientID
        {
            get
            {
                return lblEndDate != null ? lblEndDate.ClientID : String.Empty;
            }
        }


        protected MetricTrac.Control.MetricCategorySelect ddlMetricCategorySelect
        {
            get
            {
                return (MetricTrac.Control.MetricCategorySelect)mfMetric.FindControl("mcs");
            }
        }

        private DropDownList ddlMetricType
        {
            get
            {
                return (DropDownList)mfMetric.FindControl("ddlMetricType");
            }
        }

        private DropDownList ddlDataType
        {
            get
            {
                return (DropDownList)mfMetric.FindControl("ddlDataType");
            }
        }

        private DropDownList ddlUnitOfMeasure
        {
            get
            {
                return (DropDownList)mfMetric.FindControl("ddlUnitOfMeasure");
            }
        }

        private DropDownList ddlInputUnitOfMeasure
        {
            get
            {
                return (DropDownList)mfMetric.FindControl("ddlInputUnitOfMeasure");
            }
        }

        protected DropDownList ddlInputPeriod
        {
            get
            {
                return (DropDownList)mfMetric.FindControl("ddlInputPeriod");
            }
        }

        protected RadComboBox rcbUpGood
        {
            get
            {
                return (RadComboBox)mfMetric.FindControl("rcbUpGood");
            }
        }

        protected bool IsUoMRequest
        {
            get
            {
                RadAjaxManager ram = RadAjaxManager.GetCurrent(Page);
                return ram.IsAjaxRequest;
            }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (MetricID != Guid.Empty)
            {
                Utils.MetricUtils.InitLinqDataSources(ldsMetric);
                ldsMetric.WhereParameters.Add("MetricID", System.Data.DbType.Guid, MetricID.ToString());
                mfMetric.ChangeMode(DetailsViewMode.Edit);
            }
            else
            {
                DateTime dtCurrent = DateTime.Now;
                ((Micajah.Common.WebControls.DatePicker)mfMetric.Rows[7].Cells[1].Controls[0]).SelectedDate = new DateTime(dtCurrent.Year, dtCurrent.Month, dtCurrent.Day);
            }
            if (!IsPostBack)
            {
                DeleteCommentCookie();
                if (MetricID != Guid.Empty)
                {
                    Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;
                    OrgTree.CustomRootNodeText = user.SelectedOrganization.Name;
                    OrgTree.EntityNodeId = MetricID;
                    OrgTree.LoadTree();
                }
                else
                    OrgTree.Visible = false;
            }
        }

        private string CommenttCookieName = "FormulaComment";

        private void DeleteCommentCookie()
        {
            HttpCookie _c = new HttpCookie(CommenttCookieName, String.Empty);
            string CookiePath = String.Empty;
            for (int i = 0; i < Request.Url.Segments.Count()-1; i++ )
                CookiePath += Request.Url.Segments[i];
            _c.Path = CookiePath;
            Response.Cookies.Add(_c);
        }        

        private string ErrorMessage
        {
            set { ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage = value; }
            get { return ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage; }
        }

        private void RedirectToGrid()
        {
            if (!String.IsNullOrEmpty(PerformanceIndicatorID))
                Response.Redirect("PerformanceIndicatorEdit.aspx?PerformanceIndicatorID=" + PerformanceIndicatorID);
            else
                Response.Redirect("MetricList.aspx");
        }

        // =============================================================================
         // LinqDS Selected and preRender        
        Guid? InputID = null;
        protected void ldsMetric_Selected(object sender, LinqDataSourceStatusEventArgs e)
        {
            List<MetricTrac.Bll.Metric> l = (List<MetricTrac.Bll.Metric>)e.Result;
            if (l != null && l.Count > 0)
            {
                RegisterStartScript(l[0].MetricTypeID == 1, l[0].MetricDataTypeID == 1);
                InputID = l[0].InputUnitOfMeasureID;
            }
        }

        private Bll.Metric.Extend InitMagicForm()
        {
            Bll.Metric.Extend CopyMetric = Bll.Metric.Get(CopyMetricID);   
            if (CopyMetric != null)
            {
                ((Micajah.Common.WebControls.HtmlEditor)mfMetric.Rows[17].Cells[1].Controls[0]).Content = CopyMetric.Notes;
                ((Micajah.Common.WebControls.HtmlEditor)mfMetric.Rows[18].Cells[1].Controls[0]).Content = CopyMetric.Definition;
                ((Micajah.Common.WebControls.HtmlEditor)mfMetric.Rows[19].Cells[1].Controls[0]).Content = CopyMetric.Documentation;
                ((Micajah.Common.WebControls.HtmlEditor)mfMetric.Rows[20].Cells[1].Controls[0]).Content = CopyMetric.MetricReferences;

                ((Micajah.Common.WebControls.CheckBox)mfMetric.Rows[16].Cells[1].Controls[0]).Checked = CopyMetric.AllowCustomNames;

                ddlMetricCategorySelect.MetricCategoryID = CopyMetric.MetricCategoryID;
                ddlMetricType.SelectedValue = CopyMetric.MetricTypeID.ToString();
                ddlDataType.SelectedValue = CopyMetric.MetricDataTypeID.ToString();
                ddlUnitOfMeasure.SelectedValue = CopyMetric.UnitOfMeasureID.ToString();                
                ddlInputUnitOfMeasure.SelectedValue = CopyMetric.InputUnitOfMeasureID == null ? String.Empty : CopyMetric.InputUnitOfMeasureID.ToString();
                ddlInputUnitOfMeasure.DataBind();                
                if (CopyMetric.MetricTypeID == 2)
                    heExpression.Text = MetricFormula.Formula;
                ddlInputPeriod.SelectedValue = CopyMetric.FrequencyID.ToString();
                rcbUpGood.SelectedValue = CopyMetric.GrowUpIsGood.ToString();

                if (CopyMetric.CollectionStartDate != null)
                    ((Micajah.Common.WebControls.DatePicker)mfMetric.Rows[7].Cells[1].Controls[0]).SelectedDate = (DateTime)CopyMetric.CollectionStartDate;
                else
                    ((Micajah.Common.WebControls.DatePicker)mfMetric.Rows[7].Cells[1].Controls[0]).Clear();

                if (CopyMetric.CollectionEndDate != null)
                    ((Micajah.Common.WebControls.DatePicker)mfMetric.Rows[8].Cells[1].Controls[0]).SelectedDate = (DateTime)CopyMetric.CollectionEndDate;
                else
                    ((Micajah.Common.WebControls.DatePicker)mfMetric.Rows[8].Cells[1].Controls[0]).Clear();
                ((Micajah.Common.WebControls.TextBox)mfMetric.Rows[0].Cells[1].Controls[0]).Text = CopyMetric.Name;
                ((Micajah.Common.WebControls.TextBox)mfMetric.Rows[1].Cells[1].Controls[0]).Text = CopyMetric.Alias;
                ((Micajah.Common.WebControls.TextBox)mfMetric.Rows[2].Cells[1].Controls[0]).Text = CopyMetric.Code;
                ((Micajah.Common.WebControls.TextBox)mfMetric.Rows[11].Cells[1].Controls[0]).Text = CopyMetric.NODecPlaces.ToString();
                ((Micajah.Common.WebControls.TextBox)mfMetric.Rows[12].Cells[1].Controls[0]).Text = CopyMetric.NOMinValue.ToString();
                ((Micajah.Common.WebControls.TextBox)mfMetric.Rows[13].Cells[1].Controls[0]).Text = CopyMetric.NOMaxValue.ToString();
                ((Micajah.Common.WebControls.TextBox)mfMetric.Rows[21].Cells[1].Controls[0]).Text = CopyMetric.FormulaCode;
            }
            return CopyMetric;
        }

        private void RegisterStartScript(bool ShowType, bool ShowNumeric)
        {
            Page.ClientScript.RegisterStartupScript(this.GetType(), "_startupscript",
                String.Format("HideShowNumericOptions({1}); HideShowType({0}); HideRow(22);PutSubmitInput();",
                    ShowType.ToString().ToLower(),
                    ShowNumeric.ToString().ToLower()),
            true);
        }

        private void InitAjaxManager()
        {
            RadAjaxManager ram = RadAjaxManager.GetCurrent(Page);
            ram.UpdatePanelsRenderMode = UpdatePanelRenderMode.Inline;
            ram.AjaxSettings.AddAjaxSetting(ddlUnitOfMeasure, ddlInputUnitOfMeasure, ralpOutputUoM);
        }

        protected void mfMetric_PreRender(object sender, EventArgs e)
        {
            ddlMetricType.Attributes.Add("onchange", "fMetricTypeChange('" + ddlMetricType.ClientID + "', '" + ddlDataType.ClientID + "');");
            ddlDataType.Attributes.Add("onchange", "fDataTypeChange('" + ddlDataType.ClientID + "');");            
            ddlInputPeriod.Attributes.Add("onchange", "fClearFormulaBox();");
            InitAjaxManager();
            if (!IsPostBack)
            {
                if (ddlInputUnitOfMeasure.Items.FindByValue(InputID.ToString()) != null)
                    ddlInputUnitOfMeasure.SelectedValue = InputID.ToString();
                if (MetricID == Guid.Empty)
                {            
                    if (CopyMetricID != Guid.Empty)
                    {
                        Bll.Metric.Extend CopyMetric = InitMagicForm();
                        if (CopyMetric != null)
                            RegisterStartScript(CopyMetric.MetricTypeID == 1, CopyMetric.MetricDataTypeID == 1);
                        else
                            RegisterStartScript(true, true);
                    }
                    else
                        RegisterStartScript(true, true);
                }
            }
            else
                RegisterStartScript(ddlMetricType.SelectedValue == "1", ddlDataType.SelectedValue == "1");            
            heExpression.Attributes.Add("readonly", "readonly");
        }

        // Metric Formula select
        protected Bll.MetricFormula MetricFormula
        {
            get
            {
                Bll.MetricFormula mf = Bll.Metric.GetMetricFormula(MetricID != Guid.Empty ? MetricID : CopyMetricID);                
                return mf;
            }
        }

        protected string GetCurrentCommentValue()
        {
            string result = String.Empty;
            HttpCookie _c = HttpContext.Current.Request.Cookies[CommenttCookieName];
            if (_c != null)
                result = Microsoft.JScript.GlobalObject.unescape(_c.Value);
            return result;
        }
        
        // --- Insert ---
        protected void mfMetric_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            if (CheckForUniqueName(e.Values["Name"].ToString()))
            {
                e.Cancel = true;
                this.ErrorMessage = "There is already a metric with the same Name.";
            }
            else
            {
                if (e.Values["CollectionStartDate"] != null && e.Values["CollectionEndDate"] != null)                
                    if (((DateTime)e.Values["CollectionStartDate"]) > ((DateTime)e.Values["CollectionEndDate"]))
                    {
                        e.Cancel = true;
                        this.ErrorMessage = "Start Date should be less than end one.";
                    }
            }
            if (!e.Cancel)
                if (e.Values["MetricTypeID"].ToString() == "1" && e.Values["CollectionStartDate"] == null) // for now IsEmpty may be false even if input element have no data. wait for new MC3
                {
                    e.Cancel = true;
                    this.ErrorMessage = "Start Date is required for Input metrics.";
                }

            if (!e.Cancel)
            {
                Guid MetricID = Guid.NewGuid();
                string Variable = Bll.Metric.GetGuidAsNumber(MetricID);
                e.Values["MetricID"] = MetricID;
                e.Values["Variable"] = Variable;
                e.Values["FormulaCode"] = CreateCodeFromName(e.Values["Name"].ToString());
                if (e.Values["MetricTypeID"].ToString() == "2")
                {
                    e.Values["MetricDataTypeID"] = 1;
                    e.Values["InputUnitOfMeasureID"] = e.Values["UnitOfMeasureID"];
                    e.Values["CollectionStartDate"] = null;
                    e.Values["CollectionEndDate"] = null;
                }
                else
                    e.Values["InputUnitOfMeasureID"] = ddlInputUnitOfMeasure.SelectedValue == String.Empty ? ((Guid?)null) : new Guid(ddlInputUnitOfMeasure.SelectedValue);
                CorrectNullValues(e.Values);
            }
        }

        protected void ldsMetric_Inserted(object sender, LinqDataSourceStatusEventArgs e)
        {
            Bll.Metric m = (Bll.Metric)e.Result;
            if (m.MetricTypeID == 2)
            {
                string Formula = heExpression.Text.Trim();                
                if (!String.IsNullOrEmpty(Formula))
                    Bll.Metric.UpdateMetricFormulaRelations(m.MetricID, 
                        new Guid(hfFormulaID.Value), Formula,
                        DateTime.Parse(hfBeginDate.Value), hfEndDate.Value == String.Empty ? (DateTime?)null : DateTime.Parse(hfEndDate.Value), GetCurrentCommentValue());
            }            
        }

        protected void mfMetric_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            switch (IsSaveAndCopy)
            {
                case false:
                    Response.Redirect("PerformanceIndicatorEdit.aspx?IsNew=True&MetricID=" + e.Values["MetricID"].ToString());
                    break;
                case true:
                    e.KeepInInsertMode = true;
                    break;
                case null:
                default: RedirectToGrid();
                    break;
            }
        }

        // --- Update ---
        protected void mfMetric_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {            
            if (CheckForUniqueName(e.NewValues["Name"].ToString()))
            {
                e.Cancel = true;
                this.ErrorMessage = "There is already a metric with the same Name.";
            }
            else
                if (e.NewValues["CollectionStartDate"] != null && e.NewValues["CollectionEndDate"] != null)
                    if (((DateTime)e.NewValues["CollectionStartDate"]) > ((DateTime)e.NewValues["CollectionEndDate"]))
                    {
                        e.Cancel = true;
                        this.ErrorMessage = "Start Date should be less than end one.";
                    }
            if (!e.Cancel)
                if (e.NewValues["MetricTypeID"].ToString() == "1" && e.NewValues["CollectionStartDate"] == null)
                {
                    e.Cancel = true;
                    this.ErrorMessage = "Start Date is required for Input metrics.";
                }
            if (!e.Cancel)
            {
                e.NewValues["FormulaCode"] = CreateCodeFromName(e.NewValues["Name"].ToString());
                if (e.NewValues["MetricTypeID"].ToString() == "2")
                {
                    e.NewValues["MetricDataTypeID"] = 1;
                    e.NewValues["InputUnitOfMeasureID"] = e.NewValues["UnitOfMeasureID"];
                    e.NewValues["CollectionStartDate"] = null;
                    e.NewValues["CollectionEndDate"] = null;
                }
                else
                    e.NewValues["InputUnitOfMeasureID"] = ddlInputUnitOfMeasure.SelectedValue == String.Empty ? ((Guid?)null) : new Guid(ddlInputUnitOfMeasure.SelectedValue);
                CorrectNullValues(e.NewValues);
            }
        }

        protected void mfMetric_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {            
            Guid OldFormulaID = Guid.Empty;
            Guid NewFormulaID = Guid.Empty;
            if (e.NewValues["MetricTypeID"].ToString() == "2")
            {
                OldFormulaID = new Guid(hfFormulaID.Value);
                string Formula = heExpression.Text.Trim();                
                NewFormulaID = Bll.Metric.UpdateMetricFormulaRelations(MetricID,
                    OldFormulaID, Formula,
                    DateTime.Parse(hfBeginDate.Value), hfEndDate.Value == String.Empty ? (DateTime?)null : DateTime.Parse(hfEndDate.Value), GetCurrentCommentValue());
            }
            // if code is changed - then change related formulas
            if (e.NewValues["FormulaCode"].ToString() != e.OldValues["FormulaCode"].ToString())
                Bll.Metric.ChangeRelatedFormulas(MetricID, e.OldValues["FormulaCode"].ToString(), e.NewValues["FormulaCode"].ToString());

            if (e.NewValues["MetricTypeID"].ToString() == "2")
            {
                // update related metric values
                Bll.MetricValue.MakeFormulaRelatedInputsDirty(OldFormulaID, NewFormulaID);
                // run calc process
                //MetricValuesCalc.ProcessCalc();
            }            
            // Save metric relations
            OrgTree.SaveTree();
            
            // Run Denormalization
            Bll.Mc_EntityNode.DenormalizeOrgLocations();

            // check save&copy opportunity
            switch (IsSaveAndCopy)
            {
                case false:
                    Response.Redirect("PerformanceIndicatorEdit.aspx?IsNew=True&MetricID=" + MetricID.ToString());
                    break;
                case true:
                    Response.Redirect("MetricEdit.aspx?CopyMetricID=" + MetricID.ToString() + (!String.IsNullOrEmpty(PerformanceIndicatorID) ? "&PI=" + PerformanceIndicatorID : String.Empty));
                    break;
                case null:
                default: RedirectToGrid();
                    break;
            }
        }

        // --- Delete ---       

        protected void mfMetric_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            RedirectToGrid();
        }

        // --- Cancel ---

        protected void mfMetric_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        // Check for unique name for updating and inserting
        private bool CheckForUniqueName(string Name)
        {
            return Name == String.Empty ? false : Bll.Metric.CheckUniqueName(Name, MetricID);
        }

        // Create code from name for updating and inserting
        private string CreateCodeFromName(string Name)
        {
            if (String.IsNullOrEmpty(Name)) return Name;
            string result = Name;
            string repChar = Name[0].ToString().ToUpper();
            result = result.Remove(0, 1).Insert(0, repChar);
            for (int i = 1; i < result.Length; i++)
                if ((result[i - 1] == ' ') && (result[i] != ' '))
                {
                    repChar = result[i].ToString().ToUpper();
                    result = result.Remove(i, 1).Insert(i, repChar);
                }
            if ((result[0] >= '1') && (result[0] <= '9'))
                result = "M" + result;            
            result = result.Replace(" ", "");            
            foreach (char c in Bll.Metric.aChars)
                result = result.Replace(c.ToString(), "");
            return result;
        }       

        // Correct null string values for updating and inserting
        private void CorrectNullValues(System.Collections.Specialized.IOrderedDictionary NewValues)
        {
            CorrectNullValue(NewValues, "NODecPlaces");
            CorrectNullValue(NewValues, "NOMinValue");
            CorrectNullValue(NewValues, "NOMaxValue");            
            CorrectNullValue(NewValues, "Alias");
            CorrectNullValue(NewValues, "Code");
        }

        private void CorrectNullValue(System.Collections.Specialized.IOrderedDictionary NewValues, string Name)
        {
            if (String.IsNullOrEmpty(NewValues[Name].ToString())) NewValues[Name] = null;
        }

        private bool? IsSaveAndCopy = null;
        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "SaveCopy" || eventArgument == "SaveCreatePI")
            {
                this.Validate();
                if (Page.IsValid)
                {
                    IsSaveAndCopy = eventArgument == "SaveCopy";
                    if (mfMetric.CurrentMode == DetailsViewMode.Edit)
                        mfMetric.UpdateItem(true);
                    else
                        mfMetric.InsertItem(true);
                }
                else
                    this.ErrorMessage = "Page is not valid";
            }
        }

        protected void ddlUnitOfMeasure_SelectedIndexChanged1(object sender, EventArgs e)
        {            
            if (IsUoMRequest)
                ddlInputUnitOfMeasure.DataBind();
            if (ddlInputUnitOfMeasure.Items.FindByValue(ddlUnitOfMeasure.SelectedValue) != null)
                ddlInputUnitOfMeasure.SelectedValue = ddlUnitOfMeasure.SelectedValue;
        }

        /*SelectedValue='<%# xxx(DataBinder.Eval(Container.DataItem,"InputUnitOfMeasureID")) %>'*/

        protected string xxx(object a)
        {
            string res = a == null ? String.Empty : a.ToString();
            return res;
        }

        protected void ldsUnitOfMeasure_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = Bll.Mc_UnitsOfMeasure.GetOrganizationUoMs(); ;
        }

        protected void ldsInputUnitOfMeasure_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            ddlInputUnitOfMeasure.Items.Clear();            
            Guid? MainUoM = null;
            if (!String.IsNullOrEmpty(ddlUnitOfMeasure.SelectedValue))
                MainUoM = new Guid(ddlUnitOfMeasure.SelectedValue);
            if (MainUoM == null)
            {
                ddlInputUnitOfMeasure.Items.Add(new ListItem(String.Empty, String.Empty));                
                e.Cancel = true;
            }
            else
            {
                List<Micajah.Common.Bll.MeasureUnit> l = Bll.Mc_UnitsOfMeasure.GetConvertedUoMs(MainUoM);
                e.Result = l;
            }
        }
    }
}