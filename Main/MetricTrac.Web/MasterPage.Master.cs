using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace MetricTrac
{
    public partial class MasterPage : Micajah.Common.Pages.MasterPage
    {
        bool mIncludeJquery;
        public bool IncludeJquery
        {
            get { return mIncludeJquery; }
            set { mIncludeJquery = value; }
        }

        bool mIncludeJqueryUi;
        public bool IncludeJqueryUi
        {
            get { return mIncludeJqueryUi; }
            set { mIncludeJqueryUi = value; if (value) IncludeJquery = true; }
        }

        public string ScriptPath
        {
            get 
            {
                string s = Request.ApplicationPath;
                if (!s.EndsWith("/")) s += "/";
                s += "includes/script/";
                return s;
            }
        }

        public System.Web.UI.HtmlControls.HtmlHead PageHead
        {
            get { return pageHead; }
        }

        DateTime InitTime0;
        DateTime InitTime1;
        DateTime LoadTime0;
        DateTime LoadTime1;
        DateTime PrerenderTime0;
        DateTime PrerenderTime1;
        DateTime RenderTime0;
        DateTime RenderTime1;

        protected override void OnInit(EventArgs e)
        {
            InitTime0 = DateTime.Now;
            base.OnInit(e);
            InitTime1 = DateTime.Now;
        }

        protected override void OnLoad(EventArgs e)
        {
            LoadTime0 = DateTime.Now;
            base.OnLoad(e);
            LoadTime1 = DateTime.Now;
        }

        protected override void OnPreRender(EventArgs e)
        {
            PrerenderTime0 = DateTime.Now;
            base.OnPreRender(e);
            PrerenderTime1 = DateTime.Now;
        }

        public void AddStyleSheet(string Href)
        {
            System.Web.UI.HtmlControls.HtmlLink l = new System.Web.UI.HtmlControls.HtmlLink();
            l.Attributes.Add("rel", "stylesheet");
            l.Attributes.Add("media", "screen");
            l.Attributes.Add("type", "text/css");
            l.Href = Href;
            PageHead.Controls.Add(l);
        }

        public ScriptManager scriptManager
        {
            get { return csmManager; }
        }

        public string ralpLoadingPanel1ID
        {
            get { return LoadingPanel1.ID; }
        }

        public RadAjaxLoadingPanel ralpLoadingPanel1
        {
            get { return LoadingPanel1; }
        }

        private void WriteTime(DateTime dt, HtmlTextWriter writer, string Name)
        {
            TimeSpan ts = dt - (InitTime0 == DateTime.MinValue ? LoadTime0 : InitTime0);
            writer.WriteLine(Name.PadLeft(21)+"='"+dt.ToString("HH:mm:ss,fff") + ts.TotalSeconds.ToString("#0.000").PadLeft(9)+"'");
        }

        Dictionary<string, string> DebufInfo;
        public void AddDebufInfo(string Name, string Val)
        {
            if (DebufInfo == null) DebufInfo = new Dictionary<string, string>();
            DebufInfo.Add(Name,Val);
        }

        protected override void Render(HtmlTextWriter writer)
        {
            RenderTime0 = DateTime.Now;
            base.Render(writer);
            RenderTime1 = DateTime.Now;

            writer.WriteLine("\n<DebugInfo ");
            WriteTime(InitTime0     , writer,"Init_Begin");
            WriteTime(InitTime1     , writer,"Init_End");
            WriteTime(LoadTime0     , writer,"Load_Begin");
            WriteTime(LoadTime1     , writer,"Load_End");
            WriteTime(PrerenderTime0, writer,"Prerender_Begin");
            WriteTime(PrerenderTime1, writer, "Prerender_End");
            WriteTime(RenderTime0   , writer, "Render_Begin");
            WriteTime(RenderTime1   , writer, "Render_End");
            if (DebufInfo != null)
            {
                foreach (string n in DebufInfo.Keys)
                {
                    writer.WriteLine(n.PadLeft(21) + "='" + DebufInfo[n] + "'");
                }
            }
            writer.WriteLine("/>");
        }
    }
}
