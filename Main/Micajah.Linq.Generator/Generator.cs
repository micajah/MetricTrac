using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;

namespace Micajah.Linq.Generator
{
    class Generator
    {
        string CsContent;
        string XmlContent;
        DataSet XmlDS;

        readonly string LogFileName = "Micajah.Linq.Generator.Log.txt";
        void WriteLog(string Name, string Value)
        {
            try
            {
                if (!Config.Log) return;
                string s = DateTime.Now.ToString("HH:mm:ss,fffffff ") + Name + ":\r\n" + Value + "\r\n\r\n";
                System.IO.File.AppendAllText(LogFileName, s);
            }
            catch { }
        }

        void ClearLog()
        {
            try
            {
                if (!Config.Log) return;
                if (System.IO.File.Exists(LogFileName))
                    System.IO.File.Delete(LogFileName);
            }
            catch { }
        }

        string ErrorString = string.Empty;
        string OutputString = string.Empty;

        void GenerateContent()
        {
            string TempCsFileName = Config.GeneratedFilesName + "Temp.cs";
            string TempXmlFileName = Config.GeneratedFilesName + "Temp.xml";

            string SqlMetalCall;
            SqlMetalCall = " /conn:\"" + Config.ConnectionString + "\" ";
            SqlMetalCall += "/map:" + TempXmlFileName + " /code:" + TempCsFileName + " ";
            SqlMetalCall += "/namespace:" + Config.Namespace + " ";
            SqlMetalCall += "/context:LinqDBDataContext /views /sprocs /functions ";
            SqlMetalCall += "/entitybase:"+Config.EntityBase+" ";
            WriteLog("SqlMetal Arguments", SqlMetalCall);

            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Config.SqlMetalFile;
            p.StartInfo.Arguments = SqlMetalCall;
            p.StartInfo.UseShellExecute = false;
            //p.StartInfo.RedirectStandardError = true;
            //p.StartInfo.RedirectStandardOutput = true;
 
            p.ErrorDataReceived += new System.Diagnostics.DataReceivedEventHandler(Process_ErrorDataReceived);
            p.OutputDataReceived += new System.Diagnostics.DataReceivedEventHandler(Process_OutputDataReceived);

            

            p.Start();            
            p.WaitForExit();

            //ErrorString += p.StandardError.ReadToEnd();
            //OutputString += p.StandardOutput.ReadToEnd();

            if (!string.IsNullOrEmpty(ErrorString)) WriteLog("SqlMetal Error", ErrorString);
            if (!string.IsNullOrEmpty(OutputString)) WriteLog("SqlMetal Output", OutputString);

            CsContent = System.IO.File.ReadAllText(TempCsFileName);
            XmlContent = System.IO.File.ReadAllText(TempXmlFileName);

            XmlDS = new System.Data.DataSet();
            XmlDS.ReadXml(TempXmlFileName);

            System.IO.File.Delete(TempCsFileName);
            System.IO.File.Delete(TempXmlFileName);
        }

        void Process_OutputDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            OutputString += e.Data;
        }

        void Process_ErrorDataReceived(object sender, System.Diagnostics.DataReceivedEventArgs e)
        {
            ErrorString += e.Data;
        }
        void RemoveBaseMethods()
        {
            string[] RemoveParen = new string[]
            {
                "\t\tprivate static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);\r\n\t\t\r\n",
                "\t\tpublic event PropertyChangingEventHandler PropertyChanging;\r\n\t\t\r\n",
                "\t\tpublic event PropertyChangedEventHandler PropertyChanged;\r\n\t\t\r\n",
                ", INotifyPropertyChanging, INotifyPropertyChanged",
                "\t\tprotected virtual void SendPropertyChanging()\r\n\t\t{\r\n\t\t\tif ((this.PropertyChanging != null))\r\n\t\t\t{\r\n\t\t\t\tthis.PropertyChanging(this, emptyChangingEventArgs);\r\n\t\t\t}\r\n\t\t}\r\n\t\t\r\n",
                "\t\tprotected virtual void SendPropertyChanged(String propertyName)\r\n\t\t{\r\n\t\t\tif ((this.PropertyChanged != null))\r\n\t\t\t{\r\n\t\t\t\tthis.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));\r\n\t\t\t}\r\n\t\t}\r\n"
            };
            foreach (string p in RemoveParen)
            {
                CsContent = CsContent.Replace(p, string.Empty);
            }
        }

