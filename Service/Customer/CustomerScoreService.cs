using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Message.CustomerMessage;
using ISession = NHibernate.ISession;
using NHibernate.Linq;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerScoreService {
    private readonly ISession session;

    // private readonly CustomerService customerService;
    private readonly CustomerMessageScoreChangeService customerMessageScoreChangeService;
    private readonly CustomerMessageMoneyEarnedService customerMessageMoneyEarnedService;
    private readonly CustomerVipDiscountService customerVipDiscountService;

    private readonly CustomerAccountService customerAccountService;
    // private readonly CustomerMessageClientUrlService customerMessageClientUrlService;

    public CustomerScoreService(ISession session,
            // CustomerService customerService,
            CustomerMessageScoreChangeService customerMessageScoreChangeService,
            CustomerMessageMoneyEarnedService customerMessageMoneyEarnedService,
            CustomerVipDiscountService customerVipDiscountService,
            CustomerAccountService customerAccountService)
        // CustomerMessageClientUrlService customerMessageClientUrlService)
    {
        this.session = session;
        // this.customerService = customerService;
        this.customerMessageScoreChangeService = customerMessageScoreChangeService;
        this.customerMessageMoneyEarnedService = customerMessageMoneyEarnedService;
        this.customerVipDiscountService = customerVipDiscountService;
        this.customerAccountService = customerAccountService;
        // this.customerMessageClientUrlService = customerMessageClientUrlService;
    }

    /// <summary>
    /// 添加支付产生积分
    /// 同时添加  集运消费积分 和 会员推广积分
    /// </summary>
    /// <param name="orderPayId">对应的集运订单支付Id</param>
    public void AddScore(int orderPayId) {
        var orderPay = session.Get<LogisticsOrderPayInfo>(orderPayId);
        if (orderPay == null) {
            throw new ValidationException("指定的集运订单支付不存在！");
        }

        NHibernateUtil.Initialize(orderPay.Order);

        var logisticsConsumeRule = session.Query<CustomerScoreRuleInfo>()
            .Where(e => e.Type == CustomerScoreType.LogisticsConsume)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (logisticsConsumeRule == null) {
            // throw new Exception("集运消费积分规则未设置！");
            return;
        }

        if (logisticsConsumeRule.Amount <= 0) {
            // throw new Exception("集运消费积分规则，集运消费金额设置小于等于零，不符合要求！");
            return;
        }

        if (logisticsConsumeRule.Score <= 0) {
            // throw new Exception("集运消费积分规则，产生积分设置小于等于零，不符合要求！");
            return;
        }

        LogisticsOrderInfo? order = session.Get<LogisticsOrderInfo>(orderPay.Order?.Id ?? 0);
        if (order == null) {
            throw new ValidationException("订单支付对应订单为空，数据非法！");
        }

        CustomerInfo? orderCustomer = session.Get<CustomerInfo>(order.Customer?.Id ?? 0); // 取订单的客户
        if (orderCustomer == null) {
            throw new ValidationException("订单支付对应订单的会员为空，数据非法！");
        }

        bool isValidScoreLogisticsConsume = true; // 是不是集运消费有效积分


        decimal amount = orderPay.Amount;
        if (amount < 0) // 这里假设可以为0，也许也有因为优惠最终不用付钱的
        {
            throw new ValidationException("支付金额不可能小于等于0，数据非法！");
        }

        IList<CustomerVipDiscountInfo> customerVipDiscountList = customerVipDiscountService.GetCustomerVipDiscountList(); // 这个数据总共才 20 来条

        // IList<CustomerMessageVipUpgradeParam> customerMessageVipUpgradeParamList = new List<CustomerMessageVipUpgradeParam>();

        // 集运消费积分

        #region 集运消费积分

        decimal logisticsConsumeScore = (amount / logisticsConsumeRule.Amount) * logisticsConsumeRule.Score;

        decimal orderCustomerOldTotalScore = orderCustomer.TotalScore;

        orderCustomer.TotalScore = orderCustomer.TotalScore + logisticsConsumeScore;

        decimal validScoreBefore = orderCustomer.ValidScore;
        if (isValidScoreLogisticsConsume) {
            orderCustomer.ValidScore = orderCustomer.ValidScore + logisticsConsumeScore;
        }

        decimal validScoreAfter = orderCustomer.ValidScore;

        session.Update(orderCustomer);
        session.Flush();

        decimal orderCustomerNewTotalScore = orderCustomer.TotalScore;

        // todo vip 升级的逻辑
        if (customerVipDiscountService.ShouldVipUpgrade(orderCustomerOldTotalScore, orderCustomerNewTotalScore, customerVipDiscountList)) {
            // var customerVipDiscount = customerVipDiscountService.GetDiscount(orderCustomerNewTotalScore, customerVipDiscountList);
            // var customerVipDiscountNextLevel = customerVipDiscountService.GetNextLevelDiscount(orderCustomerNewTotalScore, customerVipDiscountList);

            // CustomerMessageVipUpgradeParam customerMessageVipUpgradeParam = new CustomerMessageVipUpgradeParam();
            // customerMessageVipUpgradeParam.Customer = orderCustomer;
            // customerMessageVipUpgradeParam.NewVipLevel = customerVipDiscount?.LevelName;
            // customerMessageVipUpgradeParam.LogisticsFirstDiscount = $"{customerVipDiscount?.LogisticsFirstDiscount.ToString("#0.00")}%";
            // customerMessageVipUpgradeParam.LogisticsSecondDiscount = $"{customerVipDiscount?.LogisticsSecondDiscount.ToString("#0.00")}%";
            // customerMessageVipUpgradeParam.MallDiscount = $"{customerVipDiscount?.MallDiscount.ToString("#0.00")}%";
            // customerMessageVipUpgradeParam.NextVipLevel = customerVipDiscountNextLevel?.LevelName;
            //
            // customerMessageVipUpgradeParamList.Add(customerMessageVipUpgradeParam);

            // 发送可能得VIP升级信息 customerMessage
        }

        CustomerScoreLogInfo scoreLog = new CustomerScoreLogInfo {
            Customer = orderCustomer,
            ScoreType = CustomerScoreType.LogisticsConsume,
            Score = logisticsConsumeScore,
            Amount = amount,
            IsValidScore = isValidScoreLogisticsConsume,
            LogisticsOrderPay = orderPay,
            LogisticsOrder = order,
            LogisticsOrderCode = order.Code,
            LogisticsOrderCustomer = orderCustomer, // 订单的会员，这里跟积分会员 是 同一个
            Description = ""
        };

        session.Save(scoreLog);
        session.Flush();

        // 有效积分时才发送
        if (isValidScoreLogisticsConsume) {
            customerMessageScoreChangeService.SendMessage(orderCustomer, $"集运消费支付{amount:#0.00}元", logisticsConsumeScore, validScoreBefore, validScoreAfter);
        }

        #endregion

        // 会员推广积分

        #region 会员推广积分

        CustomerScoreRuleInfo? promotionRule = session.Query<CustomerScoreRuleInfo>()
            .Where(e => e.Type == CustomerScoreType.Promotion)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (promotionRule == null) {
            throw new Exception("会员推广积分规则未设置！");
        }

        if (promotionRule.Amount <= 0) {
            throw new Exception("会员推广积分规则，下线消费金额设置小于等于零，不符合要求！");
        }

        if (promotionRule.Score <= 0) {
            throw new Exception("会员推广积分规则，集运积分设置小于等于零，不符合要求！");
        }

        decimal promotionScore = (amount / promotionRule.Amount) * promotionRule.Score;

        var customerPromotion = session.Query<CustomerPromotionInfo>()
            .Fetch(e => e.Parent)
            .Where(e => e.Child!.Id == orderCustomer.Id)
            .Take(1)
            .SingleOrDefault();

        if (customerPromotion != null) {
            // 直接上家
            CustomerInfo promotionParentCustomer = customerPromotion.Parent;

            // 直接上家是不是超级会员
            // int promotionParentCustomerId = promotionParentCustomer.Id;
            // var superVipDiscount = session.Query<CustomerSuperVipDiscountInfo>()
            //     .Where(e => e.Customer.Id == promotionParentCustomerId)
            //     .Take(1)
            //     .SingleOrDefault();

            // 当前总是 有效积分
            bool isValidScorePromotion = true; // 是不是集运消费有效积分

            // 是超级VIP
            // if (superVipDiscount != null) {
            //     isValidScorePromotion = false;
            // }

            decimal promotionParentCustomerOldTotalScore = promotionParentCustomer.TotalScore;
            promotionParentCustomer.TotalScore = promotionParentCustomerOldTotalScore + promotionScore;

            if (isValidScorePromotion) {
                promotionParentCustomer.ValidScore = promotionParentCustomer.ValidScore + promotionScore;
            }

            session.Update(promotionParentCustomer);
            session.Flush();

            // todo vip 升级的逻辑
            // decimal directParentCustomerNewTotalScore = promotionParentCustomer.TotalScore ?? 0m;
            //
            // if (customerVipDiscountService.IsVipUpgrade(promotionParentCustomerOldTotalScore, directParentCustomerNewTotalScore, customerVipDiscountList)) {
            //     CustomerVipDiscountInfo customerVipDiscount = customerVipDiscountService.GetDiscount(directParentCustomerNewTotalScore, customerVipDiscountList);
            //     CustomerVipDiscountInfo customerVipDiscountNextLevel = customerVipDiscountService.GetNextLevelDiscount(directParentCustomerNewTotalScore, customerVipDiscountList);
            //
            //     CustomerMessageVipUpgradeParam customerMessageVipUpgradeParam = new CustomerMessageVipUpgradeParam();
            //     customerMessageVipUpgradeParam.Customer = promotionParentCustomer;
            //     customerMessageVipUpgradeParam.NewVipLevel = customerVipDiscount?.LevelName;
            //     customerMessageVipUpgradeParam.LogisticsFirstDiscount = $"{customerVipDiscount?.LogisticsFirstDiscount.ToString("#0.00")}%";
            //     customerMessageVipUpgradeParam.LogisticsSecondDiscount = $"{customerVipDiscount?.LogisticsSecondDiscount.ToString("#0.00")}%";
            //     customerMessageVipUpgradeParam.MallDiscount = $"{customerVipDiscount?.MallDiscount.ToString("#0.00")}%";
            //     customerMessageVipUpgradeParam.NextVipLevel = customerVipDiscountNextLevel?.LevelName;
            //
            //     customerMessageVipUpgradeParamList.Add(customerMessageVipUpgradeParam);
            // }

            CustomerScoreLogInfo promotionScoreLog = new CustomerScoreLogInfo {
                Customer = promotionParentCustomer,
                ScoreType = CustomerScoreType.Promotion,
                Score = promotionScore,
                Amount = amount,
                IsValidScore = isValidScorePromotion,
                LogisticsOrderPay = orderPay,
                LogisticsOrder = order,
                LogisticsOrderCode = order.Code,
                LogisticsOrderCustomer = orderCustomer, // 订单的会员，这里跟 积分会员 不是 同一个
                Description = ""
            };

            session.Save(promotionScoreLog);
            session.Flush();

            // 有效积分时才发送
            if (isValidScorePromotion) {
                customerMessageMoneyEarnedService.SendMessage(promotionParentCustomer, promotionScore);
            }
        }

        #endregion

        // todo vip 升级的逻辑
        // IList<int> custimerIdList = customerMessageVipUpgradeParamList.Select(e => e.Customer.Id).ToList();
        //
        // // 这是更新所有的会员，如果可以的话，可以只更新当前的
        // // customerService.UpdateAllCustomerVip();
        //
        // // 只更新指定的会员
        // customerService.UpdateCustomerVip(custimerIdList);
        //
        // // 找出这里的超级VIP客户ID列表
        // IList<int> superVipCustomerIdList = session.Query<CustomerSuperVipDiscountInfo>()
        //     .Where(e => custimerIdList.Contains(e.Customer.Id))
        //     .Select(e => e.Customer.Id)
        //     .ToList();
        //
        // foreach (CustomerMessageVipUpgradeParam msgParam in customerMessageVipUpgradeParamList) {
        //     if (superVipCustomerIdList.Contains(msgParam.Customer?.Id ?? 0) == false) // 不是超级VIP的才发送， 超级VIP不需要发送VIP升级消息
        //     {
        //         customerMessageService.SendMessageVipShengJi(msgParam.Customer, msgParam.NewVipLevel, msgParam.LogisticsFirstDiscount,
        //             msgParam.LogisticsSecondDiscount, msgParam.MallDiscount, msgParam.NextVipLevel);
        //     }
        // }
    }

    /// <summary>
    /// 积分换余额
    /// </summary>
    /// <param name="customerId">会员Id</param>
    /// <param name="score">积分</param>
    /// <param name="description">描述</param>
    /// <returns>余额</returns>
    public decimal ScoreToAccount(int customerId, decimal score, string description = "") {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException("指定的会员不存在！");
        }

        if (score <= 0) {
            throw new ValidationException("需要兑换余额的积分应该大于0！");
        }

        if (customer.ValidScore <= 0) {
            throw new ValidationException("会员当前没有可用剩余积分，操作失败！");
        }

        if (customer.ValidScore < score) {
            throw new ValidationException("会员积分少于需要兑换余额的剩余积分，操作失败！");
        }

        CustomerScoreRuleInfo? scoreToBalanceRule = session.Query<CustomerScoreRuleInfo>()
            .Where(e => e.Type == CustomerScoreType.ScoreToBalance)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (scoreToBalanceRule == null) {
            throw new Exception("积分兑换余额规则未设置！");
        }

        if (scoreToBalanceRule.Score <= 0) {
            throw new Exception("积分兑换余额规则，兑换积分设置小于等于零，不符合要求！");
        }

        if (scoreToBalanceRule.Amount <= 0) {
            throw new Exception("积分兑换余额规则，兑换金额设置小于等于零，不符合要求！");
        }

        // 积分换成余额
        decimal amount = (score / scoreToBalanceRule.Score) * scoreToBalanceRule.Amount;

        // 减少积分
        customer.ValidScore = customer.ValidScore - score;

        session.Update(customer);
        session.Flush();

        // 记录积分变动日志
        CustomerScoreLogInfo scoreLog = new CustomerScoreLogInfo {
            Customer = customer,
            ScoreType = CustomerScoreType.ScoreToBalance,
            Score = -score,
            Amount = amount,
            IsValidScore = true, // 有效积分才能换余额
            Description = description
        };

        session.Save(scoreLog);
        session.Flush();

        // 增加余额，记录余额变动日志
        customerAccountService.Change(CustomerAccountChangeType.ScoreToAccount, customer.Id, amount, description);

        return amount;
    }
}