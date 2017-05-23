using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMCG.Common;

namespace FMCG.BLL
{
    public class UOM : INotifyPropertyChanged
    {
        
        #region Field
        private static ObservableCollection<UOM> _toList;

        private int _id;
        private string _formalName;
        private string _symbol;
        private int _companyId;

        public List<BLL.Validation> lstValidation = new List<BLL.Validation>();

        #endregion

        #region Property

        public static ObservableCollection<UOM> toList
        {
            get
            {
                try
                {
                    if (_toList == null)
                    {
                        _toList = new ObservableCollection<UOM>();
                        var l1 = FMCGHubClient.FMCGHub.Invoke<List<UOM>>("UOM_List").Result;
                        _toList = new ObservableCollection<UOM>(l1);
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
        public string FormalName
        {
            get
            {
                return _formalName;
            }

            set
            {
                if (_formalName != value)
                {
                    _formalName = value;
                    NotifyPropertyChanged(nameof(FormalName));
                }
            }
        }
        public string Symbol
        {
            get
            {
                return _symbol;
            }

            set
            {
                if (_symbol != value)
                {
                    _symbol = value;
                    NotifyPropertyChanged(nameof(Symbol));
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

                UOM d = toList.Where(x => x.Id == Id).FirstOrDefault();

                if (d == null)
                {
                    d = new UOM();
                    toList.Add(d);
                }

                this.toCopy<UOM>(d);
                if (isServerCall == false)
                {

                    var i = FMCGHubClient.FMCGHub.Invoke<int>("UOM_Save", this).Result;
                    d.Id = i;
                }

                return true;
            }
            catch (Exception ex)
            {
                lstValidation.Add(new Validation() { Name = string.Empty, Message = ex.Message });
                return false;

            }

        }

        public void Clear()
        {
            new UOM().toCopy<UOM>(this);
            NotifyAllPropertyChanged();
        }

        public bool Find(int pk)
        {
            var d = toList.Where(x => x.Id == pk).FirstOrDefault();
            if (d != null)
            {
                d.toCopy<UOM>(this);
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
                if (isServerCall == false) FMCGHubClient.FMCGHub.Invoke<int>("UOM_Delete", this.Id);
                return true;
            }

            return false;
        }

        public bool isValid()
        {
            bool RValue = true;
            lstValidation.Clear();

            if (string.IsNullOrWhiteSpace(Symbol))
            {
                lstValidation.Add(new Validation() { Name = nameof(Symbol), Message =  string.Format(Message.PL.Existing_Data,Symbol)});
                RValue = false;
            }
            else if (toList.Where(x => x.Symbol.ToLower() == Symbol.ToLower() && x.Id != Id).Count() > 0)
            {
                lstValidation.Add(new Validation() { Name = nameof(Symbol), Message = string.Format(Message.PL.Existing_Data, Symbol) });
                RValue = false;
            }


            return RValue;

        }

        public bool Find(string Symbol)
        {
            var d = toList.Where(x => x.Symbol == Symbol & x.Id!=Id).FirstOrDefault();
            if(d==null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
