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
    public class Bank : INotifyPropertyChanged
    {

        #region Fields
        private static ObservableCollection<Bank> _toList;

        public int _id;
        public string _accountNo;
        public string _accountName;
        public int? _bankAccountTypeId;
        public string _bankName;
        public string _personIncharge;
        public string _addressLine1;
        public string _addressLine2;
        public int? _cityId;
        public string _telephoneNo;
        public string _mobileNo;
        public string _eMailId;
        public int? _accountGroupId;
        public decimal _ledgerOPDr;
        public decimal _ledgerOPCr;
        public int? _companyId;
        public string _accountGroupName;
        private string _cityName;

        #endregion

        #region Property
        public static ObservableCollection<Bank> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<Bank>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<Bank>>("Bank_List").Result;
                        _toList = new ObservableCollection<Bank>(l1);
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
                return _id;
            }

            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }
        public string AccountNo
        {
            get
            {
                return _accountNo;
            }

            set
            {
                if (_accountNo != value)
                {
                    _accountNo = value;
                    NotifyPropertyChanged(nameof(AccountNo));
                }
            }
        }
        public string AccountName
        {
            get
            {
                return _accountName;
            }

            set
            {
                if (_accountName != value)
                {
                    _accountName = value;
                    NotifyPropertyChanged(nameof(AccountName));
                }
            }
        }
        public int? BankAccountTypeId
        {
            get
            {
                return _bankAccountTypeId;
            }

            set
            {
                if (_bankAccountTypeId != value)
                {
                    _bankAccountTypeId = value;
                    NotifyPropertyChanged(nameof(BankAccountTypeId));
                }
            }
        }
        public string BankName
        {
            get
            {
                return _bankName;
            }

            set
            {
                if (_bankName != value)
                {
                    _bankName = value;
                    NotifyPropertyChanged(nameof(BankName));
                }
            }
        }
        public string PersonIncharge
        {
            get
            {
                return _personIncharge;
            }

            set
            {
                if (_personIncharge != value)
                {
                    _personIncharge = value;
                    NotifyPropertyChanged(nameof(PersonIncharge));
                }
            }
        }
        public string AddressLine1
        {
            get
            {
                return _addressLine1;
            }

            set
            {
                if (_addressLine1 != value)
                {
                    _addressLine1 = value;
                    NotifyPropertyChanged(nameof(AddressLine1));
                }
            }
        }
        public string AddressLine2
        {
            get
            {
                return _addressLine2;
            }

            set
            {
                if (_addressLine2 != value)
                {
                    _addressLine2 = value;
                    NotifyPropertyChanged(nameof(AddressLine2));
                }
            }
        }
        public int? CityId
        {
            get
            {
                return _cityId;
            }

            set
            {
                if (_cityId != value)
                {
                    _cityId = value;
                    NotifyPropertyChanged(nameof(CityId));
                }
            }
        }
        public string TelephoneNo
        {
            get
            {
                return _telephoneNo;
            }

            set
            {
                if (_telephoneNo != value)
                {
                    _telephoneNo = value;
                    NotifyPropertyChanged(nameof(TelephoneNo));
                }
            }
        }
        public string MobileNo
        {
            get
            {
                return _mobileNo;
            }

            set
            {
                if (_mobileNo != value)
                {
                    _mobileNo = value;
                    NotifyPropertyChanged(nameof(MobileNo));
                }
            }
        }
        public string EMailId
        {
            get
            {
                return _eMailId;
            }

            set
            {
                if (_eMailId != value)
                {
                    _eMailId = value;
                    NotifyPropertyChanged(nameof(EMailId));
                }
            }
        }
        public int? AccountGroupId
        {
            get
            {
                return _accountGroupId;
            }

            set
            {
                if (_accountGroupId != value)
                {
                    _accountGroupId = value;
                    NotifyPropertyChanged(nameof(AccountGroupId));
                }
            }
        }
        public decimal LedgerOPDr
        {
            get
            {
                return _ledgerOPDr;
            }

            set
            {
                if (_ledgerOPDr != value)
                {
                    _ledgerOPDr = value;
                    NotifyPropertyChanged(nameof(LedgerOPDr));
                }
            }
        }
        public decimal LedgerOPCr
        {
            get
            {
                return _ledgerOPCr;
            }

            set
            {
                if (_ledgerOPCr != value)
                {
                    _ledgerOPCr = value;
                    NotifyPropertyChanged(nameof(LedgerOPCr));
                }
            }
        }
        public int? CompanyId
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
        public string CityName
        {
            get
            {
                return _cityName;
            }
            set
            {
                if (_cityName != value)
                {
                    _cityName = value;
                    NotifyPropertyChanged(nameof(CityName));
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

        public bool Save(bool isServerCall = false)
        {
            if (!isValid()) return false;
            try
            {

                Bank d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Bank();
                    toList.Add(d);
                }

                this.toCopy<Bank>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("Bank_Save", this).Result;
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
            new Bank().toCopy<Bank>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Bank>(this);
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
                if (isServerCall == false) FMCGHubClient.FMCGHub.Invoke<int>("Bank_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;

            if (toList.Where(x => x.BankName.ToLower() == BankName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }


            return RValue;

        }
        #endregion
    }
}
