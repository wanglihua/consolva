using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class ExpressReturnFeeRateService {
    private readonly ISession session;

    public ExpressReturnFeeRateService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 获取快递退运费率
    /// </summary>
    /// <returns>每快递操作费</returns>
    /// <exception cref="ValidationException"></exception>
    public decimal GetFeeRate() {
        var expressReturnFeeRate = session.Query<ExpressReturnFeeRateInfo>()
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();
        if (expressReturnFeeRate == null) {
            throw new ValidationException("快递单退运费率未设置，请联系管理员！");
        }

        return expressReturnFeeRate.FeePerExpress;
    }

    /// <summary>
    /// 保存快递退运费率
    /// </summary>
    /// <param name="feePerExpress">每快递操作费</param>
    public void SaveFeeRate(decimal feePerExpress) {
        var expressReturnFeeRate = session.Query<ExpressReturnFeeRateInfo>()
            .OrderBy(e => e.Id)
            .Take(1)
            .SingleOrDefault();

        if (expressReturnFeeRate == null) {
            expressReturnFeeRate = new ExpressReturnFeeRateInfo();
        }

        expressReturnFeeRate.FeePerExpress = feePerExpress;

        session.SaveOrUpdate(expressReturnFeeRate);
        session.Flush();
    }
}