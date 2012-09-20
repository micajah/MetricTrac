using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class Mc_User
    {
        public class Extend : Mc_User
        {
            public string FullName { get; set; }
        }

        public static List<Extend> List()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();

            return (from u in dc.Mc_User
                    join f in dc.ViewnameUser on u.UserId equals f.UserId
                    join ou in dc.Mc_OrganizationsUsers on u.UserId equals ou.UserId
                    join i in dc.Mc_Instance on ou.OrganizationId equals i.OrganizationId
                    where
                    i.InstanceId == LinqMicajahDataContext.InstanceId
                    &&
                    u.Deleted == false
                    orderby u.FirstName, u.LastName
                    select new Extend
                    {
                        UserId = u.UserId,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        MiddleName = u.MiddleName,
                        LastLoginDate = u.LastLoginDate,
                        Deleted = u.Deleted,
                        FullName = u.FirstName + ' ' + u.LastName//FullName = f.FullName - with email
                    }).ToList();
        }

        public static Extend GetValueInputUser(LinqMicajahDataContext dc, Guid ValueID)
        {            
            Extend e =
                    (from v in dc.MetricValue
                     join u in dc.Mc_User on new { v.InstanceId, UserId = (v.InputUserId == null ? Guid.Empty : (Guid)v.InputUserId) } equals new { LinqMicajahDataContext.InstanceId, u.UserId }
                     join f in dc.ViewnameUser on u.UserId equals f.UserId
                     where
                         v.MetricValueID == ValueID
                     select new Extend
                     {
                         UserId = u.UserId,
                         Email = u.Email,
                         FirstName = u.FirstName,
                         LastName = u.LastName,
                         MiddleName = u.MiddleName,
                         LastLoginDate = u.LastLoginDate,
                         Deleted = u.Deleted,
                         FullName = u.FirstName + ' ' + u.LastName
                     }).FirstOrNull();
            return e;
        }

        public static Extend GetValueInputUser(Guid ValueID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            return GetValueInputUser(dc, ValueID);
        }
    }
}
