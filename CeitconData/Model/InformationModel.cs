using Ceitcon_Data.Utilities;
using System;
using System.ComponentModel;

namespace Ceitcon_Data.Model
{
    public class InformationModel : INotifyPropertyChanged
    {
        public InformationModel(ProjectModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _ProjectName = "Project_" + _Id.Substring(0, 6);
            _Interval = new TimeSpan(0,1,0); //60 second default
        }

        public InformationModel(InformationModel copy, ProjectModel parent, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Parent = fullCopy ? copy.Parent : parent;
            _ProjectName = fullCopy ? copy.ProjectName : "Project_" + _Id.Substring(0, 6);
            _CreatorName = copy.CreatorName;
            _OwnerName = copy.OwnerName;
            _MainContact = copy.MainContact;
            _Phone = copy.Phone;
            _Interval = copy.Interval;
        }

        public InformationModel Save()
        {
            return new InformationModel(this, null, true);
        }

        public void Restore(InformationModel copy)
        {
            Memento.Enable = false;
            _Id = copy.Id;
            Parent = copy.Parent;
            ProjectName = copy.ProjectName;
            CreatorName = copy.CreatorName;
            OwnerName = copy.OwnerName;
            MainContact = copy.MainContact;
            Phone = copy.Phone;
            Interval = copy.Interval;
            Memento.Enable = true;
        }

        private string _Id;
        private ProjectModel _Parent;
        private string _ProjectName;
        private string _CreatorName;
        private string _OwnerName;
        private string _MainContact;
        private string _Phone;
        private TimeSpan _Interval;

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

        public ProjectModel Parent
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

        public string ProjectName
        {
            get { return _ProjectName; }
            set
            {
                if (_ProjectName != value)
                {
                    //Memento.Push(Save());
                    _ProjectName = value;
                    OnPropertyChanged("ProjectName");
                }
            }
        }

        public string CreatorName
        {
            get { return _CreatorName; }
            set
            {
                if (_CreatorName != value)
                {
                    //Memento.Push(Save());
                    _CreatorName = value;
                    OnPropertyChanged("CreatorName");
                }
            }
        }

        public string OwnerName
        {
            get { return _OwnerName; }
            set
            {
                if (_OwnerName != value)
                {
                    //Memento.Push(Save());
                    _OwnerName = value;
                    OnPropertyChanged("OwnerName");
                }
            }
        }

        public string MainContact
        {
            get { return _MainContact; }
            set
            {
                if (_MainContact != value)
                {
                    //Memento.Push(Save());
                    _MainContact = value;
                    OnPropertyChanged("MainContact");
                }
            }
        }

        public string Phone
        {
            get { return _Phone; }
            set
            {
                if (_Phone != value)
                {
                    //Memento.Push(Save());
                    _Phone = value;
                    OnPropertyChanged("Phone");
                }
            }
        }

        public TimeSpan Interval
        {
            get { return _Interval; }
            set
            {
                if (_Interval != value)
                {
                    //Memento.Push(Save());
                    _Interval = value;
                    OnPropertyChanged("Interval");
                }
            }
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
