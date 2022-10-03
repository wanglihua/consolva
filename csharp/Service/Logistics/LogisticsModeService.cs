using NHibernate;
using NLog;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Logistics;

namespace shunda.Service.Logistics;

[Service(ServiceType.Scoped)]
public class LogisticsModeService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private ISession session;
    // private LogisticsExpressService logisticsExpressService = null;
    // private LogisticsFeeService logisticsFeeService = null;

    public LogisticsModeService(
        ISession session
        // LogisticsExpressService logisticsExpressService,
        // LogisticsFeeService logisticsFeeService
    )
    {
        this.session = session;
        // this.logisticsExpressService = logisticsExpressService;
        // this.logisticsFeeService = logisticsFeeService;
    }

    #region 判断物流方式是不是EMS类型的物流方式

    /// <summary>
    /// 判断物流方式是不是EMS类型的物流方式
    /// todo : 通过就上一个字段的方式来实现 Channel
    /// </summary>
    /// <param name="logisticsMode"></param>
    /// <returns></returns>
    public bool IsEms(LogisticsModeInfo logisticsMode)
    {
        if (logisticsMode == null)
        {
            return false;
        }

        if (logisticsMode.Type != LogisticsModeType.CnAuExpress)
        {
            return false;
        }

        if (logisticsMode.Name?.ToLower().Contains("ems".ToLower()) != true) // 左边是可空的，只能用 != true，不能用 == false
        {
            return false;
        }

        return true;
    }

    #endregion
}