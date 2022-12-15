using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.Customer;

[Service(ServiceType.Scoped)]
public class CustomerScoreService
{
    private readonly ISession session;

    // private readonly CustomerService customerService;
    // private readonly CustomerMessageService customerMessageService;
    // private readonly CustomerMessageWoZhuanDaoDeQianService customerMessageWoZhuanDaoDeQianService;
    // private readonly CustomerVipDiscountService customerVipDiscountService;
    private readonly CustomerAccountService customerAccountService;
    // private readonly CustomerClientUrlService customerClientUrlService;

    public CustomerScoreService(ISession session,
            // CustomerService customerService,
            // CustomerMessageService customerMessageService,
            // CustomerMessageWoZhuanDaoDeQianService customerMessageWoZhuanDaoDeQianService,
            // CustomerVipDiscountService customerVipDiscountService,
            CustomerAccountService customerAccountService)
        // CustomerClientUrlService customerClientUrlService)
    {
        this.session = session;
        // this.customerService = customerService;
        // this.customerMessageService = customerMessageService;
        // this.customerMessageWoZhuanDaoDeQianService = customerMessageWoZhuanDaoDeQianService;
        // this.customerVipDiscountService = customerVipDiscountService;
        this.customerAccountService = customerAccountService;
        // this.customerClientUrlService = customerClientUrlService;
    }

