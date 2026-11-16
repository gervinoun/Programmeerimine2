using KooliProjekt.WpfApplication;

namespace KooliProjekt.WpfApplication.Api
{
    public class Car : NotifyPropertyChangedBase
    {
        private int _id;
        private string _numberPlate;
        private int _typeId;
        private decimal _kmrate;
        private decimal _timeRate;
        private bool _isAvailable;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string NumberPlate
        {
            get => _numberPlate;
            set
            {
                if (_numberPlate != value)
                {
                    _numberPlate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int TypeId
        {
            get => _typeId;
            set
            {
                if (_typeId != value)
                {
                    _typeId = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal Kmrate
        {
            get => _kmrate;
            set
            {
                if (_kmrate != value)
                {
                    _kmrate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public decimal TimeRate
        {
            get => _timeRate;
            set
            {
                if (_timeRate != value)
                {
                    _timeRate = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set
            {
                if (_isAvailable != value)
                {
                    _isAvailable = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
