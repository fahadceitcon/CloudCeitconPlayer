using Ceitcon_Data.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Telerik.Windows.Controls.ScheduleView;
using System.Linq;
using log4net;
using Ceitcon_Data.Model.Data;

namespace Ceitcon_Designer.Utilities
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class SQLiteHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string databaseFile = "Ceitcon_Designer.db";
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
            //if (File.Exists(databasePath))
            //    File.Delete(databasePath);

            if (!File.Exists(databasePath))
            {
                SQLiteConnection.CreateFile(databasePath);
                using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
                {
                    var sb = new StringBuilder();
                    sb.Append("CREATE TABLE Application (Name VARCHAR(30) PRIMARY KEY, Value TEXT);");
                    sb.Append("CREATE TABLE Project (Id VARCHAR(30) PRIMARY KEY, Name TEXT NOT NULL, Location TEXT NOT NULL, Version INTEGER);");
                    sb.Append("CREATE TABLE Location (Id VARCHAR(30) PRIMARY KEY, Country TEXT NOT NULL, City TEXT NOT NULL, Latitude NUMERIC NOT NULL, Longnitude NUMERIC NOT NULL);");

                    sb.Append("INSERT INTO Application (Name, Value) values ('Name', 'Ceitcon Designer');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('Version', 1);");

                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('1', 'Saudi Arabia', 'Al Qunfudhah', 19.1264, 41.0789);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('2', 'Saudi Arabia', 'At Taif', 21.2622, 40.3823);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('3', 'Saudi Arabia', 'Qal at Bishah', 20.0087, 42.5987);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('4', 'Saudi Arabia', 'Al Hillah', 23.4895, 46.7564);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('5', 'Saudi Arabia', 'As Sulayyil', 20.4623, 45.5722);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('6', 'Saudi Arabia', 'Al Wajh', 26.2324, 36.4636);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('7', 'Saudi Arabia', 'An Nabk', 31.3333, 37.3333);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('8', 'Saudi Arabia', 'Al Kharj', 24.1556, 47.312);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('9', 'Saudi Arabia', 'Najran', 17.5065, 44.1316);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('10', 'Saudi Arabia', 'Al Bahah', 20.0129, 41.4677);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('11', 'Saudi Arabia', 'Medina', 24.5, 39.58);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('12', 'Saudi Arabia', 'Hail', 27.5236, 41.7001);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('13', 'Saudi Arabia', 'Jizan', 16.9066, 42.5566);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('14', 'Saudi Arabia', 'Makkah', 21.43, 39.82);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('15', 'Saudi Arabia', 'Tabuk', 28.3838, 36.555);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('16', 'Saudi Arabia', 'Arar', 30.99, 41.0207);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('17', 'Saudi Arabia', 'Abha', 18.2301, 42.5001);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('18', 'Saudi Arabia', 'Ad Damman', 26.4282, 50.0997);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('19', 'Saudi Arabia', 'Buraydah', 26.3664, 43.9628);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('20', 'Saudi Arabia', 'Sakakah', 30, 40.1333);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('21', 'Saudi Arabia', 'Riyadh', 24.6408, 46.7727);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('22', 'Saudi Arabia', 'Jeddah', 21.5169, 39.2192);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('23', 'Saudi Arabia', 'Hafar al Batin', 28.4337, 45.9601);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('24', 'Saudi Arabia', 'Az Zahran', 26.2914, 50.1583);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('25', 'Saudi Arabia', 'Al Jubayl', 27.0046, 49.646);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('26', 'Saudi Arabia', 'Al-Qatif', 26.5196, 50.0115);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('27', 'Saudi Arabia', 'Yanbu al Bahr', 24.0943, 38.0493);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('28', 'Saudi Arabia', 'Al Mubarraz', 25.4291, 49.5659);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('29', 'Saudi Arabia', 'Al Hufuf', 25.3487, 49.5856);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('30', 'Saudi Arabia', 'Al Quwayiyah', 24.0737, 45.2806);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('31', 'Saudi Arabia', 'Rafha', 29.6202, 43.4948);");
                    sb.Append("INSERT INTO Location (Id, Country, City, Latitude, Longnitude) values ('32', 'Saudi Arabia', 'Dawmat al Jandal', 29.8153, 39.8664);");

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

        public bool InsertApplication(string name, string value)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string oldValue = String.Empty;
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT Value FROM Application WHERE Name ='{0}' LIMIT 1", name);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string verString = Convert.ToString(r["Value"]);
                            if (!String.IsNullOrEmpty(verString))
                                oldValue = verString;
                            break;
                        }
                    }
                }

                if (oldValue == value)
                    return true;

                if (!String.IsNullOrEmpty(oldValue))
                {
                    string sql = String.Format("UPDATE Application SET Value = '{0}' WHERE Name = '{1}'", value, name);
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
                else
                {
                    string sql = String.Format("INSERT INTO Application values ('{0}', '{1}')", name, value);
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

        #region Project
        public int InsertProject(string id, string name, string location)
        {
            int version = -1;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                version = 0;
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Project WHERE Id ='{0}' LIMIT 1", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string verString = Convert.ToString(r["Version"]);
                            if (!String.IsNullOrEmpty(verString))
                                version = Convert.ToInt32(verString) + 1;
                            break;
                        }
                    }
                }

                if (version > 0)
                {
                    //Update
                    string sql = String.Format("UPDATE Project SET Version = {0} WHERE Id = '{1}'", version, id);
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                else
                {
                    //m_dbConnection.Open();
                    string sql = String.Format("INSERT INTO Project values ('{0}', '{1}', '{2}', {3})", id, name, location, version);
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            return version;
        }

        public ProjectInfoModel GetProject(string id)
        {
            ProjectInfoModel pr = null;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Project WHERE Id = '{0}' LIMIT 1", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            pr = new ProjectInfoModel()
                            {
                                Id = Convert.ToString(r["Id"]),
                                Name = Convert.ToString(r["Name"]),
                                Location = Convert.ToString(r["Location"]),
                                Version = Convert.ToString(r["Version"])
                            };
                            break;
                        }
                    }
                }
            }
            return pr;
        }

        public void DeleteProject(string id)
        {
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sql = String.Format("DELETE FROM Project WHERE Id = '{0}'", id);
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        public ProjectInfoModel[] GetProjects()
        {
            List<ProjectInfoModel> resultList = new List<ProjectInfoModel>();
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Project");
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            resultList.Add(new ProjectInfoModel()
                            {
                                Id = Convert.ToString(r["Id"]),
                                Name = Convert.ToString(r["Name"]),
                                Location = Convert.ToString(r["Location"]),
                                Version = Convert.ToString(r["Version"])
                            });
                        }
                    }
                }
            }
            return resultList.ToArray();
        }
        #endregion

        #region  Flights
        public List<FlightModel> GetFlightsData(string DataSourceID)
        {
            List<FlightModel> objList = new List<FlightModel>();


            SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath));

            m_dbConnection.Open();

            SQLiteDataAdapter da = new SQLiteDataAdapter("Select Data from DataSource where Id='" + DataSourceID + "'", m_dbConnection);
            System.Data.DataTable dt = new System.Data.DataTable();
            da.Fill(dt);

            try

            {
                string xml = "<root>" + dt.Rows[0][0].ToString() + "</root>";
                XDocument xmldoc = XDocument.Parse(xml);
                List<XElement> _FlightsList = xmldoc.Descendants("FLIGHT").ToList<XElement>();
                foreach (XElement _Flight in _FlightsList)

                {

                    List<XElement> _childRecords = _Flight.Descendants().ToList();
                    FlightModel objF = new FlightModel();
                    objF.INT_DOM = _childRecords[0].Value;
                    objF.ARR_DEP = _childRecords[1].Value;
                    objF.FL_STATUS_1 = _childRecords[2].Value;
                    objF.AIRLINE = _childRecords[3].Value;
                    objF.AIRLINE_DESCR = _childRecords[4].Value;
                    objF.FL_NUMBER = _childRecords[5].Value;
                    objF.SCH_TIME = _childRecords[6].Value;
                    objF.EST_TIME = _childRecords[7].Value;
                    objF.JOINT_FLAG = _childRecords[8].Value;
                    objF.JOINT_MASTER_ID = _childRecords[9].Value;
                    objF.ROUTING = _childRecords[10].Value;
                    objF.ROUTING_ENG = _childRecords[11].Value;
                    objF.ROUTING_KA = _childRecords[12].Value;
                    objF.VIA = _childRecords[13].Value;
                    objF.VIA_ENG = _childRecords[14].Value;
                    objF.VIA_KA = _childRecords[15].Value;
                    objF.TD_TIME = _childRecords[16].Value;
                    objF.TOFF_TIME = _childRecords[17].Value;
                    objF.PUB_RMK = _childRecords[18].Value;
                    objF.PUB_RMK_ENG = _childRecords[19].Value;
                    objF.PUB_RMK_KA = _childRecords[20].Value;
                    objF.AIRCRAFT_TYPE = _childRecords[21].Value;
                    objF.REGISTRATION = _childRecords[22].Value;
                    objF.CHECKIN_1 = _childRecords[23].Value;
                    objF.CHECKIN_2 = _childRecords[24].Value;
                    objF.GATE_1 = _childRecords[25].Value;
                    objF.BAGGAGE_1 = _childRecords[26].Value;
                    objF.TERMINAL = _childRecords[27].Value;

                    objList.Add(objF);


                }
            }
            catch (Exception ex)
            {
                m_dbConnection.Close();
                throw ex;
            }

            m_dbConnection.Close();

            return objList;
        }

        public List<FlightModel> GetFlightsDataFromXml(string xmlData)
        {
            List<FlightModel> objList = new List<FlightModel>();
            try
            {
                XDocument xmldoc;
                if (xmlData.StartsWith("<?xml") == false)
                {

                    xmldoc = XDocument.Parse("<root>" + xmlData + "</root>");
                }
                else
                {
                    xmldoc = XDocument.Parse(xmlData);
                }

                List<XElement> _FlightsList = xmldoc.Descendants("FLIGHT").ToList<XElement>();
                foreach (XElement _Flight in _FlightsList)
                {
                    List<XElement> _childRecords = _Flight.Descendants().ToList();
                    FlightModel objF = new FlightModel();
                    objF.INT_DOM = _childRecords[0].Value;
                    objF.ARR_DEP = _childRecords[1].Value;
                    objF.FL_STATUS_1 = _childRecords[2].Value;
                    objF.AIRLINE = _childRecords[3].Value;
                    objF.AIRLINE_DESCR = _childRecords[4].Value;
                    objF.FL_NUMBER = _childRecords[5].Value;
                    objF.SCH_TIME = _childRecords[6].Value;
                    objF.SCH_TIME_O = _childRecords[6].Value;
                    objF.EST_TIME = _childRecords[7].Value;
                    objF.EST_TIME_O = _childRecords[7].Value;
                    objF.JOINT_FLAG = _childRecords[8].Value;
                    objF.JOINT_MASTER_ID = _childRecords[9].Value;
                    objF.ROUTING = _childRecords[10].Value;
                    objF.ROUTING_ENG = _childRecords[11].Value;
                    objF.ROUTING_KA = _childRecords[12].Value;
                    objF.VIA = _childRecords[13].Value;
                    objF.VIA_ENG = _childRecords[14].Value;
                    objF.VIA_KA = _childRecords[15].Value;
                    objF.TD_TIME = _childRecords[16].Value;
                    objF.TOFF_TIME = _childRecords[17].Value;
                    objF.PUB_RMK = _childRecords[18].Value;
                    objF.PUB_RMK_ENG = _childRecords[19].Value;
                    objF.PUB_RMK_KA = _childRecords[20].Value;
                    objF.AIRCRAFT_TYPE = _childRecords[21].Value;
                    objF.REGISTRATION = _childRecords[22].Value;
                    objF.CHECKIN_1 = _childRecords[23].Value;
                    objF.CHECKIN_2 = _childRecords[24].Value;
                    objF.GATE_1 = _childRecords[25].Value;
                    objF.BAGGAGE_1 = _childRecords[26].Value;
                    objF.TERMINAL = _childRecords[27].Value;
                    objList.Add(objF);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error", ex);
            }
            log.Info(String.Format("Number of records {0}", objList.Count()));

            return objList;
        }
        #endregion

        #region Weather

        #endregion

        #region Location
        public LocationModel[] GetLocations()
        {
            List<LocationModel> resultList = new List<LocationModel>();
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Location");
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            resultList.Add(new LocationModel()
                            {
                                Id = Convert.ToString(r["Id"]),
                                Country = Convert.ToString(r["Country"]),
                                City = Convert.ToString(r["City"]),
                                Latitude = Convert.ToDouble(r["Latitude"]),
                                Longnitude = Convert.ToDouble(r["Longnitude"])
                            });
                        }
                    }
                }
            }
            return resultList.ToArray();
        }
        #endregion

    }
}
