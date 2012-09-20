using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.Configuration;
using System.Web.Configuration;
using System.Net.Configuration;
using System.Text;
using System.Data;

namespace MetricTrac.Utils
{
    public interface IValueSelectControl
    {
        object SelectedValue { get; set; }
        bool IsValueSelected { get; }
    }

    public class DataViewConfig
    {
        public class ReferenceEntity
        {
            public string ReadableName { get; set; }
            public string SqlName { get; set; }
            public Guid EntityID { get; set; }
            public string SqlNameFieldName { get; set; }
            public string Alias { get; set; }

            public class UseField
            {
                public string ReadableName { get; set; }
                public string SqlName { get; set; }
                public string FullColumnName { get; set; }
                public string SelectControl { get; set; }
                public ReferenceEntity Entity { get; set; }
                public string NODecPlaces  { get; set; }
            };
            public List<UseField> UseFieldList { get; set; }
            public bool OneToMany { get; set; }
            public string ReferenceTable { get; set; }
        }

        public List<ReferenceEntity> ReferenceEntityList;
        public string Name;
        public Guid BaseEntityID;
        public Guid DataViewTypeID;


        private DataViewConfig(string name, Guid baseEntityID, Guid dataViewTypeID)
        {
            Name = name;
            BaseEntityID = baseEntityID;
            DataViewTypeID = dataViewTypeID;
        }

        private static Dictionary<Guid, DataViewConfig> DataViewConfigTypes;
        public static DataViewConfig Get(Guid DataViewTypeID)
        {
            if (DataViewConfigTypes == null) DataViewConfigTypes = new Dictionary<Guid,DataViewConfig>();
            if (DataViewConfigTypes.Keys.Contains(DataViewTypeID)) return DataViewConfigTypes[DataViewTypeID];

            string p = System.Web.HttpContext.Current.Request.PhysicalApplicationPath;
            p = p + "Micajah.Common.config";
            DataSet ds = new System.Data.DataSet();
            ds.ReadXml(p);
            
            DataTable DataViewTable = ds.Tables["DataView"];
            DataRow [] rr = DataViewTable.Select("DataViewTypeID='"+DataViewTypeID+"'");
            if(rr==null || rr.Length<1) return null;

            string Name = (string)rr[0]["Name"];
            Guid BaseEntityID = new Guid((string)rr[0]["BaseEntityID"]);
            int DataView_Id = (int)rr[0]["DataView_Id"];

            DataTable ReferenceEntitiesTable = ds.Tables["ReferenceEntities"];
            rr = ReferenceEntitiesTable.Select("DataView_Id=" + DataView_Id);
            if (rr == null || rr.Length < 1) return null;
            int ReferenceEntities_Id = (int)rr[0]["ReferenceEntities_Id"];

            DataTable ReferenceEntityTable = ds.Tables["ReferenceEntity"];
            rr = ReferenceEntityTable.Select("ReferenceEntities_Id=" + ReferenceEntities_Id);
            if (rr == null || rr.Length < 1) return null;

            List<ReferenceEntity> rel = new List<ReferenceEntity>();
            foreach (DataRow r in rr)
            {
                ReferenceEntity re = new ReferenceEntity();                
                re.EntityID = new Guid((string)r["EntityID"]);
                re.OneToMany = ReferenceEntityTable.Columns.Contains("ReferenceType") && (r["ReferenceType"] is string) && (string)r["ReferenceType"] == "OneToMany";
                re.ReferenceTable = ReferenceEntityTable.Columns.Contains("ReferenceTable") && (r["ReferenceTable"] is string)?(string)r["ReferenceTable"]:null;
                re.Alias = r["Alias"] as string;

                DataRow[] RR = ds.Tables["entity"].Select("id='" + re.EntityID + "'");
                if (RR == null || RR.Length < 1) return null;
                DataRow EntityRow = RR[0];
                re.ReadableName = (string)EntityRow["name"];
                re.SqlName = (string)EntityRow["tableName"];
                //re.SqlNameFieldName = (string)EntityRow[];

                int entity_Id = (int)EntityRow["entity_Id"];
                RR = ds.Tables["fields"].Select("entity_Id=" + entity_Id);
                if (RR == null || RR.Length < 1) return null;
                int fields_Id = (int)RR[0]["fields_Id"];
                

                int ReferenceEntity_Id = (int)r["ReferenceEntity_Id"];
                RR = ds.Tables["UseField"].Select("ReferenceEntity_Id=" + ReferenceEntity_Id);
                if (RR == null || RR.Length < 1) return null;
                bool SelectControlExist = ds.Tables["UseField"].Columns.Contains("SelectControl");

                re.UseFieldList = new List<ReferenceEntity.UseField>();
                DataRow[] FieldsRows;
                FieldsRows = ds.Tables["field"].Select("fields_Id=" + fields_Id + " AND name='Name'");
                if (FieldsRows != null && FieldsRows.Length > 0)
                {
                    re.SqlNameFieldName = FieldsRows[0]["columnName"].ToString();
                }

                foreach (DataRow R in RR)
                {
                    ReferenceEntity.UseField uf = new ReferenceEntity.UseField();
                    uf.Entity = re;
                    uf.ReadableName = (string)R["Name"];
                    FieldsRows = ds.Tables["field"].Select("fields_Id=" + fields_Id + " AND name='"+uf.ReadableName+"'");
                    if (FieldsRows == null || FieldsRows.Length < 1) return null;
                    uf.SqlName = (string)FieldsRows[0]["columnName"];
                    uf.FullColumnName = re.SqlName + "." + uf.SqlName;
                    if (R.Table.Columns.Contains("NODecPlaces") && R["NODecPlaces"] is string) uf.NODecPlaces = (string)R["NODecPlaces"];
                    if (SelectControlExist) uf.SelectControl = R["SelectControl"] as string;
                    re.UseFieldList.Add(uf);
                }

                rel.Add(re);
            }

            DataViewConfig ret = new DataViewConfig(Name, BaseEntityID, DataViewTypeID);
            ret.ReferenceEntityList = rel;

            DataViewConfigTypes[DataViewTypeID] = ret;
            return ret;
        }


