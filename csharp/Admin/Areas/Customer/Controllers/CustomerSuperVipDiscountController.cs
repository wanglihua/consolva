using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Customer.Models.CustomerSuperVipDiscount;
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

    private ISession session;
    private CustomerCodeService customerCodeService;
    private LoginUserService loginUserService;

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
        CustomerSuperVipDiscountIndexModel model = new CustomerSuperVipDiscountIndexModel();
        return View("CustomerSuperVipDiscountIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/list")]
    public IActionResult List(CustomerSuperVipDiscountListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerSuperVipDiscountInfo> customerSuperVipDiscountList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerSuperVipDiscountInfo>();

        foreach (CustomerSuperVipDiscountInfo customerSuperVipDiscount in customerSuperVipDiscountList)
        {
            customerSuperVipDiscount.CustomerCodeCache = customerCodeService.GetCustomerCode(customerSuperVipDiscount.Customer?.Id ?? 0);
        }

        CustomerSuperVipDiscountListModel model = new CustomerSuperVipDiscountListModel(listPager);
        model.CustomerSuperVipDiscountList = customerSuperVipDiscountList;

        return View("CustomerSuperVipDiscountList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerSuperVipDiscountListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerSuperVipDiscountInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.CustomerCode) == false)
        {
            int customerId = customerCodeService.GetCustomerIdFromCustomerCode(queryParam.CustomerCode);
            criteria = criteria.Add(Restrictions.Where<CustomerSuperVipDiscountInfo>(e => e.Customer.Id == customerId));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        CustomerSuperVipDiscountInfo customerSuperVipDiscount;

        CustomerSuperVipDiscountDetailModel model = new CustomerSuperVipDiscountDetailModel();

        if (id == null || id.Value == 0)
        {
            customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
        }
        else
        {
            customerSuperVipDiscount = session.Get<CustomerSuperVipDiscountInfo>(id.Value);
            if (customerSuperVipDiscount == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }

            customerSuperVipDiscount.CustomerCodeCache = customerCodeService.GetCustomerCode(customerSuperVipDiscount.Customer?.Id ?? 0);
        }

        model.CustomerSuperVipDiscount = customerSuperVipDiscount;

        return View("CustomerSuperVipDiscountDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/save")]
    public IActionResult Save(CustomerSuperVipDiscountDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        CustomerSuperVipDiscountInfo customerSuperVipDiscountFromUi = model.CustomerSuperVipDiscount;
        int customerSuperVipDiscountId = customerSuperVipDiscountFromUi.Id;

        int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerSuperVipDiscountFromUi.CustomerCodeCache);
        if (customerId == 0)
        {
            throw new ValidationException("指定的客户ID格式不对或客户ID未指定！");
        }

        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的客户不存在！");
        }

        bool isCreate = customerSuperVipDiscountId == 0;

        CustomerSuperVipDiscountInfo customerSuperVipDiscount;

        if (isCreate)
        {
            customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
        }
        else
        {
            customerSuperVipDiscount = session.Get<CustomerSuperVipDiscountInfo>(customerSuperVipDiscountId);
        }

        customerSuperVipDiscount.Customer = customer;
        customerSuperVipDiscount.Description = model.CustomerSuperVipDiscount.Description;
        customerSuperVipDiscount.Discount = customerSuperVipDiscountFromUi.Discount;
        customerSuperVipDiscount.User = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        if (isCreate)
        {
            session.Save(customerSuperVipDiscount);
        }
        else
        {
            session.Update(customerSuperVipDiscount);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            CustomerSuperVipDiscountInfo customerSuperVipDiscount = session.Load<CustomerSuperVipDiscountInfo>(id);
            session.Delete(customerSuperVipDiscount);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/export")]
    public IActionResult Export()
    {
        string fileName = "超级VIP折扣.xlsx";
        string sheetName = "超级VIP折扣";

        //NPOI.xs book = new NPOI.HSSF.UserModel.HSSFWorkbook();
        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        IList<CustomerSuperVipDiscountInfo> superVipDiscountList = session
            .Query<CustomerSuperVipDiscountInfo>()
            .OrderBy(e => e.Id)
            .ToList();

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);

        // 会员 备注 折扣

        int rowIndex = 0;

        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        int cellIndex = 0;
        ICell cell = row.CreateCell(cellIndex++);
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

        foreach (CustomerSuperVipDiscountInfo superVipDiscount in superVipDiscountList)
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

            // 商城折扣
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(superVipDiscount.Discount.ToString("#0.00"));
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }

    [HttpPost]
    [Route("/admin/customer/super/vip/discount/import")]
    [Transactional]
    public IActionResult Import(IFormFile excelFile)
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

        UserInfo user = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        IList<CustomerSuperVipDiscountInfo> superVipDiscountList = new List<CustomerSuperVipDiscountInfo>();

        // 会员  备注  商城折扣    集运头程折扣    集运二程折扣

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
                ICell cell = row.GetCell(cellIndex);
                if (cell == null)
                {
                    cell = row.CreateCell(cellIndex);
                }

                cell?.SetCellType(CellType.String);
            }

            cellIndex = 0;

            // 会员
            string customerIdStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(customerIdStr))
            {
                break;
            }

            int customerId = customerCodeService.GetCustomerIdFromCustomerCode(customerIdStr);
            if (customerId == 0)
            {
                throw new ValidationException($"第{rowIndex}行数据 会员 数据格式有误，导入失败！");
            }

            CustomerInfo customer = session.Get<CustomerInfo>(customerId);
            if (customer == null)
            {
                throw new ValidationException($"第{rowIndex}行数据 会员 不存在，导入失败！");
            }

            // 备注
            string description = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            // 折扣
            string mallDiscountStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(mallDiscountStr))
            {
                throw new ValidationException($"第{rowIndex}行数据 折扣 不能为空，导入失败！");
            }

            if (decimal.TryParse(mallDiscountStr, out decimal discount) == false)
            {
                throw new ValidationException($"第{rowIndex}行数据 折扣 数据格式有误 不能为空，导入失败！");
            }


            CustomerSuperVipDiscountInfo customerSuperVipDiscount = new CustomerSuperVipDiscountInfo();
            customerSuperVipDiscount.Customer = customer;
            customerSuperVipDiscount.Description = description;
            customerSuperVipDiscount.Discount = discount;
            customerSuperVipDiscount.User = user;

            superVipDiscountList.Add(customerSuperVipDiscount);
        }

        foreach (CustomerSuperVipDiscountInfo discount in superVipDiscountList)
        {
            CustomerSuperVipDiscountInfo? vipDiscountInDb = session.Query<CustomerSuperVipDiscountInfo>()
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

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "导入成功！" },
        });
    }
}