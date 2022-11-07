using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Fee.Models.CouponRuleDeduct;
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
public class CouponRuleDeductController : Controller
{
    private ISession session;
    private CouponRuleDeductService couponRuleDeductService;

    public CouponRuleDeductController(
        ISession session,
        CouponRuleDeductService couponRuleDeductService)
    {
        this.session = session;
        this.couponRuleDeductService = couponRuleDeductService;
    }

    [HttpGet]
    [Route("/admin/coupon/rule/deduct/detail")]
    public IActionResult Detail(int id, int batchId)
    {
        session.DefaultReadOnly = true;

        CouponBatchInfo batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"优惠券批次#{batchId}不存在！");
        }

        CouponRuleDeductInfo couponRuleDeduct;

        if (id == 0)
        {
            couponRuleDeduct = new CouponRuleDeductInfo();
        }
        else
        {
            couponRuleDeduct = session.Get<CouponRuleDeductInfo>(id);
            if (couponRuleDeduct == null)
            {
                throw new ValidationException($"指定的 优惠券抵扣规则#{id} 不存在！");
            }
        }

        CouponRuleDeductDetailModel model = new CouponRuleDeductDetailModel();

        model.CouponRuleDeductId = couponRuleDeduct.Id;
        model.BatchId = batchId;
        model.Amount = couponRuleDeduct.Amount;
        model.CanModify = batch.Status?.Released() != true;

        return View("CouponRuleDeductDetail", model);
    }

    [HttpPost]
    [Route("/admin/coupon/rule/deduct/save")]
    [Transactional]
    public IActionResult Save(CouponRuleDeductDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int couponRuleDeductId = model.CouponRuleDeductId;

        bool isCreate = couponRuleDeductId == 0;

        CouponRuleDeductInfo rule;

        if (isCreate)
        {
            rule = new CouponRuleDeductInfo();
        }
        else
        {
            rule = session.Get<CouponRuleDeductInfo>(couponRuleDeductId);
            if (rule == null)
            {
                throw new ValidationException($"指定的 优惠券抵扣规则#{couponRuleDeductId} 不存在！");
            }
        }

        CouponBatchInfo batch = session.Get<CouponBatchInfo>(model.BatchId);
        if (batch == null)
        {
            throw new ValidationException($"优惠券批次#{model.BatchId}不存在！");
        }

        if (batch.Status?.Released() == true)
        {
            throw new ValidationException($"优惠券批次已经投放，不能再修改优惠规则！");
        }

        rule.Batch = batch;
        rule.Amount = model.Amount;

        couponRuleDeductService.CheckRuleValid(rule);

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
        batch.Rule = couponRuleDeductService.GetRule(rule);
        batch.RuleDesc = couponRuleDeductService.GetRuleDesc(rule);
        session.Update(batch);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}