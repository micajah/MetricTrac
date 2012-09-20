using System;
using System.Data;
using System.Data.SqlClient;

using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Linq.Expressions;
using System.ComponentModel;

namespace System.Linq
{
    public static class MutiBaseQueryable
    {
        public static TSource FirstOrNull<TSource>(this IEnumerable<TSource> source) where TSource : class
        {
            foreach (TSource s in source) return s;
            return null;
        }

        private static int Delete(System.Data.Common.DbCommand c)
        {
            //"SELECT [t0].[CompanyID], [t0].[AccessKeyID], [t0].[UserID], [t0].[AccessKeyTypeID], [t0].[PublicAccessKey], [t0].[PrivateAccessKey]\r\nFROM [dbo].[AccessKey] AS [t0]\r\nWHERE [t0].[AccessKeyID] = @p0"
            string t = c.CommandText.ToLower();

            int n1 = t.IndexOf("from ");
            if (n1 < 1) throw new Exception("Can execute DELETE LINQ statement!");

            int n2 = t.IndexOf(" as ",n1);
            if (n2 < 1) throw new Exception("Can execute DELETE LINQ statement!");

            int n3 = t.IndexOf("where ",n1);
            if (n3 < 1) throw new Exception("Can execute DELETE LINQ statement!");

            string From = c.CommandText.Substring(n1,n2-n1)+" ";
            string Where = c.CommandText.Substring(n3).Replace("[t0].",string.Empty);

            c.CommandText = "DELETE " + From + Where;

            bool NeedOpenConnection = (c.Connection.State & ConnectionState.Open) == 0;
            try
            {
                if(NeedOpenConnection) c.Connection.Open();
                c.ExecuteNonQuery();
            }
            finally 
            {
                if(NeedOpenConnection) c.Connection.Close(); 
            }           
            return 0;
        }

        public static int Delete<TEntity>(this System.Data.Linq.Table<TEntity> source, System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            System.Data.Linq.DataContext dc = source.Context;
            System.Data.Common.DbCommand c = dc.GetCommand(source.Where(predicate));
            return Delete(c);
        }

        public static int Delete<TEntity>(this System.Data.Linq.Table<TEntity> source, System.Linq.Expressions.Expression<Func<TEntity, int, bool>> predicate) where TEntity : class
        {
            System.Data.Linq.DataContext dc = source.Context;
            System.Data.Common.DbCommand c = dc.GetCommand(source.Where(predicate));
            return Delete(c);
        }

        public static void Update<TEntity>(this System.Data.Linq.Table<TEntity> source, System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate, object NewValues) where TEntity : class
        {
        }

        public static void Update<TEntity>(this System.Data.Linq.Table<TEntity> source, System.Linq.Expressions.Expression<Func<TEntity, int, bool>> predicate, object NewValues) where TEntity : class
        {
        }

        public static void UpdateByPrimaryKey<TEntity>(this System.Data.Linq.Table<TEntity> source, TEntity PrimaryKey, object NewValues) where TEntity : class
        {
        }

        public static void DeleteByPrimaryKey<TEntity>(this System.Data.Linq.Table<TEntity> source, TEntity PrimaryKey) where TEntity : class
        {
        }

        public static TEntity SelectByPrimaryKey<TEntity>(this System.Data.Linq.Table<TEntity> source, TEntity PrimaryKey) where TEntity : class
        {
            return null;
        }
    }
}

namespace MetricTrac.Bll
{
    public sealed class LinqMicajahDataContext : LinqDBDataContext
    {
        public static DataSet Execute(string queryString)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(queryString, GetConnectionString());
            DataSet Result = new DataSet();
            adapter.Fill(Result);
            return Result;
        }

        System.IO.StringWriter LogStringWriter;
        public string LogString
        {
            get { return LogStringWriter.ToString(); }
        }

        public bool IsInstanceAvailable
        {
            get { return Micajah.Common.Security.UserContext.Current.Instances.Count > 0; }
        }
        public static Guid InstanceId
        {
            get
            {
                if (Micajah.Common.Security.UserContext.Current!=null && Micajah.Common.Security.UserContext.Current.Instances.Count > 0)
                    return Micajah.Common.Security.UserContext.Current.Instances[0].InstanceId;
                else return Guid.Empty;
            }
        }

