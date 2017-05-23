using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using FMCG.Common;
using System.Web.Script.Serialization;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;

namespace FMCG.SL.Hubs
{
    public class FMCGHub : Hub
    {

        #region Field

        private static DAL.FMCGDBEntities DB = new DAL.FMCGDBEntities();

        private static List<User> UserList = new List<User>();
        private static List<DAL.EntityType> _entityTypeList;
        private static List<DAL.LogDetailType> _logDetailTypeList;

        #endregion

        #region Property

        private static List<DAL.EntityType> EntityTypeList
        {
            get
            {
                if (_entityTypeList == null)
                {
                    _entityTypeList = DB.EntityTypes.ToList();

                }
                return _entityTypeList;
            }
            set
            {
                _entityTypeList = value;
            }
        }
        private static List<DAL.LogDetailType> LogDetailTypeList
        {
            get
            {
                if (_logDetailTypeList == null) _logDetailTypeList = DB.LogDetailTypes.ToList();
                return _logDetailTypeList;
            }
            set
            {
                _logDetailTypeList = value;
            }
        }
        public bool isLoginUser
        {
            get
            {
                User u = UserList.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
                return u.UserId != 0 ? true : false;
            }
        }

        #region ClientSelection

        private User Caller
        {
            get
            {
                User u = UserList.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
                if (u == null)
                {
                    u = new User() { ConnectionId = Context.ConnectionId, UserId = 0, CompanyId = 0 };
                    UserList.Add(u);
                }
                return u;
            }
        }

        private List<string> AllClients
        {
            get
            {
                return UserList.Select(x => x.ConnectionId.ToString()).ToList();
            }
        }

