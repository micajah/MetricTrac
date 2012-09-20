using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MetricTrac.Bll
{
    public partial class DataViewList
    {

        public class Extend : DataViewList
        {
            public List<string> SelectList { get; set; }
            public List<string> OrderByList { get; set; }
            public List<string> GroupByList { get; set; }

            public List<DataViewWhere.Extend> WhereList { get; set; }

            public List<string> SelectSlaveList { get; set; }
            public List<string> OrderBySlaveList { get; set; }
            public List<string> GroupBySlaveList { get; set; }

            public List<DataViewWhere.Extend> WhereSlaveList { get; set; }
        }

        public static List<DataViewList> List(Guid DataViewTypeID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var dv = from v in dc.DataViewList where v.InstanceId == LinqMicajahDataContext.InstanceId && v.Status == true && v.DataViewTypeID==DataViewTypeID select v;
            return dv.ToList();
        }

        private static List<string> GetColumnList(LinqMicajahDataContext dc, int DataViewColumnListTypeID, Guid DataViewListID)
        {
            var rr = (
                    from c in dc.DataViewColumn
                    where c.DataViewListID == DataViewListID &&
                        c.Status == true &&
                        c.InstanceId == LinqMicajahDataContext.InstanceId &&
                        c.DataViewColumnListTypeID == DataViewColumnListTypeID
                    orderby c.OrderNumber
                    select c
                    ).ToList();

            List<string> l = new List<string>();
            foreach(var r in rr)
            {
                l.Add(r.TableName+","+r.ColumnName);
            }
            return l;
        }

        public static DataViewList.Extend Get(Guid DataViewListID)
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var dv = (
                from v in dc.DataViewList
                where v.InstanceId == LinqMicajahDataContext.InstanceId &&
                    v.Status == true &&
                    v.DataViewListID == DataViewListID
                select new Extend()
                {
                    DataViewListID = v.DataViewListID,
                    DataViewTypeID = v.DataViewTypeID,
                    Description = v.Description,
                    InstanceId = v.InstanceId,
                    Name = v.Name,
                    Slave = v.Slave,
                    WhereCondition = v.WhereCondition,
                    WhereConditionSlave = v.WhereConditionSlave
                }
                ).FirstOrNull();
            if (dv != null)
            {
                dv.SelectList = GetColumnList(dc, 1, DataViewListID);
                dv.GroupByList = GetColumnList(dc, 3, DataViewListID);
                dv.OrderByList = GetColumnList(dc, 4, DataViewListID);

                dv.WhereList = MetricTrac.Bll.DataViewWhere.List(DataViewListID, 2);

                dv.SelectSlaveList = GetColumnList(dc, 11, DataViewListID);
                dv.GroupBySlaveList = GetColumnList(dc, 13, DataViewListID);
                dv.OrderBySlaveList = GetColumnList(dc, 14, DataViewListID);

                dv.WhereSlaveList = MetricTrac.Bll.DataViewWhere.List(DataViewListID, 12);
            }
            return dv;
        }

        

        public static void SaveColumns(Guid DataViewTypeID, List<string> TableColumns, int DataViewColumnListTypeID, Guid DataViewListID)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var NeedDelColumns = dc.DataViewColumn.Where(c => c.DataViewColumnListTypeID == DataViewColumnListTypeID && c.Status == true && c.InstanceId == LinqMicajahDataContext.InstanceId && c.DataViewListID == DataViewListID).ToList();
                dc.DataViewColumn.DeleteAllOnSubmit(NeedDelColumns);

                for(int i=0;i<TableColumns.Count;i++)
                {
                    string NeedInsertColumn = TableColumns[i];
                    string [] ss = NeedInsertColumn.Split(',');
                    DataViewColumn c = new DataViewColumn()
                    {
                        TableName = ss[0],
                        ColumnName = ss[1],
                        DataViewListID = DataViewListID,
                        DataViewColumnListTypeID = DataViewColumnListTypeID,
                        OrderNumber = i
                    };
                    dc.DataViewColumn.InsertOnSubmit(c);
                }

                dc.SubmitChanges();
            }
        }

        public override void Delete(LinqDBDataContext dcOld)
        {
            using (LinqMicajahDataContext dc = new LinqMicajahDataContext())
            {
                var dvs = dc.DataViewList.Where(v => v.InstanceId == InstanceId && v.DataViewListID == DataViewListID && v.Status == true);
                foreach (var dv in dvs) dv.Status = false;

                var cs = dc.DataViewColumn.Where(c => c.InstanceId == InstanceId && c.Status == true && c.DataViewListID == DataViewListID);
                foreach (var c in cs) c.Status = false;

                dc.SubmitChanges();
            }
        }
    }
}
