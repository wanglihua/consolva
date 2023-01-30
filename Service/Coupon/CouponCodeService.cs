using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Coupon;

namespace shunda.Service.Coupon;

[Service(ServiceType.Scoped)]
public class CouponCodeService {
    private readonly ISession session;

    public CouponCodeService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 优惠券编号生成，跟kotlin中的同名方法一样的
    /// </summary>
    /// <param name="batchId"></param>
    /// <param name="couponType"></param>
    /// <param name="pickSeq"></param>
    /// <returns></returns>
    public string Generate(int batchId, CouponType couponType, int pickSeq) {
        return batchId.ToString().PadLeft(3, '0') + couponType.Code + pickSeq.ToString().PadLeft(3, '0');
    }
}