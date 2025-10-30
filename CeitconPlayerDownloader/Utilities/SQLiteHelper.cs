using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
namespace Ceitcon_Downloader.Utilities
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
            CreteDatabase();
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

        public void CreteDatabase()
        {
            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
                using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
                {
                    var sb = new StringBuilder();
                    sb.Append("CREATE TABLE Application (Name VARCHAR(30) PRIMARY KEY, Value TEXT NOT NULL);");
                    sb.Append("CREATE TABLE MQTT (Id INTEGER PRIMARY KEY AUTOINCREMENT, Topic VARCHAR(100), Message TEXT);");
                    sb.Append("INSERT INTO Application (Name, Value) values ('Name', 'Ceitcon Downloader');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('Version', '1');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('Server', '');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('Player', '');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('Media', '');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('OrganizationID', '');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('OrganizationName', '');");                    
                    sb.Append("INSERT INTO Application (Name, Value) values ('Password', '');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('Token', '');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('TokenExpiry', '');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('TokenExpiryDate', '');");
                    m_dbConnection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sb.ToString(), m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        #region Application
        public string GetApplication(string name)
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT Value FROM Application Where Name = '{0}' LIMIT 1", name);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            result = Convert.ToString(r["Value"]);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public bool UpdateApplication(string name, string value)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                if (!String.IsNullOrEmpty(name) && value != null)
                {
                    m_dbConnection.Open();
                    string sql = String.Format("UPDATE Application SET Value = '{0}' WHERE Name = '{1}'", value, name);
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            return result;
        }
        #endregion

        #region RTMessage
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

        public bool InsertRTMessage(string topic, string message)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string oldValue = String.Empty;
                m_dbConnection.Open();

                string sql = String.Format("INSERT INTO MQTT (Topic, Message) values ('{0}', '{1}')", topic, message);
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }

            }
            return result;
            //using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            //{
            //    string oldValue = String.Empty;
            //    m_dbConnection.Open();
            //    using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
            //    {
            //        fmd.CommandText = String.Format(@"SELECT Message FROM MQTT WHERE Id ='{0}' ORDER BY MessageId DESC LIMIT 1", id);
            //        fmd.CommandType = CommandType.Text;
            //        using (SQLiteDataReader r = fmd.ExecuteReader())
            //        {
            //            while (r.Read())
            //            {
            //                string verString = Convert.ToString(r["Value"]);
            //                if (!String.IsNullOrEmpty(verString))
            //                    oldValue = verString;
            //                break;
            //            }
            //        }
            //    }

            //    if (oldValue == value)
            //        return true;

            //    if (!String.IsNullOrEmpty(oldValue))
            //    {
            //        string sql = String.Format("UPDATE MQTT SET Value = '{0}' WHERE Id = '{1}'", value, id);
            //        using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
            //        {
            //            command.ExecuteNonQuery();
            //            result = true;
            //        }
            //    }
            //    else
            //    {
            //        string sql = String.Format("INSERT INTO MQTT values ('{0}', '{1}')", id, value);
            //        using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
            //        {
            //            command.ExecuteNonQuery();
            //            result = true;
            //        }
            //    }
            //}
            //return result;
        }

        public bool ClearRTMessage()
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sql = "DELETE FROM MQTT";
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            return result;
        }
        #endregion

    }
}
