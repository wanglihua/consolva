using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using shunda.Admin.Areas.Express.Models.Express;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
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
public class ExpressController : Controller {
    private readonly ISession session;
    private readonly ExpressListService expressListService;
    private readonly ExpressSizeWeightService expressSizeWeightService;
    private readonly ExpressPhotoFileService expressPhotoFileService;
    private readonly LoginUserService loginUserService;
    private readonly CustomerCodeService customerCodeService;
    private readonly ExpressExcelService expressExcelService;

    public ExpressController(ISession session,
        ExpressListService expressListService,
        ExpressSizeWeightService expressSizeWeightService,
        ExpressPhotoFileService expressPhotoFileService,
        LoginUserService loginUserService,
        CustomerCodeService customerCodeService,
        ExpressExcelService expressExcelService) {
        this.session = session;
        this.expressListService = expressListService;
        this.expressSizeWeightService = expressSizeWeightService;
        this.expressPhotoFileService = expressPhotoFileService;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
        this.expressExcelService = expressExcelService;
    }

    [HttpGet]
    [Route("/admin/express")]
    public IActionResult Index() {
        ExpressIndexModel model = new ExpressIndexModel();

        model.WarehouseList = session.Query<WarehouseInfo>()
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

        return View("ExpressIndex", model);
    }

    [HttpPost]
    [Route("/admin/express/list")]
    public IActionResult List(ExpressListQueryParam queryParam) {
        int totalCount = expressListService.Count(ExpressListMenuCate.All, queryParam);
        ListPager listPager = new ListPager(queryParam, totalCount);

        IList<ExpressInfo> expressList = expressListService.List(ExpressListMenuCate.All, queryParam, listPager);

        ExpressListModel model = new ExpressListModel(listPager);
        model.ExpressList = expressList;

        return View("ExpressList", model);
    }

    [HttpGet]
    [Route("/admin/express/tracking/no/detail/{trackingNo}")]
    public IActionResult TrackingNoDetail(string trackingNo) {
        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null) {
            throw new ValidationException($"指定的快递单号#{trackingNo}不存在！");
        }

        return Detail(ExpressListMenuCate.All.Id, (express?.Id ?? 0));
    }

    [HttpGet]
    [Route("/admin/express/detail/{cateId:int}/{id:int}")]
    public IActionResult Detail(int cateId, int id) {
        int expressId = id;

        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        ExpressDetailModel model = new ExpressDetailModel();
        model.Express = express;
        model.AddUser = express.AddUser;

        model.Cate = ExpressListMenuCate.GetById(cateId);

        var customer = express.Customer;
        if (customer != null) {
            customer.CustomerCodeCache = customerCodeService.GetCustomerCode(customer.Id);
        }

        model.Customer = customer;

        IList<ExpressGoodsInfo> expressGoodsList = session
            .Query<ExpressGoodsInfo>()
            .Fetch(e => e.Goods)
            .Where(e => e.Express.Id == expressId)
            .ToList();

        foreach (var expressGoods in expressGoodsList) {
        }

        model.ExpressGoodsList = expressGoodsList;

        NHibernateUtil.Initialize(express.Order);

        string trackingNo = express.TrackingNo;

        ExpressExpectInfo? expressExpect = session.Query<ExpressExpectInfo>()
            .Fetch(e => e.Customer)
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();
        model.ExpressExpect = expressExpect;

        model.ExpressSizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .ToList();

        model.ExpressPhotoList = session.Query<ExpressPhotoInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .OrderBy(e => e.Type)
            .ToList();

        foreach (ExpressPhotoInfo orderPhoto in model.ExpressPhotoList) {
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

        model.ExpressRemarkList = session.Query<ExpressRemarkInfo>().Where(e => e.Express!.Id == expressId).ToList();

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

        var expressReturn = session.Query<ExpressReturnInfo>()
            .Where(e => e.Express.Id == expressId)
            .Take(1)
            .SingleOrDefault();
        model.ExpressReturn = expressReturn ?? new ExpressReturnInfo();

        return View("ExpressDetail", model);
    }

    [AdminAuth(PassRes.Express.ExpressServiceRemark)]
    [HttpPost]
    [Route("/admin/express/detail/save/customer/service/remark")]
    [Transactional]
    public IActionResult SaveExpressCustomerServiceRemark(int expressId, string serviceRemark) {
        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException("指定的快递单不存在！");
        }

        // todo 是不是要判断状态，某些状态下不能保存这个客服备注

        if (string.IsNullOrWhiteSpace(serviceRemark)) {
            // throw new ValidationException("客服备注不能为空！");
            serviceRemark = string.Empty;
        }

        if (serviceRemark.Length > 300) {
            throw new ValidationException("客服备注不能超过500个字符！");
        }

        var expressServiceRemark = session.Query<ExpressRemarkInfo>()
            .Where(e => e.Express!.Id == expressId && e.Type == ExpressRemarkType.Service)
            .Take(1)
            .SingleOrDefault();

        if (expressServiceRemark == null) {
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
    [Route("/admin/express/export")]
    public IActionResult Export(ExpressListQueryParam queryParam) {
        int totalCount = expressListService.Count(ExpressListMenuCate.All, queryParam);
        if (totalCount > 10000) // 暂时不能导出超1万个吧
        {
            throw new ValidationException("导出的快递单号太多了，请缩小查询条件！");
        }

        IList<ExpressInfo> expressList = expressListService.List(ExpressListMenuCate.All, queryParam);

        string fileName = $"快递单导出.xlsx";
        var excel = expressExcelService.Export(expressList);

        return File(excel, ContentType.Xlsx, fileName);
    }
}