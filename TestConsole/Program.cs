using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {

            var client = new RestClient("http://localhost:9103");
            client.Timeout = -1;
            var request = new RestRequest("/api/player/DownloadFile?sFileName=Upload\\Network\\Domain\\Country\\Region\\Location Group\\Location\\Floor\\Zone\\Test2\\83b80483-6731-4d44-8f05-e67ccc00be9c.jpeg&Offset=0&organizationID=1", Method.POST);
            // request.AddHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6ImNlaXRjb24uY29tIiwic3ViIjoiY2VpdGNvbi5jb20iLCJqdGkiOiIwZTI3ZTE5My1iYzNiLTQ4ZTYtYTExZS04ZGZmMjUyMzBiYWIiLCJuYmYiOjE3NjE3Mjc3ODcsImV4cCI6MTc2MjU5MTc4NywiaWF0IjoxNzYxNzI3Nzg3fQ.iS3W-5odlaGB0RX4LUIng_SCThdYe36TLw_TdEHa0Q0");
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            string sM = DateTime.Now.TimeOfDay.Minutes.ToString();
            if (sM.Length == 1)
                sM = "0" + sM;
            string sSe = DateTime.Now.TimeOfDay.Seconds.ToString();
            if (sSe.Length == 1)
                sSe = "0" + sSe;

            string sTime = DateTime.Now.TimeOfDay.Hours.ToString() + ":" + sM;
            string sInput = System.IO.File.ReadAllText("C:\\Users\\Ceitcon-Dev\\Downloads\\Project.cdp");
            string sResult = Crypt.Decrypt(sInput);

            Console.ReadLine();


        }

        static void DeleteOldRecords()
        {
            Console.ReadLine();
        }

    }
}
