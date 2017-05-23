using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
    public class JournalCheque : INotifyPropertyChanged
    {

        #region Fields

        private long _Id;
        private long _JournalId;
        private Nullable<int> _BankId;
        private string _ChequeNo;
        private Nullable<System.DateTime> _IssueDate;
        private Nullable<System.DateTime> _ChequeDate;
        private Nullable<System.DateTime> _CollectionDate;
        private Nullable<System.DateTime> _ReturnDate;
        private Nullable<decimal> _ReturnCharge;
        private string _BankName;
        private string _Status;

        private bool _IsShowReturn;
        private bool _IsShowComplete;



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
        public string ChequeNo
        {
            get
            {
                return _ChequeNo;
            }
            set
            {
                if (_ChequeNo != value)
                {
                    _ChequeNo = value;
                    NotifyPropertyChanged(nameof(ChequeNo));
                }
            }
        }
        public Nullable<System.DateTime> IssueDate
        {
            get
            {
                return _IssueDate;
            }
            set
            {
                if (_IssueDate != value)
                {
                    _IssueDate = value;
                    NotifyPropertyChanged(nameof(IssueDate));
                }
            }
        }
        public Nullable<System.DateTime> ChequeDate
        {
            get
            {
                return _ChequeDate;
            }
            set
            {
                if (_ChequeDate != value)
                {
                    _ChequeDate = value;
                    NotifyPropertyChanged(nameof(ChequeDate));
                }
            }
        }
        public Nullable<System.DateTime> CollectionDate
        {
            get
            {
                return _CollectionDate;
            }
            set
            {
                if (_CollectionDate != value)
                {
                    _CollectionDate = value;
                    NotifyPropertyChanged(nameof(CollectionDate));
                }
            }
        }
        public Nullable<System.DateTime> ReturnDate
        {
            get
            {
                return _ReturnDate;
            }
            set
            {
                if (_ReturnDate != value)
                {
                    _ReturnDate = value;
                    NotifyPropertyChanged(nameof(ReturnDate));
                }
            }
        }
        public Nullable<decimal> ReturnCharge
        {
            get
            {
                return _ReturnCharge;
            }
            set
            {
                if (_ReturnCharge != value)
                {
                    _ReturnCharge = value;
                    NotifyPropertyChanged(nameof(ReturnCharge));
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

        public string Status
        {
            get
            {
                return _Status;
            }
            set
            {
                if (_Status != value)
                {
                    _Status = value;
                    IsShowComplete = value == "Completed";
                    IsShowReturn = value == "Returned";
                    NotifyPropertyChanged(nameof(Status));
                }
            }
        }



        public bool IsShowComplete
        {
            get
            {
                return _IsShowComplete;
            }
            set
            {
                if (_IsShowComplete != value)
                {
                    _IsShowComplete = value;
                    NotifyPropertyChanged(nameof(IsShowComplete));
                }
            }
        }

        public bool IsShowReturn
        {
            get
            {
                return _IsShowReturn;
            }
            set
            {
                if (_IsShowReturn != value)
                {
                    _IsShowReturn = value;
                    NotifyPropertyChanged(nameof(IsShowReturn));
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
