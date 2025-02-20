using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Ceitcon_Data.Utilities;

namespace Ceitcon_Data.Model
{
    public enum AlertType
    {
        Prayer = 0,
        Global = 1,
    }

    public class AlertModel : INotifyPropertyChanged
    {
        public AlertModel(ProjectModel parent)
        {
            _Id = Guid.NewGuid().ToString();
            _Parent = parent;
            _Name = "Alert_" + _Id.Substring(0, 6);
            _Type = AlertType.Prayer;
            _Controls = new ObservableCollection<ControlModel>();
            _Slots = new ObservableCollection<SlotModel>();
            _ZIndex = 1;
            _IsSelected = true;
            _IsVisible = true;
        }

        public AlertModel(AlertModel copy, ProjectModel parent, bool fullCopy = false)
        {
            _Id = fullCopy ? copy.Id : Guid.NewGuid().ToString();
            _Parent = fullCopy ? copy.Parent : parent;
            _Name = fullCopy ? copy.Name : "Alert_" + _Id.Substring(0, 6);
            _Type = copy.Type;
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
            _Slots = new ObservableCollection<SlotModel>();
            foreach (var i in copy.Slots)
            {
                _Slots.Add(new SlotModel(i, this, fullCopy));
            }
            _SelectedSlot = _Slots.FirstOrDefault();
        }

        public AlertModel Save()
        {
            return new AlertModel(this, null, true);
        }

        public void Restore(AlertModel copy)
        {
            Memento.Enable = false;
            _Id = copy.Id;
            Parent = copy.Parent;
            Name = copy.Name;
            Type = copy.Type;
            ZIndex = copy.ZIndex;
            IsVisible = copy.IsVisible;
            IsLocked = copy.IsLocked;
            IsSelected = copy.IsSelected;
            Controls.Clear();
            foreach (ControlModel item in copy.Controls)
            {
                Controls.Add(new ControlModel(item, null, true));
            }
            SelectedControl = copy.SelectedControl;
            Slots.Clear();
            foreach (SlotModel item in copy.Slots)
            {
                Slots.Add(new SlotModel(item, null, true));
            }
            SelectedSlot = copy.SelectedSlot;
            Memento.Enable = true;
        }

        private string _Id;
        private ProjectModel _Parent;
        private string _Name;
        private AlertType _Type;
        private ObservableCollection<ControlModel> _Controls;
        private ControlModel _SelectedControl;
        private ObservableCollection<SlotModel> _Slots;
        private SlotModel _SelectedSlot;
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

        public ProjectModel Parent
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

        public AlertType Type
        {
            get { return _Type; }
            set
            {
                if (_Type != value)
                {
                    Memento.Push(Save());
                    _Type = value;
                    OnPropertyChanged("Type");
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
                _Parent.SelectedObject = value;
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


        public ObservableCollection<SlotModel> Slots
        {
            get { return _Slots; }
            set
            {
                if (_Slots != value)
                {
                    //Memento.Push(Save());
                    _Slots = value;
                    OnPropertyChanged("Slots");
                }
            }
        }

        public SlotModel SelectedSlot
        {
            get { return _SelectedSlot; }
            set
            {
                _Parent.SelectedObject = value;
                if (_SelectedSlot != value)
                {
                    //Memento.Push(Save());
                    _SelectedSlot = value;
                    OnPropertyChanged("SelectedSlot");
                    OnPropertyChanged("HasSelectedSlot");
                }
            }
        }

        public bool HasSelectedSlot
        {
            get { return _SelectedSlot != null; }
        }

        public int Width
        {
            get { return Parent.SelectedResolution.Width; }
        }

        public int Height
        {
            get { return Parent.SelectedResolution.Height; }
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
