using Ceitcon_Data.Utilities;
using System.Windows;

namespace Ceitcon_Data.Model.Playlist
{
    public class AnimateBorderModel : PlaylistModel
    {
        public AnimateBorderModel(ControlModel parent) : base(parent)
        {
            _Type = PlaylistType.AnimateBorder;
            _Name = "Animate Border";
        }

        public AnimateBorderModel(AnimateBorderModel copy, ControlModel parent, bool fullCopy = false) : base(copy, parent, fullCopy)
        {
            _Type = PlaylistType.AnimateBorder;
            _Name = copy.Name;
            _BorderThicknessFrom = copy.BorderThicknessFrom;
            _BorderThicknessTo = copy.BorderThicknessTo;
        }

        public override PlaylistModel Save()
        {
            return new AnimateBorderModel(this, null, true);
        }

        public override void Restore(object copyObj)
        {
            var copy = copyObj as AnimateBorderModel;
            Memento.Enable = false;
            base.Restore(copy);
            BorderThicknessFrom = copy.BorderThicknessFrom;
            BorderThicknessTo = copy.BorderThicknessTo;
            Memento.Enable = true;
        }

        private Thickness _BorderThicknessFrom;
        private Thickness _BorderThicknessTo;

        public Thickness BorderThicknessFrom
        {
            get { return _BorderThicknessFrom; }
            set
            {
                if (_BorderThicknessFrom != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessFrom = value;
                    OnPropertyChanged("BorderThicknessFrom");
                    OnPropertyChanged("BorderThicknessLeftFrom");
                    OnPropertyChanged("BorderThicknessRightFrom");
                    OnPropertyChanged("BorderThicknessTopFrom");
                    OnPropertyChanged("BorderThicknessBottomFrom");
                }
            }
        }

        public double BorderThicknessLeftFrom
        {
            get { return _BorderThicknessFrom.Left; }
            set
            {
                if (_BorderThicknessFrom.Left != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessFrom.Left = value;
                    OnPropertyChanged("BorderThicknessLeftFrom");
                    OnPropertyChanged("BorderThicknessFrom");
                }
            }
        }

        public double BorderThicknessRightFrom
        {
            get { return _BorderThicknessFrom.Right; }
            set
            {
                if (_BorderThicknessFrom.Right != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessFrom.Right = value;
                    OnPropertyChanged("BorderThicknessRightFrom");
                    OnPropertyChanged("BorderThicknessFrom");
                }
            }
        }

        public double BorderThicknessTopFrom
        {
            get { return _BorderThicknessFrom.Top; }
            set
            {
                if (_BorderThicknessFrom.Top != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessFrom.Top = value;
                    OnPropertyChanged("BorderThicknessTopFrom");
                    OnPropertyChanged("BorderThicknessFrom");
                }
            }
        }

        public double BorderThicknessBottomFrom
        {
            get { return _BorderThicknessFrom.Bottom; }
            set
            {
                if (_BorderThicknessFrom.Bottom != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessFrom.Bottom = value;
                    OnPropertyChanged("BorderThicknessBottomFrom");
                    OnPropertyChanged("BorderThicknessFrom");
                }
            }
        }

        public Thickness BorderThicknessTo
        {
            get { return _BorderThicknessTo; }
            set
            {
                if (_BorderThicknessTo != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessTo = value;
                    OnPropertyChanged("BorderThicknessTo");
                    OnPropertyChanged("BorderThicknessLeftTo");
                    OnPropertyChanged("BorderThicknessRightTo");
                    OnPropertyChanged("BorderThicknessTopTo");
                    OnPropertyChanged("BorderThicknessBottomTo");
                }
            }
        }

        public double BorderThicknessLeftTo
        {
            get { return _BorderThicknessTo.Left; }
            set
            {
                if (_BorderThicknessTo.Left != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessTo.Left = value;
                    OnPropertyChanged("BorderThicknessLeftTo");
                    OnPropertyChanged("BorderThicknessTo");
                }
            }
        }

        public double BorderThicknessRightTo
        {
            get { return _BorderThicknessTo.Right; }
            set
            {
                if (_BorderThicknessTo.Right != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessTo.Right = value;
                    OnPropertyChanged("BorderThicknessRightTo");
                    OnPropertyChanged("BorderThicknessTo");
                }
            }
        }

        public double BorderThicknessTopTo
        {
            get { return _BorderThicknessTo.Top; }
            set
            {
                if (_BorderThicknessTo.Top != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessTo.Top = value;
                    OnPropertyChanged("BorderThicknessTopTo");
                    OnPropertyChanged("BorderThicknessTo");
                }
            }
        }

        public double BorderThicknessBottomTo
        {
            get { return _BorderThicknessTo.Bottom; }
            set
            {
                if (_BorderThicknessTo.Bottom != value)
                {
                    Memento.Push(Save());
                    _BorderThicknessTo.Bottom = value;
                    OnPropertyChanged("BorderThicknessBottomTo");
                    OnPropertyChanged("BorderThicknessTo");
                }
            }
        }
    }
}
