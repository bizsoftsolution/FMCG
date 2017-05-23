using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace FMCG.BLL
{
    public class PaymentSupplier:INotifyPropertyChanged
    {

        #region fields

        private long _Id;
        private long _PaymentId;
        private int _SupplierId;
        private string _SupplierName;
        private long _PurchaseId;
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

        public long PurchaseId
        {
            get
            {
                return _PurchaseId;
            }
            set
            {
                if (_PurchaseId != value)
                {
                    _PurchaseId = value;
                    NotifyPropertyChanged(nameof(PurchaseId));
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
