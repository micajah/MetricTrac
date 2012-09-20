﻿using System;
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

namespace MetricTrac.Control
{    
    public partial class PerformanceIndicatorList : System.Web.UI.UserControl
    {        
        private Guid _PerformanceIndicatorFormID;
        public Guid PerformanceIndicatorFormID
        {
            get
            {
                if (_PerformanceIndicatorFormID != Guid.Empty) return _PerformanceIndicatorFormID;
                try
                {
                    _PerformanceIndicatorFormID = new Guid(Request.QueryString["PerformanceIndicatorFormID"]);
                }
                catch
                {
                    _PerformanceIndicatorFormID = Guid.Empty;
                }
                return _PerformanceIndicatorFormID;
            }
        }

        private Guid _GCAID;
        public Guid GCAID
        {            
            get { return _GCAID; }
            set { _GCAID = value; }
        }

        private int _SectorID;
        public int SectorID
        {
            get { return _SectorID; }
            set { _SectorID = value; }
        }

        private MetricTrac.Bll.PerformanceIndicatorListMode _Mode;
        public MetricTrac.Bll.PerformanceIndicatorListMode ListMode
        {
            set { _Mode = value; }
            get { return _Mode; }
        }

        public string SelectedIndicators
        {
            get
            {
                string _spi = String.Empty;
                if (ListMode == MetricTrac.Bll.PerformanceIndicatorListMode.FormSelect)
                {
                    for (int i = 0; i < cgvPerformanceIndicator.Rows.Count; i++)
                    {
                        System.Web.UI.WebControls.CheckBox chbProjectCheck = (System.Web.UI.WebControls.CheckBox)cgvPerformanceIndicator.Rows[i].FindControl("chbProjectCheck");
                        if (chbProjectCheck != null)
                            if (chbProjectCheck.Checked)
                            {
                                HiddenField hfComplexId = (HiddenField)cgvPerformanceIndicator.Rows[i].FindControl("hfComplexId");
                                if (hfComplexId != null) _spi += hfComplexId.Value + "|";
                            }
                    }
                    _spi = _spi.TrimEnd('|');
                }                
                return _spi;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {            
            switch (ListMode)
            {
                case MetricTrac.Bll.PerformanceIndicatorListMode.List:
                    cgvPerformanceIndicator.DataSourceID = "ldsPerformanceIndicator";
                    cgvPerformanceIndicator.ShowAddLink = true;
                    cgvPerformanceIndicator.AutoGenerateEditButton = false;                    
                    cgvPerformanceIndicator.AutoGenerateDeleteButton = true;
                    cgvPerformanceIndicator.RowEditing += new GridViewEditEventHandler(cgvPerformanceIndicator_RowEditing);
                    cgvPerformanceIndicator.Action += new EventHandler<CommonGridViewActionEventArgs>(cgvPerformanceIndicator_Action);
                    cgvPerformanceIndicator.Columns[0].Visible = false;
                    cgvPerformanceIndicator.Columns[1].Visible = true;
                    break;
                case MetricTrac.Bll.PerformanceIndicatorListMode.FormJunc:
                    cgvPerformanceIndicator.DataSourceID = "ldsPerformanceIndicatorFormPerformanceIndicators";
                    cgvPerformanceIndicator.ShowAddLink = true;
                    cgvPerformanceIndicator.AutoGenerateEditButton = false;
                    cgvPerformanceIndicator.AutoGenerateDeleteButton = true;                    
                    cgvPerformanceIndicator.Action += new EventHandler<CommonGridViewActionEventArgs>(cgvPerformanceIndicator_Action);
                    cgvPerformanceIndicator.Columns[0].Visible = false;
                    cgvPerformanceIndicator.Columns[1].Visible = false;
                    cgvPerformanceIndicator.DeleteButtonCaption = DeleteButtonCaptionType.Remove;
                    break;
                case MetricTrac.Bll.PerformanceIndicatorListMode.FormSelect:
                    cgvPerformanceIndicator.DataSourceID = "ldsPerformanceIndicator";
                    cgvPerformanceIndicator.ShowAddLink = false;
                    cgvPerformanceIndicator.AutoGenerateEditButton = false;
                    cgvPerformanceIndicator.AutoGenerateDeleteButton = false;
                    cgvPerformanceIndicator.Columns[0].Visible = true;
                    cgvPerformanceIndicator.Columns[1].Visible = false;
                    cgvPerformanceIndicator.RowDataBound += new GridViewRowEventHandler(cgvPerformanceIndicator_RowDataBound);
                    break;
            }
            Utils.MetricUtils.InitLinqDataSources(ldsPerformanceIndicator);
            Utils.MetricUtils.InitLinqDataSources(ldsPerformanceIndicatorFormPerformanceIndicators);           
            Parameter PIFIdParam = new Parameter("PerformanceIndicatorFormID", System.Data.DbType.Guid, MetricTrac.Bll.LinqMicajahDataContext.InstanceId.ToString());
            ldsPerformanceIndicatorFormPerformanceIndicators.WhereParameters.Add(PIFIdParam);            
        }

        // === simple List and Select for PerformanceIndicatorForm modes ===
            // grid
        void cgvPerformanceIndicator_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                System.Web.UI.WebControls.CheckBox chbHeader = (System.Web.UI.WebControls.CheckBox)e.Row.FindControl("chbHeaderCheck");
                chbHeader.Checked = false;
                chbHeader.Attributes.Add("onclick", String.Format("toggleCheckBoxes('{0}');", chbHeader.ClientID));
            }
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            MetricTrac.Bll.PerformanceIndicator.Extend rData = (MetricTrac.Bll.PerformanceIndicator.Extend)e.Row.DataItem;
            HiddenField hfComplexId = (HiddenField)e.Row.FindControl("hfComplexId");
            if (hfComplexId != null) hfComplexId.Value = rData.PerformanceIndicatorID.ToString();
            //CreateToolTip(rData, e.Row.ClientID);            
        }

