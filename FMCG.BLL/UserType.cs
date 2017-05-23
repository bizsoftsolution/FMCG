using FMCG.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
    public class UserType : INotifyPropertyChanged
    {
        
        #region Field

        private static List<UserType> _toList;

        private int _id;
        private string _typeOfUser;
        private string _description;
        #endregion

        #region Property

        public static List<UserType> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = FMCGHubClient.FMCGHub.Invoke<List<UserType>>("UserType_List").Result;
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
        public string TypeOfUser
        {
            get
            {
                return _typeOfUser;
            }
            set
            {
                if (_typeOfUser != value)
                {
                    _typeOfUser = value;
                    NotifyPropertyChanged(nameof(TypeOfUser));
                }
            }
        }
        public string Description
        {
            get
            {
                return _description;
            }
            set
            {
                if (_description != value)
                {
                    _description = value;
                    NotifyPropertyChanged(nameof(Description));
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

        private void NotifyAllPropertyChanged()
        {
            foreach (var p in this.GetType().GetProperties()) NotifyPropertyChanged(p.Name);
        }

        #endregion

        #region Methods

        public bool Save(bool isServerCall = false)
        {
            try
            {

                UserType d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new UserType();
                    toList.Add(d);
                }

                this.toCopy<UserType>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("userType_Save", this).Result;
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
            new UserType().toCopy<UserType>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<UserType>(this);
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
                if (isServerCall == false) FMCGHubClient.FMCGHub.Invoke<int>("userType_Delete", this.Id);
                return true;
            }

            return false;
        }

        #endregion
    }
}
