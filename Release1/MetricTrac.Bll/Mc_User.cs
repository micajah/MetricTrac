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
                   join f in dc.UserFullNameView on new { LinqMicajahDataContext.InstanceId, u.UserId } equals new {f.InstanceId, f.UserId }
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

        public static Extend GetValueInputUser(Guid ValueID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            return GetValueInputUser(dc, ValueID);
        }

        public static Extend GetValueInputUser(LinqMicajahDataContext dc, Guid ValueID)
        {
            Extend e = 
                    (from v in dc.MetricValue
                    join u in dc.Mc_User on new { v.InstanceId, UserId = (v.InputUserId == null ? Guid.Empty : (Guid)v.InputUserId)} equals new { LinqMicajahDataContext.InstanceId, u.UserId }
                    join f in dc.UserFullNameView on new { LinqMicajahDataContext.InstanceId, u.UserId } equals new { f.InstanceId, f.UserId }
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
    }
}
