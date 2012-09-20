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
    public partial class ScoreCardList : System.Web.UI.UserControl
    {
        public bool MyDashboardMode { get; set; }
        public Guid? ScoreCardMetricID
        {
            get { return (Guid?)ViewState["ScoreCardMetricID"]; }
            set { ViewState["ScoreCardMetricID"] = value; }
        }
        public event EventHandler SelectedIndexChanged;

        private Guid? LastUser = Guid.Empty;
        private bool IsPublicShown = false;
        private bool AjaxInitialized;
        protected Guid LoggedinUserId
        {
            get
            {
                Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;
                return user.UserId;
            }
        }

        int HiddenScoreCardCount = MetricTrac.Bll.ScoreCard.HiddenScoreCardsCount();
        protected void rScoreCard_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            MetricTrac.Bll.ScoreCard.Extend sc = e.Item.DataItem as MetricTrac.Bll.ScoreCard.Extend;
            if (sc == null) return;
            MetricTrac.MTControls.MTGridView cgvMetric = e.Item.FindControl("cgvMetric") as MetricTrac.MTControls.MTGridView;
            string NewUrl;
            if (MyDashboardMode)  NewUrl = "ScoreCardDashboardEdit.aspx";
            else NewUrl = "ScoreCardMetricEdit.aspx?ScoreCardID=" + sc.ScoreCardID;
            cgvMetric.EmptyDataText = "No Metrics Found. Click to <a href='" + NewUrl + "'>Add New</a>";
            cgvMetric.DataSource = sc.MetricValue;
            cgvMetric.DataBind();

            if (MyDashboardMode)
            {
                if (ScoreCardMetricID == null && cgvMetric.Rows.Count>0)
                {
                    ScoreCardMetricID = (Guid)cgvMetric.DataKeys[0].Value;
                    cgvMetric.SelectedIndex = 0;
                    if (SelectedIndexChanged != null) SelectedIndexChanged(this, null);
                }

                cgvMetric.Columns[0].Visible = false;
                cgvMetric.Columns[cgvMetric.Columns.Count-1].Visible = false;

                for(int i=0;i<cgvMetric.Rows.Count;i++)
                {
                    GridViewRow r = cgvMetric.Rows[i];
                    MetricTrac.Bll.ScoreCardDashboard.Extend d = ((List<MetricTrac.Bll.ScoreCardDashboard.Extend>)sc.MetricValue)[i];
                    r.Attributes.Add("onclick", "RowClick('" + d.ScoreCardMetricID + "')");
                    r.Attributes.Add("onmouseover", "rollIn(this)");
                    r.Attributes.Add("onmouseout", "rollOut(this)");
                }
            }
            else
            {
                PlaceHolder phPublicScoreCards = (PlaceHolder)e.Item.FindControl("phPublicScoreCards");
                PlaceHolder phHideLinks = (PlaceHolder)e.Item.FindControl("phHideLinks");
                phHideLinks.Visible = (sc.UserId != LoggedinUserId);
                if (!IsPublicShown)
                    phPublicScoreCards.Visible = IsPublicShown = !IsPublicShown &&
                        ((LastUser == LoggedinUserId && LastUser != sc.UserId)
                            ||
                         (LastUser == Guid.Empty && sc.UserId != LoggedinUserId));
                PlaceHolder phViewAll = (PlaceHolder)e.Item.FindControl("phViewAll");
                phViewAll.Visible = (phPublicScoreCards.Visible && HiddenScoreCardCount > 0);

                AjaxInitialized = true;
                Telerik.Web.UI.RadAjaxManager ram = Telerik.Web.UI.RadAjaxManager.GetCurrent(this.Page);
                ram.UpdatePanelsRenderMode = UpdatePanelRenderMode.Inline;
                Panel pMetric = (Panel)e.Item.FindControl("pMetric");
                Telerik.Web.UI.RadAjaxLoadingPanel ralpMetric = e.Item.FindControl("ralpMetric") as Telerik.Web.UI.RadAjaxLoadingPanel;
                ram.AjaxSettings.AddAjaxSetting(e.Item.FindControl("lbRefresh"), pMetric, ralpMetric);
                foreach (GridViewRow r in cgvMetric.Rows)
                    ram.AjaxSettings.AddAjaxSetting(r.FindControl("ibRefresh"), pMetric, ralpMetric);
            }
            LastUser = sc.UserId;
        }
        DateTime DTNow;
        protected void Page_Load(object sender, EventArgs e)
        {
            DTNow = DateTime.Now;
            aNewCard.Visible = !MyDashboardMode;
            if (MyDashboardMode)
            {
                if (!RowSelectProcess())
                {
                    ScoreCardMetricID = null;
                }
            }
        }

        private bool RowSelectProcess()
        {
            string s = Request.Form["__EVENTTARGET"];
            if (s != btAjax.ClientID && s != btAjax.UniqueID) return false;
            s = Request.Form["__EVENTARGUMENT"];
            if (string.IsNullOrEmpty(s)) return false;
            try { ScoreCardMetricID = new Guid(s); }
            catch { return false; }

            if (SelectedIndexChanged != null) SelectedIndexChanged(this,null);
            return true;
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (!AjaxInitialized && !MyDashboardMode)
            {
                Telerik.Web.UI.RadAjaxManager ram = Telerik.Web.UI.RadAjaxManager.GetCurrent(this.Page);
                ram.UpdatePanelsRenderMode = UpdatePanelRenderMode.Inline;
                foreach (RepeaterItem ri in rScoreCard.Items)
                {
                    Panel pMetric = (Panel)ri.FindControl("pMetric");
                    Telerik.Web.UI.RadAjaxLoadingPanel ralpMetric = ri.FindControl("ralpMetric") as Telerik.Web.UI.RadAjaxLoadingPanel;
                    ram.AjaxSettings.AddAjaxSetting(ri.FindControl("lbRefresh"), pMetric, ralpMetric);
                    MetricTrac.MTControls.MTGridView cgvMetric = ri.FindControl("cgvMetric") as MetricTrac.MTControls.MTGridView;
                    foreach (GridViewRow r in cgvMetric.Rows)
                    {
                        ram.AjaxSettings.AddAjaxSetting(r.FindControl("ibRefresh"), pMetric, ralpMetric);
                    }
                }
            }

            if(MyDashboardMode && rScoreCard.Items.Count > 0)
            {
                MetricTrac.MTControls.MTGridView cgvMetric = (MetricTrac.MTControls.MTGridView) rScoreCard.Items[0].FindControl("cgvMetric");
                if (ScoreCardMetricID != null)
                {
                    cgvMetric.SelectedIndex = -1;
                    for (int i = 0; i < cgvMetric.Rows.Count; i++)
                    {
                        object o = cgvMetric.DataKeys[i].Value;
                        if (o is Guid && (Guid)o == (Guid)ScoreCardMetricID)
                        {
                            cgvMetric.SelectedIndex = i;
                            break;
                        }
                    }
                }
            }
        }

        private Guid? GetGuid(CommandEventArgs e)
        {
            if (e.CommandArgument == null) return null;
            Guid ID;
            try { ID = new Guid(e.CommandArgument.ToString()); }
            catch { return null; }
            return ID;
        }

        protected void ibRefresh_Command(object sender, CommandEventArgs e)
        {
            Guid? ScoreCardMetricID = GetGuid(e);
            if (ScoreCardMetricID == null) return;
            MetricTrac.Utils.ScoreCardCache.ProcessScoreCardMetric((Guid)ScoreCardMetricID);
        }

        protected void lbRefreshAll_Command(object sender, CommandEventArgs e)
        {
            Guid? ScoreCardID = GetGuid(e);
            if (ScoreCardID == null) return;
            MetricTrac.Utils.ScoreCardCache.ProcessScoreCard((Guid)ScoreCardID, true);
        }

        protected string GetChartUrl(object container)
        {
            if (MyDashboardMode) return " ";
            System.Web.UI.WebControls.GridViewRow ri = container as System.Web.UI.WebControls.GridViewRow;
            MetricTrac.Bll.ScoreCardMetric.Extend v = (MetricTrac.Bll.ScoreCardMetric.Extend)ri.DataItem;
            string FreqDate = DTNow.ToString("MM-dd-yyyy");
            return "DataValueChart.aspx?ScoreCardMetricID=" + v.ScoreCardMetricID  + "&Date=" + FreqDate;
        }

        protected void dsScoreCard_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (MyDashboardMode) e.Result = MetricTrac.Bll.ScoreCard.ListMyDashBoard(rdpDate.SelectedDate);
            else
            {
                List<MetricTrac.Bll.ScoreCard.Extend> l = MetricTrac.Bll.ScoreCard.List(rdpDate.SelectedDate);
                phMyScoreCards.Visible = false;
                if (l.Count > 0)
                    if (l[0].UserId != null)
                        if (l[0].UserId == LoggedinUserId)
                            phMyScoreCards.Visible = true;

                int PublicCardsCount = l.Where(s => (s.IsPublic == true && (s.UserId == null || s.UserId != LoggedinUserId))).ToList().Count;
                lbViewAllAlternative.Visible = (PublicCardsCount == 0 && HiddenScoreCardCount > 0);
                e.Result = l;
            }
        }

        protected string GetImageHtml(object ChangeValue, object GrowUpIsGood)
        {
            if (!(ChangeValue is double)) return string.Empty;
            double cv = (double)ChangeValue;
            if (cv == 0) return string.Empty;
            bool? g = (bool?)GrowUpIsGood;

            string s = (cv > 0 ? "Up" : "Down") + ((g==null)?"Gray":((cv > 0) == g) ? "Green" : "Red");
            return "<img src='../images/buttons/" + s + ".gif'  style='position:relative;top:3' />";
        }

        private MetricTrac.Bll.ScoreCard.Extend GetValue(object container)
        {
            System.Web.UI.WebControls.RepeaterItem ri = container as System.Web.UI.WebControls.RepeaterItem;
            return ri.DataItem as MetricTrac.Bll.ScoreCard.Extend;
        }
        protected string GetScoreCardUser(object container)
        {
            MetricTrac.Bll.ScoreCard.Extend val = GetValue(container);
            string UserStr = String.Empty;
            if (val.UserId != null)
                if (!String.IsNullOrEmpty(val.CreateUserName) && val.UserId != LoggedinUserId)
                    UserStr = ",&nbsp;&nbsp;&nbsp;" + val.CreateUserName;
            return UserStr;
        }
        public Unit Width
        {
            get
            {
                object o = ViewState["ScoreCardWidth"];
                if (!(o is Unit))
                {
                    Unit u = new Unit("888px");
                    ViewState["ScoreCardWidth"] = u;
                    return u;
                }
                return (Unit)o;
            }
            set
            {
                ViewState["ScoreCardWidth"] = value;
            }

        }

        protected void lbHide_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "Hide")
            {
                Guid CardId = Guid.Empty;
                try
                {
                    CardId = new Guid(e.CommandArgument.ToString());
                }
                catch { CardId = Guid.Empty; }
                if (CardId != Guid.Empty)
                {
                    MetricTrac.Bll.ScoreCard.HideScoreCard(CardId);
                    Response.Redirect(Request.Url.ToString());
                }
            }   
        }

        protected void lbHideAll_Command(object sender, CommandEventArgs e)
        {
            MetricTrac.Bll.ScoreCard.HideAllPublicScoreCards();
            Response.Redirect(Request.Url.ToString());
        }

        protected void lbViewAll_Command(object sender, CommandEventArgs e)
        {
            MetricTrac.Bll.ScoreCard.ViewAllScoreCards();
            Response.Redirect(Request.Url.ToString());
        }
    }
}