        private void CreateToolTip(MetricTrac.Bll.PerformanceIndicator.Extend rData, string rowClientID)
        {
            RadToolTip rtt = new RadToolTip();            
            rtt.Position = ToolTipPosition.BottomCenter;
            rtt.Title = rData.Name;            
            rtt.TargetControlID = rowClientID;
            rtt.IsClientID = true;
            rtt.Animation = ToolTipAnimation.Slide;       
            rtt.AutoCloseDelay = 12000;
            rtt.Text = "<table border=\"0\" cellpadding=\"2px\" cellspacing=\"0\" style=\"color:Gray;width:300px;\">" +
                "<tr><td class=\"GridHeader\">Description</td><td>" + (String.IsNullOrEmpty(rData.Description) ? "---" : rData.Description) + "</td>" +
                "<tr><td class=\"GridHeader\">Group->Category->Aspect</td><td>" + (String.IsNullOrEmpty(rData.GCAName) ? "---" : rData.GCAName) + "</td>" +
                "<tr><td class=\"GridHeader\">Sector</td><td>" + (String.IsNullOrEmpty(rData.SectorName) ? "---" : rData.SectorName) + "</td>" +
                "<tr><td class=\"GridHeader\">Requirement</td><td>" + (String.IsNullOrEmpty(rData.RequirementName) ? "---" : rData.RequirementName) + "</td>" +
                "<tr><td class=\"GridHeader\">Help Text</td><td>" + (String.IsNullOrEmpty(rData.Help) ? "---" : rData.Help) + "</td>" +
                "<tr><td colspan=\"2\"><span class=\"GridHeader\">Metrics</span><br />";
                

            List<MetricTrac.Bll.Metric.Extend> _Metrics = MetricTrac.Bll.Metric.List(rData.PerformanceIndicatorID);
            if (_Metrics.Count > 0)
            {
                rtt.Text += "<table border=\"0\" cellpadding=\"2px\" cellspacing=\"0\" style=\"color:Gray;\">";
                rtt.Text += "<tr style=\"background-color:Gray;color:White;\"><td>Category</td><td>Name</td><td>Input&nbsp;Unit&nbsp;of&nbsp;Measure</td><td>Output&nbsp;Unit&nbsp;of&nbsp;Measure</td><td>Collection&nbsp;Frequency</td><td>Description</td><tr>";
                foreach (MetricTrac.Bll.Metric.Extend m in _Metrics)
                    rtt.Text += "<tr><td>" + m.MetricCategoryName + "</td><td>" + m.Name + "</td><td>" + m.InputUnitOfMeasureName + "</td><td>" + m.UnitOfMeasureName + "</td><td>" + m.FrequencyName + "</td><td>" + m.Notes + "</td></tr>";
                rtt.Text += "</table>";
            }
            else
                rtt.Text += "No assigned Metrics";
            rtt.Text += "</td></tr></table>";
            pnlToolTips.Controls.Add(rtt);
        }

        protected void cgvPerformanceIndicator_RowEditing(object sender, GridViewEditEventArgs e)
        {
            if (e.NewEditIndex < 0 || e.NewEditIndex >= cgvPerformanceIndicator.DataKeys.Count) return;
            Response.Redirect("PerformanceIndicatorEdit.aspx?PerformanceIndicatorID=" + cgvPerformanceIndicator.DataKeys[e.NewEditIndex].Value.ToString());
        }

        protected void cgvPerformanceIndicator_Action(object sender, Micajah.Common.WebControls.CommonGridViewActionEventArgs e)
        {
            if (e.Action != CommandActions.Add) return;
            if (ListMode == MetricTrac.Bll.PerformanceIndicatorListMode.List)
                Response.Redirect("PerformanceIndicatorEdit.aspx?IsNew=True");
            else
            {
                if (Request.Params["ctl00$PageBody$mfPerformanceIndicatorForm$ctl01$txt"] != null)
                    Session[Utils.MetricUtils.SessionObjectNameForPIFormName] = Request.Params["ctl00$PageBody$mfPerformanceIndicatorForm$ctl01$txt"].ToString();
                Response.Redirect(String.Format("PIFPIJunc.aspx?PerformanceIndicatorFormID={0}", PerformanceIndicatorFormID.ToString()));
            }
        }

