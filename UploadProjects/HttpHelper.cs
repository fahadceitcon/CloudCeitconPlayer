using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using UploadProjects.Model;

namespace UploadProjects
{
    internal class HttpHelper
    {

        #region[Data Members]
        public static string ServerIP = "http://64.251.7.228:5056/";
        #endregion

        public static List<Model.Schedule> GetAllSchedules()
        {
            List<Model.Schedule> sResult = new List<Model.Schedule>();
            try
            {

                #region [Handling SSL Exception]
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                  delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                  {
                      // Replace this line with code to validate server certificate.
                      return true;
                  };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                #endregion

                var client = new RestClient(ServerIP + "API/Signage/GetAllScedulers");
                //client.Timeout = -1;
                var request = new RestRequest();
                request.Method = Method.Get;
                request.Timeout = null;
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("Authorization", "Basic c21hcnR1c2VyOlNtQHJ0I0AhWHB5VG4lJA==");
                RestResponse response = client.Execute(request);
                sResult = JsonConvert.DeserializeObject<List<Model.Schedule>>(response.Content.ToString());

            }
            catch (Exception Ex)
            {

            }
            return sResult;
        }

        public static Result UploadScheduler(Schedule schedule)
        {
            Result result = new Result();
            try
            {
                #region [Handling SSL Exception]
                System.Net.ServicePointManager.ServerCertificateValidationCallback =
                  delegate (Object obj, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                  {
                      // Replace this line with code to validate server certificate.
                      return true;
                  };
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                #endregion

                var client = new RestClient(ServerIP + "API/Signage/UploadScheduler");
                var request = new RestRequest();
                request.Method = Method.Post;
                request.Timeout = null;

                request.AlwaysMultipartFormData = true;
                request.AddParameter("UniqueId", schedule.UniqueId);
                request.AddParameter("ProjectName", schedule.ProjectName);
                request.AddParameter("Version", schedule.Version);
                request.AddParameter("StartTime", schedule.StartTime);
                request.AddParameter("EndTime", schedule.EndTime);
                request.AddParameter("LocationGroup", schedule.LocationGroup);
                request.AddParameter("CDPFileContent", schedule.CDPFileContent);
                request.AddParameter("Location", schedule.Location);
                foreach (var item in schedule.Files)
                {
                    request.AddFile("Files", item);
                }
                RestResponse response = client.Execute(request);
                result = JsonConvert.DeserializeObject<Model.Result>(response.Content.ToString());
            }
            catch (Exception Ex)
            {

            }
            return result;
        }

    }
}