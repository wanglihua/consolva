using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Fee.Models.CustomerCharge;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.CustomerCharge)]
public class CustomerChargeController : Controller
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();
    private readonly CustomerChargeService customerChargeService;
    private readonly CustomerCodeService customerCodeService;
    private readonly LoginUserService loginUserService;

    private readonly ISession session;

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
        var model = new CustomerChargeIndexModel();
        model.ChargeTypeList = CustomerChargeType.GetList();
        model.HasManualChargePass = PassUtil.HasPassRes(loginUserService.GetPassResIdMap(), loginUserService.IsAdmin, PassRes.Fee.CustomerChargeManualCharge);

        return View("CustomerChargeIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/charge/list")]
    public IActionResult List(CustomerChargeListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var customerChargeList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerChargeInfo>();

        foreach (var customerChargeInfo in customerChargeList)
        {
            // customerChargeInfo
        }

        var model = new CustomerChargeListModel(listPager);
        model.CustomerChargeList = customerChargeList;

        return View("CustomerChargeList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerChargeListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<CustomerChargeInfo>();

        criteria.SetReadOnly(true);

        criteria = criteria.Add(Restrictions.Where<CustomerChargeInfo>(e => e.Status == CustomerChargeStatus.Complete));

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCodeQuery) == false)
        {
            var customerCodeQuery = queryParam.CustomerCodeQuery.Trim();
            var customerIdQuery = customerCodeService.GetCustomerIdFromCustomerCode(customerCodeQuery);
            criteria = criteria.Add(Restrictions.Where<CustomerChargeInfo>(e => e.Customer.Id == customerIdQuery));
        }

        if (queryParam.ChargeTypeQuery != null && queryParam.ChargeTypeQuery != CustomerChargeType.None)
        {
            var chargeTypeQuery = queryParam.ChargeTypeQuery;
            criteria = criteria.Add(Restrictions.Where<CustomerChargeInfo>(e => e.ChargeType == chargeTypeQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/charge/manual/charge")]
    [AdminAuth(PassRes.Fee.CustomerChargeManualCharge)]
    public IActionResult ManualCharge()
    {
        var model = new CustomerChargeDetailModel();
        model.ChargeReasonList = session.Query<ManualChargeReasonInfo>().Where(e => e.Status == ManualChargeReasonStatus.Enabled).ToList();

        return View("CustomerChargeDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/charge/sea/save")]
    [AdminAuth(PassRes.Fee.CustomerChargeManualCharge)]
    [Transactional]
    public IActionResult ManualChargeSave(CustomerChargeDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        UserInfo loginUser = loginUserService.GetLoginUser();

        var customerId = customerCodeService.GetCustomerIdFromCustomerCode(model.CustomerCode);
        var customer = session.Get<CustomerInfo>(customerId);

        if (customer == null) throw new ValidationException("指定的客户ID不存在！");

        if (model.Amount == 0) throw new ValidationException("充值金额不能为0！");

        var amount = model.Amount;

        var customerCharge = new CustomerChargeInfo();
        customerCharge.ChargeType = CustomerChargeType.Manual;
        customerCharge.Customer = customer;
        customerCharge.Amount = model.Amount;
        customerCharge.ManualChargeReason = model.ChargeReason;
        customerCharge.User = loginUser;
        customerCharge.Status = CustomerChargeStatus.New; // 先是这个状态，后面完成充值的时候会修改状态

        session.Save(customerCharge);
        session.Flush();

        customerChargeService.CompleteCharge(customerCharge, amount, CustomerChargeType.Manual);

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "手工充值成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/charge/import")]
    // 内部用事务处理
    public IActionResult Import(IFormFile excelFile)
    {
        if (excelFile == null) throw new ValidationException("导入失败，文件未上传！");

        UserInfo loginUser = loginUserService.GetLoginUser();

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        var sheet = workbook.GetSheetAt(0);
        if (sheet == null) throw new ValidationException("导入的Excel文件应该至少有一个工作表！");

        IList<CustomerChargeImportItem> importItemList = new List<CustomerChargeImportItem>();

        // 略过标题行，所以 rowIndex = 1 开始
        var lastRowNum = sheet.LastRowNum;
        for (var rowIndex = 1; rowIndex <= lastRowNum; rowIndex++)
        {
            var row = sheet.GetRow(rowIndex);

            if (row == null) break;

            // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
            var cellIndex = 0;
            int lastCellNum = row.LastCellNum;
            for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++) row.GetCell(cellIndex)?.SetCellType(CellType.String);

            // 客户ID 充值金额

            cellIndex = 0;

            var importItem = new CustomerChargeImportItem();

            // 客户ID
            var customerCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(customerCode)) break;

            importItem.CustomerCode = customerCode;
            var customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerCode);
            if (customerId == 0) throw new ValidationException($"导入的第 {rowIndex} 行，客户ID输入有误！");

            var customer = session.Get<CustomerInfo>(customerId);
            if (customer == null) throw new ValidationException($"导入的第 {rowIndex} 行，指定的客户ID {customerCode} 不存在！");

            importItem.Customer = customer;

            // 充值金额
            var amountStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            if (decimal.TryParse(amountStr, out var amount) == false) throw new ValidationException($"导入的第 {rowIndex} 行，充值金额输入有误！");

            if (amount <= 0) throw new ValidationException($"导入的第 {rowIndex} 行，充值金额应大于0！");

            importItem.Amount = amount;

            importItemList.Add(importItem);
        }

        Transactional.Wrap(session, () => {
            var dataIndex = 1;
            foreach (var importItem in importItemList)
            {
                var customer = importItem.Customer;

                if (customer == null) throw new ValidationException($"导入的第 {dataIndex} 行，编号为 {importItem.CustomerCode} 的客户不存在！");

                var amount = importItem.Amount;

                var customerCharge = new CustomerChargeInfo();
                customerCharge.ChargeType = CustomerChargeType.Manual;
                customerCharge.Customer = customer;
                customerCharge.Amount = amount;
                customerCharge.ManualChargeReason = string.Empty;
                customerCharge.User = loginUser;
                customerCharge.Status = CustomerChargeStatus.New; // 先是这个状态，后面完成充值的时候会修改状态

                session.Save(customerCharge);
                session.Flush();

                customerChargeService.CompleteCharge(customerCharge, amount, CustomerChargeType.Manual);

                dataIndex++;
            }

            session.Flush();
        });

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "导入成功！" }
        });
    }

    private class CustomerChargeImportItem
    {
        public string CustomerCode { get; set; } = string.Empty;

        public CustomerInfo Customer { get; set; } = new();

        public decimal Amount { get; set; }
    }
}