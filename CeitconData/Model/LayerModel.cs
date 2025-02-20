using Ceitcon_Data.Utilities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Ceitcon_Data.Model
{
    public class LayerModel : INotifyPropertyChanged
    {
        public LayerModel(SlideModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _Name = "Layer_" + _Id.Substring(0, 6);
            _Controls = new ObservableCollection<ControlModel>();
            _ZIndex = 1;
            _IsSelected = true;
            _IsVisible = true;

            //For test
            //var cm = new ControlModel(ControlType.Image, this);
            //_Controls.Add(cm);
            //_SelectedControl = cm;
        }

        public LayerModel(LayerModel copy, SlideModel parent, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Parent = fullCopy ? copy.Parent : parent;
            _Name = fullCopy ? copy.Name : "Layer_" + _Id.Substring(0, 6);
            _ZIndex = copy.ZIndex;
            _IsVisible = copy.IsVisible;
            _IsLocked = copy.IsLocked;
            _IsSelected = copy.IsSelected;
            _Controls = new ObservableCollection<ControlModel>();
            foreach (var i in copy.Controls)
            {
                _Controls.Add(new ControlModel(i, this, fullCopy));
            }
            _SelectedControl = _Controls.FirstOrDefault();
        }

        public LayerModel Save()
        {
            return new LayerModel(this, null, true);
        }

        public void Restore(LayerModel copy)
        {
            Memento.Enable = false;
            _Id = copy.Id;
            Parent = copy.Parent;
            Name = copy.Name;
            ZIndex = copy.ZIndex;
            IsVisible = copy.IsVisible;
            IsLocked = copy.IsLocked;
            IsSelected = copy.IsSelected;
            Controls.Clear();
            foreach (ControlModel item in copy.Controls)
            {
                Controls.Add(new ControlModel(item,null,true));
            }
            SelectedControl = copy.SelectedControl;
            Memento.Enable = true;
        }

        private string _Id;
        private SlideModel _Parent;
        private string _Name;
        private ObservableCollection<ControlModel> _Controls;
        private ControlModel _SelectedControl;
        private int _ZIndex;
        private bool _IsVisible;
        private bool _IsLocked;
        private bool _IsSelected;


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

        public SlideModel Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent != value)
                {
                    Memento.Push(Save());
                    _Parent = value;
                    OnPropertyChanged("Parent");
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
                    Memento.Push(Save());
                    _Name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public ObservableCollection<ControlModel> Controls
        {
            get { return _Controls; }
            set
            {
                if (_Controls != value)
                {
                    //Memento.Push(Save());
                    _Controls = value;
                    OnPropertyChanged("Controls");
                }
            }
        }

        public ControlModel SelectedControl
        {
            get { return _SelectedControl; }
            set
            {
                _Parent.Parent.Parent.SelectedObject = value;
                if (_SelectedControl != value)
                {
                    //Memento.Push(Save());
                    _SelectedControl = value;
                    OnPropertyChanged("SelectedControl");
                    OnPropertyChanged("HasSelectedControl");
                }
            }
        }

        public bool HasSelectedControl
        {
            get { return _SelectedControl != null; }
        }

        public int ZIndex
        {
            get { return _ZIndex; }
            set
            {
                if (_ZIndex != value)
                {
                    Memento.Push(Save());
                    _ZIndex = value;
                    OnPropertyChanged("ZIndex");
                }
            }
        }

        public bool IsVisible
        {
            get { return _IsVisible; }
            set
            {
                if (_IsVisible != value)
                {
                    Memento.Push(Save());
                    _IsVisible = value;
                    OnPropertyChanged("IsVisible");
                }
            }
        }

        public bool IsLocked
        {
            get { return _IsLocked; }
            set
            {
                if (_IsLocked != value)
                {
                    Memento.Push(Save());
                    _IsLocked = value;
                    OnPropertyChanged("IsLocked");
                }
            }
        }

        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                if (_IsSelected != value)
                {
                    //Memento.Push(Save());
                    _IsSelected = value;
                    OnPropertyChanged("IsSelected");
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
