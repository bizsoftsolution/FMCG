using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace FMCG.BLL
{
   public class Gender:INotifyPropertyChanged
    {

        #region Fields
        private static ObservableCollection<Gender> _toList;

        private int _id;
        private string _GenderName;
        #endregion

        #region Property
        public static ObservableCollection<Gender> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<Gender>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<Gender>>("Gender_List").Result;
                        _toList = new ObservableCollection<Gender>(l1);
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
        public string GenderName
        {
            get
            {
                return _GenderName;
            }
            set
            {
                if (_GenderName != value)
                {
                    _GenderName = value;
                    NotifyPropertyChanged(nameof(GenderName));
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
