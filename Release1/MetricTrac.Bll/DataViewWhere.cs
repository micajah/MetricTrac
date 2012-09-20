using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class DataViewWhere
    {
        public class Extend : DataViewWhere
        {
            public string TableName { get; set; }
            public string ColumnName { get; set; }
            public int OrderNumber { get; set; }
        }

        public static List<Extend> List(Guid DataViewListID, int DataViewColumnListTypeID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            /*WhereCondition = null;
            Slave = false;
            var dv = (from l in dc.DataViewList
                      where l.InstanceId == LinqMicajahDataContext.InstanceId && l.Status == true &&
                          l.DataViewListID == DataViewListID
                      select l).FirstOrNull();
            if (dv == null) return null;

            WhereCondition = dv.WhereCondition;
            Slave = dv.Slave;*/

            var r = (from c in dc.DataViewColumn
                     join w in dc.DataViewWhere on
                         new { LinqMicajahDataContext.InstanceId, Status = (bool?)true, c.DataViewColumnID } equals
                         new { w.InstanceId, w.Status, w.DataViewColumnID }
                     where c.InstanceId == LinqMicajahDataContext.InstanceId &&
                         c.Status == true && c.DataViewListID == DataViewListID && 
                         c.DataViewColumnListTypeID == DataViewColumnListTypeID &&
                         w.Slave == (DataViewColumnListTypeID!=2)
                     select new Extend()
                     {
                         TableName = c.TableName,
                         ColumnName = c.ColumnName,
                         CompareValue = w.CompareValue,
                         DataViewColumnID = c.DataViewColumnID,
                         DataViewConditionTypeID = w.DataViewConditionTypeID,
                         DataViewWhereID = w.DataViewWhereID,
                         OrderNumber = c.OrderNumber
                     }).ToList();
            return r;
        }

        public static void SaveWhere(Guid DataViewTypeID, List<Extend> WhereCriteria, string Condition, int DataViewColumnListTypeID, Guid DataViewListID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var wl = (from l in dc.DataViewList
                         where l.InstanceId == LinqMicajahDataContext.InstanceId && l.Status == true &&
                            l.DataViewListID == DataViewListID
                         select l).FirstOrNull();

                if (wl == null) return;
                wl.WhereCondition = Condition;

                var ColumnForDel = (from c in dc.DataViewColumn
                             where c.InstanceId == LinqMicajahDataContext.InstanceId && c.Status == true &&
                             c.DataViewColumnListTypeID == DataViewColumnListTypeID && c.DataViewListID == DataViewListID
                             select c).ToList();
                var ColumnForDelID = ColumnForDel.Select(c=>c.DataViewColumnID).ToList();
                var WhereForDel = from w in dc.DataViewWhere
                                  where w.InstanceId == LinqMicajahDataContext.InstanceId && w.Status == true &&
                                    w.DataViewListID == DataViewListID && w.Slave== (DataViewColumnListTypeID!=2)
                                  select w;
                dc.DataViewColumn.DeleteAllOnSubmit(ColumnForDel);
                dc.DataViewWhere.DeleteAllOnSubmit(WhereForDel);

                foreach (var w in WhereCriteria)
                {
                    DataViewColumn NewColumn = new DataViewColumn()
                    {
                        TableName = w.TableName,
                        ColumnName = w.ColumnName,
                        DataViewListID = DataViewListID,
                        DataViewColumnListTypeID = DataViewColumnListTypeID,
                        InstanceId = LinqMicajahDataContext.InstanceId,
                        OrderNumber = WhereCriteria.IndexOf(w),
                        DataViewColumnID = Guid.NewGuid()
                    };
                    dc.DataViewColumn.InsertOnSubmit(NewColumn);

                    DataViewWhere NewWhere = new DataViewWhere()
                    {
                        DataViewListID = DataViewListID,
                        DataViewColumnID = NewColumn.DataViewColumnID,
                        DataViewConditionTypeID = w.DataViewConditionTypeID,
                        CompareValue = w.CompareValue,
                        Slave = (DataViewColumnListTypeID!=2)
                    };
                    dc.DataViewWhere.InsertOnSubmit(NewWhere);
                }

                dc.SubmitChanges();
            }
        }
    }
}
