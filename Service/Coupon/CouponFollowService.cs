using shunda.Core.Service;
using shunda.Domain.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponFollowService {
    private readonly ISession session;

    public CouponFollowService(ISession session) {
        this.session = session;
    }

    public void NewFollor(int customerId) {
        // todo 判断这个客户是不是已经领取过
        
        var batchFollowList = session.Query<CouponBatchInfo>()
            .Where(e => e.GetMethod == CouponGetMethod.Follow
                        && e.Status != CouponBatchStatus.Complete)
            .ToList();

        foreach (var batch in batchFollowList) {
            int qtyPerCustomer = batch.MaxQtyPerCustomer;
            for (int index = 0; index < qtyPerCustomer; index++) {
                var coupon = new CouponInfo();
            }
        }
    }
}