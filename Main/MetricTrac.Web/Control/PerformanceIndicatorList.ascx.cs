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

namespace MetricTrac.Control
{    
    public partial class PerformanceIndicatorList : System.Web.UI.UserControl
    {
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

        protected void Page_Load(object sender, EventArgs e)
        {  
            Utils.MetricUtils.InitLinqDataSources(ldsPerformanceIndicator);
        }

        protected void cgvPerformanceIndicator_Action(object sender, Micajah.Common.WebControls.CommonGridViewActionEventArgs e)
        {
            if (e.Action == CommandActions.Add)
                Response.Redirect("PerformanceIndicatorEdit.aspx?IsNew=True");
        }
        
        protected void ldsPerformanceIndicator_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = Bll.PerformanceIndicator.List(GCAID, SectorID);            
        }

        // using this method on RowDataBound - CreateToolTip(rData, e.Row.ClientID);
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


            /*List<MetricTrac.Bll.Metric.Extend> _Metrics = MetricTrac.Bll.Metric.List(rData.PerformanceIndicatorID);
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
            rtt.Text += "</td></tr></table>";*/
            pnlToolTips.Controls.Add(rtt);
        }
    }
}
