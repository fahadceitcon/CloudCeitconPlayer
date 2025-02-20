using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Ceitcon_Service
{
    [DataContract]
    public class FileData
    {
        [DataMember]
        public string Filename { get; set; }
        [DataMember]
        public int Offset { get; set; }
        [DataMember]
        public byte[] Buffer { get; set; }
    }

    [Obfuscation(Feature = "Apply to member * when method or constructor: virtualization", Exclude = false)]
    [ServiceContract(Namespace = "http://Microsoft.ServiceModel.Samples")]
    public interface ICeitconServer
    {
        [OperationContract]
        string ReadCDPFile(string fileName);

        [OperationContract]
        string TestConnection();

        [OperationContract]
        RemoteFileInfo DownloadFile(DownloadRequest request);

        [OperationContract]
        string CheckUploadSceduler(string client, string files);

        [OperationContract]
        long GetPreviosOffSet(string sFileName, string client);

        [OperationContract]
        bool UploadScedulerCDP(RemoteFileInfo request);

        [OperationContract]
        SyncResponse UploadScedulerChunk(RemoteFileInfo request);

        [OperationContract]
        bool ClearDictory();

        [OperationContract]
        SyncResponse UploadSceduler(RemoteFileInfo request);

        [OperationContract]
        string DeleteSceduler(string id);

        [OperationContract]
        string GetSceduler(string client);

        [OperationContract]
        string GetAllScedulers();

        [OperationContract]
        string LoginUser(string username, string password);

        [OperationContract]
        string UpdateUser(string id, string groupId, string name, string password, string email, string phone, string permissions);

        [OperationContract]
        string DeleteUser(string id);

        [OperationContract]
        string GetGroups();

        [OperationContract]
        string GetNetwork();

        [OperationContract]
        string UpdateNetwork(string type, string id, string parentId, string name, string description, bool active);

        [OperationContract]
        string DeleteNetwork(string type, string id);

        [OperationContract]
        string CheckPlayerName(string name);

        [OperationContract]
        string CheckPlayerExist(string name, string hostName, string ipAddress, int licence);

        [OperationContract]
        string CheckFreeLicence(int licence);

        [OperationContract]
        string GetPlayerName(string hostName, string ipAddress);
        [OperationContract]
        string GetPlayerInfo(string playerName, string hostName, string ipAddress);

        [OperationContract]
        string RegistratePlayer(string name, string hostName, string ipAddress, int screens, int licence, int status);

        [OperationContract]
        string UpdatePlayerStatus(string name, int screens, int status);

        [OperationContract]
        string GetPlayerRefresh(string name);

        [OperationContract]
        string UpdatePlayerRefresh(string name, int refreshTime);

        [OperationContract]
        string GetWeathers(string location);

        [OperationContract]
        string RegistredPlayerCount();

        [OperationContract]
        string GetPlayerLicence(string name);

        [OperationContract]
        string DisconnectPlayer(string name);

        [OperationContract]
        string StopPlayer(string name);

        [OperationContract]
        string GetDataSources();

        [OperationContract]
        string InsertDataSource(string id, string name, string columns, string description, string type, string data, string url, string urlUsername, string urlPassword);

        [OperationContract]
        string UpdateDataSource(string id, string name, string columns, string description, string type, string data, string url, string urlUsername, string urlPassword);

        [OperationContract]
        string DeleteDataSource(string id);

        [OperationContract]
        string GetDataRecords(string id);

        [OperationContract]
        SyncResponse UpdateLogo(RemoteFileInfo request);

        [OperationContract]
        string DeleteLogo(string id);

        [OperationContract]
        string GetLogos();



    }

    [DataContract]
    public class SyncResponse
    {
        //[MessageHeader(MustUnderstand = true)]
        [DataMember]
        public bool Success;

        [DataMember]
        public int Status;

        [DataMember]
        public string Message;
    }

    [DataContract]
    public class DownloadRequest
    {
        [DataMember]
        public string FileName;

        [DataMember]
        public long Offset;
    }

  

    [DataContract]
    public class RemoteFileInfo : IDisposable
    {
        [DataMember]
        public string Id;

        [DataMember]
        public string Client;

        [DataMember]
        public string Version;

        [DataMember]
        public string Start;

        [DataMember]
        public string End;

        [DataMember]
        public string Location;

        [DataMember]
        public string FileList;

        [DataMember]
        public string FileName;

        [DataMember]
        public long Length;

        [DataMember]
        public int BufferSize;

        [DataMember]
        public byte[] DataArray;

        [DataMember]
        public MemoryStream Data;

        [DataMember]
        public string Content;

        public void Dispose()
        {
            if (Data != null)
            {
                Data.Close();
                Data = null;
            }
        }
    }
}
    /*
    [MessageContract]
    public class SyncResponse
    {
        [MessageHeader(MustUnderstand = true)]
        public bool Success;

        [MessageHeader(MustUnderstand = true)]
        public string Message;
    }

    [MessageContract]
    public class DownloadRequest
    {
        [MessageHeader(MustUnderstand = true)]
        public string FileName;

        [MessageHeader(MustUnderstand = true)]
        public long Offset;
    }

    [MessageContract]
    public class RemoteFileInfo : IDisposable
    {
        [MessageHeader(MustUnderstand = true)]
        public string Id;

        [MessageHeader(MustUnderstand = true)]
        public string Client;

        [MessageHeader(MustUnderstand = true)]
        public string Version;

        [MessageHeader(MustUnderstand = true)]
        public string Start;

        [MessageHeader(MustUnderstand = true)]
        public string End;

        [MessageHeader(MustUnderstand = true)]
        public string FileName;

        [MessageHeader(MustUnderstand = true)]
        public long Length;

        [MessageBodyMember(Order = 1)]
        public Stream Data;

        public void Dispose()
        {
            if (Data != null)
            {
                Data.Close();
                Data = null;
            }
        }
    }
}

    */