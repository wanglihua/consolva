using Newtonsoft.Json;
using NHibernate;
using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using shunda.Service.Fee;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponService {
    private readonly ISession session;
    private readonly CouponBatchService couponBatchService;
    private readonly CouponRuleDeductService couponRuleDeductService;
    private readonly CouponRuleDiscountService couponRuleDiscountService;
    private readonly CouponRulePriceBreakService couponRulePriceBreakService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly CustomerVipDiscountService customerVipDiscountService;

    public CouponService(
        ISession session,
        CouponBatchService couponBatchService,
        CouponRuleDeductService couponRuleDeductService,
        CouponRuleDiscountService couponRuleDiscountService,
        CouponRulePriceBreakService couponRulePriceBreakService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        CustomerVipDiscountService customerVipDiscountService
    ) {
        this.session = session;
        this.couponBatchService = couponBatchService;
        this.couponRuleDeductService = couponRuleDeductService;
        this.couponRuleDiscountService = couponRuleDiscountService;
        this.couponRulePriceBreakService = couponRulePriceBreakService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.customerVipDiscountService = customerVipDiscountService;
    }

    /// <summary>
    /// 支付时查询有没可用的优惠券
    /// </summary>
    /// <param name="customerId">登录客户</param>
    /// <param name="orderId">订单，这个参数当前还没用起来</param>
    /// <returns></returns>
    public bool HasCouponWhenPay(int customerId, int orderId) {
        // 检查客服所在VIP级别是否支持优惠券
        if (customerVipDiscountService.CustomerCanCoupon(customerId) == false) {
            return false;
        }

        couponBatchService.CheckAndSetExpire();

        var coupon = session.Query<CouponInfo>()
            .Where(e => e.Customer.Id == customerId
                        && (e.Status == CouponStatus.Pick || e.Order!.Id == orderId))
            .Take(1)
            .SingleOrDefault();

        return coupon != null;
    }

    /// <summary>
    /// 支付时检查优惠券合法性
    /// </summary>
    /// <param name="couponId"></param>
    /// <param name="orderId"></param>
    /// <param name="customerId"></param>
    public void CheckCouponValidWhenPay(int couponId, int orderId, int customerId) {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        NHibernateUtil.Initialize(coupon.Batch);

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.Customer.Id != customerId) {
            throw new ValidationException($"指定的订单#{orderId}不属于会员#{customerId}！");
        }

        // 是不是当前会员的
        if (coupon.Customer.Id != customerId) {
            throw new ValidationException($"优惠券{coupon.Code}不是当前登录用户#{customerId}的！");
        }

        // 状态，有效期
        if ((coupon.Order?.Id ?? 0) != orderId && coupon.Status == CouponStatus.Use) {
            throw new ValidationException($"优惠券{coupon.Code}之前使用过，已无效！");
        }

        if ((coupon.Order?.Id ?? 0) != orderId && coupon.Status == CouponStatus.Expire) {
            throw new ValidationException($"优惠券{coupon.Code}已过期！");
        }

        var today = DateTime.Today;
        if ((coupon.Order?.Id ?? 0) != orderId && coupon.Batch.Expire < today) {
            throw new ValidationException($"优惠券{coupon.Code}已过期！");
        }
        
        // 检查客服所在VIP级别是否支持优惠券
        if (customerVipDiscountService.CustomerCanCoupon(customerId) == false) {
            throw new ValidationException($"客户#{customerId}VIP级别不支持使用优惠券！");
        }        
    }

    public void SetCouponUsed(int couponId, LogisticsOrderInfo order, decimal actualDiscountAmount) {
        var now = DateTime.Now;

        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }
        
        // 检查客服所在VIP级别是否支持优惠券
        int customerId = order.Customer.Id;
        if (customerVipDiscountService.CustomerCanCoupon(customerId) == false) {
            throw new ValidationException($"客户#{customerId}VIP级别不支持使用优惠券！");
        }        

        coupon.Order = order;
        coupon.ActualDiscountAmount = actualDiscountAmount;
        coupon.Status = CouponStatus.Use;
        coupon.UseTime = now;

        session.Update(coupon);

        // 记录日志
        var couponLog = new CouponLogInfo {
            Coupon = coupon,
            Type = CouponLogType.Use,
            Description = $"订单{order.Code}，实扣{actualDiscountAmount}"
        };
        session.Save(couponLog);

        session.Flush();
    }

    public void UpdateActualDiscountAmount(int couponId, decimal actualDiscountAmount) {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        coupon.ActualDiscountAmount = actualDiscountAmount;

        session.Update(coupon);
        session.Flush();
    }

    /// <summary>
    /// 清除订单使用的优惠券
    /// </summary>
    /// <param name="orderId"></param>
    public void setCouponNotUsed(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var orderCouponList = session.Query<CouponInfo>() // 一般也就只有一张
            .Fetch(e => e.Batch)
            .Where(e => e.Order!.Id == orderId)
            .ToList();
        foreach (var coupon in orderCouponList) {
            var actualDiscountAmountBefore = coupon.ActualDiscountAmount;

            coupon.Order = null;
            coupon.ActualDiscountAmount = 0;
            if (coupon.Batch.Status == CouponBatchStatus.Expire) {
                coupon.Status = CouponStatus.Expire;
            } else {
                coupon.Status = CouponStatus.Pick;
            }

            coupon.UseTime = null;

            session.Update(coupon);

            // 记录日志
            var couponLog = new CouponLogInfo {
                Coupon = coupon,
                Type = CouponLogType.Back,
                Description = $"订单{order.Code}，原实扣{actualDiscountAmountBefore}"
            };
            session.Save(couponLog);
        }

        session.Flush();
    }

    public void SetCouponUsedAndSumSaveBatchTotalUsed(int couponId, LogisticsOrderInfo order, decimal actualDiscountAmount) {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        SetCouponUsed(couponId, order, actualDiscountAmount);

        couponBatchService.SumAndSaveBatchTotalUsed(coupon.Batch?.Id ?? 0);
    }

    public void SetCouponNotUsedAndSumSaveBatchTotalUsed(int couponId) {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        setCouponNotUsed(couponId);

        couponBatchService.SumAndSaveBatchTotalUsed(coupon.Batch?.Id ?? 0);
    }

    public class CouponDiscountResult {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        [JsonProperty("couponCode")]
        public string CouponCode { get; set; } = string.Empty;

        [JsonProperty("rule")]
        public string Rule { get; set; } = string.Empty;

        private decimal discount;

        [JsonProperty("discount")]
        public decimal Discount {
            get { return discount; }
            set {
                if (value < 0) {
                    throw new ValidationException("优惠券实际折扣不能小于0！");
                }

                discount = value;
            }
        }
    }

    /// <summary>
    /// 计算优惠券折扣
    /// </summary>
    /// <param name="couponId">优惠券</param>
    /// <param name="orderAmount">物流费用，现在假设是 原订单总费用</param>
    public CouponDiscountResult CalculateCouponDiscount(int couponId, decimal orderAmount) {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null) {
            throw new ValidationException($"指定的优惠券{couponId}不存在！");
        }

        NHibernateUtil.Initialize(coupon.Batch);

        int batchId = coupon.Batch?.Id ?? 0;

        var couponType = coupon.Batch?.Type ?? CouponType.None;
        if (couponType == CouponType.Deduct) {
            CouponRuleDeductInfo? rule = session.Query<CouponRuleDeductInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null) {
                throw new ValidationException($"优惠券批次#{batchId}未关联抵扣券规则！");
            }

            decimal discount = couponRuleDeductService.GetDiscount(rule, orderAmount);

            return new CouponDiscountResult() {
                Success = true,
                Message = "success",
                CouponCode = coupon.Code,
                Rule = couponRuleDeductService.GetRuleDesc(rule),
                Discount = discount,
            };
        }

        if (couponType == CouponType.Discount) {
            CouponRuleDiscountInfo? rule = session.Query<CouponRuleDiscountInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null) {
                throw new ValidationException($"优惠券批次#{batchId}未关联折扣券规则！");
            }

            decimal discount = couponRuleDiscountService.GetDiscount(rule, orderAmount);

            return new CouponDiscountResult() {
                Success = true,
                Message = "success",
                CouponCode = coupon.Code,
                Rule = couponRuleDiscountService.GetRuleDesc(rule),
                Discount = discount,
            };
        }

        if (couponType == CouponType.PriceBreak) {
            CouponRulePriceBreakInfo? rule = session.Query<CouponRulePriceBreakInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null) {
                throw new ValidationException($"优惠券批次#{batchId}未关联满减券规则！");
            }

            decimal discount = couponRulePriceBreakService.GetDiscount(rule, orderAmount);
            if (discount <= 0) {
                return new CouponDiscountResult() {
                    Success = false,
                    Message = "头程二程总费用未达到满减标准！",
                    CouponCode = coupon.Code,
                    Rule = couponRulePriceBreakService.GetRuleDesc(rule),
                    Discount = 0,
                };
            }

            return new CouponDiscountResult() {
                Success = true,
                Message = "success",
                CouponCode = coupon.Code,
                Rule = couponRulePriceBreakService.GetRuleDesc(rule),
                Discount = discount,
            };
        }

        throw new ValidationException("未知类别优惠券！");
    }

    /// <summary>
    /// 保存优惠券到订单，并生成响应的订单费用，负的费用
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="orderId"></param>
    /// <param name="couponId"></param>
    public void SaveToOrder(int customerId, int orderId, int couponId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.Customer.Id != customerId) {
            throw new ValidationException($"订单#{orderId}不属于当前登录用户！");
        }

        CouponInfo? coupon = null;
        if (couponId != 0) {
            coupon = session.Get<CouponInfo>(couponId);
            if (coupon == null) {
                throw new ValidationException($"指定的优惠券#{couponId}不存在！");
            }

            // 检查有效性
            CheckCouponValidWhenPay(couponId: couponId, orderId: orderId, customerId: customerId);
        }

        var orderCoupon = session.Query<CouponInfo>()
            .Where(e => e.Order!.Id == orderId)
            .Take(1)
            .SingleOrDefault();

        if ((coupon?.Id ?? 0) == (orderCoupon?.Id ?? 0)) {
            // 如果当前的选择就是原来的，那就什么都不用做
            return;
        }

        // decimal totalAmount = order.OrderAmount; 不能把优惠费用计算在内
        var orderFeeList = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        decimal orderAmount = 0.0m;
        foreach (var fee in orderFeeList) {
            if (fee.Type != LogisticsOrderFeeType.CouponDiscountFee) {
                orderAmount += fee.Fee;
            }
        }

        decimal couponDiscount = 0m;
        if (coupon != null) {
            var couponDiscountResult = CalculateCouponDiscount(couponId, orderAmount);
            if (couponDiscountResult.Success == false) {
                throw new ValidationException(couponDiscountResult.Message);
            }

            couponDiscount = couponDiscountResult.Discount;
        }

        // 把原来的取消掉
        if (orderCoupon != null) {
            setCouponNotUsed(orderId: orderId);
        }

        // 把新的设置上
        if (coupon != null) {
            SetCouponUsed(couponId, order, couponDiscount);
        }

        var orderCouponBatchId = orderCoupon?.Batch.Id ?? 0;
        if (orderCouponBatchId != 0) {
            couponBatchService.SumAndSaveBatchTotalUsed(orderCouponBatchId);
        }

        var couponBatchId = coupon?.Batch.Id ?? 0;
        if (couponBatchId != 0) {
            couponBatchService.SumAndSaveBatchTotalUsed(couponBatchId);
        }

        // 先把原来的优惠券费用删除掉
        var couponOrderFeeList = orderFeeList
            .Where(e => e.Type == LogisticsOrderFeeType.CouponDiscountFee)
            .ToList();
        if (couponOrderFeeList.Count != 0) {
            foreach (var couponOrderFee in couponOrderFeeList) // 一般也只有一个
            {
                session.Delete(couponOrderFee);
            }
        }

        session.Flush();

        if (coupon != null) {
            // 插入新的优惠券费用
            var couponFee = new LogisticsOrderFeeInfo();
            couponFee.Customer = order.Customer;
            couponFee.Order = order;
            couponFee.OrderCode = order.Code;
            couponFee.GoodsType = LogisticsGoodsType.None;
            couponFee.Type = LogisticsOrderFeeType.CouponDiscountFee;
            couponFee.Description = "优惠券实扣," + (coupon.Code);
            couponFee.Fee = -couponDiscount;

            session.Save(couponFee);
            session.Flush();
        }

        // 更新订单总费用和支付状态
        // 当前方法的调用者，一般也会在调用后调用 这个 SumFeeAndUpdatePayStatus(), 会有可能得重复调用
        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(order.Id);
    }
}