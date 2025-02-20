using log4net;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ceitcon_Data.Model.Network
{
    //Statuses
    //Status 0: unknown status
    //Status 1: mean that the downloader is running but no content to show which mean the player is not running the status color should be Orange
    //Status 2: mean that the downloader is running but the player is not running becoz it was unregisterd from the server its status color should be yellow
    //Status 3: mean the downloader and player both are running so its status color should be Green
    //Status 4: no player, no download
    //Status 5: player was closed normaly
    //If have IP addres then show Proow of Play
    //Show registred button when status is 2
    //Show Unregistred button when status is 1 and 3


    /// <summary>
    /// |  Status No  | Downloader  |  Player  | Content| Colour  | IP  |  POP | Unregistred | Registred |
    /// |      0      |      ?      |     ?    |    ?   |  Gray   |  -  |  -   |     -       |     -     | (Unknown)
    /// |      1      |      +      |     -    |    -   |  Orange |  -  |  -   |     +       |     -     | (No Content)
    /// |      2      |      +      |     -    |    ?   |  Yelow  |  -  |  +   |     -       |     +     | (Player down)
    /// |      3      |      +      |     +    |    ?   |  Green  |  +  |  -   |     +       |     -     | (OK)
    /// |      4      |      -      |     -    |    ?   |  Red    |  -  |  -   |     +       |     -     | (Disconnected)
    /// |      5      |      -      |     -    |    ?   |  White  |  +  |  -   |     +       |     -     | (Closed normaly)
    /// </summary>
    public class PlayerModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Name;
        private string _Description;
        private string _HostName;
        private string _IPAddress;
        private int _VNCPort;
        private DateTime? _RegistredTime;
        private int _Licence;
        private int _Screens;
        private int _RefreshTime;
        private int _Status;
        private DateTime? _LastConnection;
        private bool _Active;
        private PlayerGroupModel _Parent;
        private ObservableCollection<FaceGroupModel> _FaceGroups;

        public PlayerModel(PlayerGroupModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Name = String.Format("Player_{0}", _Id.Substring(0, 6));
            _VNCPort = 52962;
            _Licence = 0;
            _Screens = 0;
            _RefreshTime = 30;
            _Status = 0;
            _Parent = parent;
            _FaceGroups = new ObservableCollection<FaceGroupModel>();
            _Active = true;
        }

        public string Id
        {
            get { return _Id; }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                if (_Name != value)
                {
                    _Name = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("Identification");
                }
            }
        }

        public string Description
        {
            get { return _Description; }
            set
            {
                if (_Description != value)
                {
                    _Description = value;
                    OnPropertyChanged("Description");
                }
            }
        }

        public string HostName
        {
            get { return _HostName; }
            set
            {
                if (_HostName != value)
                {
                    _HostName = value;
                    OnPropertyChanged("HostName");
                }
            }
        }

        public string IPAddress
        {
            get { return _IPAddress; }
            set
            {
                if (_IPAddress != value)
                {
                    _IPAddress = value;
                    OnPropertyChanged("IPAddress");
                }
            }
        }

        public int VNCPort
        {
            get { return _VNCPort; }
            set
            {
                if (_VNCPort != value)
                {
                    _VNCPort = value;
                    OnPropertyChanged("VNCPort");
                }
            }
        }

        public DateTime? RegistredTime
        {
            get { return _RegistredTime; }
            set
            {
                if (_RegistredTime != value)
                {
                    _RegistredTime = value;
                    OnPropertyChanged("RegistredTime");
                }
            }
        }

        public int Licence
        {
            get { return _Licence; }
            set
            {
                if (_Licence != value)
                {
                    _Licence = value;
                    OnPropertyChanged("Licence");
                    OnPropertyChanged("IsRegistred");
                    OnPropertyChanged("IsUnregistred");
                    OnPropertyChanged("StatusIcon");
                    OnPropertyChanged("Connected");
                }
            }
        }

        public int Screens
        {
            get { return _Screens; }
            set
            {
                if (_Screens != value)
                {
                    _Screens = value;
                    OnPropertyChanged("Screens");
                }
            }
        }

        public int RefreshTime
        {
            get { return _RefreshTime; }
            set
            {
                if (_RefreshTime != value)
                {
                    _RefreshTime = value;
                    OnPropertyChanged("RefreshTime");
                }
            }
        }

        public int Status
        {
            get { return _Status; }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    OnPropertyChanged("Status");
                    OnPropertyChanged("StatusIcon");
                    OnPropertyChanged("StatusText");
                }
            }
        }

        public DateTime? LastConnection
        {
            get { return _LastConnection; }
            set
            {
                if (_LastConnection != value)
                {
                    _LastConnection = value;
                    OnPropertyChanged("LastConnection");
                    OnPropertyChanged("StatusIcon");
                }
            }
        }

        public bool Active
        {
            get { return _Active; }
            set
            {
                if (_Active != value)
                {
                    _Active = value;
                    OnPropertyChanged("Active");
                }
            }
        }

        public PlayerGroupModel Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent != value)
                {
                    _Parent = value;
                    OnPropertyChanged("Parent");
                }
            }
        }

        public ObservableCollection<FaceGroupModel> FaceGroups
        {
            get { return _FaceGroups; }
            set
            {
                if (_FaceGroups != value)
                {
                    _FaceGroups = value;
                    OnPropertyChanged("FaceGroups");
                }
            }
        }

        public string Identification
        {
            get
            {
                return String.Format("{0}/{1}", Parent.Identification, Name);
            }
        }

        #region Statuses

        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        public bool IsRegistred
        {
            get
            {
                log.Info($"IsRegistred License value {Licence} ");
                log.Info($" IsRegistred License result {(!String.IsNullOrEmpty(HostName) && !String.IsNullOrEmpty(IPAddress)).ToString()} ");
                
                //return Licence > 0;
                return !String.IsNullOrEmpty(HostName) && !String.IsNullOrEmpty(IPAddress);
            }
        }

        public bool IsUnregistred
        {
            get
            {
                log.Info($"IsUnregistred License value {Licence} ");
                log.Info($"IsUnregistred Host value {HostName} ");
                log.Info($"IsUnregistred IPAddress value {IPAddress} ");
                //log.Info($" IsUnregistred License result {(!String.IsNullOrEmpty(HostName) && !String.IsNullOrEmpty(IPAddress)).ToString()} ");
                //return Licence == 0 && !String.IsNullOrEmpty(HostName) && !String.IsNullOrEmpty(IPAddress);
                //return  !String.IsNullOrEmpty(HostName) && !String.IsNullOrEmpty(IPAddress);
                log.Info($" IsUnregistred License result {(Licence > 0).ToString()} ");
                return String.IsNullOrEmpty(HostName) && String.IsNullOrEmpty(IPAddress);
            }
        }

        public string StatusIcon
        {
            get
            {
                switch (Status)
                {
                    case 0:
                        return @"../Images/iconStatusGray.png";
                    case 1:
                        return @"../Images/iconStatusOrange.png";
                    case 2:
                        return @"../Images/iconStatusYellow.png";
                    case 3:
                        {
                            if (LastConnection > DateTime.Now.AddMinutes(-10))
                                return @"../Images/iconStatusGreen.png";
                            else
                                return @"../Images/iconStatusGray.png";
                        }
                    case 4:
                        return @"../Images/iconStatusRed.png";
                    case 5:
                        return @"../Images/iconStatusWhite.png";
                    default:
                        return String.Empty;
                }
            }
        }

        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case 0:
                        return "Unknown status";
                    case 1:
                        return @"No content";
                    case 2:
                        return @"Payer not started";
                    case 3:
                        {
                            if (LastConnection > DateTime.Now.AddMinutes(-10))
                                return @"All is OK";
                            else
                                return @"Not responding";
                        }
                    case 4:
                        return @"Disconnected";
                    case 5:
                        return @"Closed normaly";
                    default:
                        return String.Empty;
                }
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }
}
