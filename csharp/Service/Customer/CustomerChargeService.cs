using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Service.Fee;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerChargeService
{
    private ISession session;

    private ExchangeRateService exchangeRateService;
    private CustomerAccountService customerAccountService;

    public CustomerChargeService(ISession session,
        ExchangeRateService exchangeRateService,
        CustomerAccountService customerAccountService)
    {
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
    public void CompleteCharge(CustomerChargeInfo customerCharge, decimal amount, CustomerChargeType chargeType)
    {
        if (customerCharge == null)
        {
            throw new ValidationException("指定的充值记录为空！");
        }

        string chargeDescription = "";
        if (chargeType == CustomerChargeType.AliPay)
        {
            chargeDescription = "支付宝充值" + amount.ToString("0.00") + "元";
        }
        else if (chargeType == CustomerChargeType.Wechat)
        {
            chargeDescription = "微信充值" + amount.ToString("0.00") + "元";
        }
        else if (chargeType == CustomerChargeType.PayPal)
        {
            decimal audExchangeRate = exchangeRateService.GetAudFinalRate();
            if (audExchangeRate <= 0)
            {
                throw new ValidationException("汇率值不合法!");
            }

            string totalAmountAud = (amount / audExchangeRate).ToString("#0.00");
            chargeDescription = $"PayPal 人民币支付：{amount.ToString("#0.00")}, 折算澳币: {totalAmountAud}";
        }
        else if (chargeType == CustomerChargeType.Manual)
        {
            chargeDescription = "手工充值" + amount.ToString("0.00") + "元";
        }

        // 标记订单为已支付
        customerCharge.Amount = amount;
        customerCharge.Status = CustomerChargeStatus.Complete;
        session.Update(customerCharge);
        session.Flush();

        CustomerInfo? customer = customerCharge.Customer;

        // 充值和余额日志
        customerAccountService.Change(CustomerAccountChangeType.Charge, customer.Id, amount, chargeDescription);

        /* todo 这个充值奖励暂时禁用吧，舜达的不一定会是这样的优惠

        // 计算充值奖励
        {
            // 判断是否是超级VIP

            bool isSuperVip = false;
            int customerId = customer?.Id ?? 0;
            CustomerSuperVipDiscountInfo? customerSuperVipDiscount = session
                .Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == customerId)
                .Take(1)
                .SingleOrDefault();

            if (customerSuperVipDiscount != null)
            {
                isSuperVip = true;
            }

            // 判断VIP级别

            if (isSuperVip == false)
            {
                decimal customerTotalScore = customer?.TotalScore ?? 0;
                int levelCount = session
                    .Query<CustomerVipDiscountInfo>()
                    .Where(e => e.Score <= customerTotalScore)
                    .Count();

                if (levelCount < 10) // VIP 10 级以内才计算奖励
                {
                    decimal award = 0m;
                    if (amount < 10000)
                    {
                        award = 0m;
                    }
                    else if (amount >= 10000 && amount < 15000)
                    {
                        award = 500m;
                    }
                    else if (amount >= 15000 && amount < 20000)
                    {
                        award = 900m;
                    }
                    else if (amount >= 20000 && amount < 25000)
                    {
                        award = 1400m;
                    }
                    else if (amount >= 25000 && amount < 30000)
                    {
                        award = 2000m;
                    }
                    else if (amount >= 30000 && amount < 40000)
                    {
                        award = 3000m;
                    }
                    else if (amount >= 40000 && amount < 50000)
                    {
                        award = 6000m;
                    }
                    // else if (amount >= 50000 && amount < 60000)
                    else if (amount >= 50000)
                    {
                        award = 10000m;
                    }

                    if (award > 0)
                    {
                        // 设置奖励金额
                        customerCharge.Award = award;
                        session.Update(customerCharge);
                        session.Flush();

                        // 充值和日志
                        string awardDescription = $"充值奖惩，充值{amount.ToString("0.00")}，奖励{award.ToString("0.00")}";
                        customerAccountService.Change(CustomerAccountChangeType.ChargeAward, customer?.Id ?? 0, award, awardDescription);
                    }
                }
            }
        }
        */
    }
}