﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

using Micajah.Common.WebControls;
using Telerik.Web.UI;
using MetricTrac.Utils;

namespace MetricTrac.Control
{
    public partial class MetricList : System.Web.UI.UserControl
    {
        protected readonly string CheckBoxPrefix = "AddPIMetric";
        public enum enMode { List, PIEdit, PIAdd, PiRef}
        public enMode Mode { get; set; }

        private bool IsNewPerformanceIndicator
        {
            get { return PerformanceIndicatorID == Guid.Empty; }
        }

        private Guid? mPerformanceIndicatorID;
        private bool mPerformanceIndicatorIDReady;
        public Guid? PerformanceIndicatorID
        {
            get
            {
                if (mPerformanceIndicatorIDReady) return mPerformanceIndicatorID;
                mPerformanceIndicatorIDReady = true;
                mPerformanceIndicatorID = null;
                if (Request.QueryString["IsNew"] == "True")
                {
                    mPerformanceIndicatorID = Guid.Empty;
                    return Guid.Empty;
                }
                string s = Request.QueryString["PerformanceIndicatorID"];
                if (string.IsNullOrEmpty(s)) return null;

                try { mPerformanceIndicatorID = new Guid(s); }
                catch { }
                return (Guid)mPerformanceIndicatorID;
            }
        }

        public Guid? FirstMetricID
        {
            get
            {
                string s = Request.QueryString["MetricID"];
                if (string.IsNullOrEmpty(s)) return null;
                Guid? mMetricID = null;
                try { mMetricID = new Guid(s); }
                catch { }
                return mMetricID;
            }
        }
                
        public bool ShowDeleteColumn
        {            
            set { cgvMetric.AutoGenerateDeleteButton = value; }
        }

        MetricCache _MetricCache;
        MetricCache MetricCache
        {
            get
            {
                if (_MetricCache != null) return _MetricCache;
                _MetricCache = new MetricCache((Guid)PerformanceIndicatorID, Session, IsPostBack || Request.QueryString["Continue"] == "True" || Mode==enMode.PIAdd);
                return _MetricCache;
            }
        }

        public Guid?[] FilterPIID { get; set; }        
        public Guid? FilterGCAID { get; set; }
        public Guid?[] FilterOrgLocationsID { get; set; }
        public Guid? FilterDataCollectorID { get; set; }
        public Guid?[] FilterMetricID { get; set; }

        public Guid? FilterMetricCategoryID { get; set; }
        public string FilterNameDescription { get; set; }

        public void SaveInsert(Guid PerformanceIndicatorID)
        {
            MetricCache.Save(PerformanceIndicatorID);
        }

        public void Add()
        {
            foreach (string k in Request.Form.Keys)
            {
                if (string.IsNullOrEmpty(k)) continue;
                if (!k.StartsWith(CheckBoxPrefix)) continue;
                Guid MetricID;
                try { MetricID = new Guid(k.Substring(CheckBoxPrefix.Length)); }
                catch { continue; }

                if (IsNewPerformanceIndicator) MetricCache.Add(MetricID);
                else MetricTrac.Bll.PerformanceIndicatorMetricJunc.Insert((Guid)PerformanceIndicatorID, MetricID);
            }
            if (IsNewPerformanceIndicator) MetricCache.WriteSesion(Session);
        }

        protected void cgvMetric_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (e.NewEditIndex < 0 || e.NewEditIndex >= cgvMetric.DataKeys.Count) return;
            if (Mode==enMode.List)
            {
                Response.Redirect("~/home/KPISetup/MetricEdit.aspx?MetricID=" + cgvMetric.DataKeys[e.NewEditIndex].Value.ToString());
            }
            else
                if (Mode == enMode.PIEdit || Mode == enMode.PiRef)
                    Response.Redirect("~/home/KPISetup/MetricEdit.aspx?MetricID=" + cgvMetric.DataKeys[e.NewEditIndex].Value.ToString() + "&PI=" + PerformanceIndicatorID.ToString());
        }

        protected string AddNewLink
        {
            get
            {
                string _AddNewLink = String.Empty;
                switch (Mode)
                {
                    case enMode.List:
                        _AddNewLink = ResolveUrl("~/home/KPISetup/MetricEdit.aspx");
                        break;
                    case enMode.PIEdit:
                        _AddNewLink = ResolveUrl("~/home/KPISetup/MetricList.aspx?PerformanceIndicatorID=" + PerformanceIndicatorID.ToString());
                        break;
                }
                return _AddNewLink;
            }            
        }

