using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
    public class UserAccount : INotifyPropertyChanged
    {
        #region Field

        public static UserAccount User = new UserAccount();
        public static CompanyDetail Company = new CompanyDetail();
        public static UserType Type = new UserType();
        public static List<UserTypeDetail> TypeDetails = new List<UserTypeDetail>();

        private static List<UserAccount> _toList;

        private int _id;
        private int _companyId;
        private int _userTypeId;
        private string _userName;
        private string _loginId;
        private string _password;

        #endregion

        #region Property
        
        public static List<UserAccount> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = FMCGHubClient.FMCGHub.Invoke<List<UserAccount>>("UserAccount_List").Result;
                }

                return _toList;
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
        public int UserTypeId
        {
            get
            {
                return _userTypeId;
            }
            set
            {
                if (_userTypeId != value)
                {
                    _userTypeId = value;
                    NotifyPropertyChanged(nameof(UserTypeId));
                }
            }
        }
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName != value)
                {
                    _userName = value;
                    NotifyPropertyChanged(nameof(UserName));
                }
            }
        }
        public string LoginId
        {
            get
            {
                return _loginId;
            }
            set
            {
                if (_loginId != value)
                {
                    _loginId = value;
                    NotifyPropertyChanged(nameof(LoginId));
                }
            }
        }
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (_password != value)
                {
                    _password = value;
                    NotifyPropertyChanged(nameof(Password));
                }
            }
        }

        #endregion

        #region Property Notify Changed

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string PropertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }


        #endregion

        #region Method
        public static bool Login(string AccYear, String CompanyName, String LoginId, String Password)
        {
            var ua = FMCGHubClient.FMCGHub.Invoke<UserAccount>("UserAccount_Login",AccYear,  CompanyName, LoginId, Password).Result;            
            if (ua.Id != 0)
            {
                User = ua;
                Company = CompanyDetail.toList.Where(x => x.Id == ua.CompanyId).FirstOrDefault();
                Type = UserType.toList.Where(x => x.Id == ua.UserTypeId).FirstOrDefault();
                TypeDetails = UserTypeDetail.ToList.Where(x => x.UserTypeId == ua.UserTypeId).ToList();                
            }
            return ua.Id != 0;
        }

        public static bool AllowFormShow(string FormName)
        {
            bool rv = true;
            var t  = TypeDetails.Where(x => x.FormName == FormName).FirstOrDefault();
            if (t!= null)   rv = t.IsViewForm;            
            return rv;
        }

        public static bool AllowInsert(string FormName)
        {
            bool rv = true;
            var t = TypeDetails.Where(x => x.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowInsert;
            return rv;
        }

        public static bool AllowUpdate(string FormName)
        {
            bool rv = true;
            var t = TypeDetails.Where(x => x.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowUpdate;
            return rv;
        }

        public static bool AllowDelete(string FormName)
        {
            bool rv = true;
            var t = TypeDetails.Where(x => x.FormName == FormName).FirstOrDefault();
            if (t != null) rv = t.AllowDelete;
            return rv;
        }


        #endregion
    }
}
