using Alipay.AopSdk.AspnetCore;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatOfficialAccountOrderAlipay;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Pay.Controllers;

/// <summary>
/// 公众号支付宝支付 处理支付接口
/// </summary>
[PayArea]
public class WechatOfficialAccountOrderAlipayController : Controller
{
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly PayService payService;
    private readonly AlipayService alipayService;
    private readonly AlipayOrderService alipayOrderService;

    public WechatOfficialAccountOrderAlipayController(
        ISession session,
        LoginCustomerService loginCustomerService,
        PayService payService,
        AlipayService alipayService,
        AlipayOrderService alipayOrderService
    )
    {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.payService = payService;
        this.alipayService = alipayService;
        this.alipayOrderService = alipayOrderService;
    }

    private const string ORDER_ALIPAY_URL = "/client/order/alipay/{scoreDeduct}/{orderPayCode}/{couponPayCode}";

    [HttpPost]
    [Route("/client/order/alipay/create/url")]
    public IActionResult CreateUrl([FromBody] WechatOfficialAccountOrderAlipayCreateUrlParam param)
    {
        int orderId = param.OrderId;
        int couponId = param.CouponId;

        int customerId = loginCustomerService.GetLoginCustomerId();

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.Customer.Id != customerId)
        {
            throw new ValidationException($"指定的订单{order.Code}不是当前登录用户的！");
        }

        string orderPayCode = payService.GenereatePayCodeInUrl(orderId);
        string couponPayCode = payService.GenereatePayCodeInUrl(couponId);

