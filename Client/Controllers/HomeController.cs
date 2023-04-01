using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Client.Models.Home;
using shunda.Client.Services;
using shunda.Core.Config;
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
    private readonly AdvertBannerImageService advertBannerImageService;
    private readonly CustomerVipDiscountService customerVipDiscountService;

    public HomeController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerCodeService customerCodeService,
        WechatAppLoginService wechatAppLoginService,
        ClientHomeDataService clientHomeDataService,
        AdvertBannerImageService advertBannerImageService,
        CustomerVipDiscountService customerVipDiscountService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerCodeService = customerCodeService;
        this.wechatAppLoginService = wechatAppLoginService;
        this.clientHomeDataService = clientHomeDataService;
        this.advertBannerImageService = advertBannerImageService;
        this.customerVipDiscountService = customerVipDiscountService;
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
        var customer = loginCustomerService.GetLoginCustomer();

        // 客户信息
        string customerCode = customerCodeService.GetCustomerCode(loginCustomerId);
        decimal discountPercent = 0; // >=0

        var vipDiscountList = customerVipDiscountService.GetCustomerVipDiscountList();

        // VIP折扣
        CustomerVipDiscountInfo? vipDiscount = customerVipDiscountService.GetDiscount(loginCustomerId, vipDiscountList);
        if (vipDiscount != null && vipDiscount.Discount > discountPercent) {
            discountPercent = vipDiscount.Discount;
        }

        // 超级VIP折扣
        CustomerSuperVipDiscountInfo? superVipDiscount = session.Query<CustomerSuperVipDiscountInfo>()
            .Where(e => e.Customer.Id == loginCustomerId)
            .Take(1)
            .SingleOrDefault();
        if (superVipDiscount != null && superVipDiscount.Discount > discountPercent) {
            discountPercent = superVipDiscount.Discount;
        }

        string discountDesc = "无折扣";
        if (discountPercent > 0) {
            discountDesc = (int)(100 - discountPercent) + "折";
        }

        var vipDiscountFromDiscountPercent = vipDiscountList
            .Where(e => e.Discount <= discountPercent)
            .OrderByDescending(e => e.Discount)
            .Take(1)
            .SingleOrDefault();

        string customerLevelTitle = vipDiscountFromDiscountPercent?.Rank.Name ?? string.Empty; // VIP1,2,3...
        if (string.IsNullOrWhiteSpace(customerLevelTitle)) {
            customerLevelTitle = customer.Level.Name;
        }

        customerCode = $"SD{customerCode} {customerLevelTitle} {discountDesc}";

        if (vipDiscountFromDiscountPercent != null
            && (customer.Level != CustomerLevel.Vip || customer.VipRank != vipDiscountFromDiscountPercent.Rank)) {
            Transactional.Wrap(session, () => {
                customer.Level = CustomerLevel.Vip;
                customer.VipRank = vipDiscountFromDiscountPercent.Rank;

                session.Update(customer);
                session.Flush();
            });
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new HomeClientHeaderDataModel {
                CustomerCode = customerCode,
                Agreed = customer.Agreed,
            },
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

        var topImageList = session.Query<AdvertBannerImageInfo>()
            .Where(e => e.Banner.Loc == AdvertBannerLoc.HomeTop)
            .OrderBy(e => e.ImageNum)
            .ThenBy(e => e.Id)
            .ToList();

        var topImageItemList = topImageList
            .Select(e => new HomeClientImageItem() {
                Id = e.Id,
                ImageUrl = SiteConfig.SiteUrlBase + advertBannerImageService.GetImageFullUrl(e),
                LinkUrl = e.LinkUrl,
            })
            .ToList();

        var homeData = clientHomeDataService.GetCustomerClientHomeData(loginCustomerId);

        string homeNote = @"
<div style='font-weight: bold; margin-bottom: 8px;'>亲爱的舜达全球集运用户</div>
<div>你进入小程序后，请先点击或取仓库地址，一键复制所有信息，在淘宝/京东/拼多多等粘贴您的地址信息，请一定要将ID编号输入在收件人名字后面和地址结尾处，避免被快递公司隐藏。</div>
";

        var model = new HomeClientHomeDataModel {
            TopImageList = topImageItemList,
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
            BeginnerGuideUrl = homeData.BeginnerGuideUrl,
            FaqUrl = homeData.FaqUrl,
            ScoreRemarkUrl = homeData.ScoreRemarkUrl,
            HomeNote = homeNote,
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