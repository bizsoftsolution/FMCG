﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using FMCG.Common;
using System.Collections.ObjectModel;

namespace FMCG.BLL
{
    public class SalesOrder : INotifyPropertyChanged
    {
        #region Field
        private static ObservableCollection<SalesOrder> _SOPendingList;

        private long _Id;
        private DateTime? _SODate;
        private string _RefNo;
        private string _SONo;
        private int? _CustomerId;
        private int? _TransactionTypeId;
        private decimal? _ItemAmount;
        private decimal? _DiscountAmount;
        private decimal? _GSTAmount;
        private decimal? _ExtraAmount;
        private decimal? _TotalAmount;
        private string _Narration;
        private int? _CompanyId;

        private string _CustomerName;
        private string _TransactionType;
        private string _AmountInwords;

        private string _SearchText;

        private SalesOrderDetail _SODetail;
        private ObservableCollection<SalesOrderDetail> _SODetails;

        #endregion

        #region Property

        public static ObservableCollection<SalesOrder> SOPendingList
        {
            get
            {
                if (_SOPendingList == null)
                {
                    _SOPendingList = new ObservableCollection<SalesOrder>();
                    var l1 = FMCGHubClient.FMCGHub.Invoke<List<SalesOrder>>("SalesOrder_SOPendingList").Result;
                    _SOPendingList = new ObservableCollection<SalesOrder>(l1);
                }
                return _SOPendingList;
            }
            set
            {
                _SOPendingList = value;
            }
        }


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

        public DateTime? SODate
        {
            get
            {
                return _SODate;
            }
            set
            {
                if (_SODate != value)
                {
                    _SODate = value;
                    NotifyPropertyChanged(nameof(SODate));
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
        public string SONo
        {
            get
            {
                return _SONo;
            }
            set
            {
                if (_SONo != value)
                {
                    _SONo = value;
                    NotifyPropertyChanged(nameof(SONo));
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

        public SalesOrderDetail SODetail
        {
            get
            {
                if (_SODetail == null) _SODetail = new SalesOrderDetail();
                return _SODetail;
            }
            set
            {
                if (_SODetail != value)
                {
                    _SODetail = value;
                    NotifyPropertyChanged(nameof(SODetail));
                }
            }
        }

        public ObservableCollection<SalesOrderDetail> SODetails
        {
            get
            {
                if (_SODetails == null) _SODetails = new ObservableCollection<SalesOrderDetail>();
                return _SODetails;
            }
            set
            {
                if (_SODetails != value)
                {
                    _SODetails = value;
                    NotifyPropertyChanged(nameof(SODetails));
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("SalesOrder_Save", this).Result;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void Clear()
        {
            new SalesOrder().toCopy<SalesOrder>(this);
            this.SODetail = new SalesOrderDetail();
            this.SODetails = new ObservableCollection<SalesOrderDetail>();

            SODate = DateTime.Now;

            NotifyAllPropertyChanged();
        }

        public bool Find()
        {
            try
            {
                SalesOrder po = FMCGHubClient.FMCGHub.Invoke<SalesOrder>("SalesOrder_Find", SearchText).Result;
                if (po.Id == 0) return false;
                po.toCopy<SalesOrder>(this);
                this.SODetails = po.SODetails;
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
                return FMCGHubClient.FMCGHub.Invoke<bool>("SalesOrder_Delete", this.Id).Result;
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
            if (SODetail.ProductId != 0)
            {
                SalesOrderDetail pod = SODetails.Where(x => x.ProductId == SODetail.ProductId).FirstOrDefault();

                if (pod == null)
                {
                    pod = new SalesOrderDetail();
                    SODetails.Add(pod);
                }
                else
                {
                    SODetail.Quantity += pod.Quantity;
                }
                SODetail.toCopy<SalesOrderDetail>(pod);
                ClearDetail();
                ItemAmount = SODetails.Sum(x => x.Amount);
            }

        }

        public void ClearDetail()
        {
            SalesOrderDetail pod = new SalesOrderDetail();
            pod.toCopy<SalesOrderDetail>(SODetail);
        }

        public void DeleteDetail(string PName)
        {
            SalesOrderDetail pod = SODetails.Where(x => x.ProductName == PName).FirstOrDefault();

            if (pod != null)
            {
                SODetails.Remove(pod);
                ItemAmount = SODetails.Sum(x => x.Amount);
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
                rv = FMCGHubClient.FMCGHub.Invoke<bool>("Find_SORef", RefNo, this).Result;
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
