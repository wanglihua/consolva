using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Logistics.Models.HsGoods;
using shunda.Admin.Areas.Logistics.Models.LogisticsGoodsTrade;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Order;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Goods.GoodsTrade)]
public class LogisticsGoodsTradeController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private LoginUserService loginUserService;
    private LogisticsGoodsImageService logisticsGoodsImageService;
    private LogisticsGoodsLogService logisticsGoodsLogService;

    public LogisticsGoodsTradeController(
        ISession session,
        LoginUserService loginUserService,
        LogisticsGoodsImageService logisticsGoodsImageService,
        LogisticsGoodsLogService logisticsGoodsLogService)
    {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsGoodsImageService = logisticsGoodsImageService;
        this.logisticsGoodsLogService = logisticsGoodsLogService;
    }

    [HttpGet]
    [Route("/admin/logistics/goods/trade/{statusId:int}")]
    public IActionResult Index(int statusId)
    {
        LogisticsGoodsTradeIndexModel model = new LogisticsGoodsTradeIndexModel();

        model.StatusId = statusId;
        model.GoodsTypeList = LogisticsGoodsType.GetInnerList();

        return View("LogisticsGoodsTradeIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/list")]
    public IActionResult List(TradeGoodsSearchListQueryParam queryParam)
    {
        if (string.IsNullOrWhiteSpace(queryParam.UpdateUserQuery) == false)
        {
            string updateUserQuery = queryParam.UpdateUserQuery.Trim();
            queryParam.UpdateUserId = session.Query<UserInfo>()
                .Where(e => e.LoginName == updateUserQuery || e.UserName == updateUserQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        int totalCount = GetTradeGoodsSearchListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsGoodsInfo> logisticsGoodsList = GetTradeGoodsSearchListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsGoodsInfo>();

        IList<string> createExpressTrackingNoList = logisticsGoodsList.Select(e => e.CreateExpressTrackingNo).ToList();
        IList<ExpressPhotoInfo> createExpressImageListAll = logisticsGoodsImageService.GetGoodsCreateExpressImageList(createExpressTrackingNoList);

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
        {
            logisticsGoods.CreateExpressPhotoListCache = createExpressImageListAll.Where(e => e.TrackingNo == logisticsGoods.CreateExpressTrackingNo).OrderBy(e => e.Id).ToList();

            IList<ExpressPhotoInfo> goodsExpressPhotoList = logisticsGoodsImageService.GetGoodsRecentOrderImageList(logisticsGoods.Id);

            IList<int> orderIdList = goodsExpressPhotoList.Where(e => e.Order != null).Select(e => e.Order!.Id).Distinct().OrderByDescending(e => e).ToList();

            if (orderIdList.Count > 0)
            {
                int orderId = orderIdList[0];
                logisticsGoods.FirstOrderExpressPhotoListCache = goodsExpressPhotoList.Where(e => e.Order != null && e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
            }

            if (orderIdList.Count > 1)
            {
                int orderId = orderIdList[1];
                logisticsGoods.SecondOrderExpressPhotoListCache = goodsExpressPhotoList.Where(e => e.Order != null && e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
            }

            if (orderIdList.Count > 2)
            {
                int orderId = orderIdList[2];
                logisticsGoods.ThirdOrderExpressPhotoListCache = goodsExpressPhotoList.Where(e => e.Order != null && e.Order!.Id == orderId).OrderBy(e => e.Id).ToList();
            }
        }

        LogisticsGoodsTradeListModel model = new LogisticsGoodsTradeListModel(listPager);
        model.LogisticsGoodsList = logisticsGoodsList;

        return View("LogisticsGoodsTradeList", model);
    }

    [NonAction]
    public ICriteria GetTradeGoodsSearchListCriteria(TradeGoodsSearchListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);
        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.UpdateUser), nameof(LogisticsGoodsInfo.UpdateUser), JoinType.LeftOuterJoin);

        if (queryParam.StatusQuery != null)
        {
            LogisticsGoodsStatus statusQuery = queryParam.StatusQuery;

            if (statusQuery == LogisticsGoodsStatus.NotConfirmed)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status == LogisticsGoodsStatus.PhotoRequired || e.Status == LogisticsGoodsStatus.NotConfirmed));
            }

            if (statusQuery == LogisticsGoodsStatus.Confirmed)
            {
                criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Status == LogisticsGoodsStatus.Confirmed));
            }
        }

        if (queryParam.TypeQuery != null && queryParam.TypeQuery != LogisticsGoodsType.None)
        {
            LogisticsGoodsType logisticsGoodsType = queryParam.TypeQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Type == logisticsGoodsType));
        }


        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CnHsCodeQuery) == false)
        {
            string cnHsCodeQuery = queryParam.CnHsCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.CnHsCode)}", cnHsCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.CnHsNameQuery) == false)
        {
            string cnHsNameQuery = queryParam.CnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.CnHsName)}", cnHsNameQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.EnHsCodeQuery) == false)
        {
            string enHsCodeQuery = queryParam.EnHsCodeQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.EnHsCode)}", enHsCodeQuery));
        }

        if (string.IsNullOrWhiteSpace(queryParam.EnHsNameQuery) == false)
        {
            string enHsNameQuery = queryParam.EnHsNameQuery.Trim();
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.HsGoods)}.{nameof(LogisticsGoodsInfo.HsGoods.EnHsName)}", enHsNameQuery));
        }

        if (queryParam.UpdateUserId != 0)
        {
            int updateUserId = queryParam.UpdateUserId;
            criteria = criteria.Add(Restrictions.Eq($"{nameof(LogisticsGoodsInfo.UpdateUser)}.{nameof(UserInfo.Id)}", updateUserId));
        }

        if (queryParam.BeginUpdateDateQuery != null)
        {
            DateTime beginUpdateDataQuery = queryParam.BeginUpdateDateQuery.Value.Date;
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.UpdateTime >= beginUpdateDataQuery));
        }

        if (queryParam.EndUpdateDateQuery != null)
        {
            DateTime endUpdateDataQuery = queryParam.EndUpdateDateQuery.Value.Date.AddDays(1);
            criteria = criteria.Add(Restrictions.Where<LogisticsOrderInfo>(e => e.UpdateTime < endUpdateDataQuery));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        IList<int> goodsIdList = idList.ToList();

        IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Id))
            .ToList();

        IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Goods.Id))
            .ToList();

        IList<LogisticsGoodsImageInfo> logisticsGoodsImageList = session.Query<LogisticsGoodsImageInfo>()
            .Where(e => goodsIdList.Contains(e.Goods.Id))
            .ToList();

        foreach (int goodsId in goodsIdList)
        {
            LogisticsGoodsInfo? logisticsGoods = logisticsGoodsList.Where(e => e.Id == goodsId).Take(1).SingleOrDefault();
            if (logisticsGoods == null)
            {
                throw new ValidationException($"指定的物品不存在！#{goodsId}");
            }

            var orderGoods = expressGoodsList
                .Where(e => e.Goods.Id == goodsId)
                .Take(1)
                .SingleOrDefault();

            if (orderGoods != null)
            {
                throw new ValidationException($"物品 {logisticsGoods.Name}, 在快递单物品中有被引用，不能删除！");
            }
        }

        foreach (var goodsImage in logisticsGoodsImageList)
        {
            session.Delete(goodsImage);
        }

        foreach (var logisticsGoods in logisticsGoodsList)
        {
            session.Delete(logisticsGoods);
        }

        session.Flush();

        foreach (var goodsImage in logisticsGoodsImageList)
        {
            try
            {
                logisticsGoodsImageService.DeleteFile(goodsImage);
            }
            catch (Exception exp)
            {
                log.Error(exp);
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/show/select/hs/code")]
    public IActionResult ShowSelectHsCode()
    {
        LogisticsGoodsTradeSelectHsCodeModel model = new LogisticsGoodsTradeSelectHsCodeModel();

        return View("LogisticsGoodsTradeSelectHsCode", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/show/select/hs/code/list")]
    public IActionResult ShowSelectHsCodeList(HsGoodsListQueryParam queryParam)
    {
        int totalCount = GetShowSelectHsCodeListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<HsGoodsInfo> hsGoodsList = GetShowSelectHsCodeListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<HsGoodsInfo>();

        foreach (HsGoodsInfo hsGoods in hsGoodsList)
        {
            // hsGoods
        }

        HsGoodsListModel model = new HsGoodsListModel(listPager);
        model.HsGoodsList = hsGoodsList;

        return View("LogisticsGoodsTradeSelectHsCodeList", model);
    }

    [NonAction]
    private ICriteria GetShowSelectHsCodeListCriteria(HsGoodsListQueryParam queryParam)
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

    [HttpPost]
    [Route("/admin/logistics/goods/trade/set/no/photo/status")]
    [Transactional]
    public IActionResult SetNoPhotoStatus(int[] idList)
    {
        IList<int> goodsIdList = idList.ToList();

        if (goodsIdList == null || goodsIdList.Count == 0)
        {
            throw new ValidationException("请至少选择一条数据！");
        }

        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId)!;

        IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
            .Where(e => goodsIdList.Contains(e.Id))
            .ToList();

        foreach (int goodsId in goodsIdList)
        {
            LogisticsGoodsInfo? logisticsGoods = logisticsGoodsList
                .Where(e => e.Id == goodsId)
                .Take(1)
                .SingleOrDefault();

            if (logisticsGoods == null)
            {
                throw new ValidationException($"指定的物品#{goodsId}不存在！");
            }
        }

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
        {
            logisticsGoods.UpdateUser = loginUser;
            logisticsGoods.Status = LogisticsGoodsStatus.PhotoRequired;
            session.Update(logisticsGoods);
        }

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
        {
            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.SetPhotoRequire, logisticsGoods.Id, loginUserId);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "手工设置需拍照状态成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/import")]
    public IActionResult Import(IFormFile excelFile)
    {
        if (excelFile == null)
        {
            throw new ValidationException("导入失败，文件未上传！");
        }

        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId)!;

        // todo a 后面改造
        IList<HsGoodsInfo> hsGoodsList = session
            .Query<HsGoodsInfo>()
            .ToList();

        IWorkbook workbook = new XSSFWorkbook(excelFile.OpenReadStream());
        ISheet sheet = workbook.GetSheetAt(0);
        if (sheet == null)
        {
            throw new ValidationException("导入的Excel文件应该至少有一个工作表！");
        }

        IList<LogisticsGoodsInfo> logisticsGoodsList = new List<LogisticsGoodsInfo>();

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

            cellIndex = 0;

            // 扩展名
            string name = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                break;
            }

            name = name.Trim();

            // 物品类型
            string goodsTypeStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            if (string.IsNullOrWhiteSpace(goodsTypeStr))
            {
                throw new ValidationException($"第{rowIndex}行数据 物品类型 不能为空，导入失败！");
            }

            LogisticsGoodsType goodsType = LogisticsGoodsType.GetByName(goodsTypeStr.Trim());
            if (goodsType == LogisticsGoodsType.None)
            {
                throw new ValidationException($"第{rowIndex}行数据 物品类型 数据有误，导入失败！");
            }

            // 品牌
            string brandName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            LogisticsGoodsInfo? logisticsGoodsInDb = session
                .Query<LogisticsGoodsInfo>()
                .Where(e => e.Name == name
                            && e.Type == goodsType
                            && e.BrandName == brandName)
                .Take(1)
                .SingleOrDefault();
            if (logisticsGoodsInDb == null)
            {
                throw new ValidationException($"第{rowIndex}行数据指定的 扩展名+特征属性 不存在，导入失败！");
            }

            logisticsGoodsInDb.Name = name;
            logisticsGoodsInDb.Type = goodsType;
            logisticsGoodsInDb.BrandName = brandName;

            // 中国海关申报名
            string cnHsName = row.GetCell(cellIndex++)?.StringCellValue;

            if (string.IsNullOrWhiteSpace(cnHsName))
            {
                throw new ValidationException($"第{rowIndex}行数据 中国海关申报名 不能为空，导入失败！");
            }

            cnHsName = cnHsName.Trim();

            HsGoodsInfo? hsGoods = hsGoodsList
                .Where(e => e.CnHsName == cnHsName)
                .Take(1)
                .SingleOrDefault();

            if (hsGoods == null)
            {
                throw new ValidationException($"第{rowIndex}行数据指定的 中国海关申报名 不存在，导入失败！");
            }

            logisticsGoodsInDb.HsGoods = hsGoods;

            logisticsGoodsList.Add(logisticsGoodsInDb);
        }

        // 数据库事务
        Transactional.Wrap(session, () => {
            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
            {
                logisticsGoods.UpdateUser = loginUser;
                logisticsGoods.Status = LogisticsGoodsStatus.Confirmed;
                session.Update(logisticsGoods);
            }

            session.Flush();

            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
            {
                logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Confirm, logisticsGoods.Id, loginUserId);
            }

            session.Flush();
        });

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "导入成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/trade/export")]
    public IActionResult Export(TradeGoodsSearchListQueryParam queryParam)
    {
        if (string.IsNullOrWhiteSpace(queryParam.UpdateUserQuery) == false)
        {
            string updateUserQuery = queryParam.UpdateUserQuery.Trim();
            queryParam.UpdateUserId = session.Query<UserInfo>()
                .Where(e => e.LoginName == updateUserQuery || e.UserName == updateUserQuery)
                .Select(e => e.Id)
                .Take(1)
                .SingleOrDefault();
        }

        string fileName = "确认扩展名" + ".xlsx";
        string sheetName = "确认扩展名";

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        IList<LogisticsGoodsInfo> logisticsGoodsList = GetTradeGoodsSearchListCriteria(queryParam)
            .List<LogisticsGoodsInfo>();

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        // 扩展名    物品类型    品牌    中国海关申报名    中国海关编码
        int rowIndex = 0;

        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        int cellIndex = 0;
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("扩展名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("物品类型");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("品牌");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国海关申报名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("中国海关编码");

        foreach (LogisticsGoodsInfo goods in logisticsGoodsList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 扩展名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.Name);

            // 物品类型
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.Type.Name);

            // 品牌
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.BrandName);

            // 中国海关申报名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.HsGoods?.CnHsName ?? string.Empty);

            // 中国海关编码
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.HsGoods?.CnHsCode ?? string.Empty);
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }
}