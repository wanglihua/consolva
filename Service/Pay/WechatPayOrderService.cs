using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Core.WechatPay;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class WechatPayOrderService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;

    public WechatPayOrderService(
        ISession session,
        PayService payService
    ) {
        this.session = session;
        this.payService = payService;
    }

    /// <summary>
    /// 创建订单支付
    /// 在数据库创建本地支付记录
    /// 在微信端创建 unified order
    /// </summary>
    /// <param name="orderId"></param>
    /// <param name="notifyUrl">回调URL，没有域名部分，比如 /client/wechat/app/charge/wechat/pay/notify</param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public WechatPayCreateResultData Create(int orderId, string notifyUrl) {
        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        NHibernateUtil.Initialize(order.Customer);

        string outTradeNo = payService.GenerateOutTradeNo();

        LogisticsOrderPayInfo logisticsOrderPay = new LogisticsOrderPayInfo {
            Order = order,
            Customer = order.Customer,
            PayType = PayType.Wechat,
            OutTradeNo = outTradeNo,
            Amount = order.ArrearsAmountFormula,
            Status = PayStatus.NotPaid
        };

        session.Save(logisticsOrderPay);
        session.Flush();

        // 订单名称
        string subject = "订单" + order.Code + "_" + DateTime.Now.ToString("yyyyMMddHHmmss");

        // 付款金额，单位是分
        int totalAmount = (int)(order.ArrearsAmountFormula * 100);

        // 商品描述
        string itemBody = subject + "物流" + order.FirstMode.Name;

        string appId = WechatAppConfig.AppId;

        WechatPayData initData = new WechatPayData();
        initData.SetValue("appid", appId);
        initData.SetValue("body", subject); //商品描述
        initData.SetValue("attach", itemBody); //附加数据
        initData.SetValue("out_trade_no", outTradeNo);
        initData.SetValue("total_fee", totalAmount); //总金额
        initData.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss")); //交易起始时间
        initData.SetValue("time_expire", DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss")); //交易结束时间
        initData.SetValue("goods_tag", "customer charge"); //商品标记
        initData.SetValue("trade_type", "JSAPI");

        string openId = order.Customer.WechatAppOpenId;
        initData.SetValue("openid", openId);

        initData.SetValue("notify_url", SiteConfig.SiteRoot + notifyUrl);
        WechatPayData result = WechatPayApi.UnifiedOrder(initData);
        if (!result.IsSet("appid") || !result.IsSet("prepay_id") || string.IsNullOrWhiteSpace(result.GetValue("prepay_id")?.ToString())) {
            throw new WechatPayException("wechat pay UnifiedOrder response error!");
        }

        string timeStamp = WechatPayApi.GenerateTimeStamp();
        string nonceStr = WechatPayApi.GenerateNonceStr();
        string package = "prepay_id=" + result.GetValue("prepay_id");
        string signType = WechatPayData.SIGN_TYPE_HMAC_SHA256;

        WechatPayData unifiedData = new WechatPayData();
        unifiedData.SetValue("appId", appId);
        unifiedData.SetValue("timeStamp", timeStamp);
        unifiedData.SetValue("nonceStr", nonceStr);
        unifiedData.SetValue("package", package);
        unifiedData.SetValue("signType", signType);

        string paySign = unifiedData.MakeSign(WechatPayData.SIGN_TYPE_HMAC_SHA256);

        return new WechatPayCreateResultData {
            TimeStamp = timeStamp,
            NonceStr = nonceStr,
            Package = package,
            SignType = signType,
            PaySign = paySign,
        };
    }

    /// <summary>
    /// 订单支付回调
    /// </summary>
    public void Notify(WechatPayData requestData) {
        
    }
}

/*
 

    WxPayData unifiedOrderResult = null;
    LogisticsOrderInfo order = null;

    ITransaction transaction = null;
    try
    {
        transaction = session.BeginTransaction();

        WxPayData notifyData = GetNotifyData(request);

        //检查openid和product_id是否返回
        // if (!notifyData.IsSet("openid") || !notifyData.IsSet("product_id"))
        if (!notifyData.IsSet("openid"))
        {
            throw new WxPayException("回调数据异常，openid 未设置！ ");
        }

        string outTradeNo = notifyData.GetValue("out_trade_no").ToString();
        IList<LogisticsOrderPayInfo> orderPayList = session.Query<LogisticsOrderPayInfo>().Where(e => e.OutTradeNo == outTradeNo).ToList();
        if (orderPayList.Count != 1)
        {
            throw new WxPayException($"out_trade_no 本地系统中不存在或有多个。out_trade_no：{outTradeNo}");
        }

        LogisticsOrderPayInfo orderPay = orderPayList[0];
        order = orderPay.Order;

        // 实际支付金额
        decimal payAmount = ((decimal)int.Parse(notifyData.GetValue("total_fee").ToString())) / 100m;

        //调统一下单接口，获得下单结果
        string openid = notifyData.GetValue("openid").ToString();
        // string productId = notifyData.GetValue("product_id").ToString();
        unifiedOrderResult = UnifiedOrder(openid, order.Id, payAmount);

        //若下单失败，则立即返回结果给微信支付后台
        if (!unifiedOrderResult.IsSet("appid") || !unifiedOrderResult.IsSet("mch_id") || !unifiedOrderResult.IsSet("prepay_id"))
        {
            throw new WxPayException("统一下单失败, appid 或 mch_id 或 prepay_id 未设置！");
        }

        // 如果支付记录是【还没】支付的，就先做本地费数据处理；
        // 如果支付记录是【已经】支付的，就直接返回success
        if (orderPay.Status == LogisticsOrderPayStatus.NotPaid)
        {
            // 标记订单为已支付
            orderPay.Amount = payAmount;
            orderPay.Status = LogisticsOrderPayStatus.Paid;
            session.Update(orderPay);
            session.Flush();


            // 设置支付类型，合计标记支付状态
            logisticsOrderFeeService.SetPayTypeSumFeeUpdatePayStatus(order, PayType.Wechat);

            logisticsOrderActionService.NewAction(order, LogisticsOrderActionType.Pay, $"金额：{payAmount.ToString("#0.00")}, 微信单号：{orderPay.OutTradeNo}");

            // 会员积分
            customerScoreService.AddScore(orderPay.Id);
        }

        transaction.Commit();
    }
    catch (Exception exp)
    {
        log.Error(exp);

        transaction?.Rollback();

        throw; // 重新抛出异常
    }

    // 支付成功后提交打包
    try
    {
        transaction = session.BeginTransaction();

        if (order.Cate == LogisticsOrderCate.Sea
            && order.OrderStatus == LogisticsOrderStatus.CnAllCheckin
            && order.FeeMethod == LogisticsOrderFeeMethod.Express
            && order.ArrearsAmountNative <= 0)
        {
            logisticsOrderService.SubmitPackWhenPaid(order?.Id ?? 0);
        }

        transaction.Commit();
    }
    catch (Exception exp)
    {
        transaction?.Rollback();

        log.Error(exp);

        // 这里不重新抛出异常
    }

*/