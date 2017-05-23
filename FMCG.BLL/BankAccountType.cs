using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FMCG.BLL
{
    public class BankAccountType:INotifyPropertyChanged
    {

        #region Fields

        private static List<BLL.BankAccountType> _tolist;

        private int _id;
        private string _accountType;
        #endregion

        #region Property
        public static List<BLL.BankAccountType> toList
        {
            get
            {
                if(_tolist==null)
                {
                    _tolist = new List<BankAccountType>();
                    _tolist = FMCGHubClient.FMCGHub.Invoke<List<BLL.BankAccountType>>("accountType_List").Result;
                }
                return _tolist;
            }
            set
            {
                _tolist = value;
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

        public string AccountType
        {
            get
            {
                return _accountType;
            }
            set
            {
                if (_accountType != value)
                {
                    _accountType = value;
                    NotifyPropertyChanged(nameof(AccountType));
                }
            }
        }
        #endregion


        #region Property  Changed Event

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion

    }
}
