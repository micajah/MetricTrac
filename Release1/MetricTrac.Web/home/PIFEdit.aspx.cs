using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;

namespace MetricTrac
{
    public partial class PIFEdit : System.Web.UI.Page, IPostBackEventHandler
    {
        private Guid mPIFID;
        protected Guid PerformanceIndicatorFormID
        {
            get
            {
                if (mPIFID != Guid.Empty) return mPIFID;
                try
                {
                    mPIFID = new Guid(Request.QueryString["PerformanceIndicatorFormID"]);
                }
                catch
                {
                    mPIFID = Guid.Empty;
                }
                return mPIFID;
            }
        }

        protected MetricTrac.Control.PerformanceIndicatorList mainPerformanceIndicatorList
        {
            get
            {
                return (MetricTrac.Control.PerformanceIndicatorList)mfPerformanceIndicatorForm.FindControl("mainPerformanceIndicatorList");
            }
        }

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
        }        

        protected void Page_Load(object sender, EventArgs e)
        {
            Utils.MetricUtils.InitLinqDataSources(ldsPerformanceIndicatorForm);
            if (PerformanceIndicatorFormID != Guid.Empty)
            {
                ldsPerformanceIndicatorForm.WhereParameters.Add("PerformanceIndicatorFormID", System.Data.DbType.Guid, PerformanceIndicatorFormID.ToString());
                mfPerformanceIndicatorForm.ChangeMode(DetailsViewMode.Edit);
            }
            else
            {
                // init name box with saved name value                
                string sName = String.Empty;
                if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForPIFormName] != null)
                    sName = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForPIFormName];
                if (!IsPostBack && !String.IsNullOrEmpty(sName))
                    ((TextBoxField)mfPerformanceIndicatorForm.Fields[0]).DefaultText = sName;
                Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForPIFormName] = null;
            }            
            mfPerformanceIndicatorForm.Fields[2].Visible = mfPerformanceIndicatorForm.CurrentMode != DetailsViewMode.Insert;
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (mfPerformanceIndicatorForm.CurrentMode == DetailsViewMode.Insert)
                InitButtonSection();
        }

        private string ErrorMessage
        {
            set { ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage = value; }
            get { return ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage; }
        }

        private bool IsShortInsert = false;
        private Guid InsertedID;
        public void RaisePostBackEvent(string eventArgument)
        {
            if (eventArgument == "assign")
            {
                this.Validate();
                if (Page.IsValid)
                {
                    IsShortInsert = true;
                    mfPerformanceIndicatorForm.InsertItem(true);                    
                }
                else
                    this.ErrorMessage = "Page is not valid";                
            }            
        }

        // different events handlers
        protected void ldsPerformanceIndicatorForm_Selected(object sender, LinqDataSourceStatusEventArgs e)
        {// just init name box with saved value
            string sName = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForPIFormName] != null)
                sName = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForPIFormName];            
            List<Bll.PerformanceIndicatorForm> pif = ((List<Bll.PerformanceIndicatorForm>)e.Result);
            if (!IsPostBack && !String.IsNullOrEmpty(sName))
                if (pif.Count > 0)
                    pif[0].Name = sName;
            Session[Utils.MetricUtils.SessionObjectNameForPIFormName] = null;
        }

        protected void ldsPerformanceIndicatorForm_Inserted(object sender, LinqDataSourceStatusEventArgs e)
        {// save related performance indicators
            InsertedID = ((Bll.PerformanceIndicatorForm)e.Result).PerformanceIndicatorFormID;
            string _AddedPerformanceIndicators = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] != null)
                _AddedPerformanceIndicators = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs];            
            if (!String.IsNullOrEmpty(_AddedPerformanceIndicators))
            {
                string[] _AddedPIList = _AddedPerformanceIndicators.Split('|');
                Guid[] gAddPI = _AddedPIList.Select(s => new Guid(s)).ToArray();
                Bll.PerformanceIndicatorForm.AddPerformanceIndicatorsToForm(((Bll.PerformanceIndicatorForm)e.Result).PerformanceIndicatorFormID, gAddPI);
            }
            
            //Org Tree
            //SaveAssignedOrgTreeLocation(((Bll.PerformanceIndicatorForm)e.Result).PerformanceIndicatorFormID);
        }

        protected void ldsPerformanceIndicatorForm_Updated(object sender, LinqDataSourceStatusEventArgs e)
        {// save and delete related performance indicators            
            string _DeletedPerformanceIndicators = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForDeletedPIs] != null)
                _DeletedPerformanceIndicators = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForDeletedPIs];            
            if (!String.IsNullOrEmpty(_DeletedPerformanceIndicators))
            {
                string[] _DeletedPIList = _DeletedPerformanceIndicators.Split('|');
                Guid[] gDeletePI = _DeletedPIList.Select(s => new Guid(s)).ToArray();
                foreach (Guid pi in gDeletePI)
                    Bll.PerformanceIndicatorForm.RemovePerformanceIndicator(PerformanceIndicatorFormID, pi);                
            }                     
            string _AddedPerformanceIndicators = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] != null)
                _AddedPerformanceIndicators = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs];  
            if (!String.IsNullOrEmpty(_AddedPerformanceIndicators))
            {
                string[] _AddedPIList = _AddedPerformanceIndicators.Split('|');
                Guid[] gAddPI = _AddedPIList.Select(s => new Guid(s)).ToArray();
                Bll.PerformanceIndicatorForm.AddPerformanceIndicatorsToForm(PerformanceIndicatorFormID, gAddPI);
            }

            // Org Tree
            //SaveAssignedOrgTreeLocation(PerformanceIndicatorFormID);            
        }

        /*private void SaveAssignedOrgTreeLocation(Guid PIFID)
        {
            string _RemovedLocations = String.Empty;
            if (Session[Utils.MetricUtils.SessionObjectNameForRemovedOrgLocations] != null)
                _RemovedLocations = (string)Session[Utils.MetricUtils.SessionObjectNameForRemovedOrgLocations];
            if (!String.IsNullOrEmpty(_RemovedLocations))
            {
                string[] __RemovedLocationsList = _RemovedLocations.Split('|');
                Guid[] gRemovedLocations = __RemovedLocationsList.Select(s => new Guid(s)).ToArray();
                //foreach (Guid ont in gRemovedLocations)
                  //  Bll.PerformanceIndicatorForm.RemoveAssignOrgLocation(PIFID, ont);
            }

            string _AddedLocations = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedOrgLocations] != null)
                _AddedLocations = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedOrgLocations];
            if (!String.IsNullOrEmpty(_AddedLocations))
            {
                string[] _AddedLocationsList = _AddedLocations.Split('|');
                Guid[] gAddLocations = _AddedLocationsList.Select(s => new Guid(s)).ToArray();
                //Bll.PerformanceIndicatorForm.AssignOrgLocations(PIFID, gAddLocations);
            }
        }*/

        // Methods just for Form redirect to List
        protected void mfPerformanceIndicatorForm_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {           
            RedirectToGrid();
        }

        protected void mfPerformanceIndicatorForm_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {
            if (IsShortInsert)
            {
                Utils.MetricUtils.ClearSession(Page.Session);
                Response.Redirect(String.Format("PIFOrgLocationJunc.aspx?PerformanceIndicatorFormID={0}", InsertedID.ToString()));
            }
            else
                RedirectToGrid();
        }

        protected void mfPerformanceIndicatorForm_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {
            RedirectToGrid();
        }

        protected void mfPerformanceIndicatorForm_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        private void RedirectToGrid()
        {
            Utils.MetricUtils.ClearSession(Page.Session);
            Response.Redirect("PIFList.aspx");
        }

        protected void lbOrgTreeJunc_Click(object sender, EventArgs e)
        {
            if (Request.Params["ctl00$PageBody$mfPerformanceIndicatorForm$ctl01$txt"] != null)
                Session[Utils.MetricUtils.SessionObjectNameForPIFormName] = Request.Params["ctl00$PageBody$mfPerformanceIndicatorForm$ctl01$txt"].ToString();
            Response.Redirect(String.Format("PIFOrgLocationJunc.aspx?PerformanceIndicatorFormID={0}", PerformanceIndicatorFormID.ToString()));
        }
    }
}