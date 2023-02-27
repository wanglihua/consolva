using shunda.Core.Service;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class PayService {
    private static int outTradeNoSequence = 0;
    private static readonly object lockObjOutTradeNoSequence = new object();

    /// <summary>
    /// 生成支付外部订单号，商户网站订单系统中唯一的订单号 
    /// </summary>
    /// <returns></returns>
    public string GenerateOutTradeNo() {
        lock (lockObjOutTradeNoSequence) {
            outTradeNoSequence = (outTradeNoSequence + 1) % 100000;
        }

        return $"{DateTime.Now: yyyyMMddHHmmssfff}{outTradeNoSequence:00000}";
    }
}