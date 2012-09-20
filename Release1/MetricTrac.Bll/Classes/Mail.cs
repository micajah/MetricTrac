using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Text;


namespace MetricTrac.Utils
{
    public class Mail
    {
        protected static System.Net.Mail.SmtpClient smtp = null;
        protected static System.Net.Mail.SmtpClient SMTPServer
        {
            get
            {
                if (smtp == null)
                {

                    /*Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
                    MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
                    if (settings != null)
                    {
                        try
                        {
                            smtp = new System.Net.Mail.SmtpClient(settings.Smtp.Network.Host, settings.Smtp.Network.Port);                            
                            smtp.Credentials = System.Net.CredentialCache.DefaultNetworkCredentials;
                            smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

                            //smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.SpecifiedPickupDirectory;
                            //smtp.PickupDirectoryLocation = @"C:\Inetpub\mailroot\Pickup\";

                        }
                        catch
                        {
                            EventLog.WriteEntry("MetricTrac.SendMail", "Can't create SmtpClient object", EventLogEntryType.Error);
                        }
                    }
                    else EventLog.WriteEntry("MetricTrac.SendMail", "Can't find SMTPServer key value.", EventLogEntryType.Error);
                     */
                    smtp = new System.Net.Mail.SmtpClient();
                }
                return smtp;
            }
        }

