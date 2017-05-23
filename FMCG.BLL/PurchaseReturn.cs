﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Collections.ObjectModel;
using FMCG.Common;

namespace FMCG.BLL
{
    public class PurchaseReturn:INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<PurchaseReturn> _PRPendingList;

        private long _Id;
        private DateTime? _PRDate;
        private string _RefNo;
        private string _InvoiceNo;
        private int? _SupplierId;
        private int? _TransactionTypeId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _ExtraAmount;
        private decimal? _TotalAmount;
        private string _Narration;
        private int? _CompanyId;
        private decimal? _PaidAmount;
        private decimal? _PayAmount;
        private string _SupplierName;
        private string _TransactionType;
        private string _AmountInwords;

        private string _SearchText;

        private PurchaseReturnDetail _PRDetail;
        private ObservableCollection<PurchaseReturnDetail> _PRDetails;

        #endregion

        #region Property
        public long ReceiptSupplierId { get; set; }
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

        public DateTime? PRDate
        {
            get
            {
                return _PRDate;
            }
            set
            {
                if (_PRDate != value)
                {
                    _PRDate = value;
                    NotifyPropertyChanged(nameof(PRDate));
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
        public string InvoiceNo
        {
            get
            {
                return _InvoiceNo;
            }
            set
            {
                if (_InvoiceNo != value)
                {
                    _InvoiceNo = value;
                    NotifyPropertyChanged(nameof(InvoiceNo));
                }
            }
        }
        public int? SupplierId
        {
            get
            {
                return _SupplierId;
            }
            set
            {
                if (_SupplierId != value)
                {
                    _SupplierId = value;
                    NotifyPropertyChanged(nameof(SupplierId));
                }
            }
        }
        public int? TransactionTypeId
        {
            get
            {
                return _TransactionTypeId;
            }
            set
            {
                if (_TransactionTypeId != value)
                {
                    _TransactionTypeId = value;
                    NotifyPropertyChanged(nameof(TransactionTypeId));
                }
            }
        }
        public decimal? ItemAmount
        {
            get
            {
                if (_ItemAmount == null) _ItemAmount = 0;
                return _ItemAmount;
            }
            set
            {
                if (_ItemAmount != value)
                {
                    _ItemAmount = value;
                    NotifyPropertyChanged(nameof(ItemAmount));
                    if (value != null) SetAmount();
                }
            }
        }
        public decimal? DiscountAmount
        {
            get
            {
                if (_DiscountAmount == null) _DiscountAmount = 0;
                return _DiscountAmount;
            }
            set
            {
                if (_DiscountAmount != value)
                {
                    _DiscountAmount = value;
                    NotifyPropertyChanged(nameof(DiscountAmount));
                    if (value != null) SetAmount();
                }
            }
        }
        public decimal? GSTAmount
        {
            get
            {
                if (_GSTAmount == null) _GSTAmount = 0;
                return _GSTAmount;
            }
            set
            {
                if (_GSTAmount != value)
                {
                    _GSTAmount = value;
                    NotifyPropertyChanged(nameof(GSTAmount));
                }
            }
        }
        public decimal? ExtraAmount
        {
            get
            {
                if (_ExtraAmount == null) _ExtraAmount = 0;
                return _ExtraAmount;
            }
            set
            {
                if (_ExtraAmount != value)
                {
                    _ExtraAmount = value;
                    NotifyPropertyChanged(nameof(ExtraAmount));
                    if (value != null) SetAmount();
                }
            }
        }
        public decimal? TotalAmount
        {
            get
            {
                if (_TotalAmount == null) _TotalAmount = 0;
                return _TotalAmount;
            }
            set
            {
                if (_TotalAmount != value)
                {
                    _TotalAmount = value;
                    NotifyPropertyChanged(nameof(TotalAmount));
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
        public int? CompanyId
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
        public decimal? PaidAmount
        {
            get
            {
                if (_PaidAmount == null) _PaidAmount = 0;
                return _PaidAmount;
            }
            set
            {
                if (_PaidAmount != value)
                {
                    _PaidAmount = value;
                    NotifyPropertyChanged(nameof(PaidAmount));
                    AmountInwords = value.ToCurrencyInWords();
                }
            }
        }
        public decimal? BalanceAmount
        {
            get
            {
                if (_TotalAmount == null) return null;
                if (_PaidAmount == null) return _TotalAmount.Value;
                return _TotalAmount.Value - _PaidAmount.Value;
            }
        }
        public decimal? PayAmount
        {
            get
            {
                if (_PayAmount == null) _PayAmount = 0;
                return _PayAmount;
            }
            set
            {
                if (_PayAmount != value)
                {
                    _PayAmount = value;
                    NotifyPropertyChanged(nameof(PayAmount));
                    AmountInwords = value.ToCurrencyInWords();
                }
            }
        }
        public string SupplierName
        {
            get
            {
                return _SupplierName;
            }
            set
            {
                if (_SupplierName != value)
                {
                    _SupplierName = value;
                    NotifyPropertyChanged(nameof(SupplierName));
                }
            }
        }
        public string TransactionType
        {
            get
            {
                return _TransactionType;
            }
            set
            {
                if (_TransactionType != value)
                {
                    _TransactionType = value;
                    NotifyPropertyChanged(nameof(TransactionType));
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

        public PurchaseReturnDetail PRDetail
        {
            get
            {
                if (_PRDetail == null) _PRDetail = new PurchaseReturnDetail();
                return _PRDetail;
            }
            set
            {
                if (_PRDetail != value)
                {
                    _PRDetail = value;
                    NotifyPropertyChanged(nameof(PRDetail));
                }
            }
        }

        public ObservableCollection<PurchaseReturnDetail> PRDetails
        {
            get
            {
                if (_PRDetails == null) _PRDetails = new ObservableCollection<PurchaseReturnDetail>();
                return _PRDetails;
            }
            set
            {
                if (_PRDetails != value)
                {
                    _PRDetails = value;
                    NotifyPropertyChanged(nameof(PRDetails));
                }
            }
        }
        public static ObservableCollection<PurchaseReturn> PRPendingList
        {
            get
            {
                if (_PRPendingList == null)
                {
                    _PRPendingList = new ObservableCollection<PurchaseReturn>();
                    var l1 = FMCGHubClient.FMCGHub.Invoke<List<PurchaseReturn>>("PurchaseReturn_PRPendingList").Result;
                    _PRPendingList = new ObservableCollection<PurchaseReturn>(l1);
                }
                return _PRPendingList;
            }
            set
            {
                _PRPendingList = value;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("PurchaseReturn_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new PurchaseReturn().toCopy<PurchaseReturn>(this);
            this.PRDetail = new PurchaseReturnDetail();
            this.PRDetails = new ObservableCollection<PurchaseReturnDetail>();

            PRDate = DateTime.Now;

            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                PurchaseReturn po = FMCGHubClient.FMCGHub.Invoke<PurchaseReturn>("PurchaseReturn_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<PurchaseReturn>(this);
                this.PRDetails = po.PRDetails;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("PurchaseReturn_Delete", this.Id).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        #endregion

        #region Detail

        public void SaveDetail()
        {
            if (PRDetail.ProductId != 0)
            {
                PurchaseReturnDetail pod = PRDetails.Where(x => x.ProductId == PRDetail.ProductId).FirstOrDefault();

                if (pod == null)
                {
                    pod = new PurchaseReturnDetail();
                    PRDetails.Add(pod);
                }
                else
                {
                    PRDetail.Quantity += pod.Quantity;
                }
                PRDetail.toCopy<PurchaseReturnDetail>(pod);
                ClearDetail();
                ItemAmount = PRDetails.Sum(x => x.Amount);
            }

        }

        public void ClearDetail()
        {
            PurchaseReturnDetail pod = new PurchaseReturnDetail();
            pod.toCopy<PurchaseReturnDetail>(PRDetail);
        }

        public void DeleteDetail(string PName)
        {
            PurchaseReturnDetail pod = PRDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                PRDetails.Remove(pod);
                ItemAmount = PRDetails.Sum(x => x.Amount);
            }
        }

        #endregion

        private void SetAmount()
        {
            GSTAmount = ((ItemAmount ?? 0) - (DiscountAmount ?? 0)) * Common.AppLib.GSTPer;
            TotalAmount = (ItemAmount ?? 0) - (DiscountAmount ?? 0) + GSTAmount + (ExtraAmount ?? 0);
        }

        public bool FindRefNo()
        {
            var rv = false;
            try
            {
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_PRRef", RefNo, this).Result;
            }
            catch (Exception ex)
            {
                rv = true;
            }
            return rv;
        }
        #endregion
    }
}
