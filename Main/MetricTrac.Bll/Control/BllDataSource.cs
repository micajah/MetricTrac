using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using Micajah.Common.WebControls;

namespace MetricTrac.Bll.Web.UI.WebControls
{
    public class BllDataSource : LinqDataSource
    {
        public bool EnableAllChange
        {
            set { EnableInsert = value; EnableDelete = value; EnableUpdate = value; }
        }

        public string BllSelectMethod { get; set; }

        private void AddWhereParameters(string Name, System.Data.DbType t, string val)
        {
            foreach (Parameter p in WhereParameters) if (p.Name == Name) return;
            WhereParameters.Add(Name, t, val);
        }

        public BllDataSource()
        {
            AutoGenerateWhereClause = true;
            ContextTypeName = typeof(MetricTrac.Bll.LinqMicajahDataContext).FullName;
            Selecting += new EventHandler<LinqDataSourceSelectEventArgs>(MTDataSource_Selecting);
            Load += new EventHandler(BllDataSource_Load);
        }

        void BllDataSource_Load(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(BllSelectMethod) && (TableName!=null))
            {
                System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(typeof(MetricTrac.Bll.Metric));
                Type t = a.GetType("MetricTrac.Bll." + this.TableName);
                if (t.GetProperty("InstanceId") != null)
                    AddWhereParameters("InstanceId", System.Data.DbType.Guid, MetricTrac.Bll.LinqMicajahDataContext.InstanceId.ToString());
                if (t.GetProperty("Status") != null)
                    AddWhereParameters("Status", System.Data.DbType.Boolean, "True");
            }
        }


        void MTDataSource_Selecting(object sender, LinqDataSourceSelectEventArgs e)
        {
            if(string.IsNullOrEmpty(TableName)) return;
            if (string.IsNullOrEmpty(BllSelectMethod)) return;
                            
            System.Reflection.Assembly a = System.Reflection.Assembly.GetAssembly(typeof(MetricTrac.Bll.Metric));
            Type t = a.GetType("MetricTrac.Bll." + this.TableName);
            if (t == null) return;
            System.Reflection.MethodInfo mi = t.GetMethod(BllSelectMethod,new Type[0]);
            if (mi == null) return;
            if (!mi.IsStatic) return;
            e.Result = mi.Invoke(null, null);
            
        }
    }
}
