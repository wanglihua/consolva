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
using shunda.Service.Customer;
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
    private readonly ISession session;
    private readonly ExpressListService expressListService;
    private readonly ExpressSizeWeightService expressSizeWeightService;
    private readonly ExpressPhotoFileService expressPhotoFileService;
    private readonly LoginUserService loginUserService;
    private readonly CustomerCodeService customerCodeService;

    public ExpressClaimController(ISession session,
        ExpressListService expressListService,
        ExpressSizeWeightService expressSizeWeightService,
        ExpressPhotoFileService expressPhotoFileService,
        LoginUserService loginUserService,
        CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.expressListService = expressListService;
        this.expressSizeWeightService = expressSizeWeightService;
        this.expressPhotoFileService = expressPhotoFileService;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
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
        IList<string> trackingNoList = expressList.Select(e => e.TrackingNo).ToList();
        IList<int> orderIdList = expressList.Where(e => e.Order != null).Select(e => e.Order!.Id).Distinct().ToList();

        List<int> userIdList = new List<int>();
        userIdList.AddRange(expressList.Select(e => e.AddUser.Id));
        userIdList.AddRange(expressList.Where(e => e.OnShelfUser != null).Select(e => e.OnShelfUser.Id));
        userIdList.AddRange(expressList.Where(e => e.OffShelfUser != null).Select(e => e.OffShelfUser.Id));

        var orderStatusNameList = session.Query<LogisticsOrderInfo>()
            .Where(e => orderIdList.Contains(e.Id))
            .Select(new {OrderId = e.Id, OrderStatusName = e.OrderStatus.Name})
            .ToList();

        IList<ExpressExpectInfo> expressExpectList = session.Query<ExpressExpectInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IList<ExpressSizeWeightInfo> expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        IList<ExpressRemarkInfo> expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        IList<UserInfo> userList = session.Query<UserInfo>().Where(e => userIdList.Contains(e.Id)).ToList();

        foreach (ExpressInfo express in expressList)
        {
            int expressId = express.Id;
            string trackingNo = express.TrackingNo;
            int orderId = express.Order?.Id ?? 0;

            express.CustomerCodeCache = customerCodeService.GetCustomerCode(express.Customer?.Id ?? 0);

            if(orderId != 0)
            {
                experss.OrderStatusName = orderStatusNameList.Where(e => e.OrderId == orderId).Take(1).SingleOrDefault()?.OrderStatusName ?? string.Empty;
            }

            express.ExpressExpectCache = expressExpectList.Where(e => e.TrackingNo == trackingNo).Take(1).SingleOrDefault();

            var goodsListForExpress = expressGoodsList
                .Where(e => e.Express.Id == expressId)
                .ToList();
            express.GoodsNamesCache = string.Join(",", goodsListForExpress.Select(e => e.Goods.Name).ToArray());

            express.ExpressSizeWeightListCache = expressSizeWeightList.Where(e => e.Express!.Id == expressId).ToList();
            express.ExpressRemarkListCache = expressRemarkList.Where(e => (e.Express?.Id ?? 0) == expressId).ToList();

            int addUserId = express.AddUser.Id;
            UserInfo? addUser = userList.Where(e => e.Id == addUserId).Take(1).SingleOrDefault();
            experss.AddUserNameCache = addUser?.UserName ?? string.Empty;

            if(express.OnShelfUser != null)
            {
                int onShelfUserId = express.OnShelfUser.Id;
                UserInfo? onShelfUser = userList.Where(e => e.Id == onShelfUserId).Take(1).SingleOrDefault();
                express.OnShelfUserNameCache = onShelfUser?.UserName ?? string.Empty;
            }

            if(express.OffShelfUser != null)
            {
                int offShelfUserId = express.OffShelfUser.Id;
                UserInfo? offShelfUser = userList.Where(e => e.Id == OffShelfUserId).Take(1).SingleOrDefault();
                express.OffShelfUserNameCache = offShelfUser?.UserName ?? string.Empty;
            }
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
            .Fetch(e => e.Goods)
            .Where(e => e.Express.Id == expressId)
            .ToList();

        foreach (var expressGoods in expressGoodsList)
        {
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

        model.ExpressItemList = expressSizeWeightService.GetExpressSizeWeightList(trackingNo);

        model.OrderPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
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
        model.CanCustomerServiceRemark = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Express.ExpressServiceRemark);

        // 打包用户和时间
        var packExpressList = session.Query<LogisticsOrderPackExpressInfo>()
            .Fetch(e => e.OrderPack).ThenFetch(e => e.PackUser)
            .Where(e => e.Express.Id == expressId)
            .OrderBy(e => e.OrderPack.Id)
            .ToList();

        model.PackUserTimeList = packExpressList.Select(e =>
                new UserTimeItem() {
                    User = e.OrderPack.PackUser,
                    Time = e.OrderPack.PackTime
                }
            )
            .ToList();

        // 装箱用户和时间
        IList<int> packIdList = Enumerable.ToList(packExpressList.Select(e => e.OrderPack.Id));
        var packBoxList = session.Query<LogisticsOrderPackBoxInfo>()
            .Fetch(e => e.ShipUser)
            .Where(e => packIdList.Contains(e.OrderPack.Id))
            .ToList();

        model.ShipUserTimeList = packBoxList
            .Where(e => e.ShipUser != null && e.ShipTime != null)
            .Select(e => new UserTimeItem() {
                User = e.ShipUser!,
                Time = e.ShipTime!.Value,
            })
            .ToList();

        return View("ExpressClaimDetail", model);
    }

    [AdminAuth(PassRes.Express.ExpressServiceRemark)]
    [HttpPost]
    [Route("/admin/express/claim/detail/save/customer/service/remark")]
    [Transactional]
    public IActionResult SaveExpressCustomerServiceRemark(int expressId, string serviceRemark)
    {
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null)
        {
            throw new ValidationException("指定的快递单不存在！");
        }

        // todo 是不是要判断状态，某些状态下不能保存这个客服备注

        if (string.IsNullOrWhiteSpace(serviceRemark))
        {
            // throw new ValidationException("客服备注不能为空！");
            serviceRemark = string.Empty;
        }

        if (serviceRemark.Length > 300)
        {
            throw new ValidationException("客服备注不能超过500个字符！");
        }

        var expressServiceRemark = session.Query<ExpressRemarkInfo>()
            .Where(e => e.Express!.Id == expressId && e.Type == ExpressRemarkType.Service)
            .Take(1)
            .SingleOrDefault();

        if (expressServiceRemark == null)
        {
            expressServiceRemark = new ExpressRemarkInfo();
            expressServiceRemark.Type = ExpressRemarkType.Service;
        }

        expressServiceRemark.Remark = serviceRemark;

        session.SaveOrUpdate(expressServiceRemark);
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

        IList<ExpressInfo> expressList = expressListService.List(ExpressListMenuCate.All, queryParam);

        IList<int> expressIdList = expressList.Select(e => e.Id).ToList();
        IList<string> trackingNoList = expressList.Select(e => e.TrackingNo).ToList();
        IList<int> orderIdList = expressList.Where(e => e.Order != null).Select(e => e.Order!.Id).Distinct().ToList();

        List<int> userIdList = new List<int>();
        userIdList.AddRange(expressList.Select(e => e.AddUser.Id));
        userIdList.AddRange(expressList.Where(e => e.OnShelfUser != null).Select(e => e.OnShelfUser.Id));
        userIdList.AddRange(expressList.Where(e => e.OffShelfUser != null).Select(e => e.OffShelfUser.Id));

        var orderStatusNameList = session.Query<LogisticsOrderInfo>()
            .Where(e => orderIdList.Contains(e.Id))
            .Select(new {OrderId = e.Id, OrderStatusName = e.OrderStatus.Name})
            .ToList();

        IList<ExpressExpectInfo> expressExpectList = session.Query<ExpressExpectInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        IList<ExpressGoodsInfo> expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();

        IList<ExpressSizeWeightInfo> expressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        IList<ExpressRemarkInfo> expressRemarkList = session.Query<ExpressRemarkInfo>()
            .Where(e => expressIdList.Contains(e.Express!.Id))
            .ToList();

        IList<UserInfo> userList = session.Query<UserInfo>().Where(e => userIdList.Contains(e.Id)).ToList();

        foreach (ExpressInfo express in expressList)
        {
            int expressId = express.Id;
            string trackingNo = express.TrackingNo;
            int orderId = express.Order?.Id ?? 0;

            express.CustomerCodeCache = customerCodeService.GetCustomerCode(express.Customer?.Id ?? 0);

            if(orderId != 0)
            {
                experss.OrderStatusName = orderStatusNameList.Where(e => e.OrderId == orderId).Take(1).SingleOrDefault()?.OrderStatusName ?? string.Empty;
            }

            express.ExpressExpectCache = expressExpectList.Where(e => e.TrackingNo == trackingNo).Take(1).SingleOrDefault();

            var goodsListForExpress = expressGoodsList
                .Where(e => e.Express.Id == expressId)
                .ToList();
            express.GoodsNamesCache = string.Join(",", goodsListForExpress.Select(e => e.Goods.Name).ToArray());

            express.ExpressSizeWeightListCache = expressSizeWeightList.Where(e => e.Express!.Id == expressId).ToList();
            express.ExpressRemarkListCache = expressRemarkList.Where(e => (e.Express?.Id ?? 0) == expressId).ToList();

            int addUserId = express.AddUser.Id;
            UserInfo? addUser = userList.Where(e => e.Id == addUserId).Take(1).SingleOrDefault();
            experss.AddUserNameCache = addUser?.UserName ?? string.Empty;

            if(express.OnShelfUser != null)
            {
                int onShelfUserId = express.OnShelfUser.Id;
                UserInfo? onShelfUser = userList.Where(e => e.Id == onShelfUserId).Take(1).SingleOrDefault();
                express.OnShelfUserNameCache = onShelfUser?.UserName ?? string.Empty;
            }

            if(express.OffShelfUser != null)
            {
                int offShelfUserId = express.OffShelfUser.Id;
                UserInfo? offShelfUser = userList.Where(e => e.Id == OffShelfUserId).Take(1).SingleOrDefault();
                express.OffShelfUserNameCache = offShelfUser?.UserName ?? string.Empty;
            }
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
        cell.SetCellValue("客户备注");        

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

        foreach (ExpressInfo cg in expressList)
        {
            row = sheet.CreateRow(rowIndex++);
            row.HeightInPoints = rowHeight;

            cellIndex = 0;

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.CheckinTrackingNo);

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
            cell.SetCellValue(cg.GoodsType.Name);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.GoodsNamesCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.OrderCode);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.Order?.OrderStatus.Name ?? string.Empty);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(string.Join(",", cg.ExpressSizeWeightListCache.Select(e => $"{e.Length}✕{e.Width}✕{e.Height}")));

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(string.Join(",", cg.ExpressSizeWeightListCache.Select(e => $"{e.VolumeWeightCache}")));

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(string.Join(",", cg.ExpressSizeWeightListCache.Select(e => $"{e.Weight}")));

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = precision0Style;
            cell.SetCellType(CellType.Numeric);
            cell.SetCellValue(cg.PackageCount);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.BillCustomerCode);

            string expectStr = "";
            if (cg.ExpressExpectCache != null)
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
            cell.SetCellValue(cg.CustomerCodeCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.ClaimTime?.ToString("yyyy-MM-dd HH:mm") ?? "");

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
            cell.SetCellValue(cg.AddUserNameCache);

            cell = row.CreateCell(cellIndex++);
            cell.CellStyle = contentCenterCellStyle;
            cell.SetCellType(CellType.String);
            cell.SetCellValue(cg.AddTime.ToString("yyyy-MM-dd HH:mm"));

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