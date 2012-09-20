using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Data.Linq;

namespace MetricTrac.Bll
{
    public partial class ScoreCard
    {
        public class Extend : ScoreCard
        {
            public string CreateUserName { get; set; }
            public System.Collections.IList MetricValue { get; set; }
        }

        static public List<Extend> List()
        {
            return List(LinqMicajahDataContext.InstanceId, null, null, null, null, null, null);
        }

        /*static public List<Extend> List(Guid ScoreCardInstaceId)
        {
            return List(ScoreCardInstaceId, DateTime.Today, null, null);
        }*/

        static public List<Extend> List(DateTime? BeginDate, DateTime? EndDate, DateTime? BeginCompDate, DateTime? EndCompDate, MetricTrac.Bll.ScoreCardMetric.CalcStringFormula Calculator)
        {
            return List(LinqMicajahDataContext.InstanceId, BeginDate, EndDate, BeginCompDate, EndCompDate, LinqMicajahDataContext.LogedUserId, Calculator);
        }

        static public List<Extend> List(Guid ScoreCardInstaceId, DateTime? BeginDate, DateTime? EndDate, DateTime? BeginCompDate, DateTime? EndCompDate, Guid? CurrentUserId, MetricTrac.Bll.ScoreCardMetric.CalcStringFormula Calculator)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var r = from c in dc.ScoreCard                    
                    join _u in dc.Mc_User on new { c.UserId, Deleted = false } equals new { UserId = (Guid?)_u.UserId, _u.Deleted } into __u
                    from u in __u.DefaultIfEmpty()
                    join _h in dc.ScoreCardHidden on new { c.InstanceId, c.ScoreCardID, CurrentUserId  } equals new { _h.InstanceId, _h.ScoreCardID, CurrentUserId = (Guid?)_h.UserId } into __h
                    from h in __h.DefaultIfEmpty()
                    where c.InstanceId == ScoreCardInstaceId &&
                        c.Status == true &&
                        ((CurrentUserId == c.UserId && c.UserId != null) || (c.IsPublic && h.ScoreCardHiddenID == null) || CurrentUserId == null)
                    orderby ((CurrentUserId == c.UserId && c.UserId != null) ? 1 : 2), (c.UserId == null ? 2 : 1), u.LastName, u.FirstName
                    select new Extend
                    {
                        InstanceId = c.InstanceId,
                        ScoreCardID = c.ScoreCardID,
                        Name = c.Name,
                        Description = c.Description,
                        Status = c.Status,
                        Created = c.Created,
                        Updated = c.Updated,
                        UserId = c.UserId,
                        IsPublic = c.IsPublic,
                        CreateUserName = ((u.FirstName == null) ? String.Empty : u.FirstName + " ") + ((u.LastName == null) ? String.Empty : u.LastName)
                    };

            var l = r.ToList();

            foreach (Extend cs in l)
                cs.MetricValue = MetricTrac.Bll.ScoreCardMetric.List(cs.ScoreCardID, BeginDate, EndDate, BeginCompDate, EndCompDate, dc, ScoreCardInstaceId, Calculator);
            return l;
        }

        static public List<Extend> ListMyDashBoard(DateTime? Date, Bll.ScoreCardMetric.CalcStringFormula csf)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            List<Extend> r = new List<Extend>();
            r.Add(new Extend()
                    {
                        InstanceId = LinqMicajahDataContext.InstanceId,
                        ScoreCardID = Guid.Empty,
                        Name = "My Dasboard",
                        MetricValue = MetricTrac.Bll.ScoreCardDashboard.List(Date, csf)
                    }
                );

            return r;
        }

        public static ScoreCard Get(Guid ScoreCardID, Guid CardInstanceId)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            return (
                from c in dc.ScoreCard 
                where c.InstanceId==CardInstanceId && c.ScoreCardID==ScoreCardID && c.Status==true 
                select c
                ).FirstOrNull();
        }

        // Hidden ScoreCards
        public static void HideScoreCard(Guid ScoreCardId)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                ScoreCardHidden sch = new ScoreCardHidden();
                sch.ScoreCardID = ScoreCardId;
                sch.UserId = (Guid)LinqMicajahDataContext.LogedUserId;
                sch.Created = DateTime.Now;
                dc.ScoreCardHidden.InsertOnSubmit(sch);
                dc.SubmitChanges();
            }
        }

        public static void ViewAllScoreCards()
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {                
                IQueryable<ScoreCardHidden> hiddens = from sch in dc.ScoreCardHidden where sch.UserId == LinqMicajahDataContext.LogedUserId && sch.InstanceId == LinqMicajahDataContext.InstanceId select sch;
                foreach (ScoreCardHidden s in hiddens)
                    dc.ScoreCardHidden.DeleteOnSubmit(s);
                dc.SubmitChanges();
            }
        }

        public static void HideAllPublicScoreCards()
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var r = from c in dc.ScoreCard                        
                        join _h in dc.ScoreCardHidden on new { c.InstanceId, c.ScoreCardID, CurrentUserId = LinqMicajahDataContext.LogedUserId } equals new { _h.InstanceId, _h.ScoreCardID, CurrentUserId = (Guid?)_h.UserId } into __h
                        from h in __h.DefaultIfEmpty()
                        where c.InstanceId == LinqMicajahDataContext.InstanceId && c.Status == true &&
                            (c.IsPublic && h.ScoreCardHiddenID == null && (LinqMicajahDataContext.LogedUserId != c.UserId || c.UserId == null))                        
                        select new Extend
                        {
                            InstanceId = c.InstanceId,
                            ScoreCardID = c.ScoreCardID
                        };

                var l = r.ToList();

                foreach (Extend cs in l)
                {
                    ScoreCardHidden sch = new ScoreCardHidden();
                    sch.ScoreCardID = cs.ScoreCardID;
                    sch.UserId = (Guid)LinqMicajahDataContext.LogedUserId;
                    sch.Created = DateTime.Now;
                    dc.ScoreCardHidden.InsertOnSubmit(sch);
                }
                dc.SubmitChanges();
            }
        }

        public static int HiddenScoreCardsCount()
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                IQueryable<ScoreCardHidden> hiddens = from sch in dc.ScoreCardHidden where sch.UserId == LinqMicajahDataContext.LogedUserId && sch.InstanceId == LinqMicajahDataContext.InstanceId select sch;
                return hiddens.Count();
            }
        }
    }
}
