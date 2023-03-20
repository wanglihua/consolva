using NHibernate;
using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;
using shunda.Domain.Customer;
using shunda.Service.Customer;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponBatchService {
    private readonly ISession session;
    private readonly CouponCodeService couponCodeService;
    private readonly CouponRuleDeductService couponRuleDeductService;
    private readonly CouponRuleDiscountService couponRuleDiscountService;
    private readonly CouponRulePriceBreakService couponRulePriceBreakService;
    private readonly CustomerVipDiscountService customerVipDiscountService;

    public CouponBatchService(
        ISession session,
        CouponCodeService couponCodeService,
        CouponRuleDeductService couponRuleDeductService,
        CouponRuleDiscountService couponRuleDiscountService,
        CouponRulePriceBreakService couponRulePriceBreakService,
        CustomerVipDiscountService customerVipDiscountService) {
        this.session = session;
        this.couponCodeService = couponCodeService;
        this.couponRuleDeductService = couponRuleDeductService;
        this.couponRuleDiscountService = couponRuleDiscountService;
        this.couponRulePriceBreakService = couponRulePriceBreakService;
        this.customerVipDiscountService = customerVipDiscountService;
    }

    public bool HasBatchToPick(int customerId) {
        // 检查客服所在VIP级别是否支持优惠券
        if (customerVipDiscountService.CustomerCanCoupon(customerId) == false) {
            return false;
        }

        CheckAndSetExpire();

        // DateTime today = DateTime.Today;
        // var batchId = session.Query<CouponBatchInfo>()
        //     .Where(e => e.Status == CouponBatchStatus.Release // 投放状态
        //                 && e.Expire >= today) // 未过有效期
        //     .OrderByDescending(e => e.Id)
        //     .Select(e => e.Id)
        //     .Take(1)
        //     .SingleOrDefault();
        //
        // return batchId != 0;

        DateTime today = DateTime.Today;
        var batchList = session.Query<CouponBatchInfo>()
            .Where(e => e.Status == CouponBatchStatus.Release
                        && e.Expire >= today)
            .ToList();

        // 获取这些批次中，登录客户领取的优惠券
        var batchIdList = batchList.Select(e => e.Id).ToList();
        var couponPickList = session.Query<CouponInfo>()
            .Where(e => e.Customer.Id == customerId
                        && e.GetMethod == CouponGetMethod.Pick
                        && batchIdList.Contains(e.Batch.Id)
            )
            .ToList();

        // 各个批次已领取的数量
        var batchPickCountDict = new Dictionary<int, int>();
        foreach (var coupon in couponPickList) {
            int batchId = coupon.Batch.Id;
            if (batchPickCountDict.TryGetValue(batchId, out int pickCount) == false) {
                batchPickCountDict[batchId] = 1;
            } else {
                batchPickCountDict[batchId] = pickCount + 1;
            }
        }

        foreach (var batch in batchList) {
            int batchId = batch.Id;
            if (batchPickCountDict.TryGetValue(batchId, out int pickCount) == false // 批次中客户还没领取
                || pickCount < batch.MaxQtyPerCustomer // 批次中客户还没领完
               ) {
                return true;
            }
        }

        return false;
    }

    public void CheckRuleExist(int batchId) {
        CheckRuleExistValid(batchId, checkValid: false, adminCheck: true);
    }

    public void CheckRuleValid(int batchId) {
        CheckRuleExistValid(batchId, checkValid: true, adminCheck: true);
    }

    private void CheckRuleExistValid(int batchId, bool checkValid, bool adminCheck = true) {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null) {
            throw new ValidationException($"指定优惠券批次#{batchId}不存在！");
        }

        if (batch.Type == CouponType.Deduct) {
            var rule = session.Query<CouponRuleDeductInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null) {
                if (adminCheck == true) {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请先编辑优惠规则！");
                } else {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请联系管理员！");
                }
            }

            // 检查合法性
            if (checkValid) {
                couponRuleDeductService.CheckRuleValid(rule);
            }
        } else if (batch.Type == CouponType.Discount) {
            var rule = session.Query<CouponRuleDiscountInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null) {
                if (adminCheck == true) {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请先编辑优惠规则！");
                } else {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请联系管理员！");
                }
            }

            // 检查合法性
            if (checkValid) {
                couponRuleDiscountService.CheckRuleValid(rule);
            }
        } else if (batch.Type == CouponType.PriceBreak) {
            var rule = session.Query<CouponRulePriceBreakInfo>()
                .Where(e => e.Batch.Id == batchId)
                .Take(1)
                .SingleOrDefault();
            if (rule == null) {
                if (adminCheck == true) {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请先编辑优惠规则！");
                } else {
                    throw new ValidationException($"指定优惠券批次#{batchId}优惠规则不存在，请联系管理员！");
                }
            }

            // 检查合法性
            if (checkValid) {
                couponRulePriceBreakService.CheckRuleValid(rule);
            }
        } else {
            throw new Exception($"优惠券批次#{batchId}类型未知！");
        }
    }

    public void CheckAndSetExpire() {
        CheckAndSetExpire(-1);
    }

    /// <summary>
    /// 检查批次数据是否过期，并设置正确的过期状态
    /// 一般是在展示的时候调用，领取的时候发现时间过期时也可以调用
    /// <param name="batchId">需要检查的批次，如果batchId不大于0，就表示检查所有批次</param>
    /// </summary>
    public void CheckAndSetExpire(int batchId) {
        var now = DateTime.Now;
        var today = now.Date;

        IList<CouponBatchStatus> mayExpireStatusList = CouponBatchStatus.GetMayExpireStatusList();

        var expireBatchQuery = session.Query<CouponBatchInfo>()
            .Where(e => e.Expire < today // 已过期 
                        && mayExpireStatusList.Contains(e.Status)); // 不是新创建和已完结的状态
        if (batchId > 0) {
            expireBatchQuery = expireBatchQuery.Where(e => e.Id == batchId);
        }

        IList<CouponBatchInfo> expireBatchList = expireBatchQuery.ToList();

        IList<int> expireBatchIdList = expireBatchList.Select(e => e.Id).ToList();
        if (expireBatchIdList != null && expireBatchIdList.Count != 0) {
            foreach (CouponBatchInfo batch in expireBatchList) {
                batch.Status = CouponBatchStatus.Expire;

                session.Update(batch);
            }

            session.Flush();

            IList<CouponInfo> expireCouponList = session.Query<CouponInfo>()
                .Where(e => expireBatchIdList.Contains(e.Batch.Id) // 已过期，，且批次不是新创建和已完结的状态
                            && e.Status == CouponStatus.Pick) // 未使用，只是领取创建了
                .ToList();
            foreach (CouponInfo coupon in expireCouponList) {
                coupon.Status = CouponStatus.Expire;

                session.Update(coupon);
            }

            session.Flush();
        }

        // 这个这里就不设置了，领取的时候设置这个 已领取状态 就可以了
        // var pickedBatchListQuery = session.Query<CouponBatchInfo>()
        //     .Where(e => e.Status == CouponBatchStatus.Release
        //                 && e.PickQuantity >= e.TotalQuantity);
        //
        // if (batchId > 0)
        // {
        //     pickedBatchListQuery = pickedBatchListQuery.Where(e => e.Id == batchId);
        // }
        //
        // IList<CouponBatchInfo> pickedBatchList = pickedBatchListQuery.ToList();
        //
        // foreach (CouponBatchInfo batch in pickedBatchList)
        // {
        //     batch.Status = CouponBatchStatus.Picked;
        //
        //     session.Update(batch);
        // }
        //
        // session.Flush();
    }

    /// <summary>
    /// 检查批次数据是否已领完，并设置正确的领完状态
    /// 同时也更新了领取数量
    /// 一般是在领取的时候调用的
    /// </summary>
    public void CheckAndSetPicked(CouponBatchInfo batch, int pickedCount) {
        if (pickedCount >= batch.TotalQuantity) {
            batch.PickQuantity = pickedCount; // 更新数量

            if (batch.Status == CouponBatchStatus.Release) {
                batch.Status = CouponBatchStatus.Picked; // 更新状态
            }

            session.Update(batch);
            session.Flush();
        }
    }

    public void SumAndSaveBatchTotalPicked(int batchId) {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null) {
            throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
        }

        var batchCouponPickedCount = session.Query<CouponInfo>()
            .Where(e => e.Batch.Id == batchId)
            .Select(e => e.Id)
            .Count();

        batch.PickQuantity = batchCouponPickedCount;
        session.Update(batch);

        session.Flush();
    }

    public void SumAndSaveBatchTotalUsed(int batchId) {
        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null) {
            throw new ValidationException($"指定的优惠券批次#{batchId}不存在！");
        }

        var batchCouponUsedCount = session.Query<CouponInfo>()
            .Where(e => e.Batch.Id == batchId
                        && e.Status == CouponStatus.Use)
            .Select(e => e.Id)
            .Count();

        batch.UseQuantity = batchCouponUsedCount;
        session.Update(batch);

        session.Flush();
    }

    public void Pick(int batchId, int customerId) {
        DateTime now = DateTime.Now;
        DateTime today = DateTime.Today;

        var batch = session.Get<CouponBatchInfo>(batchId);
        if (batch == null) {
            throw new ValidationException($"指定的优惠券批次#${batchId}不存在！");
        }

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        // 判断批次是否有效
        if (batch.Status == CouponBatchStatus.New) {
            throw new ValidationException($"指定的优惠券批次 {batch.Name} 未投放！");
        }

        if (batch.Status == CouponBatchStatus.Picked) {
            throw new ValidationException($"指定的优惠券批次 {batch.Name} 已领取完！");
        }

        if (batch.Status == CouponBatchStatus.Complete) {
            throw new ValidationException($"指定的优惠券批次 {batch.Name} 已完结！");
        }

        if (batch.Status == CouponBatchStatus.Expire) {
            throw new ValidationException($"指定的优惠券批次 {batch.Name} 已过期！");
        }

        if (batch.Expire < today) {
            CheckAndSetExpire(batchId);
            throw new ValidationException($"指定的优惠券批次 {batch.Name} 已过期！");
        }

        // 检查客服所在VIP级别是否支持优惠券
        if (customerVipDiscountService.CustomerCanCoupon(customerId) == false) {
            throw new ValidationException($"客户#{customerId}VIP级别不支持使用优惠券！");
        }

        // 判断批次是否已经抢完，这是当前会员抢之前的检查和设置
        int batchPickedCount = session.Query<CouponInfo>()
            .Where(e => e.Batch.Id == batchId)
            .Count();
        if (batchPickedCount >= batch.TotalQuantity) {
            CheckAndSetPicked(batch, batchPickedCount);
            throw new ValidationException($"指定的优惠券批次 {batch.Name} 已领取完！");
        }

        // 判断会员当前是不是还可以继续抢券，会员在这个批次中抢的数量是不是超过了可抢数量，后台发放的不算
        int customerPickedCount = session.Query<CouponInfo>()
            .Where(e => e.Batch.Id == batchId
                        && e.Customer.Id == customerId
                        && e.GetMethod == CouponGetMethod.Pick) // 只计算会员领取的
            .Count();
        if (customerPickedCount >= batch.MaxQtyPerCustomer) {
            throw new ValidationException($"该批次最多可领{batch.MaxQtyPerCustomer}张，您已领取！");
        }

        var coupon = new CouponInfo {
            Batch = batch,
            Code = Guid.NewGuid().ToString("N"), // 后面会做修正
            Customer = customer,
            PickTime = now,
            GetMethod = CouponGetMethod.Pick,
            Status = CouponStatus.Pick,
        };

        session.Save(coupon);
        session.Flush();

        // 记录日志
        var couponLog = new CouponLogInfo {
            Coupon = coupon,
            Type = CouponLogType.Pick,
            Description = string.Empty,
        };
        session.Save(couponLog);

        coupon.Code = couponCodeService.Generate(batchId, batch.Type, coupon.Id);
        session.Update(coupon);
        session.Flush();

        // 更新批次已认领数量，不更新其实也可以的，后面的 CheckAndSetPicked 里面也会更新这个的。
        batch.PickQuantity = batchPickedCount + 1;
        session.Update(batch);
        session.Flush();

        // 检查并按需要设置批次已经领取完状态，这是当前会员抢之后的检查和设置
        CheckAndSetPicked(batch, batchPickedCount + 1); // 每次抢一张，抢了之后再判断是不是抢完了
    }
}