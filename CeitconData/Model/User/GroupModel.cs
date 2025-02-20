using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ceitcon_Data.Model.User
{
    public class GroupModel
    {
        public GroupModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Name = "Group_" + _Id.Substring(0, 6);
            _PermissionDesign = true;
            _PermissionScheduler = true;
            _PermissionAlert = true;
            _PermissionNetwork = true;
            _PermissionDataSource = true;
            _PermissionAirlineLogo = true;
            _PermissionPlayersInformation = true;
            _PermissionProjectsInformation = true;
            _PermissionProofOfPlay = true;
            _PermissionChannelsPublishing = true;
            _PermissionReport = true;
            _PermissionUser = true;
            _Users = new ObservableCollection<UserModel>();
        }

        public GroupModel(GroupModel copy, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Name = fullCopy ? copy.Name : "Group_" + _Id.Substring(0, 6);
            _PermissionDesign = copy.PermissionDesign;
            _PermissionScheduler = copy.PermissionScheduler;
            _PermissionAlert = copy.PermissionAlert;
            _PermissionNetwork = copy.PermissionNetwork;
            _PermissionDataSource = copy.PermissionDataSource;
            _PermissionAirlineLogo = copy.PermissionAirlineLogo;
            _PermissionPlayersInformation = copy.PermissionPlayersInformation;
            _PermissionProjectsInformation = copy.PermissionProjectsInformation;
            _PermissionProofOfPlay = copy.PermissionProofOfPlay;
            _PermissionChannelsPublishing = copy.PermissionChannelsPublishing;
            _PermissionReport = copy.PermissionReport;
            _PermissionUser = copy.PermissionUser;
            _Users = new ObservableCollection<UserModel>();
            foreach (var i in copy.Users)
            {
                _Users.Add(new UserModel(i, this, fullCopy));
            }
            _SelectedUser = _Users.FirstOrDefault();
        }

        public GroupModel Save()
        {
            return new GroupModel(this, true);
        }

        public void Restore(GroupModel copy)
        {
            _Id = copy.Id;
            Name = copy.Name;
            PermissionDesign = copy.PermissionDesign;
            PermissionScheduler = copy.PermissionScheduler;
            PermissionAlert = copy.PermissionAlert;
            PermissionNetwork = copy.PermissionNetwork;
            PermissionDataSource = copy.PermissionDataSource;
            PermissionAirlineLogo = copy.PermissionAirlineLogo;
            PermissionPlayersInformation = copy.PermissionPlayersInformation;
            PermissionProjectsInformation = copy.PermissionProjectsInformation;
            PermissionProofOfPlay = copy.PermissionProofOfPlay;
            PermissionChannelsPublishing = copy.PermissionChannelsPublishing;
            PermissionReport = copy.PermissionReport;
            PermissionUser = copy.PermissionUser;
            Users.Clear();
            foreach (var item in copy.Users)
            {
                Users.Add(new UserModel(item, null, true));
            }
            SelectedUser = copy.SelectedUser;
        }

        public string GetPermissions()
        {
            return String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}{11}"
                , PermissionDesign ? 1 : 0
                , PermissionScheduler ? 1 : 0
                , PermissionAlert ? 1 : 0
                , PermissionNetwork ? 1 : 0
                , PermissionDataSource ? 1 : 0
                , PermissionAirlineLogo ? 1 : 0
                , PermissionPlayersInformation ? 1 : 0
                , PermissionProjectsInformation ? 1 : 0
                , PermissionProofOfPlay ? 1 : 0
                , PermissionChannelsPublishing ? 1 : 0
                , PermissionReport ? 1 : 0
                , PermissionUser ? 1 : 0);
        }

        public bool SetPermissions(string permissinos)
        {
            if (String.IsNullOrWhiteSpace(permissinos) || permissinos.Length != 12)
                return false;

            var list = permissinos.ToCharArray();
            PermissionDesign = list[0] == '1';
            PermissionScheduler = list[1] == '1';
            PermissionAlert = list[2] == '1';
            PermissionNetwork = list[3] == '1';
            PermissionDataSource = list[4] == '1';
            PermissionAirlineLogo = list[5] == '1';
            PermissionPlayersInformation = list[6] == '1';
            PermissionProjectsInformation = list[7] == '1';
            PermissionProofOfPlay = list[8] == '1';
            PermissionChannelsPublishing = list[9] == '1';
            PermissionReport = list[10] == '1';
            PermissionUser = list[11] == '1';
            return true;
        }

        private string _Id;
        private string _Name;
        private bool _PermissionDesign;
        private bool _PermissionScheduler;
        private bool _PermissionAlert;
        private bool _PermissionNetwork;
        private bool _PermissionDataSource;
        private bool _PermissionAirlineLogo;
        private bool _PermissionPlayersInformation;
        private bool _PermissionProjectsInformation;
        private bool _PermissionProofOfPlay;
        private bool _PermissionChannelsPublishing;
        private bool _PermissionReport;
        private bool _PermissionUser;
        private ObservableCollection<UserModel> _Users;
        private UserModel _SelectedUser;

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
                }
            }
        }

        public bool PermissionDesign
        {
            get { return _PermissionDesign; }
            set
            {
                if (_PermissionDesign != value)
                {
                    _PermissionDesign = value;
                    OnPropertyChanged("PermissionDesign");
                }
            }
        }

        public bool PermissionScheduler
        {
            get { return _PermissionScheduler; }
            set
            {
                if (_PermissionScheduler != value)
                {
                    _PermissionScheduler = value;
                    OnPropertyChanged("PermissionScheduler");
                }
            }
        }

        public bool PermissionAlert
        {
            get { return _PermissionAlert; }
            set
            {
                if (_PermissionAlert != value)
                {
                    _PermissionAlert = value;
                    OnPropertyChanged("PermissionAlert");
                }
            }
        }

        public bool PermissionNetwork
        {
            get { return _PermissionNetwork; }
            set
            {
                if (_PermissionNetwork != value)
                {
                    _PermissionNetwork = value;
                    OnPropertyChanged("PermissionNetwork");
                }
            }
        }

        public bool PermissionDataSource
        {
            get { return _PermissionDataSource; }
            set
            {
                if (_PermissionDataSource != value)
                {
                    _PermissionDataSource = value;
                    OnPropertyChanged("PermissionDataSource");
                }
            }
        }

        public bool PermissionAirlineLogo
        {
            get { return _PermissionAirlineLogo; }
            set
            {
                if (_PermissionAirlineLogo != value)
                {
                    _PermissionAirlineLogo = value;
                    OnPropertyChanged("PermissionAirlineLogo");
                }
            }
        }

        public bool PermissionPlayersInformation
        {
            get { return _PermissionPlayersInformation; }
            set
            {
                if (_PermissionPlayersInformation != value)
                {
                    _PermissionPlayersInformation = value;
                    OnPropertyChanged("PermissionPlayersInformation");
                }
            }
        }

        public bool PermissionProjectsInformation
        {
            get { return _PermissionProjectsInformation; }
            set
            {
                if (_PermissionProjectsInformation != value)
                {
                    _PermissionProjectsInformation = value;
                    OnPropertyChanged("PermissionProjectsInformation");
                }
            }
        }

        public bool PermissionProofOfPlay
        {
            get { return _PermissionProofOfPlay; }
            set
            {
                if (_PermissionProofOfPlay != value)
                {
                    _PermissionProofOfPlay = value;
                    OnPropertyChanged("PermissionProofOfPlay");
                }
            }
        }

        public bool PermissionChannelsPublishing
        {
            get { return _PermissionChannelsPublishing; }
            set
            {
                if (_PermissionChannelsPublishing != value)
                {
                    _PermissionChannelsPublishing = value;
                    OnPropertyChanged("PermissionChannelsPublishing");
                }
            }
        }

        public bool PermissionReport
        {
            get { return _PermissionReport; }
            set
            {
                if (_PermissionReport != value)
                {
                    _PermissionReport = value;
                    OnPropertyChanged("PermissionReport");
                }
            }
        }

        public bool PermissionUser
        {
            get { return _PermissionUser; }
            set
            {
                if (_PermissionUser != value)
                {
                    _PermissionUser = value;
                    OnPropertyChanged("PermissionUser");
                }
            }
        }

        public ObservableCollection<UserModel> Users
        {
            get { return _Users; }
            set
            {
                if (_Users != value)
                {
                    _Users = value;
                    OnPropertyChanged("Users");
                }
            }
        }

        public UserModel SelectedUser
        {
            get { return _SelectedUser; }
            set
            {
                if (_SelectedUser != value)
                {
                    _SelectedUser = value;
                    OnPropertyChanged("SelectedUser");
                    OnPropertyChanged("HasSelectedUser");
                }
            }
        }

        public bool HasSelectedUser
        {
            get { return _SelectedUser != null; }
        }

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
