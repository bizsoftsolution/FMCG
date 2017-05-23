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
    public class SalesReturn : INotifyPropertyChanged
    {

        #region Field
        private static ObservableCollection<SalesReturn> _SRPendingList;
        private long _Id;
        private DateTime? _SRDate;
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

        private SalesReturnDetail _SRDetail;
        private ObservableCollection<SalesReturnDetail> _SRDetails;

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
        public DateTime? SRDate
        {
            get
            {
                return _SRDate;
            }
            set
            {
                if (_SRDate != value)
                {
                    _SRDate = value;
                    NotifyPropertyChanged(nameof(SRDate));
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

        public SalesReturnDetail SRDetail
        {
            get
            {
                if (_SRDetail == null) _SRDetail = new SalesReturnDetail();
                return _SRDetail;
            }
            set
            {
                if (_SRDetail != value)
                {
                    _SRDetail = value;
                    NotifyPropertyChanged(nameof(SRDetail));
                }
            }
        }

        public ObservableCollection<SalesReturnDetail> SRDetails
        {
            get
            {
                if (_SRDetails == null) _SRDetails = new ObservableCollection<SalesReturnDetail>();
                return _SRDetails;
            }
            set
            {
                if (_SRDetails != value)
                {
                    _SRDetails = value;
                    NotifyPropertyChanged(nameof(SRDetails));
                }
            }
        }
        public static ObservableCollection<SalesReturn> SRPendingList
        {
            get
            {
                if (_SRPendingList == null)
                {
                    _SRPendingList = new ObservableCollection<SalesReturn>();
                    var l1 = FMCGHubClient.FMCGHub.Invoke<List<SalesReturn>>("SalesReturn_SRPendingList").Result;
                    _SRPendingList = new ObservableCollection<SalesReturn>(l1);
                }
                return _SRPendingList;
            }
            set
            {
                _SRPendingList = value;
            }
        }

        public long PaymentCustomerId { get; set; }
        
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("SalesReturn_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new SalesReturn().toCopy<SalesReturn>(this);
            this.SRDetail = new SalesReturnDetail();
            this.SRDetails = new ObservableCollection<SalesReturnDetail>();

            SRDate = DateTime.Now;

            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                SalesReturn po = FMCGHubClient.FMCGHub.Invoke<SalesReturn>("SalesReturn_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<SalesReturn>(this);
                this.SRDetails = po.SRDetails;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("SalesReturn_Delete", this.Id).Result;
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
            if (SRDetail.ProductId != 0)
            {
                SalesReturnDetail pod = SRDetails.Where(x => x.ProductId == SRDetail.ProductId).FirstOrDefault();

                if (pod == null)
                {
                    pod = new SalesReturnDetail();
                    SRDetails.Add(pod);
                }
                else
                {
                    SRDetail.Quantity += pod.Quantity;
                }
                SRDetail.toCopy<SalesReturnDetail>(pod);
                ClearDetail();
                ItemAmount = SRDetails.Sum(x => x.Amount);
            }

        }

        public void ClearDetail()
        {
            SalesReturnDetail pod = new SalesReturnDetail();
            pod.toCopy<SalesReturnDetail>(SRDetail);
        }

        public void DeleteDetail(string PName)
        {
            SalesReturnDetail pod = SRDetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                SRDetails.Remove(pod);
                ItemAmount = SRDetails.Sum(x => x.Amount);
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
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_SRRef", RefNo, this).Result;
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
