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
    public class Staff : INotifyPropertyChanged
    {
        #region Fields
        private static ObservableCollection<Staff> _toList;

        private int _Id;
        private string _StaffName;
        private string _NRICNo;
        private string _AddressLine1;
        private string _AddressLine2;
        private int? _CityId;
        private string _TelephoneNo;
        private string _MobileNo;
        private string _EMailId;
        private int? _GenderId;
        private DateTime? _DOB;
        private DateTime? _DOJ;
        private decimal? _Salary;
        private int? _DesignationId;
        private int? _UserAccountId;
        private int _CompanyId;
        private string _DesignationName;
        private string _LoginId;
        private string _Password;

        #endregion

        #region Property
        public static ObservableCollection<Staff> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<Staff>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<Staff>>("Staff_List").Result;
                        _toList = new ObservableCollection<Staff>(l1);
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
        public string StaffName
        {
            get
            {
                return _StaffName;
            }

            set
            {
                if (_StaffName != value)
                {
                    _StaffName = value;
                    NotifyPropertyChanged(nameof(StaffName));
                }
            }
        }

        public string NRICNo
        {
            get
            {
                return _NRICNo;

            }

            set
            {
                if (_NRICNo != value)
                {
                    _NRICNo = value;
                    NotifyPropertyChanged(nameof(NRICNo));
                }
            }
        }

        public string AddressLine1
        {
            get
            {
                return _AddressLine1;
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
        public int? CityId
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

        public int? GenderId
        {
            get
            {
                return _GenderId;
            }
            set
            {
                if (_GenderId != value)
                {
                    _GenderId = value;
                    NotifyPropertyChanged(nameof(GenderId));
                }
            }
        }

        public DateTime? DOB
        {
            get
            {
                return _DOB;
            }
            set
            {
                if (_DOB != value)
                {
                    _DOB = value;
                    NotifyPropertyChanged(nameof(DOB));
                }
            }
        }

        public DateTime? DOJ
        {
            get
            {
                return _DOJ;
            }
            set
            {
                if (_DOJ != value)
                {
                    _DOJ = value;
                    NotifyPropertyChanged(nameof(DOJ));
                }
            }
        }

        public decimal? Salary
        {
            get
            {
                return _Salary;
            }
            set
            {
                if (_Salary != value)
                {
                    _Salary = value;
                    NotifyPropertyChanged(nameof(Salary));
                }
            }
        }

        public int? DesignationId
        {
            get
            {
                return _DesignationId;
            }
            set
            {
                if (_DesignationId != value)
                {
                    _DesignationId = value;
                    NotifyPropertyChanged(nameof(DesignationId));
                }
            }
        }

        public int? UserAccountId
        {
            get
            {
                return _UserAccountId;
            }
            set
            {
                if (_UserAccountId != value)
                {
                    _UserAccountId = value;
                    NotifyPropertyChanged(nameof(UserAccountId));
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

        public string DesignationName
        {
            get
            {
                return _DesignationName;
            }
            set
            {
                if (_DesignationName != value)
                {
                    _DesignationName = value;
                    NotifyPropertyChanged(nameof(DesignationName));
                }
            }
        }


        public String LoginId
        {
            get
            {
                return _LoginId;
            }
            set
            {
                if (_LoginId != value)
                {
                    _LoginId = value;
                    NotifyPropertyChanged(nameof(LoginId));
                }
            }
        }
        public String Password
        {
            get
            {
                return _Password;
            }
            set
            {
                if (_Password != value)
                {
                    _Password = value;
                    NotifyPropertyChanged(nameof(Password));
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

                Staff d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Staff();
                    toList.Add(d);
                }

                this.toCopy<Staff>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("Staff_Save", this).Result;
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
            new Staff().toCopy<Staff>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Staff>(this);
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
                if (isServerCall == false) FMCGHubClient.FMCGHub.Invoke<int>("Staff_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            if (toList.Where(x => x.StaffName.ToLower() == StaffName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }
            return RValue;

        }


        #endregion
    }
}
