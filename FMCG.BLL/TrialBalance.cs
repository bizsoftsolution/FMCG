using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
    public class TrialBalance: INotifyPropertyChanged
    {
        private static List<TrialBalance> _toList;
        private static List<TrialBalance> _toPRList;
        private static List<TrialBalance> _PLList;

        public static List<TrialBalance> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = FMCGHubClient.FMCGHub.Invoke<List<TrialBalance>>("TrialBalance_List").Result;
                }

                return _toList;
            }
        }

        public static List<TrialBalance> PRList
        {
            get
            {
                if (_toPRList == null)
                {
                    _toPRList = FMCGHubClient.FMCGHub.Invoke<List<TrialBalance>>("PRList").Result;
                }

                return _toPRList;
            }
        }

        public static List<TrialBalance> PLList
        {
            get
            {
                if (_PLList == null)
                {
                    _PLList = FMCGHubClient.FMCGHub.Invoke<List<TrialBalance>>("PL_List").Result;
                }

                return _PLList;
            }
        }


        #region Fields

        private string _LedgerName;
        private string _GroupName;
        private decimal? _CrAmt;
        private decimal? _DrAmt;
        #endregion

        public string LedgerName
        {
            get
            {
                return _LedgerName;
            }
            set
            {
                if (_LedgerName != value)
                {
                    _LedgerName = value;
                    NotifyPropertyChanged(nameof(LedgerName));
                }
            }
        }
        public string GroupName {
            get
            {
                return _GroupName;
            }
            set
            {
                if (_GroupName != value)
                {
                    _GroupName = value;
                    NotifyPropertyChanged(nameof(GroupName));
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


    }
}
