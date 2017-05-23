using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace FMCG.BLL
{
    public class ReceiptCustomer:INotifyPropertyChanged
    {

        #region fields

        private long _Id;
        private long _ReceiptId;
        private int _CustomerId;
        private string _CustomerName;
        private long _SalesId;
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

        public long ReceiptId
        {
            get
            {
                return _ReceiptId;
            }
            set
            {
                if (_ReceiptId != value)
                {
                    _ReceiptId = value;
                    NotifyPropertyChanged(nameof(ReceiptId));
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

        public long SalesId
        {
            get
            {
                return _SalesId;
            }
            set
            {
                if (_SalesId != value)
                {
                    _SalesId = value;
                    NotifyPropertyChanged(nameof(SalesId));
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
                    NotifyPropertyChanged(nameof(SalesId));
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
