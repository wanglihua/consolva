using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Storage.Models.StorageLoc;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service;
using shunda.Service.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Storage.Controllers;

[StorageArea]
[AdminAuth(PassRes.Storage.Warehouse)]
public class StorageLocController : Controller
{
    private ISession session;
    private StorageLocDesignService storageLocDesignService;
    private StorageLocLabelService storageLocLabelService;
    private LoginUserService loginUserService;

    public StorageLocController(ISession session,
        StorageLocDesignService storageLocDesignService,
        StorageLocLabelService storageLocLabelService,
        LoginUserService loginUserService)
    {
        this.session = session;
        this.storageLocDesignService = storageLocDesignService;
        this.storageLocLabelService = storageLocLabelService;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/storage/loc/{shelfId:int}")]
    public IActionResult Index(int shelfId)
    {
        ShelfInfo shelfInfo = session.Get<ShelfInfo>(shelfId);
        if (shelfInfo == null)
        {
            return Content("指定的货架不存在！");
        }

        WarehouseInfo warehouseInfo = session.Get<WarehouseInfo>(shelfInfo.Warehouse.Id);

        StorageLocIndexModel model = new StorageLocIndexModel();
        model.ShelfId = shelfId;
        model.Shelf = shelfInfo;
        model.Warehouse = warehouseInfo;

        /*
        IList<StorageLocDuplicateInfo> storageDuplicateList = session.Query<StorageLocDuplicateInfo>()
            .OrderBy(e => e.WarehouseName).ThenBy(e => e.ShelfName).ThenBy(e => e.StorageLocRowNum).ThenBy(e => e.StorageLocColNum)
            .ToList();
        model.StorageDuplicateList = storageDuplicateList;
        */

        return View("StorageLocIndex", model);
    }

    [HttpPost]
    [Route("/admin/storage/loc/list")]
    public IActionResult List(StorageLocListQueryParam queryParam)
    {
        int totalCount = GetListCriteria(queryParam)
            .SetProjection(Projections.RowCount())
            .UniqueResult<int>();

        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<StorageLocInfo> storageLocList = GetListCriteria(queryParam)
            .AddOrder(new Order(listPager.SortField, listPager.IsAscending))
            .AddOrder(Order.Desc(nameof(Entity.Id)))
            .SetFirstResult(listPager.PageIndex * listPager.PageSize)
            .SetMaxResults(listPager.PageSize)
            .List<StorageLocInfo>();

        foreach (StorageLocInfo storageLocInfo in storageLocList)
        {
            // storageLocInfo.StatusName = storageLocInfo.StatusName;
        }

        StorageLocListModel model = new StorageLocListModel(listPager);
        model.StorageLocSpecAlias = storageLocSpecAlias;
        model.StorageLocList = storageLocList;

        return View("StorageLocList", model);
    }

    private static readonly string storageLocSpecAlias = "sls";
    private static readonly string storageLocShelfAlias = "s";

    [NonAction]
    private ICriteria GetListCriteria(StorageLocListQueryParam queryParam)
    {
        ICriteria criteria = session.CreateCriteria<StorageLocInfo>();
        criteria = criteria.SetReadOnly(true);

        criteria = criteria.CreateAlias(nameof(StorageLocInfo.Spec), storageLocSpecAlias);
        criteria = criteria.CreateAlias(nameof(StorageLocInfo.Shelf), storageLocShelfAlias);

        int shelfId = queryParam.ShelfId;
        criteria = criteria.Add(Restrictions.Where<StorageLocInfo>(e => e.Shelf.Id == shelfId));

        if (queryParam.RowNumQuery != null)
        {
            int rowNumQueryValue = queryParam.RowNumQuery.Value;
            criteria = criteria.Add(Restrictions.Where<StorageLocInfo>(e => e.RowNum == rowNumQueryValue));
        }

        if (queryParam.ColNumQuery != null)
        {
            int colNumQueryValue = queryParam.ColNumQuery.Value;
            criteria = criteria.Add(Restrictions.Where<StorageLocInfo>(e => e.ColNum == colNumQueryValue));
        }

        return criteria;
    }

    [HttpPost]
    [Route("/admin/storage/loc/detail")]
    public IActionResult Detail(StorageLocDetailModel model)
    {
        int storageLocId = model.Id;
        int shelfId = model.ShelfId;

        StorageLocInfo storageLoc;

        if (storageLocId == 0)
        {
            storageLoc = new StorageLocInfo();
            storageLoc.Shelf = session.Load<ShelfInfo>(shelfId);
        }
        else
        {
            storageLoc = session.Get<StorageLocInfo>(storageLocId);
            if (storageLoc == null)
            {
                throw new ValidationException("指定的数据项不存在！");
            }

            if (storageLoc.Spec != null)
            {
                model.SpecId = storageLoc.Spec.Id;
            }
        }

        ShelfInfo shelfInfo = session.Get<ShelfInfo>(shelfId);
        if (shelfInfo == null)
        {
            throw new ValidationException("指定的货架数据项不存在！");
        }

        var shelfType = shelfInfo.Type;
        IList<StorageLocSpecInfo> storageLocSpecList = session
            .Query<StorageLocSpecInfo>()
            .Where(e => e.ShelfType == shelfType)
            .OrderBy(e => e.Id)
            .ToList();

        model.StorageLoc = storageLoc;
        model.ShelfId = storageLoc.Shelf.Id;

        model.StorageLocStatusList = StorageLocStatus.GetList();
        model.StorageLocSpecList = storageLocSpecList;

        return View("StorageLocDetail", model);
    }

    [HttpPost]
    [Route("/admin/storage/loc/save")]
    [Transactional]
    public IActionResult Save(StorageLocDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int loginUserId = loginUserService.GetLoginUserId();
        UserInfo loginUser = session.Get<UserInfo>(loginUserId);

        StorageLocSpecInfo storageLocSpec = session.Load<StorageLocSpecInfo>(model.SpecId);
        if (storageLocSpec == null)
        {
            throw new ValidationException("指定的仓位规格不存在！");
        }

        StorageLocInfo storageLocFromUi = model.StorageLoc;
        int storageLocId = storageLocFromUi.Id;

        bool isCreate = storageLocId == 0;

        StorageLocInfo storageLoc;

        if (isCreate)
        {
            storageLoc = new StorageLocInfo();

            if (storageLocSpec.ShelfType == ShelfType.LongNoPickDump || storageLocSpec.ShelfType == ShelfType.LongNoProcessDump)
            {
                string newStorageLocName = DateTime.Now.ToString("yyyyMMdd") + "_" + loginUser.LoginName;

                StorageLocInfo? storageLocNew = session.Query<StorageLocInfo>()
                    .Where(e => e.Shelf.Id == model.ShelfId && e.Name == newStorageLocName)
                    .Take(1)
                    .SingleOrDefault();

                if (storageLocNew != null)
                {
                    throw new ValidationException("相同名称的仓位号已存在！");
                }

                storageLoc.Name = newStorageLocName;
            }
        }
        else
        {
            storageLoc = session.Get<StorageLocInfo>(storageLocId);
        }

        storageLoc.RowNum = storageLocFromUi.RowNum;
        storageLoc.ColNum = storageLocFromUi.ColNum;

        if ((storageLocSpec.ShelfType == ShelfType.LongNoPickDump || storageLocSpec.ShelfType == ShelfType.LongNoProcessDump) == false)
        {
            storageLoc.Name = $"{storageLocFromUi.RowNum}层{storageLocFromUi.ColNum}列";
        }

        storageLoc.Shelf = session.Load<ShelfInfo>(model.ShelfId);
        storageLoc.Status = storageLocFromUi.Status;
        storageLoc.OrderNum = storageLocFromUi.OrderNum;
        storageLoc.OffShelfOrderNum = storageLocFromUi.OffShelfOrderNum;

        storageLoc.Spec = session.Load<StorageLocSpecInfo>(model.SpecId);

        storageLoc.Status = storageLocFromUi.Status;

        if (isCreate)
        {
            session.Save(storageLoc);
        }
        else
        {
            session.Update(storageLoc);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/enable")]
    [Transactional]
    public IActionResult Enable(int[] idList)
    {
        foreach (int id in idList)
        {
            StorageLocInfo storageLoc = session.Get<StorageLocInfo>(id);
            if (storageLoc == null)
            {
                throw new ValidationException("指定的仓位不存在！");
            }

            if (storageLoc.Status == StorageLocStatus.Enabled)
            {
                throw new ValidationException($"仓位{storageLoc.Name}已经是启用状态！");
            }

            storageLoc.Status = StorageLocStatus.Enabled;

            session.Save(storageLoc);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "启用成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/disable")]
    [Transactional]
    public IActionResult Disable(int[] idList)
    {
        foreach (int id in idList)
        {
            StorageLocInfo storageLoc = session.Get<StorageLocInfo>(id);
            if (storageLoc == null)
            {
                throw new ValidationException("指定的仓位不存在！");
            }

            if (storageLoc.Status == StorageLocStatus.Disabled)
            {
                throw new ValidationException($"仓位{storageLoc.Name}已经是停用状态！");
            }

            storageLoc.Status = StorageLocStatus.Disabled;

            session.Save(storageLoc);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "停用成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/batch/enable")]
    [Transactional]
    public IActionResult BatchEnable(int shelfId)
    {
        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>()
            .Where(e => e.Shelf.Id == shelfId)
            .ToList();

        foreach (StorageLocInfo storageLoc in storageLocList)
        {
            /*
            if (storageLoc.Status == StorageLocStatus.Enabled)
            {
                throw new ValidationException($"仓位{storageLoc.Name}已经是启用状态！");
            }
            */

            storageLoc.Status = StorageLocStatus.Enabled;

            session.Save(storageLoc);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "批量启用成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/batch/disable")]
    [Transactional]
    public IActionResult BatchDisable(int shelfId)
    {
        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>()
            .Where(e => e.Shelf.Id == shelfId)
            .ToList();

        foreach (StorageLocInfo storageLoc in storageLocList)
        {
            /*
            if (storageLoc.Status == StorageLocStatus.Disabled)
            {
                throw new ValidationException($"仓位{storageLoc.Name}已经是停用状态！");
            }
            */

            storageLoc.Status = StorageLocStatus.Disabled;

            session.Save(storageLoc);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "批量停用成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/delete")]
    [Transactional]
    public IActionResult Delete(int[] idList)
    {
        /*
        LogisticsOrderGoodsInfo orderGoods = session.Query<LogisticsOrderGoodsInfo>()
            .Where(e => idList.Contains(e.StorageLoc.Id))
            .Take(1)
            .SingleOrDefault();

        if (orderGoods != null)
        {
            return Json(new Dictionary<string, object>() {
                { "success", false },
                { "message", "有物品正引用选择的仓位, 删除失败!" },
            });
        }
        */

        foreach (int id in idList)
        {
            StorageLocInfo storageLoc = session.Load<StorageLocInfo>(id);
            session.Delete(storageLoc);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "删除成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/design")]
    public IActionResult Design(StorageLocDesignModel model)
    {
        int shelfId = model.ShelfId;

        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null)
        {
            throw new ValidationException("指定的货架不存在！");
        }

        WarehouseInfo warehouse = session.Get<WarehouseInfo>(shelf.Warehouse?.Id ?? 0);

        model.ShelfId = shelfId;
        model.Shelf = shelf;
        model.Warehouse = warehouse;

        model.DesignContent = storageLocDesignService.ReadStorageLocDesign(shelfId);

        return View("StorageLocDesign", model);
    }

    [HttpPost]
    [Route("/admin/storage/loc/design/save")]
    [Transactional]
    public IActionResult DesignSave(StorageLocDesignModel model)
    {
        int shelfId = model.ShelfId;

        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null)
        {
            throw new ValidationException("指定的货架不存在！");
        }

        string designContent = model.DesignContent;
        storageLocDesignService.SaveStorageLocDesign(shelfId, designContent);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存仓位设计成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/preview")]
    public IActionResult Preview(StorageLocPreviewModel model)
    {
        int shelfId = model.ShelfId;

        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null)
        {
            throw new ValidationException("指定的货架不存在！");
        }

        model.Shelf = shelf;

        WarehouseInfo warehouse = session.Get<WarehouseInfo>(shelf.Warehouse?.Id ?? 0);
        model.Warehouse = warehouse;

        ShelfType shelfType = shelf.Type;
        IList<StorageLocSpecInfo> storageLocSpecList = session
            .Query<StorageLocSpecInfo>()
            .Where(e => e.ShelfType == shelfType)
            .OrderBy(e => e.Id)
            .ToList();

        model.StorageLocSpecList = storageLocSpecList;

        IList<StorageLocInfo> storageLocList = session
            .Query<StorageLocInfo>()
            .Where(e => e.Shelf.Id == shelfId)
            .OrderBy(e => e.Id)
            .ToList();

        /*
        var occupyStorageLocOrderStatusList = LogisticsOrderStatus.GetOccupyStorageLocStatusList();
        var occupyStorageLocOrderGoodsStatusList = LogisticsOrderGoodsStatus.GetOccupyStorageLocStatusList();
        IList<LogisticsOrderGoodsInfo> orderGoodsListHasStorageLoc = session
            .Query<LogisticsOrderGoodsInfo>()
            .Fetch(e => e.Express).ThenFetch(e => e.Order)
            .Where(e => e.StorageLoc.Shelf.Id == shelfId
                        && occupyStorageLocOrderStatusList.Contains(e.Express.Order.OrderStatus)
                        && occupyStorageLocOrderGoodsStatusList.Contains(e.Status)
            )
            .ToList();

        foreach (StorageLocInfo loc in storageLocList)
        {
            loc.HasGoodsCache = orderGoodsListHasStorageLoc.Where(e => e.StorageLoc!.Id == loc.Id).Count() != 0;
        }
        */

        model.StorageLocList = storageLocList;

        return View("StorageLocPreview", model);
    }

    [HttpPost]
    [Route("/admin/storage/loc/get/label/print/data")]
    [NoLoginRequired]
    public IActionResult GetLabelPrintData(IList<int> idList)
    {
        if (idList == null || idList.Count == 0)
        {
            throw new ValidationException("请选择要打印标签的仓位！");
        }

        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>()
            .Fetch(e => e.Shelf)
            .Fetch(e => e.Spec)
            .Where(e => idList.Contains(e.Id))
            .OrderBy(e => e.OrderNum)
            .ThenBy(e => e.Id)
            .ToList();

        IList<IDictionary<string, string>> storageLocLabelList = new List<IDictionary<string, string>>();

        foreach (StorageLocInfo storageLoc in storageLocList)
        {
            var storageLocLabel = new Dictionary<string, string>() {
                { "fullName", storageLoc.GetFullName() },
                { "shelfName", storageLoc.GetShelfName() },
                { "storageLocName", storageLoc.GetRowColName() },
                { "labelCode", storageLocLabelService.GetLabelCode(storageLoc) },
            };

            storageLocLabelList.Add(storageLocLabel);
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "操作成功！" },
            { "labelList", storageLocLabelList },
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/get/label/print/data/by/shelf")]
    [NoLoginRequired]
    public IActionResult GetLabelPrintDataByShelf(int shelfId)
    {
        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null)
        {
            throw new ValidationException("指定的货架不存在！");
        }

        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>()
            .Fetch(e => e.Shelf)
            .Fetch(e => e.Spec)
            .Where(e => e.Shelf.Id == shelfId)
            .OrderBy(e => e.OrderNum)
            .ThenBy(e => e.Id)
            .ToList();

        IList<IDictionary<string, string>> storageLocLabelList = new List<IDictionary<string, string>>();

        foreach (StorageLocInfo storageLoc in storageLocList)
        {
            var storageLocLabel = new Dictionary<string, string>() {
                { "fullName", storageLoc.GetFullName() },
                { "shelfName", storageLoc.GetShelfName() },
                { "storageLocName", storageLoc.GetRowColName() },
                { "labelCode", storageLocLabelService.GetLabelCode(storageLoc) },
            };

            storageLocLabelList.Add(storageLocLabel);
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "操作成功！" },
            { "labelList", storageLocLabelList },
        });
    }

    /// <summary>
    /// 这是按一个货架生成
    /// </summary>
    /// <param name="shelfId"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    [HttpPost]
    [Route("/admin/storage/loc/auto/generate/off/shelf/order/num")]
    [Transactional]
    public IActionResult AutoGenerateOffShelfOrderNum(int shelfId)
    {
        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null)
        {
            throw new ValidationException("指定的货架不存在！");
        }

        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>()
            .Where(e => e.Shelf.Id == shelfId && e.Status == StorageLocStatus.Enabled)
            .ToList();

        int offShelfOrderNum = 1;

        // column list
        IList<int> columnList = storageLocList.OrderBy(e => e.ColNum).Select(e => e.ColNum).Distinct().ToList();
        for (int columnIndex = 0; columnIndex < columnList.Count; columnIndex++)
        {
            int column = columnList[columnIndex];
            IList<int> rowList = storageLocList.Where(e => e.ColNum == column).OrderBy(e => e.RowNum).Select(e => e.RowNum).Distinct().ToList();

            if (columnIndex % 2 == 0)
            {
                rowList = rowList.Reverse().ToList();
            }

            foreach (int row in rowList)
            {
                StorageLocInfo? storageLoc = storageLocList
                    .Where(e => e.ColNum == column && e.RowNum == row)
                    .Take(1)
                    .SingleOrDefault();

                if (storageLoc != null)
                {
                    storageLoc.OffShelfOrderNum = offShelfOrderNum++;
                    session.Save(storageLoc);
                }
            }
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "自动生成下架顺序成功！" },
        });
    }

    /// <summary>
    /// 这是按整个仓库生成
    /// </summary>
    /// <param name="warehouseId"></param>
    /// <returns></returns>
    [HttpGet]
    [Route("/admin/storage/loc/auto/generate/off/shelf/order/num/warehouse")]
    [Transactional]
    public IActionResult AutoGenerateOffShelfOrderNumWarehouse(int warehouseId)
    {
        IList<ShelfInfo> shelfList = session.Query<ShelfInfo>().Where(e => e.Warehouse.Id == warehouseId && e.Status == ShelfStatus.Enabled).ToList();

        foreach (ShelfInfo shelf in shelfList)
        {
            int shelfId = shelf?.Id ?? 0;

            IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>()
                .Where(e => e.Shelf.Id == shelfId && e.Status == StorageLocStatus.Enabled)
                .ToList();

            int offShelfOrderNum = 1;

            // column list
            IList<int> columnList = storageLocList.OrderBy(e => e.ColNum).Select(e => e.ColNum).Distinct().ToList();
            for (int columnIndex = 0; columnIndex < columnList.Count; columnIndex++)
            {
                int column = columnList[columnIndex];
                IList<int> rowList = storageLocList.Where(e => e.ColNum == column).OrderBy(e => e.RowNum).Select(e => e.RowNum).Distinct().ToList();

                if (columnIndex % 2 == 0)
                {
                    rowList = rowList.Reverse().ToList();
                }

                foreach (int row in rowList)
                {
                    StorageLocInfo? storageLoc = storageLocList
                        .Where(e => e.ColNum == column && e.RowNum == row)
                        .Take(1)
                        .SingleOrDefault();

                    if (storageLoc != null)
                    {
                        storageLoc.OffShelfOrderNum = offShelfOrderNum++;
                        session.Save(storageLoc);
                    }
                }
            }

            session.Flush();
        }

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "自动生成下架顺序成功！" },
        });
    }

    [HttpPost]
    [Route("/admin/storage/loc/export/off/shelf/order")]
    public IActionResult ExportOffShelfOrder(int shelfId)
    {
        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null)
        {
            throw new ValidationException($"指定的货架{shelfId}，不存在！");
        }

        string fileName = "货架_" + shelf.Name + "_仓位下架顺序" + ".xlsx";
        string sheetName = "仓位下架顺序";

        IList<StorageLocInfo> storageLocList = session.Query<StorageLocInfo>()
            .Where(e => e.Shelf.Id == shelfId && e.Status == StorageLocStatus.Enabled)
            .OrderBy(e => e.RowNum).ThenBy(e => e.ColNum)
            .ToList();

        IWorkbook workbook = new XSSFWorkbook();
        ISheet sheet = workbook.CreateSheet(sheetName);

        int rowHeight = 22;
        ICellStyle headerCenterCellStyle = ExcelUtil.CreateHeaderCellStyle(workbook, HorizontalAlignment.Center);
        ICellStyle contentCellStyle = ExcelUtil.CreateContentCellStyle(workbook);
        ICellStyle contentCenterCellStyle = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Center);

        IDataFormat dataFormat = workbook.CreateDataFormat();

        ICellStyle precision0Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision0Style.DataFormat = dataFormat.GetFormat("0");

        ICellStyle precision1Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision1Style.DataFormat = dataFormat.GetFormat("0.0");

        ICellStyle precision2Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision2Style.DataFormat = dataFormat.GetFormat("0.00");

        ICellStyle precision3Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision3Style.DataFormat = dataFormat.GetFormat("0.000");

        ICellStyle precision6Style = ExcelUtil.CreateContentCellStyle(workbook, HorizontalAlignment.Right);
        precision6Style.DataFormat = dataFormat.GetFormat("0.000000");

        int rowIndex = 0;

        IRow row = sheet.CreateRow(rowIndex++);
        row.HeightInPoints = rowHeight;

        // 层 列 下架顺序

        int cellIndex = 0;
        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("层");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("列");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("下架顺序");

        foreach (var sl in storageLocList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            // 层
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision0Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(sl.RowNum);

            // 列
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision0Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(sl.ColNum);

            // 下架顺序
            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision0Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(sl.OffShelfOrderNum);
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }

    [HttpPost]
    [Route("/admin/storage/loc/import/off/shelf/order")]
    [Transactional]
    public IActionResult ImportOffShelfOrder(int shelfId, IFormFile excelFile)
    {
        ShelfInfo shelf = session.Get<ShelfInfo>(shelfId);
        if (shelf == null)
        {
            throw new ValidationException($"指定的货架{shelfId}，不存在！");
        }

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

        IList<StorageLocInfo> storageLocListInDb = session.Query<StorageLocInfo>()
            .Where(e => e.Shelf.Id == shelfId)
            .ToList();

        IList<StorageLocInfo> storageLocListFromExcel = new List<StorageLocInfo>();

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

            // 层 列 下架顺序
            cellIndex = 0;
            string rowNumStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            if (string.IsNullOrWhiteSpace(rowNumStr))
            {
                break;
            }

            string colNumStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;
            string offShelfOrderStr = row.GetCell(cellIndex++)?.StringCellValue ?? string.Empty;

            if (int.TryParse(rowNumStr, out int rowNum) == false)
            {
                throw new ValidationException($"第{rowIndex}行数据 行数 数据格式不对！");
            }

            if (int.TryParse(colNumStr, out int colNum) == false)
            {
                throw new ValidationException($"第{rowIndex}行数据 列数 数据格式不对！");
            }

            if (int.TryParse(offShelfOrderStr, out int offShelfOrderNum) == false)
            {
                throw new ValidationException($"第{rowIndex}行数据 下架顺序 数据格式不对！");
            }

            StorageLocInfo storageLoc = new StorageLocInfo();
            storageLoc.RowNum = rowNum;
            storageLoc.ColNum = colNum;
            storageLoc.OffShelfOrderNum = offShelfOrderNum;

            var slInDb = storageLocListInDb.Where(e => e.RowNum == storageLoc.RowNum && e.ColNum == storageLoc.ColNum).Take(1).SingleOrDefault();
            if (slInDb == null)
            {
                throw new ValidationException($"第{rowIndex}行数据 指定的行{rowNum}列{colNum}仓位，在当前货架不存在！");
            }

            storageLocListFromExcel.Add(storageLoc);
        }

        foreach (var sl in storageLocListFromExcel)
        {
            var slInDb = storageLocListInDb.Where(e => e.RowNum == sl.RowNum && e.ColNum == sl.ColNum).Take(1).SingleOrDefault();

            if (slInDb != null)
            {
                slInDb.OffShelfOrderNum = sl.OffShelfOrderNum;
                session.Update(slInDb);
            }
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "导入成功！" },
        });
    }
}