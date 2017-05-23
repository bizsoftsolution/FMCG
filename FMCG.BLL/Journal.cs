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
    public class Journal : INotifyPropertyChanged
    {

        #region fields
        private long _Id;
        private string _RefNo;
        private DateTime? _JournalDate;
        private decimal? _BalanceAmount;
        private decimal? _Amount;
        private string _Narration;
        private int _CompanyId;
        private string _AmountInwords;

        #region AccountFrom

        private JournalCash _JCash;
        private JournalCheque _JCheque;
        private JournalOnline _JOnline;
        private JournalTT _JTT;

        #endregion

        #region AccountTo

        private JournalCustomer _JCustomer;
        private JournalSupplier _JSupplier;
        private JournalStaff _JStaff;
        private JournalBank _JBank;
        private JournalLedger _JLedger;

        #endregion


        private string _TMode;
        private string _To;

        private static List<string> _TModeList;
        private static List<string> _ChequeStatusList;
        private static List<string> _ToList;
        private ObservableCollection<SalesReturn> _SRPendingList;
        private ObservableCollection<Purchase> _PPendingList;

        private string _SearchText;

        private bool _IsShowChequeDetail;
        private bool _IsShowOnlineDetail;
        private bool _IsShowTTDetail;

        private bool _IsShowCustomerDetail;
        private bool _IsShowSupplierDetail;
        private bool _IsShowStaffDetail;
        private bool _IsShowBankDetail;
        private bool _IsShowLedgerDetail;

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
        public string RefNo
        {
            get
            {
                return _RefNo;
            }
            set
            {
                if (_RefNo != value)
                {
                    _RefNo = value;
                    NotifyPropertyChanged(nameof(RefNo));
                }
            }
        }
        public DateTime? JournalDate
        {
            get
            {
                return _JournalDate;
            }
            set
            {
                if (_JournalDate != value)
                {
                    _JournalDate = value;
                    NotifyPropertyChanged(nameof(JournalDate));
                }
            }
        }

        public decimal? BalanceAmount
        {
            get
            {
                return _BalanceAmount;
            }
            set
            {
                if (_BalanceAmount != value)
                {
                    _BalanceAmount = value;
                    NotifyPropertyChanged(nameof(BalanceAmount));
                }
            }
        }
        public decimal? Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                if (_Amount != value)
                {
                    _Amount = value;
                    NotifyPropertyChanged(nameof(Amount));
                    AmountInwords = value.ToCurrencyInWords();
                }
            }
        }
        public string Narration
        {
            get
            {
                return _Narration;
            }
            set
            {
                if (_Narration != value)
                {
                    _Narration = value;
                    NotifyPropertyChanged(nameof(Narration));
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

        public string AmountInwords
        {
            get
            {
                if (_AmountInwords == null) _AmountInwords = "";
                return _AmountInwords;
            }
            set
            {
                if (_AmountInwords != value)
                {
                    _AmountInwords = value;
                    NotifyPropertyChanged(nameof(AmountInwords));
                }
            }
        }

        public int CustomerId
        {
            get
            {
                return JCustomer.CustomerId;
            }
            set
            {
                if (JCustomer.CustomerId != value)
                {
                    JCustomer.CustomerId = value;
                    var l1 = SalesReturn.SRPendingList.Where(x => x.CustomerId == value).ToList();
                    SRPendingList = new ObservableCollection<SalesReturn>(l1);
                    BalanceAmount = SRPendingList.Sum(x => x.BalanceAmount ?? 0);
                    NotifyPropertyChanged(nameof(CustomerId));
                }
            }
        }

        public int SupplierId
        {
            get
            {
                return JSupplier.SupplierId;
            }
            set
            {
                if (JSupplier.SupplierId != value)
                {
                    JSupplier.SupplierId = value;
                    var l1 = Purchase.PPendingList.Where(x => x.SupplierId == value).ToList();
                    PPendingList = new ObservableCollection<Purchase>(l1);
                    BalanceAmount = PPendingList.Sum(x => x.BalanceAmount ?? 0);
                    NotifyPropertyChanged(nameof(SupplierId));
                }
            }
        }

        #region AccountFrom

        public JournalCash JCash
        {
            get
            {
                if (_JCash == null) _JCash = new JournalCash();
                return _JCash;
            }
            set
            {
                if (_JCash != value)
                {
                    _JCash = value;
                    NotifyPropertyChanged(nameof(JCash));
                }
            }
        }
        public JournalCheque JCheque
        {
            get
            {
                if (_JCheque == null) _JCheque = new JournalCheque();
                return _JCheque;
            }
            set
            {
                if (_JCheque != value)
                {
                    _JCheque = value;
                    NotifyPropertyChanged(nameof(JCheque));
                }
            }
        }
        public JournalOnline JOnline
        {
            get
            {
                if (_JOnline == null) _JOnline = new JournalOnline();
                return _JOnline;
            }
            set
            {
                if (_JOnline != value)
                {
                    _JOnline = value;
                    NotifyPropertyChanged(nameof(JOnline));
                }
            }
        }
        public JournalTT JTT
        {
            get
            {
                if (_JTT == null) _JTT = new JournalTT();
                return _JTT;
            }
            set
            {
                if (_JTT != value)
                {
                    _JTT = value;
                    NotifyPropertyChanged(nameof(JTT));
                }
            }
        }

        #endregion

        #region AccountTo

        public JournalCustomer JCustomer
        {
            get
            {
                if (_JCustomer == null) _JCustomer = new JournalCustomer();
                return _JCustomer;
            }
            set
            {
                if (_JCustomer != value)
                {
                    _JCustomer = value;
                    NotifyPropertyChanged(nameof(JCustomer));
                }
            }
        }
        public JournalSupplier JSupplier
        {
            get
            {
                if (_JSupplier == null) _JSupplier = new JournalSupplier();
                return _JSupplier;
            }
            set
            {
                if (_JSupplier != value)
                {
                    _JSupplier = value;
                    NotifyPropertyChanged(nameof(JSupplier));
                }
            }
        }
        public JournalStaff JStaff
        {
            get
            {
                if (_JStaff == null) _JStaff = new JournalStaff();
                return _JStaff;
            }
            set
            {
                if (_JStaff != value)
                {
                    _JStaff = value;
                    NotifyPropertyChanged(nameof(JStaff));
                }
            }
        }
        public JournalBank JBank
        {
            get
            {
                if (_JBank == null) _JBank = new JournalBank();
                return _JBank;
            }
            set
            {
                if (_JBank != value)
                {
                    _JBank = value;
                    NotifyPropertyChanged(nameof(JBank));
                }
            }
        }
        public JournalLedger JLedger
        {
            get
            {
                if (_JLedger == null) _JLedger = new JournalLedger();
                return _JLedger;
            }
            set
            {
                if (_JLedger != value)
                {
                    _JLedger = value;
                    NotifyPropertyChanged(nameof(JLedger));
                }
            }
        }

        #endregion



        public string TMode
        {
            get
            {
                return _TMode;
            }
            set
            {
                if (_TMode != value)
                {
                    _TMode = value;
                    IsShowChequeDetail = value == "Cheque";
                    IsShowOnlineDetail = value == "Online";
                    IsShowTTDetail = value == "TT";
                    NotifyPropertyChanged(nameof(TMode));
                }
            }
        }

        public string To
        {
            get
            {
                return _To;
            }
            set
            {
                if (_To != value)
                {
                    _To = value;
                    IsShowCustomerDetail = value == "Customer";
                    IsShowSupplierDetail = value == "Supplier";
                    IsShowStaffDetail = value == "Staff";
                    IsShowBankDetail = value == "Bank";
                    IsShowLedgerDetail = value == "Ledger";
                    NotifyPropertyChanged(nameof(To));
                }
            }
        }

        public static List<string> TModeList
        {
            get
            {
                if (_TModeList == null)
                {
                    _TModeList = new List<string>();
                    _TModeList.Add("Cash");
                    _TModeList.Add("Cheque");
                    _TModeList.Add("Online");
                    _TModeList.Add("TT");
                }
                return _TModeList;
            }
            set
            {
                if (_TModeList != value)
                {
                    _TModeList = value;
                }
            }
        }

        public static List<string> ToList
        {
            get
            {
                if (_ToList == null)
                {
                    _ToList = new List<string>();
                    _ToList.Add("Customer");
                    _ToList.Add("Supplier");
                    _ToList.Add("Staff");
                    _ToList.Add("Bank");
                    _ToList.Add("Ledger");
                }
                return _ToList;
            }
            set
            {
                if (_ToList != value)
                {
                    _ToList = value;
                }
            }
        }

        public static List<string> ChequeStatusList
        {
            get
            {
                if (_ChequeStatusList == null)
                {
                    _ChequeStatusList = new List<string>();
                    _ChequeStatusList.Add("Issued");
                    _ChequeStatusList.Add("Completed");
                    _ChequeStatusList.Add("Returned");
                }
                return _ChequeStatusList;
            }
            set
            {
                _ChequeStatusList = value;
            }
        }

        public static ObservableCollection<Bank> BankList
        {
            get
            {
                return Bank.toList;
            }
        }
        public static ObservableCollection<Customer> CustomerList
        {
            get
            {
                return Customer.toList;
            }
        }
        public static ObservableCollection<Supplier> SupplierList
        {
            get
            {
                return Supplier.toList;
            }
        }
        public static ObservableCollection<Staff> StaffList
        {
            get
            {
                return Staff.toList;
            }
        }
        public static ObservableCollection<Ledger> LedgerList
        {
            get
            {
                return Ledger.toList;
            }
        }

        public ObservableCollection<SalesReturn> SRPendingList
        {
            get
            {
                if (_SRPendingList == null)
                {
                    _SRPendingList = new ObservableCollection<SalesReturn>();
                }
                return _SRPendingList;
            }
            set
            {
                if (_SRPendingList != value)
                {
                    _SRPendingList = value;
                    NotifyPropertyChanged(nameof(SRPendingList));
                }
            }
        }

        public ObservableCollection<Purchase> PPendingList
        {
            get
            {
                if (_PPendingList == null)
                {
                    _PPendingList = new ObservableCollection<Purchase>();
                }
                return _PPendingList;
            }
            set
            {
                if (_PPendingList != value)
                {
                    _PPendingList = value;
                    NotifyPropertyChanged(nameof(PPendingList));
                }
            }
        }



        public string SearchText
        {
            get
            {
                return _SearchText;
            }
            set
            {
                if (_SearchText != value)
                {
                    _SearchText = value;
                    NotifyPropertyChanged(nameof(SearchText));
                }
            }
        }

        public bool IsShowChequeDetail
        {
            get
            {
                return _IsShowChequeDetail;
            }
            set
            {
                if (_IsShowChequeDetail != value)
                {
                    _IsShowChequeDetail = value;
                    NotifyPropertyChanged(nameof(IsShowChequeDetail));
                }
            }
        }

        public bool IsShowOnlineDetail
        {
            get
            {
                return _IsShowOnlineDetail;
            }
            set
            {
                if (_IsShowOnlineDetail != value)
                {
                    _IsShowOnlineDetail = value;
                    NotifyPropertyChanged(nameof(IsShowOnlineDetail));
                }
            }
        }

        public bool IsShowTTDetail
        {
            get
            {
                return _IsShowTTDetail;
            }
            set
            {
                if (_IsShowTTDetail != value)
                {
                    _IsShowTTDetail = value;
                    NotifyPropertyChanged(nameof(IsShowTTDetail));
                }
            }
        }

        public bool IsShowCustomerDetail
        {
            get
            {
                return _IsShowCustomerDetail;
            }
            set
            {
                if (_IsShowCustomerDetail != value)
                {
                    _IsShowCustomerDetail = value;
                    NotifyPropertyChanged(nameof(IsShowCustomerDetail));
                }
            }
        }
        public bool IsShowSupplierDetail
        {
            get
            {
                return _IsShowSupplierDetail;
            }
            set
            {
                if (_IsShowSupplierDetail != value)
                {
                    _IsShowSupplierDetail = value;
                    NotifyPropertyChanged(nameof(IsShowSupplierDetail));
                }
            }
        }
        public bool IsShowStaffDetail
        {
            get
            {
                return _IsShowStaffDetail;
            }
            set
            {
                if (_IsShowStaffDetail != value)
                {
                    _IsShowStaffDetail = value;
                    NotifyPropertyChanged(nameof(IsShowStaffDetail));
                }
            }
        }
        public bool IsShowBankDetail
        {
            get
            {
                return _IsShowBankDetail;
            }
            set
            {
                if (_IsShowBankDetail != value)
                {
                    _IsShowBankDetail = value;
                    NotifyPropertyChanged(nameof(IsShowBankDetail));
                }
            }
        }
        public bool IsShowLedgerDetail
        {
            get
            {
                return _IsShowLedgerDetail;
            }
            set
            {
                if (_IsShowLedgerDetail != value)
                {
                    _IsShowLedgerDetail = value;
                    NotifyPropertyChanged(nameof(IsShowLedgerDetail));
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

        #region Methods

        #region Master
        public bool Save()
        {
            try
            {
                return FMCGHubClient.FMCGHub.Invoke<bool>("Journal_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Journal().toCopy<Journal>(this);
            JournalDate = DateTime.Now;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                Journal po = FMCGHubClient.FMCGHub.Invoke<Journal>("Journal_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<Journal>(this);
                NotifyAllPropertyChanged();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete()
        {
            try
            {
                return FMCGHubClient.FMCGHub.Invoke<bool>("Journal_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_JournalRefNo", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                rv = true;
            }
            return rv;
        }

        #endregion

        #region Detail

        #endregion


        public void SetTotalAmount()
        {
            if (To == "Customer")
            {
                Amount = SRPendingList.Sum(x => x.PayAmount ?? 0);
            }
            else if (To == "Supplier")
            {
                Amount = PPendingList.Sum(x => x.PayAmount ?? 0);
            }
        }




        #endregion


    }
}
