using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
    public class JournalOnline : INotifyPropertyChanged
    {
        #region Fields

        private long _Id;
        private long _JournalId;
        private Nullable<int> _BankId;
        private string _RefNo;
        private Nullable<System.DateTime> _TransactionDate;
        private string _BankName;

        #endregion

        #region Properties

        public long Id
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
        public long JournalId
        {
            get
            {
                return _JournalId;
            }
            set
            {
                if (_JournalId != value)
                {
                    _JournalId = value;
                    NotifyPropertyChanged(nameof(JournalId));
                }
            }
        }
        public Nullable<int> BankId
        {
            get
            {
                return _BankId;
            }
            set
            {
                if (_BankId != value)
                {
                    _BankId = value;
                    NotifyPropertyChanged(nameof(BankId));
                }
            }
        }
        public string RefNo
        {
            get
            {
                return _RefNo;
            }
            set
            {
                if (_RefNo != value)
                {
                    _RefNo = value;
                    NotifyPropertyChanged(nameof(RefNo));
                }
            }
        }
        public Nullable<System.DateTime> TransactionDate
        {
            get
            {
                return _TransactionDate;
            }
            set
            {
                if (_TransactionDate != value)
                {
                    _TransactionDate = value;
                    NotifyPropertyChanged(nameof(TransactionDate));
                }
            }
        }

        public string BankName
        {
            get
            {
                return _BankName;
            }
            set
            {
                if (_BankName != value)
                {
                    _BankName = value;
                    NotifyPropertyChanged(nameof(BankName));
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