        string [] Inc = Config.IncludeObjectsNames;
        string [] Exc = Config.ExcludeObjectsPrefixes;
        bool IsExclide(string ClassName)
        {
            foreach (string s in Inc) if (ClassName == s) return false;
            foreach (string s in Exc) if (ClassName.StartsWith(s)) return true;
            return false;
        }

        int ExcludeClass(int ClassGroup, string re)
        {
            Regex r = new Regex(re, RegexOptions.Singleline);
            MatchCollection mc = r.Matches(CsContent);


            for (int i = mc.Count - 1; i >= 0; i--)
            {
                string ClassName = mc[i].Groups[ClassGroup].Value;
                if (IsExclide(ClassName))
                {
                    CsContent = CsContent.Remove(mc[i].Index, mc[i].Value.Length);                    
                }
            }

            return mc.Count;
        }

        int ExcludeXml(int GroupName, string re)
        {
            Regex r = new Regex(re, RegexOptions.Singleline);
            MatchCollection mc = r.Matches(XmlContent);


            for (int i = mc.Count - 1; i >= 0; i--)
            {
                string XmlName = mc[i].Groups[GroupName].Value;
                if (IsExclide(XmlName))
                {
                    XmlContent = XmlContent.Remove(mc[i].Index, mc[i].Value.Length);
                }
            }

            return mc.Count;
        }

        void RemoveExcessCsInfo()
        {
            int n;
            n = ExcludeClass(1, "\\r\\n\\tpublic partial class (\\w+)( \\: \\S+)?\\r\\n\\t\\{.+?\\r\\n\\t\\}\\r\\n");
            n = ExcludeClass(5, "\\r\\n    partial void ((Insert)|(Update)|(Delete))(\\w+)\\(\\w+ instance\\);");
            n = ExcludeClass(1, "\\r\\n\\t\\tpublic System.Data.Linq.Table\\<(\\w+)\\> (\\w+)\\r\\n.+?\\r\\n\\t\\t\\}\\r\\n");
            n = ExcludeClass(1, "\\r\\n\\t\\tpublic \\S+ (\\w+)\\([^\\\\)]*\\)\\r\\n.+?\\r\\n\\t\\t\\}\\r\\n");
            n = ExcludeClass(1, "\\r\\n\\t\\tprivate EntitySet\\<(\\w+)\\> \\w+;\\r\\n\\t\\t");
            n = ExcludeClass(1, "\\r\\n\\t\\tprivate EntityRef\\<(\\w+)\\> \\w+;\\r\\n\\t\\t");
            n = ExcludeClass(1, "\\r\\n\\t\\tpublic EntitySet\\<(\\w+)\\> \\w+\\r\\n\\t\\t\\{.+?\\r\\n\\t\\t\\}\\r\\n");
            n = ExcludeClass(1, "\\r\\n\\t\\tpublic (\\w+) \\w+\\r\\n\\t\\t{.+?\\r\\n\\t\\t\\}\\r\\n");
            n = ExcludeClass(1, "\\r\\n\\t\\tprivate void attach_(\\w+)\\(\\w+ entity\\)\\r\\n\\t\\t\\{.+?\\r\\n\\t\\t\\}\\r\\n");
            n = ExcludeClass(1, "\\r\\n\\t\\tprivate void detach_(\\w+)\\(\\w+ entity\\)\\r\\n\\t\\t\\{.+?\\r\\n\\t\\t\\}\\r\\n");
            n = ExcludeClass(1, "\\r\\n\\t\\t\\tthis\\._(\\w+) = new EntitySet\\<\\w+\\>\\(.*?;");
            n = ExcludeClass(1, "\\r\\n\\t\\t\\tthis\\._(\\w+) = default\\(EntityRef\\<\\w+\\>\\);");
            n = ExcludeClass(1, "\\r\\n\\t\\t\\t\\t\\tif \\(this\\._(\\w+)\\..*?\\r\\n\\t\\t\\t\\t\\t}");
        }

