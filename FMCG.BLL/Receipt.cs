using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FMCG.Common;
using System.Collections.ObjectModel;

namespace FMCG.BLL
{

    public class Receipt:INotifyPropertyChanged
    {

        #region fields
        private long _Id;
        private string _RefNo;
        private DateTime? _ReceiptDate;
        private decimal? _BalanceAmount;
        private decimal? _Amount;
        private string _Narration;
        private int _CompanyId;
        private string _AmountInwords;

        #region AccountFrom

        private ReceiptCash _PCash;
        private ReceiptCheque _PCheque;
        private ReceiptOnline _POnline;
        private ReceiptTT _PTT;

        #endregion

        #region AccountTo

        private ReceiptCustomer _PCustomer;
        private ReceiptSupplier _PSupplier;
        private ReceiptStaff _PStaff;
        private ReceiptBank _PBank;
        private ReceiptLedger _PLedger;

        #endregion


        private string _PayMode;
        private string _PayTo;

        private static List<string> _PayModeList;
        private static List<string> _ChequeStatusList;
        private static List<string> _PayToList;
        private ObservableCollection<Sale> _SPendingList;
        private ObservableCollection<PurchaseReturn> _PRPendingList;

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
        public DateTime? ReceiptDate
        {
            get
            {
                return _ReceiptDate;
            }
            set
            {
                if (_ReceiptDate != value)
                {
                    _ReceiptDate = value;
                    NotifyPropertyChanged(nameof(ReceiptDate));
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
                return PCustomer.CustomerId;
            }
            set
            {
                if (PCustomer.CustomerId != value)
                {
                    PCustomer.CustomerId = value;
                    var l1 = Sale.SPendingList.Where(x => x.CustomerId == value).ToList();
                    SPendingList = new ObservableCollection<Sale>(l1);
                    BalanceAmount = SPendingList.Sum(x => x.BalanceAmount ?? 0);
                    NotifyPropertyChanged(nameof(CustomerId));
                }
            }
        }

        public int SupplierId
        {
            get
            {
                return PSupplier.SupplierId;
            }
            set
            {
                if (PSupplier.SupplierId != value)
                {
                    PSupplier.SupplierId = value;
                    var l1 = PurchaseReturn.PRPendingList.Where(x => x.SupplierId == value).ToList();
                    PRPendingList = new ObservableCollection<PurchaseReturn>(l1);
                    BalanceAmount = PRPendingList.Sum(x => x.BalanceAmount ?? 0);
                    NotifyPropertyChanged(nameof(SupplierId));
                }
            }
        }

        #region AccountFrom

        public ReceiptCash PCash
        {
            get
            {
                if (_PCash == null) _PCash = new ReceiptCash();
                return _PCash;
            }
            set
            {
                if (_PCash != value)
                {
                    _PCash = value;
                    NotifyPropertyChanged(nameof(PCash));
                }
            }
        }
        public ReceiptCheque PCheque
        {
            get
            {
                if (_PCheque == null) _PCheque = new ReceiptCheque();
                return _PCheque;
            }
            set
            {
                if (_PCheque != value)
                {
                    _PCheque = value;
                    NotifyPropertyChanged(nameof(PCheque));
                }
            }
        }
        public ReceiptOnline POnline
        {
            get
            {
                if (_POnline == null) _POnline = new ReceiptOnline();
                return _POnline;
            }
            set
            {
                if (_POnline != value)
                {
                    _POnline = value;
                    NotifyPropertyChanged(nameof(POnline));
                }
            }
        }
        public ReceiptTT PTT
        {
            get
            {
                if (_PTT == null) _PTT = new ReceiptTT();
                return _PTT;
            }
            set
            {
                if (_PTT != value)
                {
                    _PTT = value;
                    NotifyPropertyChanged(nameof(PTT));
                }
            }
        }

        #endregion

        #region AccountTo

        public ReceiptCustomer PCustomer
        {
            get
            {
                if (_PCustomer == null) _PCustomer = new ReceiptCustomer();
                return _PCustomer;
            }
            set
            {
                if (_PCustomer != value)
                {
                    _PCustomer = value;
                    NotifyPropertyChanged(nameof(PCustomer));
                }
            }
        }
        public ReceiptSupplier PSupplier
        {
            get
            {
                if (_PSupplier == null) _PSupplier = new ReceiptSupplier();
                return _PSupplier;
            }
            set
            {
                if (_PSupplier != value)
                {
                    _PSupplier = value;
                    NotifyPropertyChanged(nameof(PSupplier));
                }
            }
        }
        public ReceiptStaff PStaff
        {
            get
            {
                if (_PStaff == null) _PStaff = new ReceiptStaff();
                return _PStaff;
            }
            set
            {
                if (_PStaff != value)
                {
                    _PStaff = value;
                    NotifyPropertyChanged(nameof(PStaff));
                }
            }
        }
        public ReceiptBank PBank
        {
            get
            {
                if (_PBank == null) _PBank = new ReceiptBank();
                return _PBank;
            }
            set
            {
                if (_PBank != value)
                {
                    _PBank = value;
                    NotifyPropertyChanged(nameof(PBank));
                }
            }
        }
        public ReceiptLedger PLedger
        {
            get
            {
                if (_PLedger == null) _PLedger = new ReceiptLedger();
                return _PLedger;
            }
            set
            {
                if (_PLedger != value)
                {
                    _PLedger = value;
                    NotifyPropertyChanged(nameof(PLedger));
                }
            }
        }

        #endregion



        public string PayMode
        {
            get
            {
                return _PayMode;
            }
            set
            {
                if (_PayMode != value)
                {
                    _PayMode = value;
                    IsShowChequeDetail = value == "Cheque";
                    IsShowOnlineDetail = value == "Online";
                    IsShowTTDetail = value == "TT";
                    NotifyPropertyChanged(nameof(PayMode));
                }
            }
        }

        public string PayTo
        {
            get
            {
                return _PayTo;
            }
            set
            {
                if (_PayTo != value)
                {
                    _PayTo = value;
                    IsShowCustomerDetail = value == "Customer";
                    IsShowSupplierDetail = value == "Supplier";
                    IsShowStaffDetail = value == "Staff";
                    IsShowBankDetail = value == "Bank";
                    IsShowLedgerDetail = value == "Ledger";
                    NotifyPropertyChanged(nameof(PayTo));
                }
            }
        }

        public static List<string> PayModeList
        {
            get
            {
                if (_PayModeList == null)
                {
                    _PayModeList = new List<string>();
                    _PayModeList.Add("Cash");
                    _PayModeList.Add("Cheque");
                    _PayModeList.Add("Online");
                    _PayModeList.Add("TT");
                }
                return _PayModeList;
            }
            set
            {
                if (_PayModeList != value)
                {
                    _PayModeList = value;
                }
            }
        }

        public static List<string> PayToList
        {
            get
            {
                if (_PayToList == null)
                {
                    _PayToList = new List<string>();
                    _PayToList.Add("Customer");
                    _PayToList.Add("Supplier");
                    _PayToList.Add("Staff");
                    _PayToList.Add("Bank");
                    _PayToList.Add("Ledger");
                }
                return _PayToList;
            }
            set
            {
                if (_PayToList != value)
                {
                    _PayToList = value;
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

        public ObservableCollection<Sale> SPendingList
        {
            get
            {
                if (_SPendingList == null)
                {
                    _SPendingList = new ObservableCollection<Sale>();
                }
                return _SPendingList;
            }
            set
            {
                if (_SPendingList != value)
                {
                    _SPendingList = value;
                    NotifyPropertyChanged(nameof(SPendingList));
                }
            }
        }

        public ObservableCollection<PurchaseReturn> PRPendingList
        {
            get
            {
                if (_PRPendingList == null)
                {
                    _PRPendingList = new ObservableCollection<PurchaseReturn>();
                }
                return _PRPendingList;
            }
            set
            {
                if (_PRPendingList != value)
                {
                    _PRPendingList = value;
                    NotifyPropertyChanged(nameof(PRPendingList));
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Receipt_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Receipt().toCopy<Receipt>(this);
            ReceiptDate = DateTime.Now;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                Receipt po = FMCGHubClient.FMCGHub.Invoke<Receipt>("Receipt_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<Receipt>(this);
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Receipt_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Detail

        #endregion


        public void SetTotalAmount()
        {
            if (PayTo == "Customer")
            {
                Amount = SPendingList.Sum(x => x.PayAmount ?? 0);
            }
            else if (PayTo == "Supplier")
            {
                Amount = PRPendingList.Sum(x => x.PayAmount ?? 0);
            }
        }

        #endregion

    }

}
