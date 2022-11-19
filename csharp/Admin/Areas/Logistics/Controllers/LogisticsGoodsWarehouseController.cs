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
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Logistics.Controllers;

[LogisticsArea]
[AdminAuth(PassRes.Goods.GoodsWarehouse)]
public class LogisticsGoodsWarehouseController : Controller
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    private LoginUserService loginUserService;
    private LogisticsGoodsImageService logisticsGoodsImageService;
    private LogisticsGoodsLogService logisticsGoodsLogService;

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
            LogisticsGoodsInfo? logisticsGoods = session.Get<LogisticsGoodsInfo>(queryParam.GoodsId);
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
    public IActionResult Save(LogisticsGoodsWarehouseSaveModel model)
    {
        if (!this.ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId)!;

        bool isCreate = true;
        LogisticsGoodsInfo? logisticsGoods = null;

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
                interactImage.StorePath = image.StorePath;
                interactImage.StoreName = image.StoreName;

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
        IList<int> goodsIdList = idList.ToList();
        
        foreach (int id in goodsIdList)
        {
            LogisticsGoodsInfo? logisticsGoods = session.Get<LogisticsGoodsInfo>(id);
            if (logisticsGoods != null)
            {
                if (logisticsGoods.Status == LogisticsGoodsStatus.Confirmed)
                {
                    throw new ValidationException($"扩展名 {logisticsGoods.Name} 是已确认状态，不能在仓库添加时删除！");
                }
            }
        }

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
            LogisticsGoodsInfo? logisticsGoods = logisticsGoodsList
                .Where(e => e.Id == goodsId)
                .Take(1)
                .SingleOrDefault();
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
                throw new ValidationException($"物品 {logisticsGoods.Name}, 在订单物品中有被引用，不能删除！");
            }
        }

        // 数据库事务
        Transactional.Wrap(session, () => {
            foreach (var goodsImage in logisticsGoodsImageList)
            {
                session.Delete(goodsImage);
            }

            foreach (var logisticsGoods in logisticsGoodsList)
            {
                session.Delete(logisticsGoods);
            }

            session.Flush();
        });

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
        string storePath = result.Item2;
        string storeName = result.Item3;

        LogisticsGoodsWarehouseImageModel model = new LogisticsGoodsWarehouseImageModel();
        model.ImageFileUrl = imageFileUrl;
        model.StorePath = storePath;
        model.StoreName = storeName;

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

        var goodsId = logisticsGoodsImage.Goods.Id;
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

        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId)!;        

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

            logisticsGoods.Type = goodsType;

            // 扩展名
            string name = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(name))
            {
                break;
            }

            name = name.Trim();

            if (name.Length > 200)
            {
                throw new ValidationException($"第{rowIndex}行数据 扩展名 {name} 太长，超过200长度，导入失败！");
            }

            logisticsGoods.Name = name.Trim();

            // 品牌
            string brandName = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            logisticsGoods.BrandName = brandName;

            logisticsGoodsList.Add(logisticsGoods);
        }

        // 扩展名重复检查

        // 自身导入的判断重复
        IDictionary<string, int> logisticsGoodsNameRowNumMap = new Dictionary<string, int>();
        int logisticsGoodsRowNum = 1;
        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) // 这里logisticsGoodsList还是保持了导入顺序
        {
            string name = logisticsGoods.Name;
            if (logisticsGoodsNameRowNumMap.TryGetValue(name, out int rowNumExist) == true)
            {
                throw new ValidationException($"导入数据中，第{logisticsGoodsRowNum}行 扩展名 {name} 跟第{rowNumExist}行 数据重复！");
            }

            logisticsGoodsNameRowNumMap[name] = logisticsGoodsRowNum;
            logisticsGoodsRowNum = logisticsGoodsRowNum + 1;
        }

        // 跟数据中现有的判断重复
        IList<string> goodsNameList = logisticsGoodsList.Select(e => e.Name).ToList();
        IList<LogisticsGoodsInfo> logisticsGoodsListExist = session.Query<LogisticsGoodsInfo>()
            .Where(e => goodsNameList.Contains(e.Name))
            .ToList();
        IDictionary<string, int> logisticsGoodsMapExist = logisticsGoodsListExist.ToDictionary(e => e.Name, e => e.Id);

        logisticsGoodsRowNum = 1;
        foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList) // 这里logisticsGoodsList还是保持了导入顺序
        {
            string name = logisticsGoods.Name;
            if (logisticsGoodsMapExist.ContainsKey(name))
            {
                throw new ValidationException($"导入数据中，第{logisticsGoodsRowNum}行 扩展名 {name} 数据库中已存在！");
            }

            logisticsGoodsRowNum = logisticsGoodsRowNum + 1;
        }

        ITransaction? transaction = null;
        try
        {
            transaction = session.BeginTransaction();

            foreach (LogisticsGoodsInfo logisticsGoods in logisticsGoodsList)
            {
                // 海关编码暂时置空
                logisticsGoods.HsGoods = null;
                logisticsGoods.AddUser = loginUser;
                logisticsGoods.UpdateUser = loginUser;
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

        // 扩展名    物品类型    品牌
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