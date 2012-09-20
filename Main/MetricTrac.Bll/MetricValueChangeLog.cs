using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public enum MetricValueChangeTypeEnum
    {
        ValueChanged = 1,
        NoteChanged,
        StatusChanged,
        CommentToDataCollector,
        ValueEntered
    }

    public partial class MetricValueChangeLog
    {
        public class Extend : MetricValueChangeLog
        {
            public string UserName { get; set; }
            public string TypeName { get; set; }
        }

        public static void LogChange(LinqMicajahDataContext dc, Guid MetricValueID, MetricValueChangeTypeEnum Type, string OldValue, string NewValue, string ChangeMessage)
        {   
            MetricValueChangeLog change = new MetricValueChangeLog();
            change.OldValue = OldValue;
            change.NewValue = NewValue;
            change.Event = ChangeMessage;
            change.MetricValueID = MetricValueID;
            change.MetricValueChangeTypeID = (int)Type;
            change.CreatedTime = DateTime.Now;
            change.UserId = Micajah.Common.Security.UserContext.Current.UserId;
            dc.MetricValueChangeLog.InsertOnSubmit(change);
            dc.SubmitChanges();
        }

        public static void LogChange(Guid MetricValueID, MetricValueChangeTypeEnum Type, string OldValue, string NewValue, string ChangeMessage)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                MetricValueChangeLog change = new MetricValueChangeLog();
                change.OldValue = OldValue;
                change.NewValue = NewValue;
                change.Event = ChangeMessage;
                change.MetricValueID = MetricValueID;
                change.MetricValueChangeTypeID = (int)Type;
                change.CreatedTime = DateTime.Now;
                change.UserId = Micajah.Common.Security.UserContext.Current.UserId;
                dc.MetricValueChangeLog.InsertOnSubmit(change);
                dc.SubmitChanges();                
            }
        }
        
        public static List<Extend> GetLog(Guid MetricValueID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IQueryable<Extend> flist =
                from l in dc.MetricValueChangeLog
                 join t in dc.MetricValueChangeType on l.MetricValueChangeTypeID equals t.MetricValueChangeTypeID
                 join u in dc.Mc_User on l.UserId equals u.UserId
                 where
                     l.InstanceId == LinqMicajahDataContext.InstanceId &&
                     l.MetricValueID == MetricValueID
                 orderby l.CreatedTime descending, l.MetricValueChangeTypeID
                 select new Extend
                 {
                     InstanceId = l.InstanceId,
                     MetricValueChangeLogID = l.MetricValueChangeLogID,
                     MetricValueID = l.MetricValueID,
                     Event = l.Event,
                     OldValue = l.OldValue,
                     NewValue = l.NewValue,
                     CreatedTime = l.CreatedTime,
                     MetricValueChangeTypeID = l.MetricValueChangeTypeID,
                     UserId = l.UserId,
                     UserName = u.FirstName + " " + u.LastName,
                     TypeName = t.Name
                 };

            return flist.ToList();
        }
    }
}
