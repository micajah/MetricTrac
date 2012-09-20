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
            dsPIForm.WhereParameters.Add("Status", System.Data.DbType.Boolean, "True");
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
            if (!IsPostBack)
            {
                if (SelectedRule == null) return;
                if (SelectedRule.UserId == null && SelectedRule.GroupId == null) return;
                if (SelectedRule.UserId == null)
                {
                    if (ddlGroup.Items.FindByValue(SelectedRule.GroupId.ToString()) != null)
                        ddlGroup.SelectedValue = SelectedRule.GroupId.ToString();
                }
                else
                {
                    if (ddlUser.Items.FindByValue(SelectedRule.UserId.ToString()) != null)
                        ddlUser.SelectedValue = SelectedRule.UserId.ToString();
                }
            }
            else
                ddlMetric.DataBind();
        }

        MetricTrac.Bll.DataRule SelectedRule = null;
        protected void ldsDataCollector_Selected(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (e.Result is List<MetricTrac.Bll.DataRule>)
            {
                List<MetricTrac.Bll.DataRule> r = e.Result as List<MetricTrac.Bll.DataRule>;
                if (r.Count > 0)
                    SelectedRule = (MetricTrac.Bll.DataRule)r[0];
            }   
        }

        protected void dsMetric_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {            
            MetricTrac.Control.GCASelect cASelect = (MetricTrac.Control.GCASelect)mfDataCollector.FindControl("cASelect");
            Guid?[] OrgLocationsId =
                (!IsPostBack) ?
                    (SelectedRule != null ?
                        (SelectedRule.RuleClusterID != null ?
                            MetricTrac.Bll.DataRule.OrgLocationsClusterList((Guid)SelectedRule.RuleClusterID) 
                            : new Guid?[1] {SelectedRule.OrgLocationID}) 
                        : new Guid?[0])
                : SelectOrgLocationsID;
            e.Result = MetricTrac.Bll.Metric.List(OrgLocationsId, cASelect.GCAID, GetDdlSelectedValue(ddlPI), GetDdlSelectedValue(ddlPIF), null).ToList();        
        }

        protected void ldsDataCollector_Inserted(object sender, LinqDataSourceStatusEventArgs e)
        {
            if (SelectOrgLocationsID.Length > 1 && e.Result != null)
                MetricTrac.Bll.DataRule.InsertOrgLocationsClusterList(((MetricTrac.Bll.DataRule)e.Result).DataRuleID, SelectOrgLocationsID);
        }

        protected void ldsDataCollector_Updating(object sender, LinqDataSourceUpdateEventArgs e)
        {
            MetricTrac.Bll.DataRule drOld = (MetricTrac.Bll.DataRule)e.OriginalObject;
            MetricTrac.Bll.DataRule drNew = (MetricTrac.Bll.DataRule)e.NewObject;

            if (drOld.RuleClusterID != null)
                MetricTrac.Bll.DataRule.DeleteOrgLocationsClusterList((Guid)drOld.RuleClusterID, (Guid?)drOld.DataRuleID);
            drNew.OrgLocationID = (SelectOrgLocationsID.Length > 0) ? SelectOrgLocationsID[0] : null;
            if (SelectOrgLocationsID.Length > 1 && drOld.RuleClusterID == null)
                drNew.RuleClusterID = Guid.NewGuid();

            if (SelectOrgLocationsID.Length <= 1 && drOld.RuleClusterID != null)
                drNew.RuleClusterID = null;
        }

        protected void ldsDataCollector_Updated(object sender, LinqDataSourceStatusEventArgs e)
        {
            MetricTrac.Bll.DataRule UpdatedRule = (MetricTrac.Bll.DataRule)e.Result;
            if (SelectOrgLocationsID.Length > 1)
                MetricTrac.Bll.DataRule.InsertOrgLocationsClusterList(UpdatedRule.DataRuleID, SelectOrgLocationsID);
        }

        private Guid? GetDdlSelectedValue(DropDownList ddl)
        {
            if (string.IsNullOrEmpty(ddl.SelectedValue)) return null;
            try { return new Guid(ddl.SelectedValue); }
            catch { return null; }
        }

        #region MagicForm handlers
        protected void mfDataCollector_DataBound(object sender, EventArgs e)
        {
            if (SelectedRule != null)
            {
                if (SelectedRule.RuleClusterID != null)
                    SelectOrgLocationsID = MetricTrac.Bll.DataRule.OrgLocationsClusterList((Guid)SelectedRule.RuleClusterID);
                else
                    SelectOrgLocationsID = new Guid?[1] { SelectedRule.OrgLocationID };
            }
        }

        // Before
        protected void mfDataCollector_ItemInserting(object sender, DetailsViewInsertEventArgs e)
        {
            if (SelectOrgLocationsID.Length > 0)
            {
                e.Values["OrgLocationID"] = SelectOrgLocationsID[0];
                if (SelectOrgLocationsID.Length > 1)
                    e.Values["RuleClusterID"] = Guid.NewGuid();
            }
            UpdateUserGroup(e.Values);
        }

        protected void mfDataCollector_ItemUpdating(object sender, DetailsViewUpdateEventArgs e)
        {   
            UpdateUserGroup(e.NewValues);
        }

        private void UpdateUserGroup(System.Collections.Specialized.IOrderedDictionary v)
        {
            RadioButton rbUser = mfDataCollector.FindControl("rbUser") as RadioButton;
            if (rbUser.Checked)
            {
                v["GroupId"] = (Guid?)null;
                v["UserId"] = new Guid(ddlUser.SelectedValue);
            }
            else
            {
                v["UserId"] = (Guid?)null;
                v["GroupId"] = new Guid(ddlGroup.SelectedValue);
            }
        }

        // After
        protected void mfDataCollector_ItemUpdated(object sender, DetailsViewUpdatedEventArgs e)
        {   
            RedirectToGrid();
        }

        protected void mfMDataCollector_ItemInserted(object sender, DetailsViewInsertedEventArgs e)
        {            
            RedirectToGrid();
        }

        protected void mfDataCollector_ItemDeleted(object sender, DetailsViewDeletedEventArgs e)
        {   
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
        private DropDownList ddlUser
        {
            get { return mfDataCollector.FindControl("ddlUser") as DropDownList; }
        }

        private DropDownList ddlGroup
        {
            get { return mfDataCollector.FindControl("ddlGroup") as DropDownList; }
        }

        protected Guid? SelectOrgLocationID
        {
            get
            {
                return orgLocationSelect.OrgLocationID;
            }
            set
            {
                orgLocationSelect.OrgLocationID = value;
            }
        }

        protected Guid?[] SelectOrgLocationsID
        {
            get
            {
                return orgLocationSelect.OrgLocationsID;
            }
            set
            {
                orgLocationSelect.OrgLocationsID = value;
            }
        }

        private OrglocationMultipick orgLocationSelect
        {
            get { return mfDataCollector.FindControl("orgLocationSelect") as OrglocationMultipick; }
        }

        DropDownList mddlPIF;
        DropDownList ddlPIF
        {
            get
            {
                if (mddlPIF != null) return mddlPIF;
                mddlPIF = GetDDL("PerformanceIndicatorFormID");
                return mddlPIF;
            }
        }
        DropDownList mddlPI;
        DropDownList ddlPI
        {
            get
            {
                if (mddlPI != null) return mddlPI;
                mddlPI = GetDDL("PerformanceIndicatorID");
                return mddlPI;
            }
        }
        DropDownList mddlMetric;
        DropDownList ddlMetric
        {
            get
            {
                if (mddlMetric != null) return mddlMetric;
                mddlMetric = GetDDL("MetricID");
                return mddlMetric;
            }
        }

        DropDownList GetDDL(string DataField)
        {
            int FieldsIndex = -1;
            for (int i = 0; i < mfDataCollector.Fields.Count; i++)
            {
                object f = mfDataCollector.Fields[i];
                Type t = f.GetType();
                System.Reflection.PropertyInfo pi = t.GetProperty("DataField");
                if (pi == null) continue;
                string df = pi.GetValue(f, null).ToString();
                if (df == DataField)
                {
                    FieldsIndex = i;
                    break;
                }
            }
            if (FieldsIndex < 0) return null;
            DetailsViewRow r = mfDataCollector.Controls[0].Controls[FieldsIndex + 1] as DetailsViewRow;
            DropDownList ddl = r.Cells[1].Controls[1] as DropDownList;
            return ddl;
        }
        #endregion

        #region Public properties
        public int DataRuleTypeID { get; set; }
        public string RedirectListUrl { get; set; }

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
        #endregion
    }
}