using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class ApplicationLog
    {
        public static void LogAppMessage(string Code, string Message)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                ApplicationLog AppLog = new ApplicationLog();
                AppLog.ApplicationLogID = Guid.NewGuid();
                AppLog.Code = Code;
                AppLog.Message = Message;
                AppLog.Created = DateTime.Now;
                dc.ApplicationLog.InsertOnSubmit(AppLog);
                dc.SubmitChanges();
            }

        }

        public static string GetLastAppMessage(string Code)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            ApplicationLog al =
                    (from a in dc.ApplicationLog
                    where a.Code == Code
                    orderby a.Created descending
                    select a).FirstOrNull();
                 
            string AppLog = String.Empty;
            if (al != null)
                AppLog = "Date: " + al.Created + " |Message:" + al.Message;
            return AppLog;
        }
    }
}
