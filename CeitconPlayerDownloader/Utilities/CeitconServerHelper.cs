using Ceitcon_Downloader.CeitconServerService;
using log4net;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Configuration;
using System.IO;
using System.Reflection;

namespace Ceitcon_Downloader.Utilities
{

    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    public static class CeitconServerHelper
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static string Address = String.Empty; //@"http://localhost:8733/Design_Time_Addresses/CeitconServer/Service1/";
        public static decimal OrganizationID;
        public static CeitconToken PlayerToken;

        public static CeitconToken LogIn(string sOrganization, string sPassword)
        {
            try
            {
                PlayerToken = new CeitconToken();
                string sModuleName = ConfigurationManager.AppSettings["ModuleName"].ToString();
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/API/Auth/Login", Method.Post);
                var body = @"{" + "\n" +
@"    ""OrganizationName"": """ + sOrganization + @"""," + "\n" +
@"    ""Password"": """ + sPassword + @"""," + "\n" +
@"    ""Module"": """ + sModuleName + @"""" + "\n" + @"}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = restClient.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response != null)
                    {
                        PlayerToken = JsonConvert.DeserializeObject<CeitconToken>(response.Content);
                        if (PlayerToken != null && PlayerToken.expiry > 0)
                        {
                            DateTime utcTime = new DateTime(PlayerToken.expiry, DateTimeKind.Utc);
                            DateTime localTime = utcTime.ToLocalTime();
                            PlayerToken.expirydate = localTime;
                        }
                        if (PlayerToken != null && PlayerToken.ouid > 0)
                            OrganizationID = PlayerToken.ouid;
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    PlayerToken.token = string.Empty;
                    PlayerToken.expiry = 0;
                    PlayerToken.expirydate = DateTime.MinValue;
                    PlayerToken.ouid = -1;
                }
                return PlayerToken;
            }
            catch (Exception e)
            {
                log.Error(e);
                PlayerToken.token = string.Empty;
                PlayerToken.expiry = 0;
                PlayerToken.expirydate = DateTime.MinValue;
                PlayerToken.ouid = 0;
            }
            return PlayerToken;
        }
        public static CeitconToken RefreshToken()
        {
            try
            {

                string sModuleName = ConfigurationManager.AppSettings["ModuleName"].ToString();
                string sResult = string.Empty;
                string sOrganization = SQLiteHelper.Instance.GetApplication("OrganizationName");
                string sPassword = SQLiteHelper.Instance.GetApplication("Password");
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/API/Auth/Login", Method.Post);
                var body = @"{" + "\n" +
@"    ""OrganizationName"": """ + sOrganization + @"""," + "\n" +
@"    ""Password"": """ + sPassword + @"""," + "\n" +
@"    ""Module"": """ + sModuleName + @"""" + "\n" + @"}";
                request.AddStringBody(body, DataFormat.Json);
                RestResponse response = restClient.Execute(request);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    if (response != null)
                    {
                        PlayerToken = JsonConvert.DeserializeObject<CeitconToken>(response.Content);
                        if (PlayerToken != null && PlayerToken.expiry > 0)
                        {
                            DateTime utcTime = new DateTime(PlayerToken.expiry, DateTimeKind.Utc);
                            DateTime localTime = utcTime.ToLocalTime();
                            PlayerToken.expirydate = localTime;
                        }
                        if (PlayerToken != null && PlayerToken.ouid > 0)
                            OrganizationID = PlayerToken.ouid;
                        SQLiteHelper.Instance.UpdateApplication("Token", PlayerToken.token);
                        SQLiteHelper.Instance.UpdateApplication("TokenExpiry", PlayerToken.expiry.ToString());
                        SQLiteHelper.Instance.UpdateApplication("TokenExpiryDate", PlayerToken.expirydate.ToString());
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    PlayerToken.token = string.Empty;
                    PlayerToken.expiry = 0;
                    PlayerToken.expirydate = DateTime.MinValue;
                    PlayerToken.ouid = -1;
                }
                return PlayerToken;
            }
            catch (Exception e)
            {
                log.Error(e);
                PlayerToken.token = string.Empty;
                PlayerToken.expiry = 0;
                PlayerToken.expirydate = DateTime.MinValue;
                PlayerToken.ouid = 0;
            }
            return PlayerToken;
        }
        public static CeitconToken LoadToken()
        {
            try
            {
                if (PlayerToken == null)
                    PlayerToken = new CeitconToken();
                string token = SQLiteHelper.Instance.GetApplication("Token");
                if (token == null || token.Trim() == string.Empty)
                {
                    RefreshToken();
                }
                else
                {
                    PlayerToken.token = token;
                    PlayerToken.expiry = Convert.ToInt64(SQLiteHelper.Instance.GetApplication("TokenExpiry"));
                    PlayerToken.expirydate = Convert.ToDateTime(SQLiteHelper.Instance.GetApplication("TokenExpiryDate"));
                    PlayerToken.ouid = Convert.ToInt32(SQLiteHelper.Instance.GetApplication("OrganizationID"));
                    if (PlayerToken != null && PlayerToken.ouid > 0)
                        OrganizationID = PlayerToken.ouid;
                    if (PlayerToken.expirydate.AddMinutes(-5) <= DateTime.Now)
                        RefreshToken();
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                PlayerToken.token = string.Empty;
                PlayerToken.expiry = 0;
                PlayerToken.expirydate = DateTime.MinValue;
                PlayerToken.ouid = 0;
            }
            return PlayerToken;
        }
        public static bool TestConnection()
        {
            try
            {
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var client = new RestClient(options);
                var request = new RestRequest("/api/Player/TestConnection", Method.Get);
                //request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = client.Execute(request);
                return !String.IsNullOrWhiteSpace(response.Content);
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
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetSceduler/" + client + "/" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    result = response.Content;
                result = result.Replace("\"", "");
                result = result.Replace("\\", "");
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return result;
        }
        public static string GetScedulerByPlayerID(decimal playerid)
        {
            string result = String.Empty;
            try
            {
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetScedulerByPlayerID/" + playerid + "/" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    result = response.Content;
                result = result.Replace("\"", "");
                result = result.Replace("\\", "");
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            return result;
        }
        public static bool Download(string fromPath, string toPath)
        {
            try
            {
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

                DownloadRequest requestData = new DownloadRequest();

                RemoteFileInfo fileInfo = new RemoteFileInfo();
                requestData.FileName = fromPath;
                requestData.Offset = iExistLen;

                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/DownloadFile?sFileName=" + requestData.FileName + "&Offset=" + requestData.Offset + "&organizationID=" + OrganizationID, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                //var body = "{" + "\n" + @"    ""FileName"": """ + requestData.FileName + @"""," + @"\n ""Offset"": " + requestData.Offset + "," + @"\n ""organizationID"":" + OrganizationID + "\n" + "}";
                //request.AddStringBody(body, DataFormat.Json);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                fileInfo = JsonConvert.DeserializeObject<RemoteFileInfo>(sResult);

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
                byte[] bytes = Convert.FromBase64String(fileInfo.Content);
                fileInfo.Data = new MemoryStream(bytes);
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
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/CheckPlayerName/" + name + "/" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = (sResult.ToLower() == "true");
            }
            catch (Exception e)
            {
                log.Error(e);
                return false;
            }
            return result;
        }
        public static decimal CheckPlayerID(string name)
        {
            decimal result = 0;
            try
            {
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/CheckPlayerID/" + name + "/" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = Convert.ToDecimal(sResult);
            }
            catch (Exception e)
            {
                log.Error(e);
                return -1;
            }
            return result;
        }
        public static bool CheckPlayerExist(string name, string hostName, string ipAddress, int licence)
        {
            bool result = false;
            try
            {
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/CheckPlayerExist?name=" + name + "&hostName=" + hostName + "&ipAddress=" + ipAddress + "&licence=" + licence + "&organizationID=" + OrganizationID, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = (sResult.ToLower() == "true");
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
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/CheckFreeLicence/" + licence + "/" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = (sResult.ToLower() == "true");
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
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetPlayerName?hostName=" + hostName + "&ipAddress=" + ipAddress + "&organizationID=" + OrganizationID, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = sResult;
            }
            catch (Exception e)
            {
                log.Error(e);
                return "Error";
            }
            return result;
        }
        public static string GetPlayerNameByPlayerID(decimal playerid, string hostName, string ipAddress)
        {
            string result = String.Empty;
            try
            {
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetPlayerNameByPlayerID?playerid=" + playerid + "&hostName=" + hostName + "&ipAddress=" + ipAddress + "&organizationID=" + OrganizationID, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = sResult;
            }
            catch (Exception e)
            {
                log.Error(e);
                return "Error";
            }
            return result;
        }
        public static string GetPlayerNameInfo(string playername, string hostName, string ipAddress)
        {
            string result = String.Empty;
            try
            {
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetPlayerInfo?playerName=" + playername + "&hostName=" + hostName + "&ipAddress=" + ipAddress + "&organizationID=" + OrganizationID, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = sResult;
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
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetPlayerRefresh?name=" + name + "&organizationID=" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = Convert.ToInt32(sResult);
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
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/RegistratePlayer?name=" + name + "&hostName=" + hostName + "&ipAddress=" + ipAddress +
                    "&screens=" + screens + "&licence=" + licence + "&status=" + status + "&organizationID=" + OrganizationID, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = (sResult.ToLower() == "true");
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
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/UpdatePlayerStatus?name=" + name +
                    "&screens=" + screens + "&status=" + status + "&organizationID=" + OrganizationID, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = Convert.ToInt32(sResult);
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
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetWeathers?location=" + locations, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                // sResult = sResult.Replace("\"", "");
                result = sResult;
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
                string sResult = string.Empty;
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/RegistredPlayerCount?organizationID=" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    sResult = response.Content.ToString();
                sResult = sResult.Replace("\"", "");
                result = Convert.ToInt32(sResult);
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
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/StopPlayer?name=" + name + "&organizationID=" + OrganizationID, Method.Post);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    result = response.Content.ToString();
                result = result.Replace("\"", "");
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
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetPlayerLicence?name=" + name + "&organizationID=" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    result = response.Content.ToString();
                result = result.Replace("\"", "");
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
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetDataRecords?id=" + id + "&organizationID=" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    result = response.Content.ToString();
                result = result.Replace("\"", "");
            }
            catch (Exception e)
            {
                log.Error(e);
                return String.Empty;
            }
            return result;
        }
        public static string GetLogos()
        {
            string result = String.Empty;
            try
            {
                var options = new RestClientOptions(Address)
                {
                    Timeout = TimeSpan.FromMinutes(5)
                };
                var restClient = new RestClient(options);
                var request = new RestRequest("/api/Player/GetLogos?organizationID=" + OrganizationID, Method.Get);
                request.AddHeader("Authorization", "Bearer " + PlayerToken.token);
                RestResponse response = restClient.Execute(request);
                if (!String.IsNullOrWhiteSpace(response.Content))
                    result = response.Content.ToString();
                result = result.Replace("\"", "");
            }
            catch (Exception e)
            {
                log.Error(e);
                return String.Empty;
            }
            return result;
        }

    }

    public class CeitconToken
    {
        public string token { get; set; }
        public long expiry { get; set; }
        public DateTime expirydate { get; set; }
        public int ouid { get; set; }
    }


}