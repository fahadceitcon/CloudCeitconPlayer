using Ceitcon_Data.Model.Data;
using Ceitcon_Data.Model.Network;
using Ceitcon_Data.Model.User;
using Ceitcon_Designer.CeitconServerService;
using Ceitcon_Designer.View;
using log4net;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using System.Xml.Linq;
using Telerik.Windows.Controls.ScheduleView;

namespace Ceitcon_Designer.Utilities
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public static class CeitconServerHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string Address = String.Empty; //@"http://localhost:8733/Design_Time_Addresses/CeitconServer/Service1/";

        public static bool TestConnection()
        {
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                return !String.IsNullOrWhiteSpace(service.TestConnection());
            }
            catch (Exception)
            {
                return false;
            }
        }


        #region Scheduler
        public static bool UploadScheduler(Appointment item, out string errorResponse, IProgressable prog = null)
        {
            bool result = false;
            CeitconServerClient service = null;

            errorResponse = String.Empty;
            try
            {
                string path = Path.Combine(Environment.CurrentDirectory, "Projects", item.Location);
                if (!Directory.Exists(path))
                {
                    errorResponse = String.Format(@"Error: Missing projct directory {0}", path);
                    return false;
                }

                string projectFile = Directory.GetFiles(path, "*.cdp").FirstOrDefault();
                if (!File.Exists(projectFile))
                {
                    errorResponse = String.Format(@"Error: Missing projct file {0}", projectFile);
                    return false;
                }

                //Get file for uploaded
                string[] contentFiles = Ceitcon_Data.Utilities.IOManagerProject.GetContents(projectFile);
                string contentFileList = Ceitcon_Data.Utilities.IOManagerProject.GetContentsString(contentFiles);

                log.Info($"Content Files {contentFileList}");
                //Check files on server
                var sb = new StringBuilder();
                foreach (var c in contentFiles)
                {
                    
                    if (sb.Length > 0)
                        sb.Append('$');
                    sb.Append(c);
                }

                //57bc7cfa-8e40-49bb-b5c5-5f6cb2b54c03.jpg;13766$3f299523-b4e0-41cb-9418-f98f2e7e880f.jpg;235035
                service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);

                if (sb.Length > 0)
                {
                    string resp = service.CheckUploadSceduler(item.Subject, sb.ToString());
                    if (resp.StartsWith("Error"))
                    {
                        errorResponse = resp;
                        return false;

                    }
                    else if (String.IsNullOrEmpty(resp))
                    {
                        contentFiles = new List<string>().ToArray();
                    }
                    else
                    {
                        contentFiles = resp.Split('$');
                    }
                }

                List<string> list = new List<string>();
                foreach (string file in contentFiles)
                {
                    int v = file.IndexOf(';');
                    string fileName = file.Substring(0, v);
                    list.Add(Path.Combine(path, fileName));
                }
                //list.Add(projectFile); 

                int progress = 0;
                int progressStep = list.Count == 0 ? 100 : 100 / list.Count + 1; //+1 for cdp on the end
                int iCount = 0;

                RemoteFileInfo uploadRequestInfo;
                //Upload contents 
                foreach (string fileName in list)
                {
                    if (prog != null)
                    {
                        (prog as ProgressWindow).SafeSetProgress(progress, String.Format("Uploading {0}", Path.GetFileName(fileName)));
                        Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.Background, new ThreadStart(delegate { }));
                        progress += progressStep;
                    }
                    iCount++;
             

                    #region "New Chunks Upload"

                    string guid = Guid.NewGuid().ToString();

                    // amount of data to be sent in each service call 
                    const int chunkSize = 8388608; //8MB chunk
                    byte[] fileData = new byte[chunkSize];
                    FileInfo fsInfor = new FileInfo(fileName);
                    string _fileName = fsInfor.Name;
                    string sExtention = fsInfor.Extension;
                    long _maxlenght = fsInfor.Length;
                    fsInfor = null;
                    guid = guid + sExtention;
                    int iResult = 0;

                    using (FileStream fs = File.OpenRead(fileName))
                    {
                        int bytesRead = 0;
                        long offset = service.GetPreviosOffSet(_fileName, item.Subject);
                        if (offset < _maxlenght)
                        {
                            fs.Seek(offset, SeekOrigin.Begin);
                        }

                        uploadRequestInfo = new RemoteFileInfo();
                        uploadRequestInfo.Id = item.UniqueId;
                        uploadRequestInfo.Client = item.Subject;
                        uploadRequestInfo.FileName = Path.GetFileName(fileName);
                        uploadRequestInfo.Length = fs.Length;
                        uploadRequestInfo.Version = item.Url;
                        uploadRequestInfo.Location = item.Location;

                        // keep reading and sending the chunks over
                        while ((bytesRead = fs.Read(fileData, 0, fileData.Length)) != 0)
                        {
                            uploadRequestInfo.BufferSize = bytesRead;
                            uploadRequestInfo.DataArray = fileData;
                            SyncResponse response = service.UploadScedulerChunk(uploadRequestInfo);
                            iResult = response.Status;

                            if (iResult == 2) // 2 mean the the file is already copied to the server
                                break;
                        }
                    }
                    if (iResult == 1 || iResult == 2)
                        result = true;
                    #endregion

                    if (!result)
                    {
                        service.ClearDictory();
                        return false;
                    }
                    else
                    {
                        if (iCount == list.Count)
                        {
                            service.ClearDictory();
                        }
                    }
                }

                //Upload CDP file 
                uploadRequestInfo = new RemoteFileInfo();
                uploadRequestInfo.Id = item.UniqueId;
                uploadRequestInfo.Client = item.Subject;
                uploadRequestInfo.Start = item.Start.ToString("yyyy-MM-dd HH:mm:ss");
                uploadRequestInfo.End = item.End.ToString("yyyy-MM-dd HH:mm:ss");
                uploadRequestInfo.FileName = Path.GetFileName(projectFile);
                uploadRequestInfo.Version = item.Url;
                uploadRequestInfo.Location = item.Location;
                uploadRequestInfo.FileList = contentFileList;
                uploadRequestInfo.Content = File.ReadAllText(projectFile);
                result = service.UploadScedulerCDP(uploadRequestInfo);
            }
            catch (Exception e)
            {
                errorResponse = String.Format(@"Error: {0}", e.Message);
                log.Error("UploadScheduler", e);
                if(service != null)
                    service.ClearDictory();
                return false;
            }
             return result;
        }

        public static ObservableCollection<Appointment> GetAllSchedularts()
        {
            var collection = new ObservableCollection<Appointment>();
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.GetAllScedulers();
                log.Info($"GetAllSchedularts Response:{response}");
                XDocument xdoc = XDocument.Parse(response);
                foreach (var item in xdoc.Descendants("Scedulers").First().Descendants("Sceduler"))
                {
                    var appoitment = new Appointment();
                    appoitment.UniqueId = item.Element("Id").Value;
                    appoitment.Subject = item.Element("Client").Value;
                    appoitment.Body = item.Element("Project").Value;
                    appoitment.Url = item.Element("Version").Value;
                    appoitment.Start = Convert.ToDateTime(item.Element("StartTime").Value);
                    appoitment.End = Convert.ToDateTime(item.Element("EndTime").Value);
                    appoitment.Location = item.Element("Location").Value;
                    collection.Add(appoitment);
                }
            }
            catch (Exception ex)
            {
                log.Info($"GetAllSchedularts Error:{ex.Message}");

                return collection;
            }
            return collection;
        }

        public static bool DeleteScheduler(Appointment item)
        {
            var service = new CeitconServerClient();
            service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
            string response = service.DeleteSceduler(item.UniqueId);
            return (response == "Success");
        }
        #endregion

        #region User
        public static UserModel LoginUser(string username, string password, out string errorResponse)
        {
            UserModel user = null;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.LoginUser(username, password);
                if (String.IsNullOrEmpty(response) || response.StartsWith("Error"))
                {
                    errorResponse = response;
                    return null;
                }
                XDocument xdoc = XDocument.Parse(response);
                XElement item = xdoc.Descendants("User").First();
                user = new UserModel(new GroupModel());
                user.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                user.Parent.Id = item.Element("GroupId") == null ? "" : item.Element("GroupId").Value;
                user.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                user.Password = item.Element("Password") == null ? "" : item.Element("Password").Value;
                user.Email = item.Element("Email") == null ? "" : item.Element("Email").Value;
                user.Phone = item.Element("Phone") == null ? "" : item.Element("Phone").Value;
                user.SetPermissions(item.Element("Permissions").Value);
                errorResponse = String.Empty;
            }
            catch (Exception e)
            {
                errorResponse = e.Message;
                return null;
            }
            return user;
        }

        public static bool DeleteUser(UserModel item)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.DeleteUser(item.Id);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        private static ObservableCollection<UserModel> LoadUsers(XElement xml, GroupModel parent)
        {
            var collection = new ObservableCollection<UserModel>();
            foreach (var item in xml.Descendants("User"))
            {
                UserModel user = new UserModel(parent);
                user.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                user.Parent.Id = item.Element("GroupId") == null ? "" : item.Element("GroupId").Value;
                user.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                user.Password = item.Element("Password") == null ? "" : item.Element("Password").Value;
                user.Email = item.Element("Email") == null ? "" : item.Element("Email").Value;
                user.Phone = item.Element("Phone") == null ? "" : item.Element("Phone").Value;
                user.SetPermissions(item.Element("Permissions").Value);
                collection.Add(user);
            }
            return collection;
        }

        public static bool UpdateUser(UserModel user)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.UpdateUser(user.Id, user.Parent.Id, user.Name, user.Password, user.Email, user.Phone, user.GetPermissions());
                result = (response == "Success");
            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }
        #endregion

        #region Group
        public static ObservableCollection<GroupModel> GetGroups()
        {
            var collection = new ObservableCollection<GroupModel>();
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.GetGroups();
                XDocument xdoc = XDocument.Parse(response);
                foreach (var item in xdoc.Descendants("Groups").First().Descendants("Group"))
                {
                    GroupModel group = new GroupModel();
                    group.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                    group.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                    group.SetPermissions(item.Element("Permissions").Value);
                    group.Users = LoadUsers(item.Descendants("Users").First(), group);
                    collection.Add(group);
                }
            }
            catch (Exception)
            {
                return collection;
            }

            return collection;
        }

        private static ObservableCollection<GroupModel> LoadGroups(XElement xml)
        {
            var collection = new ObservableCollection<GroupModel>();
            foreach (var item in xml.Descendants("Group"))
            {
                GroupModel group = new GroupModel();
                group.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                group.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                group.SetPermissions(item.Element("Permissions").Value);
                group.Users = LoadUsers(item.Descendants("Users").First(), group);
                collection.Add(group);
            }
            return collection;
        }
        #endregion

        #region Network
        public static ObservableCollection<NetworkModel> GetNetwork()
        {
            var collection = new ObservableCollection<NetworkModel>();
            try
            {
                var model = new NetworkModel() { };
                model.Domains = GetDomain(model);
                collection.Add(model);
            }
            catch (Exception) { }

            return collection;
        }

        public static ObservableCollection<DomainModel> GetDomain(NetworkModel parent)
        {
            var collection = new ObservableCollection<DomainModel>();
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.GetNetwork();
                //string response = SQLiteHelper.Instance.GetNetwork();

                XDocument xdoc = XDocument.Parse(response);
                foreach (var item in xdoc.Descendants("Domains").First().Descendants("Domain"))
                {
                    var model = new DomainModel(parent);
                    model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                    model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                    model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                    model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                    model.Countries = LoadCountries(item.Descendants("Countries").First(), model);
                    collection.Add(model);
                }
            }
            catch (Exception)
            {
                return collection;
            }

            return collection;
        }

        private static ObservableCollection<CountryModel> LoadCountries(XElement xml, DomainModel parent)
        {
            var collection = new ObservableCollection<CountryModel>();
            foreach (var item in xml.Descendants("Country"))
            {
                var model = new CountryModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.Regions = LoadNetworkRegions(item.Descendants("Regions").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<NetworkRegionModel> LoadNetworkRegions(XElement xml, CountryModel parent)
        {
            var collection = new ObservableCollection<NetworkRegionModel>();
            foreach (var item in xml.Descendants("Region"))
            {
                var model = new NetworkRegionModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.LocationGroups = LoadLocationGroups(item.Descendants("LocationGroups").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<LocationGroupModel> LoadLocationGroups(XElement xml, NetworkRegionModel parent)
        {
            var collection = new ObservableCollection<LocationGroupModel>();
            foreach (var item in xml.Descendants("LocationGroup"))
            {
                var model = new LocationGroupModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.Locations = LoadLocations(item.Descendants("Locations").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<LocationModel> LoadLocations(XElement xml, LocationGroupModel parent)
        {
            var collection = new ObservableCollection<LocationModel>();
            foreach (var item in xml.Descendants("Location"))
            {
                var model = new LocationModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.Floors = LoadFloors(item.Descendants("Floors").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<FloorModel> LoadFloors(XElement xml, LocationModel parent)
        {
            var collection = new ObservableCollection<FloorModel>();
            foreach (var item in xml.Descendants("Floor"))
            {
                var model = new FloorModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.Zones = LoadZones(item.Descendants("Zones").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<ZoneModel> LoadZones(XElement xml, FloorModel parent)
        {
            var collection = new ObservableCollection<ZoneModel>();
            foreach (var item in xml.Descendants("Zone"))
            {
                var model = new ZoneModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.PlayerGroups = LoadPlayerGroups(item.Descendants("PlayerGroups").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<PlayerGroupModel> LoadPlayerGroups(XElement xml, ZoneModel parent)
        {
            var collection = new ObservableCollection<PlayerGroupModel>();
            foreach (var item in xml.Descendants("PlayerGroup"))
            {
                var model = new PlayerGroupModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.Players = LoadPlayers(item.Descendants("Players").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<PlayerModel> LoadPlayers(XElement xml, PlayerGroupModel parent)
        {
            var collection = new ObservableCollection<PlayerModel>();
            foreach (var item in xml.Descendants("Player"))
            {
                var model = new PlayerModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.HostName = item.Element("HostName") == null || String.IsNullOrEmpty(item.Element("HostName").Value) ? null : item.Element("HostName").Value;
                model.IPAddress = item.Element("IPAddress") == null || String.IsNullOrEmpty(item.Element("HostName").Value) ? null : item.Element("IPAddress").Value;
                model.RegistredTime = item.Element("RegistredTime") == null || String.IsNullOrEmpty(item.Element("RegistredTime").Value) ? (DateTime?)null : DateTime.Parse(item.Element("RegistredTime").Value);
                model.Licence =  item.Element("Licence") == null || String.IsNullOrEmpty(item.Element("Licence").Value) ? 0 : Convert.ToInt32(item.Element("Licence").Value);
                model.Screens = item.Element("Screens") == null || String.IsNullOrEmpty(item.Element("Screens").Value) ? 0 : Convert.ToInt32(item.Element("Screens").Value);
                model.Status = item.Element("Status") == null || String.IsNullOrEmpty(item.Element("Status").Value) ? 0 : Convert.ToInt32(item.Element("Status").Value);
                model.RefreshTime = item.Element("RefreshTime") == null || String.IsNullOrEmpty(item.Element("RefreshTime").Value) ? 0 : Convert.ToInt32(item.Element("RefreshTime").Value);
                model.LastConnection = item.Element("LastConnection") == null || String.IsNullOrEmpty(item.Element("LastConnection").Value) ? (DateTime?)null : DateTime.Parse(item.Element("LastConnection").Value);
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.FaceGroups = LoadFaceGroups(item.Descendants("FaceGroups").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<FaceGroupModel> LoadFaceGroups(XElement xml, PlayerModel parent)
        {
            var collection = new ObservableCollection<FaceGroupModel>();
            foreach (var item in xml.Descendants("FaceGroup"))
            {
                var model = new FaceGroupModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                model.Faces = LoadFaces(item.Descendants("Faces").First(), model);
                collection.Add(model);
            }
            return collection;
        }

        private static ObservableCollection<FaceModel> LoadFaces(XElement xml, FaceGroupModel parent)
        {
            var collection = new ObservableCollection<FaceModel>();
            foreach (var item in xml.Descendants("Face"))
            {
                var model = new FaceModel(parent);
                model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                model.Description = item.Element("Description") == null ? "" : item.Element("Description").Value;
                model.Active = item.Element("Active") == null ? false : item.Element("Active").Value == "1" ? true : false;
                collection.Add(model);
            }
            return collection;
        }

        public static bool DeleteNetwork(string type, string id)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.DeleteNetwork(type, id);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public static bool UpdateNetwork(string type, string id, string parentId, string name, string description, bool activ)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.UpdateNetwork(type, id, parentId, name, description, activ);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return result;
            }

            return result;
        }

        public static bool CheckFreeLicence(int licence)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.CheckFreeLicence(licence);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public static bool RegistratePlayer(string name, string hostName, string ipAddress, int screens, int licence, int status)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.RegistratePlayer(name, hostName, ipAddress, screens, licence, status);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public static bool DisconectPlayer(string name)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.DisconnectPlayer(name);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public static bool UpdatePlayerRefresh(string name, int refreshTime)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.UpdatePlayerRefresh(name, refreshTime);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public static string GetWeathers(string locations)
        {
            string result = String.Empty;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                result = service.GetWeathers(locations);
            }
            catch (Exception)
            {
            }
            return result;
        }

        #region FIDS
        public static DataGridModel GetDataSources()
        {
            DataGridModel dg = new DataGridModel();
            try
            {
                string response = String.Empty;
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                response = service.GetDataSources();
                if (!String.IsNullOrEmpty(response))
                {
                    var collection = new ObservableCollection<DataSourceModel>();
                    XDocument xdoc = XDocument.Parse(response);
                    //dg.Id = xdoc.Descendants("Id").First().ToString();
                    //dg.Title = xdoc.Descendants("Name").First().ToString();
                    foreach (var item in xdoc.Descendants("DataSources").First().Descendants("DataSource"))
                    {
                        var model = new DataSourceModel();
                        model.Id = item.Element("Id") == null ? "" : item.Element("Id").Value;
                        model.Name = item.Element("Name") == null ? "" : item.Element("Name").Value;
                        model.Columns = new ObservableCollection<DataColumnModel>();
                        string s = item.Element("Columns") == null ? "" : item.Element("Columns").Value;
                        string[] values = s.Split(';');

                        //Add extracolumn
                        //model.Columns.Add(new DataColumnModel() { Name = "FLIGHT", Title = "FLIGHT" });
                        foreach (string i in values)
                        {
                            // if(i != "AIRLINE" && i != "FL_NUMBER")
                            model.Columns.Add(new DataColumnModel() { Name = i, Title = i });
                        }

                        //model.T = item.Element("Type") == null ? "" : item.Element("Type").Value;
                        model.Source = item.Element("Description") == null ? "" : item.Element("Description").Value;
                        model.Url = item.Element("Url") == null ? "" : item.Element("Url").Value;
                        model.UrlUsername = item.Element("UrlUsername") == null ? "" : item.Element("UrlUsername").Value;
                        model.UrlPassword = item.Element("UrlPassword") == null ? "" : item.Element("UrlPassword").Value;
                        collection.Add(model);
                    }
                    dg.Sources = collection;

                }
            }
            catch (Exception)
            {
            }
            return dg;
        }

        public static bool InsertDataSource(string id, string name, string columns, string description, string type, string data, string url, string urlUsername, string urlPassword)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.InsertDataSource(id, name, columns, description, type, data, url, urlUsername, urlPassword);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public static bool UpdateDataSource(string id, string name, string columns, string description, string type, string data, string url, string urlUsername, string urlPassword)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.UpdateDataSource(id, name, columns, description, type, data, url, urlUsername, urlPassword);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public static bool DeleteDataSource(string id)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.DeleteDataSource(id);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }

        public static string GetDataRecords(string id)
        {
            string result = String.Empty;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                result = service.GetDataRecords(id);
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static ObservableCollection<LogoModel> GetLogos()
        {
            ObservableCollection<LogoModel> result = new ObservableCollection<LogoModel>();
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.GetLogos();
                if (!String.IsNullOrEmpty(response))
                {

                }
            }
            catch (Exception)
            {
            }
            return result;
        }

        public static bool UploadLogo(LogoModel item, out string errorResponse)
        {
            bool result = false;
            errorResponse = String.Empty;
            try
            {
                string path = item.FileLocation;//System.IO.Path.Combine(System.Environment.CurrentDirectory, "Projects", item.FileLocation);
                if (!File.Exists(path))
                {
                    errorResponse = String.Format(@"Error: Missing projct directory {0}", path);
                    return false;
                }

                SyncResponse response = new SyncResponse();
                //var fileInfo = new System.IO.FileInfo(item.FileLocation);

                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);

                var uploadRequestInfo = new RemoteFileInfo();

                using (var stream = new System.IO.FileStream(item.FileLocation, FileMode.Open, FileAccess.Read))
                {
                    stream.Position = 0;
                    System.IO.MemoryStream streamx = new System.IO.MemoryStream();
                    stream.CopyTo(streamx);

                    uploadRequestInfo.Id = item.Id;
                    uploadRequestInfo.Client = item.Description;
                    uploadRequestInfo.FileName = Path.GetFileName(item.FileLocation);
                    uploadRequestInfo.Length = item.FileSize;//fileInfo.Length;
                    uploadRequestInfo.Data = streamx;
                    response = (service as ICeitconServer).UpdateLogo(uploadRequestInfo);
                }
                result = response.Success;

                if (!result)
                    return false;

            }
            catch (Exception e)
            {
                errorResponse = String.Format(@"Error: {0}", e.Message);
                return false;
            }
            return result;
        }

        public static bool DeleteLogo(string id)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.DeleteLogo(id);
                result = (response == "Success");
            }
            catch (Exception)
            {
                return false;
            }
            return result;
        }
        #endregion

        #endregion
    }
}
