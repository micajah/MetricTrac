using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;

using Telerik.Web.UI;

namespace MetricTrac
{
    public partial class Dashboard : System.Web.UI.Page
    {
        void InitGauge(Dundas.Gauges.WebControl.GaugeContainer c, double v, double? min, double? max, double? break1, double? break2, double MaxValue)
        {
            c.Parent.Visible = true;
            Dundas.Gauges.WebControl.CircularGauge g = c.CircularGauges[0];
            g.Pointers[0].Value = v;

            if (min == null) min = 0;
            if (max == null) max = MaxValue;

            if (g.Scales[0].Maximum <= (double)min)
            {
                g.Scales[0].Maximum = (double)max;
                g.Scales[0].Minimum = (double)min;
            }
            else
            {
                g.Scales[0].Minimum = (double)min;
                g.Scales[0].Maximum = (double)max;                
            }

            if (break1 >= max) break1 = null;
            if (break2 >= max) break2 = null;

            g.Ranges[0].StartValue = (double)min;
            g.Ranges[0].EndValue = (double)max;

            if (break1 == null)
            {
                g.Ranges[1].Visible = false;
            }
            else
            {
                g.Ranges[0].EndValue = (double)break1;
                g.Ranges[1].Visible = true;
                g.Ranges[1].StartValue = (double)break1;
                g.Ranges[1].EndValue = (double)max;
            }

            if (break2 == null)
            {
                g.Ranges[2].Visible = false;
            }
            else
            {
                if (break1 == null) g.Ranges[0].EndValue = (double)break2;
                g.Ranges[1].EndValue = (double)break2;
                g.Ranges[2].Visible = true;
                g.Ranges[2].StartValue = (double)break2;
                g.Ranges[2].EndValue = (double)max;
            }
        }

        void InitDiagrams()
        {
            if (ScoreCardMetricID == null)
            {
                string s = Request.QueryString["ScoreCardMetricID"];
                if (!string.IsNullOrEmpty(s))
                {
                    try
                    {
                        ScoreCardMetricID = new Guid(s);
                    }
                    catch { }
                }
            }

            phCurrent.Visible = false;
            phPrevious.Visible = false;
            phChange.Visible = false;
            dataValueChart.Visible = false;

            if (ScoreCardMetricID != null)
            {
                var m = MetricTrac.Bll.ScoreCardMetric.Get((Guid)ScoreCardMetricID);
                MetricTrac.Bll.ScoreCardDashboard d=null;
                if (DasboardID!=null) d = MetricTrac.Bll.ScoreCardDashboard.Get((Guid)DasboardID);
                if (m != null)
                {
                    double MaxValue = 0;
                    if (m.CurrentValue != null) MaxValue = (double)m.CurrentValue;
                    if (m.PreviousValue != null && m.PreviousValue > MaxValue) MaxValue = (double)m.PreviousValue;
                    if (d!=null && d.MaxValue != null && (double)((decimal)d.MaxValue) > MaxValue) MaxValue = (double)d.MaxValue;

                    double MinValue = 0;
                    if (d!=null && d.MinValue != null) MinValue = (double)d.MinValue;
                    if (m.CurrentValue != null && m.CurrentValue < MinValue) MinValue = (double)m.CurrentValue;
                    if (m.PreviousValue != null && m.PreviousValue < MinValue) MinValue = (double)m.PreviousValue;

                    double? Breakpoint1Value = d == null || d.Breakpoint1Value==null ? (double?)null : (double)((decimal)(d.Breakpoint1Value));
                    double? Breakpoint2Value = d == null || d.Breakpoint2Value == null ? (double?)null : (double)((decimal)(d.Breakpoint2Value));

                    if (m.CurrentValue != null)
                    {
                        InitGauge(dgwcCurrent, (double)m.CurrentValue, MinValue, MaxValue, Breakpoint1Value, Breakpoint2Value, MaxValue*1.1);
                            //, (double?)m.MinValue, (double?)m.MaxValue, true, MaxValue);
                        lbCurrent.Text = m.CurrentValueStr;
                    }

                    if (m.PreviousValue != null)
                    {
                        InitGauge(dgwcPrevious, (double)m.PreviousValue, MinValue, MaxValue, Breakpoint1Value, Breakpoint2Value, MaxValue*1.1);
                        lbPrevious.Text = m.PreviousValueStr;
                    }

                    if (m.ChangeValue != null)
                    {
                        InitGauge(dgwcChange, (double)m.ChangeValue, -100, 100, null, null, 100);
                        lbChange.Text = m.ChangeValueStr;
                    }

                    dataValueChart.Visible = true;
                    dataValueChart.ScoreCardMetricID = m.ScoreCardMetricID;
                    dataValueChart.DasboardID = DasboardID;
                    //dataValueChart.OrgLocationID = m.OrgLocationID;
                }
            }
        }

        private bool PrerenderExecuted;
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            InitDiagrams();
            PrerenderExecuted = true;
            //InitAjah();
        }

        Guid? ScoreCardMetricID
        {
            get { object o = ViewState["ScoreCardMetricID"]; if (o is Guid) return (Guid)o; return null; }
            set { ViewState["ScoreCardMetricID"] = value; }
        }

        Guid? DasboardID
        {
            get { object o = ViewState["DasboardID"]; if (o is Guid) return (Guid)o; return null; }
            set { ViewState["DasboardID"] = value; }
        }
        protected void scl_SelectedIndexChanged(object sender, EventArgs e)
        {
            ScoreCardMetricID = scl.ScoreCardMetricID;
            DasboardID = scl.DasboardID;
            if (PrerenderExecuted) InitDiagrams();
            //InitAjah();
        }

        //private bool AjaxInitialized;

        /*void InitAjah()
        {
            if (AjaxInitialized) return;
            AjaxInitialized = true;
            Telerik.Web.UI.RadAjaxManager ram = Telerik.Web.UI.RadAjaxManager.GetCurrent(this);
            ram.UpdatePanelsRenderMode = UpdatePanelRenderMode.Inline;
            ram.AjaxSettings.AddAjaxSetting(scl, pDashboard, ralpDashboard);
        }*/
    }
}
