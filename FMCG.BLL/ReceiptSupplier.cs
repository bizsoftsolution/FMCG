using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace FMCG.BLL
{
    public class ReceiptSupplier:INotifyPropertyChanged
    {
        #region fields

        private long _Id;
        private long _ReceiptId;
        private int _SupplierId;
        private string _SupplierName;
        private long _PurchaseReturnId;
        private decimal? _Amount;
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

        public int SupplierId
        {
            get
            {
                return _SupplierId;
            }
            set
            {
                if (_SupplierId != value)
                {
                    _SupplierId = value;
                    NotifyPropertyChanged(nameof(SupplierId));
                }
            }
        }

        public string SupplierName
        {
            get
            {
                return _SupplierName;
            }
            set
            {
                if (_SupplierName != value)
                {
                    _SupplierName = value;
                    NotifyPropertyChanged(nameof(SupplierName));
                }
            }
        }

        public long PurchaseReturnId
        {
            get
            {
                return _PurchaseReturnId;
            }
            set
            {
                if (_PurchaseReturnId != value)
                {
                    _PurchaseReturnId = value;
                    NotifyPropertyChanged(nameof(PurchaseReturnId));
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
                    NotifyPropertyChanged(nameof(PurchaseReturnId));
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
