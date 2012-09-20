using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace MetricTrac.Control
{
    public partial class PIFormulaBuilder : System.Web.UI.UserControl
    {
        public string Expression 
        { 
            get
            {
                return reExpression.Content;
            }
            set 
            {
                reExpression.Content = value;
            }
        }        

        public string InputClientID
        {
            get
            {
                return reExpression.ClientID;
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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var ml = Bll.Metric.ListNumeric(PIID);
                cbMetric.DataSource = ml;
                cbMetric.DataBind();
            }
        }

        List<Bll.MetricCategory> mc;
        protected void cbMetric_ItemDataBound(object sender, RadComboBoxItemEventArgs e)
        {
            if (mc == null) mc = Bll.MetricCategory.SelectAll().ToList();
            Bll.Metric.Extend m = (Bll.Metric.Extend)e.Item.DataItem;
            Guid? mcID = m.MetricCategoryID;
            string CategoryListStr = null;
            while (mcID != null)
            {
                if (CategoryListStr != null) CategoryListStr += ",";
                CategoryListStr += mcID.ToString();

                Bll.MetricCategory NextMc = mc.Where(c => c.MetricCategoryID == mcID).FirstOrDefault();
                mcID = null;
                if (NextMc.MetricCategoryID!=Guid.Empty && NextMc.ParentId!=Guid.Empty)
                {
                    mcID = NextMc.ParentId;
                }
            }
            if (CategoryListStr != null) e.Item.Attributes.Add("MetricCateroryID", CategoryListStr);
        }
    }
}