        public static Guid GetSingleInstanceId()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var I = (from i in dc.Mc_Instance select i.InstanceId).ToList();
            if (I.Count != 1) throw new Exception("LinqMicajahDataContext.GetSingleInstanceId method can not return single InstanceId");
            return I[0];
        }

        public static Guid GetSingleOrganizationId()
        {
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            var O = (from o in dc.Mc_Organization select o.OrganizationId).ToList();
            if (O.Count != 1) throw new Exception("LinqMicajahDataContext.GetSingleOrganizationId method can not return single OrganizationId");
            return O[0];        
        }

        public static Guid OrganizationId
        {
            get
            {             
                if (Micajah.Common.Security.UserContext.Current != null)
                    if (Micajah.Common.Security.UserContext.Current.SelectedOrganization != null)
                        return Micajah.Common.Security.UserContext.Current.SelectedOrganization.OrganizationId;
                return Guid.Empty;
            }
        }

        public static string OrganizationName
        {
            get
            {
                if (Micajah.Common.Security.UserContext.Current != null)
                    if (Micajah.Common.Security.UserContext.Current.SelectedOrganization != null)
                        return Micajah.Common.Security.UserContext.Current.SelectedOrganization.Name;
                return "Organization Location";
            }
        }

        public static Guid? LogedUserId
        {
            get
            {
                Micajah.Common.Security.UserContext uc = Micajah.Common.Security.UserContext.Current;
                if (uc == null) return null;
                return uc.UserId;
            }
        }

        private static string GetConnectionString()
        {
            System.Configuration.ConnectionStringSettingsCollection cssc = System.Configuration.ConfigurationManager.ConnectionStrings;
            return cssc["Micajah.Common.ConnectionString"].ConnectionString;
        }

        private static System.Data.Linq.Mapping.MappingSource GetMapping()
        {
            System.Reflection.Assembly ass = System.Reflection.Assembly.GetAssembly(typeof(MetricTrac.Bll.LinqDBDataContext));
            System.IO.Stream s = ass.GetManifestResourceStream("MetricTrac.DataContext.LinqDB.xml");
            System.Data.Linq.Mapping.XmlMappingSource ms = System.Data.Linq.Mapping.XmlMappingSource.FromStream(s);
            return ms;
        }

        public LinqMicajahDataContext()
            : base(GetConnectionString(), GetMapping())
        {
            LogStringWriter = new System.IO.StringWriter();
            this.Log = LogStringWriter;
        }

        public static bool IsActive(bool? Status)
        {
            return Status == true;
        }

        private void SetParam(LinqMicajahEntitybase entity, string name, object value)
        {
            try
            {
                Type t = entity.GetType();
                System.Reflection.PropertyInfo pi = t.GetProperty(name);
                if (pi == null) return;
                pi.SetValue(entity, value, null);
            }
            catch { }
        }

        private object GetParam(LinqMicajahEntitybase entity, string name)
        {
            try
            {
                Type t = entity.GetType();
                System.Reflection.PropertyInfo pi = t.GetProperty(name);
                if (pi == null) return null;
                else return pi.GetValue(entity, null);                
            }
            catch { return null; }
        }

        public static string GetPK(LinqMicajahEntitybase entity)
        {
            return entity.GetType().Name + "ID";
        }

        public override void SubmitChanges(System.Data.Linq.ConflictMode failureMode)
        {
            System.Data.Linq.ChangeSet cs = this.GetChangeSet();
            bool Cancel = false;

            List<LinqMicajahEntitybase> Inserted = new List<LinqMicajahEntitybase>();
            List<LinqMicajahEntitybase> Updated = new List<LinqMicajahEntitybase>();
            List<LinqMicajahEntitybase> Deleted = new List<LinqMicajahEntitybase>();
            
            for (int i=0; i<cs.Inserts.Count;i++)
            {
                object o = cs.Inserts[i];
                if (!(o is LinqMicajahEntitybase)) continue;
                LinqMicajahEntitybase entity = (LinqMicajahEntitybase)o;

                if (InstanceId != Guid.Empty)
                    SetParam(entity, "InstanceId", InstanceId);
                SetParam(entity, "Created", DateTime.Now);
                SetParam(entity, "Status", true);

                string PKName = GetPK(entity);
                o = GetParam(entity, PKName);
                if (o is Guid)
                    if (((Guid)o)==Guid.Empty)
                        SetParam(entity, PKName, Guid.NewGuid());                
                entity.OnInserting(this, ref Cancel);
                Inserted.Add(entity);
            }

            for (int i = 0; i < cs.Updates.Count; i++)
            {
                object o = cs.Updates[i];
                if (!(o is LinqMicajahEntitybase)) continue;
                LinqMicajahEntitybase entity = (LinqMicajahEntitybase)o;
                SetParam(entity, "Updated", DateTime.Now);
                entity.OnUpdating(this, ref Cancel);
                Updated.Add(entity);
            }

            for (int i = 0; i < cs.Deletes.Count; i++)
            {
                object o = cs.Deletes[i];
                if (!(o is LinqMicajahEntitybase)) continue;
                LinqMicajahEntitybase entity = (LinqMicajahEntitybase)o;
                entity.OnDeleting(this, ref Cancel);
                Deleted.Add(entity);
            }

            if (!Cancel) base.SubmitChanges(failureMode);

            foreach (LinqMicajahEntitybase e in Inserted)
                e.OnInserted(this);
            foreach (LinqMicajahEntitybase e in Updated)
                e.OnUpdated(this);
            foreach (LinqMicajahEntitybase e in Deleted)
                e.OnDeleted(this);
        }



        public static System.Collections.IEnumerable SelectGuid(string TableName, string IDColumnName, string TextColumnName)
        {
            string s = "select distinct " + IDColumnName + ", " + TextColumnName + ", InstanceId  from " + TableName + " where InstanceId='" + MetricTrac.Bll.LinqMicajahDataContext.InstanceId + "' AND Status=1";
            Type t = typeof(MetricTrac.Bll.LinqMicajahEntitybase).Assembly.GetType("MetricTrac.Bll." + TableName);
            LinqMicajahDataContext dc = new LinqMicajahDataContext();
            System.Collections.IEnumerable ret = dc.ExecuteQuery(t, s, new object[0]);
            return ret;
        }

    }

    public abstract class LinqMicajahEntitybase : INotifyPropertyChanging, INotifyPropertyChanged
    {
        protected static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void SendPropertyChanging()
        {
            if ((this.PropertyChanging != null)) this.PropertyChanging(this, emptyChangingEventArgs);
        }

        protected virtual void SendPropertyChanged(String propertyName)
        {
            if ((this.PropertyChanged != null)) this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }


        public virtual void OnInserting(LinqMicajahDataContext dc, ref bool Cancel)
        {
        }

        public virtual void OnInserted(LinqMicajahDataContext dc)
        {
        }

        public virtual void OnUpdating(LinqMicajahDataContext dc, ref bool Cancel)
        {
        }

        public virtual void OnUpdated(LinqMicajahDataContext dc)
        {
        }

        public virtual void OnDeleting(LinqMicajahDataContext dc, ref bool Cancel)
        {
        }

        public virtual void OnDeleted(LinqMicajahDataContext dc)
        {
        }

        public virtual void Delete(LinqDBDataContext dc)
        {
            Type t = this.GetType();
            object[] oo = t.GetCustomAttributes(typeof(TableAttribute), true);
            if (oo == null || oo.Length < 1) return;
            TableAttribute ta = (TableAttribute)oo[0];

            string q;
            List<object> Params = new List<object>();
            PropertyInfo PiStatus = t.GetProperty("Status");            
            if (PiStatus != null && PiStatus.PropertyType.FullName == typeof(bool?).FullName)
            {
                q = "UPDATE " + ta.Name + " SET Status=0, Updated={0} WHERE ";
                Params.Add(DateTime.Now); // It doesn't work
            }
            else
            {
                q = "DELETE " + ta.Name + " WHERE ";
            }

            PropertyInfo[] pis = t.GetProperties();
            bool FirstWhere = true;
            foreach (PropertyInfo pi in pis)
            {
                oo = pi.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (oo == null || oo.Length < 1) continue;
                ColumnAttribute ca = (ColumnAttribute)oo[0];
                if (!ca.IsPrimaryKey) continue;

                object v;
                if (pi.Name == "InstanceId") v = LinqMicajahDataContext.InstanceId;
                else v = pi.GetValue(this, null);
                Params.Add(v);
                q += (FirstWhere ? "" : " AND ") + ca.Name + "={" + (Params.Count - 1) + "} ";

                FirstWhere = false;
            }

            dc.ExecuteCommand(q, Params.ToArray());
        }

        public void CopyFrom(object Source)
        {
            Type SType = Source.GetType();
            Type DType = this.GetType();
            foreach (PropertyInfo SPI in SType.GetProperties())
            {
                string STypeName = SPI.PropertyType.FullName;
                PropertyInfo DPI = DType.GetProperty(SPI.Name);
                if (DPI == null) continue;
                if (DPI.PropertyType.IsArray) continue;

                object v = SPI.GetValue(Source,null);
                if (!(v is string) && v!=null)
                {
                    if (v is System.Collections.IEnumerable) continue;
                    if (v is LinqMicajahEntitybase) continue;
                    if (!(v is Nullable) && SPI.PropertyType.IsClass) continue;
                }

                DPI.SetValue(this, v, null);
            }
        }
    }
}
