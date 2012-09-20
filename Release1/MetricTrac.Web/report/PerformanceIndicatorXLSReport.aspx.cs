using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;
using Syncfusion.XlsIO;
using PIMetricInfo = MetricTrac.Bll.Sp_SelectPIReportValuesResult1;

namespace MetricTrac
{
    public partial class PerformanceIndicatorXLSReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {                
                DateTime EndDate = DateTime.Now;
                DateTime BeginDate = new DateTime(EndDate.Year, EndDate.Month, 1);
                rdpBeginDate.SelectedDate = BeginDate;
                rdpEndDate.SelectedDate = EndDate;
            }            
        }

        private string GetCellRange(char y, int x)
        {
            return y + x.ToString();
        }

        private string GetCellRange(char y1, int x1, char y2, int x2)
        {
            return y1 + x1.ToString() + ":" + y2 + x2.ToString();
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            Guid? OrgLocationTypeID = null;
            char y = 'A';
            if (ddlLocationType.SelectedValue != String.Empty)
            {
                OrgLocationTypeID = new Guid(ddlLocationType.SelectedValue);
                y = 'B';
            }
            IList<PIMetricInfo> MetricInfo = Bll.PerformanceIndicator.GetPIReportValues(sOrgLocation.OrgLocationID, new Guid(ddlGroup.SelectedValue), int.Parse(ddlFrequency.SelectedValue), OrgLocationTypeID, (DateTime)rdpBeginDate.SelectedDate, (DateTime)rdpEndDate.SelectedDate);
            ExcelEngine excelEngine = new ExcelEngine();
            IApplication application = excelEngine.Excel;
            IWorkbook workbook = application.Workbooks.Create(1);
            workbook.Version = ExcelVersion.Excel2007;
            IWorksheet sheet = workbook.Worksheets[0];            

            int x = 1;
            // 1 row            
            sheet.Range[GetCellRange(y, x)].Text = "Report:";
            sheet.Range[GetCellRange(y, x)].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
            sheet.Range[GetCellRange((char)(y + 1), x)].Text = txtTitle.Text;
            sheet.Range[GetCellRange((char)(y + 1), x, (char)(y + 24), x)].Merge(false);

            x++;
            // 2 row
            sheet.Range[GetCellRange(y, x)].Text = "Group:"; sheet.Range["A2"].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
            sheet.Range[GetCellRange((char)(y + 1), x)].Text = ddlGroup.SelectedItem.Text;
            sheet.Range[GetCellRange((char)(y + 2), x)].Text = "Period:";
            sheet.Range[GetCellRange((char)(y + 2), x)].CellStyle.HorizontalAlignment = ExcelHAlign.HAlignRight;
            sheet.Range[GetCellRange((char)(y + 3), x)].Text = ((DateTime)rdpBeginDate.SelectedDate).ToShortDateString() + " - " + ((DateTime)rdpEndDate.SelectedDate).ToShortDateString();

            x++;
            // 3 row
            sheet.Range[GetCellRange((char)(y + 1), x)].Text = "Org Location:";
            sheet.Range[GetCellRange((char)(y + 2), x)].Text = sOrgLocation.OrgLocationFullName;
            sheet.Range[GetCellRange((char)(y + 2), x, (char)(y + 24), x)].Merge(false);

            x = x + 2;
            // 5 row
            if (OrgLocationTypeID != null)
            {
                sheet.Range[GetCellRange((char)(y - 1), x)].Text = "Location";
                sheet.Range[GetCellRange((char)(y - 1), x)].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;
            }
            sheet.Range[GetCellRange(y, x)].Text = "Category";
            sheet.Range[GetCellRange((char)(y + 1), x)].Text = "Aspect";
            sheet.Range[GetCellRange((char)(y + 2), x)].Text = "Code";
            sheet.Range[GetCellRange((char)(y + 3), x)].Text = "Name";
            sheet.Range[GetCellRange((char)(y + 4), x)].Text = "Value";
            sheet.Range[GetCellRange((char)(y + 5), x)].Text = "Unit Of Measure";

            string cLocationName = String.Empty;
            Guid? cCategory = Guid.Empty;
            Guid? cAspect = Guid.Empty;
            Guid cPI = Guid.Empty;
            x++;            
            for (int i = 0; i < MetricInfo.Count; i++)
            {
                string rLocationName = String.Empty;
                Guid? rCategory = null;
                Guid? rAspect = null;                
                string rCategoryName = String.Empty;
                string rAspectName = String.Empty;

                
                if (!String.IsNullOrEmpty(MetricInfo[i].LocationFullName))
                    rLocationName = MetricInfo[i].LocationFullName;

                if (rLocationName != cLocationName && OrgLocationTypeID != null)
                {
                    x++;
                    sheet.Range[GetCellRange((char)(y - 1), x)].Text = rLocationName;
                    sheet.Range[GetCellRange((char)(y - 1), x)].CellStyle.Font.Underline = ExcelUnderline.Single;
                    sheet.Range[GetCellRange((char)(y - 1), x, (char)(y + 24), x)].Merge(false);
                    x++;
                }


                if ((MetricInfo[i].GCA1ID != null) && (MetricInfo[i].GCA2ID != null))
                {
                    rCategory = MetricInfo[i].GCA1ID;
                    rCategoryName = MetricInfo[i].GCA1Name;
                    rAspect = MetricInfo[i].GCAID;
                    rAspectName = MetricInfo[i].GCAName;
                }
                else
                    if ((MetricInfo[i].GCA1ID != null) && (MetricInfo[i].GCA2ID == null))
                    {
                        rCategory = MetricInfo[i].GCAID;
                        rCategoryName = MetricInfo[i].GCAName;                        
                    }

                if ((rCategory != cCategory) || (rAspect != cAspect))
                { // start gca row
                    x++;
                    sheet.Range[GetCellRange(y, x)].Text = rCategoryName;
                    sheet.Range[GetCellRange((char)(y + 1), x)].Text = rAspectName;
                    sheet.Range[GetCellRange(y, x)].CellStyle.Font.Underline = ExcelUnderline.Single;
                    sheet.Range[GetCellRange(y, x)].CellStyle.Font.Bold = true;
                    sheet.Range[GetCellRange((char)(y + 1), x)].CellStyle.Font.Underline = ExcelUnderline.Single;
                    x++;
                }
                Guid rPI = (Guid)MetricInfo[i].PerformanceIndicatorID;
                string rPIName = MetricInfo[i].PIName;
                if (cPI != rPI)
                {                    
                    sheet.Range[GetCellRange((char)(y + 2), x)].Text = MetricInfo[i].PICode;
                    sheet.Range[GetCellRange((char)(y + 3), x)].Text = MetricInfo[i].PIName;
                    sheet.Range[GetCellRange((char)(y + 2), x)].CellStyle.Font.Bold = true;
                    sheet.Range[GetCellRange((char)(y + 3), x)].CellStyle.Font.Bold = true;
                    x++;
                }
                sheet.Range[GetCellRange((char)(y + 2), x)].Text = MetricInfo[i].MetricCode;
                string MetricName = MetricInfo[i].MetricName;
                if ((bool)MetricInfo[i].AllowCustomMetricNames && (!String.IsNullOrEmpty(MetricInfo[i].CustomMetricAlias) || !String.IsNullOrEmpty(MetricInfo[i].CustomMetricCode)) )
                {
                    if (!String.IsNullOrEmpty(MetricInfo[i].CustomMetricAlias))
                        MetricName += "  " + MetricInfo[i].CustomMetricAlias + " ";
                    if (!String.IsNullOrEmpty(MetricInfo[i].CustomMetricAlias) && !String.IsNullOrEmpty(MetricInfo[i].CustomMetricCode))
                        MetricName += "-";
                    if (!String.IsNullOrEmpty(MetricInfo[i].CustomMetricCode))
                        MetricName += "  " + MetricInfo[i].CustomMetricCode;
                }
                sheet.Range[GetCellRange((char)(y + 3), x)].Text = MetricName;
                sheet.Range[GetCellRange((char)(y + 4), x)].Number = double.Parse(MetricInfo[i].SumValue, System.Globalization.NumberStyles.Any);
                sheet.Range[GetCellRange((char)(y + 5), x)].Text = MetricInfo[i].UoMName;
                x++;

                cLocationName = rLocationName;
                cCategory = rCategory;
                cAspect = rAspect;
                cPI = rPI;
            }

            // general
            sheet.Range[GetCellRange(y, 1, (char)(y + 5), x + 1)].CellStyle.Font.FontName = "Calibri";
            sheet.Range[GetCellRange(y, 1, (char)(y + 5), x + 1)].CellStyle.Font.Size = 11;
            sheet.Range[GetCellRange(y, 1, (char)(y + 5), x + 1)].RowHeight = 15;
            sheet.Range[GetCellRange(y, 1, (char)(y + 5), x + 1)].CellStyle.VerticalAlignment = ExcelVAlign.VAlignCenter;

            //label
            sheet.Range[GetCellRange(y, 1, y, 2)].RowHeight = 21;            
            sheet.Range[GetCellRange((char)(y + 1), 1, (char)(y + 1), 2)].CellStyle.Font.Size = 16;
            sheet.Range[GetCellRange((char)(y + 1), 1, (char)(y + 1), 2)].CellStyle.Font.Bold = true;
            sheet.Range[GetCellRange((char)(y + 3), 2)].CellStyle.Font.Size = 16;
            sheet.Range[GetCellRange((char)(y + 3), 2)].CellStyle.Font.Bold = true;
            sheet.Range[GetCellRange((char)(y + 2), 3)].CellStyle.Font.Size = 16;
            sheet.Range[GetCellRange((char)(y + 2), 3)].CellStyle.Font.Bold = true;
            sheet.Range[GetCellRange(y, 5, (char)(y + 5), 5)].CellStyle.Borders[ExcelBordersIndex.EdgeBottom].LineStyle = ExcelLineStyle.Thin;

            // autofit
            sheet.Range[GetCellRange(y, 1, (char)(y + 24), x + 1)].AutofitColumns();
            sheet.Range[GetCellRange(y, 1, (char)(y + 24), x + 1)].AutofitRows();            
                        
            workbook.SaveAs(txtTitle.Text + ".xlsx", ExcelSaveType.SaveAsXLS, Response, ExcelDownloadType.PromptDialog);
            workbook.Close();
            excelEngine.Dispose();
        }

        protected void ldsLocationType_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            e.Result = Bll.PerformanceIndicator.GetOrgLocationTypes();            
        }        
    }
}