using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace FMCG.BLL
{
    public class UserTypeFormDetail : INotifyPropertyChanged
    {

        #region Field

        private static List<UserTypeFormDetail> _toList;

        private int _id;
        private string _formName;

        #endregion

        #region Property

        public static List<UserTypeFormDetail> ToList
        {
            get
            {
                if (_toList == null) _toList= FMCGHubClient.FMCGHub.Invoke<List<UserTypeFormDetail>>("UserTypeFormDetail_List").Result;
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
                if(_id!=value)
                {
                    _id = value;
                    NotifyPropertyChanged(nameof(Id));
                }
            }
        }

        public string FormName
        {
            get
            {
                return _formName;
            }
            set
            {
                if (_formName != value)
                {
                    _formName = value;
                    NotifyPropertyChanged(nameof(FormName));
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

    }
}