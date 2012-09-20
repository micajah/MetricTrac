using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Micajah.Linq.Generator
{
    public static class Config
    {
        private static string GetConfigContent()
        {
            string ConfigFile = ConfigurationManager.AppSettings["ConfigFile"];
            if (System.IO.File.Exists(ConfigFile)) return System.IO.File.ReadAllText(ConfigFile);

            ConfigFile = ConfigFile.Replace("..\\", "");
            for (int i = 0; i < 5; i++)
            {
                ConfigFile = "..\\" + ConfigFile;
                if (System.IO.File.Exists(ConfigFile)) return System.IO.File.ReadAllText(ConfigFile);
            }

            throw new Exception("Can not read configuration file.");
        }

        private static void CheckN(int n)
        {
            if (n < 0) throw new Exception("Can not fing connection string in configuration file.");
        }

        private static string GetSubString(string s, string begin, string end)
        {
            int n1 = s.IndexOf(begin);
            CheckN(n1);
            int n2 = s.IndexOf(end, n1 + begin.Length);
            CheckN(n2);
            return s.Substring(n1 + begin.Length, n2 - n1 - begin.Length);
        }

        private static string mConnectionString;
        public static string ConnectionString
        {
            get 
            {
                if(mConnectionString!=null) return mConnectionString;                

                string ConfigContent = GetConfigContent();
                string ConfigConnectionString = ConfigurationManager.AppSettings["ConfigConnectionString"];

                ConfigContent = GetSubString(ConfigContent, "<connectionStrings", "</connectionStrings");
                ConfigContent = GetSubString(ConfigContent, "name=\""+ConfigConnectionString+"\"", "/>");
                ConfigContent = GetSubString(ConfigContent, "connectionString=\"", "\"");

                mConnectionString = ConfigContent;
                return mConnectionString;
            }
        }

        public static string SqlMetalFile
        {
            get { return ConfigurationManager.AppSettings["SqlMetalFile"]; }
        }

        public static string Namespace
        {
            get { return ConfigurationManager.AppSettings["Namespace"]; }
        }

        public static string GeneratedFilesName
        {
            get { return ConfigurationManager.AppSettings["GeneratedFilesName"]; }
        }

        public static string EntityBase
        {
            get { return ConfigurationManager.AppSettings["EntityBase"]; }
        }

        private static string[] GetList(string KeyName)
        {
            string s = ConfigurationManager.AppSettings[KeyName].Trim();
            if (string.IsNullOrEmpty(s)) return new string[0];
            return s.Split(',');
        }

        public static string[] IncludeObjectsNames
        {
            get { return GetList("IncludeObjectsNames"); }
        }
        public static string [] ExcludeObjectsPrefixes
        {
            get { return GetList("ExcludeObjectsPrefixes"); }
        }
        public static bool Log
        {
            get { return ConfigurationManager.AppSettings["Log"] == "True"; }
        }
    }
}
