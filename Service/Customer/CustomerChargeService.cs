using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Service.Fee;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerChargeService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly ExchangeRateService exchangeRateService;
    private readonly CustomerAccountService customerAccountService;

    public CustomerChargeService(ISession session,
        ExchangeRateService exchangeRateService,
        CustomerAccountService customerAccountService
    ) {
        this.session = session;
        this.exchangeRateService = exchangeRateService;
        this.customerAccountService = customerAccountService;
    }

    /// <summary>
    /// 完成充值
    /// </summary>
    /// <param name="customerCharge">确保已经是数据库中存在，但状态还不是完成状态</param>
    /// <param name="amount">RMB, 包括PayPal方式下</param>
    /// <param name="chargeType">充值方式</param>
    public void CompleteCharge(CustomerChargeInfo customerCharge, decimal amount, CustomerChargeType chargeType) {
        if (customerCharge == null) {
            throw new ValidationException("指定的充值记录为空！");
        }

        if (customerCharge.Status == CustomerChargeStatus.Complete) {
            log.Error($"会员充值#{customerCharge.Id}之前已完成！");
            return;
        }

        string chargeDescription = "";
        if (chargeType == CustomerChargeType.Alipay) {
            chargeDescription = $"支付宝充值{amount:#0.00}元";
        } else if (chargeType == CustomerChargeType.Wechat) {
            chargeDescription = $"微信充值{amount:#0.00}元";
        } else if (chargeType == CustomerChargeType.PayPal) {
            chargeDescription = $"PayPal充值{amount:#0.00}元";
        } else if (chargeType == CustomerChargeType.Manual) {
            chargeDescription = $"手工充值{amount:#0.00}元";
        }

        // 标记订单为已支付
        customerCharge.Amount = amount;
        customerCharge.Status = CustomerChargeStatus.Complete;
        session.Update(customerCharge);
        session.Flush();

        CustomerInfo customer = customerCharge.Customer;

        // 充值和余额日志
        customerAccountService.Change(CustomerAccountChangeType.Charge, customer.Id, amount, chargeDescription);

        // 计算充值奖励
        {
            var chargeAwardSetting = session.Query<CustomerChargeAwardSettingInfo>()
                .Where(e => e.AmountBegin <= amount && amount < e.AmountEnd)
                .OrderBy(e => e.AmountBegin)
                .Take(1)
                .SingleOrDefault();

            decimal award = chargeAwardSetting?.Award ?? 0;

            if (award > 0) {
                // 设置奖励金额
                customerCharge.Award = award;
                session.Update(customerCharge);
                session.Flush();

                // 充值和日志
                string awardDescription = $"充值奖惩，充值{amount:#0.00}，奖励{award: #0.00}";
                customerAccountService.Change(CustomerAccountChangeType.ChargeAward, customer.Id, award, awardDescription);
            }
        }
    }
}