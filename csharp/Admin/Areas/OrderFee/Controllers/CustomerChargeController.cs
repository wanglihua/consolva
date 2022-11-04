using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.OrderFee.Models.CustomerCharge;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.OrderFee.Controllers;

[OrderFeeArea]
[AdminAuth(PassRes.OrderFee.CustomerCharge)]
public class CustomerChargeController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private CustomerCodeService customerCodeService;
    private CustomerChargeService customerChargeService;
    private LoginUserService loginUserService;

    public CustomerChargeController(
        ISession session,
        CustomerCodeService customerCodeService,
        CustomerChargeService customerChargeService,
        LoginUserService loginUserService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.customerChargeService = customerChargeService;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/customer/charge")]
    public IActionResult Index()
    {
        CustomerChargeIndexModel model = new CustomerChargeIndexModel();
        model.ChargeTypeList = CustomerChargeType.GetList();
        model.HasManualChargePass = PassUtil.HasPassRes(loginUserService.GetPassResIdMap(), loginUserService.IsAdmin, PassRes.OrderFee.CustomerChargeManualCharge);

        return View("CustomerChargeIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/charge/list")]
    public IActionResult List(CustomerChargeListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerChargeInfo> customerChargeList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerChargeInfo>();

        foreach (CustomerChargeInfo customerChargeInfo in customerChargeList)
        {
            // customerChargeInfo
        }

        CustomerChargeListModel model = new CustomerChargeListModel(listPager);
        model.CustomerChargeList = customerChargeList;

        return View("CustomerChargeList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerChargeListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerChargeInfo>();

        criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Where<CustomerChargeInfo>(e => e.Status == CustomerChargeStatus.Complete));

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            string customerCodeQuery = queryParam.CustomerCodeQuery.Trim();
            int customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<CustomerChargeInfo>(e => e.Customer.Id == customerIdQuery));
        }

        if (queryParam.ChargeTypeQuery != null && queryParam.ChargeTypeQuery != CustomerChargeType.None)
        {
            CustomerChargeType chargeTypeQuery = queryParam.ChargeTypeQuery;
            criteria = criteria.Add(Restrictions.Where<CustomerChargeInfo>(e => e.ChargeType == chargeTypeQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/charge/manual/charge")]
    [AdminAuth(PassRes.OrderFee.CustomerChargeManualCharge)]
    public IActionResult ManualCharge()
    {
        CustomerChargeDetailModel model = new CustomerChargeDetailModel();
        model.ChargeReasonList = session.Query<ManualChargeReasonInfo>().Where(e => e.Status == ManualChargeReasonStatus.Enabled).ToList();

        return View("CustomerChargeDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/charge/sea/save")]
    [AdminAuth(PassRes.OrderFee.CustomerChargeManualCharge)]
    [Transactional]
    public IActionResult ManualChargeSave(CustomerChargeDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();

        int customerId = customerCodeService.GetCustomerIdFromCustomerCode(model.CustomerCode);
        CustomerInfo customer = session.Get<CustomerInfo>(customerId);

        if (customer == null)
        {
            throw new ValidationException("指定的会员ID不存在！");
        }

        if (model.Amount == 0)
        {
            throw new ValidationException("充值金额不能为0！");
        }

        decimal amount = model.Amount;

        CustomerChargeInfo customerCharge = new CustomerChargeInfo();
        customerCharge.ChargeType = CustomerChargeType.Manual;
        customerCharge.Customer = customer;
        customerCharge.Amount = model.Amount;
        customerCharge.ManualChargeReason = model.ChargeReason;
        customerCharge.User = session.Get<UserInfo>(loginUserId);
        customerCharge.Status = CustomerChargeStatus.New; // 先是这个状态，后面完成充值的时候会修改状态

        session.Save(customerCharge);
        session.Flush();

        customerChargeService.CompleteCharge(customerCharge, amount, CustomerChargeType.Manual);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "手工充值成功！" },
        });
    }

    private class CustomerChargeImportItem
    {
        public string CustomerCode { get; set; } = String.Empty;

        public CustomerInfo Customer { get; set; } = new CustomerInfo();

        public decimal Amount { get; set; }
    }

    [HttpPost]
    [Route("/admin/customer/charge/import")]
    // 内部用事务处理
    public IActionResult Import(IFormFile excelFile)
    {
        if (excelFile == null)
        {
            throw new ValidationException("导入失败，文件未上传！");
        }

        int loginUserId = loginUserService.GetLoginUserId();

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet == null)
        {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        IList<CustomerChargeImportItem> importItemList = new List<CustomerChargeImportItem>();

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

            // 客户ID 充值金额

            cellIndex = 0;

            CustomerChargeImportItem importItem = new CustomerChargeImportItem();

            // 客户ID
            string customerCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(customerCode))
            {
                break;
            }

            importItem.CustomerCode = customerCode;
            int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCode);
            if (customerId == 0)
            {
                throw new ValidationException($"导入的第 {rowIndex} 行，客户ID输入有误！");
            }

            CustomerInfo customer = session.Get<CustomerInfo>(customerId);
            if (customer == null)
            {
                throw new ValidationException($"导入的第 {rowIndex} 行，指定的客户ID {customerCode} 不存在！");
            }

            importItem.Customer = customer;

            // 充值金额
            string amountStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            if (decimal.TryParse(amountStr, out decimal amount) == false)
            {
                throw new ValidationException($"导入的第 {rowIndex} 行，充值金额输入有误！");
            }

            if (amount <= 0)
            {
                throw new ValidationException($"导入的第 {rowIndex} 行，充值金额应大于0！");
            }

            importItem.Amount = amount;

            importItemList.Add(importItem);
        }

        ITransaction? transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            int dataIndex = 1;
            foreach (CustomerChargeImportItem importItem in importItemList)
            {
                CustomerInfo customer = importItem.Customer;

                if (customer == null)
                {
                    throw new ValidationException($"导入的第 {dataIndex} 行，编号为 {importItem.CustomerCode} 的会员不存在！");
                }

                decimal amount = importItem.Amount;

                CustomerChargeInfo customerCharge = new CustomerChargeInfo();
                customerCharge.ChargeType = CustomerChargeType.Manual;
                customerCharge.Customer = customer;
                customerCharge.Amount = amount;
                customerCharge.ManualChargeReason = string.Empty;
                customerCharge.User = session.Get<UserInfo>(loginUserId);
                customerCharge.Status = CustomerChargeStatus.New; // 先是这个状态，后面完成充值的时候会修改状态

                session.Save(customerCharge);
                session.Flush();

                customerChargeService.CompleteCharge(customerCharge, amount, CustomerChargeType.Manual);

                dataIndex++;
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
            { "message", "导入成功！" },
        });
    }
}