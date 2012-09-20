using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Micajah.Common.WebControls;

namespace MetricTrac
{
    public partial class PerformanceIndicatorEdit : System.Web.UI.Page
    {
        private Guid? mPerformanceIndicatorID;
        protected Guid PerformanceIndicatorID
        {
            get
            {
                if (mPerformanceIndicatorID != null) return (Guid)mPerformanceIndicatorID;
                mPerformanceIndicatorID = Guid.Empty;
                string strPerformanceIndicatorID = HttpContext.Current.Request.QueryString["PerformanceIndicatorID"];
                if (string.IsNullOrEmpty(strPerformanceIndicatorID)) return Guid.Empty;
                try
                {
                    mPerformanceIndicatorID = new Guid(strPerformanceIndicatorID);
                }
                catch { };
                return (Guid)mPerformanceIndicatorID;
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            Micajah.Common.Pages.MasterPage mp = Page.Master as Micajah.Common.Pages.MasterPage;
            rapMetric.LoadingPanelID = ((MetricTrac.MasterPage)mp).ralpLoadingPanel1ID;
            base.OnPreInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(!IsPostBack)
            {
                if (PerformanceIndicatorID != Guid.Empty)
                {
                    mfPerformanceIndicator.ChangeMode(DetailsViewMode.Edit);
                }
                if (PerformanceIndicatorID != Guid.Empty)
                {
                    Micajah.Common.Security.UserContext user = Micajah.Common.Security.UserContext.Current;
                    OrgTree.CustomRootNodeText = user.SelectedOrganization.Name;
                    OrgTree.EntityNodeId = PerformanceIndicatorID;
                    OrgTree.LoadTree();
                }
                else
                    OrgTree.Visible = false;
            }
        }

        MetricTrac.Bll.PerformanceIndicator.Extend GetPI()
        {
            MetricTrac.Bll.PerformanceIndicator.Extend pi = null;
            if (PerformanceIndicatorID != Guid.Empty) pi = MetricTrac.Bll.PerformanceIndicator.Get(PerformanceIndicatorID);
            return pi;
        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (rapMetric.IsAjaxRequest) cMetricList.Rebind();
        }

        private void RedirectToGrid()
        {
            Response.Redirect("PerformanceIndicatorList.aspx");
        }

        protected void mfPerformanceIndicator_Action(object sender, Micajah.Common.WebControls.MagicFormActionEventArgs e)
        {
            if (e.Action == Micajah.Common.WebControls.CommandActions.Cancel) RedirectToGrid();
        }

        private string GetSortCode(string Code)
        {
            int CastLength = 25;
            string result = String.Empty;
            if (!String.IsNullOrEmpty(Code))
            {
                int i = Code.Length - 1;
                while (Code[i] >= '0' && Code[i] <= '9' && i >= 0)
                {
                    i--;
                    if (i < 0) break;
                }
                if (i < Code.Length - 1)
                {
                    int InsertLength = CastLength - Code.Length;
                    result = Code.Substring(0, i + 1);
                    for (int k = 1; k <= InsertLength; k++)
                        result += "0";
                    result += Code.Substring(i + 1, Code.Length - i - 1);
                }
            }
            return result;
        }

        protected string GetDateString(object d)
        {
            if (!(d is DateTime)) return "Non given";
            return ((DateTime)d).ToShortDateString();
        }

        public MetricTrac.Bll.PerformanceIndicator.Extend _SelectPI()
        {
            MetricTrac.Bll.PerformanceIndicator.Extend pi = GetPI();
            string p = string.Empty;
            if (pi!=null && pi.FormulaBeginDate != null) p += System.Web.HttpUtility.UrlEncode(((DateTime)pi.FormulaBeginDate).ToShortDateString());
            p += "&";
            if (pi != null && pi.FormulaEndDate != null) p += System.Web.HttpUtility.UrlEncode(((DateTime)pi.FormulaEndDate).ToShortDateString());
            p += "&";
            if (pi != null && pi.FormulaComment != null) p += System.Web.HttpUtility.UrlEncode(pi.FormulaComment);
            hfFormulaParameters.Value = p;
            return pi;
        }
        public void _SavePI(MetricTrac.Bll.PerformanceIndicator.Extend pi)
        {
            if (!string.IsNullOrEmpty(hfFormulaParameters.Value))
            {
                DateTime dt;
                string[] ss = hfFormulaParameters.Value.Split('&');
                if (ss.Length > 0 && !string.IsNullOrEmpty(ss[0]))
                {
                    string s = System.Web.HttpUtility.UrlDecode(ss[0]);
                    if (DateTime.TryParse(s, out dt)) pi.FormulaBeginDate = dt;
                }
                if (ss.Length > 1 && !string.IsNullOrEmpty(ss[1]))
                {
                    string s = System.Web.HttpUtility.UrlDecode(ss[1]);
                    if (DateTime.TryParse(s, out dt)) pi.FormulaEndDate = dt;
                }
                if (ss.Length > 2 && !string.IsNullOrEmpty(ss[2]))
                {
                    pi.FormulaComment = System.Web.HttpUtility.UrlDecode(ss[2]);
                }
            }

            pi.Formula = Request.Form[mfPerformanceIndicator.UniqueID + "$heExpression$txt"];

            bool InserMode = pi.PerformanceIndicatorID == Guid.Empty;
            pi.SortCode = GetSortCode(pi.Code);
            MetricTrac.Bll.PerformanceIndicator.Save(pi);
            if (InserMode)
            {
                cMetricList.SaveInsert(pi.PerformanceIndicatorID);
            }
            if(PerformanceIndicatorID != Guid.Empty) OrgTree.SaveTree();
            RedirectToGrid();
        }
        void _DeletePI(MetricTrac.Bll.PerformanceIndicator.Extend pi)
        {
            MetricTrac.Bll.PerformanceIndicator.Delete(pi.PerformanceIndicatorID);
            RedirectToGrid();
        }

        protected void ldsUnitOfMeasure_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = Bll.Mc_UnitsOfMeasure.GetOrganizationUoMs(); ;
        }

        static MetricTrac.PerformanceIndicatorEdit CurrentPage { get {return ((MetricTrac.PerformanceIndicatorEdit)(HttpContext.Current.CurrentHandler));} }
        public static MetricTrac.Bll.PerformanceIndicator.Extend SelectPI(){return CurrentPage._SelectPI();}
        public static void SavePI(MetricTrac.Bll.PerformanceIndicator.Extend pi) { CurrentPage._SavePI(pi); }
        public static void DeletePI(MetricTrac.Bll.PerformanceIndicator.Extend pi) { CurrentPage._DeletePI(pi); }
    }
}
