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
using System.Text;
using Telerik.Web.UI;

namespace MetricTrac
{
    public partial class ApproveWorkList : System.Web.UI.Page
    {
        protected bool GroupByMetric { get { return GroupBy.GroupByMetric; } }
        private const int ValueCount = 10;

        protected RadAjaxManager ramManager
        {
            get { return RadAjaxManager.GetCurrent(Page); }
        }
        
        protected void Page_Load(object sender, EventArgs e)
        {            
            ((MetricTrac.MasterPage)(Page.Master)).AddStyleSheet("~/css/WorkList.css");
            ramManager.UpdatePanelsRenderMode = UpdatePanelRenderMode.Inline;
            ramManager.ClientEvents.OnRequestStart = "OnRequestStart";
            ramManager.ClientEvents.OnResponseEnd = "OnResponseEnd";
            ramManager.RequestQueueSize = 10;
            if (!ramManager.IsAjaxRequest)
                ReBind();
            else
                RecreateButtons();
        }       

        void ReBind()
        {
            List<Bll.EntityValue> fms = GetData(null);            
            if (!GroupByMetric)                
                foreach (MetricTrac.Bll.EntityValue mmv in fms)
                    mmv.EntityName = MetricTrac.Bll.Mc_EntityNode.GetHtmlFullName(mmv.EntityName);
            rMetric.DataSource = fms;
            rMetric.DataBind();
        }
        
