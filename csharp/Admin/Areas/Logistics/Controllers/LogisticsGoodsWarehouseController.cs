using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.SqlCommand;
using NLog;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Logistics.Models.LogisticsGoodsWarehouse;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Goods.GoodsWarehouse)]
public class LogisticsGoodsWarehouseController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session = null;
    private LoginUserService loginUserService = null;
    private LogisticsGoodsImageService logisticsGoodsImageService = null;
    private LogisticsGoodsLogService logisticsGoodsLogService = null;

    public LogisticsGoodsWarehouseController(ISession session,
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
    [Route("/admin/logistics/goods/warehouse/")]
    public IActionResult Index()
    {
        LogisticsGoodsWarehouseIndexModel model = new LogisticsGoodsWarehouseIndexModel();
        // model.GoodsTypeList = LogisticsGoodsType.GetInnerList();

        return View("LogisticsGoodsWarehouseIndex", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/list")]
    public IActionResult List(LogisticsGoodsWarehouseListQueryParam queryParam)
    {
        int totalCount = GetWarehouseGoodsSearchListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<LogisticsGoodsInfo> logisticsGoodsList = GetWarehouseGoodsSearchListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(EntityNoUpdate.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<LogisticsGoodsInfo>();

        // IList<int> userIdList = logisticsGoodsList.Select(e => e.AddUser.Id).ToList();
        // IList<UserInfo> userList = session.Query<UserInfo>().Where(e => e.LoginName != UserInfo.DevLoginName && userIdList.Contains(e.Id)).ToList();

        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
        {
        }

        LogisticsGoodsWarehouseListModel model = new LogisticsGoodsWarehouseListModel(listPager);
        model.LogisticsGoodsList = logisticsGoodsList;

        return View("LogisticsGoodsWarehouseList", model);
    }

    [NonAction]
    public ICriteria GetWarehouseGoodsSearchListCriteria(LogisticsGoodsWarehouseListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<LogisticsGoodsInfo>();

        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(LogisticsGoodsInfo.HsGoods), nameof(LogisticsGoodsInfo.HsGoods), JoinType.LeftOuterJoin);

        // 这里不需要状态过滤
        // int statusNewId = LogisticsGoodsStatus.New.Id;
        // criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.StatusId == statusNewId));

        if (queryParam.GoodsTypeQuery != null && queryParam.GoodsTypeQuery != LogisticsGoodsType.None)
        {
            LogisticsGoodsType logisticsGoodsType = queryParam.GoodsTypeQuery;
            criteria = criteria.Add(Restrictions.Where<LogisticsGoodsInfo>(e => e.Type == logisticsGoodsType));
        }

        if (string.IsNullOrWhiteSpace(queryParam.NameQuery) == false)
        {
            string nameQuery = queryParam.NameQuery.Trim();
            criteria = criteria.Add(Restrictions.On<LogisticsGoodsInfo>(e => e.Name).IsInsensitiveLike(nameQuery, MatchMode.Anywhere));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/detail")]
    public IActionResult Detail(LogisticsGoodsWarehouseListQueryParam queryParam)
    {
        LogisticsGoodsWarehouseDetailModel model = new LogisticsGoodsWarehouseDetailModel();

        model.TypeList = LogisticsGoodsType.GetInnerList();

        if (queryParam.GoodsId != 0)
        {
            LogisticsGoodsInfo logisticsGoods = session.Get<LogisticsGoodsInfo>(queryParam.GoodsId);
            if (logisticsGoods == null)
            {
                throw new ValidationException("指定的物品不存在！");
            }

            model.Name = logisticsGoods.Name;
            model.Type = logisticsGoods.Type;
            model.BrandName = logisticsGoods.BrandName;
            model.CreateExpressTrackingNo = logisticsGoods.CreateExpressTrackingNo;
        }
        else
        {
            model.Name = queryParam.NameQuery ?? string.Empty;
            model.Type = queryParam.GoodsTypeQuery ?? LogisticsGoodsType.None;

            model.CreateExpressTrackingNo = string.Empty;

            // 如果是新增的话，就自动判断类型
            model.Type = GetGoodsTypeByDetailModel(model);
        }

        /*
         
        model.ImageList = session.Query<LogisticsGoodsImageInfo>()
            .Where(e => e.Goods.Id == queryParam.GoodsId)
            .OrderBy(e => e.Id)
            .ToList();

        foreach (var image in model.ImageList)
        {
            image.ImageFullUrlCache = logisticsGoodsImageService.GetFullImageUrl(image.SubPath, image.FileName);
        }
        
        */

        return View("LogisticsGoodsWarehouseDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/add/save")]
    public IActionResult Save(LogisticsGoodsWarehouseDetailModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId);

        bool isCreate = true;
        LogisticsGoodsInfo logisticsGoods = new LogisticsGoodsInfo();

        if (model.GoodsId != 0)
        {
            logisticsGoods = session.Get<LogisticsGoodsInfo>(model.GoodsId);
            if (logisticsGoods == null)
            {
                throw new ValidationException("指定的物品不存在！");
            }

            if (logisticsGoods.Status == LogisticsGoodsStatus.Confirmed)
            {
                throw new ValidationException("物品当前状态已确认，不能修改！");
            }

            int goodsId = model.GoodsId;
            string goodsName = model.Name;
            int count = session
                .Query<LogisticsGoodsInfo>()
                .Where(e => e.Id != goodsId
                            && e.Name == goodsName
                            && e.BrandName == model.BrandName)
                .Count();

            if (count != 0)
            {
                throw new ValidationException("该扩展名已经存在，不能添加相同的扩展名！");
            }

            isCreate = false;
        }
        else
        {
            logisticsGoods = new LogisticsGoodsInfo();

            string goodsName = model.Name;
            int count = session.Query<LogisticsGoodsInfo>()
                .Where(e => e.Name == goodsName
                            && e.BrandName == model.BrandName)
                .Count();

            if (count != 0)
            {
                throw new ValidationException("该扩展名已经存在，不能添加相同的扩展名！");
            }

            isCreate = true;
        }

        logisticsGoods.Name = model.Name.Trim();
        logisticsGoods.Type = model.Type;

        logisticsGoods.BrandName = model.BrandName ?? string.Empty;
        logisticsGoods.CreateExpressTrackingNo = model.CreateExpressTrackingNo ?? string.Empty;

        logisticsGoods.AddUser = loginUser;
        logisticsGoods.UpdateUser = loginUser;

        if (isCreate)
        {
            logisticsGoods.Status = LogisticsGoodsStatus.PhotoRequired; // 后面再改这个状态

            session.Save(logisticsGoods);
            session.Flush();

            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Create, logisticsGoods.Id, loginUserId);
        }
        else
        {
            logisticsGoods.UpdateUser = loginUser;
            session.Update(logisticsGoods);
            session.Flush();

            logisticsGoodsLogService.AddLog(LogisticsGoodsLogType.Modify, logisticsGoods.Id, loginUserId);
        }

        session.Flush();

        // 保存图片，这个其实已经不用了
        if (model.NewImageList != null && model.NewImageList.Count != 0)
        {
            foreach (var image in model.NewImageList)
            {
                LogisticsGoodsImageInfo interactImage = new LogisticsGoodsImageInfo();
                interactImage.Goods = logisticsGoods;
                interactImage.StorePath = image.SubPath;
                interactImage.StoreName = image.FileName;

                session.Save(interactImage);
            }
        }

        session.Flush();

        // 当前没照片状态的，才需要这样的判断
        if (logisticsGoods.Status == LogisticsGoodsStatus.PhotoRequired)
        {
            bool hasPhoto = logisticsGoodsImageService.HasGoodsImage(logisticsGoods.Id);
            if (hasPhoto)
            {
                logisticsGoods.Status = LogisticsGoodsStatus.NotConfirmed;

                session.Update(logisticsGoods);
                session.Flush();
            }
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/delete")]
    public IActionResult Delete(int[] idList)
    {
        foreach (int id in idList)
        {
            LogisticsGoodsInfo logisticsGoods = session.Get<LogisticsGoodsInfo>(id);
            if (logisticsGoods != null)
            {
                if (logisticsGoods.Status == LogisticsGoodsStatus.Confirmed)
                {
                    throw new ValidationException($"扩展名 {logisticsGoods.Name} 是已确认状态，不能在仓库添加时删除！");
                }
            }
        }

        IList<LogisticsGoodsInfo> logisticsGoodsList = session.Query<LogisticsGoodsInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        IList<LogisticsOrderGoodsInfo> logisticsOrderGoodsList = session.Query<LogisticsOrderGoodsInfo>()
            .Where(e => idList.Contains(e.GoodsId) || idList.Contains(e.GoodsAdminId.Value))
            .ToList();

        IList<LogisticsGoodsImageInfo> logisticsGoodsImageList = session.Query<LogisticsGoodsImageInfo>()
            .Where(e => idList.Contains(e.Id))
            .ToList();

        foreach (int id in idList)
        {
            LogisticsGoodsInfo logisticsGoods = logisticsGoodsList.Where(e => e.Id == id).Take(1).SingleOrDefault();
            if (logisticsGoods == null)
            {
                throw new ValidationException($"指定的物品不存在！#{id}");
            }

            var orderGoods = logisticsOrderGoodsList
                .Where(e => e.GoodsId == id || e.GoodsAdminId == id)
                .Take(1)
                .SingleOrDefault();

            if (orderGoods != null)
            {
                throw new ValidationException($"物品 {logisticsGoods.Name}, 在订单物品中有被引用，不能删除！");
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
    [Route("/admin/logistics/goods/warehouse/get/goods/type")]
    public IActionResult GetGoodsType(LogisticsGoodsWarehouseDetailModel model)
    {
        LogisticsGoodsType type = GetGoodsTypeByDetailModel(model);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "获取物品类型成功！" },
            { "goodsType", type.Id },
        });
    }

    #region Image

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/image/upload")]
    public IActionResult ImageUpload(IFormFile imageFile)
    {
        if (imageFile == null)
        {
            throw new ValidationException("文件未能上传成功！");
        }

        string fileExt = logisticsGoodsImageService.GetFileExt(imageFile.FileName);
        if (fileExt.ToLower() != ".png".ToLower() && fileExt.ToLower() != ".jpg".ToLower() && fileExt.ToLower() != ".jpeg".ToLower())
        {
            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", "只支持 png 和 jpg,jpeg 格式的图片文件！" },
            });
        }

        var result = logisticsGoodsImageService.SaveImageFile(imageFile);

        string imageFileUrl = result.Item1;
        string subPath = result.Item2;
        string fileName = result.Item3;

        LogisticsGoodsWarehouseImageModel model = new LogisticsGoodsWarehouseImageModel();
        model.ImageFileUrl = imageFileUrl;
        model.SubPath = subPath;
        model.FileName = fileName;

        return View("LogisticsGoodsWarehouseImage", model);
    }

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/image/delete")]
    public IActionResult ImageDelete(int imageId)
    {
        var logisticsGoodsImage = session.Get<LogisticsGoodsImageInfo>(imageId);
        if (logisticsGoodsImage == null)
        {
            throw new ValidationException("指定的客服仓库交互图片不存在！");
        }

        var goodsId = logisticsGoodsImage.Goods?.Id ?? 0;
        var logisticsGoods = session.Get<LogisticsGoodsInfo>(goodsId);
        if (logisticsGoods == null)
        {
            throw new ValidationException("图片对应的客服仓库交互不存在！");
        }

        session.Delete(logisticsGoodsImage);

        logisticsGoodsImageService.DeleteFile(logisticsGoodsImage);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "图片删除成功！" },
        });
    }

    #endregion

    [HttpPost]
    [Route("/admin/logistics/goods/warehouse/import")]
    public IActionResult Import(IFormFile excelFile)
    {
        if (excelFile == null)
        {
            throw new ValidationException("导入失败，文件未上传！");
        }
        
        // todo 这里的逻辑有点问题，应该是拿Excel里面的 Name 列表到数据里面取查找。主要是物品总量有点多

        IList<string> seaNameList = session
            .Query<LogisticsGoodsInfo>()
            .Where(e => e.Cate == LogisticsGoodsCate.Sea)
            .Select(e => e.Name)
            .Distinct()
            .ToList();

        IDictionary<string, bool> seaNameDict = new Dictionary<string, bool>();
        foreach (string seaName in seaNameList)
        {
            if (seaNameDict.ContainsKey(seaName) == false)
            {
                seaNameDict.Add(seaName, true);
            }
        }

        IList<string> airNameList = session
            .Query<LogisticsGoodsInfo>()
            .Where(e => e.Cate == LogisticsGoodsCate.Air)
            .Select(e => e.Name)
            .Distinct()
            .ToList();

        IDictionary<string, bool> airNameDict = new Dictionary<string, bool>();
        foreach (string airName in airNameList)
        {
            if (airNameDict.ContainsKey(airName) == false)
            {
                airNameDict.Add(airName, true);
            }
        }

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

            LogisticsGoodsInfo logisticsGoods = new LogisticsGoodsInfo();

            cellIndex = 0;

            string cateName = row.GetCell(cellIndex++)?.StringCellValue;
            if (string.IsNullOrWhiteSpace(cateName))
            {
                break;
            }

            cateName = cateName.Trim();

            if (cateName == LogisticsGoodsCate.Air.Name)
            {
                logisticsGoods.Cate = LogisticsGoodsCate.Air;
            }
            else if (cateName == LogisticsGoodsCate.Sea.Name)
            {
                logisticsGoods.Cate = LogisticsGoodsCate.Sea;
            }
            else
            {
                throw new ValidationException($"第{rowIndex}行数据 类别不对，应该是 {LogisticsGoodsCate.Sea.Name} 或者 {LogisticsGoodsCate.Air.Name} !");
            }

            // 扩展名
            string name = row.GetCell(cellIndex++)?.StringCellValue;
            if (string.IsNullOrWhiteSpace(name))
            {
                break;
            }

            name = name.Trim();

            if (logisticsGoods.Cate == LogisticsGoodsCate.Sea)
            {
                if (seaNameDict.ContainsKey(name))
                {
                    throw new ValidationException($"第{rowIndex}行数据 {LogisticsGoodsCate.Sea.Name} 扩展名 {name} 已存在或重复，导入失败！");
                }

                seaNameDict.Add(name, true);
            }

            if (logisticsGoods.Cate == LogisticsGoodsCate.Air)
            {
                if (airNameDict.ContainsKey(name))
                {
                    throw new ValidationException($"第{rowIndex}行数据  {LogisticsGoodsCate.Air.Name}  扩展名 {name} 已存在或重复，导入失败！");
                }

                airNameDict.Add(name, true);
            }

            if (name.Length > 200)
            {
                throw new ValidationException($"第{rowIndex}行数据  {cateName}  扩展名 {name} 太长，超过200长度，导入失败！");
            }

            logisticsGoods.Name = name.Trim();

            /*

            // 物品类型
            string goodsTypeStr = row.GetCell(cellIndex++)?.StringCellValue;
            if (string.IsNullOrWhiteSpace(goodsTypeStr))
            {
                throw new ValidationException($"第{rowIndex}行数据 物品类型 不能为空，导入失败！");
            }

            LogisticsGoodsType goodsType = LogisticsGoodsType.GetByName(goodsTypeStr.Trim());
            if (goodsType == LogisticsGoodsType.None)
            {
                throw new ValidationException($"第{rowIndex}行数据 物品类型 数据有误，导入失败！");
            }

            logisticsGoods.Type = goodsType;

            */

            // 是否品牌
            bool hasBrand = GetBoolValue(
                row.GetCell(cellIndex++)?.StringCellValue,
                $"第{rowIndex}行数据 是否品牌 不能为空，导入失败！",
                $"第{rowIndex}行数据 是否品牌 数据有误，导入失败！");

            logisticsGoods.HasBrand = hasBrand;

            // 品牌
            string brandName = row.GetCell(cellIndex++)?.StringCellValue;
            if (hasBrand && string.IsNullOrWhiteSpace(brandName))
            {
                throw new ValidationException($"第{rowIndex}行数据 有品牌商品品牌名 不能为空，导入失败！");
            }

            logisticsGoods.BrandName = brandName;

            // 是否含电池或其他限制
            logisticsGoods.HasBattery = GetBoolValue(
                row.GetCell(cellIndex++)?.StringCellValue,
                $"第{rowIndex}行数据 是否含电池或其他限制 不能为空，导入失败！",
                $"第{rowIndex}行数据 是否含电池或其他限制 数据有误，导入失败！");

            // 是否含草木竹
            logisticsGoods.HasBamboo = GetBoolValue(
                row.GetCell(cellIndex++)?.StringCellValue,
                $"第{rowIndex}行数据 是否含草木竹 不能为空，导入失败！",
                $"第{rowIndex}行数据 是否含草木竹 数据有误，导入失败！");

            // 是否含实木包装
            logisticsGoods.HasWoodPackage = GetBoolValue(
                row.GetCell(cellIndex++)?.StringCellValue,
                $"第{rowIndex}行数据 是否含实木包装 不能为空，导入失败！",
                $"第{rowIndex}行数据 是否含实木包装 数据有误，导入失败！");

            // 是否特货类
            logisticsGoods.IsSpecial = GetBoolValue(
                row.GetCell(cellIndex++)?.StringCellValue,
                $"第{rowIndex}行数据 是否特货类 不能为空，导入失败！",
                $"第{rowIndex}行数据 是否特货类 数据有误，导入失败！");

            // 是否禁止类
            logisticsGoods.IsForbidden = GetBoolValue(
                row.GetCell(cellIndex++)?.StringCellValue,
                $"第{rowIndex}行数据 是否禁止类 不能为空，导入失败！",
                $"第{rowIndex}行数据 是否禁止类 数据有误，导入失败！");

            logisticsGoods.HsGoods = null;

            logisticsGoodsList.Add(logisticsGoods);
        }

        int loginUserId = loginUserService.GetLoginUserId();

        ITransaction? transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
            {
                logisticsGoods.Type = logisticsGoodsType;

                logisticsGoods.AddUserId = loginUserId;
                logisticsGoods.Status = LogisticsGoodsStatus.NotConfirmed;

                session.Save(logisticsGoods);
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
    [Route("/admin/logistics/goods/warehouse/export")]
    public IActionResult Export(LogisticsGoodsWarehouseListQueryParam queryParam)
    {
        string fileName = "扩展名添加" + ".xlsx";
        string sheetName = "扩展名添加";

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        IList<LogisticsGoodsInfo> logisticsGoodsList = GetWarehouseGoodsSearchListCriteria(queryParam)
            .List<LogisticsGoodsInfo>();

        int rowHeight = 22;
        ICellStyle headerCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        // 扩展名    物品类型    是否品牌    品牌    是否含电池或其他限制    是否含草木竹    是否含实木包装    是否禁止类    中国海关申报名    中国海关编码
        int rowIndex = 0;

        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        int cellIndex = 0;

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("类别");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
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
        cell.SetCellValue("是否品牌");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("品牌");

        sheet.SetColumnWidth(cellIndex, 18 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("是否含电池或其他限制");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("是否含草木竹");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("是否含实木包装");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("是否特货类");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("是否禁止类");

        /*
        sheet.SetColumnWidth(cellIndex, 10 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("状态");
        */

        foreach (LogisticsGoodsInfo goods in logisticsGoodsList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 类别
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.Cate?.Name);

            // 扩展名
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.Name);

            /*

            // 物品类型
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.Type.Name);

            */

            // 是否品牌
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(GetBoolStr(goods.HasBrand));

            // 品牌
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(goods.BrandName);

            // 是否含电池或其他限制
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(GetBoolStr(goods.HasBattery));

            // 是否含草木竹
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(GetBoolStr(goods.HasBamboo));

            // 是否含实木包装
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(GetBoolStr(goods.HasWoodPackage));

            // 是否特货类
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(GetBoolStr(goods.IsSpecial));

            // 是否禁止类
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(GetBoolStr(goods.IsForbidden));

            /*
            // 状态
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(LogisticsGoodsStatus.GetById(goods.StatusId)?.Name);
            */
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }
}