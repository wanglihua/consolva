using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.Fee.Models.CouponRulePriceBreak;
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
public class CouponRulePriceBreakController : Controller
{
    private ISession session;
    private CouponRulePriceBreakService couponRulePriceBreakService;

    public CouponRulePriceBreakController(
        ISession session,
        CouponRulePriceBreakService couponRulePriceBreakService)
    {
        this.session = session;
        this.couponRulePriceBreakService = couponRulePriceBreakService;
    }

    [HttpGet]
    [Route("/admin/coupon/rule/price/break/detail")]
    public IActionResult Detail(int id, int batchId)
    {
        session.DefaultReadOnly = true;

        CouponBatchInfo batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"优惠券批次#{batchId}不存在！");
        }

        CouponRulePriceBreakInfo couponRulePriceBreak;

        if (id == 0)
        {
            couponRulePriceBreak = new CouponRulePriceBreakInfo();
        }
        else
        {
            couponRulePriceBreak = session.Get<CouponRulePriceBreakInfo>(id);
            if (couponRulePriceBreak == null)
            {
                throw new ValidationException($"指定的 优惠券满减规则#{id} 不存在！");
            }
        }

        CouponRulePriceBreakDetailModel model = new CouponRulePriceBreakDetailModel();

        model.CouponRulePriceBreakId = couponRulePriceBreak.Id;
        model.BatchId = batchId;
        model.Amount = couponRulePriceBreak.Amount;
        model.Discount = couponRulePriceBreak.Discount;
        model.CanModify = batch.Status?.Released() != true;

        return View("CouponRulePriceBreakDetail", model);
    }

    [HttpPost]
    [Route("/admin/coupon/rule/price/break/save")]
    [Transactional]
    public IActionResult Save(CouponRulePriceBreakDetailModel model)
    {
        if (!ModelState.IsValid)
        {
            throw new ValidationException(ValidationUtil.GetErrorMessages(this.ModelState));
        }

        int couponRulePriceBreakId = model.CouponRulePriceBreakId;

        bool isCreate = couponRulePriceBreakId == 0;

        CouponRulePriceBreakInfo rule;

        if (isCreate)
        {
            rule = new CouponRulePriceBreakInfo();
        }
        else
        {
            rule = session.Get<CouponRulePriceBreakInfo>(couponRulePriceBreakId);
            if (rule == null)
            {
                throw new ValidationException($"指定的 优惠券满减规则#{couponRulePriceBreakId} 不存在！");
            }
        }

        CouponBatchInfo batch = session.Get<CouponBatchInfo>(model.BatchId);

        rule.Batch = batch;
        rule.Amount = model.Amount;
        rule.Discount = model.Discount;

        couponRulePriceBreakService.CheckRuleValid(rule);

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
        batch.Rule = couponRulePriceBreakService.GetRule(rule);
        batch.RuleDesc = couponRulePriceBreakService.GetRuleDesc(rule);
        session.Update(batch);
        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }
}