using HazeMY.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HazeMY.Models
{
    public class ExceptionLite
    {
        public string Name { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
    }

    public class HazeWithHistoryContainer
    {
        public ExceptionLite Exception { get; set; }
        public HazeWithHistory HazeWithHistory { get; set; }
    }

    public class HazeWithHistory : BindableObjectBase
    {
        private Haze _haze;
        public Haze Haze
        {
            get
            {
                return _haze;
            }

            set
            {
                _haze = value;
                OnPropertyChanged(() => Haze);
            }
        }

        private List<History> _histories;
        public List<History> Histories
        {
            get
            {
                return _histories;
            }

            set
            {
                _histories = value;
                OnPropertyChanged(() => Histories);
            }
        }
    }

    public class Haze : BindableObjectBase
    {
        private string _id;
        private string _location;
        private string _psi;
        private string _timeDiff;
        private string _color;

        public string ID
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                OnPropertyChanged(() => ID);
            }
        }

        public string Location
        {
            get
            {
                return _location;
            }

            set
            {
                _location = value;
                OnPropertyChanged(() => Location);
            }
        }

        public string PSI
        {
            get
            {
                return _psi;
            }

            set
            {
                _psi = value;
                OnPropertyChanged(() => PSI);
            }
        }

        public string TimeDiff
        {
            get
            {
                return _timeDiff;
            }

            set
            {
                _timeDiff = value;
                OnPropertyChanged(() => TimeDiff);
            }
        }

        public string Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                OnPropertyChanged(() => Color);
            }
        }
    }

    public class History : BindableObjectBase
    {
        private string _psi;
        private string _psiDiff;
        private string _color;
        private string _colorDiff;
        private string _timeDiff;

        public string PSI
        {
            get
            {
                return _psi;
            }

            set
            {
                _psi = value;
                OnPropertyChanged(() => PSI);
            }
        }

        public string PSIDiff
        {
            get
            {
                return _psiDiff;
            }

            set
            {
                _psiDiff = value;
                OnPropertyChanged(() => PSIDiff);
            }
        }

        public string Color
        {
            get
            {
                return _color;
            }

            set
            {
                _color = value;
                OnPropertyChanged(() => Color);
            }
        }

        public string ColorDiff
        {
            get
            {
                return _colorDiff;
            }

            set
            {
                _colorDiff = value;
                OnPropertyChanged(() => ColorDiff);
            }
        }

        public string TimeDiff
        {
            get
            {
                return _timeDiff;
            }

            set
            {
                _timeDiff = value;
                OnPropertyChanged(() => TimeDiff);
            }
        }
    }
}
