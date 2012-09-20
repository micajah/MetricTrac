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
        static string DateFormatString = "MMM dd yyyy";

        public bool MyDashboardMode { get; set; }
        public Guid? ScoreCardMetricID
        {
            get { return (Guid?)ViewState["ScoreCardMetricID"]; }
            set { ViewState["ScoreCardMetricID"] = value; }
        }
        public Guid? DasboardID
        {
            get { return (Guid?)ViewState["DasboardID"]; }
            set { ViewState["DasboardID"] = value; }
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

        void SetPeriodName(MetricTrac.MTControls.MTGridView cgvMetric, DropDownList ddl, string DataField, int CustomSuffix )
        {
            string PeriodName = null;
            if (CustomSuffix == 2 && !isCompDateExist)
            {
                PeriodName = string.Empty;
            }
            else
            {
                if (ddl.SelectedIndex >= 0) PeriodName = ddl.SelectedItem.Text;
                if (string.IsNullOrEmpty(PeriodName)) PeriodName = "Custom";
                if (PeriodName.ToLower().Trim() == "custom") PeriodName += " " + CustomSuffix;
            }
            if (cgvMetric.HeaderRow != null)
            {
                var c = GetColumn(cgvMetric, DataField);
                int n = cgvMetric.Columns.IndexOf(c);
                TableCell ts = cgvMetric.HeaderRow.Cells[n];
                ts.Text = PeriodName;
            }
        }

        DataControlField GetColumn(MetricTrac.MTControls.MTGridView cgvMetric, string DataFildOrHeaderText)
        {
            foreach (DataControlField c in cgvMetric.Columns)
            {
                if (c.HeaderText == DataFildOrHeaderText) return c;
                if (!(c is BoundField)) continue;
                if (((BoundField)c).DataField == DataFildOrHeaderText) return c;
            }
            return null;
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

            

            GetColumn(cgvMetric, "Period").Visible = MyDashboardMode;

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
                    r.Attributes.Add("onclick", "RowClick('" + d.ScoreCardMetricID+","+d.ScoreCardDashboardID + "')");
                    r.Attributes.Add("onmouseover", "rollIn(this)");
                    r.Attributes.Add("onmouseout", "rollOut(this)");
                }
            }
            else
            {
                SetPeriodName(cgvMetric, ddlDatePeriod, "CurrentValueStr", 1);
                SetPeriodName(cgvMetric, ddlCompDatePeriod, "PreviousValueStr", 2);

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
                //Panel pMetric = (Panel)e.Item.FindControl("pMetric");
                //Telerik.Web.UI.RadAjaxLoadingPanel ralpMetric = e.Item.FindControl("ralpMetric") as Telerik.Web.UI.RadAjaxLoadingPanel;
                //ram.AjaxSettings.AddAjaxSetting(e.Item.FindControl("lbRefresh"), pMetric, ralpMetric);
                //foreach (GridViewRow r in cgvMetric.Rows)
                //    ram.AjaxSettings.AddAjaxSetting(r.FindControl("ibRefresh"), pMetric, ralpMetric);
            }
            LastUser = sc.UserId;
        }

        void InitListItem(DropDownList ddl, string val, DateTime begin, DateTime End)
        {
            ListItem li = ddl.Items.FindByValue(val);
            if (li == null) return;
            if (li.Attributes["DateBegin"] == null) li.Attributes.Add("DateBegin", begin.ToString(/*"yyyy-MM-dd"*/DateFormatString));
            if (li.Attributes["DateEnd"] == null) li.Attributes.Add("DateEnd", End.ToString(DateFormatString));
        }

        void InitListItem(DropDownList ddl, string ItemIdSufix, int FrequencyID, DateTime DtToday)
        {
            DateTime BeginPeriod = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID, DtToday);
            DateTime EndPeriod = MetricTrac.Bll.Frequency.AddPeriod(BeginPeriod, FrequencyID, 1);
            DateTime PrevPeriod = MetricTrac.Bll.Frequency.AddPeriod(BeginPeriod, FrequencyID, -1);
            InitListItem(ddl, "t_" + ItemIdSufix, BeginPeriod, EndPeriod.AddDays(-1));
            InitListItem(ddl, "t_" + ItemIdSufix + "_d", BeginPeriod, DtToday);
            InitListItem(ddl, "l_" + ItemIdSufix, PrevPeriod, BeginPeriod.AddDays(-1));
            InitListItem(ddl, "l_" + ItemIdSufix + "_d", PrevPeriod, DtToday);
        }
        void InitPeriodDdl(DropDownList ddl, DateTime DtToday)
        {
            //ddl.Attributes.Add("", "");            
            InitListItem(ddl,"d",1,DtToday);
            InitListItem(ddl,"w",2,DtToday);
            InitListItem(ddl,"m",3,DtToday);
            InitListItem(ddl,"q",4,DtToday);
            InitListItem(ddl,"fq",8,DtToday);
            InitListItem(ddl,"y",6,DtToday);
            InitListItem(ddl,"fy",10,DtToday);
        }
        void InitDateRange()
        {
            DateTime DtToday = DateTime.Today;
            InitPeriodDdl(ddlDatePeriod, DtToday);
            InitPeriodDdl(ddlCompDatePeriod, DtToday);
            if (!IsPostBack)
            {
                ReadDbCompPeriod(DtToday);
            }
            else
            {
                trComparePeriod.Style[HtmlTextWriterStyle.Visibility] = hfComparePeriod.Value;
                AddCompPeriodHref.Style[HtmlTextWriterStyle.Visibility] = hfComparePeriod.Value == "hidden" ? "visible" : "hidden";
            }
        }
        

        DateTime DTNow;
        protected void Page_Load(object sender, EventArgs e)
        {
            MetricTrac.MasterPage mp = Page.Master as MetricTrac.MasterPage;
            mp.IncludeJqueryUi = true;

            InitDateRange();           


            DTNow = DateTime.Now;
            aNewCard.Visible = !MyDashboardMode;
            phSelectPeriod.Visible = !MyDashboardMode;
            apMain.EnableAJAX = !MyDashboardMode;

            if (MyDashboardMode)
            {
                if (!RowSelectProcess())
                {
                    ScoreCardMetricID = null;
                }
            }
            else
            {
            }
        }

        private bool RowSelectProcess()
        {
            string s = Request.Form["__EVENTTARGET"];
            if (s != btAjax.ClientID && s != btAjax.UniqueID) return false;
            s = Request.Form["__EVENTARGUMENT"];
            if (string.IsNullOrEmpty(s)) return false;

            string s2=null;
            if (s.Contains(","))
            {
                string[] ss = s.Split(',');
                s = ss[0];
                s2 = ss[1];
            }
            try {
                if (!string.IsNullOrEmpty(s2)) DasboardID = new Guid(s2); 
                ScoreCardMetricID = new Guid(s); 
            }
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
            //MetricTrac.Utils.ScoreCardCache.ProcessScoreCardMetric((Guid)ScoreCardMetricID);
        }

        protected void lbRefreshAll_Command(object sender, CommandEventArgs e)
        {
            Guid? ScoreCardID = GetGuid(e);
            if (ScoreCardID == null) return;
            //MetricTrac.Utils.ScoreCardCache.ProcessScoreCard((Guid)ScoreCardID, true);
        }

        protected string GetChartUrl(object container)
        {
            if (MyDashboardMode) return " ";
            System.Web.UI.WebControls.GridViewRow ri = container as System.Web.UI.WebControls.GridViewRow;
            MetricTrac.Bll.ScoreCardMetric.Extend v = (MetricTrac.Bll.ScoreCardMetric.Extend)ri.DataItem;
            string FreqDate = DTNow.ToString("MM-dd-yyyy");
            return "DataValueChart.aspx?ScoreCardMetricID=" + v.ScoreCardMetricID  + "&Date=" + FreqDate;
        }
        bool isCompDateExist
        {
            get
            {
                return !string.IsNullOrEmpty(hfComparePeriod.Value) && hfComparePeriod.Value != "hidden" && hfComparePeriod.Value != "none";
            }
        }
        void ResetCompPeriod()
        {
            Period = null;
            CompPeriod = null;
            dtBeginDate = null;
            dtEndDate = null;
            dtBeginCompDate = null;
            dtEndCompDate = null;
        }
        void GetRequestCompPeriod()
        {
            ResetCompPeriod();

            Period = ddlDatePeriod.SelectedValue;
            if (Period == "" || Period == "c") Period = null;

            if (dpDateBegin.SelectedDate != null && dpDateBegin.SelectedDate != DateTime.MinValue && dpDateEnd.SelectedDate != null && dpDateEnd.SelectedDate != DateTime.MinValue)
            {
                dtBeginDate = dpDateBegin.SelectedDate;
                dtEndDate = dpDateEnd.SelectedDate;
            }

            if (isCompDateExist)
            {
                CompPeriod = ddlCompDatePeriod.SelectedValue;
                if (CompPeriod == "" || CompPeriod == "c") CompPeriod = null;

                if (dpCompDateBegin.SelectedDate != null && dpCompDateBegin.SelectedDate != DateTime.MinValue && dpCompDateEnd.SelectedDate != null && dpCompDateEnd.SelectedDate != DateTime.MinValue)
                {
                    dtBeginCompDate = dpCompDateBegin.SelectedDate;
                    dtEndCompDate = dpCompDateEnd.SelectedDate;
                }
            }
        }

        void GetPeriodRange(string PeriodValue, out DateTime? Begin, out DateTime? End)
        {
            Begin = null;
            End = null;
            DateTime dt;
            var li = ddlDatePeriod.Items.FindByValue(PeriodValue);
            if (li == null) return;
            string attr = li.Attributes["DateBegin"];
            if(!string.IsNullOrEmpty(attr))
            {
                if(DateTime.TryParseExact(attr,DateFormatString, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat,System.Globalization.DateTimeStyles.None,out dt))
                {
                    Begin=dt;
                }
            }
            
            attr = li.Attributes["DateEnd"];
            if(!string.IsNullOrEmpty(attr))
            {
                if(DateTime.TryParseExact(attr,DateFormatString, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat,System.Globalization.DateTimeStyles.None,out dt))
                {
                    End=dt;
                }
            }
        }

        void ReadDbCompPeriod(DateTime DtToday)
        {
            ResetCompPeriod();
            var p = Bll.ScoreCardCompPeriod.GetPeriod();
            if(p==null || p.PeriodType1==null)
            {
                Period = "t_m";
                dtBeginDate = MetricTrac.Bll.Frequency.GetNormalizedDate(3, DtToday);
                dtEndDate = dtBeginDate.Value.AddMonths(1).AddDays(-1);
            }
            else
            {
                DateTime? dt1;
                DateTime? dt2;

                if(p.PeriodType1==0)
                {
                    Period = "c";
                    dtBeginDate =p.Begin1;
                    dtEndDate = p.End1;
                }
                else
                {
                    Period = p.PeriodValue;
                    GetPeriodRange(Period, out dt1, out dt2);
                    dtBeginDate =dt1;
                    dtEndDate = dt2;
                }

                if(p.PeriodType2!=null)
                {            
                    if(p.PeriodType2==0)
                    {
                        CompPeriod = "c";
                        dtBeginCompDate =p.Begin2;
                        dtEndCompDate = p.End2;
                    }
                    else
                    {
                        CompPeriod = p.CompPeriodValue;
                        GetPeriodRange(CompPeriod, out dt1, out dt2);
                        dtBeginCompDate =dt1;
                        dtEndCompDate = dt2;
                    }
                }
            }


            hfComparePeriod.Value = trComparePeriod.Style[HtmlTextWriterStyle.Visibility] = CompPeriod==null ? "hidden" : "visible";
            if (Period != null) ddlDatePeriod.SelectedValue = Period;
            if (CompPeriod != null) ddlCompDatePeriod.SelectedValue = CompPeriod;
            if (dtBeginDate != null) dpDateBegin.SelectedDate = (DateTime)dtBeginDate;
            if (dtEndDate != null) dpDateEnd.SelectedDate = (DateTime)dtEndDate;
            if (dtBeginCompDate != null) dpCompDateBegin.SelectedDate = (DateTime)dtBeginCompDate;
            if (dtEndCompDate != null) dpCompDateEnd.SelectedDate = (DateTime)dtEndCompDate;
        }

        DateTime? dtBeginDate;
        DateTime? dtEndDate;
        DateTime? dtBeginCompDate;
        DateTime? dtEndCompDate;
        string Period;
        string CompPeriod;
        protected void dsScoreCard_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (MyDashboardMode)
            {
                phMyScoreCards.Visible = false;
                e.Result = MetricTrac.Bll.ScoreCard.ListMyDashBoard(null, MetricTrac.PerformanceIndicatorCalc.CalcStringFormula);
            }
            else
            {
                GetRequestCompPeriod();

                DateTime? NextDate = dtEndDate == null ? (DateTime?)null : ((DateTime)dtEndDate).AddDays(1);
                DateTime? NextCompDate = dtEndCompDate == null ? (DateTime?)null : ((DateTime)dtEndCompDate).AddDays(1);
                List<MetricTrac.Bll.ScoreCard.Extend> l = MetricTrac.Bll.ScoreCard.List(dtBeginDate, NextDate, dtBeginCompDate, NextCompDate, MetricTrac.PerformanceIndicatorCalc.CalcStringFormula);
                phMyScoreCards.Visible = false;
                if (!MyDashboardMode && l.Count > 0 && l[0].UserId != null && l[0].UserId == LoggedinUserId) phMyScoreCards.Visible = true;

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

        protected void bApplyPeriod_Click(object sender, EventArgs e)
        {
            rScoreCard.DataBind();
            if (!MyDashboardMode)
            {
                Bll.ScoreCardCompPeriod.SavePeriod(Period, CompPeriod, dtBeginDate, dtEndDate, dtBeginCompDate, dtEndCompDate);
            }
        }
    }
}
