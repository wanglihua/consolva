using NHibernate;
using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponBatchService
{
    private ISession session;
    private CouponRuleDeductService couponRuleDeductService;
    private CouponRuleDiscountService couponRuleDiscountService;
    private CouponRulePriceBreakService couponRulePriceBreakService;

    public CouponBatchService(
        ISession session,
        CouponRuleDeductService couponRuleDeductService,
        CouponRuleDiscountService couponRuleDiscountService,
        CouponRulePriceBreakService couponRulePriceBreakService)
    {
        this.session = session;
        this.couponRuleDeductService = couponRuleDeductService;
        this.couponRuleDiscountService = couponRuleDiscountService;
        this.couponRulePriceBreakService = couponRulePriceBreakService;
    }

    public void CheckRuleExist(int batchId)
    {
        CheckRuleExistValid(batchId, checkValid: false, adminCheck: true);
    }

    public void CheckRuleValid(int batchId)
    {
        CheckRuleExistValid(batchId, checkValid: true, adminCheck: true);
    }

    private void CheckRuleExistValid(int batchId, bool checkValid, bool adminCheck = true)
    {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"指定优惠券批次#{batchId}不存在！");
        }

        if (batch.Type == CouponType.Deduct)
        {
            var rule = session.Query<CouponRuleDeductInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null)
            {
                if (adminCheck == true)
                {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请先编辑优惠规则！");
                }
                else
                {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请联系管理员！");
                }
            }

            // 检查合法性
            couponRuleDeductService.CheckRuleValid(rule);
        }
        else if (batch.Type == CouponType.Discount)
        {
            var rule = session.Query<CouponRuleDiscountInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null)
            {
                if (adminCheck == true)
                {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请先编辑优惠规则！");
                }
                else
                {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请联系管理员！");
                }
            }

            // 检查合法性
            couponRuleDiscountService.CheckRuleValid(rule);
        }
        else if (batch.Type == CouponType.PriceBreak)
        {
            var rule = session.Query<CouponRulePriceBreakInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null)
            {
                if (adminCheck == true)
                {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请先编辑优惠规则！");
                }
                else
                {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请联系管理员！");
                }
            }

            // 检查合法性
            couponRulePriceBreakService.CheckRuleValid(rule);
        }
        else
        {
            throw new Exception($"优惠券批次#{batchId}类型未知！");
        }
    }

    /// <summary>
    /// 跟kotlin里面的同名方法逻辑一致的
    /// </summary>
    public void CheckAndSetStatus()
    {
        var now = DateTime.Now;
        var today = now.Date;

        IList<CouponBatchStatus> mayExpireStatusList = CouponBatchStatus.GetMayExpireStatusList();
        IList<int> expireBatchIdList = session.Query<CouponBatchInfo>()
            .Where(e => e.Expire < today // 已过期 
                        && mayExpireStatusList.Contains(e.Status)) // 不是新创建和已完结的状态
            .Select(e => e.Id)
            .ToList();

        if (expireBatchIdList != null && expireBatchIdList.Count != 0)
        {
            session.Query<CouponBatchInfo>()
                .Where(e => expireBatchIdList.Contains(e.Id)) // 已过期，且不是新创建和已完结的状态
                .UpdateBuilder()
                .Set(e => e.Status, CouponBatchStatus.Expire)
                .Set(e => e.UpdateTime, now)
                .Update();


            session.Query<CouponInfo>()
                .Where(e => expireBatchIdList.Contains(e.Batch.Id) // 已过期，，且批次不是新创建和已完结的状态
                            && e.Status == CouponStatus.Pick) // 未使用，只是领取创建了
                .UpdateBuilder()
                .Set(e => e.Status, CouponStatus.Expire)
                .Set(e => e.UpdateTime, now)
                .Update();
        }

        session.Query<CouponBatchInfo>()
            .Where(e => e.Status == CouponBatchStatus.Release
                        && e.PickQuantity >= e.TotalQuantity
            )
            .UpdateBuilder()
            .Set(e => e.Status, CouponBatchStatus.Picked)
            .Update();
    }

    public void SumAndSaveBatchTotalPicked(int batchId)
    {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
        }

        var batchCouponPickedCount = session.Query<CouponInfo>()
            .Where(e => e.Batch.Id == batchId
                        && e.Status == CouponStatus.Pick)
            .Select(e => e.Id)
            .Count();

        /*
         采用这种更新方式，应该是不会立即把数据库中的值反映到实体属性值中
        session.Query<CouponBatchInfo>()
            .Where(e => e.Id == batchId)
            .UpdateBuilder()
            .Set(e => e.PickQuantity, batchCouponPickedCount)
            .Update();
            
         */

        batch.PickQuantity = batchCouponPickedCount;
        session.Update(batch);

        session.Flush();
    }

    public void SumAndSaveBatchTotalUsed(int batchId)
    {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null)
        {
            throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
        }

        var batchCouponUsedCount = session.Query<CouponInfo>()
            .Where(e => e.Batch.Id == batchId
                        && e.Status == CouponStatus.Use)
            .Select(e => e.Id)
            .Count();

        /*
         采用这种更新方式，应该是不会立即把数据库中的值反映到实体属性值中
        session.Query<CouponBatchInfo>()
            .Where(e => e.Id == batchId)
            .UpdateBuilder()
            .Set(e => e.UseQuantity, batchCouponUsedCount)
            .Update();
         */

        batch.UseQuantity = batchCouponUsedCount;
        session.Update(batch);

        session.Flush();
    }
}