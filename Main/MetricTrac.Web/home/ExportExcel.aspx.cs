using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Syncfusion.XlsIO;

namespace MetricTrac.home
{
    public partial class ExportExcel : System.Web.UI.Page
    {//OnUse="MF_Use" OnGroupChanged="MF_GroupChanged"
        private const int InitTimePeriods = 7;
        const string SesionName = "ExportExcel_FilterInfo";
        public static MetricTrac.Bll.MetricFilter.Extend Filter
        {
            get { return HttpContext.Current.Session[SesionName] as MetricTrac.Bll.MetricFilter.Extend; }
            set { HttpContext.Current.Session[SesionName] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            MetricTrac.MasterPage mp = Page.Master as MetricTrac.MasterPage;
            mp.IncludeJqueryUi = true;

            mBackgroundColorCount = 0;

            List<int> UsedFrequencyId = MetricTrac.Bll.Frequency.ListUsed();
            phDaily.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.Daily);
            phWeekly.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.Weekly);
            phMonthly.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.Monthly);
            phQtrly.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.Qtrly);
            phSemiAnnual.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.SemiAnnual);
            phAnnual.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.Annual);
            phBiAnnual.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.BiAnnual);
            phFiscalQtrly.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.FiscalQtr);
            phFiscalSemiAnnual.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.FiscalSemiAnnual);
            phFiscalAnnual.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.FiscalAnnual);
            phFiscalBiAnnual.Visible = UsedFrequencyId.Contains((int)MetricTrac.Bll.Frequency.FrequencyName.FiscalBiAnnual);

        }

        protected void Page_Prerender(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Filter != null)
                {
                    MF.SelectedOrgLocations = Filter.FilterOrgLocation;
                    MF.SelectedGroupCategoryAspect = Filter.GroupCategoryAspectID;
                    MF.SelectedPerformanceIndicators = Filter.FilterPI;
                    MF.SelectedMetrics = Filter.FilterMetric;
                    MF.SelectedDataCollector = Filter.DataCollectorID;
                }

                DateTime DNNow = DateTime.Now;
                dpCommonTo.SelectedDate = DNNow;
                dpCommonFrom.SelectedDate = DNNow.AddYears(-1);

                dpCommonTo.SelectedDate = DNNow;
                dpSelectDayFrom.SelectedDate = DNNow.AddDays(-InitTimePeriods);

                DateTime dt = MetricTrac.Bll.Frequency.GetNormalizedDate(2, DNNow);
                cSelectWeekTo.SelectedDate = dt;
                cSelectWeekFrom.SelectedDate = dt.AddDays(-InitTimePeriods * 7);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(3, DNNow);
                cSelectMonthTo.SelectedDate = dt;
                cSelectMonthFrom.SelectedDate = dt.AddMonths(-InitTimePeriods);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(4, DNNow);
                cSelectQuarterTo.SelectedDate = dt;
                cSelectQuarterFrom.SelectedDate = dt.AddMonths(-InitTimePeriods * 3);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(5, DNNow);
                cSelectSemiYearFrom.SelectedDate = dt;
                cSelectSemiYearFrom.SelectedDate = dt.AddMonths(-InitTimePeriods * 6);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(6, DNNow);
                cSelectYearTo.SelectedDate = dt;
                cSelectYearFrom.SelectedDate = new DateTime(2000, 1, 1);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(7, DNNow);
                cSelectBiYearTo.SelectedDate = dt;
                cSelectBiYearFrom.SelectedDate = new DateTime(2000, 1, 1);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(8, DNNow);
                cSelectFiscalQuarterTo.SelectedDate = dt;
                cSelectFiscalQuarterFrom.SelectedDate = dt.AddMonths(-InitTimePeriods * 3);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(9, DNNow);
                cSelectFiscalSemiYearTo.SelectedDate = dt;
                cSelectFiscalSemiYearFrom.SelectedDate = dt.AddMonths(-InitTimePeriods * 6);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(10, DNNow);
                cSelectFiscalYearTo.SelectedDate = dt;
                cSelectFiscalYearFrom.SelectedDate = new DateTime(2000, 1, 1);

                dt = MetricTrac.Bll.Frequency.GetNormalizedDate(11, DNNow);
                cSelectFiscalBiYearTo.SelectedDate = dt;
                cSelectFiscalBiYearFrom.SelectedDate = new DateTime(2000, 1, 1);
            }
        }

        private void CloseOnReload()
        {
            ScriptManager.RegisterStartupScript(this, typeof(MetricList), "_CorrectReturn_", "CloseOnReload(false);", true);
        }

        protected string CellHeight { get { return "21px"; } }
        protected string NameWidth { get { return "99px"; } }
        protected string FromWidth { get { return "40px"; } }
        protected string ToWidth { get { return "21px"; } }
        protected string InputWidth { get { return "175px"; } }

        int mBackgroundColorCount;
        protected string BackgroundColor
        {
            get
            {
                mBackgroundColorCount++;
                if ((mBackgroundColorCount & 1) != 0) return string.Empty;
                return "style='background-color:#F0F0F0'";
            }
        }

        string GetChar(int n)
        {
            return new string(new char[] { ((char)('A' + n)) });
        }

        string GetColumnName(int n)
        {
            string ColumnName = GetChar(n % 26);
            if (n >= 26) ColumnName = GetChar(n / 26 - 1) + ColumnName;
            return ColumnName;
        }

        void ClearWorSheet(IWorksheet worksheet, string cell)
        {
            worksheet.Range[cell].Text = string.Empty;
        }

        void ClearWorSheet(IWorksheet worksheet)
        {
            string[] ClearWorSheetCells = new string[] { "A4","A5","A6","B4","B5","B6","C4","C5","C6",
                "D4","D5","D6","E4","E5","E6","F4","F5","F6"};
            foreach (string c in ClearWorSheetCells) worksheet.Range[c].Text = string.Empty;
        }

        bool AnyPeriodExist = false;
        private void AddExportPeriod(DateTime? Begin, DateTime? End, int FrequencyID, string PeriodName, string PeriodInterval)
        {
            if (CommonBegin != null) Begin = CommonBegin;
            if (CommonEnd != null) End = CommonEnd;
            if (Begin == null || End == null || Begin == DateTime.MinValue || End == DateTime.MinValue) return;

            DateTime dtBegin = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID, (DateTime)Begin);
            DateTime dtEnd = MetricTrac.Bll.Frequency.GetNormalizedDate(FrequencyID, (DateTime)End);
            dtEnd = MetricTrac.Bll.Frequency.AddPeriod(dtEnd, FrequencyID, 1);
            AnyPeriodExist = true;

            MetricTrac.Bll.FrequencyMetric fm = MetricTrac.Bll.MetricValue.ExportList(dtBegin, dtEnd, FrequencyID,
                MF.SelectedMetrics, MF.SelectedOrgLocations, MF.SelectedGroupCategoryAspect,
                MF.SelectedPerformanceIndicators, MF.SelectedDataCollector, true, true, null);

            workbook.Worksheets.AddCopy(0);
            IWorksheet worksheet = workbook.Worksheets[workbook.Worksheets.Count - 1];
            ClearWorSheet(worksheet);
            worksheet.Range["D1"].Text = PeriodInterval;
            worksheet.Name = PeriodName;
            worksheet.Range["D2"].Text = "";
            worksheet.Range["D3"].Text = "";
            worksheet.Range["E2"].Text = "";
            worksheet.Range["E3"].Text = "";
            worksheet.Range["F2"].Text = "";
            worksheet.Range["F3"].Text = "";

            for (int DateIndex = 0; DateIndex < fm.Date.Count; DateIndex++)
            {
                string ColumnName = GetColumnName(DateIndex + 3);
                worksheet.Range[2, DateIndex + 4].Text = fm.Date[DateIndex].sDate.Replace("\n", " ");
                worksheet.Range[3, DateIndex + 4].DateTime = fm.Date[DateIndex].Date;
            }

            for (int MerticIndex = 0; MerticIndex < fm.Metrics.Count; MerticIndex++)
            {
                string RowName = (MerticIndex + 4).ToString();
                var m = fm.Metrics[MerticIndex];
                worksheet.Range["A" + RowName].Text = m.OrgLocationFullName;
                worksheet.Range["B" + RowName].Text = m.Name;
                worksheet.Range["C" + RowName].Text = m.InputUnitOfMeasureName;
                for (int ValueIndex = 0; ValueIndex < m.MetricValues.Count; ValueIndex++)
                {
                    var v = m.MetricValues[ValueIndex];
                    if (string.IsNullOrEmpty(v.Value)) continue;
                    string ValueColumnName = GetColumnName(ValueIndex + 3);
                    worksheet.Range[ValueColumnName + RowName].Text = v.Value;
                }
            }

        }

        DateTime? CommonBegin;
        DateTime? CommonEnd;
        IWorkbook workbook;
        ExcelEngine excelEngine;

        protected void bExport_Click(object sender, EventArgs e)
        {
            try
            {
                excelEngine = new ExcelEngine();
                IApplication application = excelEngine.Excel;
                workbook = application.Workbooks.Open(Request.MapPath("~/App_Data/ExportMetricTemplate.xls"));
                AnyPeriodExist = false;

                if (rbIntervalCommon.Checked)
                {
                    if (dpCommonFrom.IsEmpty) return;
                    if (dpCommonTo.IsEmpty) return;
                    CommonBegin = dpCommonFrom.SelectedDate;
                    CommonEnd = dpCommonTo.SelectedDate;
                }
                else if (rbIntervalFull.Checked)
                {
                    MetricTrac.Bll.MetricValue.GetFullDateRange(out CommonBegin, out CommonEnd);
                }

                if (phDaily.Visible) AddExportPeriod(dpSelectDayFrom.SelectedDate, dpSelectDayTo.SelectedDate, 1, "Daily", "Day");
                if (phWeekly.Visible) AddExportPeriod(cSelectWeekFrom.SelectedDate, cSelectWeekTo.SelectedDate, 2, "Weekly", "Week");
                if (phMonthly.Visible) AddExportPeriod(cSelectMonthFrom.SelectedDate, cSelectMonthTo.SelectedDate, 3, "Monthly", "Month");

                if (phQtrly.Visible) AddExportPeriod(cSelectQuarterFrom.SelectedDate, cSelectQuarterTo.SelectedDate, 4, "Quarterly", "Quarter");
                if (phSemiAnnual.Visible) AddExportPeriod(cSelectSemiYearFrom.SelectedDate, cSelectSemiYearTo.SelectedDate, 5, "Semiannual", "Semi-year");
                if (phAnnual.Visible) AddExportPeriod(cSelectYearFrom.SelectedDate, cSelectYearTo.SelectedDate, 6, "Annual", "Year");
                if (phBiAnnual.Visible) AddExportPeriod(cSelectBiYearFrom.SelectedDate, cSelectBiYearTo.SelectedDate, 7, "Biannual", "Bi-year");

                if (phFiscalQtrly.Visible) AddExportPeriod(cSelectFiscalQuarterFrom.SelectedDate, cSelectFiscalQuarterTo.SelectedDate, 4, "Quarterly", "Quarter");
                if (phFiscalSemiAnnual.Visible) AddExportPeriod(cSelectFiscalSemiYearFrom.SelectedDate, cSelectFiscalSemiYearTo.SelectedDate, 5, "Semiannual", "Semi-year");
                if (phFiscalAnnual.Visible) AddExportPeriod(cSelectFiscalYearFrom.SelectedDate, cSelectFiscalYearTo.SelectedDate, 6, "Annual", "Year");
                if (phFiscalBiAnnual.Visible) AddExportPeriod(cSelectFiscalBiYearFrom.SelectedDate, cSelectFiscalBiYearTo.SelectedDate, 7, "Biannual", "Bi-year");
                if (!AnyPeriodExist) return;
                workbook.Worksheets.Remove(0);
                workbook.SaveAs("MetricExport.xls", ExcelSaveType.SaveAsXLS, Response, ExcelDownloadType.PromptDialog);
            }
            finally
            {
                try
                {
                    workbook.Close();
                    excelEngine.Dispose();
                }
                catch { }
            }
        }
    }
}