        private List<string> AllLoginClients
        {
            get
            {
                return UserList.Where(x => x.UserId != 0)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> OtherClients
        {
            get
            {
                return UserList.Where(x => x.ConnectionId != Context.ConnectionId)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> OtherLoginClients
        {
            get
            {
                return UserList.Where(x => x.ConnectionId != Context.ConnectionId && x.UserId != 0)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> AllClientsOnGroup
        {
            get
            {
                return UserList.Where(x => x.CompanyId == Caller.CompanyId)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> AllLoginClientsOnGroup
        {
            get
            {
                return UserList.Where(x => x.CompanyId == Caller.CompanyId && x.UserId != 0)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> OtherClientsOnGroup
        {
            get
            {
                return UserList.Where(x => x.CompanyId == Caller.CompanyId && x.UserId != Caller.UserId)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        private List<string> OtherLoginClientsOnGroup
        {
            get
            {
                return UserList.Where(x => x.CompanyId == Caller.CompanyId && x.UserId != 0 && x.UserId != Caller.UserId)
                               .Select(x => x.ConnectionId.ToString())
                               .ToList();
            }
        }

        #endregion

        #endregion

        #region Constant
        enum LogDetailType
        {
            INSERT,
            UPDATE,
            DELETE
        }

        #endregion

        #region Method

        public override Task OnConnected()
        {
            User u = UserList.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (u == null)
            {
                u = new User() { ConnectionId = Context.ConnectionId, UserId = 0, CompanyId = 0 };
                UserList.Add(u);
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            User u = UserList.Where(x => x.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (u != null)
            {
                UserList.Remove(u);
            }

            return base.OnDisconnected(stopCalled);
        }

        private int EntityTypeId(string Type)
        {
            DAL.EntityType et = EntityTypeList.Where(x => x.Entity == Type).FirstOrDefault();
            if (et == null)
            {
                et = new DAL.EntityType();
                DB.EntityTypes.Add(et);
                EntityTypeList.Add(et);
                et.Entity = Type;
                DB.SaveChanges();
            }
            return et.Id;
        }

        private int LogDetailTypeId(LogDetailType Type)
        {
            DAL.LogDetailType ldt = LogDetailTypeList.Where(x => x.Type == Type.ToString()).FirstOrDefault();
            return ldt.Id;
        }

        private void LogDetailStore(object Data, LogDetailType Type)
        {
            try
            {
                Type t = Data.GetType();
                long EntityId = Convert.ToInt64(t.GetProperty("Id").GetValue(Data));
                int ETypeId = EntityTypeId(t.Name);

                DAL.LogMaster l = DB.LogMasters.Where(x => x.EntityId == EntityId && x.EntityTypeId == ETypeId).FirstOrDefault();
                DAL.LogDetail ld = new DAL.LogDetail();
                DateTime dt = DateTime.Now;


                if (l == null)
                {
                    l = new DAL.LogMaster();
                    DB.LogMasters.Add(l);
                    l.EntityId = EntityId;
                    l.EntityTypeId = ETypeId;
                    l.CompanyId = Caller.CompanyId;
                    l.CreatedAt = dt;
                    l.CreatedBy = Caller.UserId;
                }

                if (Type == LogDetailType.UPDATE)
                {
                    l.UpdatedAt = dt;
                    l.UpdatedBy = Caller.UserId;
                }
                else if (Type == LogDetailType.DELETE)
                {
                    l.DeletedAt = dt;
                    l.DeletedBy = Caller.UserId;
                }

                DB.SaveChanges();

                DB.LogDetails.Add(ld);
                ld.LogMasterId = l.Id;
                ld.RecordDetail = new JavaScriptSerializer().Serialize(Data);
                ld.EntryBy = Caller.UserId;
                ld.EntryAt = dt;
                ld.LogDetailTypeId = LogDetailTypeId(Type);
                DB.SaveChanges();
            }
            catch (Exception ex) { }

        }
        #endregion

        #region Hubs

        #region Master

        #region YearList
        public static List<BLL.YearList> _listYear;
        public static List<BLL.YearList> listYear
        {
            get
            {
                if (_listYear == null)
                {
                    _listYear = new List<BLL.YearList>();
                    _listYear.Add(new BLL.YearList() { AccountYear = "2015-2016" });
                    _listYear.Add(new BLL.YearList() { AccountYear = "2016-2017" });
                    _listYear.Add(new BLL.YearList() { AccountYear = "2017-2018" });
                }
                return _listYear;
            }
        }
        public List<BLL.YearList> Year_List()
        {
            return listYear;
        }


        #endregion


        #region CompanyDetail

        public static List<BLL.CompanyDetail> _listCompany;
        public static List<BLL.CompanyDetail> ListCompany
        {
            get
            {
                if (_listCompany == null)
                {
                    _listCompany = DB.CompanyDetails.Select(x => new BLL.CompanyDetail()
                    {
                        Id = x.Id,
                        CompanyName = x.CompanyName,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        CityId = x.CityId,
                        EMailId = x.EMailId,
                        GSTNo = x.GSTNo,
                        Logo = x.Logo,
                        MobileNo = x.MobileNo,
                        PostalCode = x.PostalCode,
                        TelephoneNo = x.TelephoneNo,
                        CityName = x.City.CityName,
                        UnderCompanyId = x.UnderCompanyId??0,
                        CompanyType = x.CompanyType
                    }
                    ).ToList();
                }
                return _listCompany;
            }
        }
        public List<BLL.CompanyDetail> CompanyDetail_List()
        {
            return ListCompany;
        }

        public int CompanyDetail_Save(BLL.CompanyDetail sgp)
        {
            try
            {

                BLL.CompanyDetail b = ListCompany.Where(x => x.Id == sgp.Id).FirstOrDefault();
                DAL.CompanyDetail d = DB.CompanyDetails.Where(x => x.Id == sgp.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.CompanyDetail();
                    ListCompany.Add(b);

                    d = new DAL.CompanyDetail();
                    DB.CompanyDetails.Add(d);

                    sgp.toCopy<DAL.CompanyDetail>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.CompanyDetail>(b);
                    DB.SaveChanges();

                    sgp.Id = d.Id;

                    DAL.UserAccount ua = new DAL.UserAccount();
                    ua.CompanyId = sgp.Id;
                    ua.LoginId = "Admin";
                    ua.UserName = "Admin";
                    ua.Password = "Admin";
                    ua.UserTypeId = 1;

                    DB.UserAccounts.Add(ua);
                    DB.SaveChanges();



                    // LogDetailStore(sgp, LogDetailType.INSERT);
                }
                else
                {
                    sgp.toCopy<BLL.CompanyDetail>(b);
                    sgp.toCopy<DAL.CompanyDetail>(d);
                    DB.SaveChanges();
                    // LogDetailStore(sgp, LogDetailType.UPDATE);
                }

                Clients.Others.CompanyDetail_Save(sgp);

                return sgp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        #endregion

        #region UserAccount

        public BLL.UserAccount UserAccount_Login(string AccYear, String CompanyName, String LoginId, String Password)
        {
            BLL.UserAccount u = new BLL.UserAccount();

            DAL.UserAccount ua = DB.UserAccounts
                                   .Where(x => x.CompanyDetail.CompanyName == CompanyName

                                                && x.LoginId == LoginId
                                                && x.Password == Password)
                                   .FirstOrDefault();
            if (ua != null)
            {
                Groups.Add(Context.ConnectionId, ua.CompanyId.ToString());
                Caller.CompanyId = ua.CompanyId;
                Caller.UserId = ua.Id;
                Caller.AccYear = AccYear;
                ua.toCopy<BLL.UserAccount>(u);
            }

            return u;
        }

        #endregion

        #region UOM

        #region list
        public static List<BLL.UOM> _uomList;

        public static List<BLL.UOM> UOMList
        {
            get
            {
                if (_uomList == null)
                {
                    _uomList = new List<BLL.UOM>();
                    foreach (var d1 in DB.UOMs.OrderBy(x => x.Symbol).ToList())
                    {
                        BLL.UOM d2 = new BLL.UOM();
                        d1.toCopy<BLL.UOM>(d2);
                        _uomList.Add(d2);
                    }

                }
                return _uomList;
            }
            set
            {
                _uomList = value;
            }

        }

        #endregion

        public List<BLL.UOM> UOM_List()
        {
            return UOMList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public int UOM_Save(BLL.UOM uom)
        {
            try
            {
                uom.CompanyId = Caller.CompanyId;

                BLL.UOM b = UOMList.Where(x => x.Id == uom.Id).FirstOrDefault();
                DAL.UOM d = DB.UOMs.Where(x => x.Id == uom.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.UOM();
                    UOMList.Add(b);

                    d = new DAL.UOM();
                    DB.UOMs.Add(d);

                    uom.toCopy<DAL.UOM>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.UOM>(b);

                    DB.SaveChanges();
                    uom.Id = d.Id;
                    LogDetailStore(uom, LogDetailType.INSERT);
                }
                else
                {
                    uom.toCopy<BLL.UOM>(b);
                    uom.toCopy<DAL.UOM>(d);
                    DB.SaveChanges();
                    LogDetailStore(uom, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).UOM_Save(uom);

                return uom.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void UOM_Delete(int pk)
        {
            try
            {
                BLL.UOM b = UOMList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.UOMs.Where(x => x.Id == pk).FirstOrDefault();

                    DB.UOMs.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    UOMList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).UOM_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }

        #endregion

        #region UserType

        public static List<BLL.UserType> _listUserType;
        public static List<BLL.UserType> ListUserType
        {
            get
            {
                if (_listUserType == null)
                {
                    _listUserType = DB.UserTypes.Select(x => new BLL.UserType()
                    {
                        Id = x.Id,
                        TypeOfUser = x.TypeOfUser,
                        Description = x.Description
                    }
                    ).ToList();
                }
                return _listUserType;
            }

            set
            {
                _listUserType = value;
            }
        }

        public List<BLL.UserType> UserType_List()
        {
            return ListUserType;

        }

        public int userType_Save(BLL.UserType ut)
        {
            try
            {

                BLL.UserType b = ListUserType.Where(x => x.Id == ut.Id).FirstOrDefault();
                DAL.UserType d = DB.UserTypes.Where(x => x.Id == ut.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.UserType();
                    ListUserType.Add(b);

                    d = new DAL.UserType();
                    DB.UserTypes.Add(d);

                    ut.toCopy<DAL.UserType>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.UserType>(b);

                    DB.SaveChanges();
                    ut.Id = d.Id;
                    LogDetailStore(ut, LogDetailType.INSERT);
                }
                else
                {
                    ut.toCopy<BLL.UserType>(b);
                    ut.toCopy<DAL.UserType>(d);
                    DB.SaveChanges();
                    LogDetailStore(ut, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).userType_Save(ut);

                return ut.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void userType_Delete(int pk)
        {
            try
            {
                BLL.UserType b = ListUserType.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.UserTypes.Where(x => x.Id == pk).FirstOrDefault();

                    DB.UserTypes.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    ListUserType.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).userType_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }

        #endregion

        #region UserTypeDetail
        public static List<BLL.UserTypeDetail> _listUserTypeDetail;
        public static List<BLL.UserTypeDetail> ListUserTypeDetail
        {
            get
            {
                if (_listUserTypeDetail == null)
                {
                    _listUserTypeDetail = DB.UserTypeDetails.Select(x => new BLL.UserTypeDetail()
                    {
                        Id = x.Id,
                        UserTypeId = x.UserTypeId,
                        UserTypeFormDetailId = x.UserTypeFormDetailId,
                        IsViewForm = x.IsViewForm,
                        AllowDelete = x.AllowDelete,
                        AllowInsert = x.AllowInsert,
                        AllowUpdate = x.AllowUpdate,
                        FormName = x.UserTypeFormDetail.FormName,
                        UserTypeName = x.UserType.TypeOfUser

                    }
                    ).ToList();
                }
                return _listUserTypeDetail;
            }
            set
            {
                _listUserTypeDetail = value;
            }
        }

        public List<BLL.UserTypeDetail> UserTypeDetail_List()
        {
            return ListUserTypeDetail;
        }
        #endregion

        #region UserTypeForm
        public static List<BLL.UserTypeFormDetail> _listUserTypeFormDetail;
        public static List<BLL.UserTypeFormDetail> ListUserTypeFormDetail
        {
            get
            {
                if (_listUserTypeFormDetail == null)
                {
                    _listUserTypeFormDetail = DB.UserTypeFormDetails.Select(x => new BLL.UserTypeFormDetail()
                    {
                        Id = x.Id,
                        FormName = x.FormName
                    }
                    ).ToList();
                }
                return _listUserTypeFormDetail;
            }
            set
            {
                _listUserTypeFormDetail = value;
            }
        }

        public List<BLL.UserTypeFormDetail> UserTypeFormDetail_List()
        {
            return ListUserTypeFormDetail;
        }
        #endregion

        #region Stock Group
        #region list
        public static List<BLL.StockGroup> _stockList;

        public static List<BLL.StockGroup> StockList
        {
            get
            {
                if (_stockList == null)
                {
                    _stockList = new List<BLL.StockGroup>();
                    foreach (var d1 in DB.StockGroups.Select(x => new BLL.StockGroup()
                    {
                        Id = x.Id,
                        CompanyId = x.CompanyId.Value,
                        StockGroupCode = x.StockGroupCode,
                        StockGroupName = x.StockGroupName,
                        UnderStockGroupName = x.StockGroup2.StockGroupName,
                        UnderStockId = x.UnderStockId.Value
                    }).OrderBy(x => x.StockGroupName).ToList())

                    {
                        BLL.StockGroup d2 = new BLL.StockGroup();
                        d1.toCopy<BLL.StockGroup>(d2);
                        _stockList.Add(d2);
                    }

                }
                return _stockList;
            }
            set
            {
                _stockList = value;
            }

        }

        #endregion

        public List<BLL.StockGroup> stockGroup_List()
        {
            return StockList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public int StockGroup_Save(BLL.StockGroup sgp)
        {
            try
            {
                sgp.CompanyId = Caller.CompanyId;

                BLL.StockGroup b = StockList.Where(x => x.Id == sgp.Id).FirstOrDefault();
                DAL.StockGroup d = DB.StockGroups.Where(x => x.Id == sgp.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.StockGroup();
                    StockList.Add(b);

                    d = new DAL.StockGroup();
                    DB.StockGroups.Add(d);

                    sgp.toCopy<DAL.StockGroup>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.StockGroup>(b);

                    DB.SaveChanges();
                    sgp.Id = d.Id;
                    LogDetailStore(sgp, LogDetailType.INSERT);
                }
                else
                {
                    sgp.toCopy<BLL.StockGroup>(b);
                    sgp.toCopy<DAL.StockGroup>(d);
                    DB.SaveChanges();
                    LogDetailStore(sgp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).StockGroup_Save(sgp);

                return sgp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void StockGroup_Delete(int pk)
        {
            try
            {
                BLL.StockGroup b = StockList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.StockGroups.Where(x => x.Id == pk).FirstOrDefault();

                    DB.StockGroups.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    StockList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).StockGroup_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #region Product 

        #region list
        public static List<BLL.Product> _productList;
        public static List<BLL.Product> productList
        {
            get
            {
                if (_productList == null)
                {
                    _productList = DB.Products.
                        Select(x => new BLL.Product()
                        {
                            Id = x.Id,
                            CompanyId = x.CompanyId.Value,
                            GST = x.GST == null ? 0 : (double)x.GST.Value,
                            ItemCode = x.ItemCode,
                            MRP = x.MRP == null ? 0 : (decimal)x.MRP.Value,
                            OpeningStock = x.OpeningStock == null ? 0 : (double)x.OpeningStock.Value,
                            ProductName = x.ProductName,
                            PurchaseRate = x.PurchaseRate == null ? 0 : (decimal)x.PurchaseRate.Value,
                            ReOrderLevel = x.ReOrderLevel == null ? 0 : (double)x.ReOrderLevel.Value,
                            SellingRate = x.SellingRate == null ? 0 : x.SellingRate.Value,
                            StockGroupId = x.StockGroupId == null ? 0 : (int)x.StockGroupId.Value,
                            StockGroupName = x.StockGroup.StockGroupName,
                            UOMId = x.UOMId == null ? 0 : (int)x.UOMId.Value,
                            uomSymbol = x.UOM.Symbol,

                            POQty = x.PurchaseOrderDetails.Sum(y => y.Quantity ?? 0),
                            PQty = x.PurchaseDetails.Sum(y => y.Quantity ?? 0),
                            PRQty = x.PurchaseReturnDetails.Sum(y => y.Quantity ?? 0),
                            SOQty = x.SalesOrderDetails.Sum(y => y.Quantity ?? 0),
                            SQty = x.SalesDetails.Sum(y => y.Quantity ?? 0),
                            SRQty = x.SalesReturnDetails.Sum(y => y.Quantity ?? 0),
                        }).OrderBy(x => x.ProductName).ToList();

                }
                return _productList;
            }
            set
            {
                _productList = value;
            }

        }
        #endregion

        public List<BLL.Product> ProductList()
        {
            return productList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public int Product_Save(BLL.Product pro)
        {
            try
            {
                pro.CompanyId = Caller.CompanyId;

                BLL.Product b = productList.Where(x => x.Id == pro.Id).FirstOrDefault();
                DAL.Product d = DB.Products.Where(x => x.Id == pro.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.Product();
                    productList.Add(b);

                    d = new DAL.Product();
                    DB.Products.Add(d);

                    pro.toCopy<DAL.Product>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.Product>(b);

                    DB.SaveChanges();
                    pro.Id = d.Id;
                    LogDetailStore(pro, LogDetailType.INSERT);
                }
                else
                {
                    pro.toCopy<BLL.Product>(b);
                    pro.toCopy<DAL.Product>(d);
                    DB.SaveChanges();
                    LogDetailStore(pro, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Product_Save(pro);

                return pro.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void Product_Delete(int pk)
        {
            try
            {
                BLL.Product b = productList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.Products.Where(x => x.Id == pk).FirstOrDefault();

                    DB.Products.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    productList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Product_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }

        #endregion

        #region Customer

        #region list
        public static List<BLL.Customer> _customerList;
        public static List<BLL.Customer> customerList
        {
            get
            {
                if (_customerList == null)
                {
                    _customerList = DB.Customers.Select(x => new BLL.Customer()
                    {
                        Id = x.Id,
                        AccountGroupId = x.AccountGroupId == null ? 0 : (int)x.AccountGroupId,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        CityId = x.CityId == null ? 0 : (int)x.CityId,
                        CompanyId = x.CompanyId == null ? 0 : (int)x.CompanyId,
                        CreditAmount = x.CreditAmount == null ? 0 : (double)x.CreditAmount,
                        CreditLimit = x.CreditLimit == null ? 0 : (int)x.CreditLimit,
                        CreditLimitTypeId = x.CreditLimitTypeId == null ? 0 : (int)x.CreditLimitTypeId,
                        CustomerName = x.CustomerName,
                        EMailId = x.EMailId,
                        GSTNo = x.GSTNo,
                        MobileNo = x.MobileNo,
                        PersonIncharge = x.PersonIncharge,
                        TelephoneNo = x.TelephoneNo,
                        BillingAmount = x.Sales.Sum(y => y.TotalAmount ?? 0),
                        CrBillingAmount = x.Sales.Where(y => y.TransactionType != null).Where(y => y.TransactionType.Type == "Credit").Sum(y => y.TotalAmount),
                        PaidAmount = x.Sales.Sum(y => y.ReceiptCustomers.Sum(z => z.Amount)),
                        CityName = x.City.CityName,
                        CreditLimitType = x.CreditLimitType.LimitType

                    }).OrderBy(x => x.CustomerName).ToList();
                }
                return _customerList;
            }
            set
            {
                _customerList = value;
            }

        }
        #endregion

        public List<BLL.Customer> Customer_List()
        {
            return customerList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public int Customer_Save(BLL.Customer cus)
        {
            try
            {
                cus.CompanyId = Caller.CompanyId;

                BLL.Customer b = customerList.Where(x => x.Id == cus.Id).FirstOrDefault();
                DAL.Customer d = DB.Customers.Where(x => x.Id == cus.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.Customer();
                    customerList.Add(b);

                    d = new DAL.Customer();
                    DB.Customers.Add(d);

                    cus.toCopy<DAL.Customer>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.Customer>(b);

                    DB.SaveChanges();
                    cus.Id = d.Id;
                    LogDetailStore(cus, LogDetailType.INSERT);
                }
                else
                {
                    cus.toCopy<BLL.Customer>(b);
                    cus.toCopy<DAL.Customer>(d);
                    DB.SaveChanges();
                    LogDetailStore(cus, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Customer_Save(cus);

                return cus.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void Customer_Delete(int pk)
        {
            try
            {
                BLL.Customer b = customerList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.Customers.Where(x => x.Id == pk).FirstOrDefault();

                    DB.Customers.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    customerList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Customer_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #region Supplier

        #region list
        public static List<BLL.Supplier> _supplierList;
        public static List<BLL.Supplier> supplierList
        {
            get
            {
                if (_supplierList == null)
                {
                    _supplierList = new List<BLL.Supplier>();
                    foreach (var d1 in DB.Suppliers.Select(x => new BLL.Supplier()
                    {
                        Id = x.Id,
                        TelephoneNo = x.TelephoneNo,
                        PersonIncharge = x.PersonIncharge,
                        MobileNo = x.MobileNo,
                        AccountGroupId = x.AccountGroupId == null ? 0 : (int)x.AccountGroupId,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        CityId = x.CityId == null ? 0 : (int)x.CityId,
                        CompanyId = x.CompanyId == null ? 0 : (int)x.CompanyId,
                        CreditAmount = x.CreditAmount == null ? 0 : (double)x.CreditAmount,
                        CreditLimit = x.CreditLimit == null ? 0 : (int)x.CreditLimit,
                        CreditLimitTypeId = x.CreditLimitTypeId == null ? 0 : (int)x.CreditLimitTypeId,
                        EMailId = x.EMailId,
                        GSTNo = x.GSTNo,
                        SupplierName = x.SupplierName,
                        BillingAmount = x.Purchases.Sum(y => y.TotalAmount ?? 0),
                        CrBillingAmount = x.Purchases.Where(y => y.TransactionType != null).Where(y => y.TransactionType.Type == "Credit").Sum(y => y.TotalAmount),
                        PaidAmount = x.Purchases.Sum(y => y.PaymentSuppliers.Sum(z => z.Amount)),
                        CityName = x.City.CityName,
                        CreditLimitType = x.CreditLimitType.LimitType


                    }).OrderBy(x => x.SupplierName).ToList())
                    {
                        BLL.Supplier d2 = new BLL.Supplier();
                        d1.toCopy<BLL.Supplier>(d2);
                        _supplierList.Add(d2);
                    }

                }
                return _supplierList;
            }
            set
            {
                _supplierList = value;
            }

        }
        #endregion

        public List<BLL.Supplier> Supplier_List()
        {
            return supplierList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public int Supplier_Save(BLL.Supplier sup)
        {
            try
            {
                sup.CompanyId = Caller.CompanyId;

                BLL.Supplier b = supplierList.Where(x => x.Id == sup.Id).FirstOrDefault();
                DAL.Supplier d = DB.Suppliers.Where(x => x.Id == sup.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.Supplier();
                    supplierList.Add(b);

                    d = new DAL.Supplier();
                    DB.Suppliers.Add(d);

                    sup.toCopy<DAL.Supplier>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.Supplier>(b);

                    DB.SaveChanges();
                    sup.Id = d.Id;
                    LogDetailStore(sup, LogDetailType.INSERT);
                }
                else
                {
                    sup.toCopy<BLL.Supplier>(b);
                    sup.toCopy<DAL.Supplier>(d);
                    DB.SaveChanges();
                    LogDetailStore(sup, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Supplier_Save(sup);

                return sup.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void Supplier_Delete(int pk)
        {
            try
            {
                BLL.Supplier b = supplierList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.Suppliers.Where(x => x.Id == pk).FirstOrDefault();

                    DB.Suppliers.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    supplierList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Supplier_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #region Account Group

        #region list
        public static List<BLL.AccountGroup> _accountGroupList;
        public static List<BLL.AccountGroup> accountGroupList
        {
            get
            {
                if (_accountGroupList == null)
                {
                    _accountGroupList = new List<BLL.AccountGroup>();
                    foreach (var d1 in DB.AccountGroups.
                        Select(x => new BLL.AccountGroup()
                        {
                            Id = x.Id,
                            GroupCode = x.GroupCode,
                            GroupName = x.GroupName,
                            UnderGroupId = x.UnderGroupId == null ? 0 : (int)x.UnderGroupId,
                            underGroupName = x.AccountGroup2.GroupName
                        }).
                        OrderBy(x => x.GroupName).ToList())
                    {
                        BLL.AccountGroup d2 = new BLL.AccountGroup();
                        d1.toCopy<BLL.AccountGroup>(d2);
                        _accountGroupList.Add(d2);
                    }

                }
                return _accountGroupList;
            }
            set
            {
                _accountGroupList = value;
            }

        }
        #endregion

        public List<BLL.AccountGroup> accountGroup_List()
        {
            return accountGroupList;
        }

        public int AccountGroup_Save(BLL.AccountGroup agp)
        {
            try
            {

                BLL.AccountGroup b = accountGroupList.Where(x => x.Id == agp.Id).FirstOrDefault();
                DAL.AccountGroup d = DB.AccountGroups.Where(x => x.Id == agp.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.AccountGroup();
                    accountGroupList.Add(b);

                    d = new DAL.AccountGroup();
                    DB.AccountGroups.Add(d);

                    agp.toCopy<DAL.AccountGroup>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.AccountGroup>(b);

                    DB.SaveChanges();
                    agp.Id = d.Id;
                    LogDetailStore(agp, LogDetailType.INSERT);
                }
                else
                {
                    agp.toCopy<BLL.AccountGroup>(b);
                    agp.toCopy<DAL.AccountGroup>(d);
                    DB.SaveChanges();
                    LogDetailStore(agp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).AccountGroup_Save(agp);

                return agp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void AccountGroup_Delete(int pk)
        {
            try
            {
                BLL.AccountGroup b = accountGroupList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.AccountGroups.Where(x => x.Id == pk).FirstOrDefault();

                    DB.AccountGroups.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    accountGroupList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).SAccountGroup_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #region Ledger

        #region list
        public static List<BLL.Ledger> _ledgerList;
        public static List<BLL.Ledger> ledgerList
        {
            get
            {
                if (_ledgerList == null)
                {
                    _ledgerList = new List<BLL.Ledger>();
                    foreach (var d1 in DB.Ledgers
                        .Select(x => new BLL.Ledger()
                        {
                            Id = x.Id,
                            TelephoneNo = x.TelephoneNo,
                            PersonIncharge = x.PersonIncharge,
                            MobileNo = x.MobileNo,
                            AccountGroupId = x.AccountGroupId == null ? 0 : (int)x.AccountGroupId,
                            AddressLine1 = x.AddressLine1,
                            AddressLine2 = x.AddressLine2,
                            CityId = x.CityId == null ? 0 : (int)x.CityId,
                            CompanyId = x.CompanyId == null ? 0 : (int)x.CompanyId,
                            CreditAmount = x.CreditAmount == null ? 0 : (double)x.CreditAmount,
                            CreditLimit = x.CreditLimit == null ? 0 : (int)x.CreditLimit,
                            CreditLimitTypeId = x.CreditLimitTypeId == null ? 0 : (int)x.CreditLimitTypeId,
                            EMailId = x.EMailId,
                            GSTNo = x.GSTNo,
                            LedgerName = x.LedgerName,
                            CityName = x.City.CityName,
                            CreditLimitType = x.CreditLimitType.LimitType

                        })
                        .OrderBy(x => x.LedgerName).ToList())
                    {
                        BLL.Ledger d2 = new BLL.Ledger();
                        d1.toCopy<BLL.Ledger>(d2);
                        _ledgerList.Add(d2);
                    }

                }
                return _ledgerList;
            }
            set
            {
                _ledgerList = value;
            }

        }
        #endregion

        public List<BLL.Ledger> Ledger_List()
        {
            return ledgerList;
        }

        public int Ledger_Save(BLL.Ledger led)
        {
            try
            {

                BLL.Ledger b = ledgerList.Where(x => x.Id == led.Id).FirstOrDefault();
                DAL.Ledger d = DB.Ledgers.Where(x => x.Id == led.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.Ledger();
                    ledgerList.Add(b);

                    d = new DAL.Ledger();
                    DB.Ledgers.Add(d);

                    led.toCopy<DAL.Ledger>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.Ledger>(b);

                    DB.SaveChanges();
                    led.Id = d.Id;
                    LogDetailStore(led, LogDetailType.INSERT);
                }
                else
                {
                    led.toCopy<BLL.Ledger>(b);
                    led.toCopy<DAL.Ledger>(d);
                    DB.SaveChanges();
                    LogDetailStore(led, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Ledger_Save(led);

                return led.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void Ledger_Delete(int pk)
        {
            try
            {
                BLL.Ledger b = ledgerList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.Ledgers.Where(x => x.Id == pk).FirstOrDefault();

                    DB.Ledgers.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    ledgerList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Ledger_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #region Staff

        #region list
        public static List<BLL.Staff> _staffList;
        public static List<BLL.Staff> staffList
        {
            get
            {
                if (_staffList == null)
                {
                    _staffList = new List<BLL.Staff>();
                    foreach (var d1 in DB.Staffs.Select(x => new BLL.Staff()
                    {
                        Id = x.Id,
                        TelephoneNo = x.TelephoneNo,
                        StaffName = x.StaffName,
                        MobileNo = x.MobileNo,
                        UserAccountId = x.UserAccountId == null ? 0 : (int)x.UserAccountId,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        CityId = x.CityId == null ? 0 : (int)x.CityId,
                        CompanyId = x.CompanyId == null ? 0 : (int)x.CompanyId,
                        NRICNo = x.NRICNo,
                        DesignationId = x.DesignationId.Value,
                        DesignationName = x.Designation.DesignationName,
                        DOB = x.DOB.Value,
                        DOJ = x.DOJ.Value,
                        EMailId = x.EMailId,
                        GenderId = x.GenderId.Value,
                        LoginId = x.UserAccount.LoginId,
                        Password = x.UserAccount.Password,
                        Salary = x.Salary

                    }).OrderBy(x => x.StaffName).ToList())
                    {
                        BLL.Staff d2 = new BLL.Staff();
                        d1.toCopy<BLL.Staff>(d2);
                        _staffList.Add(d2);
                    }

                }
                return _staffList;
            }
            set
            {
                _staffList = value;
            }
        }
        #endregion

        public List<BLL.Staff> Staff_List()
        {
            return staffList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public int Staff_Save(BLL.Staff stf)
        {
            try
            {
                stf.CompanyId = Caller.CompanyId;

                BLL.Staff b = staffList.Where(x => x.Id == stf.Id).FirstOrDefault();
                DAL.Staff d = DB.Staffs.Where(x => x.Id == stf.Id).FirstOrDefault();


                if (d == null)
                {

                    b = new BLL.Staff();
                    staffList.Add(b);

                    d = new DAL.Staff();
                    DB.Staffs.Add(d);

                    stf.toCopy<DAL.Staff>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.Staff>(b);

                    DB.SaveChanges();
                    stf.Id = d.Id;

                    DAL.Designation ds = DB.Designations.Where(x => x.Id == stf.DesignationId).FirstOrDefault();
                    if (stf.Id != 0 && stf.LoginId != "" && stf.Password != "" && ds != null)
                    {
                        DAL.UserAccount da = new DAL.UserAccount();
                        da.LoginId = stf.LoginId;
                        da.Password = stf.Password;
                        da.UserTypeId = ds.UserTypeId.Value;
                        da.CompanyId = stf.CompanyId;
                        da.UserName = stf.StaffName;
                        DB.UserAccounts.Add(da);
                        DB.SaveChanges();
                        stf.UserAccountId = da.Id;
                        d.UserAccountId = da.Id;
                        DB.SaveChanges();
                    }

                    LogDetailStore(stf, LogDetailType.INSERT);
                }
                else
                {
                    stf.toCopy<BLL.Staff>(b);
                    stf.toCopy<DAL.Staff>(d);
                    DB.SaveChanges();
                    LogDetailStore(stf, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Staff_Save(stf);

                return stf.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void Staff_Delete(int pk)
        {
            try
            {
                BLL.Staff b = staffList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.Staffs.Where(x => x.Id == pk).FirstOrDefault();

                    DB.Staffs.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    staffList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Staff_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #region Bank

        #region list
        public static List<BLL.Bank> _bankList;
        public static List<BLL.Bank> bankList
        {
            get
            {
                if (_bankList == null)
                {
                    _bankList = new List<BLL.Bank>();
                    foreach (var d1 in DB.Banks.Select(x => new BLL.Bank()
                    {
                        Id = x.Id,
                        AccountGroupId = x.AccountGroupId.Value,
                        AccountName = x.AccountGroup.GroupName,
                        AccountNo = x.AccountNo,
                        AddressLine1 = x.AddressLine1,
                        AddressLine2 = x.AddressLine2,
                        BankAccountTypeId = x.BankAccountTypeId.Value,
                        BankName = x.BankName,
                        CityId = x.CityId == null ? 0 : (int)x.CityId,
                        CompanyId = x.CompanyId.Value,
                        EMailId = x.EMailId,
                        MobileNo = x.MobileNo,
                        PersonIncharge = x.PersonIncharge,
                        TelephoneNo = x.TelephoneNo,
                        CityName = x.City.CityName


                    }).OrderBy(x => x.BankName).ToList())
                    {
                        BLL.Bank d2 = new BLL.Bank();
                        d1.toCopy<BLL.Bank>(d2);
                        _bankList.Add(d2);
                    }

                }
                return _bankList;
            }
            set
            {
                _bankList = value;
            }

        }
        #endregion

        public List<BLL.Bank> Bank_List()
        {
            return bankList;
        }

        public int Bank_Save(BLL.Bank ban)
        {
            try
            {

                BLL.Bank b = bankList.Where(x => x.Id == ban.Id).FirstOrDefault();
                DAL.Bank d = DB.Banks.Where(x => x.Id == ban.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.Bank();
                    bankList.Add(b);

                    d = new DAL.Bank();
                    DB.Banks.Add(d);

                    ban.toCopy<DAL.Bank>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.Bank>(b);

                    DB.SaveChanges();
                    ban.Id = d.Id;
                    LogDetailStore(ban, LogDetailType.INSERT);
                }
                else
                {
                    ban.toCopy<BLL.Bank>(b);
                    ban.toCopy<DAL.Bank>(d);
                    DB.SaveChanges();
                    LogDetailStore(ban, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Bank_Save(ban);

                return ban.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void Bank_Delete(int pk)
        {
            try
            {
                BLL.Bank b = bankList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.Banks.Where(x => x.Id == pk).FirstOrDefault();

                    DB.Banks.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    bankList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Bank_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #region City

        #region list
        public static List<BLL.City> _cityList;
        public static List<BLL.City> cityList
        {
            get
            {
                if (_cityList == null)
                {
                    _cityList = new List<BLL.City>();
                    foreach (var d1 in DB.Cities.OrderBy(x => x.CityName).ToList())
                    {
                        BLL.City d2 = new BLL.City();
                        d1.toCopy<BLL.City>(d2);
                        _cityList.Add(d2);
                    }

                }
                return _cityList;
            }
            set
            {
                _cityList = value;
            }

        }
        #endregion

        public List<BLL.City> City_List()
        {
            return cityList;
        }

        #endregion

        #region State

        #region list
        public static List<BLL.State> _stateList;
        public static List<BLL.State> stateList
        {
            get
            {
                if (_stateList == null)
                {
                    _stateList = new List<BLL.State>();
                    foreach (var d1 in DB.States.OrderBy(x => x.StateName).ToList())
                    {
                        BLL.State d2 = new BLL.State();
                        d1.toCopy<BLL.State>(d2);
                        _stateList.Add(d2);
                    }

                }
                return _stateList;
            }
            set
            {
                _stateList = value;
            }

        }
        #endregion

        public List<BLL.State> StateList()
        {
            return stateList;
        }

        #endregion

        #region Designation
        #region list
        public static List<BLL.Designation> _designationList;

        public static List<BLL.Designation> DesignationList
        {
            get
            {
                if (_designationList == null)
                {
                    _designationList = new List<BLL.Designation>();
                    foreach (var d1 in DB.Designations.Select(x => new BLL.Designation()
                    {
                        Id = x.Id,
                        DesignationName = x.DesignationName,
                        UserTypeId = x.UserTypeId.Value,
                        UserTypeName = x.UserType.TypeOfUser
                    }).OrderBy(x => x.DesignationName).ToList())

                    {
                        BLL.Designation d2 = new BLL.Designation();
                        d1.toCopy<BLL.Designation>(d2);
                        _designationList.Add(d2);
                    }

                }
                return _designationList;
            }
            set
            {
                _designationList = value;
            }

        }

        #endregion

        public List<BLL.Designation> Designation_List()
        {
            return DesignationList;
        }

        public int designation_Save(BLL.Designation sgp)
        {
            try
            {

                BLL.Designation b = DesignationList.Where(x => x.Id == sgp.Id).FirstOrDefault();
                DAL.Designation d = DB.Designations.Where(x => x.Id == sgp.Id).FirstOrDefault();

                if (d == null)
                {

                    b = new BLL.Designation();
                    DesignationList.Add(b);

                    d = new DAL.Designation();
                    DB.Designations.Add(d);

                    sgp.toCopy<DAL.Designation>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.Designation>(b);

                    DB.SaveChanges();
                    sgp.Id = d.Id;
                    LogDetailStore(sgp, LogDetailType.INSERT);
                }
                else
                {
                    sgp.toCopy<BLL.Designation>(b);
                    sgp.toCopy<DAL.Designation>(d);
                    DB.SaveChanges();
                    LogDetailStore(sgp, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).designation_Save(sgp);

                return sgp.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void designation_Delete(int pk)
        {
            try
            {
                BLL.Designation b = DesignationList.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.Designations.Where(x => x.Id == pk).FirstOrDefault();

                    DB.Designations.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    DesignationList.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).Designation_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #region Gender

        #region list
        public static List<BLL.Gender> _genderList;
        public static List<BLL.Gender> genderList
        {
            get
            {
                if (_genderList == null)
                {
                    _genderList = new List<BLL.Gender>();
                    foreach (var d1 in DB.Genders.OrderBy(x => x.GenderName).ToList())
                    {
                        BLL.Gender d2 = new BLL.Gender();
                        d1.toCopy<BLL.Gender>(d2);
                        _genderList.Add(d2);
                    }

                }
                return _genderList;
            }
            set
            {
                _genderList = value;
            }

        }
        #endregion

        public List<BLL.Gender> Gender_List()
        {
            return genderList;
        }

        #endregion

        #region BankAccountType

        #region list
        public static List<BLL.BankAccountType> _accountList;
        public static List<BLL.BankAccountType> accountList
        {
            get
            {
                if (_accountList == null)
                {
                    _accountList = new List<BLL.BankAccountType>();
                    foreach (var d1 in DB.BankAccountTypes.ToList())
                    {
                        BLL.BankAccountType d2 = new BLL.BankAccountType();
                        d1.toCopy<BLL.BankAccountType>(d2);
                        _accountList.Add(d2);
                    }

                }
                return _accountList;
            }
            set
            {
                _accountList = value;
            }

        }
        #endregion

        public List<BLL.BankAccountType> accountType_List()
        {
            return accountList;
        }



        #endregion

        #region CreditLimit

        #region list
        public static List<BLL.CreditLimitType> _creditLimitList;
        public static List<BLL.CreditLimitType> creditLimitList
        {
            get
            {
                if (_creditLimitList == null)
                {
                    _creditLimitList = new List<BLL.CreditLimitType>();
                    foreach (var d1 in DB.CreditLimitTypes.ToList())
                    {
                        BLL.CreditLimitType d2 = new BLL.CreditLimitType();
                        d1.toCopy<BLL.CreditLimitType>(d2);
                        _creditLimitList.Add(d2);
                    }

                }
                return _creditLimitList;
            }
            set
            {
                _creditLimitList = value;
            }

        }
        #endregion

        public List<BLL.CreditLimitType> creditLimitType_List()
        {
            return creditLimitList;
        }



        #endregion

        #region TransactionType

        #region list
        public static List<BLL.TransactionType> _TransactionTypeList;
        public static List<BLL.TransactionType> TransactionTypeList
        {
            get
            {
                if (_TransactionTypeList == null)
                {
                    _TransactionTypeList = new List<BLL.TransactionType>();
                    foreach (var d1 in DB.TransactionTypes.OrderBy(x => x.Type).ToList())
                    {
                        BLL.TransactionType d2 = new BLL.TransactionType();
                        d1.toCopy<BLL.TransactionType>(d2);
                        _TransactionTypeList.Add(d2);
                    }

                }
                return _TransactionTypeList;
            }
            set
            {
                _TransactionTypeList = value;
            }

        }
        #endregion

        public List<BLL.TransactionType> TransactionType_List()
        {
            return TransactionTypeList;
        }

        #endregion

        #region LedgerOpening
        List<BLL.LedgerOpening> lstLedgerOpening = new List<BLL.LedgerOpening>();

        //public static List<BLL.LedgerOpening> _LedgerOPList;
        //public static List<BLL.LedgerOpening> LedgerOPList
        //{
        //    get
        //    {
        //        if (_LedgerOPList == null)
        //        {
        //            _LedgerOPList = DB.LedgerOpenings.Select(x => new BLL.LedgerOpening()
        //            {

        //                Id = x.Id,
        //               EntityType=x.EntityType, 
        //               EntityId=x.EntityId.Value, AcYear=x.AcYear, CompanyId=x.CompanyId.Value, CrAmt=x.CrAmt.Value, DrAmt=x.DrAmt.Value
        //            }).OrderBy(x => x.EntityId).ToList();
        //        }
        //        return _LedgerOPList;
        //    }
        //    set
        //    {
        //        _LedgerOPList = value;
        //    }

        //}
        #endregion


        public List<BLL.LedgerOpening> LedgerOpening_List()
        {
            BLL.LedgerOpening tb = new BLL.LedgerOpening();

            var l1 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var C1 = DB.Customers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var S1 = DB.Suppliers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var B1 = DB.Banks.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            decimal TotDr = 0, TotCr = 0;
            var lstOPBal = DB.LedgerOpenings.Where(x => x.CompanyId == Caller.CompanyId && x.AcYear == Caller.AccYear).ToList();

            #region Ledger

            var eType = "Ledger";
            foreach (var l in l1)
            {
                tb = new BLL.LedgerOpening();
                var OPBal = lstOPBal.Where(x => x.EntityId == l.Id && x.EntityType == eType).FirstOrDefault();

                tb.EntityId = l.Id;
                tb.EntityType = eType;
                tb.LedgerName = l.LedgerName;
                tb.AcYear = Caller.AccYear;
                tb.DrAmt = (OPBal ?? new DAL.LedgerOpening()).DrAmt ?? 0;
                tb.CrAmt = (OPBal ?? new DAL.LedgerOpening()).CrAmt ?? 0;
                tb.CompanyId = l.CompanyId.Value;
                if (tb.DrAmt == null)
                {
                    tb.DrAmt = 0;
                }
                if (tb.CrAmt == null)
                {
                    tb.CrAmt = 0;
                }
                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = 0;
                }


                lstLedgerOpening.Add(tb);

            }
            #endregion

            #region Customer

            eType = "Customer";
            foreach (var l in C1)
            {
                tb = new BLL.LedgerOpening();
                var OPBal = lstOPBal.Where(x => x.EntityId == l.Id && x.EntityType == eType).FirstOrDefault();
                tb.EntityId = l.Id;
                tb.EntityType = eType;
                tb.LedgerName = l.CustomerName;
                tb.AcYear = Caller.AccYear;
                tb.DrAmt = (OPBal ?? new DAL.LedgerOpening()).DrAmt ?? 0;
                tb.CrAmt = (OPBal ?? new DAL.LedgerOpening()).CrAmt ?? 0;
                tb.CompanyId = l.CompanyId.Value;

                if (tb.DrAmt == null)
                {
                    tb.DrAmt = 0;
                }
                if (tb.CrAmt == null)
                {
                    tb.CrAmt = 0;
                }
                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = 0;
                }

                lstLedgerOpening.Add(tb);

            }

            #endregion

            #region Supplier

            eType = "Supplier";
            foreach (var l in S1)
            {
                tb = new BLL.LedgerOpening();
                var OPBal = lstOPBal.Where(x => x.EntityId == l.Id && x.EntityType == eType).FirstOrDefault();
                tb.EntityId = l.Id;
                tb.EntityType = eType;
                tb.LedgerName = l.SupplierName;
                tb.AcYear = Caller.AccYear;
                tb.DrAmt = (OPBal ?? new DAL.LedgerOpening()).DrAmt ?? 0;
                tb.CrAmt = (OPBal ?? new DAL.LedgerOpening()).CrAmt ?? 0;
                tb.CompanyId = l.CompanyId.Value;

                if (tb.DrAmt == null)
                {
                    tb.DrAmt = 0;
                }
                if (tb.CrAmt == null)
                {
                    tb.CrAmt = 0;
                }
                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else

                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = 0;
                }


                lstLedgerOpening.Add(tb);

            }
            #endregion

            #region Bank

            eType = "Bank";
            foreach (var l in B1)
            {
                tb = new BLL.LedgerOpening();
                var OPBal = lstOPBal.Where(x => x.EntityId == l.Id && x.EntityType == eType).FirstOrDefault();
                tb.EntityId = l.Id;
                tb.EntityType = eType;
                tb.LedgerName = l.BankName;
                tb.AcYear = Caller.AccYear;
                tb.DrAmt = (OPBal ?? new DAL.LedgerOpening()).DrAmt ?? 0;
                tb.CrAmt = (OPBal ?? new DAL.LedgerOpening()).CrAmt ?? 0;
                tb.CompanyId = l.CompanyId.Value;

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = 0;
                }


                lstLedgerOpening.Add(tb);

            }
            #endregion

            return lstLedgerOpening;
        }

        public int LedgerOpening_Save(BLL.LedgerOpening stf)
        {
            try
            {
                stf.CompanyId = Caller.CompanyId;
                stf.AcYear = Caller.AccYear;


                BLL.LedgerOpening b = lstLedgerOpening.Where(x => x.Id == stf.Id).FirstOrDefault();
                DAL.LedgerOpening d = DB.LedgerOpenings.Where(x => x.Id == stf.Id).FirstOrDefault();


                if (d == null)
                {

                    b = new BLL.LedgerOpening();
                    lstLedgerOpening.Add(b);

                    d = new DAL.LedgerOpening();
                    DB.LedgerOpenings.Add(d);

                    stf.toCopy<DAL.LedgerOpening>(d);
                    DB.SaveChanges();
                    d.toCopy<BLL.LedgerOpening>(b);


                    stf.Id = d.Id;



                    LogDetailStore(stf, LogDetailType.INSERT);
                }
                else
                {
                    stf.toCopy<BLL.LedgerOpening>(b);
                    stf.toCopy<DAL.LedgerOpening>(d);
                    DB.SaveChanges();
                    LogDetailStore(stf, LogDetailType.UPDATE);
                }

                Clients.Clients(OtherLoginClientsOnGroup).LedgerOpening_Save(stf);

                return stf.Id;
            }
            catch (Exception ex) { }
            return 0;
        }

        public void LedgerOpening_Delete(int pk)
        {
            try
            {
                BLL.LedgerOpening b = lstLedgerOpening.Where(x => x.Id == pk).FirstOrDefault();
                if (b != null)
                {
                    var d = DB.LedgerOpenings.Where(x => x.Id == pk).FirstOrDefault();

                    DB.LedgerOpenings.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(b, LogDetailType.DELETE);
                    lstLedgerOpening.Remove(b);
                }

                Clients.Clients(OtherLoginClientsOnGroup).LedgerOpening_Delete(pk);
                Clients.All.delete(pk);
            }
            catch (Exception ex) { }
        }


        #endregion

        #endregion

        #region Transaction

        #region Purchase Order

        #region list
        public static List<BLL.PurchaseOrder> _POPendingList;
        public static List<BLL.PurchaseOrder> POPendingList
        {
            get
            {
                if (_POPendingList == null)
                {
                    _POPendingList = new List<BLL.PurchaseOrder>();
                    foreach (var d1 in DB.PurchaseOrders.OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.PurchaseOrder d2 = new BLL.PurchaseOrder();
                        d1.toCopy<BLL.PurchaseOrder>(d2);
                        _POPendingList.Add(d2);
                    }

                }
                return _POPendingList;
            }
            set
            {
                _POPendingList = value;
            }
        }
        #endregion


        public bool PurchaseOrder_Save(BLL.PurchaseOrder PO)
        {
            try
            {
                PO.CompanyId = Caller.CompanyId;

                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.PurchaseOrder();
                    DB.PurchaseOrders.Add(d);

                    PO.toCopy<DAL.PurchaseOrder>(d);

                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseOrderDetail d_pod = new DAL.PurchaseOrderDetail();
                        b_pod.toCopy<DAL.PurchaseOrderDetail>(d_pod);
                        d.PurchaseOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    PO.Id = d.Id;
                    LogDetailStore(PO, LogDetailType.INSERT);
                }
                else
                {
                    PO.toCopy<DAL.PurchaseOrder>(d);
                    foreach (var b_pod in PO.PODetails)
                    {
                        DAL.PurchaseOrderDetail d_pod = new DAL.PurchaseOrderDetail();
                        b_pod.toCopy<DAL.PurchaseOrderDetail>(d_pod);
                        d.PurchaseOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.UPDATE);
                }

                BLL.PurchaseOrder B_PO = POPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();

                if (B_PO == null)
                {
                    B_PO = new BLL.PurchaseOrder();
                    POPendingList.Add(B_PO);
                }

                PO.toCopy<BLL.PurchaseOrder>(B_PO);
                Clients.Clients(OtherLoginClientsOnGroup).PurchaseOrder_POPendingSave(B_PO);

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.PurchaseOrder PurchaseOrder_Find(string SearchText)
        {
            BLL.PurchaseOrder PO = new BLL.PurchaseOrder();
            try
            {

                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.PurchaseOrder>(PO);
                    PO.SupplierName = (d.Supplier ?? DB.Suppliers.Find(d.SupplierId) ?? new DAL.Supplier()).SupplierName;
                    PO.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseOrderDetails)
                    {
                        BLL.PurchaseOrderDetail b_pod = new BLL.PurchaseOrderDetail();
                        d_pod.toCopy<BLL.PurchaseOrderDetail>(b_pod);
                        PO.PODetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return PO;
        }

        public bool PurchaseOrder_Delete(long pk)
        {
            try
            {
                DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.PurchaseOrder PO = new BLL.PurchaseOrder();
                    d.toCopy<BLL.PurchaseOrder>(PO);
                    PO.SupplierName = d.Supplier.SupplierName;
                    PO.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.PurchaseOrderDetails)
                    {
                        BLL.PurchaseOrderDetail b_pod = new BLL.PurchaseOrderDetail();
                        d_pod.toCopy<BLL.PurchaseOrderDetail>(b_pod);
                        PO.PODetails.Add(b_pod);

                    }
                    DB.PurchaseOrderDetails.RemoveRange(d.PurchaseOrderDetails);
                    DB.PurchaseOrders.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(PO, LogDetailType.DELETE);

                    BLL.PurchaseOrder B_PO = POPendingList.Where(x => x.Id == PO.Id).FirstOrDefault();
                    if (B_PO != null)
                    {
                        Clients.Clients(OtherLoginClientsOnGroup).PurchaseOrder_POPendingDelete(B_PO.Id);
                        POPendingList.Remove(B_PO);
                    }
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.PurchaseOrder> PurchaseOrder_POPendingList()
        {
            return POPendingList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public bool Find_PORef(string RefNo, BLL.PurchaseOrder PO)

        {
            DAL.PurchaseOrder d = DB.PurchaseOrders.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        #endregion

        #region Sales Order

        #region list
        public static List<BLL.SalesOrder> _SOPendingList;
        public static List<BLL.SalesOrder> SOPendingList
        {
            get
            {
                if (_SOPendingList == null)
                {
                    _SOPendingList = new List<BLL.SalesOrder>();
                    foreach (var d1 in DB.SalesOrders.OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.SalesOrder d2 = new BLL.SalesOrder();
                        d1.toCopy<BLL.SalesOrder>(d2);
                        _SOPendingList.Add(d2);
                    }

                }
                return _SOPendingList;
            }
            set
            {
                _SOPendingList = value;
            }
        }
        #endregion

        public bool SalesOrder_Save(BLL.SalesOrder SO)
        {
            try
            {
                SO.CompanyId = Caller.CompanyId;

                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.SalesOrder();
                    DB.SalesOrders.Add(d);

                    SO.toCopy<DAL.SalesOrder>(d);

                    foreach (var b_pod in SO.SODetails)
                    {
                        DAL.SalesOrderDetail d_pod = new DAL.SalesOrderDetail();
                        b_pod.toCopy<DAL.SalesOrderDetail>(d_pod);
                        d.SalesOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(SO, LogDetailType.INSERT);
                }
                else
                {
                    SO.toCopy<DAL.SalesOrder>(d);
                    foreach (var b_pod in SO.SODetails)
                    {
                        DAL.SalesOrderDetail d_pod = new DAL.SalesOrderDetail();
                        b_pod.toCopy<DAL.SalesOrderDetail>(d_pod);
                        d.SalesOrderDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(SO, LogDetailType.UPDATE);
                }
                BLL.SalesOrder B_SO = SOPendingList.Where(x => x.Id == SO.Id).FirstOrDefault();

                if (B_SO == null)
                {
                    B_SO = new BLL.SalesOrder();
                    SOPendingList.Add(B_SO);
                }

                SO.toCopy<BLL.SalesOrder>(B_SO);
                Clients.Clients(OtherLoginClientsOnGroup).SalesOrder_SOPendingSave(B_SO);

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.SalesOrder SalesOrder_Find(string SearchText)
        {
            BLL.SalesOrder SO = new BLL.SalesOrder();
            try
            {

                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.SalesOrder>(SO);
                    SO.CustomerName = (d.Customer ?? DB.Customers.Find(d.CustomerId) ?? new DAL.Customer()).CustomerName;
                    SO.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesOrderDetails)
                    {
                        BLL.SalesOrderDetail b_pod = new BLL.SalesOrderDetail();
                        d_pod.toCopy<BLL.SalesOrderDetail>(b_pod);
                        SO.SODetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return SO;
        }

        public bool SalesOrder_Delete(long pk)
        {
            try
            {
                DAL.SalesOrder d = DB.SalesOrders.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.SalesOrder SO = new BLL.SalesOrder();
                    d.toCopy<BLL.SalesOrder>(SO);
                    SO.CustomerName = d.Customer.CustomerName;
                    SO.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.SalesOrderDetails)
                    {
                        BLL.SalesOrderDetail b_pod = new BLL.SalesOrderDetail();
                        d_pod.toCopy<BLL.SalesOrderDetail>(b_pod);
                        SO.SODetails.Add(b_pod);

                    }
                    DB.SalesOrderDetails.RemoveRange(d.SalesOrderDetails);
                    DB.SalesOrders.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(SO, LogDetailType.DELETE);

                    BLL.SalesOrder B_SO = SOPendingList.Where(x => x.Id == SO.Id).FirstOrDefault();
                    if (B_SO != null)
                    {
                        Clients.Clients(OtherLoginClientsOnGroup).SalesOrder_SOPendingDelete(B_SO.Id);
                        SOPendingList.Remove(B_SO);
                    }
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.SalesOrder> SalesOrder_SOPendingList()
        {
            return SOPendingList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public bool Find_SORef(string RefNo, BLL.SalesOrder PO)

        {
            DAL.SalesOrder d = DB.SalesOrders.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        #endregion

        #region Purchase

        #region list
        public static List<BLL.Purchase> _PPendingList;
        public static List<BLL.Purchase> PPendingList
        {
            get
            {
                if (_PPendingList == null)
                {
                    _PPendingList = new List<BLL.Purchase>();
                    foreach (var d1 in DB.Purchases.Where(x => x.TransactionType.Type == "Credit").OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.Purchase d2 = new BLL.Purchase();
                        d1.toCopy<BLL.Purchase>(d2);
                        _PPendingList.Add(d2);
                    }

                }
                return _PPendingList;
            }
            set
            {
                _PPendingList = value;
            }
        }
        #endregion

        public bool Purchase_Save(BLL.Purchase P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.Purchase d = DB.Purchases.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Purchase();
                    DB.Purchases.Add(d);

                    P.toCopy<DAL.Purchase>(d);

                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        d.PurchaseDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.Purchase>(d);
                    foreach (var b_pod in P.PDetails)
                    {
                        DAL.PurchaseDetail d_pod = new DAL.PurchaseDetail();
                        b_pod.toCopy<DAL.PurchaseDetail>(d_pod);
                        d.PurchaseDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Purchase Purchase_Find(string SearchText)
        {
            BLL.Purchase P = new BLL.Purchase();
            try
            {

                DAL.Purchase d = DB.Purchases.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Purchase>(P);
                    P.SupplierName = (d.Supplier ?? DB.Suppliers.Find(d.SupplierId) ?? new DAL.Supplier()).SupplierName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.toCopy<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool Purchase_Delete(long pk)
        {
            try
            {
                DAL.Purchase d = DB.Purchases.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Purchase P = new BLL.Purchase();
                    d.toCopy<BLL.Purchase>(P);
                    P.SupplierName = d.Supplier.SupplierName;
                    P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.PurchaseDetails)
                    {
                        BLL.PurchaseDetail b_pod = new BLL.PurchaseDetail();
                        d_pod.toCopy<BLL.PurchaseDetail>(b_pod);
                        P.PDetails.Add(b_pod);

                    }
                    DB.PurchaseDetails.RemoveRange(d.PurchaseDetails);
                    DB.Purchases.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        public List<BLL.Purchase> Purchase_PPendingList()
        {
            return PPendingList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public bool Find_PRef(string RefNo, BLL.Purchase PO)

        {
            DAL.Purchase d = DB.Purchases.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        #endregion

        #region Sales
        #region list
        public static List<BLL.Sale> _SPendingList;
        public static List<BLL.Sale> SPendingList
        {
            get
            {
                if (_SPendingList == null)
                {
                    _SPendingList = new List<BLL.Sale>();
                    foreach (var d1 in DB.Sales.Where(x => x.TransactionType.Type == "Credit").OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.Sale d2 = new BLL.Sale();
                        d1.toCopy<BLL.Sale>(d2);
                        _SPendingList.Add(d2);
                    }

                }
                return _SPendingList;
            }
            set
            {
                _SPendingList = value;
            }
        }
        #endregion
        public bool Sales_Save(BLL.Sale P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.Sale d = DB.Sales.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Sale();
                    DB.Sales.Add(d);

                    P.toCopy<DAL.Sale>(d);

                    foreach (var b_pod in P.SDetails)
                    {
                        DAL.SalesDetail d_pod = new DAL.SalesDetail();
                        b_pod.toCopy<DAL.SalesDetail>(d_pod);
                        d.SalesDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.Sale>(d);
                    foreach (var b_pod in P.SDetails)
                    {
                        DAL.SalesDetail d_pod = new DAL.SalesDetail();
                        b_pod.toCopy<DAL.SalesDetail>(d_pod);
                        d.SalesDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Sale Sales_Find(string SearchText)
        {
            BLL.Sale P = new BLL.Sale();
            try
            {

                DAL.Sale d = DB.Sales.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Sale>(P);
                    P.CustomerName = (d.Customer ?? DB.Customers.Find(d.CustomerId) ?? new DAL.Customer()).CustomerName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesDetails)
                    {
                        BLL.SalesDetail b_pod = new BLL.SalesDetail();
                        d_pod.toCopy<BLL.SalesDetail>(b_pod);
                        P.SDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool Sales_Delete(long pk)
        {
            try
            {
                DAL.Sale d = DB.Sales.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Sale P = new BLL.Sale();
                    d.toCopy<BLL.Sale>(P);
                    P.CustomerName = d.Customer.CustomerName;
                    P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.SalesDetails)
                    {
                        BLL.SalesDetail b_pod = new BLL.SalesDetail();
                        d_pod.toCopy<BLL.SalesDetail>(b_pod);
                        P.SDetails.Add(b_pod);

                    }
                    DB.SalesDetails.RemoveRange(d.SalesDetails);
                    DB.Sales.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        public List<BLL.Sale> Sales_SPendingList()
        {
            return SPendingList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public bool Find_SRef(string RefNo, BLL.Sale PO)

        {
            DAL.Sale d = DB.Sales.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        #endregion

        #region PurchaseReturn

        #region list
        public static List<BLL.PurchaseReturn> _PRPendingList;
        public static List<BLL.PurchaseReturn> PRPendingList
        {
            get
            {
                if (_PRPendingList == null)
                {
                    _PRPendingList = new List<BLL.PurchaseReturn>();
                    foreach (var d1 in DB.PurchaseReturns.Where(x => x.TransactionType.Type == "Credit").OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.PurchaseReturn d2 = new BLL.PurchaseReturn();
                        d1.toCopy<BLL.PurchaseReturn>(d2);
                        _PRPendingList.Add(d2);
                    }

                }
                return _PRPendingList;
            }
            set
            {
                _PRPendingList = value;
            }
        }
        #endregion

        public bool PurchaseReturn_Save(BLL.PurchaseReturn P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.PurchaseReturn();
                    DB.PurchaseReturns.Add(d);

                    P.toCopy<DAL.PurchaseReturn>(d);

                    foreach (var b_pod in P.PRDetails)
                    {
                        DAL.PurchaseReturnDetail d_pod = new DAL.PurchaseReturnDetail();
                        b_pod.toCopy<DAL.PurchaseReturnDetail>(d_pod);
                        d.PurchaseReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.PurchaseReturn>(d);
                    foreach (var b_pod in P.PRDetails)
                    {
                        DAL.PurchaseReturnDetail d_pod = new DAL.PurchaseReturnDetail();
                        b_pod.toCopy<DAL.PurchaseReturnDetail>(d_pod);
                        d.PurchaseReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.PurchaseReturn PurchaseReturn_Find(string SearchText)
        {
            BLL.PurchaseReturn P = new BLL.PurchaseReturn();
            try
            {

                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.PurchaseReturn>(P);
                    P.SupplierName = (d.Supplier ?? DB.Suppliers.Find(d.SupplierId) ?? new DAL.Supplier()).SupplierName;
                    P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.PurchaseReturnDetails)
                    {
                        BLL.PurchaseReturnDetail b_pod = new BLL.PurchaseReturnDetail();
                        d_pod.toCopy<BLL.PurchaseReturnDetail>(b_pod);
                        P.PRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool PurchaseReturn_Delete(long pk)
        {
            try
            {
                DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.PurchaseReturn P = new BLL.PurchaseReturn();
                    d.toCopy<BLL.PurchaseReturn>(P);
                    P.SupplierName = d.Supplier.SupplierName;
                    P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.PurchaseReturnDetails)
                    {
                        BLL.PurchaseReturnDetail b_pod = new BLL.PurchaseReturnDetail();
                        d_pod.toCopy<BLL.PurchaseReturnDetail>(b_pod);
                        P.PRDetails.Add(b_pod);

                    }
                    DB.PurchaseReturnDetails.RemoveRange(d.PurchaseReturnDetails);
                    DB.PurchaseReturns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }
        public List<BLL.PurchaseReturn> PurchaseReturn_PRPendingList()
        {
            return PRPendingList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public bool Find_PRRef(string RefNo, BLL.PurchaseReturn PO)

        {
            DAL.PurchaseReturn d = DB.PurchaseReturns.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }

        #endregion

        #region SalesReturn
        #region list
        public static List<BLL.SalesReturn> _SRPendingList;
        public static List<BLL.SalesReturn> SRPendingList
        {
            get
            {
                if (_SRPendingList == null)
                {
                    _SRPendingList = new List<BLL.SalesReturn>();
                    foreach (var d1 in DB.SalesReturns.Where(x => x.TransactionType.Type == "Credit").OrderBy(x => x.RefNo).ToList())
                    {
                        BLL.SalesReturn d2 = new BLL.SalesReturn();
                        d1.toCopy<BLL.SalesReturn>(d2);
                        _SRPendingList.Add(d2);
                    }

                }
                return _SRPendingList;
            }
            set
            {
                _SRPendingList = value;
            }
        }
        #endregion
        public bool SalesReturn_Save(BLL.SalesReturn P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.SalesReturn();
                    DB.SalesReturns.Add(d);

                    P.toCopy<DAL.SalesReturn>(d);

                    foreach (var b_pod in P.SRDetails)
                    {
                        DAL.SalesReturnDetail d_pod = new DAL.SalesReturnDetail();
                        b_pod.toCopy<DAL.SalesReturnDetail>(d_pod);
                        d.SalesReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.SalesReturn>(d);
                    foreach (var b_pod in P.SRDetails)
                    {
                        DAL.SalesReturnDetail d_pod = new DAL.SalesReturnDetail();
                        b_pod.toCopy<DAL.SalesReturnDetail>(d_pod);
                        d.SalesReturnDetails.Add(d_pod);
                    }
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.SalesReturn SalesReturn_Find(string SearchText)
        {
            BLL.SalesReturn P = new BLL.SalesReturn();
            try
            {

                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.SalesReturn>(P);
                    P.CustomerName = (d.Customer ?? DB.Customers.Find(d.CustomerId) ?? new DAL.Customer()).CustomerName;
                    // P.TransactionType = (d.TransactionType ?? DB.TransactionTypes.Find(d.TransactionTypeId) ?? new DAL.TransactionType()).Type;
                    foreach (var d_pod in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_pod = new BLL.SalesReturnDetail();
                        d_pod.toCopy<BLL.SalesReturnDetail>(b_pod);
                        P.SRDetails.Add(b_pod);
                        b_pod.ProductName = (d_pod.Product ?? DB.Products.Find(d_pod.ProductId) ?? new DAL.Product()).ProductName;
                        b_pod.UOMName = (d_pod.UOM ?? DB.UOMs.Find(d_pod.UOMId) ?? new DAL.UOM()).Symbol;
                    }

                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool SalesReturn_Delete(long pk)
        {
            try
            {
                DAL.SalesReturn d = DB.SalesReturns.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.SalesReturn P = new BLL.SalesReturn();
                    d.toCopy<BLL.SalesReturn>(P);
                    P.CustomerName = d.Customer.CustomerName;
                    //P.TransactionType = d.TransactionType.Type;
                    foreach (var d_pod in d.SalesReturnDetails)
                    {
                        BLL.SalesReturnDetail b_pod = new BLL.SalesReturnDetail();
                        d_pod.toCopy<BLL.SalesReturnDetail>(b_pod);
                        P.SRDetails.Add(b_pod);

                    }
                    DB.SalesReturnDetails.RemoveRange(d.SalesReturnDetails);
                    DB.SalesReturns.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);
                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public List<BLL.SalesReturn> SalesReturn_SRPendingList()
        {
            return SRPendingList.Where(x => x.CompanyId == Caller.CompanyId).ToList();
        }

        public bool Find_SRRef(string RefNo, BLL.SalesReturn PO)

        {
            DAL.SalesReturn d = DB.SalesReturns.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        #endregion


        #region Payments

        public bool Payment_Save(BLL.Payment P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.Payment d = DB.Payments.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Payment();
                    DB.Payments.Add(d);

                    P.toCopy<DAL.Payment>(d);
                    if (P.PayMode == "Cash")
                    {
                        DAL.PaymentCash pCash = new DAL.PaymentCash();
                        d.PaymentCashes.Add(pCash);
                    }
                    else if (P.PayMode == "Cheque")
                    {
                        DAL.PaymentCheque pCheque = new DAL.PaymentCheque();
                        P.PCheque.toCopy<DAL.PaymentCheque>(pCheque);
                        d.PaymentCheques.Add(pCheque);
                    }
                    else if (P.PayMode == "Online")
                    {
                        DAL.PaymentOnline pOnline = new DAL.PaymentOnline();
                        P.POnline.toCopy<DAL.PaymentOnline>(pOnline);
                        d.PaymentOnlines.Add(pOnline);
                    }
                    else if (P.PayMode == "TT")
                    {
                        DAL.PaymentTT pTT = new DAL.PaymentTT();
                        P.PTT.toCopy<DAL.PaymentTT>(pTT);
                        d.PaymentTTs.Add(pTT);
                    }

                    if (P.PayTo == "Customer")
                    {
                        foreach (var pc in P.SRPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.PaymentCustomer pCustomer = new DAL.PaymentCustomer();
                            P.PCustomer.toCopy<DAL.PaymentCustomer>(pCustomer);
                            pCustomer.SalesReturnId = pc.Id;
                            pCustomer.Amount = pc.PayAmount;
                            d.PaymentCustomers.Add(pCustomer);
                        }
                    }
                    else if (P.PayTo == "Supplier")
                    {
                        foreach (var pc in P.PPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.PaymentSupplier pSupplier = new DAL.PaymentSupplier();
                            P.PSupplier.toCopy<DAL.PaymentSupplier>(pSupplier);
                            pSupplier.PurchaseId = pc.Id;
                            pSupplier.Amount = pc.PayAmount;
                            d.PaymentSuppliers.Add(pSupplier);
                        }
                    }
                    else if (P.PayTo == "Staff")
                    {
                        DAL.PaymentStaff pStaff = new DAL.PaymentStaff();
                        P.PStaff.toCopy<DAL.PaymentStaff>(pStaff);
                        d.PaymentStaffs.Add(pStaff);
                    }
                    else if (P.PayTo == "Bank")
                    {
                        DAL.PaymentBank pBank = new DAL.PaymentBank();
                        P.PBank.toCopy<DAL.PaymentBank>(pBank);
                        d.PaymentBanks.Add(pBank);
                    }
                    else if (P.PayTo == "Ledger")
                    {
                        DAL.PaymentLedger pLedger = new DAL.PaymentLedger();
                        P.PLedger.toCopy<DAL.PaymentLedger>(pLedger);
                        d.PaymentLedgers.Add(pLedger);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.Payment>(d);
                    if (P.PayMode == "Cash")
                    {
                        DAL.PaymentCash pCash;
                        if (d.PaymentCashes.FirstOrDefault() == null)
                        {
                            pCash = new DAL.PaymentCash();
                            d.PaymentCashes.Add(pCash);
                        }
                        else
                        {
                            pCash = d.PaymentCashes.FirstOrDefault();
                        }
                        P.PCash.toCopy<DAL.PaymentCash>(pCash);

                        if (d.PaymentCheques.FirstOrDefault() != null) DB.PaymentCheques.RemoveRange(d.PaymentCheques);
                        if (d.PaymentOnlines.FirstOrDefault() != null) DB.PaymentOnlines.RemoveRange(d.PaymentOnlines);
                        if (d.PaymentTTs.FirstOrDefault() != null) DB.PaymentTTs.RemoveRange(d.PaymentTTs);
                    }
                    else if (P.PayMode == "Cheque")
                    {
                        DAL.PaymentCheque pCheque;
                        if (d.PaymentCheques.FirstOrDefault() == null)
                        {
                            pCheque = new DAL.PaymentCheque();
                            d.PaymentCheques.Add(pCheque);
                        }
                        else
                        {
                            pCheque = d.PaymentCheques.FirstOrDefault();
                        }
                        P.PCheque.toCopy<DAL.PaymentCheque>(pCheque);

                        if (d.PaymentCashes.FirstOrDefault() != null) DB.PaymentCashes.RemoveRange(d.PaymentCashes);
                        if (d.PaymentOnlines.FirstOrDefault() != null) DB.PaymentOnlines.RemoveRange(d.PaymentOnlines);
                        if (d.PaymentTTs.FirstOrDefault() != null) DB.PaymentTTs.RemoveRange(d.PaymentTTs);
                    }
                    else if (P.PayMode == "Online")
                    {
                        DAL.PaymentOnline pOnline;
                        if (d.PaymentCheques.FirstOrDefault() == null)
                        {
                            pOnline = new DAL.PaymentOnline();
                            d.PaymentOnlines.Add(pOnline);
                        }
                        else
                        {
                            pOnline = d.PaymentOnlines.FirstOrDefault();
                        }
                        P.POnline.toCopy<DAL.PaymentOnline>(pOnline);

                        if (d.PaymentCashes.FirstOrDefault() != null) DB.PaymentCashes.RemoveRange(d.PaymentCashes);
                        if (d.PaymentCheques.FirstOrDefault() != null) DB.PaymentCheques.RemoveRange(d.PaymentCheques);
                        if (d.PaymentTTs.FirstOrDefault() != null) DB.PaymentTTs.RemoveRange(d.PaymentTTs);
                    }
                    else if (P.PayMode == "TT")
                    {
                        DAL.PaymentTT pTT;
                        if (d.PaymentTTs.FirstOrDefault() == null)
                        {
                            pTT = new DAL.PaymentTT();
                            d.PaymentTTs.Add(pTT);
                        }
                        else
                        {
                            pTT = d.PaymentTTs.FirstOrDefault();
                        }
                        P.PTT.toCopy<DAL.PaymentTT>(pTT);

                        if (d.PaymentCashes.FirstOrDefault() != null) DB.PaymentCashes.RemoveRange(d.PaymentCashes);
                        if (d.PaymentCheques.FirstOrDefault() != null) DB.PaymentCheques.RemoveRange(d.PaymentCheques);
                        if (d.PaymentOnlines.FirstOrDefault() != null) DB.PaymentOnlines.RemoveRange(d.PaymentOnlines);
                    }

                    if (P.PayTo == "Customer")
                    {
                        foreach (var pc in P.SRPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.PaymentCustomer pCustomer;

                            if (d.PaymentCustomers.FirstOrDefault() == null)
                            {
                                pCustomer = new DAL.PaymentCustomer();
                                d.PaymentCustomers.Add(pCustomer);
                            }
                            else
                            {
                                pCustomer = d.PaymentCustomers.Where(x => x.Id == pc.PaymentCustomerId).FirstOrDefault();
                                if (pCustomer == null)
                                {
                                    pCustomer = new DAL.PaymentCustomer();
                                    d.PaymentCustomers.Add(pCustomer);
                                }
                            }

                            P.PCustomer.toCopy<DAL.PaymentCustomer>(pCustomer);
                            pCustomer.SalesReturnId = pc.Id;
                            pCustomer.Amount = pc.PayAmount;
                            d.PaymentCustomers.Add(pCustomer);
                        }
                        if (d.PaymentCustomers.FirstOrDefault() != null)
                        {
                            var l1 = d.PaymentCustomers.Where(x => !P.SRPendingList.Where(y => y.PayAmount.Value != 0).Select(y => y.PaymentCustomerId).Contains(x.Id)).ToList();
                            if (l1 != null)
                            {
                                DB.PaymentCustomers.RemoveRange(l1);
                            }
                        }
                        if (d.PaymentSuppliers.FirstOrDefault() != null) DB.PaymentSuppliers.RemoveRange(d.PaymentSuppliers);
                        if (d.PaymentStaffs.FirstOrDefault() != null) DB.PaymentStaffs.RemoveRange(d.PaymentStaffs);
                        if (d.PaymentBanks.FirstOrDefault() != null) DB.PaymentBanks.RemoveRange(d.PaymentBanks);
                        if (d.PaymentLedgers.FirstOrDefault() != null) DB.PaymentLedgers.RemoveRange(d.PaymentLedgers);
                    }
                    else if (P.PayTo == "Supplier")
                    {
                        foreach (var pc in P.PPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.PaymentSupplier pSupplier;

                            if (d.PaymentSuppliers.FirstOrDefault() == null)
                            {
                                pSupplier = new DAL.PaymentSupplier();
                                d.PaymentSuppliers.Add(pSupplier);
                            }
                            else
                            {
                                pSupplier = d.PaymentSuppliers.Where(x => x.Id == pc.PaymentSupplierId).FirstOrDefault();
                                if (pSupplier == null)
                                {
                                    pSupplier = new DAL.PaymentSupplier();
                                    d.PaymentSuppliers.Add(pSupplier);
                                }
                            }

                            P.PSupplier.toCopy<DAL.PaymentSupplier>(pSupplier);
                            pSupplier.PurchaseId = pc.Id;
                            pSupplier.Amount = pc.PayAmount;
                            d.PaymentSuppliers.Add(pSupplier);
                        }
                        if (d.PaymentSuppliers.FirstOrDefault() != null)
                        {
                            var l1 = d.PaymentSuppliers.Where(x => !P.PPendingList.Where(y => y.PayAmount.Value != 0).Select(y => y.PaymentSupplierId).Contains(x.Id)).ToList();
                            if (l1 != null)
                            {
                                DB.PaymentSuppliers.RemoveRange(l1);
                            }
                        }
                        if (d.PaymentCustomers.FirstOrDefault() != null) DB.PaymentCustomers.RemoveRange(d.PaymentCustomers);
                        if (d.PaymentStaffs.FirstOrDefault() != null) DB.PaymentStaffs.RemoveRange(d.PaymentStaffs);
                        if (d.PaymentBanks.FirstOrDefault() != null) DB.PaymentBanks.RemoveRange(d.PaymentBanks);
                        if (d.PaymentLedgers.FirstOrDefault() != null) DB.PaymentLedgers.RemoveRange(d.PaymentLedgers);
                    }
                    else if (P.PayTo == "Staff")
                    {

                        DAL.PaymentStaff pStaff;
                        if (d.PaymentStaffs.FirstOrDefault() == null)
                        {
                            pStaff = new DAL.PaymentStaff();
                            d.PaymentStaffs.Add(pStaff);
                        }
                        else
                        {
                            pStaff = d.PaymentStaffs.FirstOrDefault();
                        }
                        P.PStaff.toCopy<DAL.PaymentStaff>(pStaff);

                        if (d.PaymentCustomers.FirstOrDefault() != null) DB.PaymentCustomers.RemoveRange(d.PaymentCustomers);
                        if (d.PaymentSuppliers.FirstOrDefault() != null) DB.PaymentSuppliers.RemoveRange(d.PaymentSuppliers);
                        if (d.PaymentBanks.FirstOrDefault() != null) DB.PaymentBanks.RemoveRange(d.PaymentBanks);
                        if (d.PaymentLedgers.FirstOrDefault() != null) DB.PaymentLedgers.RemoveRange(d.PaymentLedgers);

                    }
                    else if (P.PayTo == "Bank")
                    {
                        DAL.PaymentBank pBank;
                        if (d.PaymentBanks.FirstOrDefault() == null)
                        {
                            pBank = new DAL.PaymentBank();
                            d.PaymentBanks.Add(pBank);
                        }
                        else
                        {
                            pBank = d.PaymentBanks.FirstOrDefault();
                        }
                        P.PBank.toCopy<DAL.PaymentBank>(pBank);

                        if (d.PaymentCustomers.FirstOrDefault() != null) DB.PaymentCustomers.RemoveRange(d.PaymentCustomers);
                        if (d.PaymentSuppliers.FirstOrDefault() != null) DB.PaymentSuppliers.RemoveRange(d.PaymentSuppliers);
                        if (d.PaymentStaffs.FirstOrDefault() != null) DB.PaymentStaffs.RemoveRange(d.PaymentStaffs);
                        if (d.PaymentLedgers.FirstOrDefault() != null) DB.PaymentLedgers.RemoveRange(d.PaymentLedgers);

                    }
                    else if (P.PayTo == "Ledger")
                    {
                        DAL.PaymentLedger pLedger;
                        if (d.PaymentBanks.FirstOrDefault() == null)
                        {
                            pLedger = new DAL.PaymentLedger();
                            d.PaymentLedgers.Add(pLedger);
                        }
                        else
                        {
                            pLedger = d.PaymentLedgers.FirstOrDefault();
                        }
                        P.PLedger.toCopy<DAL.PaymentLedger>(pLedger);

                        if (d.PaymentCustomers.FirstOrDefault() != null) DB.PaymentCustomers.RemoveRange(d.PaymentCustomers);
                        if (d.PaymentSuppliers.FirstOrDefault() != null) DB.PaymentSuppliers.RemoveRange(d.PaymentSuppliers);
                        if (d.PaymentStaffs.FirstOrDefault() != null) DB.PaymentStaffs.RemoveRange(d.PaymentStaffs);
                        if (d.PaymentBanks.FirstOrDefault() != null) DB.PaymentBanks.RemoveRange(d.PaymentBanks);

                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Payment Payment_Find(string SearchText)
        {
            BLL.Payment P = new BLL.Payment();
            try
            {

                DAL.Payment d = DB.Payments.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Payment>(P);
                    #region Account From
                    if (d.PaymentCashes.FirstOrDefault() != null)
                    {
                        P.PayMode = "Cash";
                        d.PaymentCashes.FirstOrDefault().toCopy<BLL.PaymentCash>(P.PCash);
                    }
                    else if (d.PaymentCheques.FirstOrDefault() != null)
                    {
                        P.PayMode = "Cheque";
                        d.PaymentCheques.FirstOrDefault().toCopy<BLL.PaymentCheque>(P.PCheque);
                    }
                    else if (d.PaymentOnlines.FirstOrDefault() != null)
                    {
                        P.PayMode = "Online";
                        d.PaymentOnlines.FirstOrDefault().toCopy<BLL.PaymentOnline>(P.POnline);
                    }
                    else if (d.PaymentTTs.FirstOrDefault() != null)
                    {
                        P.PayMode = "TT";
                        d.PaymentTTs.FirstOrDefault().toCopy<BLL.PaymentTT>(P.PTT);
                    }
                    #endregion

                    #region Account To
                    if (d.PaymentCustomers.FirstOrDefault() != null)
                    {
                        P.PayTo = "Customer";
                        foreach (var pc in d.PaymentCustomers)
                        {
                            pc.toCopy<BLL.PaymentCustomer>(P.PCustomer);
                        }
                    }
                    else if (d.PaymentSuppliers.FirstOrDefault() != null)
                    {
                        P.PayTo = "Supplier";
                        foreach (var pc in d.PaymentSuppliers)
                        {
                            pc.toCopy<BLL.PaymentSupplier>(P.PSupplier);
                        }
                    }
                    else if (d.PaymentStaffs.FirstOrDefault() != null)
                    {
                        P.PayTo = "Staff";
                        d.PaymentStaffs.FirstOrDefault().toCopy<BLL.PaymentSupplier>(P.PSupplier);
                    }
                    else if (d.PaymentBanks.FirstOrDefault() != null)
                    {
                        P.PayTo = "Bank";
                        d.PaymentBanks.FirstOrDefault().toCopy<BLL.PaymentBank>(P.PBank);
                    }
                    else if (d.PaymentLedgers.FirstOrDefault() != null)
                    {
                        P.PayTo = "Ledger";
                        d.PaymentLedgers.FirstOrDefault().toCopy<BLL.PaymentLedger>(P.PLedger);
                    }
                    #endregion
                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool Payment_Delete(long pk)
        {
            try
            {
                DAL.Payment d = DB.Payments.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Payment P = new BLL.Payment(); d.toCopy<BLL.Payment>(P);

                    if (d.PaymentCashes.FirstOrDefault() != null) DB.PaymentCashes.RemoveRange(d.PaymentCashes);
                    if (d.PaymentCheques.FirstOrDefault() != null) DB.PaymentCheques.RemoveRange(d.PaymentCheques);
                    if (d.PaymentOnlines.FirstOrDefault() != null) DB.PaymentOnlines.RemoveRange(d.PaymentOnlines);
                    if (d.PaymentTTs.FirstOrDefault() != null) DB.PaymentTTs.RemoveRange(d.PaymentTTs);

                    if (d.PaymentCustomers.FirstOrDefault() != null) DB.PaymentCustomers.RemoveRange(d.PaymentCustomers);
                    if (d.PaymentSuppliers.FirstOrDefault() != null) DB.PaymentSuppliers.RemoveRange(d.PaymentSuppliers);
                    if (d.PaymentStaffs.FirstOrDefault() != null) DB.PaymentStaffs.RemoveRange(d.PaymentStaffs);
                    if (d.PaymentBanks.FirstOrDefault() != null) DB.PaymentBanks.RemoveRange(d.PaymentBanks);
                    if (d.PaymentLedgers.FirstOrDefault() != null) DB.PaymentLedgers.RemoveRange(d.PaymentLedgers);

                    DB.Payments.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);

                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public bool Find_PayRefNo(string RefNo, BLL.PurchaseOrder PO)

        {
            DAL.Payment d = DB.Payments.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        #endregion

        #region Receipts

        public bool Receipt_Save(BLL.Receipt P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.Receipt d = DB.Receipts.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Receipt();
                    DB.Receipts.Add(d);

                    P.toCopy<DAL.Receipt>(d);
                    if (P.PayMode == "Cash")
                    {
                        DAL.ReceiptCash pCash = new DAL.ReceiptCash();
                        d.ReceiptCashes.Add(pCash);
                    }
                    else if (P.PayMode == "Cheque")
                    {
                        DAL.ReceiptCheque pCheque = new DAL.ReceiptCheque();
                        P.PCheque.toCopy<DAL.ReceiptCheque>(pCheque);
                        d.ReceiptCheques.Add(pCheque);
                    }
                    else if (P.PayMode == "Online")
                    {
                        DAL.ReceiptOnline pOnline = new DAL.ReceiptOnline();
                        P.POnline.toCopy<DAL.ReceiptOnline>(pOnline);
                        d.ReceiptOnlines.Add(pOnline);
                    }
                    else if (P.PayMode == "TT")
                    {
                        DAL.ReceiptTT pTT = new DAL.ReceiptTT();
                        P.PTT.toCopy<DAL.ReceiptTT>(pTT);
                        d.ReceiptTTs.Add(pTT);
                    }

                    if (P.PayTo == "Customer")
                    {
                        foreach (var pc in P.SPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.ReceiptCustomer pCustomer = new DAL.ReceiptCustomer();
                            P.PCustomer.toCopy<DAL.ReceiptCustomer>(pCustomer);
                            pCustomer.SalesId = pc.Id;
                            pCustomer.Amount = pc.PayAmount;
                            d.ReceiptCustomers.Add(pCustomer);
                        }
                    }
                    else if (P.PayTo == "Supplier")
                    {
                        foreach (var pc in P.PRPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.ReceiptSupplier pSupplier = new DAL.ReceiptSupplier();
                            P.PSupplier.toCopy<DAL.ReceiptSupplier>(pSupplier);
                            pSupplier.PurchaeReturnId = pc.Id;
                            pSupplier.Amount = pc.PayAmount;
                            d.ReceiptSuppliers.Add(pSupplier);
                        }
                    }
                    else if (P.PayTo == "Staff")
                    {
                        DAL.ReceiptStaff pStaff = new DAL.ReceiptStaff();
                        P.PStaff.toCopy<DAL.ReceiptStaff>(pStaff);
                        d.ReceiptStaffs.Add(pStaff);
                    }
                    else if (P.PayTo == "Bank")
                    {
                        DAL.ReceiptBank pBank = new DAL.ReceiptBank();
                        P.PBank.toCopy<DAL.ReceiptBank>(pBank);
                        d.ReceiptBanks.Add(pBank);
                    }
                    else if (P.PayTo == "Ledger")
                    {
                        DAL.ReceiptLedger pLedger = new DAL.ReceiptLedger();
                        P.PLedger.toCopy<DAL.ReceiptLedger>(pLedger);
                        d.ReceiptLedgers.Add(pLedger);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.Receipt>(d);
                    if (P.PayMode == "Cash")
                    {
                        DAL.ReceiptCash pCash;
                        if (d.ReceiptCashes.FirstOrDefault() == null)
                        {
                            pCash = new DAL.ReceiptCash();
                            d.ReceiptCashes.Add(pCash);
                        }
                        else
                        {
                            pCash = d.ReceiptCashes.FirstOrDefault();
                        }
                        P.PCash.toCopy<DAL.ReceiptCash>(pCash);

                        if (d.ReceiptCheques.FirstOrDefault() != null) DB.ReceiptCheques.RemoveRange(d.ReceiptCheques);
                        if (d.ReceiptOnlines.FirstOrDefault() != null) DB.ReceiptOnlines.RemoveRange(d.ReceiptOnlines);
                        if (d.ReceiptTTs.FirstOrDefault() != null) DB.ReceiptTTs.RemoveRange(d.ReceiptTTs);
                    }
                    else if (P.PayMode == "Cheque")
                    {
                        DAL.ReceiptCheque pCheque;
                        if (d.ReceiptCheques.FirstOrDefault() == null)
                        {
                            pCheque = new DAL.ReceiptCheque();
                            d.ReceiptCheques.Add(pCheque);
                        }
                        else
                        {
                            pCheque = d.ReceiptCheques.FirstOrDefault();
                        }
                        P.PCheque.toCopy<DAL.ReceiptCheque>(pCheque);

                        if (d.ReceiptCashes.FirstOrDefault() != null) DB.ReceiptCashes.RemoveRange(d.ReceiptCashes);
                        if (d.ReceiptOnlines.FirstOrDefault() != null) DB.ReceiptOnlines.RemoveRange(d.ReceiptOnlines);
                        if (d.ReceiptTTs.FirstOrDefault() != null) DB.ReceiptTTs.RemoveRange(d.ReceiptTTs);
                    }
                    else if (P.PayMode == "Online")
                    {
                        DAL.ReceiptOnline pOnline;
                        if (d.ReceiptCheques.FirstOrDefault() == null)
                        {
                            pOnline = new DAL.ReceiptOnline();
                            d.ReceiptOnlines.Add(pOnline);
                        }
                        else
                        {
                            pOnline = d.ReceiptOnlines.FirstOrDefault();
                        }
                        P.POnline.toCopy<DAL.ReceiptOnline>(pOnline);

                        if (d.ReceiptCashes.FirstOrDefault() != null) DB.ReceiptCashes.RemoveRange(d.ReceiptCashes);
                        if (d.ReceiptCheques.FirstOrDefault() != null) DB.ReceiptCheques.RemoveRange(d.ReceiptCheques);
                        if (d.ReceiptTTs.FirstOrDefault() != null) DB.ReceiptTTs.RemoveRange(d.ReceiptTTs);
                    }
                    else if (P.PayMode == "TT")
                    {
                        DAL.ReceiptTT pTT;
                        if (d.ReceiptTTs.FirstOrDefault() == null)
                        {
                            pTT = new DAL.ReceiptTT();
                            d.ReceiptTTs.Add(pTT);
                        }
                        else
                        {
                            pTT = d.ReceiptTTs.FirstOrDefault();
                        }
                        P.PTT.toCopy<DAL.ReceiptTT>(pTT);

                        if (d.ReceiptCashes.FirstOrDefault() != null) DB.ReceiptCashes.RemoveRange(d.ReceiptCashes);
                        if (d.ReceiptCheques.FirstOrDefault() != null) DB.ReceiptCheques.RemoveRange(d.ReceiptCheques);
                        if (d.ReceiptOnlines.FirstOrDefault() != null) DB.ReceiptOnlines.RemoveRange(d.ReceiptOnlines);
                    }

                    if (P.PayTo == "Customer")
                    {
                        foreach (var pc in P.SPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.ReceiptCustomer pCustomer;

                            if (d.ReceiptCustomers.FirstOrDefault() == null)
                            {
                                pCustomer = new DAL.ReceiptCustomer();
                                d.ReceiptCustomers.Add(pCustomer);
                            }
                            else
                            {
                                pCustomer = d.ReceiptCustomers.Where(x => x.Id == pc.ReceiptCustomerId).FirstOrDefault();
                                if (pCustomer == null)
                                {
                                    pCustomer = new DAL.ReceiptCustomer();
                                    d.ReceiptCustomers.Add(pCustomer);
                                }
                            }

                            P.PCustomer.toCopy<DAL.ReceiptCustomer>(pCustomer);
                            pCustomer.SalesId = pc.Id;
                            pCustomer.Amount = pc.PayAmount;
                            d.ReceiptCustomers.Add(pCustomer);
                        }
                        if (d.ReceiptCustomers.FirstOrDefault() != null)
                        {
                            var l1 = d.ReceiptCustomers.Where(x => !P.SPendingList.Where(y => y.PayAmount.Value != 0).Select(y => y.ReceiptCustomerId).Contains(x.Id)).ToList();
                            if (l1 != null)
                            {
                                DB.ReceiptCustomers.RemoveRange(l1);
                            }
                        }
                        if (d.ReceiptSuppliers.FirstOrDefault() != null) DB.ReceiptSuppliers.RemoveRange(d.ReceiptSuppliers);
                        if (d.ReceiptStaffs.FirstOrDefault() != null) DB.ReceiptStaffs.RemoveRange(d.ReceiptStaffs);
                        if (d.ReceiptBanks.FirstOrDefault() != null) DB.ReceiptBanks.RemoveRange(d.ReceiptBanks);
                        if (d.ReceiptLedgers.FirstOrDefault() != null) DB.ReceiptLedgers.RemoveRange(d.ReceiptLedgers);
                    }
                    else if (P.PayTo == "Supplier")
                    {
                        foreach (var pc in P.PRPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.ReceiptSupplier pSupplier;

                            if (d.ReceiptSuppliers.FirstOrDefault() == null)
                            {
                                pSupplier = new DAL.ReceiptSupplier();
                                d.ReceiptSuppliers.Add(pSupplier);
                            }
                            else
                            {
                                pSupplier = d.ReceiptSuppliers.Where(x => x.Id == pc.ReceiptSupplierId).FirstOrDefault();
                                if (pSupplier == null)
                                {
                                    pSupplier = new DAL.ReceiptSupplier();
                                    d.ReceiptSuppliers.Add(pSupplier);
                                }
                            }

                            P.PSupplier.toCopy<DAL.ReceiptSupplier>(pSupplier);
                            pSupplier.PurchaeReturnId = pc.Id;
                            pSupplier.Amount = pc.PayAmount;
                            d.ReceiptSuppliers.Add(pSupplier);
                        }
                        if (d.ReceiptSuppliers.FirstOrDefault() != null)
                        {
                            var l1 = d.ReceiptSuppliers.Where(x => !P.PRPendingList.Where(y => y.PayAmount.Value != 0).Select(y => y.ReceiptSupplierId).Contains(x.Id)).ToList();
                            if (l1 != null)
                            {
                                DB.ReceiptSuppliers.RemoveRange(l1);
                            }
                        }
                        if (d.ReceiptCustomers.FirstOrDefault() != null) DB.ReceiptCustomers.RemoveRange(d.ReceiptCustomers);
                        if (d.ReceiptStaffs.FirstOrDefault() != null) DB.ReceiptStaffs.RemoveRange(d.ReceiptStaffs);
                        if (d.ReceiptBanks.FirstOrDefault() != null) DB.ReceiptBanks.RemoveRange(d.ReceiptBanks);
                        if (d.ReceiptLedgers.FirstOrDefault() != null) DB.ReceiptLedgers.RemoveRange(d.ReceiptLedgers);
                    }
                    else if (P.PayTo == "Staff")
                    {

                        DAL.ReceiptStaff pStaff;
                        if (d.ReceiptStaffs.FirstOrDefault() == null)
                        {
                            pStaff = new DAL.ReceiptStaff();
                            d.ReceiptStaffs.Add(pStaff);
                        }
                        else
                        {
                            pStaff = d.ReceiptStaffs.FirstOrDefault();
                        }
                        P.PStaff.toCopy<DAL.ReceiptStaff>(pStaff);

                        if (d.ReceiptCustomers.FirstOrDefault() != null) DB.ReceiptCustomers.RemoveRange(d.ReceiptCustomers);
                        if (d.ReceiptSuppliers.FirstOrDefault() != null) DB.ReceiptSuppliers.RemoveRange(d.ReceiptSuppliers);
                        if (d.ReceiptBanks.FirstOrDefault() != null) DB.ReceiptBanks.RemoveRange(d.ReceiptBanks);
                        if (d.ReceiptLedgers.FirstOrDefault() != null) DB.ReceiptLedgers.RemoveRange(d.ReceiptLedgers);

                    }
                    else if (P.PayTo == "Bank")
                    {
                        DAL.ReceiptBank pBank;
                        if (d.ReceiptBanks.FirstOrDefault() == null)
                        {
                            pBank = new DAL.ReceiptBank();
                            d.ReceiptBanks.Add(pBank);
                        }
                        else
                        {
                            pBank = d.ReceiptBanks.FirstOrDefault();
                        }
                        P.PBank.toCopy<DAL.ReceiptBank>(pBank);

                        if (d.ReceiptCustomers.FirstOrDefault() != null) DB.ReceiptCustomers.RemoveRange(d.ReceiptCustomers);
                        if (d.ReceiptSuppliers.FirstOrDefault() != null) DB.ReceiptSuppliers.RemoveRange(d.ReceiptSuppliers);
                        if (d.ReceiptStaffs.FirstOrDefault() != null) DB.ReceiptStaffs.RemoveRange(d.ReceiptStaffs);
                        if (d.ReceiptLedgers.FirstOrDefault() != null) DB.ReceiptLedgers.RemoveRange(d.ReceiptLedgers);

                    }
                    else if (P.PayTo == "Ledger")
                    {
                        DAL.ReceiptLedger pLedger;
                        if (d.ReceiptBanks.FirstOrDefault() == null)
                        {
                            pLedger = new DAL.ReceiptLedger();
                            d.ReceiptLedgers.Add(pLedger);
                        }
                        else
                        {
                            pLedger = d.ReceiptLedgers.FirstOrDefault();
                        }
                        P.PLedger.toCopy<DAL.ReceiptLedger>(pLedger);

                        if (d.ReceiptCustomers.FirstOrDefault() != null) DB.ReceiptCustomers.RemoveRange(d.ReceiptCustomers);
                        if (d.ReceiptSuppliers.FirstOrDefault() != null) DB.ReceiptSuppliers.RemoveRange(d.ReceiptSuppliers);
                        if (d.ReceiptStaffs.FirstOrDefault() != null) DB.ReceiptStaffs.RemoveRange(d.ReceiptStaffs);
                        if (d.ReceiptBanks.FirstOrDefault() != null) DB.ReceiptBanks.RemoveRange(d.ReceiptBanks);

                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Receipt Receipt_Find(string SearchText)
        {
            BLL.Receipt P = new BLL.Receipt();
            try
            {

                DAL.Receipt d = DB.Receipts.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Receipt>(P);
                    #region Account From
                    if (d.ReceiptCashes.FirstOrDefault() != null)
                    {
                        P.PayMode = "Cash";
                        d.ReceiptCashes.FirstOrDefault().toCopy<BLL.ReceiptCash>(P.PCash);
                    }
                    else if (d.ReceiptCheques.FirstOrDefault() != null)
                    {
                        P.PayMode = "Cheque";
                        d.ReceiptCheques.FirstOrDefault().toCopy<BLL.ReceiptCheque>(P.PCheque);
                    }
                    else if (d.ReceiptOnlines.FirstOrDefault() != null)
                    {
                        P.PayMode = "Online";
                        d.ReceiptOnlines.FirstOrDefault().toCopy<BLL.ReceiptOnline>(P.POnline);
                    }
                    else if (d.ReceiptTTs.FirstOrDefault() != null)
                    {
                        P.PayMode = "TT";
                        d.ReceiptTTs.FirstOrDefault().toCopy<BLL.ReceiptTT>(P.PTT);
                    }
                    #endregion

                    #region Account To
                    if (d.ReceiptCustomers.FirstOrDefault() != null)
                    {
                        P.PayTo = "Customer";
                        foreach (var pc in d.ReceiptCustomers)
                        {
                            pc.toCopy<BLL.ReceiptCustomer>(P.PCustomer);
                        }
                    }
                    else if (d.ReceiptSuppliers.FirstOrDefault() != null)
                    {
                        P.PayTo = "Supplier";
                        foreach (var pc in d.ReceiptSuppliers)
                        {
                            pc.toCopy<BLL.ReceiptSupplier>(P.PSupplier);
                        }
                    }
                    else if (d.ReceiptStaffs.FirstOrDefault() != null)
                    {
                        P.PayTo = "Staff";
                        d.ReceiptStaffs.FirstOrDefault().toCopy<BLL.ReceiptSupplier>(P.PSupplier);
                    }
                    else if (d.ReceiptBanks.FirstOrDefault() != null)
                    {
                        P.PayTo = "Bank";
                        d.ReceiptBanks.FirstOrDefault().toCopy<BLL.ReceiptBank>(P.PBank);
                    }
                    else if (d.ReceiptLedgers.FirstOrDefault() != null)
                    {
                        P.PayTo = "Ledger";
                        d.ReceiptLedgers.FirstOrDefault().toCopy<BLL.ReceiptLedger>(P.PLedger);
                    }
                    #endregion
                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool Receipt_Delete(long pk)
        {
            try
            {
                DAL.Receipt d = DB.Receipts.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Receipt P = new BLL.Receipt();
                    d.toCopy<BLL.Receipt>(P);

                    if (d.ReceiptCashes.FirstOrDefault() != null) DB.ReceiptCashes.RemoveRange(d.ReceiptCashes);
                    if (d.ReceiptCheques.FirstOrDefault() != null) DB.ReceiptCheques.RemoveRange(d.ReceiptCheques);
                    if (d.ReceiptOnlines.FirstOrDefault() != null) DB.ReceiptOnlines.RemoveRange(d.ReceiptOnlines);
                    if (d.ReceiptTTs.FirstOrDefault() != null) DB.ReceiptTTs.RemoveRange(d.ReceiptTTs);

                    if (d.ReceiptCustomers.FirstOrDefault() != null) DB.ReceiptCustomers.RemoveRange(d.ReceiptCustomers);
                    if (d.ReceiptSuppliers.FirstOrDefault() != null) DB.ReceiptSuppliers.RemoveRange(d.ReceiptSuppliers);
                    if (d.ReceiptStaffs.FirstOrDefault() != null) DB.ReceiptStaffs.RemoveRange(d.ReceiptStaffs);
                    if (d.ReceiptBanks.FirstOrDefault() != null) DB.ReceiptBanks.RemoveRange(d.ReceiptBanks);
                    if (d.ReceiptLedgers.FirstOrDefault() != null) DB.ReceiptLedgers.RemoveRange(d.ReceiptLedgers);

                    DB.Receipts.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);

                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        #endregion

        #region Journal

        public bool Journal_Save(BLL.Journal P)
        {
            try
            {
                P.CompanyId = Caller.CompanyId;

                DAL.Journal d = DB.Journals.Where(x => x.Id == P.Id).FirstOrDefault();

                if (d == null)
                {

                    d = new DAL.Journal();
                    DB.Journals.Add(d);

                    P.toCopy<DAL.Journal>(d);
                    if (P.TMode == "Cash")
                    {
                        DAL.JournalCash pCash = new DAL.JournalCash();
                        d.JournalCashes.Add(pCash);
                    }
                    else if (P.TMode == "Cheque")
                    {
                        DAL.JournalCheque jCheque = new DAL.JournalCheque();
                        P.JCheque.toCopy<DAL.JournalCheque>(jCheque);
                        d.JournalCheques.Add(jCheque);
                    }
                    else if (P.TMode == "Online")
                    {
                        DAL.JournalOnline pOnline = new DAL.JournalOnline();
                        P.JOnline.toCopy<DAL.JournalOnline>(pOnline);
                        d.JournalOnlines.Add(pOnline);
                    }
                    else if (P.TMode == "TT")
                    {
                        DAL.JournalTT pTT = new DAL.JournalTT();
                        P.JTT.toCopy<DAL.JournalTT>(pTT);
                        d.JournalTTs.Add(pTT);
                    }

                    if (P.To == "Customer")
                    {
                        foreach (var pc in P.SRPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.JournalCustomer pCustomer = new DAL.JournalCustomer();
                            P.JCustomer.toCopy<DAL.JournalCustomer>(pCustomer);
                            pCustomer.SalesReturnId = pc.Id;
                            pCustomer.Amount = pc.PayAmount;
                            d.JournalCustomers.Add(pCustomer);
                        }
                    }
                    else if (P.To == "Supplier")
                    {
                        foreach (var pc in P.PPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.JournalSupplier pSupplier = new DAL.JournalSupplier();
                            P.JSupplier.toCopy<DAL.JournalSupplier>(pSupplier);
                            pSupplier.PurchaseId = pc.Id;
                            pSupplier.Amount = pc.PayAmount;
                            d.JournalSuppliers.Add(pSupplier);
                        }
                    }
                    else if (P.To == "Staff")
                    {
                        DAL.JournalStaff pStaff = new DAL.JournalStaff();
                        P.JStaff.toCopy<DAL.JournalStaff>(pStaff);
                        d.JournalStaffs.Add(pStaff);
                    }
                    else if (P.To == "Bank")
                    {
                        DAL.JournalBank pBank = new DAL.JournalBank();
                        P.JBank.toCopy<DAL.JournalBank>(pBank);
                        d.JournalBanks.Add(pBank);
                    }
                    else if (P.To == "Ledger")
                    {
                        DAL.JournalLedger pLedger = new DAL.JournalLedger();
                        P.JLedger.toCopy<DAL.JournalLedger>(pLedger);
                        d.JournalLedgers.Add(pLedger);
                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.INSERT);
                }
                else
                {
                    P.toCopy<DAL.Journal>(d);
                    if (P.TMode == "Cash")
                    {
                        DAL.JournalCash pCash;
                        if (d.JournalCashes.FirstOrDefault() == null)
                        {
                            pCash = new DAL.JournalCash();
                            d.JournalCashes.Add(pCash);
                        }
                        else
                        {
                            pCash = d.JournalCashes.FirstOrDefault();
                        }
                        P.JCash.toCopy<DAL.JournalCash>(pCash);

                        if (d.JournalCheques.FirstOrDefault() != null) DB.JournalCheques.RemoveRange(d.JournalCheques);
                        if (d.JournalOnlines.FirstOrDefault() != null) DB.JournalOnlines.RemoveRange(d.JournalOnlines);
                        if (d.JournalTTs.FirstOrDefault() != null) DB.JournalTTs.RemoveRange(d.JournalTTs);
                    }
                    else if (P.TMode == "Cheque")
                    {
                        DAL.JournalCheque pCheque;
                        if (d.JournalCheques.FirstOrDefault() == null)
                        {
                            pCheque = new DAL.JournalCheque();
                            d.JournalCheques.Add(pCheque);
                        }
                        else
                        {
                            pCheque = d.JournalCheques.FirstOrDefault();
                        }
                        P.JCheque.toCopy<DAL.JournalCheque>(pCheque);

                        if (d.JournalCashes.FirstOrDefault() != null) DB.JournalCashes.RemoveRange(d.JournalCashes);
                        if (d.JournalOnlines.FirstOrDefault() != null) DB.JournalOnlines.RemoveRange(d.JournalOnlines);
                        if (d.JournalTTs.FirstOrDefault() != null) DB.JournalTTs.RemoveRange(d.JournalTTs);
                    }
                    else if (P.TMode == "Online")
                    {
                        DAL.JournalOnline pOnline;
                        if (d.JournalCheques.FirstOrDefault() == null)
                        {
                            pOnline = new DAL.JournalOnline();
                            d.JournalOnlines.Add(pOnline);
                        }
                        else
                        {
                            pOnline = d.JournalOnlines.FirstOrDefault();
                        }
                        P.JOnline.toCopy<DAL.JournalOnline>(pOnline);

                        if (d.JournalCashes.FirstOrDefault() != null) DB.JournalCashes.RemoveRange(d.JournalCashes);
                        if (d.JournalCheques.FirstOrDefault() != null) DB.JournalCheques.RemoveRange(d.JournalCheques);
                        if (d.JournalTTs.FirstOrDefault() != null) DB.JournalTTs.RemoveRange(d.JournalTTs);
                    }
                    else if (P.TMode == "TT")
                    {
                        DAL.JournalTT pTT;
                        if (d.JournalTTs.FirstOrDefault() == null)
                        {
                            pTT = new DAL.JournalTT();
                            d.JournalTTs.Add(pTT);
                        }
                        else
                        {
                            pTT = d.JournalTTs.FirstOrDefault();
                        }
                        P.JTT.toCopy<DAL.JournalTT>(pTT);

                        if (d.JournalCashes.FirstOrDefault() != null) DB.JournalCashes.RemoveRange(d.JournalCashes);
                        if (d.JournalCheques.FirstOrDefault() != null) DB.JournalCheques.RemoveRange(d.JournalCheques);
                        if (d.JournalOnlines.FirstOrDefault() != null) DB.JournalOnlines.RemoveRange(d.JournalOnlines);
                    }

                    if (P.To == "Customer")
                    {
                        foreach (var pc in P.SRPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.JournalCustomer pCustomer;

                            if (d.JournalCustomers.FirstOrDefault() == null)
                            {
                                pCustomer = new DAL.JournalCustomer();
                                d.JournalCustomers.Add(pCustomer);
                            }
                            else
                            {
                                pCustomer = d.JournalCustomers.Where(x => x.Id == pc.CustomerId).FirstOrDefault();
                                if (pCustomer == null)
                                {
                                    pCustomer = new DAL.JournalCustomer();
                                    d.JournalCustomers.Add(pCustomer);
                                }
                            }

                            P.JCustomer.toCopy<DAL.JournalCustomer>(pCustomer);
                            pCustomer.SalesReturnId = pc.Id;
                            pCustomer.Amount = pc.PayAmount;
                            d.JournalCustomers.Add(pCustomer);
                        }
                        if (d.JournalCustomers.FirstOrDefault() != null)
                        {
                            //var l1 = d.JournalCustomers.Where(x => !P.SRPendingList.Where(y => y.PayAmount.Value != 0).Select(y => y.CustomerId).Contains(x.Id)).ToList();
                            //if (l1 != null)
                            //{
                            //    DB.JournalCustomers.RemoveRange(l1);
                            //}
                        }
                        if (d.JournalSuppliers.FirstOrDefault() != null) DB.JournalSuppliers.RemoveRange(d.JournalSuppliers);
                        if (d.JournalStaffs.FirstOrDefault() != null) DB.JournalStaffs.RemoveRange(d.JournalStaffs);
                        if (d.JournalBanks.FirstOrDefault() != null) DB.JournalBanks.RemoveRange(d.JournalBanks);
                        if (d.JournalLedgers.FirstOrDefault() != null) DB.JournalLedgers.RemoveRange(d.JournalLedgers);
                    }
                    else if (P.To == "Supplier")
                    {
                        foreach (var pc in P.PPendingList.Where(x => x.PayAmount != 0).ToList())
                        {
                            DAL.JournalSupplier pSupplier;

                            if (d.JournalSuppliers.FirstOrDefault() == null)
                            {
                                pSupplier = new DAL.JournalSupplier();
                                d.JournalSuppliers.Add(pSupplier);
                            }
                            else
                            {
                                pSupplier = d.JournalSuppliers.Where(x => x.Id == pc.SupplierId).FirstOrDefault();
                                if (pSupplier == null)
                                {
                                    pSupplier = new DAL.JournalSupplier();
                                    d.JournalSuppliers.Add(pSupplier);
                                }
                            }

                            P.JSupplier.toCopy<DAL.JournalSupplier>(pSupplier);
                            pSupplier.PurchaseId = pc.Id;
                            pSupplier.Amount = pc.PayAmount;
                            d.JournalSuppliers.Add(pSupplier);
                        }
                        if (d.JournalSuppliers.FirstOrDefault() != null)
                        {
                            //var l1 = d.JournalSuppliers.Where(x => !P.PPendingList.Where(y => y.PayAmount.Value != 0).Select(y => y.SupplierId).Contains(x.Id)).ToList();
                            //if (l1 != null)
                            //{
                            //    DB.JournalSuppliers.RemoveRange(l1);
                            //}
                        }
                        if (d.JournalCustomers.FirstOrDefault() != null) DB.JournalCustomers.RemoveRange(d.JournalCustomers);
                        if (d.JournalStaffs.FirstOrDefault() != null) DB.JournalStaffs.RemoveRange(d.JournalStaffs);
                        if (d.JournalBanks.FirstOrDefault() != null) DB.JournalBanks.RemoveRange(d.JournalBanks);
                        if (d.JournalLedgers.FirstOrDefault() != null) DB.JournalLedgers.RemoveRange(d.JournalLedgers);
                    }
                    else if (P.To == "Staff")
                    {

                        DAL.JournalStaff pStaff;
                        if (d.JournalStaffs.FirstOrDefault() == null)
                        {
                            pStaff = new DAL.JournalStaff();
                            d.JournalStaffs.Add(pStaff);
                        }
                        else
                        {
                            pStaff = d.JournalStaffs.FirstOrDefault();
                        }
                        P.JStaff.toCopy<DAL.JournalStaff>(pStaff);

                        if (d.JournalCustomers.FirstOrDefault() != null) DB.JournalCustomers.RemoveRange(d.JournalCustomers);
                        if (d.JournalSuppliers.FirstOrDefault() != null) DB.JournalSuppliers.RemoveRange(d.JournalSuppliers);
                        if (d.JournalBanks.FirstOrDefault() != null) DB.JournalBanks.RemoveRange(d.JournalBanks);
                        if (d.JournalLedgers.FirstOrDefault() != null) DB.JournalLedgers.RemoveRange(d.JournalLedgers);

                    }
                    else if (P.To == "Bank")
                    {
                        DAL.JournalBank pBank;
                        if (d.JournalBanks.FirstOrDefault() == null)
                        {
                            pBank = new DAL.JournalBank();
                            d.JournalBanks.Add(pBank);
                        }
                        else
                        {
                            pBank = d.JournalBanks.FirstOrDefault();
                        }
                        P.JBank.toCopy<DAL.JournalBank>(pBank);

                        if (d.JournalCustomers.FirstOrDefault() != null) DB.JournalCustomers.RemoveRange(d.JournalCustomers);
                        if (d.JournalSuppliers.FirstOrDefault() != null) DB.JournalSuppliers.RemoveRange(d.JournalSuppliers);
                        if (d.JournalStaffs.FirstOrDefault() != null) DB.JournalStaffs.RemoveRange(d.JournalStaffs);
                        if (d.JournalLedgers.FirstOrDefault() != null) DB.JournalLedgers.RemoveRange(d.JournalLedgers);

                    }
                    else if (P.To == "Ledger")
                    {
                        DAL.JournalLedger pLedger;
                        if (d.JournalBanks.FirstOrDefault() == null)
                        {
                            pLedger = new DAL.JournalLedger();
                            d.JournalLedgers.Add(pLedger);
                        }
                        else
                        {
                            pLedger = d.JournalLedgers.FirstOrDefault();
                        }
                        P.JLedger.toCopy<DAL.JournalLedger>(pLedger);

                        if (d.JournalCustomers.FirstOrDefault() != null) DB.JournalCustomers.RemoveRange(d.JournalCustomers);
                        if (d.JournalSuppliers.FirstOrDefault() != null) DB.JournalSuppliers.RemoveRange(d.JournalSuppliers);
                        if (d.JournalStaffs.FirstOrDefault() != null) DB.JournalStaffs.RemoveRange(d.JournalStaffs);
                        if (d.JournalBanks.FirstOrDefault() != null) DB.JournalBanks.RemoveRange(d.JournalBanks);

                    }
                    DB.SaveChanges();

                    LogDetailStore(P, LogDetailType.UPDATE);
                }

                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public BLL.Journal Journal_Find(string SearchText)
        {
            BLL.Journal P = new BLL.Journal();
            try
            {

                DAL.Journal d = DB.Journals.Where(x => x.RefNo == SearchText).FirstOrDefault();
                DB.Entry(d).Reload();
                if (d != null)
                {

                    d.toCopy<BLL.Journal>(P);
                    #region Account From
                    if (d.JournalCashes.FirstOrDefault() != null)
                    {
                        P.TMode = "Cash";
                        d.JournalCashes.FirstOrDefault().toCopy<BLL.JournalCash>(P.JCash);
                    }
                    else if (d.JournalCheques.FirstOrDefault() != null)
                    {
                        P.TMode = "Cheque";
                        d.JournalCheques.FirstOrDefault().toCopy<BLL.JournalCheque>(P.JCheque);
                    }
                    else if (d.JournalOnlines.FirstOrDefault() != null)
                    {
                        P.TMode = "Online";
                        d.JournalOnlines.FirstOrDefault().toCopy<BLL.JournalOnline>(P.JOnline);
                    }
                    else if (d.JournalTTs.FirstOrDefault() != null)
                    {
                        P.TMode = "TT";
                        d.JournalTTs.FirstOrDefault().toCopy<BLL.JournalTT>(P.JTT);
                    }
                    #endregion

                    #region Account To
                    if (d.JournalCustomers.FirstOrDefault() != null)
                    {
                        P.To = "Customer";
                        foreach (var pc in d.JournalCustomers)
                        {
                            pc.toCopy<BLL.JournalCustomer>(P.JCustomer);
                        }
                    }
                    else if (d.JournalSuppliers.FirstOrDefault() != null)
                    {
                        P.To = "Supplier";
                        foreach (var pc in d.JournalSuppliers)
                        {
                            pc.toCopy<BLL.JournalSupplier>(P.JSupplier);
                        }
                    }
                    else if (d.JournalStaffs.FirstOrDefault() != null)
                    {
                        P.To = "Staff";
                        d.JournalStaffs.FirstOrDefault().toCopy<BLL.JournalSupplier>(P.JSupplier);
                    }
                    else if (d.JournalBanks.FirstOrDefault() != null)
                    {
                        P.To = "Bank";
                        d.JournalBanks.FirstOrDefault().toCopy<BLL.JournalBank>(P.JBank);
                    }
                    else if (d.JournalLedgers.FirstOrDefault() != null)
                    {
                        P.To = "Ledger";
                        d.JournalLedgers.FirstOrDefault().toCopy<BLL.JournalLedger>(P.JLedger);
                    }
                    #endregion
                }
            }
            catch (Exception ex) { }
            return P;
        }

        public bool Journal_Delete(long pk)
        {
            try
            {
                DAL.Journal d = DB.Journals.Where(x => x.Id == pk).FirstOrDefault();

                if (d != null)
                {
                    BLL.Journal P = new BLL.Journal(); d.toCopy<BLL.Journal>(P);

                    if (d.JournalCashes.FirstOrDefault() != null) DB.JournalCashes.RemoveRange(d.JournalCashes);
                    if (d.JournalCheques.FirstOrDefault() != null) DB.JournalCheques.RemoveRange(d.JournalCheques);
                    if (d.JournalOnlines.FirstOrDefault() != null) DB.JournalOnlines.RemoveRange(d.JournalOnlines);
                    if (d.JournalTTs.FirstOrDefault() != null) DB.JournalTTs.RemoveRange(d.JournalTTs);

                    if (d.JournalCustomers.FirstOrDefault() != null) DB.JournalCustomers.RemoveRange(d.JournalCustomers);
                    if (d.JournalSuppliers.FirstOrDefault() != null) DB.JournalSuppliers.RemoveRange(d.JournalSuppliers);
                    if (d.JournalStaffs.FirstOrDefault() != null) DB.JournalStaffs.RemoveRange(d.JournalStaffs);
                    if (d.JournalBanks.FirstOrDefault() != null) DB.JournalBanks.RemoveRange(d.JournalBanks);
                    if (d.JournalLedgers.FirstOrDefault() != null) DB.JournalLedgers.RemoveRange(d.JournalLedgers);

                    DB.Journals.Remove(d);
                    DB.SaveChanges();
                    LogDetailStore(P, LogDetailType.DELETE);

                }
                return true;
            }
            catch (Exception ex) { }
            return false;
        }

        public bool Find_JournalRefNo(string RefNo, BLL.PurchaseOrder PO)

        {
            DAL.Journal d = DB.Journals.Where(x => x.RefNo == RefNo & x.Id != PO.Id).FirstOrDefault();
            if (d == null)
            {
                return false;
            }
            else
            {
                return true;
            }

        }


        #endregion

        #endregion

        #region Reports
        

        #region Trial_Balance

        public List<BLL.TrialBalance> TrialBalance_List()
        {
            List<BLL.TrialBalance> lstTrialBalance = new List<BLL.TrialBalance>();
            BLL.TrialBalance tb = new BLL.TrialBalance();

            var l1 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var C1 = DB.Customers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var S1 = DB.Suppliers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var B1 = DB.Banks.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            decimal TotDr = 0, TotCr = 0;

            #region Ledger
            foreach (var l in l1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = l.LedgerName;
                tb.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                tb.DrAmt = l.PaymentLedgers.Sum(x => x.Payment.Amount);
                tb.CrAmt = l.ReceiptLedgers.Sum(x => x.Receipt.Amount);

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = 0;
                }

                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    lstTrialBalance.Add(tb);
                    TotDr += tb.DrAmt.Value;
                    TotCr += tb.CrAmt.Value;
                }
            }
            #endregion

            #region Customer
            foreach (var c in C1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = c.CustomerName;
                tb.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                tb.CrAmt = c.PaymentCustomers.Sum(x => x.Payment.Amount);
                tb.DrAmt = c.ReceiptCustomers.Sum(x => x.Receipt.Amount);

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = tb.DrAmt;
                }

                if (tb.CrAmt != 0 || tb.DrAmt != 0)
                {
                    lstTrialBalance.Add(tb);
                    TotCr += tb.CrAmt.Value;
                    TotDr += tb.DrAmt.Value;
                }
            }
            #endregion

            #region Supplier
            foreach (var c in S1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = c.SupplierName;
                tb.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                tb.CrAmt = c.PaymentSuppliers.Sum(x => x.Payment.Amount);
                tb.DrAmt = c.ReceiptSuppliers.Sum(x => x.Receipt.Amount);

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = tb.DrAmt;
                }

                if (tb.CrAmt != 0 || tb.DrAmt != 0)
                {
                    lstTrialBalance.Add(tb);
                    TotCr += tb.CrAmt.Value;
                    TotDr += tb.DrAmt.Value;
                }
            }
            #endregion

            #region Bank    
            foreach (var c in B1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = c.BankName;
                tb.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                tb.CrAmt = c.PaymentBanks.Sum(x => x.Payment.Amount);
                tb.DrAmt = c.ReceiptBanks.Sum(x => x.Receipt.Amount);

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = tb.DrAmt;
                }

                if (tb.CrAmt != 0 || tb.DrAmt != 0)
                {
                    lstTrialBalance.Add(tb);
                    TotCr += tb.CrAmt.Value;
                    TotDr += tb.DrAmt.Value;
                }
            }
            #endregion

            tb = new BLL.TrialBalance();
            tb.LedgerName = "Total ";
            tb.DrAmt = TotDr;
            tb.CrAmt = TotCr;
            lstTrialBalance.Add(tb);


            return lstTrialBalance;
        }

        #endregion

        #region Balance_Sheet

        public List<BLL.BalanceSheet> Balance_List()
        {
            List<BLL.BalanceSheet> lstBalanceSheet = new List<BLL.BalanceSheet>();
            BLL.BalanceSheet bal = new BLL.BalanceSheet();

            #region Assets
            var l2 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Assets").ToList();
            var C2 = DB.Customers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Assets").ToList();
            var S2 = DB.Suppliers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Assets").ToList();
            var B2 = DB.Banks.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Assets").ToList();

            decimal ToAssDr = 0, ToAssCr = 0;
            bal.LedgerName = "Assets";
            lstBalanceSheet.Add(bal);

            #region Ledger
            foreach (var l in l2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", l.LedgerName);
                bal.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                bal.DrAmt = l.PaymentLedgers.Sum(x => x.Payment.Amount);
                bal.CrAmt = l.ReceiptLedgers.Sum(x => x.Receipt.Amount);
                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = 0;
                }

                if (bal.DrAmt != 0 || bal.CrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    ToAssDr += bal.DrAmt.Value;
                    ToAssCr += bal.CrAmt.Value;
                }
            }
            #endregion

            #region Customer
            foreach (var c in C2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.CustomerName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentCustomers.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptCustomers.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    ToAssCr += bal.CrAmt.Value;
                    ToAssDr += bal.DrAmt.Value;
                }
            }
            #endregion

            #region Supplier
            foreach (var c in S2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.SupplierName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentSuppliers.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptSuppliers.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    ToAssCr += bal.CrAmt.Value;
                    ToAssDr += bal.DrAmt.Value;
                }
            }
            #endregion

            #region Bank    
            foreach (var c in B2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.BankName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentBanks.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptBanks.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    ToAssCr += bal.CrAmt.Value;
                    ToAssDr += bal.DrAmt.Value;
                }
            }
            #endregion

            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Total Assets";
            bal.DrAmt = ToAssDr;
            bal.CrAmt = ToAssCr;
            lstBalanceSheet.Add(bal);

            #endregion

            #region Liabilities
            var l1 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Liabilities" || x.AccountGroup.GroupName == "Assets").ToList();
            var C1 = DB.Customers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Liabilities" || x.AccountGroup.GroupName == "Assets").ToList();
            var S1 = DB.Suppliers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Liabilities" || x.AccountGroup.GroupName == "Assets").ToList();
            var B1 = DB.Banks.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Liabilities" || x.AccountGroup.GroupName == "Assets").ToList();

            decimal TotDr = 0, TotCr = 0;
            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Liabilities";

            lstBalanceSheet.Add(bal);

            #region Ledger
            foreach (var l in l1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", l.LedgerName);
                bal.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                bal.DrAmt = l.PaymentLedgers.Sum(x => x.Payment.Amount);
                bal.CrAmt = l.ReceiptLedgers.Sum(x => x.Receipt.Amount);
                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = 0;
                }

                if (bal.DrAmt != 0 || bal.CrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    TotDr += bal.DrAmt.Value;
                    TotCr += bal.CrAmt.Value;
                }
            }
            #endregion

            #region Customer
            foreach (var c in C1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.CustomerName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentCustomers.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptCustomers.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    TotCr += bal.CrAmt.Value;
                    TotDr += bal.DrAmt.Value;
                }
            }
            #endregion

            #region Supplier
            foreach (var c in S1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.SupplierName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentSuppliers.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptSuppliers.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    TotCr += bal.CrAmt.Value;
                    TotDr += bal.DrAmt.Value;
                }
            }
            #endregion

            #region Bank    
            foreach (var c in B1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.BankName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentBanks.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptBanks.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstBalanceSheet.Add(bal);
                    TotCr += bal.CrAmt.Value;
                    TotDr += bal.DrAmt.Value;
                }
            }
            #endregion

            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Total Liabilities";
            bal.DrAmt = TotDr;
            bal.CrAmt = TotCr;
            lstBalanceSheet.Add(bal);

            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Total";
            bal.DrAmt = TotDr + ToAssDr;
            bal.CrAmt = TotCr + ToAssCr;
            lstBalanceSheet.Add(bal);

            #endregion

            return lstBalanceSheet;
        }

        #endregion

        #region Payment_Receipt
        public List<BLL.TrialBalance> PRList()
        {
            List<BLL.TrialBalance> lstPaymentReceipt = new List<BLL.TrialBalance>();
            BLL.TrialBalance tb = new BLL.TrialBalance();

            var l1 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var C1 = DB.Customers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var S1 = DB.Suppliers.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            var B1 = DB.Banks.Where(x => x.CompanyId == Caller.CompanyId).ToList();
            decimal TotDr = 0, TotCr = 0;

            #region Ledger
            foreach (var l in l1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = l.LedgerName;
                tb.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                tb.DrAmt = l.PaymentLedgers.Sum(x => x.Payment.Amount);
                tb.CrAmt = l.ReceiptLedgers.Sum(x => x.Receipt.Amount);

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = 0;
                }

                if (tb.DrAmt != 0 || tb.CrAmt != 0)
                {
                    lstPaymentReceipt.Add(tb);
                    TotDr += tb.DrAmt.Value;
                    TotCr += tb.CrAmt.Value;
                }
            }
            #endregion

            #region Customer
            foreach (var c in C1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = c.CustomerName;
                tb.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                tb.CrAmt = c.PaymentCustomers.Sum(x => x.Payment.Amount);
                tb.DrAmt = c.ReceiptCustomers.Sum(x => x.Receipt.Amount);

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = tb.DrAmt;
                }

                if (tb.CrAmt != 0 || tb.DrAmt != 0)
                {
                    lstPaymentReceipt.Add(tb);
                    TotCr += tb.CrAmt.Value;
                    TotDr += tb.DrAmt.Value;
                }
            }
            #endregion

            #region Supplier
            foreach (var c in S1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = c.SupplierName;
                tb.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                tb.CrAmt = c.PaymentSuppliers.Sum(x => x.Payment.Amount);
                tb.DrAmt = c.ReceiptSuppliers.Sum(x => x.Receipt.Amount);

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = tb.DrAmt;
                }

                if (tb.CrAmt != 0 || tb.DrAmt != 0)
                {
                    lstPaymentReceipt.Add(tb);
                    TotCr += tb.CrAmt.Value;
                    TotDr += tb.DrAmt.Value;
                }
            }
            #endregion

            #region Bank    
            foreach (var c in B1)
            {
                tb = new BLL.TrialBalance();

                tb.LedgerName = c.BankName;
                tb.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                tb.CrAmt = c.PaymentBanks.Sum(x => x.Payment.Amount);
                tb.DrAmt = c.ReceiptBanks.Sum(x => x.Receipt.Amount);

                if (tb.DrAmt > tb.CrAmt)
                {
                    tb.DrAmt = tb.DrAmt - tb.CrAmt;
                    tb.CrAmt = 0;
                }
                else
                {
                    tb.CrAmt = tb.CrAmt - tb.DrAmt;
                    tb.DrAmt = tb.DrAmt;
                }

                if (tb.CrAmt != 0 || tb.DrAmt != 0)
                {
                    lstPaymentReceipt.Add(tb);
                    TotCr += tb.CrAmt.Value;
                    TotDr += tb.DrAmt.Value;
                }
            }
            #endregion

            tb = new BLL.TrialBalance();
            tb.LedgerName = "Total ";
            tb.DrAmt = TotDr;
            tb.CrAmt = TotCr;
            lstPaymentReceipt.Add(tb);


            return lstPaymentReceipt;
        }

        #endregion

        #region Profit_Loss

        public List<BLL.BalanceSheet> PL_List()
        {
            List<BLL.BalanceSheet> lstPL = new List<BLL.BalanceSheet>();
            BLL.BalanceSheet bal = new BLL.BalanceSheet();

            #region Income
            var l2 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Income").ToList();
            var C2 = DB.Customers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Income").ToList();
            var S2 = DB.Suppliers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Income").ToList();
            var B2 = DB.Banks.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Income").ToList();

            decimal ToAssDr = 0, ToAssCr = 0;
            bal.LedgerName = "Income";
            lstPL.Add(bal);

            #region Ledger
            foreach (var l in l2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", l.LedgerName);
                bal.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                bal.DrAmt = l.PaymentLedgers.Sum(x => x.Payment.Amount);
                bal.CrAmt = l.ReceiptLedgers.Sum(x => x.Receipt.Amount);
                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = 0;
                }

                if (bal.DrAmt != 0 || bal.CrAmt != 0)
                {
                    lstPL.Add(bal);
                    ToAssDr += bal.DrAmt.Value;
                    ToAssCr += bal.CrAmt.Value;
                }
            }
            #endregion

            #region Customer
            foreach (var c in C2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.CustomerName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentCustomers.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptCustomers.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstPL.Add(bal);
                    ToAssCr += bal.CrAmt.Value;
                    ToAssDr += bal.DrAmt.Value;
                }
            }
            #endregion

            #region Supplier
            foreach (var c in S2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.SupplierName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentSuppliers.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptSuppliers.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstPL.Add(bal);
                    ToAssCr += bal.CrAmt.Value;
                    ToAssDr += bal.DrAmt.Value;
                }
            }
            #endregion

            #region Bank    
            foreach (var c in B2)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.BankName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentBanks.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptBanks.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstPL.Add(bal);
                    ToAssCr += bal.CrAmt.Value;
                    ToAssDr += bal.DrAmt.Value;
                }
            }
            #endregion

            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Total Income";
            bal.DrAmt = ToAssDr;
            bal.CrAmt = ToAssCr;
            lstPL.Add(bal);

            #endregion

            #region Liabilities
            var l1 = DB.Ledgers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Expenses" ).ToList();
            var C1 = DB.Customers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Expenses").ToList();
            var S1 = DB.Suppliers.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Expenses").ToList();
            var B1 = DB.Banks.Where(x => x.CompanyId == Caller.CompanyId && x.AccountGroup.GroupName == "Expenses").ToList();

            decimal TotDr = 0, TotCr = 0;
            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Expenses";

            lstPL.Add(bal);

            #region Ledger
            foreach (var l in l1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", l.LedgerName);
                bal.GroupName = l.AccountGroup == null ? null : l.AccountGroup.GroupName;
                bal.DrAmt = l.PaymentLedgers.Sum(x => x.Payment.Amount);
                bal.CrAmt = l.ReceiptLedgers.Sum(x => x.Receipt.Amount);
                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = 0;
                }

                if (bal.DrAmt != 0 || bal.CrAmt != 0)
                {
                    lstPL.Add(bal);
                    TotDr += bal.DrAmt.Value;
                    TotCr += bal.CrAmt.Value;
                }
            }
            #endregion

            #region Customer
            foreach (var c in C1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.CustomerName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentCustomers.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptCustomers.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstPL.Add(bal);
                    TotCr += bal.CrAmt.Value;
                    TotDr += bal.DrAmt.Value;
                }
            }
            #endregion

            #region Supplier
            foreach (var c in S1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.SupplierName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentSuppliers.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptSuppliers.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstPL.Add(bal);
                    TotCr += bal.CrAmt.Value;
                    TotDr += bal.DrAmt.Value;
                }
            }
            #endregion

            #region Bank    
            foreach (var c in B1)
            {
                bal = new BLL.BalanceSheet();

                bal.LedgerName = string.Format("     {0}", c.BankName);
                bal.GroupName = c.AccountGroup == null ? null : c.AccountGroup.GroupName;
                bal.CrAmt = c.PaymentBanks.Sum(x => x.Payment.Amount);
                bal.DrAmt = c.ReceiptBanks.Sum(x => x.Receipt.Amount);

                if (bal.DrAmt > bal.CrAmt)
                {
                    bal.DrAmt = bal.DrAmt - bal.CrAmt;
                    bal.CrAmt = 0;
                }
                else
                {
                    bal.CrAmt = bal.CrAmt - bal.DrAmt;
                    bal.DrAmt = bal.DrAmt;
                }

                if (bal.CrAmt != 0 || bal.DrAmt != 0)
                {
                    lstPL.Add(bal);
                    TotCr += bal.CrAmt.Value;
                    TotDr += bal.DrAmt.Value;
                }
            }
            #endregion

            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Total Expenses";
            bal.DrAmt = TotDr;
            bal.CrAmt = TotCr;
            lstPL.Add(bal);

            bal = new BLL.BalanceSheet();
            bal.LedgerName = "Profit/Loss";
            bal.DrAmt = TotDr + ToAssDr;
            bal.CrAmt = TotCr + ToAssCr;

            if(bal.DrAmt>bal.CrAmt)
            {
                bal.DrAmt = bal.DrAmt - bal.CrAmt;
                bal.CrAmt = 0;
            }
            else
            {
                bal.CrAmt = bal.CrAmt - bal.DrAmt;
                bal.DrAmt = 0;
            }
            lstPL.Add(bal);

            #endregion

            return lstPL;
        }

        #endregion

        #endregion





    }

    public class User
    {
        public string ConnectionId { get; set; }
        public int UserId { get; set; }
        public int CompanyId { get; set; }
        public string AccYear { get; set; }
    }
}