using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace FMCG.BLL
{
    public class State
    {
        #region Fields
        private static ObservableCollection<State> _toList;

        private int _id;
        private string _stateName;
        private int _countryId;
        #endregion

        #region Property
        public static ObservableCollection<State> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<State>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<State>>("State_List").Result;
                        _toList = new ObservableCollection<State>(l1);
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
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }
        public string StateName
        {
            get
            {
                return _stateName;
            }
            set
            {
                if (_stateName != value)
                {
                    _stateName = value;
                    NotifyPropertyChanged(nameof(StateName));
                }
            }
        }
        public int CountryId
        {
            get
            {
                return _countryId;
            }
            set
            {
                if (_countryId != value)
                {
                    _countryId = value;
                    NotifyPropertyChanged(nameof(CountryId));
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
