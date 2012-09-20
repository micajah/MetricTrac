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
    public partial class DataRuleEdit : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {            
            Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;
            dsGroup.WhereParameters.Add("OrganizationId", System.Data.DbType.Guid, user.SelectedOrganization.OrganizationId.ToString());
            if (DataRuleID != Guid.Empty)
            {
                Utils.MetricUtils.InitLinqDataSources(ldsDataCollector);
                ldsDataCollector.WhereParameters.Add("DataRuleID", System.Data.DbType.Guid, DataRuleID.ToString());
                mfDataCollector.ChangeMode(DetailsViewMode.Edit);
            }
            ldsDataCollector.InsertParameters.Add("DataRuleTypeID", DbType.Int32, DataRuleTypeID.ToString());
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {            
            MetricTrac.Control.OrglocationMultipick multiLocations = (MetricTrac.Control.OrglocationMultipick)mfDataCollector.FindControl("orgLocationSelect");
            MetricTrac.Control.GCASelect cASelect = (MetricTrac.Control.GCASelect)mfDataCollector.FindControl("cASelect");
            MetricTrac.Control.MultiSelectList multiPI = (MetricTrac.Control.MultiSelectList)mfDataCollector.FindControl("mslPI");
            MetricTrac.Control.MultiSelectList multiMetric = (MetricTrac.Control.MultiSelectList)mfDataCollector.FindControl("mslMetric");
            if (!IsPostBack)
            {
                List<Bll.DataRule.Entity> dr = Bll.PerformanceIndicator.List().Select(m => new Bll.DataRule.Entity(m.PerformanceIndicatorID, m.Name)).ToList();
                multiPI.LoadEntities(dr, null);

                // Assign
                multiLocations.OrgLocationsID = RuleOrgLocations;
                multiPI.SelectedValues = RulePerformanceIndicators;                
                                
                if (RuleGroupId != null)
                    if (ddlGroup.Items.FindByValue(RuleGroupId.ToString()) != null)
                        ddlGroup.SelectedValue = RuleGroupId.ToString();
                if (RuleUserId != null)
                    if (ddlUser.Items.FindByValue(RuleUserId.ToString()) != null)
                        ddlUser.SelectedValue = RuleUserId.ToString();
            }
            
            List<Bll.DataRule.Entity> Metrics = 
                MetricTrac.Bll.Metric.List(multiLocations.OrgLocationsID, cASelect.GCAID, multiPI.SelectedValues, null)
                .Select(m => new Bll.DataRule.Entity(m.MetricID, m.Name))
                .ToList();
            multiMetric.LoadEntities(Metrics, null);
            if (!IsPostBack)
                multiMetric.SelectedValues = RuleMetrics;
            
        }

        private Guid? RuleGroupId = null;
        private Guid? RuleUserId = null;
        private Guid?[] RuleOrgLocations = null;
        private Guid?[] RulePerformanceIndicators = null;
        private Guid?[] RuleMetrics = null;
        // === Select ===================
        protected void ldsDataCollector_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            Bll.DataRule.Extend rule = Bll.DataRule.Get(DataRuleID);
            RuleOrgLocations = rule.DataRuleOrgLocation;
            RulePerformanceIndicators = rule.DataRulePI;
            RuleMetrics = rule.DataRuleMetric;
            RuleGroupId = rule.GroupId;
            RuleUserId = rule.UserId;
            e.Result = rule;
        }
        // ==============================
        // === Insert ===================
        protected void mfDataCollector_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            Bll.DataRule.Extend UpdateRule = CreateRule(e.Values);
            Bll.DataRule.InsertOrUpdate(UpdateRule);
            e.Cancel = true;
            RedirectToGrid();
        }
        // ==============================
        // === Update ===================
        protected void mfDataCollector_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {
            Bll.DataRule.Extend UpdateRule = CreateRule(e.NewValues);
            Bll.DataRule.InsertOrUpdate(UpdateRule);
            e.Cancel = true;
            RedirectToGrid();
        }
        // ==============================
        
        #region MagicForm Actions
        private Bll.DataRule.Extend CreateRule(System.Collections.Specialized.IOrderedDictionary FormData)
        {
            Bll.DataRule.Extend rule = new MetricTrac.Bll.DataRule.Extend();
            rule.DataRuleID = DataRuleID;
            rule.DataRuleOrgLocation = (Guid?[])FormData["DataRuleOrgLocation"];
            rule.GroupCategoryAspectID = (Guid?)FormData["GroupCategoryAspectID"];
            rule.DataRulePI = (Guid?[])FormData["DataRulePI"];
            rule.DataRuleMetric = (Guid?[])FormData["DataRuleMetric"];
            rule.Description = (string)FormData["Description"];
            rule.DataRuleTypeID = DataRuleTypeID;
            RadioButton rbUser = mfDataCollector.FindControl("rbUser") as RadioButton;
            if (rbUser.Checked)
            {
                rule.GroupId = (Guid?)null;
                rule.UserId = new Guid(ddlUser.SelectedValue);
            }
            else
            {
                rule.UserId = (Guid?)null;
                rule.GroupId = new Guid(ddlGroup.SelectedValue);
            }
            return rule;
        }

        protected void mfDataCollector_ItemDeleting(object sender, DetailsViewDeleteEventArgs e)
        {
            Bll.DataRule.Delete(DataRuleID);
            RedirectToGrid();
        }

        protected void mfDataCollector_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        private void RedirectToGrid()
        {
            Response.Redirect(RedirectListUrl);
        }
        #endregion

        #region Private/Protected Properties
        private Guid mDataRuleID;
        protected Guid DataRuleID
        {
            get
            {
                if (mDataRuleID != Guid.Empty) return mDataRuleID;
                string strID = Request.QueryString["DataRuleID"];
                if (string.IsNullOrEmpty(strID)) return mDataRuleID;
                try
                {
                    mDataRuleID = new Guid(strID);
                }
                catch { }
                return mDataRuleID;
            }
        }

        private DropDownList ddlUser
        {
            get { return mfDataCollector.FindControl("ddlUser") as DropDownList; }
        }

        private DropDownList ddlGroup
        {
            get { return mfDataCollector.FindControl("ddlGroup") as DropDownList; }
        }
        #endregion

        #region Public properties
        public int DataRuleTypeID { get; set; }
        public string RedirectListUrl { get; set; }
        #endregion
    }
}