    // todo 这里面要优化，特别是跟数据库事务相关的方面的。
    /// <summary>
    /// 添加支付产生积分
    /// 同时添加  集运消费积分 和 会员推广积分
    /// </summary>
    /// <param name="logisticsOrderPayId">对应的集运订单支付Id</param>
    public void AddScore(int logisticsOrderPayId)
    {
        /*

        LogisticsOrderPayInfo? orderPay = session.Get<LogisticsOrderPayInfo>(logisticsOrderPayId);
        if (orderPay == null)
        {
            throw new ValidationException("指定的集运订单支付不存在！");
        }

        NHibernateUtil.Initialize(orderPay.Order);

        LogisticsOrderCate orderCate = orderPay.Order.Cate;

        CustomerScoreRuleInfo logisticsConsumeRule = session.Query<CustomerScoreRuleInfo>()
            .Where(e => e.OrderCate == orderCate && e.Type == CustomerScoreType.LogisticsConsume)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (logisticsConsumeRule == null)
        {
            throw new Exception("集运消费积分规则未设置！");
        }

        if (logisticsConsumeRule.Amount <= 0)
        {
            throw new Exception("集运消费积分规则，集运消费金额设置小于等于零，不符合要求！");
        }

        if (logisticsConsumeRule.Score <= 0)
        {
            throw new Exception("集运消费积分规则，产生积分设置小于等于零，不符合要求！");
        }

        CustomerScoreRuleInfo? promotionRule = session.Query<CustomerScoreRuleInfo>()
            // .Where(e => e.OrderCate == orderCate && e.Type == CustomerScoreType.Promotion)
            .Where(e => e.Type == CustomerScoreType.Promotion) // 推广积分现在还是不按订单区分的
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (promotionRule == null)
        {
            throw new Exception("会员推广积分规则未设置！");
        }

        if (promotionRule.Amount <= 0)
        {
            throw new Exception("会员推广积分规则，下线消费金额设置小于等于零，不符合要求！");
        }

        if (promotionRule.Score <= 0)
        {
            throw new Exception("会员推广积分规则，集运积分设置小于等于零，不符合要求！");
        }

        LogisticsOrderInfo? order = session.Get<LogisticsOrderInfo>(orderPay.Order?.Id ?? 0);
        if (order == null)
        {
            throw new ValidationException("订单支付对应订单为空，数据非法！");
        }

        CustomerInfo? orderCustomer = session.Get<CustomerInfo>(order.Customer?.Id ?? 0); // 取订单的客户
        if (orderCustomer == null)
        {
            throw new ValidationException("订单支付对应订单的会员为空，数据非法！");
        }

        bool isValidScoreLogisticsConsume = true; // 是不是集运消费有效积分
        if (orderCate == LogisticsOrderCate.Sea) // 海运的时候超级VIP才有可能没有有效积分
        {
            CustomerSuperVipDiscountInfo? superVipLogisticsConsume = session.Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == orderCustomer.Id)
                .Take(1)
                .SingleOrDefault();

            if (superVipLogisticsConsume != null) // 是超级VIP
            {
                isValidScoreLogisticsConsume = false;
            }
        }

        decimal amount = orderPay.Amount;
        if (amount < 0) // 这里假设可以为0，也许也有因为优惠最终不用付钱的
        {
            throw new ValidationException("支付金额不可能小于等于0，数据非法！");
        }

        IList<CustomerVipDiscountInfo> customerVipDiscountList = customerVipDiscountService.GetCustomerVipDiscountList(); // 这个数据总共才 20 来条

        IList<CustomerMessageVipUpgradeParam> customerMessageVipUpgradeParamList = new List<CustomerMessageVipUpgradeParam>();

        // 集运消费积分

        #region 集运消费积分

        decimal logisticsConsumeScore = (amount / logisticsConsumeRule.Amount) * logisticsConsumeRule.Score;

        decimal orderCustomerOldTotalScore = orderCustomer.TotalScore ?? 0m;

        orderCustomer.TotalScore = (orderCustomer.TotalScore ?? 0m) + logisticsConsumeScore;

        if (isValidScoreLogisticsConsume)
        {
            if (orderCate == LogisticsOrderCate.Sea)
            {
                orderCustomer.ValidScore = (orderCustomer.ValidScore ?? 0m) + logisticsConsumeScore;
            }

            if (orderCate == LogisticsOrderCate.Air)
            {
                orderCustomer.ValidScoreAir = (orderCustomer.ValidScoreAir ?? 0m) + logisticsConsumeScore;
            }
        }

        session.Update(orderCustomer);
        session.Flush();

        decimal orderCustomerNewTotalScore = orderCustomer.TotalScore ?? 0m;

        if (customerVipDiscountService.IsVipUpgrade(orderCustomerOldTotalScore, orderCustomerNewTotalScore, customerVipDiscountList))
        {
            CustomerVipDiscountInfo customerVipDiscount = customerVipDiscountService.GetDiscount(orderCustomerNewTotalScore, customerVipDiscountList);
            CustomerVipDiscountInfo customerVipDiscountNextLevel = customerVipDiscountService.GetNextLevelDiscount(orderCustomerNewTotalScore, customerVipDiscountList);

            CustomerMessageVipUpgradeParam customerMessageVipUpgradeParam = new CustomerMessageVipUpgradeParam();
            customerMessageVipUpgradeParam.Customer = orderCustomer;
            customerMessageVipUpgradeParam.NewVipLevel = customerVipDiscount?.LevelName;
            customerMessageVipUpgradeParam.LogisticsFirstDiscount = $"{customerVipDiscount?.LogisticsFirstDiscount.ToString("#0.00")}%";
            customerMessageVipUpgradeParam.LogisticsSecondDiscount = $"{customerVipDiscount?.LogisticsSecondDiscount.ToString("#0.00")}%";
            customerMessageVipUpgradeParam.MallDiscount = $"{customerVipDiscount?.MallDiscount.ToString("#0.00")}%";
            customerMessageVipUpgradeParam.NextVipLevel = customerVipDiscountNextLevel?.LevelName;

            customerMessageVipUpgradeParamList.Add(customerMessageVipUpgradeParam);
        }

        CustomerScoreLogInfo logisticsConsumeScoreLog = new CustomerScoreLogInfo();
        logisticsConsumeScoreLog.OrderCate = orderCate;
        logisticsConsumeScoreLog.Customer = orderCustomer;
        logisticsConsumeScoreLog.ScoreType = CustomerScoreType.LogisticsConsume;
        logisticsConsumeScoreLog.Score = logisticsConsumeScore;
        logisticsConsumeScoreLog.Amount = amount;
        logisticsConsumeScoreLog.IsValidScore = isValidScoreLogisticsConsume;
        logisticsConsumeScoreLog.LogisticsOrderPay = orderPay;
        logisticsConsumeScoreLog.LogisticsOrder = order;
        logisticsConsumeScoreLog.LogisticsOrderCode = order.Code;
        logisticsConsumeScoreLog.LogisticsOrderCustomer = orderCustomer; // 订单的会员，这里跟积分会员 是 同一个
        logisticsConsumeScoreLog.Description = "";
        logisticsConsumeScoreLog.Gap = null;

        session.Save(logisticsConsumeScoreLog);
        session.Flush();

        if (isValidScoreLogisticsConsume) // 有效积分时才发送
        {
            customerMessageWoZhuanDaoDeQianService.SendMessage(orderCustomer, orderCate, logisticsConsumeScore, 0);
        }

        #endregion

        // 会员推广积分

        #region 会员推广积分

        decimal promotionScore = (amount / promotionRule.Amount) * promotionRule.Score;

        IList<CustomerPromotionInfo> promotionList = session.Query<CustomerPromotionInfo>()
            .Fetch(e => e.Parent)
            .Where(e => e.Child.Id == orderCustomer.Id && e.Valid == true)
            .ToList();

        CustomerPromotionInfo promotionGapDirect = promotionList.Where(e => e.Gap == 1).Take(1).SingleOrDefault();
        if (promotionGapDirect != null)
        {
            // 直接上家的积分
            decimal promotionDirectScore = promotionScore; // 直接上家获取所有积分，不再跟间接的分享积分

            // 直接上家
            CustomerInfo directParentCustomer = promotionGapDirect.Parent;

            // 直接上家是不是超级会员
            CustomerSuperVipDiscountInfo? superVipPromotion = session.Query<CustomerSuperVipDiscountInfo>()
                .Where(e => e.Customer.Id == directParentCustomer.Id)
                .Take(1)
                .SingleOrDefault();

            bool isValidScorePromotion = true; // 是不是集运消费有效积分
            if (superVipPromotion != null) // 是超级VIP
            {
                isValidScorePromotion = false;
            }

            decimal directParentCustomerOldTotalScore = directParentCustomer.TotalScore ?? 0m;

            directParentCustomer.TotalScore = (directParentCustomer.TotalScore ?? 0m) + promotionDirectScore;

            if (isValidScorePromotion)
            {
                if (orderCate == LogisticsOrderCate.Sea)
                {
                    directParentCustomer.ValidScore = (directParentCustomer.ValidScore ?? 0m) + promotionDirectScore;
                }

                if (orderCate == LogisticsOrderCate.Air)
                {
                    directParentCustomer.ValidScoreAir = (directParentCustomer.ValidScoreAir ?? 0m) + promotionDirectScore;
                }
            }

            session.Update(directParentCustomer);
            session.Flush();

            decimal directParentCustomerNewTotalScore = directParentCustomer.TotalScore ?? 0m;

            if (customerVipDiscountService.IsVipUpgrade(directParentCustomerOldTotalScore, directParentCustomerNewTotalScore, customerVipDiscountList))
            {
                CustomerVipDiscountInfo customerVipDiscount = customerVipDiscountService.GetDiscount(directParentCustomerNewTotalScore, customerVipDiscountList);
                CustomerVipDiscountInfo customerVipDiscountNextLevel = customerVipDiscountService.GetNextLevelDiscount(directParentCustomerNewTotalScore, customerVipDiscountList);

                CustomerMessageVipUpgradeParam customerMessageVipUpgradeParam = new CustomerMessageVipUpgradeParam();
                customerMessageVipUpgradeParam.Customer = directParentCustomer;
                customerMessageVipUpgradeParam.NewVipLevel = customerVipDiscount?.LevelName;
                customerMessageVipUpgradeParam.LogisticsFirstDiscount = $"{customerVipDiscount?.LogisticsFirstDiscount.ToString("#0.00")}%";
                customerMessageVipUpgradeParam.LogisticsSecondDiscount = $"{customerVipDiscount?.LogisticsSecondDiscount.ToString("#0.00")}%";
                customerMessageVipUpgradeParam.MallDiscount = $"{customerVipDiscount?.MallDiscount.ToString("#0.00")}%";
                customerMessageVipUpgradeParam.NextVipLevel = customerVipDiscountNextLevel?.LevelName;

                customerMessageVipUpgradeParamList.Add(customerMessageVipUpgradeParam);
            }

            CustomerScoreLogInfo promotionDirectScoreLog = new CustomerScoreLogInfo();
            promotionDirectScoreLog.OrderCate = orderCate;
            promotionDirectScoreLog.Customer = directParentCustomer;
            promotionDirectScoreLog.ScoreType = CustomerScoreType.Promotion;
            promotionDirectScoreLog.Score = promotionDirectScore;
            promotionDirectScoreLog.Amount = amount;
            promotionDirectScoreLog.IsValidScore = isValidScorePromotion;
            promotionDirectScoreLog.LogisticsOrderPay = orderPay;
            promotionDirectScoreLog.LogisticsOrder = order;
            promotionDirectScoreLog.LogisticsOrderCode = order.Code;
            promotionDirectScoreLog.LogisticsOrderCustomer = orderCustomer; // 订单的会员，这里跟 积分会员 不是 同一个
            promotionDirectScoreLog.Description = "";
            promotionDirectScoreLog.Gap = 1;

            session.Save(promotionDirectScoreLog);
            session.Flush();

            if (isValidScorePromotion) // 有效积分时才发送
            {
                customerMessageWoZhuanDaoDeQianService.SendMessage(directParentCustomer, orderCate, promotionDirectScore, 1);
            }
        }

        #endregion

        IList<int> custimerIdList = customerMessageVipUpgradeParamList.Select(e => e.Customer.Id).ToList();

        // 这是更新所有的会员，如果可以的话，可以只更新当前的
        // customerService.UpdateAllCustomerVip();

        // 只更新指定的会员
        customerService.UpdateCustomerVip(custimerIdList);

        // 找出这里的超级VIP客户ID列表
        IList<int> superVipCustomerIdList = session.Query<CustomerSuperVipDiscountInfo>()
            .Where(e => custimerIdList.Contains(e.Customer.Id))
            .Select(e => e.Customer.Id)
            .ToList();

        foreach (CustomerMessageVipUpgradeParam msgParam in customerMessageVipUpgradeParamList)
        {
            if (superVipCustomerIdList.Contains(msgParam.Customer?.Id ?? 0) == false) // 不是超级VIP的才发送， 超级VIP不需要发送VIP升级消息
            {
                // customerMessageService.SendCustomerMessage(CustomerMessageSendType.RealTime, CustomerMessageType.VipShengJiTuiSong, msgParam.Customer,
                //    msgParam.NewVipLevel, msgParam.LogisticsFirstDiscount, msgParam.LogisticsSecondDiscount, msgParam.MallDiscount, msgParam.NextVipLevel);

                customerMessageService.SendMessageVipShengJi(msgParam.Customer, msgParam.NewVipLevel, msgParam.LogisticsFirstDiscount,
                    msgParam.LogisticsSecondDiscount, msgParam.MallDiscount, msgParam.NextVipLevel);
            }
        }
        
        */
    }