        protected void ldsMetric_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            switch (Mode)
            {
                case enMode.List:
                    e.Result = MetricTrac.Bll.Metric.List(FilterMetricCategoryID, FilterNameDescription);
                    break;
                case enMode.PIAdd:
                    if (IsNewPerformanceIndicator) e.Result = MetricCache.ListUnused(FilterPIID, FilterGCAID, FilterOrgLocationsID, FilterDataCollectorID);
                    else e.Result = MetricTrac.Bll.Metric.ListUnused((Guid)PerformanceIndicatorID, FilterPIID, FilterGCAID, FilterOrgLocationsID, FilterDataCollectorID);
                    break;
                case enMode.PIEdit:
                    if (IsNewPerformanceIndicator) e.Result = MetricCache.List();
                    else e.Result = MetricTrac.Bll.Metric.List((Guid)PerformanceIndicatorID);
                    break;
                case enMode.PiRef:
                    if (IsNewPerformanceIndicator) e.Result = MetricCache.ListReferenced();
                    else e.Result = MetricTrac.Bll.Metric.ListReferenced((Guid)PerformanceIndicatorID);
                    break;
            }            
        }

        protected void cgvMetric_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;
            if (!(e.Keys[0] is Guid)) return;
            Guid MetricID = (Guid)e.Keys[0];

            switch (Mode)
            {
                case enMode.List:
                    MetricTrac.Bll.Metric.Delete(MetricID);
                    Rebind();
                    break;
                case enMode.PIEdit:
                    if (IsNewPerformanceIndicator)
                    {
                        MetricCache.Delete(MetricID);
                    }
                    else
                    {
                        MetricTrac.Bll.PerformanceIndicatorMetricJunc.Delete((Guid)PerformanceIndicatorID, MetricID);
                    }
                    
                    break;
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            cgvMetric.Columns[4].Visible = Mode == enMode.List;
            switch (Mode)
            {           
                case enMode.PIEdit:
                    phAddNew.Visible = false;                    
                    cgvMetric.DeleteButtonCaption = DeleteButtonCaptionType.Remove;
                    ((Micajah.Common.WebControls.HyperLinkField)cgvMetric.Columns[1]).DataNavigateUrlFormatString += "&PI=" + PerformanceIndicatorID.ToString();
                    if (FirstMetricID != null)
                        MetricCache.Add((Guid)FirstMetricID);
                    break;
                case enMode.PIAdd:
                    cgvMetric.AutoGenerateDeleteButton = false;
                    phAddNew.Visible = false;
                    cgvMetric.Columns[0].Visible = true;
                    cgvMetric.Columns[1].Visible = false;
                    cgvMetric.FindField("InputUnitOfMeasureName").HeaderText = "Input Unit";
                    cgvMetric.FindField("UnitOfMeasureName").HeaderText = "Output Unit";
                    break;
                case enMode.PiRef:
                    cgvMetric.AutoGenerateDeleteButton = false;
                    phAddNew.Visible = false;
                    ((Micajah.Common.WebControls.HyperLinkField)cgvMetric.Columns[1]).DataNavigateUrlFormatString += "&PI=" + PerformanceIndicatorID.ToString();
                    break;
            }
        }

        public void Rebind()
        {
            cgvMetric.DataBind();
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if(Mode==enMode.PIEdit && IsNewPerformanceIndicator)
            {
                MetricCache.WriteSesion(Session);
            }
        }

        public Unit Width
        {
            get { return cgvMetric.Width; }
            set { cgvMetric.Width = value; }
        }

        protected void cgvMetric_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {   
                int CellIndex = 6;
                switch (Mode)
                {
                    case enMode.List:
                        CellIndex = 6;
                        break;
                    case enMode.PIAdd:                        
                    case enMode.PIEdit:                        
                    case enMode.PiRef:
                        CellIndex = 5;
                        break;
                }
                if (e.Row.DataItem is MetricTrac.Bll.Metric.Extend)
                {
                    MetricTrac.Bll.Metric.Extend rData = (MetricTrac.Bll.Metric.Extend)e.Row.DataItem;
                    if (rData.AssignedOrgLocations != null)
                        if (rData.AssignedOrgLocations.Count > 1)
                            CreateToolTip(rData.AssignedLocationName, rData.AssignedLocationsNames, /*e.Row.ClientID*/e.Row.Cells[4].ClientID); 
                    if (rData.MetricTypeID == 2)
                        e.Row.Cells[CellIndex].Text = "---";
                }
                else
                {
                    MetricTrac.Bll.Sp_SelectUnassignedMetricResult rData = (MetricTrac.Bll.Sp_SelectUnassignedMetricResult)e.Row.DataItem;
                    //CreateToolTip("", "", e.Row.ClientID); 
                    if (rData.MetricTypeID == 2)
                        e.Row.Cells[CellIndex].Text = "---";
                }
            
            }
        }
        private void CreateToolTip(string AssignedLocationName, string AssignedLocationsNames, string rowClientID)
        {
            RadToolTip rtt = new RadToolTip();
            rtt.Position = ToolTipPosition.BottomCenter;
            rtt.Title = AssignedLocationName;
            rtt.TargetControlID = rowClientID;
            rtt.IsClientID = true;
            rtt.Animation = ToolTipAnimation.Slide;
            rtt.AutoCloseDelay = 12000;
            rtt.Text = AssignedLocationsNames;
            pnlToolTips.Controls.Add(rtt);
        }

        #region Public properties
        #endregion
    }
}
