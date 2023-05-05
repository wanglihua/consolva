using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsOrderFeeSumService {
    private readonly ISession session;

    public LogisticsOrderFeeSumService(ISession session) {
        this.session = session;
    }

    public LogisticsOrderFeeSumData GetSumData(int orderId) {
        var orderFeeList = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        foreach (var orderFee in orderFeeList) {
            if (orderFee.OrderFirstMode != null) {
                NHibernateUtil.Initialize(orderFee.OrderFirstMode);
                NHibernateUtil.Initialize(orderFee.OrderFirstMode.FirstMode);
            }
        }

        decimal totalAmount = 0.0m;

        decimal totalAmountWithoutDiscount = 0.0m;
        decimal totalAmountWithoutCouponDiscount = 0.0m;
        decimal vipDiscount = 0.0m;
        foreach (var fee in orderFeeList) {
            if (fee.Type != LogisticsOrderFeeType.CouponDiscountFee) {
                totalAmountWithoutCouponDiscount += fee.Fee;
            }

            if (fee.Type != LogisticsOrderFeeType.CouponDiscountFee
                && fee.Type != LogisticsOrderFeeType.VipDiscount
                && fee.Type != LogisticsOrderFeeType.SuperVipDiscount) {
                totalAmountWithoutDiscount += fee.Fee;
            }

            if (fee.Type == LogisticsOrderFeeType.VipDiscount
                || fee.Type == LogisticsOrderFeeType.SuperVipDiscount) {
                vipDiscount += fee.Fee;
            }

            totalAmount += fee.Fee;
        }

        decimal firstModeFee = 0.0m;
        decimal firstModeFeeCouponType = 0.0m;
        var couponFeeTypeList = LogisticsOrderFeeType.GetCouponFeeTypeLList();
        foreach (var fee in orderFeeList) {
            if (couponFeeTypeList.Contains(fee.Type)) {
                firstModeFee += fee.Fee;

                if (fee.OrderFirstMode != null && fee.OrderFirstMode.FirstMode.Promotion == false) {
                    firstModeFeeCouponType += fee.Fee;
                }
            }
        }

        return new LogisticsOrderFeeSumData {
            TotaAmount = totalAmount,
            TotalAmountWithoutDiscount = totalAmountWithoutDiscount,
            VipDiscountAmount = Math.Abs(vipDiscount),
            TotalAmountWithoutCouponDiscount = totalAmountWithoutCouponDiscount,
            FirstModeFee = firstModeFee,
            FirstModeFeeCouponType = firstModeFeeCouponType,
        };
    }
}