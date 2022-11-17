using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Customer.Models.CustomerSuperVipDiscount;
using shunda.Admin.Services;
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

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerSuperVipDiscount)]
public class CustomerSuperVipDiscountController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly CustomerCodeService customerCodeService;
    private readonly LoginUserService loginUserService;

    private readonly ISession session;

    public CustomerSuperVipDiscountController(ISession session, CustomerCodeService customerCodeService, LoginUserService loginUserService)
    {
        this.session = session;
        this.customerCodeService = customerCodeService;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/customer/super/vip/discount")]
    public IActionResult Index()
    {
        var model = new CustomerSuperVipDiscountIndexModel();
        return View("CustomerSuperVipDiscountIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/list")]
    public IActionResult List(CustomerSuperVipDiscountListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var customerSuperVipDiscountList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerSuperVipDiscountInfo>();

        foreach (var customerSuperVipDiscount in customerSuperVipDiscountList) customerSuperVipDiscount.CustomerCodeCache = customerCodeService.GetCustomerCode(customerSuperVipDiscount.Customer?.Id ?? 0);

        var model = new CustomerSuperVipDiscountListModel(listPager);
        model.CustomerSuperVipDiscountList = customerSuperVipDiscountList;

        return View("CustomerSuperVipDiscountList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerSuperVipDiscountListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<CustomerSuperVipDiscountInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCode) == false)
        {
            var customerId = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCode);
            criteria = criteria.Add(Restrictions.Where<CustomerSuperVipDiscountInfo>(e => e.Customer.Id == customerId));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        CustomerSuperVipDiscountInfo? customerSuperVipDiscount;

        var model = new CustomerSuperVipDiscountDetailModel();

        if (id == null || id.Value == 0)
        {
            customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
        }
        else
        {
            customerSuperVipDiscount = session.Get<CustomerSuperVipDiscountInfo>(id.Value);
            if (customerSuperVipDiscount == null) throw new ValidationException("指定的数据项不存在！");

            customerSuperVipDiscount.CustomerCodeCache = customerCodeService.GetCustomerCode(customerSuperVipDiscount.Customer?.Id ?? 0);
        }

        model.CustomerSuperVipDiscount = customerSuperVipDiscount;

        return View("CustomerSuperVipDiscountDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/save")]
    public IActionResult Save(CustomerSuperVipDiscountDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var customerSuperVipDiscountFromUi = model.CustomerSuperVipDiscount;
        var customerSuperVipDiscountId = customerSuperVipDiscountFromUi.Id;

        var customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerSuperVipDiscountFromUi.CustomerCodeCache);
        if (customerId == 0) throw new ValidationException("指定的客户ID格式不对或客户ID未指定！");

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) throw new ValidationException("指定的客户不存在！");

        var isCreate = customerSuperVipDiscountId == 0;

        CustomerSuperVipDiscountInfo? customerSuperVipDiscount;

        if (isCreate)
        {
            var customerSuperVipDiscountExist = session.Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == customerId)
                .Take(1)
                .SingleOrDefault();

            if (customerSuperVipDiscountExist != null) throw new ValidationException($"指定的客户#{customerId}超级VIP设置已存在！");

            customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
        }
        else
        {
            var customerSuperVipDiscountExist = session.Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == customerId && e.Id != customerSuperVipDiscountId)
                .Take(1)
                .SingleOrDefault();

            if (customerSuperVipDiscountExist != null) throw new ValidationException($"指定的客户#{customerId}超级VIP设置已存在！");

            customerSuperVipDiscount = session.Get<CustomerSuperVipDiscountInfo>(customerSuperVipDiscountId);
        }

        customerSuperVipDiscount.Customer = customer;
        customerSuperVipDiscount.Description = model.CustomerSuperVipDiscount.Description;
        customerSuperVipDiscount.Discount = customerSuperVipDiscountFromUi.Discount;
        customerSuperVipDiscount.User = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        if (isCreate)
            session.Save(customerSuperVipDiscount);
        else
            session.Update(customerSuperVipDiscount);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var id in idList)
        {
            var customerSuperVipDiscount = session.Load<CustomerSuperVipDiscountInfo>(id);
            session.Delete(customerSuperVipDiscount);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/export")]
    public IActionResult Export()
    {
        var fileName = "超级VIP折扣.xlsx";
        var sheetName = "超级VIP折扣";

        //NPOI.xs book = new NPOI.HSSF.UserModel.HSSFWorkbook();
        IWorkbook workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet(sheetName);

        IList<CustomerSuperVipDiscountInfo> superVipDiscountList = session
            .Query<CustomerSuperVipDiscountInfo>()
            .OrderBy(e => e.Id)
            .ToList();

        var rowHeight = 22;
        var headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        var contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);

        // 会员 备注 折扣

        var rowIndex = 0;

        var row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        var cellIndex = 0;
        var cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("会员");

        cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("备注");

        cell = row.CreateCell(cellIndex++);
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("折扣%");

        foreach (var superVipDiscount in superVipDiscountList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 会员
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(superVipDiscount.Customer?.Id.ToString());

            // 备注
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(superVipDiscount.Description);

            // 折扣
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(superVipDiscount.Discount.ToString("#0.00"));
        }

        var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/import")]
    [Transactional]
    public IActionResult Import(IFormFile excelFile)
    {
        if (excelFile == null) throw new ValidationException("导入失败，文件未上传！");

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        var sheet = workbook.GetSheetAt(0);
        if (sheet == null) throw new ValidationException("导入的Excel文件应该至少有一个工作表！");

        var user = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        IList<CustomerSuperVipDiscountInfo> superVipDiscountList = new List<CustomerSuperVipDiscountInfo>();

        // 会员  备注  折扣 

        // 略过标题行，所以 rowIndex = 1 开始
        var lastRowNum = sheet.LastRowNum;
        for (var rowIndex = 1; rowIndex <= lastRowNum; rowIndex++)
        {
            var row = sheet.GetRow(rowIndex);

            if (row == null) break;

            // 设置单元格的格式为 String，这样后面用 StringCellValue 取值的时候，就不会出错了
            var cellIndex = 0;
            int lastCellNum = row.LastCellNum;
            for (cellIndex = 0; cellIndex < lastCellNum; cellIndex++)
            {
                var cell = row.GetCell(cellIndex);
                if (cell == null) cell = row.CreateCell(cellIndex);

                cell?.SetCellType(CellType.String);
            }

            cellIndex = 0;

            // 会员
            var customerIdStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(customerIdStr)) break;

            var customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerIdStr);
            if (customerId == 0) throw new ValidationException($"第{rowIndex}行数据 会员 数据格式有误，导入失败！");

            var customer = session.Get<CustomerInfo>(customerId);
            if (customer == null) throw new ValidationException($"第{rowIndex}行数据 会员 不存在，导入失败！");

            // 备注
            var description = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            // 折扣
            var mallDiscountStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(mallDiscountStr)) throw new ValidationException($"第{rowIndex}行数据 折扣 不能为空，导入失败！");

            if (decimal.TryParse(mallDiscountStr, out var discount) == false) throw new ValidationException($"第{rowIndex}行数据 折扣 数据格式有误 不能为空，导入失败！");


            var customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
            customerSuperVipDiscount.Customer = customer;
            customerSuperVipDiscount.Description = description;
            customerSuperVipDiscount.Discount = discount;
            customerSuperVipDiscount.User = user;

            superVipDiscountList.Add(customerSuperVipDiscount);
        }

        foreach (var discount in superVipDiscountList)
        {
            var vipDiscountInDb = session.Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == discount.Customer.Id)
                .Take(1)
                .SingleOrDefault();

            if (vipDiscountInDb == null)
            {
                vipDiscountInDb = new CustomerSuperVipDiscountInfo();
                vipDiscountInDb.Customer = discount.Customer;
            }

            vipDiscountInDb.Discount = discount.Discount;

            session.SaveOrUpdate(discount);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "导入成功！" }
        });
    }
}