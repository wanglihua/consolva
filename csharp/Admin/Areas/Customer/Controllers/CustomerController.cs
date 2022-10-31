using System;
using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Customer.Models.Customer;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Order;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service;
using shunda.Service.Customer;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerList)]
public class CustomerController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    public static readonly int NoVipRankId = -100;
    public static readonly int SuperVipRankId = -200;

    private ISession session;
    private CustomerCodeService customerCodeService;
    private LoginUserService loginUserService;
    private PasswordService passwordService;

    public CustomerController(ISession session,
        CustomerCodeService customerCodeService,
        LoginUserService loginUserService,
        PasswordService passwordService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.loginUserService = loginUserService;
        this.passwordService = passwordService;
    }


    [HttpGet]
    [Route("/admin/customer")]
    public IActionResult Index()
    {
        CustomerIndexModel model = new CustomerIndexModel();

        IList<Tuple<int, string>> vipLevelList = new List<Tuple<int, string>>();
        vipLevelList.Add(Tuple.Create(NoVipRankId, "无VIP"));
        vipLevelList.Add(Tuple.Create(SuperVipRankId, "超级VIP"));

        IList<CustomerVipDiscountInfo> customerVipDiscountList = session.Query<CustomerVipDiscountInfo>().OrderBy(e => e.Id).ToList();
        foreach (var customerVipDiscount in customerVipDiscountList)
        {
            vipLevelList.Add(Tuple.Create(customerVipDiscount.Rank.Id, customerVipDiscount.Rank.Name));
        }

        model.VipLevelList = vipLevelList;
        model.StatusList = CustomerStatus.GetList();
        model.CharacterList = session.Query<CustomerCharacterInfo>().OrderBy(e => e.Content).ToList();

        bool isAdmin = loginUserService.IsAdmin;
        Dictionary<int, bool> passResIdMap = loginUserService.GetPassResIdMap();
        model.CanImportTestCustomer = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Customer.ImportTestCustomer);

        return View("CustomerIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/list")]
    public IActionResult List(CustomerListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerInfo> customerList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerInfo>();

        foreach (CustomerInfo customer in customerList)
        {
            customer.CustomerCodeCache = customerCodeService.GetCustomerCode(customer.Id);
        }

        CustomerListModel model = new CustomerListModel(listPager);
        model.CustomerList = customerList;

        return View("CustomerList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            int customerId = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<CustomerInfo>(e => e.Id == customerId));
        }

        if ((queryParam.VipLevelQuery ?? 0) != 0)
        {
            int vipLevelQuery = queryParam.VipLevelQuery ?? 0;

            if (vipLevelQuery == NoVipRankId)
            {
                criteria = criteria.Add(Restrictions.Where<CustomerInfo>(e => e.Level == CustomerLevel.Common));
            }
            else if (vipLevelQuery == SuperVipRankId)
            {
                criteria = criteria.Add(Restrictions.Where<CustomerInfo>(e => e.Level == CustomerLevel.SuperVip));
            }
            else
            {
                CustomerVipRank vipRank = CustomerVipRank.GetById(vipLevelQuery);
                criteria = criteria.Add(Restrictions.Where<CustomerInfo>(e => e.Level == CustomerLevel.Vip && e.VipRank == vipRank));
            }
        }

        CustomerStatus statusQuery = queryParam.StatusQuery ?? CustomerStatus.None;
        if (statusQuery != CustomerStatus.None)
        {
            criteria = criteria.Add(Restrictions.Where<CustomerInfo>(e => e.Status == statusQuery));
        }

        if (queryParam.BeginDateQuery != null)
        {
            DateTime beginDateQueryValue = queryParam.BeginDateQuery.Value;
            criteria = criteria.Add(Restrictions.Where<CustomerInfo>(e => e.RegTime >= beginDateQueryValue));
        }

        if (queryParam.EndDateQuery != null)
        {
            DateTime endDateQueryValue = queryParam.EndDateQuery.Value.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<CustomerInfo>(e => e.RegTime < endDateQueryValue));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CharacterQuery) == false)
        {
            string characterQuery = queryParam.CharacterQuery.Trim();
            criteria = criteria.Add(Restrictions.Where<CustomerInfo>(e => e.CustomerCharacter == characterQuery));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/customer/detail/{id:int}")]
    [AdminAuth(PassRes.Customer.CustomerList)]
    public IActionResult Detail(int id)
    {
        int customerId = id;

        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的数据项不存在！");
        }

        CustomerDetailModel model = new CustomerDetailModel();
        model.Customer = customer;

        /*
        // 订单列表
        {
            ICriteria criteria = session.CreateCriteria<LogisticsOrderInfo>();
            criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.Customer), nameof(LogisticsOrderInfo.Customer));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.ReceivingWarehouse), nameof(LogisticsOrderInfo.ReceivingWarehouse));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.TargetWarehouse), nameof(LogisticsOrderInfo.TargetWarehouse), JoinType.LeftOuterJoin);
            criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.FirstLogisticsMode), nameof(LogisticsOrderInfo.FirstLogisticsMode));
            criteria = criteria.CreateAlias(nameof(LogisticsOrderInfo.SecondLogisticsMode), nameof(LogisticsOrderInfo.SecondLogisticsMode), JoinType.LeftOuterJoin);

            criteria = criteria.SetReadOnly(true);

            var draftStatus = LogisticsOrderStatus.Draft;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus != draftStatus));
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.OrderStatus != LogisticsOrderStatus.OrderCanceled));

            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.Customer.Id == customerId));

            criteria.AddOrder(new Order(nameof(LogisticsOrderInfo.Id), false)); // id 倒序

            IList<LogisticsOrderInfo> orderList = criteria.SetMaxResults(10).List<LogisticsOrderInfo>();

            IList<int> orderIdList = orderList.Select(e => e.Id).ToList();
            IList<LogisticsOrderGoodsExpressInfo> expressTrackingNoAbsentExpressList = session
                .Query<LogisticsOrderGoodsExpressInfo>()
                .Where(e => orderIdList.Contains(e.Order.Id) && (e.ExpressTrackingNo == null || e.ExpressTrackingNo == ""))
                .ToList();

            foreach (LogisticsOrderInfo logisticsOrder in orderList)
            {
                // logisticsOrderInfo.OrderStatusName = logisticsOrderInfo.OrderStatus.Name;
                // logisticsOrderInfo.ShippingStatusName = logisticsOrderInfo.ShippingStatus.Name;
                // logisticsOrderInfo.PayStatusName = logisticsOrderInfo.PayStatus.Name;
                int expressTrackingNoAbsentExpressCount = expressTrackingNoAbsentExpressList.Where(e => e.Order.Id == logisticsOrder.Id).Count();
                if (expressTrackingNoAbsentExpressCount != 0)
                {
                    logisticsOrder.ExpressTrackingNoAbsent = true;
                }
                else
                {
                    logisticsOrder.ExpressTrackingNoAbsent = false;
                }

                // logisticsOrder.ListColor = logisticsOrderStatusService.GetStatusColor(logisticsOrder.OrderStatus);
            }

            model.CustomerOrderList = orderList;
        }

        // 申领件列表
        {
            ICriteria criteria = session.CreateCriteria<LogisticsClaimGoodsInfo>();
            criteria = criteria.SetReadOnly(true);
            criteria = criteria.Add(Restrictions.Where<LogisticsClaimGoodsInfo>(e => e.BillCustomerId == customerId));

            IList<LogisticsClaimGoodsInfo> claimGoodsList = criteria
                .AddOrder(new Order(nameof(LogisticsClaimGoodsInfo.Id), false))
                .SetMaxResults(10)
                .List<LogisticsClaimGoodsInfo>();

            IList<int> userIdList = claimGoodsList.Select(e => e.AddUserId).Distinct().ToList();
            IList<int> orderIdList = claimGoodsList.Where(e => e.Order != null).Select(e => e.Order.Id).Distinct().ToList();
            List<int> storageLocIdList = claimGoodsList.Where(e => e.StorageLoc != null).Select(e => e.StorageLoc.Id).Distinct().ToList();

            IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();
            IList<LogisticsOrderInfo> orderList = session.Query<LogisticsOrderInfo>().Where(e => orderIdList.Contains(e.Id)).ToList();
            IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>().Where(e => storageLocIdList.Contains(e.Id)).ToList();

            foreach (LogisticsClaimGoodsInfo claimGoodsInfo in claimGoodsList)
            {
                UserInfo user = userList.Where(e => e.Id == claimGoodsInfo.AddUserId).Take(1).SingleOrDefault();
                claimGoodsInfo.AddUserNameCache = user?.UserName;

                claimGoodsInfo.CustomerNameCache = customer?.CustomerCode;

                LogisticsOrderInfo logisticsOrder = orderList.Where(e => e.Id == (claimGoodsInfo.Order?.Id ?? 0)).Take(1).SingleOrDefault();
                claimGoodsInfo.OrderCodeCache = logisticsOrder?.Code;

                StorageLocInfo storageLoc = storageLocList.Where(e => e.Id == (claimGoodsInfo.StorageLoc?.Id ?? 0)).Take(1).SingleOrDefault();
                claimGoodsInfo.StorageLocNameCache = storageLoc?.GetFullName();
            }

            model.LogisticsClaimGoodsList = claimGoodsList;
        }

        // 问题件列表
        {
            ICriteria criteria = session.CreateCriteria<LogisticsIssueGoodsInfo>();
            criteria = criteria.SetReadOnly(true);
            criteria = criteria.Add(Restrictions.Where<LogisticsIssueGoodsInfo>(e => e.CustomerId == customerId));

            IList<LogisticsIssueGoodsInfo> logisticsIssueGoodsList = criteria
                .AddOrder(new Order(nameof(LogisticsIssueGoodsInfo.Id), false))
                .SetMaxResults(10)
                .List<LogisticsIssueGoodsInfo>();

            IList<int> warehouseIdList = logisticsIssueGoodsList.Select(e => e.WarehouseId).Distinct().ToList();
            IList<WarehouseInfo> warehouseList = session.Query<WarehouseInfo>().Where(e => warehouseIdList.Contains(e.Id)).ToList();

            IList<int> customerGoodsIdList = logisticsIssueGoodsList
                .Select(e => e.CustomerGoodsId)
                .Distinct()
                .ToList();

            IList<LogisticsGoodsInfo> customerGoodsList = session.Query<LogisticsGoodsInfo>().Where(e => customerGoodsIdList.Contains(e.Id)).ToList();

            IList<int> adminGoodsIdList = logisticsIssueGoodsList
                .Where(e => e.AdminGoodsId.HasValue)
                .Select(e => e.AdminGoodsId.Value)
                .Distinct()
                .ToList();

            IList<int> storageLocIdList = logisticsIssueGoodsList.Where(e => e.StorageLoc != null).Select(e => e.StorageLoc.Id).ToList();
            IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>().Where(e => storageLocIdList.Contains(e.Id)).ToList();

            IList<LogisticsGoodsInfo> adminGoodsList = session.Query<LogisticsGoodsInfo>().Where(e => adminGoodsIdList.Contains(e.Id)).ToList();

            foreach (LogisticsIssueGoodsInfo logisticsIssueGoodsInfo in logisticsIssueGoodsList)
            {
                WarehouseInfo warehouse = warehouseList.Where(e => e.Id == logisticsIssueGoodsInfo.WarehouseId).Take(1).SingleOrDefault();
                logisticsIssueGoodsInfo.WarehouseNameCache = warehouse?.Name;

                LogisticsGoodsInfo customerGoods = customerGoodsList.Where(e => e.Id == logisticsIssueGoodsInfo.CustomerGoodsId).Take(1).SingleOrDefault();
                logisticsIssueGoodsInfo.CustomerGoodsNameCache = customerGoods?.Name;

                LogisticsGoodsInfo adminGoods = adminGoodsList.Where(e => e.Id == logisticsIssueGoodsInfo.AdminGoodsId).Take(1).SingleOrDefault();
                logisticsIssueGoodsInfo.AdminGoodsNameCache = adminGoods?.GetGoodsAdminName();

                int issueGoodsStorageLocId = logisticsIssueGoodsInfo.StorageLoc?.Id ?? 0;
                StorageLocInfo storageLoc = storageLocList.Where(e => e.Id == issueGoodsStorageLocId).Take(1).SingleOrDefault();
                logisticsIssueGoodsInfo.StorageLocNameCache = storageLoc?.GetFullName();
            }

            model.LogisticsIssueGoodsList = logisticsIssueGoodsList;
        }

        // 余额明细
        {
            IList<CustomerAccountEntryInfo> customerAccountEntryList = session.CreateCriteria<CustomerAccountEntryInfo>()
                .Add(Restrictions.Where<CustomerAccountEntryInfo>(e => e.Customer.Id == customerId))
                .AddOrder(new Order(nameof(CustomerAccountEntryInfo.Id), false)) // id 倒序
                .SetMaxResults(10)
                .List<CustomerAccountEntryInfo>();

            model.CustomerAccountEntryList = customerAccountEntryList;
        }

        // 支付明细
        {
            IList<LogisticsOrderPayInfo> logisticsOrderPayList = session.CreateCriteria<LogisticsOrderPayInfo>()
                .Add(Restrictions.Where<LogisticsOrderPayInfo>(e => e.Customer.Id == customerId && e.Status == LogisticsOrderPayStatus.Paid))
                .AddOrder(new Order(nameof(LogisticsOrderPayInfo.Id), false))
                .SetMaxResults(10)
                .List<LogisticsOrderPayInfo>();

            model.LogisticsOrderPayList = logisticsOrderPayList;
        }

        // 积分明细
        {
            IList<CustomerScoreLogInfo> customerScoreLogList = session.CreateCriteria<CustomerScoreLogInfo>()
                .Add(Restrictions.Where<CustomerScoreLogInfo>(e => e.Customer.Id == customerId))
                .AddOrder(new Order(nameof(CustomerScoreLogInfo.Id), false))
                .SetMaxResults(10)
                .List<CustomerScoreLogInfo>();

            model.CustomerScoreLogList = customerScoreLogList;
        }

        //客户地址列表
        {
            IList<CustomerAddressInfo> customerAddressList = session.CreateCriteria<CustomerAddressInfo>()
                .Add(Restrictions.Where<CustomerAddressInfo>(e => e.Customer.Id == customerId))
                .AddOrder(new Order(nameof(CustomerAddressInfo.IsDefault), false)) // 默认地址排在前面
                .AddOrder(new Order(nameof(CustomerAddressInfo.Id), false)) // id 倒序
                .SetMaxResults(3)
                .List<CustomerAddressInfo>();

            model.CustomerAddressList = customerAddressList;
        }
        */

        bool isAdmin = loginUserService.IsAdmin;
        Dictionary<int, bool> passResIdMap = loginUserService.GetPassResIdMap();
        model.CanFreezeCustomer = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Customer.CustomerFreeze);
        model.CanResetPassword = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Customer.CustomerResetPassword);
        model.CanEditCharacter = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Customer.CustomerCharacter);

        IList<CustomerCharacterInfo> customerCharacterList = session.Query<CustomerCharacterInfo>().ToList();
        if (customerCharacterList.Where(e => e.Content == customer.CustomerCharacter).Count() == 0)
        {
            customerCharacterList.Insert(0, new CustomerCharacterInfo() { Content = customer.CustomerCharacter });
        }

        model.CustomerCharacterList = customerCharacterList;

        return View("CustomerDetail", model);
    }


    [HttpPost]
    [Route("/admin/customer/unfreeze")]
    [AdminAuth(PassRes.Customer.CustomerFreeze)]
    [Transactional]
    public IActionResult Unfreeze(int id)
    {
        int customerId = id;

        CustomerInfo customerInfo = session.Get<CustomerInfo>(customerId);
        if (customerInfo == null)
        {
            throw new ValidationException("指定的客户不存在！");
        }

        if (customerInfo.Status != CustomerStatus.Frozen)
        {
            throw new ValidationException("客户当前不是冻结状态！");
        }

        customerInfo.Status = CustomerStatus.Normal;
        session.Update(customerInfo);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "客户解冻成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/customer/freeze")]
    [AdminAuth(PassRes.Customer.CustomerFreeze)]
    [Transactional]
    public IActionResult Freeze(int id)
    {
        int customerId = id;

        CustomerInfo customerInfo = session.Get<CustomerInfo>(customerId);
        if (customerInfo == null)
        {
            throw new ValidationException("指定的客户不存在！");
        }

        if (customerInfo.Status == CustomerStatus.Frozen)
        {
            throw new ValidationException("客户已经是冻结状态！");
        }

        customerInfo.Status = CustomerStatus.Frozen;
        session.Update(customerInfo);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "客户冻结成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/customer/reset/password")]
    [AdminAuth(PassRes.Customer.CustomerResetPassword)]
    [Transactional]
    public IActionResult ResetPassword(int id)
    {
        int customerId = id;

        CustomerInfo customerInfo = session.Get<CustomerInfo>(customerId);
        if (customerInfo == null)
        {
            throw new ValidationException("指定的客户不存在！");
        }

        string salt = passwordService.GenerateSalt();
        customerInfo.Salt = salt;
        customerInfo.Password = passwordService.Encrypt(CustomerInfo.DefaultPassword, salt);

        session.Update(customerInfo);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "重设客户密码成功！" },
        });
    }

    /*
    [HttpPost]
    [Route("/admin/customer/edit/character")]
    [AdminAuth(PassRes.Customer.CustomerCharacter)]
    [Transactional]
    public IActionResult EditCharacter(int customerId, string character)
    {
        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的客户不存在！");
        }

        // if (character != null && character.Length > 5)
        // {
        //     throw new ValidationException("客户性质长度不能大于5个字符！");
        // }
        

        customer.CustomerCharacter = character;

        session.Save(customer);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "客户性质修改成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/customer/update/vip")]
    [AdminAuth(PassRes.Customer.CustomerList)]
    // 这里的事务还是需要的，但事务已经做在存储过程里面了
    public IActionResult UpdateVip()
    {
        customerCodeService.UpdateAllCustomerVip();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "更新客户VIP成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/customer/import/test/cutomer")]
    [AdminAuth(PassRes.Customer.ImportTestCustomer)]
    // 内部有数据库事务
    public IActionResult ImportTestCustomer(IFormFile excelFile)
    {
        if (excelFile == null)
        {
            throw new ValidationException("导入失败，文件未上传！");
        }

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet == null)
        {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        IList<CustomerInfo> customerList = new List<CustomerInfo>();

        // 略过标题行，所以 rowIndex = 1 开始
        int lastRowNum = sheet.LastRowNum;
        for (int rowIndex = 1; rowIndex <= lastRowNum; rowIndex++)
        {
            IRow row = sheet.GetRow(rowIndex);

            if (row == null)
            {
                break;
            }

            // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
            int cellIndex = 0;
            int lastCellNum = row.LastCellNum;
            for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++)
            {
                row.GetCell(cellIndex)?.SetCellType(CellType.String);
            }

            // 手机号码 密码

            cellIndex = 0;

            // 手机号码
            string mobile = row.GetCell(cellIndex++)?.StringCellValue;
            if (string.IsNullOrWhiteSpace(mobile))
            {
                break;
            }

            CustomerInfo customer = new CustomerInfo();
            customer.Mobile = mobile.Trim();

            // 密码
            string password = row.GetCell(cellIndex++)?.StringCellValue;
            if (string.IsNullOrWhiteSpace(password) && password.Trim().Length < 6)
            {
                throw new ValidationException("密码不能为空，且不能少于6位！");
            }

            customer.Password = password;

            customerList.Add(customer);
        }

        {
            // 判断 导入部分的 mobile 重复情况
            IDictionary<string, int> dictImportMobile = new Dictionary<string, int>();

            int customerIndex = 0;
            foreach (var customer in customerList)
            {
                if (dictImportMobile.ContainsKey(customer.Mobile) == false)
                {
                    dictImportMobile[customer.Mobile] = customerIndex;
                }
                else
                {
                    int preMobileIndex = dictImportMobile[customer.Mobile];
                    throw new ValidationException($"Excel中 第 {customerIndex + 1} 行手机号码 {customer.Mobile} 与 第 {preMobileIndex + 1} 行手机号码 重复！");
                }

                customerIndex++;
            }

            // 导入部分和数据库部分的冲突
            IList<string> importMobileList = customerList.Select(e => e.Mobile).ToList();
            IList<CustomerInfo> customerListInDb = session.Query<CustomerInfo>()
                .Where(e => importMobileList.Contains(e.Mobile))
                .ToList();
            IList<string> mobileListInDb = customerListInDb.Select(e => e.Mobile).ToList();
            if (mobileListInDb.Count != 0)
            {
                throw new ValidationException($"Excel中手机号码 {string.Join(",", mobileListInDb)} 数据库中已经存在！");
            }
        }

        ITransaction transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            foreach (var customer in customerList)
            {
                customer.CustomerCharacter = "测试账户";
                customer.MobileValidated = true;

                string salt = passwordService.GenerateSalt();
                customer.Salt = salt;
                customer.Password = passwordService.Encrypt(customer.Password, salt);
                customer.PayPassword = "";

                customer.RegTime = DateTime.Now;
                customer.LastLogin = DateTime.Now;
                customer.LastIp = String.Empty;

                customer.Account = 0m;
                customer.TotalConsume = 0m;

                customer.OpenId = "";

                customer.Level = CustomerLevel.Common;
                customer.VipRank = CustomerVipRank.None;
                customer.Status = CustomerStatus.Normal;

                session.Save(customer);
            }

            session.Flush();

            transaction.Commit();
        }
        catch (Exception exp)
        {
            transaction?.Rollback();

            log.Error(exp);
            throw;
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "导入创建测试客户成功！" },
        });
    }
    */
}