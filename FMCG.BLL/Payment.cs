using FMCG.Common;
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
    public class Payment : INotifyPropertyChanged
    {

        #region fields
        private long _Id;
        private string _RefNo;
        private DateTime? _PaymentDate;
        private decimal? _BalanceAmount;
        private decimal? _Amount;
        private string _Narration;
        private int _CompanyId;
        private string _AmountInwords;

        #region AccountFrom

        private PaymentCash _PCash;
        private PaymentCheque _PCheque;
        private PaymentOnline _POnline;
        private PaymentTT _PTT;

        #endregion

        #region AccountTo

        private PaymentCustomer _PCustomer;
        private PaymentSupplier _PSupplier;
        private PaymentStaff _PStaff;
        private PaymentBank _PBank;
        private PaymentLedger _PLedger;

        #endregion


        private string _PayMode;
        private string _PayTo;

        private static List<string> _PayModeList;
        private static List<string> _ChequeStatusList;
        private static List<string> _PayToList;
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
        public DateTime? PaymentDate
        {
            get
            {
                return _PaymentDate;
            }
            set
            {
                if (_PaymentDate != value)
                {
                    _PaymentDate = value;
                    NotifyPropertyChanged(nameof(PaymentDate));
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
                return PSupplier.SupplierId;
            }
            set
            {
                if (PSupplier.SupplierId != value)
                {
                    PSupplier.SupplierId = value;
                    var l1 = Purchase.PPendingList.Where(x => x.SupplierId== value).ToList();
                    PPendingList = new ObservableCollection<Purchase>(l1);
                    BalanceAmount = PPendingList.Sum(x => x.BalanceAmount ?? 0);
                    NotifyPropertyChanged(nameof(SupplierId));
                }
            }
        }

        #region AccountFrom

        public PaymentCash PCash
        {
            get
            {
                if (_PCash == null) _PCash = new PaymentCash();
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
        public PaymentCheque PCheque
        {
            get
            {
                if (_PCheque == null) _PCheque = new PaymentCheque();
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
        public PaymentOnline POnline
        {
            get
            {
                if (_POnline == null) _POnline = new PaymentOnline();
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
        public PaymentTT PTT
        {
            get
            {
                if (_PTT == null) _PTT = new PaymentTT();
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

        public PaymentCustomer PCustomer
        {
            get
            {
                if (_PCustomer == null) _PCustomer = new PaymentCustomer();
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
        public PaymentSupplier PSupplier
        {
            get
            {
                if (_PSupplier == null) _PSupplier = new PaymentSupplier();
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
        public PaymentStaff PStaff
        {
            get
            {
                if (_PStaff == null) _PStaff = new PaymentStaff();
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
        public PaymentBank PBank
        {
            get
            {
                if (_PBank == null) _PBank = new PaymentBank();
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
        public PaymentLedger PLedger
        {
            get
            {
                if (_PLedger == null) _PLedger = new PaymentLedger();
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

        public  ObservableCollection<SalesReturn> SRPendingList
        {
            get
            {
                if(_SRPendingList== null)
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
                if(_PPendingList != value)
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Payment_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Payment().toCopy<Payment>(this);            
            PaymentDate = DateTime.Now;
            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                Payment po = FMCGHubClient.FMCGHub.Invoke<Payment>("Payment_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<Payment>(this);
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Payment_Delete", this.Id).Result;
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
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_PayRefNo", RefNo, this).Result;
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
            if (PayTo == "Customer")
            {
                Amount = SRPendingList.Sum(x => x.PayAmount ?? 0);
            }
            else if(PayTo == "Supplier")
            {
                Amount = PPendingList.Sum(x => x.PayAmount ?? 0);
            }
        }




        #endregion


    }
}
