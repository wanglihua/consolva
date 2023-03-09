using NHibernate;
using NLog;
using Senparc.CO2NET.Cache;
using shunda.Core;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Core.WechatPay;
using shunda.Domain.Express;
using shunda.Domain.Fee;
using shunda.Service.Express;
using shunda.Service.Logistics;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class WechatPayExpressReturnService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly PayService payService;
    private readonly ExpressReturnFeeRateService expressReturnFeeRateService;
    private readonly ExpressLogService expressLogService;

    public WechatPayExpressReturnService(
        ISession session,
        PayService payService,
        ExpressReturnFeeRateService expressReturnFeeRateService,
        ExpressLogService expressLogService
    ) {
        this.session = session;
        this.payService = payService;
        this.expressReturnFeeRateService = expressReturnFeeRateService;
        this.expressLogService = expressLogService;
    }

    /// <summary>
    /// 创建订单支付，调用之前确认快递单是当前登录用户的
    /// 在数据库创建本地支付记录
    /// 在微信端创建 unified order
    /// </summary>
    /// <param name="expressId"></param>
    /// <param name="notifyUrl">回调URL，没有域名部分，比如 /client/wechat/app/charge/wechat/pay/notify</param>
    /// <returns></returns>
    public WechatPayCreateResultData Create(int expressId, string notifyUrl) {
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单#{expressId}不存在！");
        }

        if (ExpressStatus.IsPicked(express.Status) == false) {
            throw new ValidationException($"快递单号{express.TrackingNo}未认领不能退运付款！");
        }

        NHibernateUtil.Initialize(express.Customer);

        // 这个判断一般不可能成立
        if (express.Customer == null) {
            throw new ValidationException($"快递单号{express.TrackingNo}认领客户为空，不能退运付款！");
        }

        string outTradeNo = payService.GenerateOutTradeNo();
        decimal feePerExpress = expressReturnFeeRateService.GetFeeRate();

        var expressReturnPay = new ExpressReturnPayInfo {
            Express = express,
            Customer = express.Customer!,
            PayType = PayType.Wechat,
            OutTradeNo = outTradeNo,
            Amount = feePerExpress,
            Status = PayStatus.NotPaid,
        };

        session.Save(expressReturnPay);
        session.Flush();

        // 订单名称
        string subject = "快递单退运" + expressId + "_" + expressReturnPay.Id;

        // 商品描述
        string itemBody = "快递单退运" + expressId + "_" + expressReturnPay.Id;

        // 付款金额，单位是分
        int totalAmount = (int)(expressReturnPay.Amount * 100);

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

        string openId = express.Customer!.WechatAppOpenId;
        initData.SetValue("openid", openId);

        initData.SetValue("notify_url", SiteConfig.SiteUrlBase + notifyUrl);

        WechatPayData result = WechatPayApi.UnifiedOrder(initData);

        if (!result.IsSet("appid") || !result.IsSet("prepay_id") || string.IsNullOrWhiteSpace(result.GetValue("prepay_id")?.ToString())) {
            throw new WechatPayException("wechat pay UnifiedOrder response error!");
        }

        string timeStamp = WechatPayApi.GenerateTimeStamp();
        string nonceStr = WechatPayApi.GenerateNonceStr();
        string package = "prepay_id=" + result.GetValue("prepay_id");
        string signType = WechatPayData.SIGN_TYPE_HMAC_SHA256;

        // 利用 WechatPayData 做签名
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
        string outTradeNo = requestData.GetValue("out_trade_no")?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(outTradeNo)) {
            throw new WechatPayException("回调数据异常，out_trade_no 未设置！");
        }

        var expressReturnPay = session.Query<ExpressReturnPayInfo>()
            .Where(e => e.OutTradeNo == outTradeNo)
            .Take(1)
            .SingleOrDefault();
        if (expressReturnPay == null) {
            throw new WechatPayException($"out_trade_no 本地系统快递单号退运支付记录中不存在。out_trade_no：{outTradeNo}");
        }

        NHibernateUtil.Initialize(expressReturnPay.Express);

        // 实际支付金额
        string totalFeeValue = requestData.GetValue("total_fee")?.ToString() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(totalFeeValue)) {
            throw new WechatPayException("回调数据异常，total_fee 未设置！");
        }

        if (int.TryParse(totalFeeValue, out int payAmountInt) == false) {
            throw new WechatPayException("回调数据异常，total_fee 格式错误！");
        }

        // 分转成元
        decimal payAmount = ((decimal)payAmountInt) / 100m;

        if (expressReturnPay.Status == PayStatus.NotPaid) {
            // 标记快递单退运种支付为已支付
            expressReturnPay.Amount = payAmount;
            expressReturnPay.Status = PayStatus.Paid;
            session.Update(expressReturnPay);
            session.Flush();

            NHibernateUtil.Initialize(expressReturnPay.Express);

            var express = expressReturnPay.Express;
            int expressId = express.Id;

            // 修改快递单状态
            if (express.Status == ExpressStatus.Checkin) {
                express.Status = ExpressStatus.WaitReturn;
                session.Update(express);

                // 退运记录
                var expressReturn = new ExpressReturnInfo {
                    Express = express,
                    TrackingNo = express.TrackingNo,
                    CheckinTrackingNo = express.CheckinTrackingNo,
                    ExpressCompany = express.ExpressCompany,
                    GoodsType = express.GoodsType,
                    AddTime = express.AddTime,
                    AddUser = express.AddUser,
                    Warehouse = express.Warehouse,
                    Customer = express.Customer!,
                    Status = ExpressStatus.WaitReturn,
                };

                session.Save(expressReturn);

                session.Flush();
            }

            // 快递单日志
            expressLogService.Log(expressId, ExpressLogType.ReturnPay, string.Empty);
        }

        session.Flush();
    }
}