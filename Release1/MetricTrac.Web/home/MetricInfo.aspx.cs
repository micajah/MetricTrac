using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;
using Micajah.FileService.WebControls;
using Telerik.Web.UI;

namespace MetricTrac
{
    public partial class MetricInfo : System.Web.UI.Page
    {
        private Guid mMetricID;
        protected Guid MetricID
        {
            get
            {
                if (mMetricID != Guid.Empty) return mMetricID;
                string strMetricID = Request.QueryString["MetricID"];
                if (string.IsNullOrEmpty(strMetricID)) return mMetricID;
                try
                {
                    mMetricID = new Guid(strMetricID);
                }
                catch { }
                return mMetricID;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleApplicationLogo = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleBreadcrumbs = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleFooter = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleFooterLinks = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeader = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeaderLinks = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleLeftArea = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleMainMenu = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleHeaderLogo = false;
            ((Micajah.Common.Pages.MasterPage)Page.Master).VisibleSearchControl = false;
            Bll.Metric.Extend m = Bll.Metric.Get(MetricID);
            lblName.Text = String.IsNullOrEmpty(m.Name) ? "---" : m.Name;
            lblCode.Text = String.IsNullOrEmpty(m.Code) ? "---" : m.Code;
            lblAlias.Text = String.IsNullOrEmpty(m.Alias) ? "---" : m.Alias;
            lblCategory.Text = String.IsNullOrEmpty(m.MetricCategoryName) ? "---" : m.MetricCategoryName;
            lblMetricType.Text = String.IsNullOrEmpty(m.MetricTypeName) ? "---" : m.MetricTypeName;
            lblDataType.Text = String.IsNullOrEmpty(m.MetricDataTypeName) ? "---" : m.MetricDataTypeName;
            lblUoM.Text = String.IsNullOrEmpty(m.UnitOfMeasureName) ? "---" : m.UnitOfMeasureName;
            lblInputUoM.Text = String.IsNullOrEmpty(m.InputUnitOfMeasureName) ? "---" : m.InputUnitOfMeasureName;
            lblDecPlaces.Text = (m.NODecPlaces == null) ? "---" : m.NODecPlaces.ToString();
            lblMinValue.Text = (m.NOMinValue == null) ? "---" : m.NOMinValue.ToString();
            lblMaxValue.Text = (m.NOMaxValue == null) ? "---" : m.NOMaxValue.ToString();
            string formula = Bll.Metric.GetMetricFormula(MetricID).Formula;
            lblFormula.Text = String.IsNullOrEmpty(formula) ? "---" : formula;
            lblFrequency.Text = String.IsNullOrEmpty(m.FrequencyName) ? "---" : m.FrequencyName;
            lblDescription.Text = String.IsNullOrEmpty(m.Notes) ? "---" : m.Notes;
            lblDefinition.Text = String.IsNullOrEmpty(m.Definition) ? "---" : m.Definition;
            lblDocumentation.Text = String.IsNullOrEmpty(m.Documentation) ? "---" : m.Documentation;
            lblReferences.Text = String.IsNullOrEmpty(m.MetricReferences) ? "---" : m.MetricReferences;
            
            if (m.MetricTypeID == 1)
            {
                rowDataType.Visible = true;
                if (m.MetricDataTypeID == 1)
                {
                    rowUoM.Visible = true;
                    rowInputUoM.Visible = true;
                    rowDecPlaces.Visible = true;
                    rowMinValue.Visible = true;
                    rowMaxValue.Visible = true;
                }
                else
                {
                    rowUoM.Visible = false;
                    rowInputUoM.Visible = false;
                    rowDecPlaces.Visible = false;
                    rowMinValue.Visible = false;
                    rowMaxValue.Visible = false;
                }
                rowFormula.Visible = false;
            }
            else
            {
                rowDataType.Visible = false;
                rowUoM.Visible = true;
                rowInputUoM.Visible = false;
                rowDecPlaces.Visible = false;
                rowMinValue.Visible = false;
                rowMaxValue.Visible = false;                
                rowFormula.Visible = true;
            }
        }
    }
}
