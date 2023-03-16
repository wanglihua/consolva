using Alipay.AopSdk.AspnetCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderAlipay;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Pay.Controllers;

/// <summary>
/// 公众号支付宝支付 处理支付接口
/// </summary>
[NoLoginRequired]
[PayArea]
public class WechatOfficialAccountOrderAlipayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly AlipayService alipayService;
    private readonly AlipayOrderService alipayOrderService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly CustomerScoreService customerScoreService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;

    public WechatOfficialAccountOrderAlipayController(
        ISession session,
        PayService payService,
        AlipayService alipayService,
        AlipayOrderService alipayOrderService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService,
        LogisticsOrderPaySumService logisticsOrderPaySumService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayService = alipayService;
        this.alipayOrderService = alipayOrderService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerScoreService = customerScoreService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
    }

    #region 支付宝支付

    /// <summary>
    /// 用于微信界面复制URL
    /// </summary>
    /// <param name="orderPayCode">订单ID加密码</param>
    /// <param name="couponPayCode">优惠券ID加密码</param>
    /// <returns></returns>
    [HttpGet]
    // [Route("/client/wechat/official/account/alipay/{orderPayCode}/{couponPayCode}")]
    [Route("/client/order/alipay/{orderPayCode}/{couponPayCode}")] // 让复制出来的URL好看一点
    public IActionResult CopyUrl(string orderPayCode, string couponPayCode) {
        // 检查参数正确性
        int orderId = payService.GetIdFromPayCodeInUrl(orderPayCode);
        int couponId = payService.GetIdFromPayCodeInUrl(couponPayCode);

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券{couponId}不存在！");
        }

        if (WebUtil.IsWechatRequest(Request)) {
            WechatOfficialAccountAlipayCopyUrlModel model = new WechatOfficialAccountAlipayCopyUrlModel {
                Url = Request.GetEncodedUrl()
            };
            return View("WechatOfficialAccountOrderAlipayCopyUrl", model);
        }

        return RedirectToAction("AliPayRequest", new { orderPayCode = orderPayCode, couponPayCode = couponPayCode });
    }

    /// <summary>
    /// 发起支付请求
    /// </summary>
    /// <param name="orderPayCode">订单ID加密码</param>
    /// <param name="couponPayCode">优惠券ID加密码</param>
    /// <returns></returns>
    [HttpGet]
    // [Route("/client/wechat/official/account/alipay/request/{orderPayCode}/{couponPayCode}")]
    [Route("/client/order/alipay/request/{orderPayCode}/{couponPayCode}")] // 让复制出来的URL好看一点
    public IActionResult AliPayRequest(string orderPayCode, string couponPayCode) {
        if (WebUtil.IsWechatRequest(Request)) {
            return RedirectToAction("CopyUrl", new { orderPayCode = orderPayCode, couponPayCode = couponPayCode });
        }

        int orderId = payService.GetIdFromPayCodeInUrl(orderPayCode);
        int couponId = payService.GetIdFromPayCodeInUrl(couponPayCode);
        bool isMobile = WebUtil.IsMobileRequest(Request);

        AlipayOrderCreateParam createParam = new AlipayOrderCreateParam {
            OrderId = orderId,
            CouponId = couponId,
            IsMobile = isMobile,
            ReturnUrl = SiteConfig.SiteUrlBase + Url.Action("Return", "WechatOfficialAccountOrderAlipay"),
            NotifyUrl = SiteConfig.SiteUrlBase + Url.Action("Notify", "WechatOfficialAccountOrderAlipay"),
        };

        string alipayRequestUrl = alipayOrderService.Create(createParam);
        return Redirect(alipayRequestUrl);
    }

    [NonAction]
    private Tuple<bool, LogisticsOrderInfo?> AlipayNotifyInternal() {
        log.Warn("支付宝支付异步回调 begin");

        Dictionary<string, string> sArray = new Dictionary<string, string>();

        ICollection<string> requestItem = Request.Form.Keys;
        foreach (var item in requestItem) {
            sArray.Add(item, Request.Form[item]);
        }

        try {
            log.Error("支付宝支付异步回调 数据: " + JsonConvert.SerializeObject(sArray));
        }
        catch (Exception exp) {
            log.Error(exp);
        }

        if (sArray.Count == 0) {
            log.Error("支付宝支付异步回调 end fail_001");

            // return Content("fail");
            return new Tuple<bool, LogisticsOrderInfo?>(false, null);
        }

        bool flag = alipayService.RSACheckV1(sArray);
        if (flag) {
            //交易状态
            //判断该笔订单是否在商户网站中已经做过处理
            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
            //请务必判断请求时的total_amount与通知时获取的total_fee为一致的
            //如果有做过处理，不执行商户的业务程序

            string outTradeNo = sArray["out_trade_no"];
            var orderPay = session.Query<LogisticsOrderPayInfo>()
                .Where(e => e.OutTradeNo == outTradeNo)
                .Take(1)
                .SingleOrDefault();
            if (orderPay == null) {
                log.Error("支付宝支付异步回调 end fail_002");
                // return Content("fail");
                return new Tuple<bool, LogisticsOrderInfo?>(false, null);
            }

            if (orderPay.Status == PayStatus.Paid) // 如果该笔支付已经标记为成功，就直接返回 success 就行
            {
                log.Warn("支付宝支付异步回调 end success 1");
                // return Content("success");
                return new Tuple<bool, LogisticsOrderInfo?>(true, orderPay.Order);
            }

            decimal.TryParse(sArray["total_amount"], out decimal payAmount);

            orderPay.Amount = payAmount;
            orderPay.Status = PayStatus.Paid;
            session.Update(orderPay);
            session.Flush();

            LogisticsOrderInfo order = orderPay.Order;

            PayStatus orderPayStatusBefore = order.PayStatus;

            // 合计标记支付状态
            logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(order.Id);

            PayStatus orderPayStatusAfter = order.PayStatus;

            // 记录日志
            logisticsOrderActionService.NewActionWithoutUserCustomer(order.Id, LogisticsOrderActionType.Pay, orderPayStatusBefore, orderPayStatusAfter, $"金额:{payAmount:#0.00}，支付单号:{orderPay.OutTradeNo}");

            // 会员积分
            customerScoreService.AddScore(orderPay.Id);


            //注意：
            //退款日期超过可退款期限后（如三个月可退款），支付宝系统发送该交易状态通知

            log.Warn("支付宝支付异步回调 end success 2");
            // return Content("success");
            return new Tuple<bool, LogisticsOrderInfo?>(true, order);
        }

        log.Error("支付宝支付异步回调 end fail_003");
        // return Content("fail");
        return new Tuple<bool, LogisticsOrderInfo?>(false, null);
    }

    /// <summary>
    /// 支付异步回调通知 需配置域名 因为是支付宝主动post请求这个action 所以要通过域名访问或者公网ip
    /// </summary>
    [HttpPost]
    // [Route("/client/wechat/official/account/alipay/notify")]
    [Route("/client/order/alipay/notify")] // 跟这个控制器中其他的URL一致
    public IActionResult Notify() {
        Dictionary<string, string> callbackDataDict = new Dictionary<string, string>();

        ICollection<string> requestItem = Request.Form.Keys;
        foreach (var item in requestItem) {
            callbackDataDict.Add(item, Request.Form[item]);
        }

        try {
            Transactional.Wrap(session, () => { alipayOrderService.Callback(callbackDataDict); });
            return Content("success");
        }
        catch (Exception exp) {
            log.Error(exp);
            log.Error("支付宝支付异步回调 end fail_004");

            return Content("fail");
        }
    }

    /// <summary>
    /// 支付同步回调
    /// </summary>
    [HttpGet]
    // [Route("/client/wechat/official/account/alipay/callback")]
    [Route("/client/order/alipay/return")] // 让浏览器里面的URL好看点
    public IActionResult Return() {
        Dictionary<string, string> callbackDataDict = new Dictionary<string, string>();

        ICollection<string> requestItem = Request.Form.Keys;
        foreach (var item in requestItem) {
            callbackDataDict.Add(item, Request.Form[item]);
        }

        try {
            Transactional.Wrap(session, () => { alipayOrderService.Callback(callbackDataDict); });
            var model = new WechatOfficialAccountOrderAlipayReturnModel {
                Success = true,
                Message = "支付成功！",
            };
            return View("WechatOfficialAccountOrderAlipayReturn", model);
        }
        catch (Exception exp) {
            log.Error(exp);
            log.Error("支付宝支付同步回调 end fail_004");

            var model = new WechatOfficialAccountOrderAlipayReturnModel {
                Success = false,
                Message = "支付失败！",
            };
            return View("WechatOfficialAccountOrderAlipayReturn", model);
        }
    }

    #endregion
}