        private const string SqlSelet =
            "select * " +
            "from			Metric as m " +
            "JOIN			Frequency as f on m.FrequencyID=f.FrequencyID " +

            //"LEFT JOIN		ViewRule as r on r.MetricID=m.MetricID AND r.InstanceId=@InstanceId " +
            "LEFT JOIN		dbo.fxGetRule(@InstanceId) as r on r.MetricID=m.MetricID AND r.InstanceId=@InstanceId " +
            
            "LEFT JOIN		ViewnameOrgLocation en on en.InstanceId = r.InstanceId AND en.OrgLocationID=r.OrgLocationID " +
            "LEFT JOIN		ViewPath as p on p.MetricID=m.MetricID AND p.OrgLocationID=r.OrgLocationID AND p.InstanceId=@InstanceId " +
            //"LEFT JOIN		D_MetricOrgLocationRule as r on r.MetricID=m.MetricID AND r.InstanceId=@InstanceId " +
            //"LEFT JOIN		EntityNodeFullNameView en on en.InstanceId is NULL AND en.EntityNodeId=r.OrgLocationID " +
            //"LEFT JOIN		D_MetricOrgLocationPath as p on p.MetricID=m.MetricID AND p.OrgLocationID=r.OrgLocationID AND p.InstanceId=@InstanceId " +

            "LEFT JOIN		PerformanceIndicator as pi on pi.PerformanceIndicatorID=p.PerformanceIndicatorID AND pi.InstanceId=@InstanceId " +
            "LEFT JOIN		ViewnameGroupCategoryAspect as gca on gca.GroupCategoryAspectID=p.GroupCategoryAspectID AND gca.InstanceId=@InstanceId " +
            //"LEFT JOIN		PerformanceIndicatorForm as pif on pif.PerformanceIndicatorFormID=p.PerformanceIndicatorFormID AND pif.InstanceId=@InstanceId " +
            //"LEFT JOIN		GCAFullNameView as gca on gca.GroupCategoryAspectID=p.GroupCategoryAspectID AND gca.InstanceId=@InstanceId " +
            "LEFT JOIN      CurrentFrequencyView cf on cf.FrequencyID=m.FrequencyID " +
            "LEFT JOIN      D_PeriodDate as pd on pd.FrequencyID=m.FrequencyID AND pd.PeriodNumber>=cf.PeriodNumber AND pd.PeriodNumber<=cf.PeriodNumber+6 " +
            "LEFT JOIN      MetricValue as v on v.MetricID=m.MetricID AND v.Date=pd.Date AND v.Status=1 AND v.InstanceId=@InstanceId " +
            "WHERE m.Status=1 AND m.InstanceId=@InstanceId";

