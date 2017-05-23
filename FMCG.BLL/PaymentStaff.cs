using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace FMCG.BLL
{
    public class PaymentStaff:INotifyPropertyChanged
    {

        #region fields

        private long _Id;
        private long _PaymentId;
        private int _StaffId;
        private string _StaffName;

        #endregion

        #region Property

        public long Id
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

        public long PaymentId
        {
            get
            {
                return _PaymentId;
            }
            set
            {
                if (_PaymentId != value)
                {
                    _PaymentId = value;
                    NotifyPropertyChanged(nameof(PaymentId));
                }
            }
        }

        public int StaffId
        {
            get
            {
                return _StaffId;
            }
            set
            {
                if (_StaffId != value)
                {
                    _StaffId = value;
                    NotifyPropertyChanged(nameof(StaffId));
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

    }
}
