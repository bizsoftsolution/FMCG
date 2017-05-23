using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMCG.BLL
{
    public class LedgerReport
    {

        #region Fields
        private static List<LedgerReport> _toList;

        //private string  _LedgerName;
        //private string _VoucherType;
        //private DateTime? _Ldate;
        //private string _Particulars;
        //private string _Narration;
        //private decimal? _CrAmt;
        //private decimal? _DrAmt;

        #endregion

        #region Property
        public static List<LedgerReport> toList
        {
            get
            {
                if (_toList == null)
                {
                    _toList = FMCGHubClient.FMCGHub.Invoke<List<LedgerReport>>("ledgerReport_List").Result;
                }

                return _toList;
            }
        }


        public string LedgerName { get; set; }
        public string VoucherType { get; set; }
        public DateTime? LDate { get; set; }
        public string Particulars { get; set; }
        public string Narration { get; set; }
        public decimal? CrAmt { get; set; }
        public decimal? DrAmt { get; set; }
        public string AccountGroup { get; set; }
       
        #endregion



    }
}
