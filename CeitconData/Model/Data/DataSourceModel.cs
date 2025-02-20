using Ceitcon_Data.Utilities;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Ceitcon_Data.Model.Data
{
    public class DataSourceModel : INotifyPropertyChanged
    {
        private string _Id;
        private string _Name;
        private string _Source;
        private string _SourceType;
        private string _Url;
        private string _UrlUsername;
        private string _UrlPassword;
        //private string _WebService;
        //private string _WebServiceMethod;
        private ObservableCollection<DataColumnModel> _Columns;
        private DataColumnModel _SelectedColumn;

        public DataSourceModel()
        {
            _Id = Guid.NewGuid().ToString();
            _Name = String.Format("DataSource_{0}", _Id.Substring(0, 6));
            _Columns = new ObservableCollection<DataColumnModel>();
            _SourceType = "XML File";
            _Url = String.Empty;
            _UrlUsername = String.Empty;
            _UrlPassword = String.Empty;
        }

        public DataSourceModel(DataSourceModel copy, bool fullCopy = false)
        {
            if (fullCopy) { _Id = copy.Id; } else { Guid.NewGuid().ToString(); }
            _Name = copy.Name;
            _Columns = new ObservableCollection<DataColumnModel>();
            foreach (DataColumnModel i in copy.Columns)
            {
                _Columns.Add(i);
            }
            _Source = copy.Source;
            _SourceType = copy.SourceType;
            _SelectedColumn = _Columns.FirstOrDefault();
            _Url = copy.Url;
            _UrlUsername = copy.SourceType;
            _UrlPassword = copy.UrlUsername;
        }

        public DataSourceModel Save()
        {
            return new DataSourceModel(this, true);
        }

        public void Restore(DataSourceModel copyObj)
        {
            var copy = copyObj as DataSourceModel;
            Memento.Enable = false;
            Name = copy.Name;
            _Columns.Clear();
            foreach (DataColumnModel i in copy.Columns)
            {
                Columns.Add(i);
            }
            Source = copy.Source;
            SourceType = copy.SourceType;
            SelectedColumn = _Columns.FirstOrDefault();
            Url = copy.Url;
            UrlUsername = copy.SourceType;
            UrlPassword = copy.UrlUsername;
            Memento.Enable = true;
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
                }
            }
        }

        public ObservableCollection<DataColumnModel> Columns
        {
            get { return _Columns; }
            set
            {
                if (_Columns != value)
                {
                    _Columns = value;
                    OnPropertyChanged("Columns");
                }
            }
        }

        public DataColumnModel SelectedColumn
        {
            get { return _SelectedColumn; }
            set
            {
                if (_SelectedColumn != value)
                {
                    _SelectedColumn = value;
                    OnPropertyChanged("SelectedColumn");
                }
            }
        }

        public string Source
        {
            get { return _Source; }
            set
            {
                if (_Source != value)
                {
                    _Source = value;
                    OnPropertyChanged("Source");
                }
            }
        }


        public string SourceType
        {
            get { return _SourceType; }
            set
            {
                if (_SourceType != value)
                {
                    _SourceType = value;
                    OnPropertyChanged("SourceType");
                }
            }
        }

        public string Url
        {
            get { return _Url; }
            set
            {
                if (_Url != value)
                {
                    _Url = value;
                    OnPropertyChanged("Url");
                }
            }
        }

        public string UrlUsername
        {
            get { return _UrlUsername; }
            set
            {
                if (_UrlUsername != value)
                {
                    _UrlUsername = value;
                    OnPropertyChanged("UrlUsername");
                }
            }
        }

        public string UrlPassword
        {
            get { return _UrlPassword; }
            set
            {
                if (_UrlPassword != value)
                {
                    _UrlPassword = value;
                    OnPropertyChanged("UrlPassword");
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

