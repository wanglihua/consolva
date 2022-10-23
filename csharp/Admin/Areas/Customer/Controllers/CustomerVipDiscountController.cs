using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Customer.Models.CustomerVipDiscount;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Customer.Controllers;

[CustomerArea]
[AdminAuth(PassRes.Customer.CustomerVipDiscount)]
public class CustomerVipDiscountController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private LoginUserService loginUserService;

    public CustomerVipDiscountController(ISession session, LoginUserService loginUserService)
    {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/customer/vip/discount")]
    public IActionResult Index()
    {
        CustomerVipDiscountIndexModel model = new CustomerVipDiscountIndexModel();
        return View("CustomerVipDiscountIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/list")]
    public IActionResult List(CustomerVipDiscountListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<CustomerVipDiscountInfo> customerVipDiscountList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerVipDiscountInfo>();

        CustomerVipDiscountListModel model = new CustomerVipDiscountListModel(listPager);
        model.CustomerVipDiscountList = customerVipDiscountList;

        return View("CustomerVipDiscountList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerVipDiscountListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<CustomerVipDiscountInfo>();

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        CustomerVipDiscountInfo customerVipDiscount;

        if (id == null || id.Value == 0)
        {
            customerVipDiscount = new CustomerVipDiscountInfo();
        }
        else
        {
            customerVipDiscount = session.Get<CustomerVipDiscountInfo>(id.Value);
            if (customerVipDiscount == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        CustomerVipDiscountDetailModel model = new CustomerVipDiscountDetailModel();
        model.CustomerVipDiscount = customerVipDiscount;

        return View("CustomerVipDiscountDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/save")]
    [Transactional]
    public IActionResult Save(CustomerVipDiscountDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        UserInfo loginUser = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        CustomerVipDiscountInfo customerVipDiscountFromUi = model.CustomerVipDiscount;
        int customerVipDiscountId = customerVipDiscountFromUi.Id;

        bool isCreate = customerVipDiscountId == 0;

        CustomerVipDiscountInfo customerVipDiscount;

        if (isCreate)
        {
            customerVipDiscount = new CustomerVipDiscountInfo();
        }
        else
        {
            customerVipDiscount = session.Get<CustomerVipDiscountInfo>(customerVipDiscountId);
        }

        customerVipDiscount.Rank = customerVipDiscountFromUi.Rank;
        customerVipDiscount.Score = customerVipDiscountFromUi.Score;
        customerVipDiscount.Discount = customerVipDiscountFromUi.Discount;
        customerVipDiscount.User = loginUser;

        if (isCreate)
        {
            session.Save(customerVipDiscount);
        }
        else
        {
            session.Update(customerVipDiscount);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            CustomerVipDiscountInfo customerVipDiscount = session.Load<CustomerVipDiscountInfo>(id);
            session.Delete(customerVipDiscount);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/export")]
    public IActionResult Export()
    {
        string fileName = "VIP折扣设置.xlsx";
        string sheetName = "VIP折扣设置";

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        IList<CustomerVipDiscountInfo> vipDiscountList = session
            .Query<CustomerVipDiscountInfo>()
            .OrderBy(e => e.Id)
            .ToList();

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);

        // 会员VIP等级    历史总积分    折扣

        int rowIndex = 0;

        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        int cellIndex = 0;
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("会员VIP等级");

        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("历史总积分");

        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("折扣%");

        foreach (CustomerVipDiscountInfo vipDiscount in vipDiscountList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 会员VIP等级
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(vipDiscount.Rank.Name);

            // 历史总积分
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(vipDiscount.Score.ToString("#0.00"));

            // 折扣
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(vipDiscount.Discount.ToString("#0.00"));
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/import")]
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

        IList<CustomerVipDiscountInfo> vipDiscountList = new List<CustomerVipDiscountInfo>();

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

                cell.SetCellType(CellType.String);
            }

            cellIndex = 0;

            // 会员VIP等级
            string rankName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rankName))
            {
                break;
            }

            CustomerVipRank rank = CustomerVipRank.GetByName(rankName);
            if (rank == CustomerVipRank.None)
            {
                throw new ValidationException($"第{rowIndex}行数据 会员VIP等级 为空或数据不合法，导入失败！");
            }

            // 历史总积分
            string scoreStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(scoreStr))
            {
                throw new ValidationException($"第{rowIndex}行数据 历史总积分 不能为空，导入失败！");
            }

            if (decimal.TryParse(scoreStr, out decimal score) == false)
            {
                throw new ValidationException($"第{rowIndex}行数据 历史总积分 数据格式有误，导入失败！");
            }

            // 折扣
            string discountStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(discountStr))
            {
                throw new ValidationException($"第{rowIndex}行数据 折扣 不能为空，导入失败！");
            }

            if (decimal.TryParse(discountStr, out decimal Discount) == false)
            {
                throw new ValidationException($"第{rowIndex}行数据 折扣 数据格式有误，导入失败！");
            }

            CustomerVipDiscountInfo vipDiscount = new CustomerVipDiscountInfo();
            vipDiscount.Rank = rank;
            vipDiscount.Score = score;
            vipDiscount.Discount = Discount;
            vipDiscount.User = user;

            vipDiscountList.Add(vipDiscount);
        }


        foreach (CustomerVipDiscountInfo vipDiscount in vipDiscountList)
        {
            CustomerVipDiscountInfo? vipDiscountInDb = session.Query<CustomerVipDiscountInfo>()
                .Where(e => e.Rank == vipDiscount.Rank)
                .Take(1)
                .SingleOrDefault();

            if (vipDiscountInDb == null)
            {
                vipDiscountInDb = new CustomerVipDiscountInfo();
                vipDiscountInDb.Rank = vipDiscount.Rank;
            }

            vipDiscountInDb.Score = vipDiscount.Score;
            vipDiscountInDb.Discount = vipDiscount.Discount;

            session.SaveOrUpdate(vipDiscount);
        }

        session.Flush();


        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "导入成功！" },
        });
    }
}