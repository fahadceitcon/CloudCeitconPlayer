using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Xml;
using System.Reflection;
using System.Globalization;
using log4net;
using System.Collections.Generic;

namespace Ceitcon_Service.Data
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class SQLiteHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private const string databaseFile = "Ceitcon_Server.db";
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
                using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;Pooling=True;Max Pool Size=100", databasePath)))
                {
                    var sb = new StringBuilder();
                    sb.Append("CREATE TABLE Application (Name VARCHAR(20) PRIMARY KEY, Value TEXT);");
                    sb.Append("CREATE TABLE Scheduler (Id VARCHAR(30) PRIMARY KEY, Client VARCHAR(900) NOT NULL, Project TEXT NOT NULL, Version TEXT, StartTime TEXT, EndTime TEXT, Location TEXT, FileList TEXT, Content TEXT);");
                    sb.Append(@"CREATE TABLE UserGroup (Id VARCHAR(30) PRIMARY KEY, Name TEXT NOT NULL, Permissions TEXT NOT NULL, Active INTEGER DEFAULT 1);");
                    sb.Append(@"CREATE TABLE User (Id VARCHAR(30) PRIMARY KEY, GroupId VARCHAR(30) NOT NULL, Name TEXT NOT NULL, Password TEXT NOT NULL, Email TEXT, Phone TEXT, 
                                Permissions TEXT NOT NULL, Active INTEGER DEFAULT 1, FOREIGN KEY(GroupId) REFERENCES UserGroup(Id));");

                    // NETWORK  //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
                    sb.Append("CREATE TABLE Domain (Id VARCHAR(30) PRIMARY KEY, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1);");
                    sb.Append("CREATE TABLE Country (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, CONSTRAINT name_unique UNIQUE (Name), FOREIGN KEY(Parent) REFERENCES Domain(Id));");
                    sb.Append("CREATE TABLE NetworkRegion (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, CONSTRAINT name_unique UNIQUE (Name), FOREIGN KEY(Parent) REFERENCES Country(Id));");
                    sb.Append("CREATE TABLE LocationGroup (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, CONSTRAINT name_unique UNIQUE (Name), FOREIGN KEY(Parent) REFERENCES NetworkRegion(Id));");
                    sb.Append("CREATE TABLE Location (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, CONSTRAINT name_unique UNIQUE (Name), FOREIGN KEY(Parent) REFERENCES LocationGroup(Id));");
                    sb.Append("CREATE TABLE Floor (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, CONSTRAINT name_unique UNIQUE (Name), FOREIGN KEY(Parent) REFERENCES Location(Id));");
                    sb.Append("CREATE TABLE Zone (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, CONSTRAINT name_unique UNIQUE (Name), FOREIGN KEY(Parent) REFERENCES Floor(Id));");
                    sb.Append("CREATE TABLE PlayerGroup (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, CONSTRAINT name_unique UNIQUE (Name), FOREIGN KEY(Parent) REFERENCES Zone(Id));");
                    sb.Append("CREATE TABLE Player (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, HostName TEXT, IPAddress TEXT, RegistredTime TEXT, Screens INTEGER DEFAULT 0, Licence INTEGER DEFAULT 0, Status INTEGER DEFAULT 0, RefreshTime INTEGER DEFAULT 30, LastConnection TEXT, Active INTEGER DEFAULT 1, CONSTRAINT name_unique UNIQUE (Name), FOREIGN KEY(Parent) REFERENCES PlayerGroup(Id));");
                    sb.Append("CREATE TABLE FaceGroup (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, FOREIGN KEY(Parent) REFERENCES Player(Id));");
                    sb.Append("CREATE TABLE Face (Id VARCHAR(30) PRIMARY KEY, Parent VARCHAR(30) NOT NULL, Name VARCHAR(50) NOT NULL, Description TEXT, Active INTEGER DEFAULT 1, FOREIGN KEY(Parent) REFERENCES FaceGroup(Id));");

                    //Values
                    sb.Append("INSERT INTO Application (Name, Value) values ('Name', 'Ceitcon Server');");
                    sb.Append("INSERT INTO Application (Name, Value) values ('Version', 1);");
                    sb.Append(@"INSERT INTO UserGroup (Id, Name, Permissions, Active) values ('0', 'Admins', '111111111111', 1);");
                    sb.Append(@"INSERT INTO UserGroup (Id, Name, Permissions, Active) values ('1', 'Managers', '111000000000', 1);");
                    sb.Append(@"INSERT INTO UserGroup (Id, Name, Permissions, Active) values ('2', 'Guests', '100000000000', 1);");
                    sb.Append(@"INSERT INTO User (Id, GroupId, Name, Password, Email, Phone, Permissions, Active)
                        values ('0', '0', 'sa','sa', 'supeadmin@gmail.com', '+38100000000', '111111111111', 1);");
                    sb.Append(@"INSERT INTO User (Id, GroupId, Name, Password, Email, Phone, Permissions, Active)
                        values ('1', '0', 'Admin','Admin', 'admin@gmail.com', '+38100000000', '111100110111', 1);");
                    sb.Append(@"INSERT INTO User (Id, GroupId, Name, Password, Email, Phone, Permissions, Active)
                        values ('2', '1', 'Manager', 'Manager', 'manager@gmail.com', '+38100000000', '111000000000', 1);");
                    sb.Append(@"INSERT INTO User (Id, GroupId, Name, Password, Email, Phone, Permissions, Active)
                        values ('3', '2', 'Guest', 'Guest', 'guest@gmail.com', '+38100000000', '100000000000', 1);");

                    sb.Append("INSERT INTO Domain (Id, Name, Description, Active) values ('0', 'Domain', '', 1);");
                    sb.Append("INSERT INTO Country (Id, Parent, Name, Description, Active) values ('0', '0', 'Country', '', 1);");
                    sb.Append("INSERT INTO NetworkRegion (Id, Parent, Name, Description, Active) values ('0', '0', 'Region', '', 1);");
                    sb.Append("INSERT INTO LocationGroup (Id, Parent, Name, Description, Active) values ('0', '0', 'Location Group', '', 1);");
                    sb.Append("INSERT INTO Location (Id, Parent, Name, Description, Active) values ('0', '0', 'Location', '', 1);");
                    sb.Append("INSERT INTO Floor (Id, Parent, Name, Description, Active) values ('0', '0', 'Floor', '', 1);");
                    sb.Append("INSERT INTO Zone (Id, Parent, Name, Description, Active) values ('0', '0', 'Zone', '', 1);");
                    sb.Append("INSERT INTO PlayerGroup (Id, Parent, Name, Description, Active) values ('0', '0', 'Player Group', '', 1);");
                    sb.Append("INSERT INTO Player (Id, Parent, Name, Description, HostName, IPAddress, RegistredTime, Screens, Licence, Status, RefreshTime, LastConnection, Active) values ('0', '0', 'Player', '', '', '', '', 0, 0, 0, 30, '', 1);");
                    sb.Append("INSERT INTO FaceGroup (Id, Parent, Name, Description, Active) values ('0', '0', 'Face Group', '', 1);");
                    sb.Append("INSERT INTO Face (Id, Parent, Name, Description, Active) values ('0', '0', 'Face', '', 1);");

                    //FIDS
                    sb.Append("CREATE TABLE DataSource (Id VARCHAR(30) PRIMARY KEY, Name TEXT NOT NULL, Columns TEXT, Description TEXT, Type TEXT, Data TEXT, Url TEXT, UrlUsername TEXT, UrlPassword TEXT, Active INTEGER DEFAULT 1);");
                    sb.Append("CREATE TABLE Logo (Id VARCHAR(3) PRIMARY KEY, Description TEXT NOT NULL, FileLocation TEXT NOT NULL, FileSize INTEGER NOT NULL, Active INTEGER DEFAULT 1);");
                    //sb.Append("CREATE TABLE DataRecord (Id VARCHAR(30) PRIMARY KEY, RecordGroup INTEGER NOT NULL, Record Text);");

                    // TEST SOURCE
                    //sb.Append("INSERT INTO DataSource(Id, Name, RecordGroup, Columns, Description, Active) values('0', 'Arrival', 0, 'Time;Flight;Airline;Description;Terminal;Exp.;Remarker','', 1);");
                    //sb.Append("INSERT INTO DataSource(Id, Name, RecordGroup, Columns, Description, Active) values('1', 'Departure', 1, 'Time;Flight;Airline;Description;Terminal;Exp.;Remarker','', 1);");

                    ////TEST ARIVAL
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('0','0','10102017 132000;SV706;MEDINA;T2;NULL;10102017 124400;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('1','0','10102017 132000;6S642;ABHA;T5;B52;10102017 132000;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('2','0','10102017 132000;SV1736;BISHA;T5;B55;10102017 130800;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('3','0','10102017 132500;SV1508;RAFHA;T5;B55;10102017 131100;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('4','0','10102017 133000;SV1396;AL - JOUF;T5;B54;10102017 132000;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('5','0','10102017 133500;XY028;JEDDAH;T5;B52;10102017 133500;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('6','0','10102017 134000;SV1030;JEDDAH;T5;B53;10102017 132900;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('7','0','10102017 134500;NA012;JEDDAH;T5;B54;10102017 133700;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('8','0','10102017 134500;SM463;SOHAG;T1;B12;10102017 133000;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('9','0','10102017 135000;SV1900;TAIF;T5;B53;10102017 202600;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('10','0','10102017 140000;XY144;GURAYAT;T5;B51;10102017 140000;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('11','0','10102017 141500;6S322;ABHA;T5;B55;10102017 141500;CANCELLED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('12','0','10102017 141500;SV811;DHAKA;T2;NULL;10102017 141500;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('13','0','10102017 142500;SV767;MADRAS;T2;B21;10102017 141000;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('14','0','10102017 143000;AI921;MUMBAI;T1;B11;10102017 143000;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('15','0','10102017 143000;XY264;CAIRO;T2;B22;10102017 144700;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('16','0','10102017 143500;SV1676;ABHA;T5;B53;10102017 131400;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('17','0','10102017 144000;SV310;CAIRO;T2;B23;10102017 142600;ON GATE');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('18','0','0102017 144000;SV1032;JEDDAH;T5;B55;10102017 142100;CAROUSEL CLOSED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('19','0','10102017 144500;F3107;JEDDAH;T5;B51;10102017 144500;ON GATE');");

                    ////TEST DEPARTED
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('20','1','10102017 133000;SV1915;TAIF;T5;GT506A;10102017 140000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('21','1','10102017 133000;PA275;ISLAMABAD;T1;GT11;10102017 133000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('22','1','10102017 133000;XY070;DAMMAM;T5;GT504B;10102017 133000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('23','1','10102017 134500;SV1805;JAZAN;T5;GT505A;10102017 114000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('24','1','10102017 135000;SV1132;DAMMAM;T5;GT507A;10102017 140500;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('25','1','10102017 135500;SV554;DUBAI;T2;GT24;10102017 135500;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('26','1','10102017 140000;SV1035;JEDDAH;T5;GT510;10102017 140000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('27','1','10102017 142000;6S204;DAMMAM;T5;GT507B;10102017 142000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('28','1','10102017 142500;SV706;KARACHI;T2;GT26;10102017 142500;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('29','1','10102017 142500;SV1389;AL - JOUF;T5;GT504A;10102017 142500;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('30','1','10102017 143000;XY027;JEDDAH;T5;GT508A;10102017 143000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('31','1','10102017 144500;SM464;SOHAG;T1;GT17;10102017 144500;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('32','1','10102017 145000;6S323;ABHA;T5;GT506B;10102017 145000;CANCELLED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('33','1','10102017 145000;SV375;CASABLANCA;T2;GT23;10102017 145000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('34','1','10102017 150000;SV1037;JEDDAH;T5;GT511;10102017 150000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('35','1','10102017 150500;SV327;SHARM;EL;SHEIKH;T2;GT22B;10102017 150500;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('36','1','10102017 150500;SV333;BORG;EL;ARAB;T2;GT21;10102017 150500;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('37','1','10102017 151000;NA013;JEDDAH;T5;GT504B;10102017 151000;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('38','1','10102017 151500;XY097;MEDINA;T5;GT508B;10102017 151500;DEPARTED');");
                    //sb.Append("INSERT INTO DataRecord(Id, RecordGroup, Record) values('39','1','10102017 151500;SV570;ABU;DHABI;T2;GT25B;10102017 151500;DEPARTED');");

                    // FIDS STATIC
                    //sb.Append("CREATE TABLE FID (Id VARCHAR(30) PRIMARY KEY, INT_DOM TEXT, ARR_DEP TEXT, FL_STATUS_1 TEXT, AIRLINE TEXT, AIRLINE_DESCR TEXT, FL_NUMBER TEXT, SCH_TIME TEXT, EST_TIME TEXT, JOINT_FLAG TEXT, JOINT_MASTER_ID TEXT, ROUTING TEXT, ROUTING_ENG TEXT, ROUTING_KA TEXT, VIA TEXT, VIA_ENG TEXT, VIA_KA TEXT, TD_TIME TEXT, TOFF_TIME TEXT, PUB_RMK TEXT, PUB_RMK_ENG TEXT, PUB_RMK_KA TEXT, AIRCRAFT_TYPE TEXT, REGISTRATION TEXT, CHECKIN_1 TEXT, CHECKIN_2 TEXT, GATE_1 TEXT, BAGGAGE_1 TEXT, TERMINAL TEXT);");

                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('1', 'I', 'A', 'ON GATE', 'SV', 'SAUDI AIRLINES', '706', '10102017 132000', '10102017 124400', '', '978379', 'MED', 'MEDINA', '??????? ???????', '', '', '', '10102017 123400', '', 'ON GATE', 'AT GATE', '??? ????????', '', '', '', '', '', '', 'T2');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('2', 'D', 'A', 'CAROUSEL CLOSED', '6S', 'Saudi Gulf Airlines', '642', '10102017 132000', '10102017 132000', '', '978381', 'AHB', 'ABHA', '?????????????', '', '', '', '10102017 133000', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B52', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('3', 'D', 'A', 'ON GATE', 'SV', 'SAUDI AIRLINES', '1736', '10102017 132000', '10102017 130800', '', '978377', 'BHH', 'BISHA', '????????????', '', '', '', '10102017 131200', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B55', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('4', 'D', 'A', 'CAROUSEL CLOSED', 'SV', 'SAUDI AIRLINES', '1508', '10102017 132500', '10102017 131100', '', '978192', 'RAH', 'RAFHA', '????????', '', '', '', '10102017 130400', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B55', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('5', 'D', 'D', 'DEPARTED', 'SV', 'SAUDI AIRLINES', '1915', '10102017 133000', '10102017 140000', '', '978374', 'TIF', 'TAIF', '??????????', '', '', '', '', '10102017 141400', 'DEPART', 'DEPARTED', '??????????????', '', '', '', '', 'GT506A', '', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('6', 'I', 'D', 'DEPARTED', 'PA', 'AIR BLUE', '275', '10102017 133000', '10102017 133000', '', '978355', 'ISB', 'ISLAMABAD', '????? ????', '', '', '', '', '10102017 131900', 'DEPART', 'DEPARTED', '??????????????', '', '', '111', '112', 'GT11', '', 'T1');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('7', 'D', 'A', 'ON GATE', 'SV', 'SAUDI AIRLINES', '1396', '10102017 133000', '10102017 132000', '', '978382', 'AJF', 'AL-JOUF', '?????????????', '', '', '', '10102017 131900', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B54', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('8', 'D', 'D', 'DEPARTED', 'XY', 'FLYNAS', '070', '10102017 133000', '10102017 133000', '', '978332', 'DMM', 'DAMMAM', '????????', '', '', '', '', '10102017 133300', 'DEPART', 'DEPARTED', '??????????????', '', '', '', '', 'GT504B', '', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('9', 'D', 'A', 'CAROUSEL CLOSED', 'XY', 'FLYNAS', '028', '10102017 133500', '10102017 133500', '', '978384', 'JED', 'JEDDAH', '????????', '', '', '', '10102017 132700', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B52', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('10', 'D', 'A', 'ON GATE', 'SV', 'SAUDI AIRLINES', '1030', '10102017 134000', '10102017 132900', '', '978386', 'JED', 'JEDDAH', '????????', '', '', '', '10102017 132100', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B53', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('11', 'D', 'A', 'CAROUSEL CLOSED', 'NA', 'NESMA AIRLINES(NA)', '012', '10102017 134500', '10102017 133700', '', '978388', 'JED', 'JEDDAH', '????????', '', '', '', '10102017 133700', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B54', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('12', 'I', 'A', 'CAROUSEL CLOSED', 'SM', 'AIR CAIRO', '463', '10102017 134500', '10102017 133000', '', '978390', 'HMB', 'SOHAG', '?????', '', '', '', '10102017 131600', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B12', 'T1');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('13', 'D', 'D', 'DEPARTED', 'SV', 'SAUDI AIRLINES', '1805', '10102017 134500', '10102017 114000', '', '978205', 'GIZ', 'JAZAN', '????????', '', '', '', '', '10102017 115900', 'DEPART', 'DEPARTED', '??????????????', '', '', '', '', 'GT505A', '', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('14', 'D', 'A', 'CAROUSEL CLOSED', 'SV', 'SAUDI AIRLINES', '1900', '10102017 135000', '10102017 202600', '', '978274', 'TIF', 'TAIF', '??????????', '', '', '', '10102017 201800', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B53', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('15', 'D', 'D', 'DEPARTED', 'SV', 'SAUDI AIRLINES', '1132', '10102017 135000', '10102017 140500', '', '978378', 'DMM', 'DAMMAM', '????????', '', '', '', '', '10102017 150500', 'DEPART', 'DEPARTED', '??????????????', '', '', '', '', 'GT507A', '', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('16', 'I', 'D', 'DEPARTED', 'SV', 'SAUDI AIRLINES', '554', '10102017 135500', '10102017 135500', '', '978101', 'DXB', 'DUBAI', '???', '', '', '', '', '10102017 140500', 'DEPART', 'DEPARTED', '??????????????', '', '', '', '', 'GT24', '', 'T2');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('17', 'D', 'D', 'DEPARTED', 'SV', 'SAUDI AIRLINES', '1035', '10102017 140000', '10102017 140000', '', '978348', 'JED', 'JEDDAH', '????????', '', '', '', '', '10102017 141000', 'DEPART', 'DEPARTED', '??????????????', '', '', '', '', 'GT510', '', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('18', 'D', 'A', 'CAROUSEL CLOSED', 'XY', 'FLYNAS', '144', '10102017 140000', '10102017 140000', '', '978394', 'URY', 'GURAYAT', '????????????', '', '', '', '10102017 133200', '', 'CAROUSEL CLOSED', 'BELT CLOSED', '????????? ????????', '', '', '', '', '', 'B51', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('19', 'D', 'A', 'CANCELLED', '6S', 'Saudi Gulf Airlines', '322', '10102017 141500', '10102017 141500', '', '978395', 'AHB', 'ABHA', '?????????????', '', '', '', '', '', 'CANCEL', 'CANCELLED', '??????????????????????', '', '', '', '', '', 'B55', 'T5');");
                    //sb.Append("INSERT INTO FID (Id, INT_DOM, ARR_DEP, FL_STATUS_1, AIRLINE, AIRLINE_DESCR, FL_NUMBER, SCH_TIME, EST_TIME, JOINT_FLAG, JOINT_MASTER_ID, ROUTING, ROUTING_ENG, ROUTING_KA, VIA, VIA_ENG, VIA_KA, TD_TIME, TOFF_TIME, PUB_RMK, PUB_RMK_ENG, PUB_RMK_KA, AIRCRAFT_TYPE, REGISTRATION, CHECKIN_1, CHECKIN_2, GATE_1, BAGGAGE_1, TERMINAL) VALUES ('20', 'I', 'A', 'ON GATE', 'SV', 'SAUDI AIRLINES', '811', '10102017 141500', '10102017 141500', '', '979558', 'DAC', 'DHAKA', '???????', '', '', '', '10102017 141900', '', 'BAGGAGE DELIVERY', 'Baggage Est. at', '????? ???????', '', '', '', '', '', '', 'T2');");

                    m_dbConnection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sb.ToString(), m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        #region Scheduler
        public bool UpdateScheduler(string id, string client, string project, string version, string startTime, string endTime, string location, string filelist, string content)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                var sb = new StringBuilder();
                sb.Append($"DELETE FROM Scheduler WHERE Id = '{id}';");
                sb.Append($"INSERT INTO Scheduler values ('{id}', '{client}', '{project}', '{version}', '{startTime}', '{endTime}', '{location}', '{filelist}', '{content}');");
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sb.ToString(), m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            return result;
        }

        public bool UpdateSchedulerFileList(string project, string filelist)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sql = $"UPDATE Scheduler SET FileList = '{filelist}' WHERE Project = '{project}'";
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            return result;
        }

        public bool DeleteScheduler(string id)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sql = $"DELETE FROM Scheduler WHERE Id = '{id}'";
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                result = true;
            }
            return result;
        }

        public string[] GetOldSchedulerFiles(string id)
        {
            List<string> result = new List<string>();
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string clientText = String.Empty;
                string projectText = String.Empty;
                string fileListText = String.Empty;

                m_dbConnection.Open();
                string fullClient = String.Empty;
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = $"SELECT Client, Project, FileList FROM Scheduler WHERE Id = '{id}'";

                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            clientText = Convert.ToString(r["Client"]);
                            projectText = Convert.ToString(r["Project"]);
                            fileListText = Convert.ToString(r["FileList"]);
                            break;
                        }
                    }
                }

                if (String.IsNullOrEmpty(clientText) || String.IsNullOrEmpty(projectText))
                    result.ToArray();

                //Check project file
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = $"SELECT Id, Client, Project, FileList FROM Scheduler WHERE Id <> '{id}' AND Client = '{clientText}' AND Project = '{projectText}'";
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        if (!r.HasRows)
                        {
                            result.Add($"{clientText.Replace('/', '\\')}/{projectText}");//Project file
                            if (!String.IsNullOrEmpty(fileListText))//Other files
                            {
                                string[] values = fileListText.Split(';');
                                foreach (var value in values)
                                {
                                    result.Add($"{clientText.Replace('/', '\\')}/{value}");
                                }

                            }
                        }
                        //else
                        //{
                        //    while (r.Read())
                        //    {
                        //        fileListText = Convert.ToString(r["FileList"]);
                        //        break;
                        //    }
                        //}
                    }
                }
            }
            return result.ToArray();
        }

        public string GetSceduler(string client)
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                string fullClient = String.Empty;
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT Player.Name as Player, 
                        PlayerGroup.Name as PlayerGroup, 
                        Zone.Name as Zone, 
                        Floor.Name as Floor, 
                        Location.Name as Location, 
                        LocationGroup.Name as LocationGroup, 
                        NetworkRegion.Name as NetworkRegion, 
                        Country.Name as Country, 
                        Domain.Name as Domain 
                        FROM Player JOIN PlayerGroup ON PlayerGroup.Id = Player.Parent JOIN Zone ON Zone.Id = PlayerGroup.Parent 
                        JOIN Floor ON Floor.Id = Zone .Parent 
                        JOIN Location ON Location.Id = Floor.Parent 
                        JOIN LocationGroup ON LocationGroup.Id = Location.Parent 
                        JOIN NetworkRegion ON NetworkRegion.Id = LocationGroup.Parent
                        JOIN Country ON Country.Id = NetworkRegion.Parent 
                        JOIN Domain ON Domain.Id = Country.Parent 
                        WHERE Player.Name = '{0}' COLLATE NOCASE LIMIT 1", client);

                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            //fullClient = String.Format(@"Network/{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}/{8}",
                            //    r["Domain"], r["Country"], r["NetworkRegion"], r["LocationGroup"], r["Location"], r["Floor"], r["Zone"], r["PlayerGroup"], r["Player"]);
                            fullClient = String.Format(@"Network/{0}/{1}/{2}/{3}/{4}/{5}/{6}/{7}",
                                r["Domain"], r["Country"], r["NetworkRegion"], r["LocationGroup"], r["Location"], r["Floor"], r["Zone"], r["PlayerGroup"]);

                            break;
                        }
                    }
                }

                if (!String.IsNullOrEmpty(fullClient))
                {
                    using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                    {
                        // Disable ar-SA date time culture
                        CultureInfo _cinfor = System.Threading.Thread.CurrentThread.CurrentCulture;
                        if (_cinfor.Name.ToLower() == "ar-sa")
                            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");


                        //fmd.CommandText = String.Format(@"SELECT * FROM Scheduler WHERE instr('{0}', Client) > 0 AND strftime('%s', '{1}') BETWEEN strftime('%s', StartTime) AND strftime('%s', EndTime) LIMIT 1", fullClient, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        fmd.CommandText = String.Format(@"SELECT * FROM Scheduler WHERE Client = '{0}' AND strftime('%s', '{1}') BETWEEN strftime('%s', StartTime) AND strftime('%s', EndTime) LIMIT 1", fullClient, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        log.Info(fmd.CommandText);

                        fmd.CommandType = CommandType.Text;
                        using (SQLiteDataReader r = fmd.ExecuteReader())
                        {
                            result = "No data";
                            while (r.Read())
                            {
                                result = String.Format("{0},{1},{2},{3},{4},{5},{6}", Convert.ToString(r["Client"]), Convert.ToString(r["Project"]), Convert.ToString(r["Version"]), Convert.ToString(r["StartTime"]), Convert.ToString(r["EndTime"]), Convert.ToString(r["Location"]), Convert.ToString(r["Content"]));
                            }
                        }
                    }
                }
            }
            return result;
        }

        public string GetAllScedulers()
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                string fullClient = String.Empty;

                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    // Disable ar-SA date time culture
                    CultureInfo _cinfor = System.Threading.Thread.CurrentThread.CurrentCulture;
                    if (_cinfor.Name.ToLower() == "ar-sa")
                        System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");


                    fmd.CommandText = String.Format(@"SELECT Id, Client, Project, Version, StartTime, EndTime ,Location, Content FROM Scheduler");
                    log.Info(fmd.CommandText);

                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Scedulers");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Sceduler");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Client", Convert.ToString(r["Client"]));
                                writer.WriteElementString("Project", Convert.ToString(r["Project"]));
                                writer.WriteElementString("Version", Convert.ToString(r["Version"]));
                                writer.WriteElementString("StartTime", Convert.ToString(r["StartTime"]));
                                writer.WriteElementString("EndTime", Convert.ToString(r["EndTime"]));
                                writer.WriteElementString("Location", Convert.ToString(r["Location"]));
                                writer.WriteElementString("Content", Convert.ToString(r["Content"]));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                    }

                }
            }
            return result;
        }
        #endregion

        #region User
        public string Login(string username, string password)
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM User WHERE Name = '{0}' COLLATE NOCASE and Password='{1}' and Active = 1 LIMIT 1", username, password);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        while (r.Read())
                        {
                            using (var writer = XmlWriter.Create(sb))
                            {
                                writer.WriteStartElement("User");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("GroupId", Convert.ToString(r["GroupId"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Password", Convert.ToString(r["Password"]));
                                writer.WriteElementString("Email", Convert.ToString(r["Email"]));
                                writer.WriteElementString("Phone", Convert.ToString(r["Phone"]));
                                writer.WriteElementString("Permissions", Convert.ToString(r["Permissions"]));
                                writer.WriteEndElement();
                            }
                            break;
                        }
                        result = sb.ToString();
                    }
                }
            }
            return result;
        }

        public string GetGroups()
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM UserGroup");
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.ConformanceLevel = ConformanceLevel.Fragment;
                        settings.OmitXmlDeclaration = true;
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb, settings))
                        {
                            writer.WriteStartElement("Groups");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Group");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Permissions", Convert.ToString(r["Permissions"]));
                                writer.WriteString(GetUsers(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                    }
                }
            }
            return result;
        }

        public string GetUsers(string groupId)
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = $"SELECT * FROM User WHERE GroupId ='{groupId}' and User.Active = 1";
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        XmlWriterSettings settings = new XmlWriterSettings();
                        settings.ConformanceLevel = ConformanceLevel.Fragment;
                        settings.OmitXmlDeclaration = true;

                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb, settings))
                        {

                            writer.WriteStartElement("Users");
                            while (r.Read())
                            {
                                writer.WriteStartElement("User");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("GroupId", Convert.ToString(r["GroupId"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Password", Convert.ToString(r["Password"]));
                                writer.WriteElementString("Email", Convert.ToString(r["Email"]));
                                writer.WriteElementString("Phone", Convert.ToString(r["Phone"]));
                                writer.WriteElementString("Permissions", Convert.ToString(r["Permissions"]));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString();
                        //if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38) result = result.Substring(39); 
                        //remove <?xml version="1.0" encoding="utf-16"?>

                        //result = XDocument.Parse(sb.ToString()).Descendants("Users").First().ToString();
                    }
                }
            }
            return result;
        }

        public bool UpdateUser(string id, string groupId, string name, string password, string email, string phone, string permissions)
        {

            bool result = false;
            bool existing = false;

            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = $"SELECT Id FROM User WHERE Id ='{id}' LIMIT 1";
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            if (id == Convert.ToString(r["Id"]))
                                existing = true;
                            break;
                        }
                    }
                }

                if (existing)
                {
                    //Update
                    string sql = $"UPDATE User SET GroupId = '{groupId}', Name = '{name}', Password = '{password}', Email = '{email}', Phone = '{phone}', Permissions = '{permissions}' WHERE Id = '{id}'";
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
                else
                {
                    string sql = $"INSERT INTO User VALUES ('{id}', '{groupId}', '{name}', '{password}', '{email}', '{phone}', '{permissions}', 1)";
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool DeleteUser(string id)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sql = $"DELETE FROM User WHERE Id='{id}'";
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                result = true;
            }
            return result;
        }
        #endregion

        #region Network
        public string GetNetwork()
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Domain");
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Domains");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Domain");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetCountries(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                    }
                }
            }
            return result;
        }

        public string GetCountries(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Country WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Countries");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Country");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetNetworkRegions(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetNetworkRegions(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM NetworkRegion WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Regions");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Region");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetLocationGroups(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetLocationGroups(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM LocationGroup WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("LocationGroups");
                            while (r.Read())
                            {
                                writer.WriteStartElement("LocationGroup");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetLocations(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetLocations(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Location WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Locations");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Location");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetFloors(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetFloors(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Floor WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Floors");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Floor");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetZones(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetZones(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Zone WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Zones");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Zone");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetPlayerGroups(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetPlayerGroups(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM PlayerGroup WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("PlayerGroups");
                            while (r.Read())
                            {
                                writer.WriteStartElement("PlayerGroup");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetPlayers(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetPlayers(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Player WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Players");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Player");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("HostName", Convert.ToString(r["HostName"]));
                                writer.WriteElementString("IPAddress", Convert.ToString(r["IPAddress"]));
                                writer.WriteElementString("RegistredTime", Convert.ToString(r["RegistredTime"]));
                                writer.WriteElementString("Screens", Convert.ToString(r["Screens"]));
                                writer.WriteElementString("Licence", Convert.ToString(r["Licence"]));
                                writer.WriteElementString("Status", Convert.ToString(r["Status"]));
                                writer.WriteElementString("RefreshTime", Convert.ToString(r["RefreshTime"]));
                                writer.WriteElementString("LastConnection", Convert.ToString(r["LastConnection"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetFaceGroups(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetFaceGroups(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/FaceGroup/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM FaceGroup WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("FaceGroups");
                            while (r.Read())
                            {
                                writer.WriteStartElement("FaceGroup");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteString(GetFaces(Convert.ToString(r["Id"])));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        public string GetFaces(string id)
        {   //Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Faces Group/Faces(Display)
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT * FROM Face WHERE Parent ='{0}'", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("Faces");
                            while (r.Read())
                            {
                                writer.WriteStartElement("Face");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString().Replace("&lt;", "<").Replace("&gt;", ">");
                        if (!String.IsNullOrEmpty(result) && result.IndexOf(">") == 38)
                            result = result.Substring(39);
                    }
                }
            }
            return result;
        }

        private int GetLicense(int extra = 0)
        {
            string text = SQLiteHelper.Instance.RegistredPlayerCount();
            log.Info($"License Info:{text}");
            int totalCount;
            bool res = int.TryParse(text, out totalCount);
            log.Info($"License res:{res.ToString()}");
            if (res == false)
            {
                totalCount = 0;
            }
            log.Info($"License Info:{totalCount} extra : {extra}");
            return LicenseValidator.LG.Validate(totalCount + extra);


        }
        private string GetLicenseMessage(int sValue)
        {
            string result = String.Empty;
            switch (sValue)
            {
                case -1:
                    result = "Error: General Exception";
                    break;
                case 0:
                    result = "Error: License information not found or missing license details";
                    break;
                case 1:
                    result = "Error: Invalid license";
                    break;
                case 2:
                    result = "Error: License is expired";
                    break;
                case 3:
                    result = "License is Valid";
                    break;
                case 4:
                    result = "Error: Total player registed is equal to licensed player";
                    break;
                case 5:
                    result = "Error: Service not running";
                    break;
            }
            return result;
        }

        public string UpdateNetwork(string type, string id, string parent, string name, string description, bool active)
        {
            string result = String.Empty;
            bool existing = false;
            string sqlFind = String.Empty;
            string sqlUpdate = String.Empty;
            string sqlInsert = String.Empty;
            switch (type)
            {
                case "Domain":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM Domain WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE Domain SET Name = '{0}', Description = '{1}', Active = {2} WHERE Id = '{3}'", name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO Domain VALUES ('{0}', '{1}', '{2}', {3})",
                        id, name, description, 1);
                    }
                    break;
                case "Country":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM Country WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE Country SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO Country VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "Region":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM NetworkRegion WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE NetworkRegion SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO NetworkRegion VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "LocationGroup":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM LocationGroup WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE LocationGroup SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO LocationGroup VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "Location":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM Location WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE Location SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO Location VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "Floor":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM Floor WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE Floor SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO Floor VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "Zone":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM Zone WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE Zone SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO Zone VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "PlayerGroup":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM PlayerGroup WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE PlayerGroup SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO PlayerGroup VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "Player":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM Player WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE Player SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO Player VALUES ('{0}', '{1}', '{2}', '{3}','','','', 0, 0, 0, 30, '', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "FaceGroup":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM FaceGroup WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE FaceGroup SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO FaceGroup VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
                case "Face":
                    {
                        sqlFind = String.Format(@"SELECT Id FROM Face WHERE Id ='{0}' LIMIT 1", id);
                        sqlUpdate = String.Format(@"UPDATE Face SET Parent = '{0}', Name = '{1}', Description = '{2}', Active = {3} WHERE Id = '{4}'",
                        parent, name, description, active ? 1 : 0, id);
                        sqlInsert = String.Format(@"INSERT INTO Face VALUES ('{0}', '{1}', '{2}', '{3}', {4})",
                        id, parent, name, description, 1);
                    }
                    break;
            }

            if (String.IsNullOrEmpty(sqlFind))
                return String.Empty;

            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = sqlFind;
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            if (id == Convert.ToString(r["Id"]))
                                existing = true;
                            break;
                        }
                    }
                }

                if (existing)
                {
                    using (SQLiteCommand command = new SQLiteCommand(sqlUpdate, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = "Success";
                    }
                }
                else
                {
                    if (type.ToLower() == "player")
                    {
                        log.Info(String.Format("GetAllScedulers"));
                        int sValue = GetLicense(1);
                        log.Info($"Arif License Info:{sValue}");
                        if (sValue != 3)
                        {
                        
                            log.Info(String.Format("LicenseValidator: {0}", sValue));
                            result = String.Format("License: {0}", GetLicenseMessage(sValue));
                            return result;
                        }

                    }
                    using (SQLiteCommand command = new SQLiteCommand(sqlInsert, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = "Success";
                    }
                }
            }
            return result;
        }

        public bool DeleteNetwork(string type, string id)
        {
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                ////Domain/Country/Region/Location Group/location/Floors/Zone/Player group/Player/Face Group/Faces(Display)
                string sql = String.Empty;
                switch (type)
                {
                    case "Domain":
                        {
                            sql = String.Format("DELETE FROM Domain WHERE Id='{0}'", id);
                        }
                        break;
                    case "Country":
                        {
                            sql = String.Format("DELETE FROM Country WHERE Id='{0}'", id);
                        }
                        break;
                    case "Region":
                        {
                            sql = String.Format("DELETE FROM NetworkRegion WHERE Id='{0}'", id);
                        }
                        break;
                    case "LocationGroup":
                        {
                            sql = String.Format("DELETE FROM LocationGroup WHERE Id='{0}'", id);
                        }
                        break;
                    case "Location":
                        {
                            sql = String.Format("DELETE FROM Location WHERE Id='{0}'", id);
                        }
                        break;
                    case "Floor":
                        {
                            sql = String.Format("DELETE FROM Floor WHERE Id='{0}'", id);
                        }
                        break;
                    case "Zone":
                        {
                            sql = String.Format("DELETE FROM Zone WHERE Id='{0}'", id);
                        }
                        break;
                    case "PlayerGroup":
                        {
                            sql = String.Format("DELETE FROM PlayerGroup WHERE Id='{0}'", id);
                        }
                        break;
                    case "Player":
                        {
                            sql = String.Format("DELETE FROM Player WHERE Id='{0}'", id);
                        }
                        break;
                    case "FaceGroup":
                        {
                            sql = String.Format("DELETE FROM FaceGroup WHERE Id='{0}'", id);
                        }
                        break;
                    case "Face":
                        {
                            sql = String.Format("DELETE FROM Face WHERE Id='{0}'", id);
                        }
                        break;
                }

                if (String.IsNullOrEmpty(sql))
                    return false;
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                }
            }
            return true;
        }

        public bool CheckPlayerName(string name)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format("SELECT Name FROM Player WHERE Name = '{0}' COLLATE NOCASE AND Active = 1 LIMIT 1", name);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string client = Convert.ToString(r["Name"]);
                            if (!String.IsNullOrEmpty(client))
                            {
                                result = true;
                            }
                            break;
                        }
                    }
                }
            }
            return result;
        }

        //public bool CheckPlayerExist(string name, string hostName, string ipAddress, int licence)
        //{
        //    bool result = false;
        //    using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
        //    {
        //        m_dbConnection.Open();
        //        using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
        //        {
        //            fmd.CommandText = String.Format("SELECT Name FROM Player WHERE Name = '{0}' COLLATE NOCASE AND HostName = '{1}' COLLATE NOCASE AND IpAddress = '{2}' COLLATE NOCASE AND Licence >= {3} LIMIT 1", name, hostName, ipAddress, licence);
        //            fmd.CommandType = CommandType.Text;
        //            using (SQLiteDataReader r = fmd.ExecuteReader())
        //            {
        //                while (r.Read())
        //                {
        //                    string client = Convert.ToString(r["Name"]);
        //                    if (!String.IsNullOrEmpty(client))
        //                    {
        //                        result = true;
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        public bool CheckPlayerExist(string name, string hostName, string ipAddress, int licence)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format("SELECT Name,IpAddress,Active FROM Player WHERE Name = '{0}' COLLATE NOCASE AND Active = 1  LIMIT 1", name);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string client = Convert.ToString(r["Name"]);
                            string ipaddress = Convert.ToString(r["IpAddress"]);
                            string active = Convert.ToString(r["Active"]);
                            if (active == "1")
                            {
                                log.Info($"palyer: { client} passed ipaddress: {ipAddress} stored in DB ipaddress {ipaddress} ");
                                if (!String.IsNullOrEmpty(client))
                                {
                                    if (ipaddress == ipAddress || ipaddress == "")
                                        result = true;
                                    else
                                        return false;
                                }
                                else
                                    log.Info($"palyer name is : { client} ");
                            }
                            else
                            {
                                log.Info($"palyer { client}  is not active , its status is { active}");
                                result = false;
                            }

                            break;
                        }
                    }
                }
            }

            return result;
        }

        public string GetPlayerName(string hostName, string ipAddress)
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT Player.Name AS Player, PlayerGroup.Name AS PlayerGroup FROM Player, PlayerGroup Where Player.Parent = PlayerGroup.Id AND Player.HostName = '{0}' COLLATE NOCASE AND Player.IPAddress = '{1}' COLLATE NOCASE AND Player.Licence > 0 AND Player.Active = 1", hostName, ipAddress);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            result = String.Format("{0};{1}", Convert.ToString(r["PlayerGroup"]), Convert.ToString(r["Player"]));
                            break;
                        }
                    }
                }
            }
            return result;
        }
        public string GetPlayerInfo(string playername, string hostName, string ipAddress)
        {
            string result = String.Empty;
            try
            {
                using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
                {

                    log.Info(string.Format("GetPlayerInfo Opening connection with {0}", databaseFile));
                    m_dbConnection.Open();
                    log.Info("GetPlayerInfo Connection Opended ");
                    using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                    {
                        fmd.CommandText = String.Format(@"SELECT HostName, IPAddress, Status  From Player WHERE Name = '{0}' COLLATE NOCASE AND Active = 1  LIMIT 1", playername);
                        fmd.CommandType = CommandType.Text;
                        using (SQLiteDataReader r = fmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                string HostName = Convert.ToString(r["HostName"]);
                                string ipaddress = Convert.ToString(r["IpAddress"]);
                                string status = Convert.ToString(r["Status"]);
                                if (ipaddress != ipAddress)//|| ipaddress != "")
                                {
                                    result = "Player already registerd with another IP Address";

                                }
                                else if (status == "4")
                                {
                                    result = "Player was unregistered from the server, try to input the player name again";
                                }
                                else
                                {
                                    r.Close();
                                    m_dbConnection.Close();
                                    UpdatePlayerStatusWithDetails(playername, 1, 3, hostName, ipAddress);
                                   
                                }

                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                
              log.Info(String.Format("GetPlayerInfo Exception : {0}", ex.Message));
            }
            return result;
        }

        public bool RegistratePlayer(string name, string hostName, string ipAddress, int screens, int licence, int status)
        {
            bool result = false;
            bool bResult = CheckPlayerExist(name, hostName, ipAddress, licence);
            log.Info($"CheckPlayerExist:{bResult.ToString()}");
            if (CheckPlayerExist(name, hostName, ipAddress, licence))
            {
                using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
                {
                    string sqlUpdate = $"UPDATE Player SET HostName = '{hostName}', IPAddress = '{ipAddress}', RegistredTime = datetime(CURRENT_TIMESTAMP, 'localtime'), Screens = {screens}, Licence = {licence}, Status = {status}, LastConnection = datetime(CURRENT_TIMESTAMP, 'localtime') WHERE Name = '{name}' COLLATE NOCASE";
                    m_dbConnection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sqlUpdate, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            log.Info($"CheckPlayerExist return :{result.ToString()}");
            return result;
        }

        public bool UpdatePlayerStatus(string name, int screens, int status)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sqlUpdate = $"UPDATE Player SET LastConnection = datetime(CURRENT_TIMESTAMP, 'localtime'), Screens = {screens}, Status = {status} WHERE Name = '{name}' COLLATE NOCASE";
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sqlUpdate, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            return result;
        }

        private bool UpdatePlayerStatusWithDetails(string name, int screens, int status, string hostname, string Ipaddress)
        {
            bool result = false;
            try
            {
                using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
                {
                    string sqlUpdate = $"UPDATE Player SET LastConnection = datetime(CURRENT_TIMESTAMP, 'localtime'), Screens = {screens}, Status = {status}, HostName = '{hostname}', IPAddress= '{Ipaddress}'  WHERE Name = '{name}' COLLATE NOCASE";
                    m_dbConnection.Open();
                    using (SQLiteCommand command = new SQLiteCommand(sqlUpdate, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                log.Info(String.Format("UpdatePlayerStatusWithDetails  Exception: {0}", ex.Message));
            }
            return result;
        }

        public string GetPlayerRegresh(string name)
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = $"SELECT RefreshTime FROM Player WHERE Name = '{name}' COLLATE NOCASE";
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            result = Convert.ToString(r["RefreshTime"]);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public bool UpdatePlayerRefresh(string name, int refreshTime)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sqlUpdate = $"UPDATE Player SET RefreshTime = {refreshTime} WHERE Name = '{name}' COLLATE NOCASE";
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sqlUpdate, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            return result;
        }

        //public string RegistredPlayerCount()
        //{
        //    string result = String.Empty;
        //    using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
        //    {
        //        m_dbConnection.Open();
        //        using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
        //        {
        //            fmd.CommandText = "SELECT SUM(Licence) AS 'LicenceCount' FROM Player WHERE Active = 1";

        //            fmd.CommandType = CommandType.Text;
        //            using (SQLiteDataReader r = fmd.ExecuteReader())
        //            {
        //                while (r.Read())
        //                {
        //                    string client = Convert.ToString(r["LicenceCount"]);
        //                    if (!String.IsNullOrEmpty(client))
        //                    {
        //                        result = client;
        //                    }
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //    return String.IsNullOrEmpty(result) ? "0" : result;
        //}

        public string RegistredPlayerCount()
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = "SELECT Count(Name) AS 'LicenceCount' FROM Player WHERE Active = 1";

                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            string client = Convert.ToString(r["LicenceCount"]);
                            if (!String.IsNullOrEmpty(client))
                            {
                                result = client;
                            }
                            break;
                        }
                    }
                }
            }
            return String.IsNullOrEmpty(result) ? "0" : result;
        }

        public int GetPlayerLicence(string name)
        {
            int result = -1;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = $"SELECT Status FROM Player Where Name = '{name}' COLLATE NOCASE AND Active = 1";
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            result = Convert.ToInt32(Convert.ToString(r["Status"]));
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public bool StopPlayer(string name)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sqlUpdate = $"UPDATE Player SET Status = 5, LastConnection = datetime(CURRENT_TIMESTAMP, 'localtime') WHERE Name = '{name}' COLLATE NOCASE";
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sqlUpdate, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            return result;
        }

        public bool DisconectPlayer(string name)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sqlUpdate = $"UPDATE Player SET Screens = 0, Licence = 0, Status = 4, RefreshTime = 30, RegistredTime = null,HostName=null,IPAddress=null, LastConnection = null WHERE Name = '{name}' COLLATE NOCASE";
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sqlUpdate, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            return result;
        }

        #endregion

        #region Data
        public string GetDataSources()
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT Id, Name, Columns, Description, Type, Url, UrlUsername, UrlPassword FROM DataSource Where Active = 1");
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        using (var writer = XmlWriter.Create(sb))
                        {
                            writer.WriteStartElement("DataSources");
                            while (r.Read())
                            {
                                writer.WriteStartElement("DataSource");
                                writer.WriteElementString("Id", Convert.ToString(r["Id"]));
                                writer.WriteElementString("Name", Convert.ToString(r["Name"]));
                                writer.WriteElementString("Columns", Convert.ToString(r["Columns"]));
                                writer.WriteElementString("Description", Convert.ToString(r["Description"]));
                                writer.WriteElementString("Type", Convert.ToString(r["Type"]));
                                writer.WriteElementString("Url", Convert.ToString(r["Url"]));
                                writer.WriteElementString("UrlUsername", Convert.ToString(r["UrlUsername"]));
                                writer.WriteElementString("UrlPassword", Convert.ToString(r["UrlPassword"]));
                                //writer.WriteElementString("Active", Convert.ToString(r["Active"]));
                                writer.WriteEndElement();
                            }
                            writer.WriteEndElement();
                        }
                        result = sb.ToString();
                    }
                }
            }
            return result;
        }

        public bool InsertDataSource(string id, string name, string columns, string description, string type, string data, string url, string urlUsername, string urlPassword)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sql = String.Format("INSERT INTO DataSource values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', 1);", id, name, columns, description, type, data, url, urlUsername, urlPassword);
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                    result = true;
                }
            }
            return result;
        }

        public bool UpdateDataSource(string id, string name, string columns, string description, string type, string data, string url, string urlUsername, string urlPassword)
        {

            bool result = false;
            bool existing = false;

            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = String.Format(@"SELECT Id FROM DataSource WHERE Id ='{0}' LIMIT 1", id);
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            if (id == Convert.ToString(r["Id"]))
                                existing = true;
                            break;
                        }
                    }
                }

                if (existing)
                {
                    //sb.Append("CREATE TABLE DataSource (Id VARCHAR(30) PRIMARY KEY, Name TEXT NOT NULL, Columns TEXT, Description Text, Type Text, Data Text, Url Text, UrlUsername Text, UrlPassword Text, Active INTEGER DEFAULT 1);");
                    //Update
                    string sql = String.Format(@"UPDATE DataSource SET Name = '{1}', Columns ='{2}', Description='{3}', Type='{4}', Data='{5}', Url='{6}', UrlUsername='{7}', UrlPassword='{8}' WHERE Id = '{0}'"
                        , id, name, columns, description, type, data, url, urlUsername, urlPassword);
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
                else
                {
                    string sql = String.Format("INSERT INTO DataSource values ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', 1);", id, name, columns, description, type, data, url, urlUsername, urlPassword);
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool DeleteDataSource(string id)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sql = String.Format("DELETE FROM DataSource WHERE Id='{0}'", id);
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                result = true;
            }
            return result;
        }

        public string GetDataRecord(string id)
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = $"SELECT Data FROM DataSource WHERE Id = '{id}'";
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            result = Convert.ToString(r["Data"]);
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public bool UpdateLogo(string id, string description, string location, long size)
        {
            bool result = false;
            bool existing = false;

            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = $"SELECT Id FROM Logo WHERE Id ='{id}' LIMIT 1";
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            if (id == Convert.ToString(r["Id"]))
                                existing = true;
                            break;
                        }
                    }
                }

                if (existing)
                {
                    //sb.Append("CREATE TABLE Logo (Id VARCHAR(2) PRIMARY KEY, Description Text NOT NULL, Location Text, Image BLOB, Active INTEGER DEFAULT 1);");
                    //Update
                    string sql = String.Format(@"UPDATE Logo SET Description = '{1}', FileLocation = '{2}', FileSize = {3} WHERE Id = '{0}'"
                        , id, description, location, size);
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
                else
                {
                    string sql = String.Format("INSERT INTO Logo values ('{0}', '{1}', '{2}', {3}, 1);", id, description, location, size);
                    using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                    {
                        command.ExecuteNonQuery();
                        result = true;
                    }
                }
            }
            return result;
        }

        public bool DeleteLogo(string id)
        {
            bool result = false;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                string sql = String.Format("DELETE FROM Logo WHERE Id='{0}'", id);
                m_dbConnection.Open();
                using (SQLiteCommand command = new SQLiteCommand(sql, m_dbConnection))
                {
                    command.ExecuteNonQuery();
                }
                result = true;
            }
            return result;
        }

        //sb.Append("CREATE TABLE Logo (Id VARCHAR(3) PRIMARY KEY, Description TEXT NOT NULL, FileLocation TEXT NOT NULL, FileSize INTEGER NOT NULL, Active INTEGER DEFAULT 1);");
        public string GetLogos()
        {
            string result = String.Empty;
            using (SQLiteConnection m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3;", databasePath)))
            {
                m_dbConnection.Open();
                using (SQLiteCommand fmd = m_dbConnection.CreateCommand())
                {
                    fmd.CommandText = @"SELECT Id, Description, FileLocation, FileSize FROM Logo";
                    fmd.CommandType = CommandType.Text;
                    using (SQLiteDataReader r = fmd.ExecuteReader())
                    {
                        var sb = new StringBuilder();
                        while (r.Read())
                        {
                            sb.Append(String.Format("{0}{1};", Convert.ToString(r["Id"]), Convert.ToString(r["FileSize"])));
                        }
                        result = sb.ToString();
                    }
                }
            }
            return result;
        }

        #endregion
    }
}