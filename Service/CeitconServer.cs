using log4net;
using System;
using System.IO;
using System.Text;
using System.Reflection;
using Ceitcon_Service.Data;
using System.Net;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
namespace Ceitcon_Service
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public class CeitconServer : ICeitconServer
    {
        static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        static Dictionary<string, FileStream> _files = new Dictionary<string, FileStream>();

        public string TestConnection()
        {
            log.Info("TestConnection");
            return DateTime.Now.ToString();
        }

        public string CheckUploadSceduler(string client, string files)
        {
            var sb = new StringBuilder();
            try
            {
                log.Info(String.Format("Arif {0} ",  files));
                log.Info(String.Format("CheckUploadSceduler {0} ; {1}", client, files));
                int sValue =  GetLicense();
                if (sValue == 3)
                {
                    //string uploadFolder = ConfigurationManager.AppSettings["WorkingDirectory"];
                    string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
                    log.Info(String.Format("uploadFolder {0} ", uploadFolder));
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);
                    string directory = Path.Combine(uploadFolder, client);
                    log.Info(String.Format("directory {0} ", directory));
                    string[] contentFiles = files.Split('$');
                    foreach (var file in contentFiles)
                    {
                        log.Info(String.Format("file {0} ", file.ToString()));
                        int v = file.IndexOf(';');
                        log.Info(String.Format("v {0} ", v.ToString()));
                        string fileName = file.Substring(0, v);
                        log.Info(String.Format("fileName {0} ", fileName.ToString()));
                        long fileSize = Convert.ToInt64(file.Substring(v + 1));
                        log.Info(String.Format("fileSize {0} ", fileSize.ToString()));
                        string path = Path.Combine(directory, fileName);
                        //if (!File.Exists(path) || fileSize != new System.IO.FileInfo(path).Length)
                        if (!FindFile(directory, fileName, fileSize))
                        {
                            if (sb.Length > 0)
                                sb.Append('$');
                            sb.Append(file);
                        }
                    }
                }
                else
                {
                    log.Info(String.Format("LicenseValidator result: {0}", sValue));
                    return GetLicenseMessage(sValue);
                }
            }
            catch (Exception ex)
            {
                var st = new System.Diagnostics.StackTrace(ex, true);
                // Get the top stack frame
                var frame = st.GetFrame(0);
                // Get the line number from the stack frame
                int line = frame.GetFileLineNumber();
                log.Error($"CheckUploadSceduler line: {line.ToString()}");
                log.Error("CheckUploadSceduler: ", ex);
                return "Error";
            }

            log.Info(String.Format("CheckUploadSceduler result: {0}", sb.ToString()));
            return sb.ToString();
        }

        private bool FindFile(string directory, string fileName, long fileSize)
        {
            bool result = false;
            if (Directory.Exists(directory))
            {
                string[] directoryFiles = Directory.GetFiles(directory, String.Format("*{0}", Path.GetExtension(fileName)));
                foreach (string directoryFile in directoryFiles)
                {
                    if (fileSize == new System.IO.FileInfo(directoryFile).Length)
                    {
                        //replicate to new name
                        try
                        {
                            File.Copy(directoryFile, Path.Combine(directory, fileName), true);
                            //File.Move(directoryFile, Path.Combine(directory, fileName));
                            result = true;
                            break;
                        }
                        catch (Exception) { }
                    }
                }
            }
            return result;
        }

        public long GetPreviosOffSet(string sFileName, string client)
        {
            long offset = 0;
            string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload", client);
            if (!Directory.Exists(uploadFolder))
                Directory.CreateDirectory(uploadFolder);
            string filepath = Path.Combine(uploadFolder, sFileName);
            if (System.IO.File.Exists(filepath))
            {
                System.IO.FileInfo _fs = new FileInfo(filepath);
                offset = _fs.Length;
            }
            return offset;
        }

        public SyncResponse UploadScedulerChunk(RemoteFileInfo request)
        {
            log.Info($"Upload Sceduler Chunk {request.Client} ; {request.FileName}");
            int bUploaded = 0;
            try
            {
                int sValue =  GetLicense();
                if (sValue != 3)
                {
                    log.Info($"License Validator {sValue}");
                    return new SyncResponse() { Message = GetLicenseMessage(sValue), Status = -1, Success = false };
                }

                FileStream targetStream = null;
                string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
                if (!Directory.Exists(uploadFolder))
                    Directory.CreateDirectory(uploadFolder);

                if (String.IsNullOrWhiteSpace(request.Client))
                    return new SyncResponse() { Message = "Empty client", Status = -1, Success = false };

                string directory = Path.Combine(uploadFolder, request.Client);
                string path = Path.Combine(directory, request.FileName);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (_files.ContainsKey(request.FileName))
                {
                    targetStream = _files[request.FileName];
                    if (File.Exists(path))
                    {
                        FileInfo _fs = new FileInfo(path);
                        if (_fs.Length == request.Length)
                        {
                            bUploaded = 2;
                        }
                        _fs = null;
                    }
                }
                else
                {
                    if (File.Exists(path))
                    {
                        FileInfo _fs = new FileInfo(path);
                        if (_fs.Length == request.Length)
                        {
                            bUploaded = 2;
                        }
                        else
                        {
                            targetStream = new FileStream(path, FileMode.Append, FileAccess.Write);
                        }
                        _fs = null;
                    }
                    else
                    {
                        targetStream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                        _files.Add(request.FileName, targetStream);
                    }
                }

                if (bUploaded != 2)
                {
                    targetStream.Write(request.DataArray, 0, request.BufferSize);
                    long _lenght = targetStream.Length;
                    targetStream.Flush();
                    if (request.Length == _lenght)
                    {
                        bUploaded = 1;
                        targetStream.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Upload Sceduler Chunk: ", ex);
            }
            return new SyncResponse() { Message = "Uploaded", Status = bUploaded, Success = true };
        }

        public bool UploadScedulerCDP(RemoteFileInfo request)
        {
            log.Info($"UploadScedulerCDP:ID:{request.Id} - {request.Client} - {request.Version}");

            if (!SQLiteHelper.Instance.UpdateScheduler(request.Id, request.Client, request.FileName, request.Version, request.Start, request.End, request.Location, request.FileList, request.Content))
                return false;


            bool b = SQLiteHelper.Instance.UpdateSchedulerFileList(request.FileName, request.FileList);

            //if (b)
            //{
            //    //"Delete Old Content"

            //    string[] strFies = request.FileList.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
            //    List<string> NewFiles = new List<string>();
            //    NewFiles.AddRange(strFies);


            //    //
            //    string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
            //    log.Info($"Arif:Checking: Upload Folder{uploadFolder}");
            //    if (Directory.Exists(uploadFolder))
            //    {
            //        string directory = Path.Combine(uploadFolder, request.Client);
            //        string[] serverFiles = System.IO.Directory.GetFiles(directory);
            //        foreach (var serverOldFiles in serverFiles)
            //        {
            //            string fileExits = NewFiles.FirstOrDefault(p => p == serverOldFiles);
            //            if (fileExits == "" || fileExits == null)
            //            {

            //                try
            //                {
            //                    log.Info($"Server File is not found in new content upload:{serverOldFiles}");
            //                    System.IO.File.Delete(serverOldFiles);
            //                    log.Info($"Server Old :{serverOldFiles} File Deleted");
            //                }
            //                catch (Exception ex)
            //                {
            //                    log.Info($"Server Old :{serverOldFiles} File Deletion Error:" + ex.Message);
            //                }

            //            }
            //            else
            //            {
            //                log.Info($"New File: {fileExits} and Server File are same:{serverOldFiles}");
            //            }

            //        }
            //        //

            //    }

            //}
            return b;
        }

        public bool ClearDictory()
        {
            bool bResult = false;
            try
            {
                if (_files.Count > 0)
                {
                    _files.Clear();
                    bResult = true;
                    log.Info("ClearDictory Cleared");
                }
                else
                    log.Info("Nothing found in ClearDictory");
            }
            catch (Exception e)
            {
                bResult = false;
                log.Error("ClearDictory", e);
            }
            return bResult;
        }

        public SyncResponse UploadSceduler(RemoteFileInfo request)
        {
            try
            {
                log.Info(String.Format("UploadSceduler {0} ; {1}", request.Client, request.FileName));
                int sValue =  GetLicense();
                if (sValue == 3)
                {
                    FileStream targetStream = null;
                    request.Data.Position = 0;
                    Stream sourceStream = request.Data;

                    string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    if (!String.IsNullOrWhiteSpace(request.Client))
                    {
                        string directory = Path.Combine(uploadFolder, request.Client);
                        string path = Path.Combine(directory, request.FileName);
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);

                        using (targetStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            byte[] buffer = new byte[6500];
                            int read;
                            while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                targetStream.Write(buffer, 0, read);
                            }
                            sourceStream.Close();
                        }

                        if (Path.GetExtension(request.FileName) == ".cdp")
                        {
                            if (SQLiteHelper.Instance.UpdateScheduler(request.Id, request.Client, request.FileName, request.Version, request.Start, request.End, request.Location, request.FileList, request.Content))
                                SQLiteHelper.Instance.UpdateSchedulerFileList(request.FileName, request.FileList);
                        }
                    }
                }
                else
                {
                    log.Info(String.Format("LicenseValidator {0}", sValue)); ;
                    return new SyncResponse() { Message = GetLicenseMessage(sValue), Success = false };
                }
            }
            catch (Exception ex)
            {
                log.Error("UploadSceduler: ", ex);
            }
            return new SyncResponse() { Message = "Uploaded", Success = true };
        }

        public string DeleteSceduler(string id)
        {
            try
            {
                log.Info($"DeleteSceduler {id}");

                var items = GetOldSchedulerFiles(id);
                bool result = SQLiteHelper.Instance.DeleteScheduler(id);
                if (!result)
                    return "Error";

                if (items == null || items.Length == 0)
                    return "Success";

                foreach (string item in items)
                {
                    string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload", item);
                    log.Info($"Deleting File {filePath}");
                    if (!File.Exists(filePath))
                        continue;

                    FileInfo fi = new FileInfo(filePath);
                    if (!IsFileLocked(fi))
                    {
                        File.Delete(filePath);
                    }
                }

                log.Info("Delete Sceduler result: {result}");
            }
            catch (Exception ex)
            {
                log.Error("DeleteScheduler: ", ex);
                return "Error";
            }
            return "Success";
        }

        private string[] GetOldSchedulerFiles(string id)
        {
            string[] result = null;
            try
            {
                log.Info($"GetListOfOldFiles {id}");
                result = SQLiteHelper.Instance.GetOldSchedulerFiles(id);
                log.Info($"DeleteSceduler result: {result.Length}");
            }
            catch (Exception ex)
            {
                log.Error("GetListOfOldFiles: ", ex);
                return result;
            }
            return result;
        }

        //private void DeleteOldFiles(string id)
        //{
        //    try
        //    {
        //        log.Info($"DeleteOldFiles {id}");

        //        var items = GetOldSchedulerFiles(id);
        //        if (items == null || items.Length == 0)
        //            return;

        //        foreach (string item in items)
        //        {
        //            string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, item);
        //            FileInfo fi = new FileInfo(filePath);
        //            if (!IsFileLocked(fi))
        //            {
        //                File.Delete(filePath);
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Error("DeleteOldFiles: ", ex);
        //    }
        //}

        public string GetSceduler(string client)
        {
            string result = String.Empty;
            try
            {
                log.Info(String.Format("GetSceduler {0}", client));
                int sValue = GetLicense();
                if (sValue == 3)
                {
                    result = SQLiteHelper.Instance.GetSceduler(client);
                }
                else
                {
                    log.Info(String.Format("LicenseValidator: {0}", sValue)); ;
                    result = String.Format("License: {0}", GetLicenseMessage(sValue));
                }
                log.Info(String.Format("GetSceduler result: {0}", result));
            }
            catch (Exception ex)
            {
                log.Error("GetSceduler: ", ex);
            }
            return result;
        }

        public string GetAllScedulers()
        {
            string result = String.Empty;
            try
            {
                log.Info(String.Format("GetAllScedulers"));
                int sValue = GetLicense();
                if (sValue == 3)
                {
                    result = SQLiteHelper.Instance.GetAllScedulers();
                }
                else
                {
                    log.Info(String.Format("LicenseValidator: {0}", sValue)); ;
                    result = String.Format("License: {0}", GetLicenseMessage(sValue));
                }
                //log.Info(String.Format("GetAllScedulers result: {0}", result));
            }
            catch (Exception ex)
            {
                log.Error("GetAllScedulers: ", ex);
            }
            return result;
        }

        static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is: still being written to or being processed by another thread or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

        public RemoteFileInfo DownloadFile(DownloadRequest request)
        {
            RemoteFileInfo result = new RemoteFileInfo();
            try
            {
                log.Info(String.Format("DownloadFile {0} Offset {1}", request.FileName, request.Offset));
                int sValue =  GetLicense();
                if (sValue == 3)
                {
                    //string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
                    //string filePath = System.IO.Path.Combine(uploadFolder, request.FileName);
                    string filePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, request.FileName);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath);

                    // check if exists
                    if (!fileInfo.Exists)
                        throw new System.IO.FileNotFoundException("File not found", request.FileName);

                    if (request.Offset > fileInfo.Length)
                        return null;

                    //Check is the file available
                    FileInfo fi = new FileInfo(filePath);
                    int j = 0;
                    while (IsFileLocked(fi) && j <= 15)
                    {
                        j++;
                        Thread.Sleep(TimeSpan.FromSeconds(2));
                    }

                    // open stream
                    System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

                    stream.Position = 0;
                    System.IO.MemoryStream streamx = new System.IO.MemoryStream();
                    stream.CopyTo(streamx);

                    if (request.Offset > 0)
                        stream.Seek(request.Offset, SeekOrigin.Begin);

                    // return result 
                    result.FileName = request.FileName;
                    result.Length = fileInfo.Length;
                    result.Data = streamx;
                }
                else
                {
                    log.Info(String.Format("LicenseValidator {0}", sValue));
                }
            }
            catch (Exception ex)
            {
                log.Error("DownloadFile: ", ex);
            }
            return result;
        }

        public string LoginUser(string username, string password)
        {
            log.Info($"Login: {username}");
            string result = String.Empty;
            int sValue =  GetLicense();
            if (sValue == 3)
            {
                result = SQLiteHelper.Instance.Login(username, password);
            }
            else
            {
                log.Info($"LicenseValidator: {sValue}");
                result = GetLicenseMessage(sValue);
            }
            log.Info($"Login result: {result}");
            return result;
        }

        public string UpdateUser(string id, string groupId, string name, string password, string email, string phone, string permissions)
        {
            log.Info($"UpdateUser {id} ; {groupId} ; {name} ; {password} ; {email} ; {phone} ; {permissions}");
            bool result = SQLiteHelper.Instance.UpdateUser(id, groupId, name, password, email, phone, permissions);
            log.Info($"UpdateUser result: {result}");
            return result ? "Success" : "Error";
        }

        public string DeleteUser(string id)
        {
            log.Info(String.Format("DeleteUser {0}", id));
            bool result = SQLiteHelper.Instance.DeleteUser(id);
            log.Info(String.Format("DeleteUser {0}", result));
            return result ? "Success" : "Error";
        }

        public string GetGroups()
        {
            log.Info("GetGroups");
            string result = SQLiteHelper.Instance.GetGroups();
            log.Info($"GetGroups: {result}");
            return result;
        }

        public string UpdateNetwork(string type, string id, string parentId, string name, string description, bool activ)
        {
            log.Info(String.Format("UpdateNetwork {0} ; {1} ; {2} ; {3} ; {4} ; {5}", type, id, parentId, name, description, activ));
            string result = SQLiteHelper.Instance.UpdateNetwork(type, id, parentId, name, description, activ);
            log.Info(String.Format("UpdateNetwork result: {0}", result));
            return result;
        }

        public string DeleteNetwork(string type, string id)
        {
            log.Info(String.Format("DeleteNetwork {0} ; {1}", type, id));
            bool result = SQLiteHelper.Instance.DeleteNetwork(type, id);
            log.Info(String.Format("DeleteNetwork  result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string GetNetwork()
        {
            log.Info(String.Format("GetNetwork"));
            string result = SQLiteHelper.Instance.GetNetwork();
            log.Info(String.Format("GetNetwork result: {0}", result));
            return result;
        }

        public string CheckPlayerName(string name)
        {
            log.Info(String.Format("CheckPlayerName {0}", name));
            bool result = SQLiteHelper.Instance.CheckPlayerName(name);
            log.Info(String.Format("CheckPlayerName result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string CheckPlayerExist(string name, string hostName, string ipAddress, int licence)
        {
            log.Info(String.Format("CheckPlayerExist {0} ; {1} ; {2} ; {3}", name, hostName, ipAddress, licence));
            bool result = SQLiteHelper.Instance.CheckPlayerExist(name, hostName, ipAddress, licence);
            log.Info(String.Format("CheckPlayerExist result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string CheckFreeLicence(int licence)
        {
            log.Info(String.Format("CheckFreeLicence {0}", licence));
            bool result = false;
            try
            {
                int res = GetLicense(licence);
                if (res == 3)
                {
                    result = true;
                }
            }
            catch (Exception) { }
            log.Info(String.Format("CheckFreeLicence result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string ReadCDPFile(string fileName)
        {
            string sContent = "";
            try
            {
                int sValue = GetLicense();
                if (sValue == 3)
                {
                    string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Upload");
                    string filePath = System.IO.Path.Combine(uploadFolder, fileName);


                    // check if exists
                    if (System.IO.File.Exists(filePath))
                    {
                        sContent = System.IO.File.ReadAllText(filePath);
                    }
                    else
                        sContent = "File Not Found";


                }
                else
                {
                    log.Info(String.Format("LicenseValidator {0}", sValue));
                }
            }
            catch (Exception ex)
            {
                log.Error("ReadCDPFile: ", ex);
            }
            return sContent;
        }

        public string GetPlayerName(string hostName, string ipAddress)
        {
            log.Info(String.Format("GetPlayerName {0} ; {1}", hostName, ipAddress));
            string result = SQLiteHelper.Instance.GetPlayerName(hostName, ipAddress);
            log.Info(String.Format("GetPlayerName result: {0}", result));
            return result;
        }

        public string GetPlayerInfo(string playerName,string hostName, string ipAddress)
        {
            log.Info(String.Format("GetPlayerInfo {0} ; {1}; {2}", hostName, ipAddress, playerName));
            string result = SQLiteHelper.Instance.GetPlayerInfo(playerName, hostName, ipAddress);
            log.Info(String.Format("GetPlayerInfo result: {0}", result));
            return result;
        }

        public string RegistratePlayer(string name, string hostName, string ipAddress, int screens, int licence, int status)
        {
            log.Info($"RegistratePlayer {name}, {hostName}, {ipAddress}, {licence}, {status}");
            bool result = SQLiteHelper.Instance.RegistratePlayer(name, hostName, ipAddress, screens, licence, status);
            log.Info(String.Format("RegistratePlayer result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string UpdatePlayerStatus(string name, int screens, int status)
        {
            log.Info($"UpdatePlayerStatus {name}, {screens}, {status}");
            int result = SQLiteHelper.Instance.GetPlayerLicence(name);
            log.Info($"PlayerStatus from DB  {status}");
            if (result > 0 && (result != 4))
                SQLiteHelper.Instance.UpdatePlayerStatus(name, screens, status);
            log.Info(String.Format("UpdatePlayerStatus result: {0}", result));
            return result.ToString();
        }

        public string GetPlayerRefresh(string name)
        {
            log.Info($"GetPlayerRegresh {name}");
            string result = SQLiteHelper.Instance.GetPlayerRegresh(name);
            log.Info($"GetPlayerRegresh result: {result}");
            return result;
        }

        public string UpdatePlayerRefresh(string name, int refreshTime)
        {
            log.Info($"UpdatePlayerRefresh {name} ; {refreshTime}");
            bool result = SQLiteHelper.Instance.UpdatePlayerRefresh(name, refreshTime);
            log.Info($"UpdatePlayerRefresh result: {result}");
            return result ? "Success" : "Error";
        }

        private string GetWeather(string location)
        {
            string response = String.Empty;
            try
            {
                string url = string.Format(System.Configuration.ConfigurationManager.AppSettings["WeatherUrl"], location);
                WebClient webclient = new WebClient();
                response = webclient.DownloadString(new Uri(url));
            }
            catch (Exception ex)
            {
                log.Debug("GetWeather", ex);
            }

            return response;
        }

        public string GetWeathers(string location)
        {
            log.Info(String.Format("GetWeathers {0}", location));
            string result = GetWeather(location);
            log.Info(String.Format("GetWeathers result: {0}", result));
            return result;
        }

        public string RegistredPlayerCount()
        {
            log.Info("RegistredPlayerCount");
            string result = SQLiteHelper.Instance.RegistredPlayerCount();
            log.Info(String.Format("RegistredPlayerCount result: {0}", result));
            return result;
        }

        public string GetPlayerLicence(string name)
        {
            log.Info(String.Format("GetPlayerStatus {0}", name));
            int result = SQLiteHelper.Instance.GetPlayerLicence(name);
            log.Info(String.Format("GetPlayerStatus result: {0}", result));
            return result.ToString();
        }

        public string StopPlayer(string name)
        {
            log.Info(String.Format("StopPlayer {0}", name));
            bool result = SQLiteHelper.Instance.StopPlayer(name);
            log.Info(String.Format("StopPlayer {0}", result));
            return result ? "Success" : "Error";
        }

        public string DisconnectPlayer(string name)
        {
            log.Info(String.Format("DisconnectPlayer {0}", name));
            bool result = SQLiteHelper.Instance.DisconectPlayer(name);
            log.Info(String.Format("DisconnectPlayer {0}", result));
            return result ? "Success" : "Error";
        }

        public string GetDataSources()
        {
            log.Info("GetDataSources");
            string result = SQLiteHelper.Instance.GetDataSources();
            log.Info(String.Format("GetDataSources result: {0}", result));
            return result.ToString();
        }

        public string InsertDataSource(string id, string name, string columns, string description, string type, string data, string url, string urlUsername, string urlPassword)
        {
            log.Info(String.Format("InsertDataSource {0}, {1}, {2}, {3}, {4}", id, name, columns, description, type));
            bool result = SQLiteHelper.Instance.InsertDataSource(id, name, columns, description, type, data, url, urlUsername, urlPassword);
            log.Info(String.Format("InsertDataSource result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string UpdateDataSource(string id, string name, string columns, string description, string type, string data, string url, string urlUsername, string urlPassword)
        {
            log.Info(String.Format("InsertDataSource {0}, {1}, {2}, {3}, {4}", id, name, columns, description, type));
            bool result = SQLiteHelper.Instance.UpdateDataSource(id, name, columns, description, type, data, url, urlUsername, urlPassword);
            log.Info(String.Format("InsertDataSource result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string DeleteDataSource(string id)
        {
            log.Info(String.Format("DeleteDataSource {0}", id));
            bool result = SQLiteHelper.Instance.DeleteDataSource(id);
            log.Info(String.Format("DeleteDataSource result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string GetDataRecords(string id)
        {
            log.Info(String.Format("GetDataRecords {0}", id));
            string result = SQLiteHelper.Instance.GetDataRecord(id);
            log.Info(String.Format("GetDataRecords result: {0}", result));
            return result.ToString();
        }

        public SyncResponse UpdateLogo(RemoteFileInfo request)
        {
            try
            {
                log.Info(String.Format("Update Logo {0} ; {1} ; {2} ; {3}", request.Id, request.Client, request.FileName, request.Length));

                int sValue =  GetLicense();
                if (sValue == 3)
                {
                    FileStream targetStream = null;
                    request.Data.Position = 0;
                    Stream sourceStream = request.Data;

                    string uploadFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logos");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);

                    if (!String.IsNullOrWhiteSpace(request.Id) && !String.IsNullOrWhiteSpace(request.Client))
                    {
                        string path = Path.Combine(uploadFolder, request.FileName);
                        using (targetStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            byte[] buffer = new byte[6500];
                            int read;
                            while ((read = sourceStream.Read(buffer, 0, buffer.Length)) > 0)
                            {
                                targetStream.Write(buffer, 0, read);
                            }
                            sourceStream.Close();
                        }

                        SQLiteHelper.Instance.UpdateLogo(request.Id, request.Client, path, request.Length);
                    }
                }
                else
                {
                    log.Info(String.Format("LicenseValidator {0}", sValue)); ;
                    return new SyncResponse() { Message = GetLicenseMessage(sValue), Success = false };
                }
            }
            catch (Exception ex)
            {
                log.Error("UploadSceduler: ", ex);
            }
            return new SyncResponse() { Message = "Uploaded", Success = true };
        }

        public string DeleteLogo(string id)
        {
            log.Info(String.Format("Delete Logo {0}", id));
            bool result = SQLiteHelper.Instance.DeleteLogo(id);
            log.Info(String.Format("Delete Logo result: {0}", result));
            return result ? "Success" : "Error";
        }

        public string GetLogos()
        {
            log.Info(String.Format("Get Logos"));
            string result = SQLiteHelper.Instance.GetLogos();
            log.Info(String.Format("Get Logos result: {0}", result));
            return result.ToString();
        }

        #region Private methds
        private int GetLicense(int extra = 0)
        {
            string text = SQLiteHelper.Instance.RegistredPlayerCount();
            int totalCount;
            bool res = int.TryParse(text, out totalCount);
            if (res == false)
            {
                totalCount = 0;
            }
            return LicenseValidator.LG.Validate(totalCount + extra);
            //return 3;
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

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

        #endregion
    }
}