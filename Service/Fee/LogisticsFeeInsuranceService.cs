using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsFeeInsuranceService {
    private readonly ISession session;

    public LogisticsFeeInsuranceService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 计算保险费用， 一般只用于订单费用的计算
    /// </summary>
    /// <param name="goodsValue">物品总价</param>
    /// <param name="fee">运费</param>
    /// <param name="insuranceId">保险费用的设置记录的 Id</param>
    /// <returns></returns>
    public decimal GetInsuranceFee(decimal goodsValue, decimal fee, int insuranceId) {
        LogisticsInsuranceInfo insurance = session.Get<LogisticsInsuranceInfo>(insuranceId);
        if (insurance == null) {
            throw new Exception($"指定的保险费用设置#{insuranceId}不存在！");
        }

        if (insurance.Type == LogisticsInsuranceType.FixedFee) {
            return insurance.FeeRate;
        }

        if (insurance.Type == LogisticsInsuranceType.PercentFee) {
            if (insurance.CalcBaseType == FeeCalcBaseType.Amount) {
                return goodsValue * insurance.FeeRate / 100m;
            }

            if (insurance.CalcBaseType == FeeCalcBaseType.Fee) {
                return fee * insurance.FeeRate / 100m;
            }

            if (insurance.CalcBaseType == FeeCalcBaseType.FeeAndAmount) {
                return (goodsValue + fee) * insurance.FeeRate / 100m;
            }

            throw new LogisticsFeeException("指定的保险费用设置中计算基数设置存在问题！");
        }

        throw new LogisticsFeeException("指定的保险费用设置中费用类型设置存在问题！");
    }
}