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
    public partial class DataValueChart : System.Web.UI.UserControl
    {
        /*Guid? mMetricID;
        public Guid MetricID
        {
            get 
            {
                if (mMetricID != null) return (Guid)mMetricID;
                mMetricID = Guid.Empty;
                string s = Request.QueryString["MetricID"];
                if(!string.IsNullOrEmpty(s)) mMetricID = new Guid(s);
                return (Guid)mMetricID;
            }
            set
            {
                mMetricID = value;
            }
        }

        Guid? mOrgLocationID;
        public Guid OrgLocationID
        {
            get 
            {
                if (mOrgLocationID != null) return (Guid)mOrgLocationID;
                mOrgLocationID = Guid.Empty;
                string s = Request.QueryString["OrgLocationID"];
                if (!string.IsNullOrEmpty(s)) mOrgLocationID = new Guid(s);
                return (Guid)mOrgLocationID;
            }
            set
            {
                mOrgLocationID = value;
            }
        }*/

        private Guid? mScoreCardMetricID;
        public Guid ScoreCardMetricID
        {
            get
            {
                if(mScoreCardMetricID!=null) return (Guid)mScoreCardMetricID;
                mScoreCardMetricID = Guid.Empty;
                string s = Request.QueryString["ScoreCardMetricID"];
                if (!string.IsNullOrEmpty(s))
                {
                    try { mScoreCardMetricID = new Guid(s); }
                    catch { }
                }
                return (Guid)mScoreCardMetricID;
            }
            set
            {
                mScoreCardMetricID = value;
            }
        }


        protected DateTime DateBegin
        {
            get
            {
                string strDate = Request.QueryString["Date"];
                DateTime dt = DateTime.Now;
                if (!string.IsNullOrEmpty(strDate))
                {
                    try
                    {
                        dt = DateTime.ParseExact(strDate, "MM-dd-yyyy", System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch { }
                }
                return dt;
            }
        }


        protected void Page_Prerender(object sender, EventArgs e)
        {
            var mo = MetricTrac.Bll.MetricValue.List(6, DateBegin, ScoreCardMetricID);
            if (mo == null) return;            
            Telerik.Charting.ChartSeries l = rcMetricOrgLocation.Series[3];
            

            lbTitle.Text = mo.Name + "<br>" + mo.OrgLocationFullName;

            for (int NumVal = 0; NumVal < mo.MetricValues.Count; NumVal++)
            {
                var v = mo.MetricValues[NumVal];
                Telerik.Charting.ChartSeriesItem it;
                if (v == null || v.DValue == null)
                {
                    it = new Telerik.Charting.ChartSeriesItem(0);
                    it.Empty = true;
                    it.Label.Visible = false;
                }
                else
                {
                    string ItemLabel = ((double)v.DValue).ToString("N" + mo.NODecPlaces);
                    it = new Telerik.Charting.ChartSeriesItem((double)v.DValue, ItemLabel);
                }

                it.XValue = NumVal;                
                rcMetricOrgLocation.PlotArea.XAxis.Items[NumVal].TextBlock.Text = v.Period;
                l.Items.Add(it);
            }            

            if (mo.UnitOfMeasureID != null)
            {
                string name = MetricTrac.Bll.Mc_UnitsOfMeasure.GetSingularAbbreviation((Guid)mo.UnitOfMeasureID);
                if (!string.IsNullOrEmpty(name))
                {                    
                    rcMetricOrgLocation.PlotArea.Appearance.Dimensions.Margins.Top = 15;
                    rcMetricOrgLocation.PlotArea.Appearance.Dimensions.Margins.Right = 7;
                    rcMetricOrgLocation.PlotArea.YAxis.AxisLabel.TextBlock.Text = name;
                    rcMetricOrgLocation.PlotArea.YAxis.AxisLabel.Visible = true;
                }
            }
            
            var scm = MetricTrac.Bll.ScoreCardMetric.Get(ScoreCardMetricID);
            
            if (scm.MaxValue != null)
            {
                rcMetricOrgLocation.PlotArea.YAxis.AutoScale = false;               
                rcMetricOrgLocation.PlotArea.YAxis.MaxValue = (double)scm.MaxValue;
                double min = 0;
                if (scm.MinValue != null) min = (double)scm.MinValue;
                rcMetricOrgLocation.PlotArea.YAxis.MinValue = min;                
                double Step = ((double)scm.MaxValue - min) / 10;
                double Pow = Math.Pow(10, ((int)(Math.Log10(Step))));
                double Base = (int)(Step / Pow);
                if (Base < 2) Step = Pow;
                else if (Base < 5) Step = 2 * Pow;
                else Step = 10 * Pow;
                rcMetricOrgLocation.PlotArea.YAxis.Step = Step;                
            }            
            InitHorizontLine(scm.BaselineValue, scm.BaselineValueLabel, 0, mo.MetricValues.Count);
            InitHorizontLine(scm.Breakpoint1Value, scm.Breakpoint1ValueLabel, 1, mo.MetricValues.Count);
            InitHorizontLine(scm.Breakpoint2Value, scm.Breakpoint2ValueLabel, 2, mo.MetricValues.Count);
        }

        private void InitHorizontLine(decimal? v, string Label, int SerieNumber, int ValuesCount)
        {
            Telerik.Charting.ChartSeries l = rcMetricOrgLocation.Series[SerieNumber];
            if (v == null)
            {
                l.Visible = false;
                return;
            }           

            l.Visible = true;
            Telerik.Charting.ChartSeriesItem it;

            it = new Telerik.Charting.ChartSeriesItem((double)v);
            it.XValue = -0.5;
            it.Label.TextBlock.Visible = false;
            l.Items.Add(it);

            it = new Telerik.Charting.ChartSeriesItem((double)v);
            it.XValue = ValuesCount - 0.5;
            if (String.IsNullOrEmpty(Label))
                it.Label.TextBlock.Visible = false;
            else            
                it.Label.TextBlock.Text = Label;
            l.Items.Add(it);
            
        }

        protected void dsMetricOrgLocation_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            var mo = MetricTrac.Bll.MetricValue.List(6, DateBegin, ScoreCardMetricID);
            e.Result = mo.MetricValues;
        }
    }
}
