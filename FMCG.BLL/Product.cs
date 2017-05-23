using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FMCG.Common;

namespace FMCG.BLL
{
    public class Product : INotifyPropertyChanged
    {

        #region Constructor

        public Product()
        {

        }

        public Product(int ProductId)
        {
            Product p = toList.Where(x => x.Id == ProductId).FirstOrDefault();
            if (p == null) p = new Product();
            p.toCopy<Product>(this);
        }

        public Product(string ProductCode)
        {
            Product p = toList.Where(x => x.ItemCode == ProductCode).FirstOrDefault();
            if (p == null) p = new Product();
            p.toCopy<Product>(this);
        }

        #endregion

        #region Fields
        private static ObservableCollection<Product> _toList;

        private int _Id;
        private string _ProductName;
        private string _ItemCode;
        private int? _StockGroupId;
        private int? _UOMId;
        private decimal _SellingRate;
        private decimal _PurchaseRate;
        private decimal _MRP;
        private double _GST;
        private double? _OpeningStock;
        private double? _ReOrderLevel;
        private byte _ProductImage;
        private string _uomSymbol;
        private string _stockGroupName;
        private int _companyId;

        private double? _POQty;
        private double? _PQty;
        private double? _PRQty;

        private double? _SOQty;
        private double? _SQty;
        private double? _SRQty;

        #endregion

        #region Property
        public static ObservableCollection<Product> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<Product>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<Product>>("ProductList").Result;
                        _toList = new ObservableCollection<Product>(l1);
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

        public double AvailableStock
        {
            get
            {
                return (OpeningStock??0 + PQty??0 + SRQty??0) - (SQty??0 + PRQty??0);
            }
        }

        public bool IsReOrderLevel
        {
            get
            {
                return ReOrderLevel > AvailableStock;
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
                    NotifyPropertyChanged(nameof(ItemCode));
                }
            }
        }

        public int? StockGroupId
        {
            get
            {
                return _StockGroupId;
            }

            set
            {
                if (_StockGroupId != value)
                {
                    _StockGroupId = value;
                    NotifyPropertyChanged(nameof(StockGroupId));
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

        public decimal SellingRate
        {
            get
            {
                return _SellingRate;
            }

            set
            {
                if (_SellingRate != value)
                {
                    _SellingRate = value;
                    NotifyPropertyChanged(nameof(SellingRate));
                }
            }
        }

        public decimal PurchaseRate
        {
            get
            {
                return _PurchaseRate;
            }
            set
            {
                if (_PurchaseRate != value)
                {
                    _PurchaseRate = value;
                    NotifyPropertyChanged(nameof(PurchaseRate));
                }

            }
        }

        public decimal MRP
        {
            get
            {
                return _MRP;
            }
            set
            {
                if (_MRP != value)
                {
                    _MRP = value;
                    NotifyPropertyChanged(nameof(MRP));
                }

            }
        }

        public double GST
        {
            get
            {
                return _GST;
            }
            set
            {
                if (_GST != value)
                {
                    _GST = value;
                    NotifyPropertyChanged(nameof(GST));
                }

            }
        }

        public double? OpeningStock
        {
            get
            {
                return _OpeningStock;
            }
            set
            {
                if (_OpeningStock != value)
                {
                    _OpeningStock = value;
                    NotifyPropertyChanged(nameof(OpeningStock));
                }

            }
        }

        public double? ReOrderLevel
        {
            get
            {
                return _ReOrderLevel;
            }
            set
            {
                if (_ReOrderLevel != value)
                {
                    _ReOrderLevel = value;
                    NotifyPropertyChanged(nameof(ReOrderLevel));
                }

            }
        }

        public byte ProductImage
        {
            get
            {
                return _ProductImage;
            }
            set
            {
                if (_ProductImage != value)
                {
                    _ProductImage = value;
                    NotifyPropertyChanged(nameof(ProductImage));
                }

            }
        }

        public string StockGroupName
        {
            get
            {
                return _stockGroupName;
            }
            set
            {
                if (_stockGroupName != value)
                {
                    _stockGroupName = value;
                    NotifyPropertyChanged(nameof(StockGroupName));
                }
            }
        }

        public string uomSymbol
        {
            get
            {
                return _uomSymbol;
            }
            set
            {
                if (_uomSymbol != value)
                {
                    _uomSymbol = value;
                    NotifyPropertyChanged(nameof(uomSymbol));
                }
            }
        }

        public int CompanyId
        {
            get
            {
                return _companyId;
            }
            set
            {
                if (_companyId != value)
                {
                    _companyId = value;
                    NotifyPropertyChanged(nameof(CompanyId));
                }
            }
        }

        public double? POQty
        {
            get
            {
                return _POQty;
            }
            set
            {
                if (_POQty != value)
                {
                    _POQty = value;
                    NotifyPropertyChanged(nameof(POQty));
                }

            }
        }

        public double? PQty
        {
            get
            {
                return _PQty;
            }
            set
            {
                if (_PQty != value)
                {
                    _PQty = value;
                    NotifyPropertyChanged(nameof(PQty));
                }

            }
        }

        public double? PRQty
        {
            get
            {
                return _PRQty;
            }
            set
            {
                if (_PRQty != value)
                {
                    _PRQty = value;
                    NotifyPropertyChanged(nameof(PRQty));
                }

            }
        }

        public double? SOQty
        {
            get
            {
                return _SOQty;
            }
            set
            {
                if (_SOQty != value)
                {
                    _SOQty = value;
                    NotifyPropertyChanged(nameof(SOQty));
                }

            }
        }

        public double? SQty
        {
            get
            {
                return _SQty;
            }
            set
            {
                if (_SQty != value)
                {
                    _SQty = value;
                    NotifyPropertyChanged(nameof(SQty));
                }

            }
        }

        public double? SRQty
        {
            get
            {
                return _SRQty;
            }
            set
            {
                if (_SRQty != value)
                {
                    _SRQty = value;
                    NotifyPropertyChanged(nameof(SRQty));
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

        #region Methods

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {

                Product d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Product();
                    toList.Add(d);
                }

                this.toCopy<Product>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("Product_Save", this).Result;
                    d.Id = i;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;

            }

        }

        public void Clear()
        {
            new Product().toCopy<Product>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Product>(this);
                return true;
            }

            return false;
        }

        public bool Delete(bool isServerCall = false)
        {
            var d = toList.Where(x => x.Id == Id).FirstOrDefault();
            if (d != null)
            {
                toList.Remove(d);
                if (isServerCall == false) FMCGHubClient.FMCGHub.Invoke<int>("Product_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            if (toList.Where(x => x.ProductName.ToLower() == ProductName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }
            return RValue;

        }

        #endregion


    }
}
