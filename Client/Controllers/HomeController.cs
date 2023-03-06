using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.Home;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Domain.Customer;
using shunda.Service.Client;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Controllers;

public class HomeController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerCodeService customerCodeService;
    private readonly WechatAppLoginService wechatAppLoginService;
    private readonly ClientHomeDataService clientHomeDataService;

    public HomeController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerCodeService customerCodeService,
        WechatAppLoginService wechatAppLoginService,
        ClientHomeDataService clientHomeDataService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerCodeService = customerCodeService;
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
    [Route("/client/header/data")]
    // [Transactional]  WechatAppLoginService.Login() 里面有事务还有互斥锁，这里事务不能加
    public IActionResult ClientHeaderData([FromBody] HomeClientHeaderDataParam param) {
        if (loginCustomerService.IsLogined == false) {
            // 这时才是需要验证这个参数
            ValidationUtil.Validate<HomeClientHeaderDataParamValidator, HomeClientHeaderDataParam>(param);
            wechatAppLoginService.Login(param.Code, Request);
        }

        // 这时肯定已经登录
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        // 客户信息
        string customerCode = customerCodeService.GetCustomerCode(loginCustomerId);

        var customer = loginCustomerService.GetLoginCustomer();
        CustomerLevel customerLevel = customer.Level;
        if (customerLevel == CustomerLevel.SuperVip) {
            customerCode += $" {CustomerLevel.SuperVip.Name}";
        } else if (customerLevel == CustomerLevel.Vip) {
            customerCode += $" {customer.VipRank.Name}";
        } else if (customerLevel == CustomerLevel.Common) {
            customerCode += $" {customer.Level.Name}";
        }

        var model = new HomeClientHeaderDataModel {
            CustomerCode = customerCode,
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = model,
        });
    }

    [NoLoginRequired] // 里面会自动登录
    [HttpPost]
    [Route("/client/home/data")]
    // [Transactional]  WechatAppLoginService.Login() 里面有事务还有互斥锁，这里事务不能加
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
            ExpressClaimBadge = homeData.ExpressClaimCount > 0 ? homeData.ExpressClaimCount.ToString() : string.Empty,
            ExpressExpectBadge = homeData.ExpressExpectCount > 0 ? homeData.ExpressExpectCount.ToString() : string.Empty,
            ExpressCheckinBadge = homeData.ExpressCheckinCount > 0 ? homeData.ExpressCheckinCount.ToString() : string.Empty,
            ExpressReturnBadge = homeData.ExpressReturnCount > 0 ? homeData.ExpressReturnCount.ToString() : string.Empty,
            OrderSubmittedBadge = homeData.OrderSubmittedCount > 0 ? homeData.OrderSubmittedCount.ToString() : string.Empty,
            OrderPayBadge = homeData.OrderPayCount > 0 ? homeData.OrderPayCount.ToString() : string.Empty,
            OrderWaitingTransportBadge = homeData.OrderWaitingTransportCount > 0 ? homeData.OrderWaitingTransportCount.ToString() : string.Empty,
            OrderTransportingBadge = homeData.OrderTransportingCount > 0 ? homeData.OrderTransportingCount.ToString() : string.Empty,
            OrderDeliveredBadge = homeData.OrderDeliveredCount > 0 ? homeData.OrderDeliveredCount.ToString() : string.Empty,
            NewMessageBadge = homeData.NewMessageCount > 0 ? homeData.NewMessageCount.ToString() : string.Empty,
            NewNoticeBadge = homeData.NewNoticeCount > 0 ? homeData.NewNoticeCount.ToString() : string.Empty,
            CouponBatchPickBadge = homeData.HasCouponBatchPick ? "抢" : string.Empty,
            UseAgreementBadge = homeData.UseAgreementHasNew ? " " : string.Empty,
            InsuranceClauseBadge = homeData.InsuranceClauseHasNew ? " " : string.Empty,
            FreightDescBadge = homeData.FreightDescHasNew ? " " : string.Empty,
            OrderProcessBadge = homeData.OrderProcessHasNew ? " " : string.Empty,
            PromotionBadge = homeData.PromotionHasNew ? " " : string.Empty,
            StorageInstructionBadge = homeData.StorageInstructionHasNew ? " " : string.Empty,
            GoodsTypeBadge = homeData.GoodsTypeHasNew ? " " : string.Empty,
            HasFollowed = homeData.HasFollowed,
            UseAgreementUrl = homeData.UseAgreementUrl,
            InsuranceClauseUrl = homeData.InsuranceClauseUrl,
            FreightDescUrl = homeData.FreightDescUrl,
            OrderProcessUrl = homeData.OrderProcessUrl,
            PromotionUrl = homeData.PromotionUrl,
            StorageInstructionUrl = homeData.StorageInstructionUrl,
            GoodsTypeUrl = homeData.GoodsTypeUrl,
            FollowTipUrl = homeData.FollowTipUrl,
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