using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsFeeGstService {
    private readonly ISession session;

    public LogisticsFeeGstService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 计算GST费用
    /// </summary>
    /// <param name="goodsValue">物品总价</param>
    /// <param name="fee">运费</param>
    /// <param name="gstId">GST费用的设置记录的 Id</param>
    /// <returns></returns>
    public decimal GetGstFee(decimal goodsValue, decimal fee, int gstId) {
        if (goodsValue < 0) {
            throw new ValidationException("GST计算，物品总价不能小于0！");
        }

        if (fee < 0) // 这里不能用  <= 0, 有些调用，调用时指定了 fee = 0
        {
            throw new ValidationException("GST计算，运费不能小于0！");
        }

        LogisticsGstInfo gst = session.Get<LogisticsGstInfo>(gstId);
        if (gst == null) {
            throw new Exception("指定的GST费用设置不存在！");
        }

        decimal startAmount = gst.StartAmount;
        decimal totalAmount = 0m;

        if (gst.CalcBaseType == FeeCalcBaseType.Amount) {
            totalAmount = goodsValue;
        } else if (gst.CalcBaseType == FeeCalcBaseType.Fee) {
            totalAmount = fee;
        } else if (gst.CalcBaseType == FeeCalcBaseType.FeeAndAmount) {
            totalAmount = (goodsValue + fee);
        } else {
            throw new LogisticsFeeException("指定的GST费用设置中计算基数设置存在问题！");
        }

        if (totalAmount <= startAmount) {
            return 0m;
        }

        return totalAmount * gst.FeeRate / 100;
    }
}