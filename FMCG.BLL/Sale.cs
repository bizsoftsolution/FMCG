﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FMCG.Common;
using System.Collections.ObjectModel;

namespace FMCG.BLL
{
    public class Sale : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<Sale> _SPendingList;

        private long _Id;
        private DateTime? _SalesDate;
        private string _RefNo;
        private string _BillNo;
        private int? _CustomerId;
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
        private string _CustomerName;
        private string _TransactionType;
        private string _AmountInwords;

        private string _SearchText;

        private SalesDetail _SDetail;
        private ObservableCollection<SalesDetail> _SDetails;

        #endregion

        #region Property
        public long ReceiptCustomerId { get; set; }
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

        public DateTime? SalesDate
        {
            get
            {
                return _SalesDate;
            }
            set
            {
                if (_SalesDate != value)
                {
                    _SalesDate = value;
                    NotifyPropertyChanged(nameof(SalesDate));
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
        public string BillNo
        {
            get
            {
                return _BillNo;
            }
            set
            {
                if (_BillNo != value)
                {
                    _BillNo = value;
                    NotifyPropertyChanged(nameof(BillNo));
                }
            }
        }
        public int? CustomerId
        {
            get
            {
                return _CustomerId;
            }
            set
            {
                if (_CustomerId != value)
                {
                    _CustomerId = value;
                    NotifyPropertyChanged(nameof(CustomerId));
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
        public string CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                if (_CustomerName != value)
                {
                    _CustomerName = value;
                    NotifyPropertyChanged(nameof(CustomerName));
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

        public SalesDetail SDetail
        {
            get
            {
                if (_SDetail == null) _SDetail = new SalesDetail();
                return _SDetail;
            }
            set
            {
                if (_SDetail != value)
                {
                    _SDetail = value;
                    NotifyPropertyChanged(nameof(SDetail));
                }
            }
        }

        public ObservableCollection<SalesDetail> SDetails
        {
            get
            {
                if (_SDetails == null) _SDetails = new ObservableCollection<SalesDetail>();
                return _SDetails;
            }
            set
            {
                if (_SDetails != value)
                {
                    _SDetails = value;
                    NotifyPropertyChanged(nameof(SDetails));
                }
            }
        }
        public static ObservableCollection<Sale> SPendingList
        {
            get
            {
                if (_SPendingList == null)
                {
                    _SPendingList = new ObservableCollection<Sale>();
                    var l1 = FMCGHubClient.FMCGHub.Invoke<List<Sale>>("Sales_SPendingList").Result;
                    _SPendingList = new ObservableCollection<Sale>(l1);
                }
                return _SPendingList;
            }
            set
            {
                _SPendingList = value;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Sales_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new Sale().toCopy<Sale>(this);
            this.SDetail = new SalesDetail();
            this.SDetails = new ObservableCollection<SalesDetail>();

            SalesDate = DateTime.Now;

            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                Sale S = FMCGHubClient.FMCGHub.Invoke<Sale>("Sales_Find", SearchText).Result;
                if (S.Id == 0) return false;
                S.toCopy<Sale>(this);
                this.SDetails = S.SDetails;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("Sales_Delete", this.Id).Result;
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
            if (SDetail.ProductId != 0)
            {
                SalesDetail sod = SDetails.Where(x => x.ProductId == SDetail.ProductId).FirstOrDefault();

                if (sod == null)
                {
                    sod = new SalesDetail();
                    SDetails.Add(sod);
                }
                else
                {
                    SDetail.Quantity += sod.Quantity;
                }
                SDetail.toCopy<SalesDetail>(sod);
                ClearDetail();
                ItemAmount = SDetails.Sum(x => x.Amount);
            }

        }

        public void ClearDetail()
        {
            SalesDetail sod = new SalesDetail();
            sod.toCopy<SalesDetail>(SDetail);
        }

        public void DeleteDetail(string PName)
        {
            SalesDetail sod = SDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (sod != null)
            {
                SDetails.Remove(sod);
                ItemAmount = SDetails.Sum(x => x.Amount);
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
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_SRef", RefNo, this).Result;
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
