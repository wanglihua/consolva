using NLog;
using shunda.Core.Service;
using shunda.Core.Util;

namespace shunda.Service.Pay;

[Service(ServiceType.Scoped)]
public class PaypalChargeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();


    public string Create(PaypalChargeCreateParam param) {
        ValidationUtil.Validate<PaypalChargeCreateParamValidator, PaypalChargeCreateParam>(param);

        return "";
    }

    public void Process(PaypalChargeProcessParam param) {
        ValidationUtil.Validate<PaypalChargeProcessParamValidator, PaypalChargeProcessParam>(param);
    }
}