using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace FMCG.BLL
{
    public class PaymentCustomer:INotifyPropertyChanged
    {


        #region fields

        private long _Id;
        private long _PaymentId;
        private int _CustomerId;
        private string _CustomerName;
        private long _SalesReturnId;
        private Decimal? _Amount;
        #endregion

        #region Property

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

        public long PaymentId
        {
            get
            {
                return _PaymentId;
            }
            set
            {
                if (_PaymentId != value)
                {
                    _PaymentId = value;
                    NotifyPropertyChanged(nameof(PaymentId));
                }
            }
        }

        public int CustomerId
        {
            get
            {
                return _CustomerId;
            }
            set
            {
                if (_CustomerId != value)
                {
                    _CustomerId = value;
                    NotifyPropertyChanged(nameof(CustomerId));
                }
            }
        }

        public string CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                if (_CustomerName != value)
                {
                    _CustomerName = value;
                    NotifyPropertyChanged(nameof(CustomerName));
                }
            }
        }

        public long SalesReturnId
        {
            get
            {
                return _SalesReturnId;
            }
            set
            {
                if (_SalesReturnId != value)
                {
                    _SalesReturnId = value;
                    NotifyPropertyChanged(nameof(SalesReturnId));
                }
            }
        }

        public Decimal? Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                    NotifyPropertyChanged(nameof(Amount));
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