        private static void AddSelect(ref string sql, string Field, List<string> TotalSelect)
        {
            if (TotalSelect.Contains(Field)) return;
            string f = Field + " as " + Field.Replace(".", "_");
            if (sql.Contains("select * ")) sql = sql.Replace("select * ", "select distinct " + f + " ");
            else sql = sql.Replace(" from", ", " + f + " from");
            TotalSelect.Add(Field);
        }

        private static void AddOrderBy(ref string sql, string Field, List<string> TotalOrderBy)
        {
            if (TotalOrderBy.Contains(Field)) return;
            if (sql.Contains("ORDER BY ")) sql += ", ";
            else sql += "ORDER BY ";
            sql += Field;
            TotalOrderBy.Add(Field);
        }

        public ReferenceEntity.UseField GetField(string s)
        {
            string[] ss = s.Split(',');
            if (ss.Length != 2) return null;
            foreach (var t in ReferenceEntityList)
            {
                if (t.SqlName != ss[0]) continue;
                foreach (var f in t.UseFieldList)
                {
                    if (f.SqlName != ss[1]) continue;
                    return f;
                }
                return null;
            }
            return null;
        }

        string GetSel(ReferenceEntity.UseField column)
        {
            string sel = column.Entity.Alias + "." + column.SqlName;
            if (sel == "v.Date") sel = "pd.ShortName";
            return sel;
        }
        string GetAlias(string column)
        {
            string a = column.Replace('.','_');
            if (a == "v_Date") a = "pd_ShortName";
            return a;
        }

        string GetAlias(ReferenceEntity.UseField column)
        {
            string a = column.Entity.Alias + "_" + column.SqlName;
            if (a == "v_Date") a = "pd_ShortName";
            return a;
        }

        List<ReferenceEntity.UseField> GetGroupByFieldList(ref string sql, List<string> TotalOrderBy, List<string> TotalSelect, List<string> dvGroupByList)
        {
            List<ReferenceEntity.UseField> GroupByFieldList = new List<ReferenceEntity.UseField>();
            if (dvGroupByList != null && dvGroupByList.Count > 0)
            {
                foreach (var g in dvGroupByList)
                {
                    var column = GetField(g);
                    if (column == null) continue;

                    GroupByFieldList.Add(column);
                    string ob = GetSel(column);
                    AddOrderBy(ref sql, ob, TotalOrderBy);
                    AddSelect(ref sql, ob, TotalSelect);
                }
            }
            return GroupByFieldList;

        }

        List<ReferenceEntity.UseField> GetSelectFieldList(ref string sql, List<string> TotalOrderBy, List<string> TotalSelect, List<string> dvSelectList)
        {
            List<ReferenceEntity.UseField> SelectFieldList = new List<ReferenceEntity.UseField>();
            if (dvSelectList != null && dvSelectList.Count > 0)
            {
                foreach (var s in dvSelectList)
                {
                    var column = GetField(s);
                    if (column == null) continue;
                    SelectFieldList.Add(column);
                    string sel = GetSel(column);
                    AddOrderBy(ref sql, sel, TotalOrderBy);
                    AddSelect(ref sql, sel, TotalSelect);
                    if (!string.IsNullOrEmpty(column.NODecPlaces)) AddSelect(ref sql, column.NODecPlaces, TotalSelect);
                }
            }
            return SelectFieldList;
        }

        bool ProcessGroup(ReferenceEntity.UseField g, int i, DataRow r, List<string> OldMasterGroup, out string GroupValue, out string GroupHeader)
        {
            string alias = GetAlias(g);
            GroupValue = r[alias].ToString();
            GroupHeader = g.Entity.ReadableName;
            if (g.ReadableName != "Name") GroupHeader += " " + g.ReadableName;
            bool IsNewGroup = OldMasterGroup[i] != GroupValue;
            OldMasterGroup[i] = GroupValue;
            return IsNewGroup;
        }

