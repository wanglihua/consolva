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
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Pay;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Pay.Controllers;

/// <summary>
/// 公众号支付宝支付 处理支付接口
/// </summary>
[PayArea]
public class WechatOfficialAccountAlipayController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly AlipayService alipayService;

    public WechatOfficialAccountAlipayController(
        ISession session,
        PayService payService,
        AlipayService alipayService
    ) {
        this.session = session;
        this.payService = payService;
        this.alipayService = alipayService;
    }
    
    [NonAction]
    private void CheckOrder(int orderId) {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException("指定支付的物流订单不存在！");
        }

        if (order.ArrearsAmountFormula <= 0) {
            throw new ValidationException("当前订单无欠费金额，无需支付！");
        }
    }

    #region 支付宝支付

    [HttpGet]
    [Route("/client/wechat/official/account/alipay/{code}")]
    public IActionResult CopyUrl(string code) {
        if (WebUtil.IsWechatRequest(Request)) {
            WechatOfficialAccountAlipayCopyUrlModel model = new WechatOfficialAccountAlipayCopyUrlModel {
                Url = Request.GetEncodedUrl()
            };
            return View("WechatOfficialAccountAlipayCopyUrl", model);
        }

        int orderId = GetOrderIdFromAlipayCode(code);
        return RedirectToAction("AliPayRequest", new { id = orderId });
    }

    [NonAction]
    private string GenerateAlipayCode(int orderId) {
        string alipayCodeSalt = "iepoch_alipay_secret_32lkjadfasd";

        string orderIdStr = orderId.ToString();
        string md5 = Md5Util.Encrypt(orderIdStr + alipayCodeSalt);
        return md5 + orderIdStr;
    }

    [NonAction]
    private int GetOrderIdFromAlipayCode(string code) {
        string errorMessage = "支付URL代码错误，请确认复制正确！";
        if (string.IsNullOrWhiteSpace(code) == true) {
            throw new ValidationException(errorMessage);
        }

        int md5Length = Md5Util.Md5Length;

        if (code.Length <= md5Length) {
            throw new ValidationException(errorMessage);
        }

        string orderIdStr = code.Substring(md5Length);
        if (int.TryParse(orderIdStr, out int orderId) == false) {
            throw new ValidationException(errorMessage);
        }

        string alipayCode = GenerateAlipayCode(orderId);
        if (alipayCode != code) {
            throw new ValidationException(errorMessage);
        }

        return orderId;
    }

    /// <summary>
    /// 发起支付请求
    /// </summary>
    /// <param name="id">物流订单Id</param>
    /// <returns></returns>
    [HttpGet]
    [Route("/pay/alipay/request")]
    public IActionResult AliPayRequest(int id) {
        if (WebUtil.IsWechatRequest(Request)) {
            return RedirectToAction("Alipay", new { code = GenerateAlipayCode(id) });
        }

        log.Warn("支付宝支付发起 begin");

        CheckOrder(id);

        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(id);

        // 外部订单号，商户网站订单系统中唯一的订单号
        string outTradeNo = payService.GenerateOutTradeNo();

        // 订单名称
        string subject = "物流订单" + order.Code + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");

        // 付款金额
        string totalAmount = order.ArrearsAmountFormula.ToString("#0.00");

        // 商品描述
        string itemBody = "头程物流" + order.FirstLogisticsMode?.Name;

        LogisticsOrderPayInfo logisticsOrderPay = new LogisticsOrderPayInfo();
        logisticsOrderPay.Order = order;
        logisticsOrderPay.Customer = order.Customer;
        logisticsOrderPay.PayType = PayType.AliPay;
        logisticsOrderPay.OutTradeNo = outTradeNo;
        logisticsOrderPay.Amount = order.ArrearsAmountFormula;
        logisticsOrderPay.Status = LogisticsOrderPayStatus.NotPaid;
        session.Save(logisticsOrderPay);
        session.Flush();

        bool isMobile = WebUtil.IsMobileRequest(Request);

        if (isMobile == false) {
            // 组装业务参数model
            AlipayTradePagePayModel payModel = new AlipayTradePagePayModel {
                Body = itemBody,
                Subject = subject,
                TotalAmount = totalAmount,
                OutTradeNo = outTradeNo,
                ProductCode = "FAST_INSTANT_TRADE_PAY"
            };

            AlipayTradePagePayRequest request = new AlipayTradePagePayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(SiteConfig.SiteRoot + Url.Action("AliPayCallback", "Pay"));
            // 设置异步通知接收地址
            request.SetNotifyUrl(SiteConfig.SiteRoot + Url.Action("AliPayNotify", "Pay"));
            // 将业务model载入到request
            request.SetBizModel(payModel);

            var response = alipayService.SdkExecute(request);

            string alipayRequestUrl = alipayService.Options.Gatewayurl + "?" + response.Body;

            //跳转支付宝支付
            return Redirect(alipayRequestUrl);
        } else {
            // 组装业务参数model
            AlipayTradeWapPayModel payModel = new AlipayTradeWapPayModel {
                Body = itemBody,
                Subject = subject,
                TotalAmount = totalAmount,
                OutTradeNo = outTradeNo,
                ProductCode = "QUICK_WAP_WAY"
            };

            AlipayTradeWapPayRequest request = new AlipayTradeWapPayRequest();
            // 设置同步回调地址
            request.SetReturnUrl(SiteConfig.SiteRoot + Url.Action("AliPayCallback", "Pay")); // 这个是不是也要改成移动界面模式的
            // 设置异步通知接收地址
            request.SetNotifyUrl(SiteConfig.SiteRoot + Url.Action("AliPayNotify", "Pay"));
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

            if (orderPay.Status == LogisticsOrderPayStatus.Paid) // 如果该笔支付已经标记为成功，就直接返回 success 就行
            {
                log.Warn("支付宝支付异步回调 end success 1");
                // return Content("success");
                return new Tuple<bool, LogisticsOrderInfo>(true, orderPay.Order);
            }

            decimal.TryParse(sArray["total_amount"], out decimal payAmount);

            orderPay.Amount = payAmount;
            orderPay.Status = LogisticsOrderPayStatus.Paid;
            session.Update(orderPay);
            session.Flush();

            LogisticsOrderInfo order = orderPay.Order;

            // 设置支付类型，合计标记支付状态
            logisticsOrderFeeService.SetPayTypeSumFeeUpdatePayStatus(order, PayType.AliPay);

            // 记录日志
            logisticsOrderActionService.NewAction(order, LogisticsOrderActionType.Pay, $"金额：{payAmount.ToString("#0.00")}，支付宝单号：{orderPay.OutTradeNo}");

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
    [NoHibernateAutoTransaction]
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

            if (success) // 前面成功了，才提交打包
            {
                try {
                    transaction = session.BeginTransaction();

                    // 支付成功后提交打包
                    if (order.Cate == LogisticsOrderCate.Sea
                        && order.OrderStatus == LogisticsOrderStatus.CnAllCheckin
                        && order.FeeMethod == LogisticsOrderFeeMethod.Express
                        && order.ArrearsAmountNative <= 0) {
                        logisticsOrderService.SubmitPackWhenPaid(order?.Id ?? 0);
                    }

                    transaction.Commit();
                }
                catch (Exception exp) {
                    transaction?.Rollback();

                    log.Error(exp);

                    // 这里不重新抛出异常                        
                }
            }

            // return Content("success");
            // return Content("fail");   
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

                if (orderPay.Status == LogisticsOrderPayStatus.Paid) // 如果该笔支付已经是已支付的，直接返回支付成功就行
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

                orderPay.Status = LogisticsOrderPayStatus.Paid;
                orderPay.Amount = payAmount;
                session.Update(orderPay);
                session.Flush();

                LogisticsOrderInfo order = orderPay.Order;

                // 设置支付类型，合计标记支付状态
                logisticsOrderFeeService.SetPayTypeSumFeeUpdatePayStatus(order, PayType.AliPay);

                // 记录日志
                logisticsOrderActionService.NewAction(order, LogisticsOrderActionType.Pay, $"金额：{payAmount.ToString("#0.00")}，支付宝单号：{orderPay.OutTradeNo}");

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
    [NoHibernateAutoTransaction]
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

            if (model.Success) {
                try {
                    transaction = session.BeginTransaction();

                    LogisticsOrderInfo order = model.Order;

                    // 支付成功后提交打包
                    if (order.Cate == LogisticsOrderCate.Sea
                        && order.OrderStatus == LogisticsOrderStatus.CnAllCheckin
                        && order.FeeMethod == LogisticsOrderFeeMethod.Express
                        && order.ArrearsAmountNative <= 0) {
                        logisticsOrderService.SubmitPackWhenPaid(order?.Id ?? 0);
                    }

                    transaction.Commit();
                }
                catch (Exception exp) {
                    transaction?.Rollback();

                    log.Error(exp);

                    // 这里不重新抛出异常
                }
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