        protected static bool IsValidEmail(string strIn)
        {
            return System.Text.RegularExpressions.Regex.IsMatch(strIn, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        protected static int SendEmail(string ToEmail, string ToName, string CC, string Subject, string Body)
        {
            System.Net.Mail.MailMessage mess = new System.Net.Mail.MailMessage();
            //mess.From = new System.Net.Mail.MailAddress(FromEmail, FromName);
            if (ToEmail != null && ToEmail.Length > 0)
            {
                string[] _toArr = ToEmail.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string _to in _toArr) mess.To.Add(new System.Net.Mail.MailAddress(_to.Trim(), ToName));
            }
            mess.IsBodyHtml = true;
            mess.Subject = Subject;
            mess.Body = Body;
            if (CC != null && CC.Length > 0)
            {
                string[] _ccArr = CC.Split(new char[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string _cc in _ccArr) mess.CC.Add(_cc);
            }
            SMTPServer.Send(mess);
            return 0;
        }

        public static void Send(string ToEmail, string ToName, string Subject, string Body)
        {
            /*string FromEmail = String.Empty;
            string FromName = "MetricTrac Support";
            Configuration config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            MailSettingsSectionGroup settings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            if (settings != null)
                FromEmail = settings.Smtp.From;
            else
                FromEmail = "noreply@metrictrac.com";*/
            //try
            {
                if (IsValidEmail(ToEmail) && SMTPServer != null)
                    SendEmail(ToEmail, ToName, String.Empty, Subject, Body);
            }
            /*catch (System.Net.Mail.SmtpException ex)
            {
                EventLog.WriteEntry("MetricTrac.SendMail", "Can't send email. Error: " + ex.ToString(), EventLogEntryType.Warning);                
            }*/
        }

        public static string BuildLogMessageBody(Bll.MetricValue.Extend OldValue, MetricTrac.Bll.MetricValue.Extend NewValue, string message, Micajah.Common.Security.UserContext context, Bll.Mc_User.Extend e, MetricTrac.Bll.MetricValueChangeTypeEnum changeType)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("User");
            if (context != null)
                sb.Append(": " + context.FirstName + " " + context.LastName);
            sb.Append(" ");
            switch (changeType)
            {
                case MetricTrac.Bll.MetricValueChangeTypeEnum.StatusChanged:
                    sb.Append("change value status from \"" + GetStatusName(OldValue.Approved) + "\" to \"" + GetStatusName(NewValue.Approved) + "\"");
                    break;
                case MetricTrac.Bll.MetricValueChangeTypeEnum.CommentToDataCollector:
                    break;
                case MetricTrac.Bll.MetricValueChangeTypeEnum.NoteChanged:
                    sb.Append("change value note from \"" + OldValue.Notes + "\" to \"" + NewValue.Notes + "\" ");
                    break;
                case MetricTrac.Bll.MetricValueChangeTypeEnum.ValueEntered:
                    sb.Append("enter value " + NewValue.Value + "\" ");
                    break;
                case MetricTrac.Bll.MetricValueChangeTypeEnum.ValueChanged:
                default:
                    sb.Append("change Value from \"" + OldValue.Value + "\" to \"" + NewValue.Value + "\" ");
                    break;
            }
            sb.Append("<br />");
            if (changeType != MetricTrac.Bll.MetricValueChangeTypeEnum.ValueChanged)
                sb.Append("Comment to Data Collector: " + HttpUtility.HtmlEncode(message) + "<br />");
            else
                sb.Append("Note: " + message + "<br />");
            if (e != null)
                sb.Append("Data Collector: " + e.FullName);
            sb.Append("<br />");
            if (NewValue != null)
            {
                sb.Append("Metric Value:<br />");
                sb.Append("Metric - " + NewValue.MetricName + "<br />");
                sb.Append("Org Location - " + NewValue.OrgLocationFullName + "<br />");
                sb.Append("Date - " + NewValue.Period + "<br />");
                sb.Append("Value - " + NewValue.Value + " " + NewValue.ValueInputUnitOfMeasureName + "<br />");
            }
            return sb.ToString();
        }

        public static string BuildEmailBody(Bll.MetricValue.Extend OldMetricValue, Bll.MetricValue.Extend NewMetricValue, string message, Micajah.Common.Security.UserContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<!DOCTYPE HTML PUBLIC\"-//IETF//DTD HTML//EN\"><html xmlns=\"http://www.w3.org/1999/xhtml\">");
            sb.Append("<head><title></title><style type=\"text/css\">body {font-family: Arial;font-size:12pt; }table {font-family: Arial;} .TableValues { text-align:left; font-weight:bolder;  } .TableHeaders { text-align:right; }</style></head>");
            sb.Append("<body>");
            if (context != null)
                sb.Append("<h3>Data Approver: " + context.FirstName + " " + context.LastName + " (" + context.Email + ")</h3> ");
            if (OldMetricValue.Approved != NewMetricValue.Approved)
                sb.Append("change value status from \"" + GetStatusName(OldMetricValue.Approved) + "\" to \"" + GetStatusName(NewMetricValue.Approved) + "\"");
            else
                sb.Append("add comment for value");
            sb.Append("<br /><br />");
            sb.Append("Comment to Data Collector: " + HttpUtility.HtmlEncode(message) + "<br /><br />");
            if (NewMetricValue != null)
            {
                sb.Append("Metric Value:<br /><table cellpadding=\"0\" cellspacing=\"5\" border=\"0\">");
                sb.Append("<tr><th style=\"text-align:right;\">Metric</th><td style=\"text-align:left;\">" + NewMetricValue.MetricName + "</td></tr>");
                sb.Append("<tr><th style=\"text-align:right;\">Org Location</th><td style=\"text-align:left;\">" + NewMetricValue.OrgLocationFullName + "</td></tr>");
                sb.Append("<tr><th style=\"text-align:right;\">Date</th><td style=\"text-align:left;\">" + NewMetricValue.Period + "</td></tr>");
                sb.Append("<tr><th style=\"text-align:right;\">Value</th><td style=\"text-align:left;\">" + NewMetricValue.Value + " " + NewMetricValue.ValueInputUnitOfMeasureName + "</td></tr>");
                sb.Append("</table>");
            }
            sb.Append("</body>");
            sb.Append("</html>");
            return sb.ToString();
        }

        private static string GetStatusName(bool? Approved)
        {
            return Approved == null ? "Under Review" : ((bool)Approved ? "Approved" : "Pending");
        }
    }
}
