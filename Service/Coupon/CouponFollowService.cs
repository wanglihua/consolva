using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Coupon;
using shunda.Domain.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponFollowService {
    private readonly ISession session;
    private readonly CouponCodeService couponCodeService;

    public CouponFollowService(
        ISession session,
        CouponCodeService couponCodeService
    ) {
        this.session = session;
        this.couponCodeService = couponCodeService;
    }

    public void NewFollor(int customerId) {
        var couponGot = session.Query<CouponInfo>()
            .Where(e => e.Customer.Id == customerId
                        && e.GetMethod == CouponGetMethod.Follow)
            .Take(1)
            .SingleOrDefault();

        // 说明之前领取过关注赠送优惠券，这次就不送了
        if (couponGot != null) {
            return;
        }

        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        DateTime now = DateTime.Now;

        var batchFollowList = session.Query<CouponBatchInfo>()
            .Where(e => e.GetMethod == CouponGetMethod.Follow
                        && e.Status != CouponBatchStatus.Complete)
            .ToList();

        var couponList = new List<CouponInfo>();
        foreach (var batch in batchFollowList) {
            int qtyPerCustomer = batch.MaxQtyPerCustomer;
            for (int index = 0; index < qtyPerCustomer; index++) {
                var coupon = new CouponInfo {
                    Batch = batch,
                    Code = Guid.NewGuid().ToString("N"), // 后面会做修正
                    Customer = customer,
                    PickTime = now,
                    GetMethod = CouponGetMethod.Pick,
                    Status = CouponStatus.Pick,
                };

                session.Save(coupon);

                couponList.Add(coupon);
            }
        }

        session.Flush();

        foreach (var coupon in couponList) {
            coupon.Code = couponCodeService.Generate(coupon.Batch.Id, coupon.Batch.Type, coupon.Id);
            session.Update(coupon);
        }

        foreach (var coupon in couponList) {
            // 记录日志
            var couponLog = new CouponLogInfo {
                Coupon = coupon,
                Type = CouponLogType.Follow,
                Description = string.Empty,
            };
            session.Save(couponLog);
        }        

        session.Flush();
    }
}