            //datasource
        protected void ldsPerformanceIndicator_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (ListMode == MetricTrac.Bll.PerformanceIndicatorListMode.List) e.Result = Bll.PerformanceIndicator.List(GCAID, SectorID);
            else
            { // more complex select 
                // for selecting grid we should show unassigned  pi                
                // - that have been added to assigned (session) earlier
                List<Bll.PerformanceIndicator.Extend> _UnassignedInDbPerformanceIndicator = Bll.PerformanceIndicatorForm.UnassignedPerformanceIndicatorList(PerformanceIndicatorFormID, GCAID);
                string _AddedPerformanceIndicators = String.Empty;
                if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] != null)
                    _AddedPerformanceIndicators = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs];
                List<Bll.PerformanceIndicator.Extend> ToDelete = new List<Bll.PerformanceIndicator.Extend>();
                if (!String.IsNullOrEmpty(_AddedPerformanceIndicators))
                {
                    foreach (Bll.PerformanceIndicator.Extend un in _UnassignedInDbPerformanceIndicator)
                        if (_AddedPerformanceIndicators.Contains(un.PerformanceIndicatorID.ToString()))
                            ToDelete.Add(un);
                    foreach (Bll.PerformanceIndicator.Extend mi in ToDelete)
                        _UnassignedInDbPerformanceIndicator.Remove(mi); 
                }
                e.Result = _UnassignedInDbPerformanceIndicator;
            }
        }
        //==============
        
        protected void ldsPerformanceIndicatorFormPerformanceIndicators_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {            
            string _AddedPerformanceIndicators = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] != null)
                _AddedPerformanceIndicators = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs];
            string _DeletedPerformanceIndicators = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForDeletedPIs] != null)
                _DeletedPerformanceIndicators = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForDeletedPIs];            
            List<Bll.PerformanceIndicator.Extend> alAddedPI = new List<Bll.PerformanceIndicator.Extend>();
            if (!String.IsNullOrEmpty(_AddedPerformanceIndicators))
            {
                string[] _AddedPIList = _AddedPerformanceIndicators.Split('|');
                Guid[] gAddPI = _AddedPIList.Select(s => new Guid(s)).ToArray();
                alAddedPI = Bll.PerformanceIndicatorForm.PerformanceIndicatorsList(gAddPI);
            }
            if (PerformanceIndicatorFormID != Guid.Empty)            
            {
                List<Bll.PerformanceIndicator.Extend> _ListAssignedPI = Bll.PerformanceIndicatorForm.AssignedPerformanceIndicatorsList(PerformanceIndicatorFormID).ToList<Bll.PerformanceIndicator.Extend>();
                List<Bll.PerformanceIndicator.Extend> ToDelete = new List<Bll.PerformanceIndicator.Extend>();
                // delete that contains in session
                foreach (Bll.PerformanceIndicator.Extend mi in _ListAssignedPI)
                    if (_DeletedPerformanceIndicators.Contains(mi.PerformanceIndicatorID.ToString()))
                        ToDelete.Add(mi);
                foreach (Bll.PerformanceIndicator.Extend mi in ToDelete)
                    _ListAssignedPI.Remove(mi);                
                // add from session
                foreach (Bll.PerformanceIndicator.Extend mi in alAddedPI)
                    _ListAssignedPI.Add(mi);
                e.Result = _ListAssignedPI;
            }
            else e.Result = alAddedPI;            
        }

        protected void ldsPerformanceIndicatorFormPerformanceIndicators_Deleting(object sender, LinqDataSourceDeleteEventArgs e)
        {
            string _AddedPI = String.Empty;
            if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] != null)
                _AddedPI = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs];            
            string delPerformanceIndicator = ((Bll.PerformanceIndicatorFormPerformanceIndicatorJunc)e.OriginalObject).PerformanceIndicatorID.ToString();

            if (_AddedPI.Contains(delPerformanceIndicator))
            {
                _AddedPI = _AddedPI.Replace(delPerformanceIndicator, "");
                _AddedPI = _AddedPI.Replace("||", "|");
                _AddedPI = _AddedPI.TrimStart('|');
                _AddedPI = _AddedPI.TrimEnd('|');
                if (String.IsNullOrEmpty(_AddedPI)) 
                    Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] = null;
                else Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForAddedPIs] = _AddedPI;
            }
            else
            {                
                string _DeletedPI = String.Empty;
                if (Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForDeletedPIs] != null)
                    _DeletedPI = (string)Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForDeletedPIs];            
                if (String.IsNullOrEmpty(_DeletedPI))
                    _DeletedPI = delPerformanceIndicator;
                else _DeletedPI = _DeletedPI + "|" + delPerformanceIndicator;
                Session[MetricTrac.Utils.MetricUtils.SessionObjectNameForDeletedPIs] = _DeletedPI;
            }
            e.Cancel = true;
        }

        public Unit Width
        {
            get { return cgvPerformanceIndicator.Width; }
            set { cgvPerformanceIndicator.Width = value; }
        }
    }
}