    /// <summary>
    /// 积分换余额
    /// </summary>
    /// <param name="customerId">会员Id</param>
    /// <param name="score">积分</param>
    /// <param name="description">描述</param>
    /// <returns>余额</returns>
    public decimal ScoreToBalance(int customerId, decimal score, string description = "")
    {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null)
        {
            throw new ValidationException("指定的会员不存在！");
        }

        if (score <= 0)
        {
            throw new ValidationException("需要兑换余额的积分应该大于0！");
        }

        if (customer.ValidScore <= 0)
        {
            throw new ValidationException("会员当前没有可用剩余积分，操作失败！");
        }

        if (customer.ValidScore < score)
        {
            throw new ValidationException("会员积分少于需要兑换余额的剩余积分，操作失败！");
        }

        CustomerScoreRuleInfo? scoreToBalanceRule = session.Query<CustomerScoreRuleInfo>()
            .Where(e => e.Type == CustomerScoreType.ScoreToBalance)
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (scoreToBalanceRule == null)
        {
            throw new Exception("积分兑换余额规则未设置！");
        }

        if (scoreToBalanceRule.Score <= 0)
        {
            throw new Exception("积分兑换余额规则，兑换积分设置小于等于零，不符合要求！");
        }

        if (scoreToBalanceRule.Amount <= 0)
        {
            throw new Exception("积分兑换余额规则，兑换金额设置小于等于零，不符合要求！");
        }

        // 积分换成余额
        decimal amount = (score / scoreToBalanceRule.Score) * scoreToBalanceRule.Amount;

        // 减少积分
        customer.ValidScore -= score;

        session.Update(customer);
        session.Flush();

        // 记录积分变动日志
        CustomerScoreLogInfo scoreLog = new CustomerScoreLogInfo();
        scoreLog.Customer = customer;
        scoreLog.ScoreType = CustomerScoreType.ScoreToBalance;
        scoreLog.Score = -score;
        scoreLog.Amount = amount;
        scoreLog.IsValidScore = true; // 有效积分才能换余额
        scoreLog.Description = description;

        session.Save(scoreLog);
        session.Flush();

        // 增加余额，记录余额变动日志
        customerAccountService.Change(CustomerAccountChangeType.ScoreToAccount, customer.Id, amount, description);

        return amount;
    }
}