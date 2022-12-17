using Newtonsoft.Json;
using NHibernate;
using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponService
{
    private ISession session;
    private CouponBatchService couponBatchService;
    private CouponRuleDeductService couponRuleDeductService;
    private CouponRuleDiscountService couponRuleDiscountService;
    private CouponRulePriceBreakService couponRulePriceBreakService;

    public CouponService(
        ISession session,
        CouponBatchService couponBatchService,
        CouponRuleDeductService couponRuleDeductService,
        CouponRuleDiscountService couponRuleDiscountService,
        CouponRulePriceBreakService couponRulePriceBreakService)
    {
        this.session = session;
        this.couponBatchService = couponBatchService;
        this.couponRuleDeductService = couponRuleDeductService;
        this.couponRuleDiscountService = couponRuleDiscountService;
        this.couponRulePriceBreakService = couponRulePriceBreakService;
    }

    /// <summary>
    /// 下单时是否有优惠券可以用
    /// 跟kotlin里面的同名方法逻辑一致的
    /// </summary>
    /// <param name="customerId"></param>
    /// <returns></returns>
    public bool HasCouponToUseBeforeOrderCreate(int customerId)
    {
        couponBatchService.CheckAndSetExpire(); // 这个检查应该要加上，因为这里在决定优惠券使用界面是否出现之前的

        var coupponStatusPick = CouponStatus.Pick;
        var coupon = session.Query<CouponInfo>()
            .Where(e => e.Customer.Id == customerId
                        && e.Status == coupponStatusPick)
            .Take(1)
            .SingleOrDefault();

        var hasCoupon = coupon != null;

        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);

        // 如果是 false 应该把 create select 中的置空，万一前面有选择
        if (createSelect != null && hasCoupon == false && (createSelect.CouponId ?? 0) != 0)
        {
            // session.Query<LogisticsOrderCreateSelectInfo>()
            //     .Where(e => e.CustomerId == customerId)
            //     .UpdateBuilder()
            //     .Set(e => e.CouponId, 0)
            //     .Update();

            createSelect.CouponId = 0;
            session.Update(createSelect);

            session.Flush();
        }

        return hasCoupon;
    }

    public void CheckCouponValidBeforeOrderCreate(int couponId, int customerId)
    {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null)
        {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        NHibernateUtil.Initialize(coupon.Batch);

        // 是不是当前会员的
        if (coupon.Customer.Id != customerId)
        {
            throw new ValidationException($"优惠券{coupon.Code}不是当前登录用户#{customerId}的！");
        }

        // 状态，有效期
        if (coupon.Status == CouponStatus.Use)
        {
            throw new ValidationException($"优惠券{coupon.Code}之前使用过，已无效！");
        }

        if (coupon.Status == CouponStatus.Expire)
        {
            throw new ValidationException($"优惠券{coupon.Code}已过期！");
        }

        var today = DateTime.Today;
        if (coupon.Batch.Expire < today)
        {
            throw new ValidationException($"优惠券{coupon.Code}已过期！");
        }
    }

    public void CheckCouponValidAfterOrderCreate(int couponId, int orderId)
    {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null)
        {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new ValidationException($"指定的#{orderId}订单不存在！");
        }

        NHibernateUtil.Initialize(coupon.Batch);

        // 是不是当前会员的
        int customerId = order.Customer?.Id ?? 0;
        if (coupon.Customer.Id != customerId)
        {
            throw new ValidationException($"优惠券{coupon.Code}不是当前登录用户#{customerId}的！");
        }

        // 状态，有效期
        if (coupon.Status == CouponStatus.Use)
        {
            throw new ValidationException($"优惠券{coupon.Code}之前使用过，已无效！");
        }

        if (coupon.Status == CouponStatus.Expire)
        {
            throw new ValidationException($"优惠券{coupon.Code}已过期！");
        }

        var today = DateTime.Today;
        if (coupon.Batch.Expire < today)
        {
            throw new ValidationException($"优惠券{coupon.Code}已过期！");
        }
    }

    public void CheckCouponValidWhenPay(int couponId, int orderId, int customerId)
    {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null)
        {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        NHibernateUtil.Initialize(coupon.Batch);

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null)
        {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        // 是不是当前会员的
        if (coupon.Customer.Id != customerId)
        {
            throw new ValidationException($"优惠券{coupon.Code}不是当前登录用户#{customerId}的！");
        }

        // 状态，有效期
        if ((coupon.Order?.Id ?? 0) != orderId && coupon.Status == CouponStatus.Use)
        {
            throw new ValidationException($"优惠券{coupon.Code}之前使用过，已无效！");
        }

        if ((coupon.Order?.Id ?? 0) != orderId && coupon.Status == CouponStatus.Expire)
        {
            throw new ValidationException($"优惠券{coupon.Code}已过期！");
        }

        var today = DateTime.Today;
        if ((coupon.Order?.Id ?? 0) != orderId && coupon.Batch.Expire < today)
        {
            throw new ValidationException($"优惠券{coupon.Code}已过期！");
        }

        if ((order.Customer?.Id ?? 0) != customerId)
        {
            throw new ValidationException($"指定的订单#{orderId}不属于会员#{customerId}！");
        }
    }

    public void SetCouponUsed(int couponId, LogisticsOrderInfo order, decimal actualDiscountAmount)
    {
        var now = DateTime.Now;
        // session.Query<CouponInfo>()
        //     .Where(e => e.Id == couponId)
        //     .UpdateBuilder()
        //     .Set(e => e.Order, order)
        //     .Set(e => e.ActualDiscountAmount, actualDiscountAmount)
        //     .Set(e => e.Status, CouponStatus.Use)
        //     .Set(e => e.UseTime, now)
        //     .Set(e => e.UpdateTime, now)
        //     .Update();

        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null)
        {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        coupon.Order = order;
        coupon.ActualDiscountAmount = actualDiscountAmount;
        coupon.Status = CouponStatus.Use;
        coupon.UseTime = now;

        session.Update(coupon);
        session.Flush();
    }

    public void UpdateActualDiscountAmount(int couponId, decimal actualDiscountAmount)
    {
        var now = DateTime.Now;
        // session.Query<CouponInfo>()
        //     .Where(e => e.Id == couponId)
        //     .UpdateBuilder()
        //     .Set(e => e.ActualDiscountAmount, actualDiscountAmount)
        //     .Set(e => e.UpdateTime, now)
        //     .Update();

        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null)
        {
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
    public void setCouponNotUsed(int orderId)
    {
        var now = DateTime.Now;
        // session.Query<CouponInfo>()
        //     .Where(e => e.Order!.Id == orderId)
        //     .UpdateBuilder()
        //     .Set(e => e.Order, (LogisticsOrderInfo?)null)
        //     .Set(e => e.ActualDiscountAmount, 0)
        //     .Set(e => e.Status, CouponStatus.Pick) // 这里暂时不考虑过期的问题
        //     .Set(e => e.UseTime, (DateTime?)null)
        //     .Set(e => e.UpdateTime, now)
        //     .Update();

        var orderCouponList = session.Query<CouponInfo>() // 一般也就只有一张
            .Where(e => e.Order!.Id == orderId)
            .ToList();
        foreach (var coupon in orderCouponList)
        {
            coupon.Order = null;
            coupon.ActualDiscountAmount = 0;
            coupon.Status = CouponStatus.Pick; // 这里暂时不考虑优惠券这时实际已经过期的问题
            coupon.UseTime = null;

            session.Update(coupon);
        }

        session.Flush();
    }

    public void SetCouponUsedAndSumSaveBatchTotalUsed(int couponId, LogisticsOrderInfo order, decimal actualDiscountAmount)
    {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null)
        {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        SetCouponUsed(couponId, order, actualDiscountAmount);

        couponBatchService.SumAndSaveBatchTotalUsed(coupon.Batch?.Id ?? 0);
    }

    public void SetCouponNotUsedAndSumSaveBatchTotalUsed(int couponId)
    {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null)
        {
            throw new ValidationException($"指定的优惠券#{couponId}不存在！");
        }

        setCouponNotUsed(couponId);

        couponBatchService.SumAndSaveBatchTotalUsed(coupon.Batch?.Id ?? 0);
    }

    public class CouponDiscountResult
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; } = string.Empty;

        [JsonProperty("couponCode")]
        public string CouponCode { get; set; } = string.Empty;

        [JsonProperty("rule")]
        public string Rule { get; set; } = string.Empty;

        private decimal discount;

        [JsonIgnore]
        public decimal Discount
        {
            get { return discount; }
            set
            {
                if (value < 0)
                {
                    throw new ValidationException("优惠券实际折扣不能小于0！");
                }

                discount = value;
            }
        }

        [JsonIgnore]
        public bool AirOrderSummary { get; set; } = false;

        [JsonProperty("discount")]
        protected string DiscountStr
        {
            get
            {
                if (AirOrderSummary)
                {
                    return "计费后计算";
                }

                return discount.ToString("0.00");
            }
        }
    }

    /// <summary>
    /// 计算优惠券折扣
    /// </summary>
    /// <param name="couponId">优惠券</param>
    /// <param name="logisticsFee">物流费用，现在假设是 头程费用 + 二程费用</param>
    public CouponDiscountResult CalculateCouponDiscount(int couponId, decimal logisticsFee)
    {
        var coupon = session.Get<CouponInfo>(couponId);
        if (coupon == null)
        {
            throw new ValidationException($"指定的优惠券{couponId}不存在！");
        }

        NHibernateUtil.Initialize(coupon.Batch);

        int batchId = coupon.Batch?.Id ?? 0;

        var couponType = coupon.Batch?.Type ?? CouponType.None;
        if (couponType == CouponType.Deduct)
        {
            CouponRuleDeductInfo? rule = session.Query<CouponRuleDeductInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null)
            {
                throw new ValidationException($"优惠券批次#{batchId}未关联抵扣券规则！");
            }

            decimal discount = couponRuleDeductService.GetDiscount(rule, logisticsFee);

            return new CouponDiscountResult() {
                Success = true,
                Message = "success",
                CouponCode = coupon.Code,
                Rule = couponRuleDeductService.GetRuleDesc(rule),
                Discount = discount,
            };
        }

        if (couponType == CouponType.Discount)
        {
            CouponRuleDiscountInfo? rule = session.Query<CouponRuleDiscountInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null)
            {
                throw new ValidationException($"优惠券批次#{batchId}未关联折扣券规则！");
            }

            decimal discount = couponRuleDiscountService.GetDiscount(rule, logisticsFee);

            return new CouponDiscountResult() {
                Success = true,
                Message = "success",
                CouponCode = coupon.Code,
                Rule = couponRuleDiscountService.GetRuleDesc(rule),
                Discount = discount,
            };
        }

        if (couponType == CouponType.PriceBreak)
        {
            CouponRulePriceBreakInfo? rule = session.Query<CouponRulePriceBreakInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null)
            {
                throw new ValidationException($"优惠券批次#{batchId}未关联满减券规则！");
            }

            decimal discount = couponRulePriceBreakService.GetDiscount(rule, logisticsFee);
            if (discount <= 0)
            {
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
}