using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MetricTrac
{
    public partial class PerformanceIndicatorFormulaEdit : System.Web.UI.Page
    {
        string mFormulaParameters;
        string FormulaParameters
        {
            get
            {
                /*if (mFormulaParameters != null) return mFormulaParameters == string.Empty ? null : mFormulaParameters;
                mFormulaParameters = string.Empty;*/

                HttpCookie hc = Request.Cookies.Get("FormulaParameters");
                if (hc != null)
                    mFormulaParameters = Microsoft.JScript.GlobalObject.unescape(hc.Value);
                
                //mFormulaParameters = Request.QueryString["fp"];
                return mFormulaParameters;
            }
        }
        string[] mFormulaParametersArray;
        string[] FormulaParametersArray
        {
            get
            {
                if (mFormulaParametersArray != null) return mFormulaParametersArray;
                mFormulaParametersArray = new string[3];
                if (!string.IsNullOrEmpty(FormulaParameters))
                {
                    string[] ss = FormulaParameters.Split('&');
                    if (ss != null)
                    {
                        if (ss.Length >= 0) mFormulaParametersArray[0] = System.Web.HttpUtility.UrlDecode(ss[0]);
                        if (ss.Length >= 1) mFormulaParametersArray[1] = System.Web.HttpUtility.UrlDecode(ss[1]);
                        if (ss.Length >= 2) mFormulaParametersArray[2] = System.Web.HttpUtility.UrlDecode(ss[2]);
                    }
                }
                return mFormulaParametersArray;
            }
        }

        string mFormulaTxt;
        string FormulaTxt
        {
            get
            {
                if (mFormulaTxt != null) return mFormulaTxt == string.Empty ? null : mFormulaTxt;
                mFormulaTxt = string.Empty;
                mFormulaTxt = Request.QueryString["txt"];
                return mFormulaTxt;
            }
        }

        DateTime mBeginDate;
        DateTime BeginDate
        {
            get
            {
                if (mBeginDate != DateTime.MinValue) return mBeginDate;
                mBeginDate = DateTime.Today;
                string s = FormulaParametersArray[0];
                if (string.IsNullOrEmpty(s)) return mBeginDate;
                DateTime dt;
                if (!DateTime.TryParse(s, out dt)) return mBeginDate;
                mBeginDate = dt;
                return mBeginDate;
            }
        }

        DateTime? mEndDate;
        bool EndDateParsed;
        DateTime? EndDate
        {
            get
            {
                if (EndDateParsed) return mEndDate;
                EndDateParsed = true;
                mEndDate = null;
                string s = FormulaParametersArray[1];
                if (string.IsNullOrEmpty(s)) return mEndDate;
                DateTime dt;
                if (!DateTime.TryParse(s, out dt)) return mEndDate;
                mEndDate = dt;
                return mEndDate;
            }
        }

        string mFormulaComment;
        bool mFormulaCommentParsed;
        string FormulaComment
        {
            get
            {
                if (mFormulaCommentParsed) return mFormulaComment;
                mFormulaCommentParsed = true;
                mFormulaComment = FormulaParametersArray[2];
                return mFormulaComment;
            }
        }

        Guid mPIID;
        bool IsPIID;
        protected Guid PIID
        {
            get
            {
                if (IsPIID) return mPIID;
                IsPIID = true;
                mPIID = Guid.Empty;

                string s = Request.QueryString["PIID"];
                if (string.IsNullOrEmpty(s)) return mPIID;
                try
                {
                    mPIID = new Guid(s);
                }
                catch { }
                return mPIID;
            }
        }

        private string ErrorMessage
        {
            set { ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage = value; }
            get { return ((Micajah.Common.Pages.MasterPage)this.Master).ErrorMessage; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MetricTrac.MasterPage mp = ((MetricTrac.MasterPage)Page.Master);
            mp.IncludeJqueryUi = true;
            mp.VisibleApplicationLogo = false;
            mp.VisibleBreadcrumbs = false;
            mp.VisibleFooter = false;
            mp.VisibleFooterLinks = false;
            mp.VisibleHeader = false;
            mp.VisibleHeaderLinks = false;
            mp.VisibleLeftArea = false;
            mp.VisibleMainMenu = false;
            mp.VisibleHeaderLogo = false;
            mp.VisibleSearchControl = false;

            rdpBeginDate.DateInput.DateFormat = rdpBeginDate.Culture.DateTimeFormat.ShortDatePattern;
            rdpEndDate.DateInput.DateFormat = rdpEndDate.Culture.DateTimeFormat.ShortDatePattern;
            if (!IsPostBack)
            {
                rdpBeginDate.SelectedDate = BeginDate;
                rdpEndDate.SelectedDate = EndDate;
                txtComment.Value = FormulaComment;

                IsSimpleFormula = false;
                if(!string.IsNullOrEmpty(FormulaTxt))
                {
                    string ft = FormulaTxt.Trim().ToLower();
                    foreach (ListItem it in ddlSimpleFormula.Items)
                    {
                        if (ft == it.Value.ToLower())
                        {
                            IsSimpleFormula = true;
                            it.Selected = true;
                            break;
                        }
                    }
                }
                if(!IsSimpleFormula) fbExpression.Expression = FormulaTxt;
                rbSimpleFormula.Checked = IsSimpleFormula;
                rbCustomFormula.Checked = !IsSimpleFormula;
            }
        }

        protected void cgvFormula_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;
            Bll.PerformanceIndicatorFormula.Extend rData = (Bll.PerformanceIndicatorFormula.Extend)e.Row.DataItem;
            e.Row.Cells[0].Text = rData.BeginDate.ToShortDateString();
            if (rData.EndDate != null)
                e.Row.Cells[1].Text = ((DateTime)rData.EndDate).ToShortDateString();
            else e.Row.Cells[1].Text = "Non given";
            e.Row.Cells[4].Text = rData.ChangeDate.ToShortDateString();
        }

        protected void ldsPIFormula_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if (PIID != Guid.Empty)
            {
                e.Result = Bll.PerformanceIndicatorFormula.GetFormulaHistory(PIID);
            }
        }

        protected bool IsSimpleFormula{get;set;}
    }
}
