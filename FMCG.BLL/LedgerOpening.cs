using FMCG.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
    public class LedgerOpening : INotifyPropertyChanged
    {
        #region Fields
        private static ObservableCollection<LedgerOpening> _toList;


        private int _Id;
        private int  _EntityId;
        private string _EntityType;
        private string _AcYear;
        private decimal? _CrAmt;
        private decimal? _DrAmt;
        private int _CompanyId;

        private string _LedgerName;
        private string _EntityName;

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

        #endregion

        #region Property
        public static ObservableCollection<LedgerOpening> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<LedgerOpening>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke <List<LedgerOpening>>("LedgerOpening_List").Result;
                        _toList = new ObservableCollection<LedgerOpening>(l1);
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
        public int EntityId
        {
            get
            {
                return _EntityId;
            }

            set
            {
                if (_EntityId != value)
                {
                    _EntityId = value;
                    NotifyPropertyChanged(nameof(EntityId));
                }
            }
        }

        public string EntityType
        {
            get
            {
                return _EntityType;
                    
            }

            set
            {
                if (_EntityType != value)
                {
                    _EntityType = value;
                    NotifyPropertyChanged(nameof(EntityType));
                }
            }
        }

        public string AcYear
        {
            get
            {
                return _AcYear;
            }

            set
            {
                if (_AcYear != value)
                {
                    _AcYear = value;
                    NotifyPropertyChanged(nameof(AcYear));
                }
            }
        }

        public decimal? CrAmt
        {
            get
            {
                return _CrAmt;
            }

            set
            {
                if (_CrAmt != value)
                {
                    _CrAmt = value;
                    NotifyPropertyChanged(nameof(CrAmt));
                }
            }
        }
        public decimal? DrAmt
        {
            get
            {
                return _DrAmt;
            }
            set
            {
                if (_DrAmt != value)
                {
                    _DrAmt = value;
                    NotifyPropertyChanged(nameof(DrAmt));
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

        public string EntityName
        {
            get
            {
                return _EntityName;
            }

            set
            {
                if (_EntityName != value)
                {
                    _EntityName = value;
                    NotifyPropertyChanged(nameof(EntityName));
                }
            }
        }

       public string LedgerName
        {
            get
            {
                return _LedgerName;
            }
            set
            {
                if(_LedgerName!=value)
                {
                    _LedgerName = value;
                    NotifyPropertyChanged(nameof(LedgerName));
                }
            }
        }
          #endregion

    
        #region Methods

        public bool Save(bool isServerCall = false)
        {
           
            try
            {

                LedgerOpening d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new LedgerOpening();
                    toList.Add(d);
                }

                this.toCopy<LedgerOpening>(d);
                if (isServerCall == false)
                {
                    var i = FMCGHubClient.FMCGHub.Invoke<int>("LedgerOpening_Save", this).Result;
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
            new LedgerOpening().toCopy<LedgerOpening>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<LedgerOpening>(this);
                return true;
            }

            return false;
        }

 
        #endregion

    }
}
