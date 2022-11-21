using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Express.Models.ExpressClaim;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using shunda.Service;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

/// <summary>
/// 快递详情
/// </summary>
[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressClaimController : Controller
{
    private ISession session;
    private ExpressListService expressListService;
    private ExpressSizeWeightService expressSizeWeightService;
    private ExpressPhotoFileService expressPhotoFileService;
    private LoginUserService loginUserService;

    public ExpressClaimController(ISession session,
        ExpressListService expressListService,
        ExpressSizeWeightService expressSizeWeightService,
        ExpressPhotoFileService expressPhotoFileService,
        LoginUserService loginUserService)
    {
        this.session = session;
        this.expressListService = expressListService;
        this.expressSizeWeightService = expressSizeWeightService;
        this.expressPhotoFileService = expressPhotoFileService;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/express/claim")]
    public IActionResult Index()
    {
        ExpressClaimIndexModel model = new ExpressClaimIndexModel();

        model.CnWarehouseList = session.Query<WarehouseInfo>()
            .Where(e => e.InnerStatus == WarehouseStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .ToList();

        model.CheckinUserList = session.Query<WarehouseWorkerInfo>()
            .Fetch(e => e.User)
            .Where(e => e.Type == WarehouseWorkerType.Checkin)
            .OrderBy(e => e.Id)
            .Select(e => e.User)
            .ToList();

        bool isAdmin = loginUserService.IsAdmin;
        Dictionary<int, bool> passResIdMap = loginUserService.GetPassResIdMap();

        model.CanForceDelete = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Express.ExpressForceDelete);

        return View("ExpressClaimIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/claim/list")]
    public IActionResult List(ExpressListQueryParam queryParam)
    {
        int totalCount = expressListService.Count(ExpressListMenuCate.All, queryParam);
        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExpressInfo> expressList = expressListService.List(ExpressListMenuCate.All, queryParam, listPager);

        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();
        IList<int> goodsIdList = expressGoodsList.Select(e => e.Goods.Id).Distinct().ToList();
        IList<LogisticsGoodsInfo> goodsList = session.Query<LogisticsGoodsInfo>().Where(e => goodsIdList.Contains(e.Id)).ToList();

        foreach (ExpressInfo express in expressList)
        {
            int expressId = express.Id;
            IList<int> goodsIdListForExpress = expressGoodsList
                .Where(e => e.Express.Id == expressId)
                .Select(e => e.Goods.Id)
                .Distinct()
                .ToList();
            express.GoodsNamesCache = string.Join(",", goodsList.Where(e => goodsIdListForExpress.Contains(e.Id)).Select(e => e.Name).ToArray());
        }

        ExpressClaimListModel model = new ExpressClaimListModel(listPager);
        model.ExpressClaimList = expressList;

        return View("ExpressClaimList", model);
    }

    [HttpGet]
    [Route("/admin/express/claim/tracking/no/detail/{trackingNo}")]
    public IActionResult TrackingNoDetail(string trackingNo)
    {
        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null)
        {
            throw new ValidationException($"指定的快递单号#{trackingNo}不存在！");
        }

        return Detail(ExpressListMenuCate.All.Id, (express?.Id ?? 0));
    }

    [HttpGet]
    [Route("/admin/express/claim/detail/{cateId:int}/{id:int}")]
    public IActionResult Detail(int cateId, int id)
    {
        int expressId = id;

        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        ExpressClaimDetailModel model = new ExpressClaimDetailModel();
        model.ClaimGoods = express;
        model.AddUser = express.AddUser;

        model.Cate = ExpressListMenuCate.GetById(cateId);
        model.Customer = express.Customer;

        IList<ExpressGoodsInfo> expressGoodsList = session
            .Query<ExpressGoodsInfo>()
            .Where(e => e.Express.Id == expressId)
            .ToList();

        foreach (var expressGoods in expressGoodsList)
        {
            LogisticsGoodsInfo goods = session.Get<LogisticsGoodsInfo>(expressGoods.GoodsId);
            expressGoods.GoodsNameCache = goods?.GetGoodsAdminName();
        }

        model.EntryList = expressGoodsList;

        NHibernateUtil.Initialize(express.Order);

        string trackingNo = express.TrackingNo;

        ExpressExpectInfo? expressExpect = session.Query<ExpressExpectInfo>()
            .Fetch(e => e.Customer)
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();
        model.ExpressExpect = expressExpect;

        LogisticsOrderPackExpressInfo orderPackExpress = session.Query<LogisticsOrderPackExpressInfo>()
            .Fetch(e => e.OrderExpress)
            .Fetch(e => e.OrderPack).ThenFetch(e => e.PackUser)
            .Where(e => e.OrderExpress.ExpressTrackingNo == trackingNo)
            .OrderByDescending(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        model.OrderPack = orderPackExpress?.OrderPack;

        model.ExpressItemList = expressSizeWeightService.GetExpressSizeWeightList(trackingNo);

        model.OrderPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.ExpressTrackingNo == trackingNo)
            .OrderBy(e => e.Type)
            .ToList();

        foreach (ExpressPhotoInfo orderPhoto in model.OrderPhotoList)
        {
            orderPhoto.BigImageFullUrlCache = expressPhotoFileService.GetBigImageFullUrlForSite(orderPhoto, OssSiteLocType.China);
            orderPhoto.SmallImageFullUrlCache = expressPhotoFileService.GetSmallImageFullUrlForSite(orderPhoto, OssSiteLocType.China);
        }

        model.ExpressLogList = session.Query<ExpressLogInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .OrderByDescending(e => e.Id)
            .ToList();

        bool isAdmin = loginUserService.IsAdmin;
        Dictionary<int, bool> passResIdMap = loginUserService.GetPassResIdMap();
        model.CanCustomerServiceRemark = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Express.ExpressCustomerServiceRemark);

        // 打包用户和时间
        var packExpressList = session.Query<LogisticsOrderPackExpressInfo>()
            .Fetch(e => e.OrderPack).ThenFetch(e => e.PackUser)
            .Where(e => e.ExpressNo == trackingNo)
            .OrderBy(e => e.OrderPack.Id)
            .ToList();

        model.PackUserTimeList = Enumerable.Select<UserTimeItem>(packExpressList, e =>
                new UserTimeItem() {
                    User = e.OrderPack.PackUser,
                    Time = e.OrderPack.PackTime
                }
            )
            .ToList();

        // 装箱用户和时间
        IList<int> packIdList = Enumerable.ToList(packExpressList.Select(e => e.OrderPack.Id));
        IList<int> packBoxIdList = session.Query<LogisticsOrderPackBoxInfo>()
            .Where(e => packIdList.Contains(e.OrderPack.Id))
            .Select(e => e.Id)
            .ToList();

        IList<LogisticsOrderPackBoxHistoryInfo> packBoxHistory = session.Query<LogisticsOrderPackBoxHistoryInfo>()
            .Fetch(e => e.User)
            .Where(e => packBoxIdList.Contains(e.OrderPackBox.Id)
                        && (e.Type == LogisticsOrderPackBoxHistoryType.LoadManual
                            || e.Type == LogisticsOrderPackBoxHistoryType.LoadScan
                            || e.Type == LogisticsOrderPackBoxHistoryType.UnloadScan
                            || e.Type == LogisticsOrderPackBoxHistoryType.UnloadManual))
            .ToList();

        IList<LoadUnloadUserTime> loadUnloadUserTimeList = new List<LoadUnloadUserTime>();
        packBoxIdList = packBoxIdList.OrderBy(e => e).ToList();
        foreach (var packBoxId in packBoxIdList)
        {
            LoadUnloadUserTime loadUnloadUserTime = new LoadUnloadUserTime();

            // load
            var loadHistory = packBoxHistory.Where(e => e.OrderPackBox.Id == packBoxId
                                                        && (e.Type == LogisticsOrderPackBoxHistoryType.LoadManual
                                                            || e.Type == LogisticsOrderPackBoxHistoryType.LoadScan))
                .Take(1).SingleOrDefault();

            if (loadHistory != null)
            {
                loadUnloadUserTime.LoadUserTime = new UserTimeItem() {
                    User = loadHistory.User,
                    Time = loadHistory.CreateTime
                };
            }

            // unload
            var unloadHistory = packBoxHistory.Where(e => e.OrderPackBox.Id == packBoxId
                                                          && (e.Type == LogisticsOrderPackBoxHistoryType.UnloadManual
                                                              || e.Type == LogisticsOrderPackBoxHistoryType.UnloadScan))
                .Take(1).SingleOrDefault();

            if (unloadHistory != null)
            {
                loadUnloadUserTime.UnloadUserTime = new UserTimeItem() {
                    User = unloadHistory.User,
                    Time = unloadHistory.CreateTime
                };
            }

            loadUnloadUserTimeList.Add(loadUnloadUserTime);
        }

        model.LoadUnloadUserTimeList = loadUnloadUserTimeList;

        return View("ExpressClaimDetail", model);
    }

    [AdminAuth(PassRes.Express.ExpressServiceRemark)]
    [HttpPost]
    [Route("/admin/express/claim/detail/save/customer/service/remark")]
    [Transactional]
    public IActionResult SaveExpressCustomerServiceRemark(int claimId, string serviceRemark)
    {
        ExpressInfo express = session.Get<ExpressInfo>(claimId);
        if (express == null)
        {
            throw new ValidationException("指定的快递单申领件不存在！");
        }

        // todo 是不是要判断状态，某些状态下不能保存这个客服备注

        if (string.IsNullOrWhiteSpace(serviceRemark))
        {
            // throw new ValidationException("客服备注不能为空！");
            serviceRemark = string.Empty;
        }

        if (serviceRemark.Length > 500)
        {
            throw new ValidationException("客服备注不能超过500个字符！");
        }

        express.ServiceRemark = serviceRemark;
        session.Update(express);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存客服备注成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/express/claim/export")]
    public IActionResult Export(ExpressListQueryParam queryParam)
    {
        int totalCount = expressListService.Count(ExpressListMenuCate.All, queryParam);
        if (totalCount > 10000) // 暂时不能导出超1万个吧
        {
            throw new ValidationException("导出的快递单号太多了，请缩小查询条件！");
        }

        IList<ExpressInfo> expressClaimList = expressListService.List(ExpressListMenuCate.All, queryParam);

        IList<int> expressIdList = expressClaimList.Select(e => e.Id).ToList();
        IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => expressIdList.Contains(e.ClaimId))
            .ToList();
        IList<int> goodsIdList = expressGoodsList.Select(e => e.GoodsId).Distinct().ToList();
        IList<LogisticsGoodsInfo> goodsList = session.Query<LogisticsGoodsInfo>().Where(e => goodsIdList.Contains(e.Id)).ToList();

        foreach (ExpressInfo express in expressClaimList)
        {
            IList<int> goodsIdListForClaim = expressGoodsList
                .Where(e => e.ClaimId == express.Id)
                .Select(e => e.GoodsId)
                .Distinct()
                .ToList();
            express.GoodsNamesCache = string.Join(",", goodsList.Where(e => goodsIdListForClaim.Contains(e.Id)).Select(e => e.Name).ToArray());
        }

        string fileName = $"快递单导出.xlsx";
        IWorkbook workbook = new XSSFWorkbook();

        string sheetName = "快递单列表";
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

        int cellIndex = 0;

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        ICell cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("澳拼入库号");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("快递单号");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("快递公司");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("物品类型");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("扩展名");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("所在订单");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("订单状态");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("尺寸cm");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("体积重kg");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("实际重kg");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("包裹数量");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("面单客户");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("是否预报");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("申领件描述");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("认领客户");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("认领时间");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("快递状态");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("上架状态");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("当前仓库");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("当前仓位号");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("入库仓库");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("入库仓位号");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("入库人员");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("入库时间");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("是否打木架");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("是否清理树皮");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("上架人员");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("上架时间");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("下架人员");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("下架时间");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("客服备注");

        sheet.SetColumnWidth(cellIndex, 16 * 256);
        cell = row.CreateCell(cellIndex++);
        cell.CellStyle = headerCenterCellStyle;
        cell.SetCellType(CellType.String);
        cell.SetCellValue("仓库备注");

        foreach (ExpressInfo cg in expressClaimList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.AoPinTrackingNo);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.TrackingNo);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.ExpressCompany);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.GoodsType?.Name ?? "");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.GoodsNamesCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.OrderCodeCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.OrderStatusNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(string.Join(",", cg.ExpressItemListCache.Select(e => $"{e.Length}✕{e.Width}✕{e.Height}")));

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(string.Join(",", cg.ExpressItemListCache.Select(e => $"{e.SingleVolumeWeight}")));

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(string.Join(",", cg.ExpressItemListCache.Select(e => $"{e.SingleWeight}")));

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision0Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(cg.ExpressPackageCount);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.BillCustomerCode);

            string expectStr = "";
            if (cg.HasExpectCache)
            {
                expectStr = "有预报";
            }
            else
            {
                expectStr = "无预报";
            }

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(expectStr);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.Description);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.CustomerNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.PickTime?.ToString("yyyy-MM-dd HH:mm") ?? "");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.Status?.Name ?? "");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.ShelfStatus?.Name ?? "");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.WarehouseNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.StorageLocNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.CheckinWarehouseNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.CheckinStorageLocNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.AddUserNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.AddTime.ToString("yyyy-MM-dd HH:mm"));

            string woodFrameStr = "";
            if (cg.WoodFrameFinal)
            {
                woodFrameStr = "要打木架";
            }
            else
            {
                woodFrameStr = "不打木架";
            }

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(woodFrameStr);

            string barkCleanStr = "";
            if (cg.BarkCleanFinal)
            {
                barkCleanStr = "要清理树皮";
            }
            else
            {
                barkCleanStr = "不清理树皮";
            }

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(barkCleanStr);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.OnShelfUserNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.OnShelfTime?.ToString("yyyy-MM-dd HH:mm") ?? "");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.OffShelfUserNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.OffShelfTime?.ToString("yyyy-MM-dd HH:mm") ?? "");

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.ServiceRemark);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.WarehouseRemark);
        }

        MemoryStream memoryStream = new MemoryStream();
        workbook.Write(memoryStream);
        memoryStream.Flush();

        return File(memoryStream.ToArray(), ContentType.Xlsx, fileName);
    }
}