using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponRuleDiscountService
{
    private ISession session;

    public CouponRuleDiscountService(ISession session)
    {
        this.session = session;
    }

    public void CheckRuleValid(CouponRuleDiscountInfo rule)
    {
        if (rule == null)
        {
            return;
        }

        if (rule.Rate <= 0)
        {
            throw new ValidationException ("折扣券折率应该大于0！");
        }

        if (rule.Rate >= 1)
        {
            throw new ValidationException("折扣券折率应该小于1！");
        }
    }

    public string GetRule(CouponRuleDiscountInfo rule)
    {
        if (rule == null)
        {
            return string.Empty;
        }

        return $"{rule.Rate * 10: #0.#}折";
    }

    public string GetRuleDesc(CouponRuleDiscountInfo rule)
    {
        if (rule == null)
        {
            return string.Empty;
        }

        return $"{rule.Rate * 10: #0.#}折优惠";
    }

    public decimal GetDiscount(int batchId, decimal logisticsFee)
    {
        CouponRuleDiscountInfo? rule = session.Query<CouponRuleDiscountInfo>()
            .Where(e => e.Batch.Id == batchId)
            .Take(1)
            .SingleOrDefault();
        if (rule == null)
        {
            throw new ValidationException($"优惠券批次#{batchId}未关联折扣券规则！");
        }

        return GetDiscount(rule, logisticsFee);
    }

    public decimal GetDiscount(CouponRuleDiscountInfo rule, decimal logisticsFee)
    {
        if (rule == null)
        {
            throw new ValidationException($"优惠券折扣券规则不能为空！");
        }

        CheckRuleValid(rule);

        return logisticsFee * (1 - rule.Rate);
    }
}