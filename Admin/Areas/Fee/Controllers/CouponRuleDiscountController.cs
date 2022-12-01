using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Fee.Models.CouponRuleDiscount;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.Fee.Controllers;

[FeeArea]
[AdminAuth(PassRes.Fee.Coupon)]
public class CouponRuleDiscountController : Controller
{
    private ISession session;
    private CouponRuleDiscountService couponRuleDiscountService ;

    public CouponRuleDiscountController(
        ISession session,
        CouponRuleDiscountService couponRuleDiscountService)
    {
        this.session = session;
        this.couponRuleDiscountService = couponRuleDiscountService;
    }

    [HttpGet]
    [Route("/admin/coupon/rule/discount/detail")]
    public IActionResult Detail(int id, int batchId)
    {
        session.DefaultReadOnly = true;

        CouponBatchInfo? batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"优惠券批次#{batchId}不存在！");
        }

        CouponRuleDiscountInfo? couponRuleDiscount;

        if (id == 0)
        {
            couponRuleDiscount = new CouponRuleDiscountInfo();
        }
        else
        {
            couponRuleDiscount = session.Get<CouponRuleDiscountInfo>(id);
            if (couponRuleDiscount == null)
            {
                throw new ValidationException($"指定的 优惠券折扣规则#{id} 不存在！");
            }
        }

        CouponRuleDiscountDetailModel model = new CouponRuleDiscountDetailModel();
        model.CouponRuleDiscountId = couponRuleDiscount.Id;

        model.BatchId = batchId;
        model.Rate = couponRuleDiscount.Rate;
        model.CanModify = batch.Status?.Released() != true;

        return View("CouponRuleDiscountDetail", model);
    }

    [HttpPost]
    [Route("/admin/coupon/rule/discount/save")]
    [Transactional]
    public IActionResult Save(CouponRuleDiscountDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int couponRuleDiscountId = model.CouponRuleDiscountId;

        bool isCreate = couponRuleDiscountId == 0;

        CouponRuleDiscountInfo? rule;

        if (isCreate)
        {
            rule = new CouponRuleDiscountInfo();
        }
        else
        {
            rule = session.Get<CouponRuleDiscountInfo>(couponRuleDiscountId);
            if (rule == null)
            {
                throw new ValidationException($"指定的 优惠券折扣规则#{couponRuleDiscountId} 不存在！");
            }
        }

        var batch = session.Get<CouponBatchInfo>(model.BatchId);

        rule.Batch = batch;
        rule.Rate = model.Rate;

        couponRuleDiscountService.CheckRuleValid(rule);

        if (isCreate)
        {
            session.Save(rule);
        }
        else
        {
            session.Update(rule);
        }

        session.Flush();

        // 生成抵扣规则描述
        batch.Rule = couponRuleDiscountService.GetRule(rule);
        batch.RuleDesc = couponRuleDiscountService.GetRuleDesc(rule);
        session.Update(batch);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}