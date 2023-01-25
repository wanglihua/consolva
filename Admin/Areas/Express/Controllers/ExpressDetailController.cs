using Microsoft.AspNetCore.Mvc;
using NHibernate;
using NHibernate.Linq;
using shunda.Admin.Areas.Express.Models.Express;
using shunda.Admin.Areas.Express.Models.ExpressDetail;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Customer;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Express.Controllers;

[ExpressArea]
[AdminAuth(PassRes.Express.ExpressList)]
public class ExpressDetailController : Controller {
    private readonly ISession session;
    private readonly ExpressPhotoFileService expressPhotoFileService;
    private readonly LoginUserService loginUserService;
    private readonly CustomerCodeService customerCodeService;

    public ExpressDetailController(ISession session,
        ExpressPhotoFileService expressPhotoFileService,
        LoginUserService loginUserService,
        CustomerCodeService customerCodeService
    ) {
        this.session = session;
        this.expressPhotoFileService = expressPhotoFileService;
        this.loginUserService = loginUserService;
        this.customerCodeService = customerCodeService;
    }
    
    [HttpGet]
    [Route("/admin/express/tracking/no/detail/{trackingNo}")]
    public IActionResult TrackingNo(string trackingNo) {
        ExpressInfo? express = session.Query<ExpressInfo>()
            .Where(e => e.TrackingNo == trackingNo)
            .Take(1)
            .SingleOrDefault();

        if (express == null) {
            throw new ValidationException($"指定的快递单号#{trackingNo}不存在！");
        }

        return Index(ExpressListMenuCate.All.Id, (express.Id));
    }

    [HttpGet]
    [Route("/admin/express/detail/{cateId:int}/{id:int}")]
    public IActionResult Index(int cateId, int id) {
        int expressId = id;

        ExpressInfo express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        var model = new ExpressDetailIndexModel();
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
        model.CanCustomerServiceRemark = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.Express.ExpressRemarkEdit);

        model.ExpressRemarkList = session.Query<ExpressRemarkInfo>().Where(e => e.Express.Id == expressId).ToList();

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

        return View("ExpressDetailIndex", model);
    }    
}