        private string GetValue(ReferenceEntity.UseField column, DataRow r)
        {
            string val = r[GetAlias(column)].ToString();
            if (string.IsNullOrEmpty(column.NODecPlaces)) return val;
            decimal dv;
            if (!decimal.TryParse(val,  System.Globalization.NumberStyles.Any, null, out dv)) return val;
            string NODecPlacesString = r[GetAlias(column.NODecPlaces)].ToString();
            if (string.IsNullOrEmpty(NODecPlacesString)) return val;
            int NODecPlacesInt;
            if (!int.TryParse(NODecPlacesString, out NODecPlacesInt)) return val;
            val = dv.ToString("F" + NODecPlacesInt);
            return val;
        }

        public DataSet GenerateSQL(Guid DataViewListID, out MetricTrac.Bll.DataViewList.Extend dv, out List<MasterGroup> MasterGroupList)
        {
            MasterGroupList = new List<MasterGroup>();
            dv = MetricTrac.Bll.DataViewList.Get(DataViewListID);
            string sql = SqlSelet.Replace("@InstanceId", "'" + dv.InstanceId.ToString() + "'");

            List<string> TotalOrderBy = new List<string>();
            List<string> TotalSelect = new List<string>();

            List<ReferenceEntity.UseField> MasterGroupByFieldList = GetGroupByFieldList(ref sql, TotalOrderBy, TotalSelect, dv.GroupByList);
            List<ReferenceEntity.UseField> MasterSelectFieldList = GetSelectFieldList(ref sql, TotalOrderBy, TotalSelect, dv.SelectList);

            List<ReferenceEntity.UseField> SlaveGroupByFieldList = GetGroupByFieldList(ref sql, TotalOrderBy, TotalSelect, dv.GroupBySlaveList);
            List<ReferenceEntity.UseField> SlaveSelectFieldList = GetSelectFieldList(ref sql, TotalOrderBy, TotalSelect, dv.SelectSlaveList);

            if (MasterSelectFieldList.Count == 0 && SlaveSelectFieldList.Count == 0) return null;

            DataSet ds = MetricTrac.Bll.LinqMicajahDataContext.Execute(sql);
            DataTable dt = ds.Tables[0];
            List<string> PrevMasterGroupValue = new List<string>();
            List<MasterGroupNode> PrevMasterGroupNode = new List<MasterGroupNode>();
            for (int i = 0; i < MasterGroupByFieldList.Count + SlaveGroupByFieldList.Count; i++)
            {
                PrevMasterGroupValue.Add(null);
                PrevMasterGroupNode.Add(null);
            }
            MasterGroup mg = null;
            MasterRecord mr = null;
            MasterHeader mh = null;

            foreach (DataRow r in dt.Rows)
            {

                string OrgLocationRootColumnName = "en_FullName";
                if (dt.Columns.Contains(OrgLocationRootColumnName))
                {
                    object o = r[OrgLocationRootColumnName];
                    if (o is string && ((string)o) == "Organization Location")
                    {
                        r[OrgLocationRootColumnName] = MetricTrac.Bll.LinqMicajahDataContext.OrganizationName;
                    }
                }

                //Fill Master Group
                bool isNewMasterGroup = false;
                string alias;
                string GroupValue;
                List<MasterGroupNode> GroupTree = null;

                for (int i = 0; i < MasterGroupByFieldList.Count; i++)
                {
                    alias = GetAlias(MasterGroupByFieldList[i]);
                    GroupValue = r[alias].ToString();
                    if (PrevMasterGroupValue[i] != GroupValue && !isNewMasterGroup)
                    {
                        GroupTree = new List<MasterGroupNode>();
                        isNewMasterGroup=true;
                        for (int k = 0; k < i; k++) PrevMasterGroupNode[k].HeaderCount++;
                    }
                    PrevMasterGroupValue[i] = GroupValue;

                    string GroupHeader = MasterGroupByFieldList[i].Entity.ReadableName;
                    if (MasterGroupByFieldList[i].ReadableName != "Name") GroupHeader += " " + MasterGroupByFieldList[i].ReadableName;

                    if (isNewMasterGroup)
                    {
                        MasterGroupNode n = new MasterGroupNode()
                        {
                            Header = GroupHeader,
                            Value = GroupValue,
                            HeaderCount = 1
                        };
                        GroupTree.Add(n);

                        for (int k = 0; k < i; k++) PrevMasterGroupNode[k].SubGroupCount++;
                        PrevMasterGroupNode[i] = n;
                    }
                }
                if (isNewMasterGroup)
                {
                    mg = new MasterGroup()
                    {
                        GroupTree = GroupTree,

                        MasterHeaderList = new List<MasterHeader>(),
                        MasterRecordList = new List<MasterRecord>(),
                        SlaveGroupList = new List<SlaveGroup>()
                    };
                    MasterGroupList.Add(mg);

                    foreach (var s in MasterSelectFieldList)
                    {
                        mh = new MasterHeader()
                        {
                            Header = s.Entity.ReadableName + (s.ReadableName == "Name" ? "" : (" " + s.ReadableName))
                        };
                        mg.MasterHeaderList.Add(mh);
                    }
                }

                //Fill Master Record
                bool isNewMasterRecord = isNewMasterGroup;
                MasterRecord PrevMasterRecord = mg.MasterRecordList.Count == 0 ? null : mg.MasterRecordList[mg.MasterRecordList.Count - 1];
                mr = new MasterRecord()
                {
                    MasterValueList = new List<MasterValue>(),
                    SlaveRecordList = new List<SlaveRecord>()
                };

                for(int i=0;i<MasterSelectFieldList.Count;i++)
                {
                    var s = MasterSelectFieldList[i];
                    MasterValue mv = new MasterValue()
                    {
                        Value = GetValue(s,r)// r[GetAlias(s)].ToString()
                    };
                    mr.MasterValueList.Add(mv);
                    if (PrevMasterRecord == null || mv.Value != PrevMasterRecord.MasterValueList[i].Value) isNewMasterRecord = true;
                }
                if (isNewMasterRecord)
                {
                    mg.MasterRecordList.Add(mr);
                    for ( int k=0; k<MasterGroupByFieldList.Count;k++) PrevMasterGroupNode[k].RecordCount++;
                }
                else
                {
                    mr = mg.MasterRecordList[mg.MasterRecordList.Count - 1];
                }

                //Fill Slave Group
                if (mg.MasterRecordList.Count==1)
                {
                    for(int i=0;i<SlaveGroupByFieldList.Count;i++)
                    {
                        var sg = SlaveGroupByFieldList[i];
                        SlaveGroup g = new SlaveGroup()
                        {
                            Group = r[GetAlias(sg)].ToString()
                        };
                        mg.SlaveGroupList.Add(g);
                    }
                }


                //Fill Slave Records
                SlaveRecord sr = new SlaveRecord()
                {
                    SlaveValueList = new List<SlaveValue>()
                };
                mr.SlaveRecordList.Add(sr);
                for (int i = 0; i < SlaveSelectFieldList.Count; i++)
                {
                    var column = SlaveSelectFieldList[i];
                    //object v = r[GetAlias(column)];
                    SlaveValue sv = new SlaveValue()
                    {
                        Value = GetValue(column,r)//v == null ? null : v.ToString()
                    };
                    sr.SlaveValueList.Add(sv);
                }
            }

            /*for (int i = 0; i < MasterGroupList.Count; i++)
            {
                var MG = MasterGroupList[i];

                for (int k = i + 1; k < MasterGroupList.Count; k++)
                {
                    if (MG.GroupTree.Count <= ChildMG.GroupTree.Count) break;
                }
            }*/


            return ds;
        }

        public class MasterGroup
        {
            public List<MasterGroupNode> GroupTree;

            public List<MasterHeader> MasterHeaderList;
            public List<MasterRecord> MasterRecordList;
            public List<SlaveGroup> SlaveGroupList;
        }

        public class MasterGroupNode
        {
            public string Header { get; set; }
            public string Value { get; set; }
            public int SubGroupCount { get; set; }
            public int HeaderCount { get; set; }
            public int RecordCount { get; set; }
        }

        public class MasterHeader
        {
            public string Header { get; set; }
            
        }
        public class MasterRecord
        {
            public List<MasterValue> MasterValueList;
            public List<SlaveRecord> SlaveRecordList;
        }
        public class MasterValue
        {
            public string Value { get; set; }
        }

        public class SlaveGroup
        {
            public string Group { get; set; }
        }

        public class SlaveRecord
        {
            public List<SlaveValue> SlaveValueList;
        }

        public class SlaveValue
        {
            public string Value { get; set; }
        }
    }
}
