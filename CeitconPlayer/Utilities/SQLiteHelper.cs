using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;

namespace Ceitcon_Player.Utilities
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class SQLiteHelper
    {
        private const string databaseFile = "Ceitcon_Downloader.db";
        private static SQLiteHelper instance;
        private string databasePath = String.Empty;

        private SQLiteHelper()
        {
            databasePath = Path.Combine(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, System.AppDomain.CurrentDomain.RelativeSearchPath ?? ""), databaseFile);
            //CreteDatabase();
        }

        public static SQLiteHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteHelper();
                }
                return instance;
            }
        }

        #region RTMessage
        //public string GetRTMessage(string id)
        //{
        //    string result = String.Empty;
        //    using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
        //    {
        //        m_dbConnection.Open();
        //        using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
        //        {
        //            fmd.CommandText = String.Format(@"SELECT * FROM RTMessage Where Id = '{0}' LIMIT 1", id);
        //            fmd.CommandType = CommandType.Text;
        //            using (SQLiteDataReader r = fmd.ExecuteReader())
        //            {
        //                while (r.Read())
        //                {
        //                    result = Convert.ToString(r["Value"]);
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        public string GetRTMessage(string topic)
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT Message FROM MQTT Where Topic = '{0}' ORDER BY Id DESC  LIMIT 1", topic);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            result = Convert.ToString(r["Message"]);
                            break;
                        }
                    }
                }
            }
            return result;
        }
        #endregion
       
    }
}
