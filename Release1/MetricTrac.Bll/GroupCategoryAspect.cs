using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace MetricTrac.Bll
{
    public partial class GroupCategoryAspect
    {
        public class Extend : GroupCategoryAspect
        {
            public string FullName { get; set; }
            public bool IsVirtual { get; set; }
        }

        public static IQueryable<GroupCategoryAspect> SelectAll()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            IQueryable<GroupCategoryAspect> ret =

                from groupCategoryAspect in dc.GroupCategoryAspect
                orderby groupCategoryAspect.ParentId, groupCategoryAspect.Name
                where groupCategoryAspect.InstanceId == LinqMicajahDataContext.InstanceId && groupCategoryAspect.Status == true
                select groupCategoryAspect;

            return ret;
        }

        public static IQueryable<D_GroupCategoryAspect> D_SelectAll()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IQueryable<D_GroupCategoryAspect> ret =
                from groupCategoryAspect in dc.D_GroupCategoryAspect                
                where groupCategoryAspect.InstanceId == LinqMicajahDataContext.InstanceId
                select groupCategoryAspect;
            return ret;
        }

        public static Guid Insert(GroupCategoryAspect newGCA)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {                
                dc.GroupCategoryAspect.InsertOnSubmit(newGCA);
                dc.SubmitChanges();
                return newGCA.GroupCategoryAspectID;
            }
        }

        public static void Delete(Guid itemID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {                
                GroupCategoryAspect ret =
                    (from groupCategoryAspect in dc.GroupCategoryAspect
                     orderby groupCategoryAspect.Name
                     where (groupCategoryAspect.InstanceId == LinqMicajahDataContext.InstanceId) && (groupCategoryAspect.GroupCategoryAspectID == itemID)
                     select groupCategoryAspect).FirstOrNull();
                if (ret != null)
                {
                    ret.Status = false;             
                    DeleteChildItems(dc, itemID);
                    dc.SubmitChanges();
                }                
            }
        }

        private static void DeleteChildItems(LinqMicajahDataContext dc, Guid _ParentID)
        {
            IQueryable<GroupCategoryAspect> rets =
                from groupCategoryAspect in dc.GroupCategoryAspect
                orderby groupCategoryAspect.Name
                where groupCategoryAspect.InstanceId == LinqMicajahDataContext.InstanceId && groupCategoryAspect.Status == true && groupCategoryAspect.ParentId == _ParentID
                select groupCategoryAspect;
            foreach (GroupCategoryAspect gca in rets)
            {
                gca.Status = false;             
                DeleteChildItems(dc, gca.GroupCategoryAspectID); 
            }
        }

        public static void Update(Guid itemID, string text)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                GroupCategoryAspect ret =
                    (from groupCategoryAspect in dc.GroupCategoryAspect
                    orderby groupCategoryAspect.Name
                     where (groupCategoryAspect.InstanceId == LinqMicajahDataContext.InstanceId) && (groupCategoryAspect.GroupCategoryAspectID == itemID)
                    select groupCategoryAspect).FirstOrNull();

                if (ret != null)
                {
                    ret.Name = text;                    
                    dc.SubmitChanges();
                }
            }
        }

        public static IQueryable<GroupCategoryAspect> SelectGroups()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            IQueryable<GroupCategoryAspect> ret =

                from groupCategoryAspect in dc.GroupCategoryAspect
                orderby groupCategoryAspect.Name
                where groupCategoryAspect.InstanceId == LinqMicajahDataContext.InstanceId && groupCategoryAspect.Status == true
                        && groupCategoryAspect.ParentId == null
                select groupCategoryAspect;

            return ret;
        }

        private class GuidPair
        {
            public Guid SourceGuid = Guid.Empty;
            public Guid DestGuid = Guid.Empty;
        }

        public static void Merge(Guid SourceID, Guid DestID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                MergeGCA(dc, SourceID, DestID);                
            }
        }

        private static void MergeGCA(LinqMicajahDataContext dc, Guid SourceID, Guid DestID)
        {
            IQueryable<GuidPair> Pairs =
                 from gca1 in dc.GroupCategoryAspect
                 join gca2 in dc.GroupCategoryAspect
                    on new { gca1.InstanceId, gca1.Name }
                    equals new { gca2.InstanceId, gca2.Name }
                 where
                    (gca1.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (gca2.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (gca1.ParentId == SourceID)
                    &&
                    (gca2.ParentId == DestID)
                    &&
                    gca1.Status == true
                    &&
                    gca2.Status == true
                 select new GuidPair
                 {
                     SourceGuid = gca1.GroupCategoryAspectID,
                     DestGuid = gca2.GroupCategoryAspectID
                 };
            foreach (GuidPair gp in Pairs)
                MergeGCA(dc, gp.SourceGuid, gp.DestGuid);

            // main merge
            IQueryable<GroupCategoryAspect> SourceGCAs =
                 from gca in dc.GroupCategoryAspect                 
                 where
                    (gca.InstanceId == LinqMicajahDataContext.InstanceId)                    
                    &&
                    (gca.ParentId == SourceID)                    
                 select gca;
            foreach (GroupCategoryAspect _gca in SourceGCAs)
                _gca.ParentId = DestID;

            GroupCategoryAspect SourceGCA =
                 (from gca in dc.GroupCategoryAspect
                  where
                     (gca.InstanceId == LinqMicajahDataContext.InstanceId)
                     &&
                     (gca.GroupCategoryAspectID == SourceID)
                  select gca).FirstOrNull();
            if (SourceGCA != null)
                SourceGCA.Status = false;
            dc.SubmitChanges();
        }

        public static void Copy(Guid SourceID, Guid DestID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                CopyGCA(dc, SourceID, DestID);
            }
        }

        private static void CopyGCA(LinqMicajahDataContext dc, Guid SourceID, Guid DestID)
        {
            GroupCategoryAspect SourceGCA =
                 (from gca in dc.GroupCategoryAspect
                  where
                     (gca.InstanceId == LinqMicajahDataContext.InstanceId)
                     &&
                     (gca.GroupCategoryAspectID == SourceID)
                     &&
                     (gca.Status == true)
                  select gca).FirstOrNull();
            if (SourceGCA != null)
            {
                GroupCategoryAspect cSource = new GroupCategoryAspect();
                cSource.CopyFrom(SourceGCA);
                cSource.ParentId = DestID;
                dc.GroupCategoryAspect.InsertOnSubmit(cSource);
                dc.SubmitChanges();

                IQueryable<GroupCategoryAspect> SourceGCAs =
                 from gca in dc.GroupCategoryAspect
                 where
                    (gca.InstanceId == LinqMicajahDataContext.InstanceId)
                    &&
                    (gca.ParentId == SourceID)
                    &&
                    (gca.Status == true)
                 select gca;
                foreach (GroupCategoryAspect gca in SourceGCAs)
                    CopyGCA(dc, gca.GroupCategoryAspectID, cSource.GroupCategoryAspectID);             
            }
        }

        public static void Move(Guid SourceID, Guid DestID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                GuidPair Pair =
                 (from gca1 in dc.GroupCategoryAspect
                  join gca2 in dc.GroupCategoryAspect
                     on new { gca1.InstanceId, gca1.Name }
                     equals new { gca2.InstanceId, gca2.Name }
                  where
                     (gca1.InstanceId == LinqMicajahDataContext.InstanceId)
                     &&
                     (gca2.InstanceId == LinqMicajahDataContext.InstanceId)
                     &&
                     (gca1.GroupCategoryAspectID == SourceID)
                     &&
                     (gca2.ParentId == DestID)
                     &&
                     gca1.Status == true
                     &&
                     gca2.Status == true
                  select new GuidPair
                  {
                      SourceGuid = gca1.GroupCategoryAspectID,
                      DestGuid = gca2.GroupCategoryAspectID
                  }).FirstOrNull();
                if (Pair == null)
                {
                    GroupCategoryAspect SourceGCA =
                     (from gca in dc.GroupCategoryAspect
                      where
                         (gca.InstanceId == LinqMicajahDataContext.InstanceId)
                         &&
                         (gca.GroupCategoryAspectID == SourceID)
                      select gca).FirstOrNull();
                    if (SourceGCA != null)
                        SourceGCA.ParentId = DestID;
                    dc.SubmitChanges();
                }
                else
                    if (SourceID != Pair.DestGuid)
                        MergeGCA(dc, SourceID, Pair.DestGuid);// merge                
            }
        }
    }
}
