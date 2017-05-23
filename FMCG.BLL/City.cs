using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace FMCG.BLL
{
    public class City:INotifyPropertyChanged
    {
        #region Fields
        private static ObservableCollection<City> _toList;

        private int _id;
        private string _cityName;
        private int _stateId;
        #endregion

        #region Property
        public static ObservableCollection<City> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<City>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<City>>("City_List").Result;
                        _toList = new ObservableCollection<City>(l1);
                    }
                }
                catch (Exception ex)
                {

                }

                return _toList;
            }
            set
            {
                _toList = value;
            }
        }


        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                if(_id!=value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }
        public string CityName
        {
            get
            {
                return _cityName;
            }
            set
            {
                if (_cityName != value)
                {
                    _cityName = value;
                    NotifyPropertyChanged(nameof(CityName));
                }
            }
        }
        public int StateId
        {
            get
            {
                return _stateId;
            }
            set
            {
                if (_stateId != value)
                {
                    _stateId = value;
                    NotifyPropertyChanged(nameof(CityName));
                }
            }
        }
        #endregion


        #region Property Changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String ProperName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(ProperName));
        }
        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion

    }
}
