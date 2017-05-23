﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace FMCG.BLL
{
    public class PurchaseReturnDetail:INotifyPropertyChanged
    {
        #region Field
        private long _Id;
        private long? _PRId;
        private long? _PDId;
        private int? _ProductId;
        private int? _UOMId;
        private double? _Quantity;
        private decimal? _UnitPrice;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _Amount;

        private string _ItemCode;
        private string _ProductName;
        private string _UOMName;
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

        public long? PRId
        {
            get
            {
                return _PRId;
            }
            set
            {
                if (_PRId != value)
                {
                    _PRId = value;
                    NotifyPropertyChanged(nameof(PRId));
                }
            }
        }

        public long? PDId
        {
            get
            {
                return _PDId;
            }
            set
            {
                if (_PDId != value)
                {
                    _PDId = value;
                    NotifyPropertyChanged(nameof(PDId));
                }
            }
        }


        public int? ProductId
        {
            get
            {
                return _ProductId;
            }
            set
            {
                if (_ProductId != value)
                {
                    _ProductId = value;
                    if (value != null) SetProduct(new Product(_ProductId.Value));
                    NotifyPropertyChanged(nameof(ProductId));
                }
            }
        }

        public int? UOMId
        {
            get
            {
                return _UOMId;
            }
            set
            {
                if (_UOMId != value)
                {
                    _UOMId = value;
                    NotifyPropertyChanged(nameof(UOMId));
                }
            }
        }

        public double? Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    Amount = Convert.ToDecimal(_Quantity ?? 0) * _UnitPrice ?? 0;
                    NotifyPropertyChanged(nameof(Quantity));
                }
            }
        }

        public decimal? UnitPrice
        {
            get
            {
                return _UnitPrice;
            }
            set
            {
                if (_UnitPrice != value)
                {
                    _UnitPrice = value;
                    Amount = Convert.ToDecimal(_Quantity ?? 0) * _UnitPrice ?? 0;
                    NotifyPropertyChanged(nameof(UnitPrice));
                }
            }
        }

        public decimal? DiscountAmount
        {
            get
            {
                return _DiscountAmount;
            }
            set
            {
                if (_DiscountAmount != value)
                {
                    _DiscountAmount = value;
                    NotifyPropertyChanged(nameof(DiscountAmount));
                }
            }
        }

        public decimal? GSTAmount
        {
            get
            {
                return _GSTAmount;
            }
            set
            {
                if (_GSTAmount != value)
                {
                    _GSTAmount = value;
                    NotifyPropertyChanged(nameof(GSTAmount));
                }
            }
        }

        public decimal? Amount
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

        public string ItemCode
        {
            get
            {
                return _ItemCode;
            }
            set
            {
                if (_ItemCode != value)
                {
                    _ItemCode = value;
                    if (value != "")
                    {
                        Product p = new Product(value);
                        if (p.Id != 0) ProductId = p.Id;
                    }
                    NotifyPropertyChanged(nameof(ItemCode));
                }
            }
        }

        public string ProductName
        {
            get
            {
                return _ProductName;
            }
            set
            {
                if (_ProductName != value)
                {
                    _ProductName = value;
                    NotifyPropertyChanged(nameof(ProductName));
                }
            }
        }

        public string UOMName
        {
            get
            {
                return _UOMName;
            }
            set
            {
                if (_UOMName != value)
                {
                    _UOMName = value;
                    NotifyPropertyChanged(nameof(UOMName));
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

        #region Methods
        private void SetProduct(Product p)
        {
            UOMId = p.UOMId;
            ProductName = p.ProductName;
            UnitPrice = p.PurchaseRate;
            Quantity = p.Id != 0 ? 1 : 0;
        }
        #endregion

    }
}
