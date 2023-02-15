using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.Home;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Service.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

public class HomeController : Controller {
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly WechatAppLoginService wechatAppLoginService;
    private readonly ClientHomeDataService clientHomeDataService;

    public HomeController(ISession session,
        LoginCustomerService loginCustomerService,
        WechatAppLoginService wechatAppLoginService,
        ClientHomeDataService clientHomeDataService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.wechatAppLoginService = wechatAppLoginService;
        this.clientHomeDataService = clientHomeDataService;
    }

    // 这个只能通过dotnet 部署的端口号访问到
    [NoLoginRequired]
    [HttpGet]
    [Route("/")]
    public IActionResult Index() {
        var model = new HomeIndexModel();
        return View("HomeIndex", model);
    }

    // 这个可以通过nginx反向代理访问到
    [HttpGet]
    [Route("/client")]
    public IActionResult ClientIndex() {
        var model = new HomeClientIndexModel();
        return View("HomeClientIndex", model);
    }

    [NoLoginRequired] // 里面会自动登录
    [HttpPost]
    [Route("/client/home/data")]
    [Transactional] // login的时候需要事务
    public IActionResult ClientHomeData([FromBody] HomeClientHomeDataParam param) {
        if (loginCustomerService.IsLogined == false) {
            // 这时才是需要验证这个参数
            ValidationUtil.Validate<HomeClientHomeDataParamValidator, HomeClientHomeDataParam>(param);
            wechatAppLoginService.Login(param.Code, Request);
        }

        // 这时肯定已经登录
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var homeData = clientHomeDataService.GetCustomerClientHomeData(loginCustomerId);

        var model = new HomeClientHomeDataModel {
            ExpressClaimBadge = homeData.ExpressClaimCount.ToString(),
            HasExpressExpectEnter = homeData.ExpressExpectEnterCount > 0,
            ExpressExpectBadge = homeData.ExpressExpectEnterCount > 0 ? homeData.ExpressExpectEnterCount.ToString() : homeData.ExpressExpectCount.ToString(),
            ExpressCheckinBadge = homeData.ExpressCheckinCount > 0 ? homeData.ExpressCheckinCount.ToString() : string.Empty,
            ExpressReturnBadge = homeData.ExpressReturnCount > 0 ? homeData.ExpressReturnCount.ToString() : string.Empty,
            OrderSubmittedBadge = homeData.OrderSubmittedCount > 0 ? homeData.OrderSubmittedCount.ToString() : string.Empty,
            OrderWaitingTransportBadge = homeData.OrderWaitingTransportCount > 0 ? homeData.OrderWaitingTransportCount.ToString() : string.Empty,
            OrderTransportingBadge = homeData.OrderTransportingCount > 0 ? homeData.OrderTransportingCount.ToString() : string.Empty,
            OrderDeliveredBadge = homeData.OrderDeliveredCount > 0 ? homeData.OrderDeliveredCount.ToString() : string.Empty,
            NewMessageBadge = homeData.NewMessageCount > 0 ? homeData.NewMessageCount.ToString() : string.Empty,
            CouponBatchPickBadge = homeData.HasCouponBatchPick ? "抢" : string.Empty,
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new {
                homeData = model,
            },
        });
    }
}