using shunda.Core.Service;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Service.LogisticsOrder;

/// <summary>
/// 物流订单头程 Excel 服务类
/// </summary>
[Service(ServiceType.Scoped)]
public class LogisticsOrderFirstModeExcelService {
    private readonly ISession session;

    public LogisticsOrderFirstModeExcelService(ISession session) {
        this.session = session;
    }

    /// <summary>
    /// 导出Excel用于国际单号导入
    /// </summary>
    /// <param name="expressList"></param>
    /// <returns></returns>
    public byte[] Export(IList<LogisticsOrderFirstModeInfo> expressList) {
        return new byte[] { };
    }
}