using shunda.Core;
using shunda.Core.Service;
using shunda.Core.WechatPay;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class WechatPayChargeService {
    private readonly ISession session;

    public WechatPayChargeService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 创建充值支付
    /// </summary>
    public WechatPayData Create(int customerId, decimal amount, string notifyUrl) {
        CustomerInfo customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在，请确认客户已登录！");
        }

        return new WechatPayData();
    }

    /// <summary>
    /// 充值支付回调
    /// </summary>
    public void Notify() {
    }
}

/*

        private WxPayData InitWxPay(int customerId, LogisticsOrderCate orderCate, decimal amount, string notifyUrl, WxPayType wxPayType)
        {
            CustomerInfo customer = session.Get<CustomerInfo>(customerId);
            if (customer == null)
            {
                throw new ValidationException("未能取得登录会员ID 或 取得的登录会员ID，不存在， 请确认会员是否已登录！");
            }

            // 外部订单号，商户网站订单系统中唯一的订单号
            string outTradeNo = payService.GenerateOutTradeNo(PayType.Wechat);


            // 付款金额，单位是分
            int totalAmount = (int)(amount * 100);


            CustomerChargeInfo customerCharge = new CustomerChargeInfo();
            customerCharge.OrderCate = orderCate;
            customerCharge.ChargeType = CustomerChargeType.Wechat;
            customerCharge.Customer = customer;
            customerCharge.OutTradeNo = outTradeNo;
            customerCharge.Amount = amount;
            customerCharge.Status = CustomerChargeStatus.New;

            session.Save(customerCharge);
            session.Flush();

            // 订单名称
            string subject = "会员充值_" + customerId + "_" + customerCharge.Id;

            // 商品描述
            string itemBody = subject + "_" + amount.ToString("0.00");

            WxPayData data = new WxPayData();
            data.SetValue("body", subject); //商品描述
            data.SetValue("attach", itemBody); //附加数据
            data.SetValue("out_trade_no", outTradeNo);
            data.SetValue("total_fee", totalAmount); //总金额
            data.SetValue("time_start", DateTime.Now.ToString("yyyyMMddHHmmss")); //交易起始时间
            data.SetValue("time_expire", DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss")); //交易结束时间
            data.SetValue("goods_tag", "customer charge"); //商品标记

            if (wxPayType == WxPayType.Native)
            {
                data.SetValue("trade_type", "NATIVE"); //交易类型
                data.SetValue("product_id", $"会员充值_{customerId}_{customerCharge.Id}");
            }

            if (wxPayType == WxPayType.JsApi)
            {
                data.SetValue("trade_type", "JSAPI");

                string openId = weiXinSessionService.GetOpenId();
                data.SetValue("openid", openId);
            }

            data.SetValue("notify_url", SiteConfig.SiteWxPayRoot + notifyUrl);

            return data;
        }
    */