        protected void rpMetric_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType < ListItemType.Item || e.Item.ItemType > ListItemType.EditItem) return;
            MetricTrac.Bll.EntityValue LastMetricMetricValue = (MetricTrac.Bll.EntityValue)e.Item.DataItem;
            MetricTrac.MTControls.MTGridView cgvMetricValue = (MetricTrac.MTControls.MTGridView)e.Item.FindControl("cgvMetricValue");
            Panel pnlPager = (Panel)e.Item.FindControl("pnlPager");
            Panel pnlUpdate = (Panel)e.Item.FindControl("pnlUpdate");
            CreateLinkButtons(pnlPager, LastMetricMetricValue.EntityID, LastMetricMetricValue.PageCount, LastMetricMetricValue.IsMoreValues);
            HiddenField hfLinksMemory = (HiddenField)pnlUpdate.FindControl("hfLinksMemory");
            hfLinksMemory.Value = LastMetricMetricValue.EntityID.ToString() + "|" + LastMetricMetricValue.PageCount.ToString() + "|" + LastMetricMetricValue.IsMoreValues.ToString();
            cgvMetricValue.DataSource = LastMetricMetricValue.EntityValues;
            cgvMetricValue.DataBind();
        }        
        
        protected void rMetric_PreRender(object sender, EventArgs e)
        {            
            Repeater rMetric = (Repeater)sender;
            foreach (RepeaterItem ri in rMetric.Items)
                if (ri.ItemType == ListItemType.Item || ri.ItemType == ListItemType.AlternatingItem)
                {
                    MetricTrac.MTControls.MTGridView cgvMetricValue = (MetricTrac.MTControls.MTGridView)ri.FindControl("cgvMetricValue");
                    Panel pnlPager = (Panel)ri.FindControl("pnlPager");
                    Panel pnlUpdate = (Panel)ri.FindControl("pnlUpdate");
                    foreach (System.Web.UI.Control c in pnlPager.Controls)
                        if (c is LinkButton)
                            ramManager.AjaxSettings.AddAjaxSetting(c, pnlUpdate, ralGrid);
                }
        }

        protected void  lb_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName != "UpdateGrid") return;
            Bll.PageEntityID fed = new Bll.PageEntityID();
            bool IsMoreValues = false;
            ParseComplexEntity(e.CommandArgument.ToString(), out fed.EntityID, out fed.PageNumber, out IsMoreValues);
            List<MetricTrac.Bll.EntityValue> fms = GetData(fed);            
            MetricTrac.MTControls.MTGridView cgvMetricValue = (MetricTrac.MTControls.MTGridView)(((System.Web.UI.Control)sender).Parent.Parent).FindControl("cgvMetricValue");
            if (fms.Count != 1) return;            
            MetricTrac.Bll.EntityValue LastMetricMetricValue = fms[0];
            Panel pnlPager = (Panel)(((System.Web.UI.Control)sender).Parent);
            Panel pnlUpdate = (Panel)(((System.Web.UI.Control)sender).Parent.Parent);
            pnlPager.Controls.Clear();            
            CreateLinkButtons(pnlPager, LastMetricMetricValue.EntityID, LastMetricMetricValue.PageCount, LastMetricMetricValue.IsMoreValues);
            HiddenField hfLinksMemory = (HiddenField)pnlUpdate.FindControl("hfLinksMemory");
            hfLinksMemory.Value = LastMetricMetricValue.EntityID.ToString() + "|" + LastMetricMetricValue.PageCount.ToString() + "|" + LastMetricMetricValue.IsMoreValues.ToString();            
            cgvMetricValue.DataSource = LastMetricMetricValue.EntityValues;
            cgvMetricValue.DataBind();
        }

        private void RecreateButtons()
        {       
            foreach (RepeaterItem ri in rMetric.Items)
            {
                Panel pnlPager = (Panel)ri.FindControl("pnlPager");
                Panel pnlUpdate = (Panel)ri.FindControl("pnlUpdate");
                HiddenField hfLinksMemory = (HiddenField)pnlUpdate.FindControl("hfLinksMemory");                
                Guid EntityID = Guid.Empty;
                int PageCount = 1;
                bool IsMoreValues = false;
                ParseComplexEntity(hfLinksMemory.Value, out EntityID, out PageCount, out IsMoreValues);
                CreateLinkButtons(pnlPager, EntityID, PageCount, IsMoreValues);
            }
        }

        private void ParseComplexEntity(string Arg, out Guid EntityID, out int PageCount, out bool IsMoreValues)
        {            
            EntityID = Guid.Empty;
            PageCount = 1;
            IsMoreValues = false;
            string[] arguments = Arg.ToString().Split('|');
            if (arguments.Length != 3) return;
            try
            {                
                EntityID = new Guid(arguments[0]);
                int.TryParse(arguments[1], out PageCount);
                bool.TryParse(arguments[2], out IsMoreValues);
            }
            catch { }
        }

        private void CreateLinkButtons(Panel pnlPager, Guid EntityID, int PageCount, bool IsMoreValues)
        {
            LiteralControl lc = new LiteralControl("<div style=\"line-height:25px; width:100%; text-align:right;\">");
            pnlPager.Controls.Add(lc);
            if ((PageCount > 1) || (IsMoreValues))
            {
                for (int i = 0; i < PageCount; i++)
                    if (i == (PageCount - 1))
                    {
                        lc = new LiteralControl("<span>" + (i + 1).ToString() + "</span><span>&nbsp;&nbsp;&nbsp;</span>");
                        pnlPager.Controls.Add(lc);
                    }
                    else
                    {
                        lc = new LiteralControl("<span>");
                        pnlPager.Controls.Add(lc);
                        LinkButton lb = new LinkButton();
                        lb.ID = "lbUpdateCommand" + i.ToString();
                        lb.CommandName = "UpdateGrid";
                        lb.CommandArgument = EntityID.ToString() + "|" + i.ToString() + "|" + IsMoreValues.ToString();                        
                        lb.Text = (i + 1).ToString();
                        lb.Command += new CommandEventHandler(lb_Command);
                        pnlPager.Controls.Add(lb);
                        lc = new LiteralControl("</span><span>&nbsp;&nbsp;&nbsp;</span>");
                        pnlPager.Controls.Add(lc);
                    }
                if (IsMoreValues)
                {
                    lc = new LiteralControl("<span>");
                    pnlPager.Controls.Add(lc);
                    LinkButton lb = new LinkButton();
                    lb.ID = "lbUpdateCommand_1";
                    lb.CommandName = "UpdateGrid";
                    lb.CommandArgument = EntityID.ToString() + "|" + PageCount.ToString() + "|" + IsMoreValues.ToString();
                    lb.Text = "Show&nbsp;More";
                    lb.Command += new CommandEventHandler(lb_Command);
                    pnlPager.Controls.Add(lb);
                    lc = new LiteralControl("</span><span>&nbsp;&nbsp;&nbsp;</span>");
                    pnlPager.Controls.Add(lc);                    
                }
            }
            lc = new LiteralControl("</div>");
            pnlPager.Controls.Add(lc);
        }

        private List<MetricTrac.Bll.EntityValue> GetData(Bll.PageEntityID fed)
        {
            Guid? ApproverUserId = MetricTrac.Bll.LinqMicajahDataContext.LogedUserId;
            return MetricTrac.Bll.MetricValue.WorkList(ValueCount, fed, ApproverUserId, GroupByMetric);
        }

        //----------

        MetricTrac.Bll.EntityValue GetMetric(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            return ri.DataItem as MetricTrac.Bll.EntityValue;
        }

        MetricTrac.Bll.MetricValue.Extend GetValue(object container)
        {
            System.Web.UI.WebControls.GridViewRow ri = container as System.Web.UI.WebControls.GridViewRow;
            return ri.DataItem as MetricTrac.Bll.MetricValue.Extend;
        }

        protected string GetEditUrl(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            return "MetricInputEdit.aspx?MetricID=" + val.MetricID +
                "&Date=" + val.Date.ToString("MM-dd-yyyy") +
                "&OrgLocationID=" + val.OrgLocationID +
                "&Mode=Approve" +
                "&BackPage=9";
        }

        protected string GetOnClickHandler(object container)
        {
            string OnClickHandler = String.Empty;
            if (MetricTrac.Utils.MetricUtils.IsPopupSupported(Request))
                OnClickHandler = "onclick=\"openRadWindow('" + GetEditUrl(container) + "');return false;\"";
            return OnClickHandler;
        }

        protected string GetValueCell(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            return Bll.MetricValue.FormatValue(val) + "&nbsp;" + val.ValueInputUnitOfMeasureName;            
        }

        protected string GetValueTitle(object container)
        {
            MetricTrac.Bll.MetricValue.Extend val = GetValue(container);
            string title = String.Empty;            
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
            return title;
        }
    }
}