using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponRulePriceBreakService
{
    private ISession session;

    public CouponRulePriceBreakService(ISession session)
    {
        this.session = session;
    }

    public void CheckRuleValid(CouponRulePriceBreakInfo rule)
    {
        if (rule == null)
        {
            return;
        }

        if (rule.Amount <= 0)
        {
            throw new ValidationException("满减券消费金额应该大于0！");
        }

        if (rule.Discount <= 0)
        {
            throw new ValidationException("满减券折减金额应大于0！");
        }

        if (rule.Discount > rule.Amount)
        {
            throw new ValidationException("满减券折减金额应小于消费金额！");
        }
    }

    public string GetRule(CouponRulePriceBreakInfo rule)
    {
        if (rule == null)
        {
            return string.Empty;
        }

        return $"{rule.Discount:#0.#}RMB";
    }

    public string GetRuleDesc(CouponRulePriceBreakInfo rule)
    {
        if (rule == null)
        {
            return string.Empty;
        }

        return $"满{rule.Amount:#0.#}减{rule.Discount:#0.#}RMB";
    }

    public decimal GetDiscount(int batchId, decimal logisticsFee)
    {
        CouponRulePriceBreakInfo? rule = session.Query<CouponRulePriceBreakInfo>()
            .Where(e => e.Batch.Id == batchId)
            .Take(1)
            .SingleOrDefault();
        if (rule == null)
        {
            throw new ValidationException($"优惠券批次#{batchId}未关联满减券规则！");
        }

        CheckRuleValid(rule);

        if (logisticsFee >= rule.Amount)
        {
            return rule.Discount;
        }

        return 0;
    }

    public decimal GetDiscount(CouponRulePriceBreakInfo rule, decimal logisticsFee)
    {
        if (rule == null)
        {
            throw new ValidationException($"优惠券满减券规则不能为空！");
        }

        CheckRuleValid(rule);

        if (logisticsFee >= rule.Amount)
        {
            return rule.Discount;
        }

        return 0;
    }
}