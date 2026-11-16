using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace KooliProjekt.WpfApplication.Api
{
    public class Car : INotifyPropertyChanged
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
            set => SetProperty(ref _id, value);
        }

        public string NumberPlate
        {
            get => _numberPlate;
            set => SetProperty(ref _numberPlate, value);
        }

        public int TypeId
        {
            get => _typeId;
            set => SetProperty(ref _typeId, value);
        }

        public decimal Kmrate
        {
            get => _kmrate;
            set => SetProperty(ref _kmrate, value);
        }

        public decimal TimeRate
        {
            get => _timeRate;
            set => SetProperty(ref _timeRate, value);
        }

        public bool IsAvailable
        {
            get => _isAvailable;
            set => SetProperty(ref _isAvailable, value);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
            {
                return false;
            }

            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
