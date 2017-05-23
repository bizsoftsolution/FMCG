using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FMCG.Common;
using System.Collections.ObjectModel;

namespace FMCG.BLL
{
    public class Customer : INotifyPropertyChanged
    {
        #region Fields
        private static ObservableCollection<Customer> _toList;


        private int _Id;
        private string _CustomerName;
        private string _PersonIncharge;
        private string _AddressLine1;
        private string _AddressLine2;
        private int _CityId;
        private string _TelephoneNo;
        private string _MobileNo;
        private string _EMailId;
        private string _GSTNo;
        private int _CreditLimit;
        private int _CreditLimitTypeId;
        private double _CreditAmount;
        private int? _AccountGroupId;
        private decimal _LedgerOPDr;
        private decimal _LedgerOPCr;
        private int _CompanyId;
        private decimal? _BillingAmount;
        private decimal? _CrBillingAmount;
        private decimal? _PaidAmount;

        private string _cityName;
        private string _creditLimitType;
        #endregion

        #region Property
        public static ObservableCollection<Customer> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<Customer>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<Customer>>("Customer_List").Result;
                        _toList = new ObservableCollection<Customer>(l1);
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

        public string PersonIncharge
        {
            get
            {
                return
                    _PersonIncharge;
            }

            set
            {
                if (_PersonIncharge != value)
                {
                    _PersonIncharge = value;
                    NotifyPropertyChanged(nameof(PersonIncharge));
                }
            }
        }

        public string AddressLine1
        {
            get
            {
                return
                    _AddressLine1;
            }

            set
            {
                if (_AddressLine1 != value)
                {
                    _AddressLine1 = value;
                    NotifyPropertyChanged(nameof(AddressLine1));
                }
            }
        }

        public string AddressLine2
        {
            get
            {
                return _AddressLine2;
            }

            set
            {
                if (_AddressLine2 != value)
                {
                    _AddressLine2 = value;
                    NotifyPropertyChanged(nameof(AddressLine2));
                }
            }
        }
        public int CityId
        {
            get
            {
                return _CityId;
            }
            set
            {
                if (_CityId != value)
                {
                    _CityId = value;
                    NotifyPropertyChanged(nameof(CityId));
                }

            }
        }

        public string TelephoneNo
        {
            get
            {
                return _TelephoneNo;
            }

            set
            {
                if (_TelephoneNo != value)
                {
                    _TelephoneNo = value;
                    NotifyPropertyChanged(nameof(TelephoneNo));
                }
            }
        }

        public string MobileNo
        {
            get
            {
                return _MobileNo;
            }

            set
            {
                if (_MobileNo != value)
                {
                    _MobileNo = value;
                    NotifyPropertyChanged(nameof(MobileNo));
                }
            }
        }

        public string GSTNo
        {
            get
            {
                return _GSTNo;
            }

            set
            {
                if (_GSTNo != value)
                {
                    _GSTNo = value;
                    NotifyPropertyChanged(nameof(GSTNo));
                }
            }
        }

        public string EMailId
        {
            get
            {
                return _EMailId;
            }

            set
            {
                if (_EMailId != value)
                {
                    _EMailId = value;
                    NotifyPropertyChanged(nameof(EMailId));
                }
            }
        }

        public int CreditLimit
        {
            get
            {
                return _CreditLimit;
            }

            set
            {
                if (_CreditLimit != value)
                {
                    _CreditLimit = value;
                    NotifyPropertyChanged(nameof(CreditLimit));
                }
            }
        }

        public int CreditLimitTypeId
        {
            get
            {
                return _CreditLimitTypeId;
            }
            set
            {
                if (_CreditLimitTypeId != value)
                {
                    _CreditLimitTypeId = value;
                    NotifyPropertyChanged(nameof(CreditLimitTypeId));
                }

            }
        }

        public double CreditAmount
        {
            get
            {
                return _CreditAmount;
            }

            set
            {
                if (_CreditAmount != value)
                {
                    _CreditAmount = value;
                    NotifyPropertyChanged(nameof(CreditAmount));
                }
            }
        }

        public int? AccountGroupId
        {
            get
            {
                return _AccountGroupId;
            }
            set
            {
                if (_AccountGroupId != value)
                {
                    _AccountGroupId = value;
                    NotifyPropertyChanged(nameof(AccountGroupId));
                }

            }
        }

        public decimal LedgerOPDr
        {
            get
            {
                return _LedgerOPDr;
            }
            set
            {
                if (_LedgerOPDr != value)
                {
                    _LedgerOPDr = value;
                    NotifyPropertyChanged(nameof(LedgerOPDr));
                }

            }
        }

        public decimal LedgerOPCr
        {
            get
            {
                return _LedgerOPCr;
            }
            set
            {
                if (_LedgerOPCr != value)
                {
                    _LedgerOPCr = value;
                    NotifyPropertyChanged(nameof(LedgerOPCr));
                }

            }
        }

        public int CompanyId
        {
            get
            {
                return _CompanyId;
            }
            set
            {
                if (_CompanyId != value)
                {
                    _CompanyId = value;
                    NotifyPropertyChanged(nameof(CompanyId));
                }

            }
        }

        public decimal? BillingAmount
        {
            get
            {
                return _BillingAmount;
            }
            set
            {
                if (_BillingAmount != value)
                {
                    _BillingAmount = value;
                    NotifyPropertyChanged(nameof(BillingAmount));
                }
            }
        }

        public decimal? CrBillingAmount
        {
            get
            {
                return _CrBillingAmount;
            }
            set
            {
                if (_CrBillingAmount != value)
                {
                    _CrBillingAmount = value;
                    NotifyPropertyChanged(nameof(CrBillingAmount));
                }
            }
        }

        public decimal? PaidAmount
        {
            get
            {
                return _PaidAmount;
            }
            set
            {
                if (_PaidAmount != value)
                {
                    _PaidAmount = value;
                    NotifyPropertyChanged(nameof(PaidAmount));
                }
            }
        }

        public string CityName
        {
            get
            {
                return _cityName;
            }
            set
            {
                if(_cityName!=value)
                {
                    _cityName = value;
                    NotifyPropertyChanged(nameof(CityName));
                }
            }
        }

        public string CreditLimitType
        {
            get
            {
                return _creditLimitType;
            }
            set
            {
                if (_creditLimitType != value)
                {
                    _creditLimitType = value;
                    NotifyPropertyChanged(nameof(CreditLimitType));
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

                Customer d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Customer();
                    toList.Add(d);
                }

                this.toCopy<Customer>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("Customer_Save", this).Result;
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
            new Customer().toCopy<Customer>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Customer>(this);
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
                if (isServerCall == false) FMCGHubClient.FMCGHub.Invoke<int>("Customer_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;

            if (toList.Where(x => x.CustomerName.ToLower() == CustomerName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }


            return RValue;

        }
        #endregion

    }
}
