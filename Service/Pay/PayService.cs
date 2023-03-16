using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;

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

        return $"{DateTime.Now:yyyyMMddHHmmssfff}{outTradeNoSequence:00000}";
    }

    /// <summary>
    /// 将ID放在支付宝支付URL种是做加密，防止被篡改
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public string GenereatePayCodeInUrl(int id) {
        string payCodeSalt = "shunda_pay_secret_32lkjadfasd45lpo";

        string idStr = id.ToString();
        string md5 = Md5Util.Encrypt(idStr + payCodeSalt);
        return md5 + idStr;
    }

    /// <summary>
    /// 从防篡改加密的CODE中获取ID
    /// </summary>
    /// <param name="payCode"></param>
    /// <returns></returns>
    /// <exception cref="ValidationException"></exception>
    public int GetIdFromPayCodeInUrl(string payCode) {
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

        string alipayCode = GenereatePayCodeInUrl(id);
        if (alipayCode != payCode) {
            throw new ValidationException(errorMessage);
        }

        return id;
    }    
}