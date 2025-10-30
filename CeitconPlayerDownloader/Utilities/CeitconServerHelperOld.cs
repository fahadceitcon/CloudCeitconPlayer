using Ceitcon_Downloader.CeitconServerService;
using log4net;
using System;
using System.IO;
using System.Reflection;
using System.ServiceModel;

namespace Ceitcon_Downloader.Utilities
{
    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public static class CeitconServerHelperOld
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
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
        }

        public static string GetSceduler(string client)
        {
            string result = String.Empty;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                result = service.GetSceduler(client);
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return result;
        }
        
        //static CeitconServerClient service = null;
        //static Int32 i32ChunkSize = 10 * 1024 * 1024;
        //static Int64 I64Offset = 0;
        //static string docName = String.Empty;
        //static bool IsFileToAppend = false;
        //static Int64 fileSize = 0;
        //static SaveFileDialog fileDialog = null;
        //static bool isFirstCall = true;
        //static Stream fileStream = null;

        public static bool Download(string fromPath, string toPath)
        {
            try
            {
                // Simple Download
                //var service = new CeitconServerClient();
                //service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                //DownloadRequest requestData = new DownloadRequest();

                //RemoteFileInfo fileInfo = new RemoteFileInfo();
                //requestData.FileName = fromPath;

                //fileInfo = (service as ICeitconServer).DownloadFile(requestData);

                //byte[] buffer = new byte[6500];
                //using (FileStream fs = new FileStream(toPath, FileMode.Create, FileAccess.Write, FileShare.None))
                //{
                //    int read;
                //    while ((read = fileInfo.Data.Read(buffer, 0, buffer.Length)) > 0)
                //    {
                //        fs.Write(buffer, 0, read);
                //    }
                //}
                //return true;
                //=============Check file zise=========
                long iExistLen = 0;
                try
                {
                    if (File.Exists(toPath))
                    {
                        FileInfo fINfo = new FileInfo(toPath);
                        iExistLen = fINfo.Length;
                    }
                }
                catch (Exception)
                {
                }

                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                DownloadRequest requestData = new DownloadRequest();

                RemoteFileInfo fileInfo = new RemoteFileInfo();
                requestData.FileName = fromPath;
                requestData.Offset = iExistLen;

                fileInfo = (service as ICeitconServer).DownloadFile(requestData);


                string directory = Path.GetDirectoryName(toPath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                //Open file stream on load existing
                FileStream fs = null;
                try
                {
                    if (iExistLen > 0)
                        fs = new System.IO.FileStream(toPath,
                          System.IO.FileMode.Append, System.IO.FileAccess.Write,
                          System.IO.FileShare.ReadWrite);
                    else
                        fs = new System.IO.FileStream(toPath,
                          System.IO.FileMode.Create, System.IO.FileAccess.Write,
                          System.IO.FileShare.ReadWrite);
                }
                catch (Exception)
                {
                }

                int iByteSize = 65000;
                byte[] buffer = new byte[iByteSize];
                long totalDownload = iExistLen;
                fileInfo.Data.Position = 0;
                int read;

                while ((read = fileInfo.Data.Read(buffer, 0, buffer.Length)) > 0)
                {
                    totalDownload += iByteSize;

                    try
                    {
                        fs.Write(buffer, 0, read);
                        decimal iper = fileInfo.Length > 0 ? (totalDownload * 100 / fileInfo.Length) : 0;
                        if (iper > 100)
                            iper = 100; 
                        Console.WriteLine(String.Format("Downloaded percentage: {0}% , Downloaded size: {1} byts", iper, totalDownload));
                        log.Info(String.Format("Downloaded percentage: {0}% , Downloaded size: {1} byts", iper, totalDownload));
                    }
                    catch (Exception)
                    {
                        fs.Close();
                        fs.Dispose();
                        return false;
                    }
                    //if(i++ > 8)
                    //    return false;
                }
                fs.Close();
                fs.Dispose();
            }
            catch (Exception ex)
            {
                //System.Web.HttpContext.Current.Response.Write("Error : " + ex.Message);
                log.Info("Error : " + ex.Message);
                return false;
            }
            return true;
        }

        public static bool CheckPlayerName(string name)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.CheckPlayerName(name);
                result = (response == "Success");
            }
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
            return result;
        }

        public static bool CheckPlayerExist(string name, string hostName, string ipAddress, int licence)
        {
            bool result = false;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.CheckPlayerExist(name, hostName, ipAddress, licence);
                result = (response == "Success");
            }
            catch (Exception e)
            {
                log.Error(e);
                return false;
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
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
            return result;
        }

        public static string GetPlayerName(string hostName, string ipAddress)
        {
            string result = String.Empty;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                result = service.GetPlayerName(hostName, ipAddress);
            }
            catch (Exception e)
            {
                log.Error(e);
                return "Error";
            }
            return result;
        }

        public static string GetPlayerNameInfo(string playername,string hostName, string ipAddress)
        {
            string result = String.Empty;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                result = service.GetPlayerInfo(playername,hostName, ipAddress);
            }
            catch (Exception e)
            {
                log.Error(e);
                return "Error";
            }
            return result;
        }

        public static int GetPlayerRefresh(string name)
        {
            int result = 0;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.GetPlayerRefresh(name);
                result = Convert.ToInt32(response);
            }
            catch (Exception e)
            {
                log.Error(e);
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
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
            return result;
        }

        public static int UpdatePlayerStatus(string name, int screens, int status)
        {
            int result = -1;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.UpdatePlayerStatus(name, screens, status);
                result = Convert.ToInt32(response);
            }
            catch (Exception e)
            {
                log.Error(e);
                return -1;
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
            catch (Exception e)
            {
                log.Error(e);
            }
            return result;
        }

        public static int RegistredPlayerCount()
        {
            int result = 0;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                string response = service.RegistredPlayerCount();
                result = Convert.ToInt32(response);
            }
            catch (Exception e)
            {
                log.Error(e);
                return 0;
            }
            return result;
        }

        public static string StopPlayer(string name)
        {
            string result = String.Empty;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                result = service.StopPlayer(name);
            }
            catch (Exception e)
            {
                log.Error(e);
                return String.Empty;
            }
            return result;
        }

        public static string GetPlayerLicence(string name)
        {
            string result = String.Empty;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                result = service.GetPlayerLicence(name);
            }
            catch (Exception e)
            {
                log.Error(e);
                return String.Empty;
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

        public static string GetLogos()
        {
            string result = String.Empty;
            try
            {
                var service = new CeitconServerClient();
                service.Endpoint.Address = new EndpointAddress(new Uri(Address), service.Endpoint.Address.Identity, service.Endpoint.Address.Headers);
                result = service.GetLogos();
            }
            catch (Exception)
            {
            }
            return result;
        }
    }
}