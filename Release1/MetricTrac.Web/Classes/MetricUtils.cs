using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web;

namespace MetricTrac.Utils
{
    public class MetricUtils
    {
        public static readonly string SessionObjectNameForAddedPIs = "_AddedPIs";
        public static readonly string SessionObjectNameForDeletedPIs = "_DeletedPIs";
        public static readonly string SessionObjectNameForPIFormName = "_PerformanceIndicatorFormName";
        public static readonly string SessionObjectNameForAddedOrgLocations = "_PerformanceIndicatorFormOrgAddedLocations";
        public static readonly string SessionObjectNameForRemovedOrgLocations = "_PerformanceIndicatorFormOrgRemovedLocations";

        public static void InitLinqDataSources(LinqDataSource lds)
        {
            Parameter InstanceIdParam = new Parameter("InstanceId", System.Data.DbType.Guid, MetricTrac.Bll.LinqMicajahDataContext.InstanceId.ToString());
            lds.WhereParameters.Add(InstanceIdParam);
        }
    
        public static void ClearSession(System.Web.SessionState.HttpSessionState httpSessionState)
        {
            httpSessionState[SessionObjectNameForAddedPIs] = null;
            httpSessionState[SessionObjectNameForDeletedPIs] = null;
            httpSessionState[SessionObjectNameForPIFormName] = null;
            httpSessionState[SessionObjectNameForAddedOrgLocations] = null;
            httpSessionState[SessionObjectNameForRemovedOrgLocations] = null;
        }

        public static Guid? GetMetricCategoryFromFullName(string FullName, IQueryable<MetricTrac.Bll.MetricCategory> DataSource)
        {
            if (string.IsNullOrEmpty(FullName)) return null;
            MetricTrac.Bll.MetricCategory[] t = DataSource.ToArray();
            string[] MetricCategorys = FullName.Split('>');

            Guid MetricCategoryID = Guid.Empty;
            string MetricCategory;

            for (int i = 0; i < MetricCategorys.Length; i++)
            {
                MetricCategory = HttpUtility.HtmlEncode(MetricCategorys[i].Trim());

                MetricTrac.Bll.MetricCategory[] c;

                if (i == MetricCategorys.Length - 1) c = t.Where(mc => mc.Name.ToLower().StartsWith(MetricCategory.ToLower())).ToArray();
                else c = t.Where(mc => mc.Name.ToLower() == MetricCategory.ToLower()).ToArray();

                if (MetricCategoryID != Guid.Empty) c = c.Where(mc => mc.ParentId == MetricCategoryID).ToArray();
                else c = c.Where(mc => mc.ParentId == null).ToArray();

                if (c.Length > 0)
                {
                    MetricCategoryID = c[0].MetricCategoryID;
                }
                else return MetricCategoryID;
            }
            return MetricCategoryID;
        }

        public static bool IsPopupSupported(HttpRequest Request)
        {
            bool CheckingResult = true;
            int IEVersion = 0;
            if (Request.Browser.Browser == "IE")
                if (!String.IsNullOrEmpty(Request.Browser.Version))
                    if (int.TryParse(Request.Browser.Version.Substring(0, 1), out IEVersion))
                        if (IEVersion == 9)
                            CheckingResult = false;
            return CheckingResult;
        }
    }
}
