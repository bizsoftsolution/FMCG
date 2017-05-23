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
    public class StockGroup : INotifyPropertyChanged
    {

        #region Field
        private static ObservableCollection<StockGroup> _toList;

        private int _id;
        private string _stockGroupName;
        private string _stockGroupCode;
        private int? _underStockId;
        private int _companyId;
        private string _underStockGroupName;

        #endregion

        #region Property

        public static ObservableCollection<StockGroup> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<StockGroup>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<StockGroup>>("stockGroup_List").Result;
                        _toList = new ObservableCollection<StockGroup>(l1);
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
        public string StockGroupCode
        {
            get
            {
                return _stockGroupCode;
            }

            set
            {
                if (_stockGroupCode != value)
                {
                    _stockGroupCode = value;
                    NotifyPropertyChanged(nameof(StockGroupCode));
                }
            }
        }
        public int? UnderStockId
        {
            get
            {
                return _underStockId;
            }

            set
            {
                if (_underStockId != value)
                {
                    _underStockId = value;
                    NotifyPropertyChanged(nameof(UnderStockId));
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
        public string UnderStockGroupName
        {
            get
            {
                return _underStockGroupName;
            }

            set
            {
                if (_underStockGroupName != value)
                {
                    _underStockGroupName = value;
                    NotifyPropertyChanged(nameof(UnderStockGroupName));
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

                StockGroup d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new StockGroup();
                    toList.Add(d);
                }

                this.toCopy<StockGroup>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("StockGroup_Save", this).Result;
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
            new StockGroup().toCopy<StockGroup>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<StockGroup>(this);
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
                if (isServerCall == false) FMCGHubClient.FMCGHub.Invoke<int>("StockGroup_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            if (toList.Where(x => x.StockGroupName.ToLower() == StockGroupName.ToLower() && x.Id != Id).Count() > 0)
            {
                RValue = false;
            }
            return RValue;

        }
        #endregion
    }
}
