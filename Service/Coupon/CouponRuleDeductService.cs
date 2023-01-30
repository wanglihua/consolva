using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponRuleDeductService {
    private readonly ISession session;

    public CouponRuleDeductService(ISession session) {
        this.session = session;
    }

    public void CheckRuleValid(CouponRuleDeductInfo rule) {
        if (rule == null) {
            return;
        }

        if (rule.Amount <= 0) {
            throw new ValidationException("抵扣券抵扣金额应大于0！");
        }
    }

    public string GetRule(CouponRuleDeductInfo rule) {
        if (rule == null) {
            return string.Empty;
        }

        return $"{rule.Amount:#0.#}RMB";
    }

    public string GetRuleDesc(CouponRuleDeductInfo rule) {
        if (rule == null) {
            return string.Empty;
        }

        return $"现金抵扣{rule.Amount:#0.#}RMB";
    }

    public decimal GetDiscount(int batchId, decimal logisticsFee) {
        CouponRuleDeductInfo? rule = session.Query<CouponRuleDeductInfo>()
            .Where(e => e.Batch.Id == batchId)
            .Take(1)
            .SingleOrDefault();
        if (rule == null) {
            throw new ValidationException($"优惠券批次#{batchId}未关联抵扣券规则！");
        }

        return GetDiscount(rule, logisticsFee);
    }

    public decimal GetDiscount(CouponRuleDeductInfo rule, decimal logisticsFee) {
        if (rule == null) {
            throw new ValidationException($"优惠券抵扣券规则不能为空！");
        }

        CheckRuleValid(rule);

        if (logisticsFee < rule.Amount) {
            return logisticsFee;
        }

        return rule.Amount;
    }
}