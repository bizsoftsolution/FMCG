using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
    public class TransactionType : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<TransactionType> _toList;

        private int _Id;
        private string _Type;
        #endregion

        #region Property

        public static ObservableCollection<TransactionType> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<TransactionType>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<TransactionType>>("TransactionType_List").Result;
                        _toList = new ObservableCollection<TransactionType>(l1);
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
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    _Id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }
        public string Type
        {
            get
            {
                return _Type;
            }
            set
            {
                if (_Type != value)
                {
                    _Type = value;
                    NotifyPropertyChanged(nameof(Type));
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