        void RemoveExcessXmlInfo()
        {
            int n;
            n = ExcludeXml(1, "\\r\\n  \\<Table Name=\"dbo.(\\w+)\" Member=\"\\w+\"\\>\\r\\n.*?\\</Table\\>");
            n = ExcludeXml(1, "\\r\\n  \\<Function Name=\"dbo.(\\w+)\" Method=\"\\w+\".*?\\</Function\\>");
        }

        void SaveFiles()
        {
            string CsFileName = Config.GeneratedFilesName + ".cs";
            string XmlFileName = Config.GeneratedFilesName + ".xml";
            System.IO.File.WriteAllText(CsFileName, CsContent);
            System.IO.File.WriteAllText(XmlFileName, XmlContent);
        }

        void CreateAttributes()
        {
            DataTable t = XmlDS.Tables["Table"];
            foreach (DataRow r in t.Rows)
            {
                string TableName = r["Name"].ToString();
                string Member = r["Member"].ToString();
                int ClassBegin = CsContent.IndexOf("\tpublic partial class " + Member + " : LinqMicajahEntitybase");
                if (ClassBegin < 0) continue;
                int ClassEnd = CsContent.IndexOf("\r\n\t}", ClassBegin);
                if (ClassEnd < 0) continue;

                CsContent = CsContent.Insert(ClassBegin, "\t[Table(Name=\"" + TableName + "\")]\r\n");

                DataTable tt = XmlDS.Tables["Type"];
                DataRow [] rr = tt.Select("Table_Id=" + r["Table_Id"]);
                if (rr.Length != 1) continue;

                DataTable c = XmlDS.Tables["Column"];
                rr = c.Select("Type_Id=" + rr[0]["Type_Id"]);
                if (rr.Length < 1) continue;

                foreach (DataRow rc in rr)
                {
                    string ColumnName=rc["Name"].ToString();
                    string ColumnMember = rc["Member"].ToString();
                    string Storage=rc["Storage"].ToString();
                    string DbType = rc["DbType"].ToString();
                    //object o = rc["IsPrimaryKey"];
                    bool IsPrimaryKey = rc["IsPrimaryKey"].ToString().ToLower()=="true";

                    Regex reg = new Regex("\\r\\n\\t\\tpublic [^ \\r\\n\\t]*? " + ColumnMember + "\\r\\n", RegexOptions.Singleline);
                    MatchCollection mc = reg.Matches(CsContent, ClassBegin);
                    if (mc.Count < 1 || mc[0].Index > ClassEnd) continue;

                    string attr = "\r\n\t\t[Column(Name=\"" + ColumnName + "\", Storage=\"" + Storage + "\", DbType=\"" + DbType + "\", IsPrimaryKey=" + IsPrimaryKey.ToString().ToLower() + ")]";
                    CsContent = CsContent.Insert(mc[0].Index, attr);
                    ClassEnd += attr.Length;
                }

            }
        }

        void CreateMethods()
        {
            Regex r = new Regex("\\r\\n(    partial )void Delete\\w+\\(\\w+ instance\\);", RegexOptions.Singleline);
            MatchCollection mc = r.Matches(CsContent);
            for (int i = mc.Count - 1; i >= 0; i--)
            {
                int n = mc[i].Index + mc[i].Length - 1;
                CsContent = CsContent.Remove(n, 1).Insert(n, " { instance.Delete(this); }");
                CsContent = CsContent.Remove(mc[i].Groups[1].Index, mc[i].Groups[1].Length);
                CsContent = CsContent.Insert(mc[i].Groups[1].Index, "\t");
            }
        }

        public void Generate()
        {
            try
            {
                ClearLog();
                GenerateContent();
                RemoveBaseMethods();
                RemoveExcessCsInfo();
                RemoveExcessXmlInfo();
                CreateAttributes();
                CreateMethods();
                SaveFiles();
            }
            catch (Exception e)
            {
                WriteLog("Exception", e.Message);
            }
        }
    }
}
