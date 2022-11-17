using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Logistics.Models.HsGoods;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[AdminAuth(PassRes.Goods.HsGoods)]
public class HsGoodsController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private LoginUserService loginUserService;

    public HsGoodsController(ISession session, LoginUserService loginUserService)
    {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/hs/goods")]
    public IActionResult Index()
    {
        HsGoodsIndexModel model = new HsGoodsIndexModel();
        return View("HsGoodsIndex", model);
    }

    [HttpPost]
    [Route("/admin/hs/goods/list")]
    public IActionResult List(HsGoodsListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<HsGoodsInfo> hsGoodsList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(EntityNoUpdate.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<HsGoodsInfo>();

        foreach (HsGoodsInfo hsGoodsInfo in hsGoodsList)
        {
            // hsGoodsInfo
        }

        HsGoodsListModel model = new HsGoodsListModel(listPager);
        model.HsGoodsList = hsGoodsList;

        return View("HsGoodsList", model);
    }

    [NonAction]
    private ICriteria GetListCriteria(HsGoodsListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<HsGoodsInfo>();

        criteria = criteria.SetReadOnly(true);

        if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false)
        {
            string cnHsNameQuery = queryParam.CnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<HsGoodsInfo>(e => e.CnHsName).IsInsensitiveLike(cnHsNameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpGet]
    [Route("/admin/hs/goods/detail/{id:int?}")]
    public IActionResult Detail(int? id)
    {
        session.DefaultReadOnly = true;

        HsGoodsInfo hsGoods;

        if (id == null || id.Value == 0)
        {
            hsGoods = new HsGoodsInfo();
        }
        else
        {
            hsGoods = session.Get<HsGoodsInfo>(id.Value);
            if (hsGoods == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }
        }

        HsGoodsDetailModel model = new HsGoodsDetailModel();
        model.HsGoods = hsGoods;

        return View("HsGoodsDetail", model);
    }

    [HttpPost]
    [Route("/admin/hs/goods/save")]
    [Transactional]
    public IActionResult Save(HsGoodsDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId);

        HsGoodsInfo hsGoodsFromUi = model.HsGoods;
        int hsGoodsIdFromUi = hsGoodsFromUi.Id;

        bool isCreate = hsGoodsIdFromUi == 0;

        HsGoodsInfo hsGoods;

        if (isCreate)
        {
            hsGoods = new HsGoodsInfo();
            hsGoods.AddUser = loginUser;
        }
        else
        {
            hsGoods = session.Get<HsGoodsInfo>(hsGoodsIdFromUi);
        }

        hsGoods.CnHsCode = hsGoodsFromUi.CnHsCode;
        hsGoods.CnHsName = hsGoodsFromUi.CnHsName;
        hsGoods.EnHsCode = hsGoodsFromUi.EnHsCode;
        hsGoods.EnHsName = hsGoodsFromUi.EnHsName;
        hsGoods.Elem01 = hsGoodsFromUi.Elem01;
        hsGoods.Elem02 = hsGoodsFromUi.Elem02;
        hsGoods.Elem03 = hsGoodsFromUi.Elem03;
        hsGoods.Elem04 = hsGoodsFromUi.Elem04;
        hsGoods.Elem05 = hsGoodsFromUi.Elem05;
        hsGoods.Elem06 = hsGoodsFromUi.Elem06;
        hsGoods.Elem07 = hsGoodsFromUi.Elem07;
        hsGoods.Elem08 = hsGoodsFromUi.Elem08;

        if (isCreate)
        {
            session.Save(hsGoods);
        }
        else
        {
            session.Update(hsGoods);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
            { "isCreate", isCreate },
            { "id", hsGoods.Id }
        });
    }

    [HttpPost]
    [Route("/admin/hs/goods/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            HsGoodsInfo hsGoods = session.Get<HsGoodsInfo>(id);

            int count = session.Query<LogisticsGoodsInfo>().Where(e => e.HsGoods!.Id == id).Count();
            if (count != 0)
            {
                throw new ValidationException($"海关品名 {hsGoods.CnHsName} 已在扩展名中使用中，不能删除！");
            }

            session.Delete(hsGoods);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/hs/goods/detail/data/{id:int}")]
    public IActionResult DetailData(int id)
    {
        HsGoodsInfo hsGoodsInfo = session.Get<HsGoodsInfo>(id);
        if (hsGoodsInfo == null)
        {
            throw new ValidationException("指定的海关物品不存在！");
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取成功！" },
            { "hsGoods", hsGoodsInfo },
        });
    }

    [HttpPost]
    [Route("/admin/hs/goods/import")]
    public IActionResult Import(IFormFile excelFile)
    {
        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId);

        if (excelFile == null)
        {
            throw new ValidationException("导入失败，文件未上传！");
        }

        IList<string> cnHsNameList = session
            .Query<HsGoodsInfo>()
            .Select(e => e.CnHsName)
            .Distinct()
            .ToList();

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet == null)
        {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        IList<HsGoodsInfo> hsGoodsList = new List<HsGoodsInfo>();

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


            string cnHsName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cnHsName))
            {
                break;
            }

            if (cnHsNameList.Contains(cnHsName))
            {
                throw new ValidationException($"第{rowIndex}行数据 中国海关申报名 已存在或重复，导入失败！");
            }

            cnHsNameList.Add(cnHsName);

            string cnHsCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(cnHsCode))
            {
                throw new ValidationException($"第{rowIndex}行数据 中国海关编码 不能为空，导入失败！");
            }

            string enHsName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(enHsName))
            {
                throw new ValidationException($"第{rowIndex}行数据 英文海关申报名 不能为空，导入失败！");
            }

            string enHsCode = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(enHsCode))
            {
                throw new ValidationException($"第{rowIndex}行数据 英文海关编码 不能为空，导入失败！");
            }

            HsGoodsInfo hsGoods = new HsGoodsInfo();
            hsGoods.CnHsName = cnHsName;
            hsGoods.CnHsCode = cnHsCode;
            hsGoods.EnHsName = enHsName;
            hsGoods.EnHsCode = enHsCode;
            hsGoods.Elem01 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem02 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem03 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem04 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem05 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem06 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem07 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.Elem08 = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            hsGoods.AddUser = loginUser;

            hsGoodsList.Add(hsGoods);
        }

        ITransaction? transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            foreach (HsGoodsInfo hsGoods in hsGoodsList)
            {
                session.Save(hsGoods);
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

    [HttpPost]
    [Route("/admin/hs/goods/export")]
    public IActionResult Export(HsGoodsListQueryParam queryParam)
    {
        string fileName = "海关品名" + ".xlsx";
        string sheetName = "海关品名";

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        var query = session
            .Query<HsGoodsInfo>();

        if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false)
        {
            query = query.Where(e => e.CnHsName.Contains(queryParam.CnHsNameQuery));
        }

        IList<HsGoodsInfo> hsGoodsList = query
            .OrderBy(e => e.CnHsName)
            .ToList();

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);

        // 中国海关申报名    中国海关编码    英文海关申报名    英文海关编码    中国申报要素1    中国申报要素2    中国申报要素3    中国申报要素4    中国申报要素5    中国申报要素6    中国申报要素7    中国申报要素8
        int rowIndex = 0;

        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        int cellIndex = 0;

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国海关申报名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国海关编码");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("英文海关申报名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("英文海关编码");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素1");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素2");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素3");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素4");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素5");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素6");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素7");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国申报要素8");

        foreach (HsGoodsInfo hsGoods in hsGoodsList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 中国海关申报名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.CnHsName);

            // 中国海关编码
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.CnHsCode);

            // 英文海关申报名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.EnHsName);

            // 英文海关编码
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.EnHsCode);

            // 中国申报要素1
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem01);

            // 中国申报要素2
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem02);

            // 中国申报要素3
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem03);

            // 中国申报要素4
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem04);

            // 中国申报要素5
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem05);

            // 中国申报要素6
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem06);

            // 中国申报要素7
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem07);

            // 中国申报要素8
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(hsGoods.Elem08);
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }
}