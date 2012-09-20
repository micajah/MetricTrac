﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class MetricFormula
    {
        public class Extend : MetricFormula
        {
            public string UserName { get; set; }
            public DateTime ChangeDate { get; set; }
        }
        public static List<Extend> GetFormulaHistory(Guid MetricID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            IQueryable<Extend> flist =
                (from f in dc.MetricFormula
                join _u in dc.Mc_User on f.UpdatedBy equals _u.UserId into __u
                from u in __u.DefaultIfEmpty()
                where
                    f.InstanceId == LinqMicajahDataContext.InstanceId &&
                    f.MetricID == MetricID &&
                    f.Status == true
                orderby f.Created descending,  f.BeginDate, f.EndDate
                select new Extend
                {
                    InstanceId = f.InstanceId,
                    MetricFormulaID = f.MetricFormulaID,
                    MetricID = f.MetricID,
                    Formula = f.Formula,
                    VariableFormula = f.VariableFormula,
                    BeginDate = f.BeginDate,
                    EndDate = f.EndDate,
                    Created = f.Created,
                    Updated = f.Updated,
                    Status = f.Status,
                    UpdatedBy = f.UpdatedBy,
                    Comment = f.Comment,
                    UserName = f.UpdatedBy == null ? String.Empty : u.FirstName + " " + u.LastName,
                    ChangeDate = f.Updated == null ? f.Created : (DateTime)f.Updated
                }).OrderByDescending(r=>r.ChangeDate);

            return flist.ToList();
        }
    }
}