        string url = ORDER_ALIPAY_URL.Replace("{scoreDeduct}", param.ScoreDeduct ? "1" : "0");
        url = url.Replace("{orderPayCode}", orderPayCode);
        url = url.Replace("{couponPayCode}", couponPayCode);
        url = SiteConfig.SiteUrlBase + url;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new WechatOfficialAccountOrderAlipayCreateUrlModel {
                Url = url,
            },
        });
    }

    /// <summary>
    /// 用于复制粘贴到浏览器的URL
    /// </summary>
    /// <param name="scoreDeduct">是否积分抵扣</param>
    /// <param name="orderPayCode">订单ID加密码</param>
    /// <param name="couponPayCode">优惠券ID加密码</param>
    /// <returns></returns>
    [NoLoginRequired]
    [HttpGet]
    [Route(ORDER_ALIPAY_URL)] // 让复制出来的URL好看一点
    public IActionResult CopyUrl(int scoreDeduct, string orderPayCode, string couponPayCode)
    {
        // 检查参数正确性
        int orderId = payService.GetIntFromPayCodeInUrl(orderPayCode);
        int couponId = payService.GetIntFromPayCodeInUrl(couponPayCode);

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (couponId != 0)
        {
            var coupon = session.Get<CouponInfo>(couponId);
            if (coupon == null)
            {
                throw new ValidationException($"指定的优惠券{couponId}不存在！");
            }
        }

        if (WebUtil.IsWechatRequest(Request))
        {
            WechatOfficialAccountOrderAlipayCopyUrlModel model = new WechatOfficialAccountOrderAlipayCopyUrlModel {
                Url = Request.GetEncodedUrl()
            };
            return View("WechatOfficialAccountOrderAlipayCopyUrl", model);
        }

        return RedirectToAction("AliPayRequest", new { scoreDeduct = scoreDeduct, orderPayCode = orderPayCode, couponPayCode = couponPayCode });
    }

    /// <summary>
    /// 发起支付请求
    /// </summary>
    /// <param name="scoreDeduct">是否积分抵扣</param>
    /// <param name="orderPayCode">订单ID加密码</param>
    /// <param name="couponPayCode">优惠券ID加密码</param>
    /// <returns></returns>
    [NoLoginRequired]
    [HttpGet]
    // [Route("/client/wechat/official/account/alipay/request/{orderPayCode}/{couponPayCode}")]
    [Route("/client/order/alipay/request/{scoreDeduct}/{orderPayCode}/{couponPayCode}")] // 让复制出来的URL好看一点
    [Transactional]
    public IActionResult AliPayRequest(int scoreDeduct, string orderPayCode, string couponPayCode)
    {
        if (WebUtil.IsWechatRequest(Request))
        {
            return RedirectToAction("CopyUrl", new { scoreDeduct = scoreDeduct, orderPayCode = orderPayCode, couponPayCode = couponPayCode });
        }

        int orderId = payService.GetIntFromPayCodeInUrl(orderPayCode);
        int couponId = payService.GetIntFromPayCodeInUrl(couponPayCode);
        bool isMobile = WebUtil.IsMobileRequest(Request);

        var createParam = new AlipayOrderCreateParam {
            OrderId = orderId,
            CouponId = couponId,
            ScoreDeduct = scoreDeduct > 0,
            IsMobile = isMobile,
            ReturnUrl = SiteConfig.SiteUrlBase + Url.Action("Return", "WechatOfficialAccountOrderAlipay"),
            NotifyUrl = SiteConfig.SiteUrlBase + Url.Action("Notify", "WechatOfficialAccountOrderAlipay"),
        };

        string alipayRequestUrl = alipayOrderService.Create(createParam);
        return Redirect(alipayRequestUrl);
    }

    [NoLoginRequired]
    [HttpPost]
    // [Route("/client/wechat/official/account/alipay/notify")]
    [Route("/client/order/alipay/notify")] // 跟这个控制器中其他的URL一致
    // 里面有事务
    public IActionResult Notify()
    {
        Dictionary<string, string> callbackDataDict = new Dictionary<string, string>();

        ICollection<string> requestItem = Request.Form.Keys;
        foreach (var item in requestItem)
        {
            callbackDataDict.Add(item, Request.Form[item]);
        }

        // "trade_status": "TRADE_SUCCESS"
        // "trade_status": "TRADE_FINISHED"
        string tradeStatus = callbackDataDict["trade_status"];
        if (tradeStatus != "TRADE_SUCCESS" && tradeStatus != "TRADE_FINISHED")
        {
            return Content("success");
        }

        try
        {
            Transactional.Wrap(session, () => { alipayOrderService.Notify(callbackDataDict); }); // 数据库事务
            return Content("success");
        }
        catch (Exception exp)
        {
            log.Error(exp);
            log.Error("支付宝支付异步回调 end fail_004");

            return Content("fail");
        }
    }

    [NoLoginRequired]
    [HttpGet]
    // [Route("/client/wechat/official/account/alipay/callback")]
    [Route("/client/order/alipay/return")] // 让浏览器里面的URL好看点
    // 里面有事务
    public IActionResult Return()
    {
        Dictionary<string, string> callbackDataDict = new Dictionary<string, string>();

        ICollection<string> requestItem = Request.Query.Keys;
        foreach (var item in requestItem)
        {
            callbackDataDict.Add(item, Request.Query[item]);
        }

        try
        {
            log.Error("支付宝支付同步回调 数据: " + JsonConvert.SerializeObject(callbackDataDict));
        }
        catch (Exception exp)
        {
            log.Error(exp);
        }

        bool checkSuccess = alipayService.RSACheckV1(callbackDataDict);
        if (checkSuccess == false)
        {
            throw new Exception("支付宝支付回调失败 end fail_003");
        }

        string outTradeNo = callbackDataDict["out_trade_no"];
        if (string.IsNullOrWhiteSpace(outTradeNo))
        {
            throw new Exception("支付宝支付回调 out_trade_no 为空！");
        }

        var model = new WechatOfficialAccountOrderAlipayReturnModel {
            Success = true,
            Message = "支付完成，等待支付宝通知，可从订单详细查看支付结果！",
        };

        var orderPay = session.Query<LogisticsOrderPayInfo>()
            .Where(e => e.OutTradeNo == outTradeNo)
            .Take(1)
            .SingleOrDefault();
        if (orderPay == null)
        {
            throw new Exception($"支付宝支付回调 指定的 out_trade_no:{outTradeNo} 不存在！");
        }

        // 如果该笔支付已经标记为成功，就直接返回 success 就行
        if (orderPay.Status == PayStatus.Paid)
        {
            model.Message = "支付已成功！";
        }

        return View("WechatOfficialAccountOrderAlipayReturn", model);
    }
}