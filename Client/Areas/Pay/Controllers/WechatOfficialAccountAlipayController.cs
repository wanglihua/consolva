using Alipay.AopSdk.AspnetCore;
using Alipay.AopSdk.Core.Domain;
using Alipay.AopSdk.Core.Request;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NHibernate;
using NLog;
using shunda.Client.Areas.Pay.Models.WechatOfficialAccountAlipay;
using shunda.Core;
using shunda.Core.Config;
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
public class WechatOfficialAccountAlipayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly AlipayService alipayService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly CustomerScoreService customerScoreService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;

    public WechatOfficialAccountAlipayController(
        ISession session,
        PayService payService,
        AlipayService alipayService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerScoreService customerScoreService,
        LogisticsOrderPaySumService logisticsOrderPaySumService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayService = alipayService;
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
    [Route("/client/wechat/official/account/alipay/{orderPayCode}/{couponPayCode}")]
    public IActionResult CopyUrl(string orderPayCode, string couponPayCode) {
        // 检查参数正确性
        int orderId = GetIdFromAlipayCode(orderPayCode);
        int couponId = GetIdFromAlipayCode(couponPayCode);

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
            return View("WechatOfficialAccountAlipayCopyUrl", model);
        }

        return RedirectToAction("AliPayRequest", new { orderPayCode = orderPayCode, couponPayCode = couponPayCode });
    }

    [NonAction]
    private string GenerateAlipayCode(int id) {
        string alipayCodeSalt = "shunda_alipay_secret_32lkjadfasd";

        string idStr = id.ToString();
        string md5 = Md5Util.Encrypt(idStr + alipayCodeSalt);
        return md5 + idStr;
    }

    [NonAction]
    private int GetIdFromAlipayCode(string payCode) {
        string errorMessage = "支付URL代码错误，请确认复制正确！";
        if (string.IsNullOrWhiteSpace(payCode) == true) {
            throw new ValidationException(errorMessage);
        }

        int md5Length = Md5Util.Md5Length;

        if (payCode.Length <= md5Length) {
            throw new ValidationException(errorMessage);
        }

        string idStr = payCode.Substring(md5Length);
        if (int.TryParse(idStr, out int id) == false) {
            throw new ValidationException(errorMessage);
        }

        string alipayCode = GenerateAlipayCode(id);
        if (alipayCode != payCode) {
            throw new ValidationException(errorMessage);
        }

        return id;
    }

    /// <summary>
    /// 发起支付请求
    /// </summary>
    /// <param name="orderPayCode">订单ID加密码</param>
    /// <param name="couponPayCode">优惠券ID加密码</param>
    /// <returns></returns>
    [HttpGet]
    [Route("/client/wechat/official/account/alipay/request/{orderPayCode}/{couponPayCode}")]
    public IActionResult AliPayRequest(string orderPayCode, string couponPayCode) {
        if (WebUtil.IsWechatRequest(Request)) {
            return RedirectToAction("CopyUrl", new { orderPayCode = orderPayCode, couponPayCode = couponPayCode });
        }

        log.Warn("支付宝支付发起 begin");

        int orderId = GetIdFromAlipayCode(orderPayCode);
        int couponId = GetIdFromAlipayCode(couponPayCode);

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券{couponId}不存在！");
        }

        if (order.ArrearsAmountFormula <= 0) {
            throw new ValidationException("当前订单无欠费金额，无需支付！");
        }

        // 外部订单号，商户网站订单系统中唯一的订单号
        string outTradeNo = payService.GenerateOutTradeNo();

        // 付款金额
        string totalAmount = order.ArrearsAmountFormula.ToString("#0.00");

        LogisticsOrderPayInfo orderPay = new LogisticsOrderPayInfo();
        orderPay.Order = order;
        orderPay.Customer = order.Customer;
        orderPay.PayType = PayType.AliPay;
        orderPay.OutTradeNo = outTradeNo;
        orderPay.Amount = order.ArrearsAmountFormula;
        orderPay.Status = PayStatus.NotPaid;
        session.Save(orderPay);
        session.Flush();

        // 订单名称
        string subject = $"订单{order.Code}_{orderPay.Id}";

        // 商品描述
        string itemBody = $"订单{order.Code}_{orderPay.Id}";

        bool isMobile = WebUtil.IsMobileRequest(Request);

        if (isMobile == false) {
            // 组装业务参数model
            AlipayTradePagePayModel payModel = new AlipayTradePagePayModel {
                Subject = subject,
                Body = itemBody,
                TotalAmount = totalAmount,
                OutTradeNo = outTradeNo,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(SiteConfig.SiteUrlBase + Url.Action("AliPayCallback", "WechatOfficialAccountAlipay"));
            // 设置异步通知接收地址
            request.SetNotifyUrl(SiteConfig.SiteUrlBase + Url.Action("AliPayNotify", "WechatOfficialAccountAlipay"));
            // 将业务model载入到request
            request.SetBizModel(payModel);

            var response = alipayService.SdkExecute(request);

            string alipayRequestUrl = alipayService.Options.Gatewayurl + "?" + response.Body;

            //跳转支付宝支付
            return Redirect(alipayRequestUrl);
        } else {
            // 组装业务参数model
            AlipayTradeWapPayModel payModel = new AlipayTradeWapPayModel {
                Subject = subject,
                Body = itemBody,
                TotalAmount = totalAmount,
                OutTradeNo = outTradeNo,
                ProductCode = "QUICK_WAP_WAY"
            };

            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(SiteConfig.SiteUrlBase + Url.Action("AliPayCallback", "WechatOfficialAccountAlipay")); // 这个是不是也要改成移动界面模式的
            // 设置异步通知接收地址
            request.SetNotifyUrl(SiteConfig.SiteUrlBase + Url.Action("AliPayNotify", "WechatOfficialAccountAlipay"));
            // 将业务model载入到request
            request.SetBizModel(payModel);

            var response = alipayService.SdkExecute(request);

            log.Warn("支付宝支付发起 end");

            string alipayRequestUrl = alipayService.Options.Gatewayurl + "?" + response.Body;

            //跳转支付宝支付
            return Redirect(alipayRequestUrl);
        }
    }

    private Tuple<bool, LogisticsOrderInfo> AlipayNotifyInternal() {
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
            return new Tuple<bool, LogisticsOrderInfo>(false, null);
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
                return new Tuple<bool, LogisticsOrderInfo>(false, null);
            }

            if (orderPay.Status == PayStatus.Paid) // 如果该笔支付已经标记为成功，就直接返回 success 就行
            {
                log.Warn("支付宝支付异步回调 end success 1");
                // return Content("success");
                return new Tuple<bool, LogisticsOrderInfo>(true, orderPay.Order);
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
            return new Tuple<bool, LogisticsOrderInfo>(true, order);
        }

        log.Error("支付宝支付异步回调 end fail_003");
        // return Content("fail");
        return new Tuple<bool, LogisticsOrderInfo>(false, null);
    }

    /// <summary>
    /// 支付异步回调通知 需配置域名 因为是支付宝主动post请求这个action 所以要通过域名访问或者公网ip
    /// </summary>
    [HttpPost]
    [Route("/pay/alipay/notify")]
    public IActionResult AliPayNotify() {
        try {
            bool success = false;
            LogisticsOrderInfo order = null;

            ITransaction transaction = null;
            try {
                transaction = session.BeginTransaction();

                Tuple<bool, LogisticsOrderInfo> result = AlipayNotifyInternal();
                success = result.Item1;
                order = result.Item2;

                transaction.Commit();
            }
            catch (Exception exp) {
                log.Error(exp);

                transaction?.Rollback();

                throw; // 重新抛出异常
            }

            if (success) {
                return Content("success");
            } else {
                return Content("fail");
            }
        }
        catch (Exception exp) {
            log.Error(exp);

            log.Error("支付宝支付异步回调 end fail_004");
            return Content("fail");
        }
    }

    private PayAliPayCallbackModel AliPayCallbackInternal() {
        log.Warn("支付宝支付同步回调 begin");

        Dictionary<string, string> sArray = new Dictionary<string, string>();

        ICollection<string> requestItem = Request.Query.Keys;
        foreach (var item in requestItem) {
            sArray.Add(item, Request.Query[item]);
        }

        try {
            log.Error("支付宝支付同步回调 数据: " + JsonConvert.SerializeObject(sArray));
        }
        catch (Exception exp) {
            log.Error(exp);
        }

        PayAliPayCallbackModel model = new PayAliPayCallbackModel();
        if (sArray.Count != 0) {
            bool flag = alipayService.RSACheckV1(sArray);
            if (flag) {
                // 标记订单为已支付
                string outTradeNo = sArray["out_trade_no"];
                var orderPay = session.Query<LogisticsOrderPayInfo>()
                    .Where(e => e.OutTradeNo == outTradeNo)
                    .Take(1)
                    .SingleOrDefault();
                if (orderPay == null) {
                    model.Success = false;
                    model.Message = "支付失败！";

                    log.Error("支付宝支付同步回调 end fail-001");
                    // return View("PayAliPayCallback", model);
                    return model;
                }

                if (orderPay.Status == PayStatus.Paid) // 如果该笔支付已经是已支付的，直接返回支付成功就行
                {
                    model.Success = true;
                    model.Order = orderPay.Order;
                    model.Message = "同步验证通过，支付成功！";

                    log.Warn("支付宝支付同步回调 end success 1");
                    // return View("PayAliPayCallback", model);
                    return model;
                }

                if (sArray.ContainsKey("total_amount") == false) {
                    throw new Exception($"total_amount 参数未提供，out_trade_no: {outTradeNo}");
                }

                decimal.TryParse(sArray["total_amount"], out decimal payAmount);

                orderPay.Status = PayStatus.Paid;
                orderPay.Amount = payAmount;
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

                model.Success = true;
                model.Order = orderPay.Order;
                model.Message = "同步验证通过，支付成功！";

                log.Warn("支付宝支付同步回调 end success 2");
                // return View("PayAliPayCallback", model);
                return model;
            } else {
                model.Success = false;
                model.Message = "支付失败！";

                log.Error("支付宝支付同步回调 end fail-002");
                // return View("PayAliPayCallback", model);
                return model;
            }
        }

        model.Success = false;
        model.Message = "";

        log.Error("支付宝支付同步回调 end fail-003");

        return model;
    }

    /// <summary>
    /// 支付同步回调
    /// </summary>
    [HttpGet]
    [Route("/pay/alipay/callback")]
    public IActionResult AliPayCallback() {
        try {
            PayAliPayCallbackModel model = new PayAliPayCallbackModel();

            ITransaction transaction = null;
            try {
                transaction = session.BeginTransaction();
                model = AliPayCallbackInternal();
                transaction.Commit();
            }
            catch (Exception exp) {
                log.Error(exp);

                transaction?.Rollback();
                throw; // 重新抛出异常                    
            }

            return View("PayAliPayCallback", model);
        }
        catch (Exception exp) {
            log.Error(exp);

            log.Error("支付宝支付同步回调 end fail-004");
            throw; // 加上，让事务回滚
        }
    }

    #endregion
}