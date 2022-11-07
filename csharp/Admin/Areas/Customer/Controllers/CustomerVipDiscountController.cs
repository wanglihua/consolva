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
    private readonly LoginUserService loginUserService;

    private readonly ISession session;

    public CustomerVipDiscountController(ISession session, LoginUserService loginUserService)
    {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/customer/vip/discount")]
    public IActionResult Index()
    {
        var model = new CustomerVipDiscountIndexModel();
        return View("CustomerVipDiscountIndex", model);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/list")]
    public IActionResult List(CustomerVipDiscountListQueryParam queryParam)
    {
        var totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        var listPager = new ListPager(queryParam, totalCount);

        var customerVipDiscountList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<CustomerVipDiscountInfo>();

        var model = new CustomerVipDiscountListModel(listPager);
        model.CustomerVipDiscountList = customerVipDiscountList;

        return View("CustomerVipDiscountList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(CustomerVipDiscountListQueryParam queryParam)
    {
        var criteria = session.CreateCriteria<CustomerVipDiscountInfo>();

        criteria = criteria.SetReadOnly(true);

        return criteria;
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        CustomerVipDiscountInfo customerVipDiscount;

        var isCreate = false;
        if (id == null || id.Value == 0)
        {
            isCreate = true;

            customerVipDiscount = new CustomerVipDiscountInfo();
        }
        else
        {
            isCreate = false;

            customerVipDiscount = session.Get<CustomerVipDiscountInfo>(id.Value);
            if (customerVipDiscount == null) throw new ValidationException("指定的数据项不存在！");
        }

        var model = new CustomerVipDiscountDetailModel();
        model.CustomerVipDiscountId = customerVipDiscount.Id;
        model.Rank = customerVipDiscount.Rank;
        model.Score = customerVipDiscount.Score;
        model.Discount = customerVipDiscount.Discount;

        model.IsCreate = isCreate;

        return View("CustomerVipDiscountDetail", model);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/save")]
    [Transactional]
    public IActionResult Save(CustomerVipDiscountDetailModel model)
    {
        if (!ModelState.IsValid) throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));

        var rank = model.Rank;

        if (rank == null || rank == CustomerVipRank.None) throw new ValidationException("指定的客户VIP级别不存在！");

        var loginUser = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        var customerVipDiscountId = model.CustomerVipDiscountId;

        var isCreate = customerVipDiscountId == 0;

        CustomerVipDiscountInfo customerVipDiscount;

        if (isCreate)
        {
            var vipDiscount = session.Query<CustomerVipDiscountInfo>()
                .Where(e => e.Rank == rank)
                .Take(1)
                .SingleOrDefault();

            if (vipDiscount != null) throw new ValidationException($"指定的客户VIP级别#{rank.Name}折扣设置已经存在！");

            customerVipDiscount = new CustomerVipDiscountInfo();
            customerVipDiscount.Rank = model.Rank; // 编辑时这个级别不能修改，所以只在新建时设置保存
        }
        else
        {
            var vipDiscount = session.Query<CustomerVipDiscountInfo>()
                .Where(e => e.Rank == rank && e.Id != customerVipDiscountId)
                .Take(1)
                .SingleOrDefault();

            if (vipDiscount != null) throw new ValidationException($"指定的客户VIP级别#{rank.Name}折扣设置已经存在！");

            customerVipDiscount = session.Get<CustomerVipDiscountInfo>(customerVipDiscountId);
        }

        customerVipDiscount.Score = model.Score;
        customerVipDiscount.Discount = model.Discount;
        customerVipDiscount.User = loginUser;

        if (isCreate)
            session.Save(customerVipDiscount);
        else
            session.Update(customerVipDiscount);

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (var id in idList)
        {
            var customerVipDiscount = session.Load<CustomerVipDiscountInfo>(id);
            session.Delete(customerVipDiscount);
        }

        session.Flush();

        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "删除成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/export")]
    public IActionResult Export()
    {
        var fileName = "VIP折扣设置.xlsx";
        var sheetName = "VIP折扣设置";

        IWorkbook workbook = new XSSFWorkbook();
        var sheet = workbook.CreateSheet(sheetName);

        IList<CustomerVipDiscountInfo> vipDiscountList = session
            .Query<CustomerVipDiscountInfo>()
            .OrderBy(e => e.Id)
            .ToList();

        var rowHeight = 22;
        var headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        var contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);

        // 会员VIP等级    历史总积分    折扣

        var rowIndex = 0;

        var row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        var cellIndex = 0;
        var cell = row.CreateCell(cellIndex++);
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

        foreach (var vipDiscount in vipDiscountList)
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

        var memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }

    [HttpPost]
    [Route("/admin/customer/vip/discount/import")]
    public IActionResult Import(IFormFile excelFile)
    {
        if (excelFile == null) throw new ValidationException("导入失败，文件未上传！");

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        var sheet = workbook.GetSheetAt(0);
        if (sheet == null) throw new ValidationException("导入的Excel文件应该至少有一个工作表！");

        var user = session.Load<UserInfo>(loginUserService.GetLoginUserId());

        IList<CustomerVipDiscountInfo> vipDiscountList = new List<CustomerVipDiscountInfo>();

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

                cell.SetCellType(CellType.String);
            }

            cellIndex = 0;

            // 会员VIP等级
            var rankName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rankName)) break;

            var rank = CustomerVipRank.GetByName(rankName);
            if (rank == CustomerVipRank.None) throw new ValidationException($"第{rowIndex}行数据 会员VIP等级 为空或数据不合法，导入失败！");

            // 历史总积分
            var scoreStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(scoreStr)) throw new ValidationException($"第{rowIndex}行数据 历史总积分 不能为空，导入失败！");

            if (decimal.TryParse(scoreStr, out var score) == false) throw new ValidationException($"第{rowIndex}行数据 历史总积分 数据格式有误，导入失败！");

            // 折扣
            var discountStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(discountStr)) throw new ValidationException($"第{rowIndex}行数据 折扣 不能为空，导入失败！");

            if (decimal.TryParse(discountStr, out var Discount) == false) throw new ValidationException($"第{rowIndex}行数据 折扣 数据格式有误，导入失败！");

            var vipDiscount = new CustomerVipDiscountInfo();
            vipDiscount.Rank = rank;
            vipDiscount.Score = score;
            vipDiscount.Discount = Discount;
            vipDiscount.User = user;

            vipDiscountList.Add(vipDiscount);
        }


        foreach (var vipDiscount in vipDiscountList)
        {
            var vipDiscountInDb = session.Query<CustomerVipDiscountInfo>()
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


        return Json(new Dictionary<string, object> {
            { "success", true },
            { "message", "导入成功！" }
        });
    }
}