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
   public class Designation:INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<Designation> _toList;

        private int _id;
        private string _DesignationName;     
        private int _UserTypeId;
        private string _UserTypeName;
        #endregion

        #region Property

        public static ObservableCollection<Designation> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<Designation>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<Designation>>("designation_List").Result;
                        _toList = new ObservableCollection<Designation>(l1);
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
        public int UserTypeId
        {
            get
            {
                return _UserTypeId;
            }

            set
            {
                if (_UserTypeId != value)
                {
                    _UserTypeId = value;
                    NotifyPropertyChanged(nameof(UserTypeId));
                }
            }
        }

        public string UserTypeName
        {
            get
            {
                return _UserTypeName;
            }

            set
            {
                if (_UserTypeName != value)
                {
                    _UserTypeName = value;
                    NotifyPropertyChanged(nameof(UserTypeName));
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

                Designation d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new Designation();
                    toList.Add(d);
                }

                this.toCopy<Designation>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("designation_Save", this).Result;
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
            new Designation().toCopy<Designation>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<Designation>(this);
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
                if (isServerCall == false) FMCGHubClient.FMCGHub.Invoke<int>("designation_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;

            if (toList.Where(x => x.DesignationName.ToLower() == DesignationName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }


            return RValue;

        }

        #endregion
    }
}
