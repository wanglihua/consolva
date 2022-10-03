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

    #region 检查物流模式的设置是否正确

    /// <summary>
    /// 检查物流模式的设置是否正确
    /// </summary>
    /// <param name="logisticsModeInfo"></param>
    /// <param name="innerMode">是不是内部计费物流方式</param>
    /// <returns></returns>
    public CheckResult ValidateLogisticsMode(LogisticsModeInfo logisticsModeInfo, bool innerMode = false)
    {
        if (logisticsModeInfo.Type.Id == 0)
        {
            return new CheckResult(true, "");
        }

        if (logisticsModeInfo.Type == LogisticsModeType.CnAuSea || logisticsModeInfo.Type == LogisticsModeType.CnAuAir || logisticsModeInfo.Type == LogisticsModeType.CnAuExpress)
        {
            if (logisticsModeInfo.CommonFeeRate == null && logisticsModeInfo.RestrictFeeRate == null && logisticsModeInfo.BambooFeeRate == null && logisticsModeInfo.SpecialFeeRate == null)
            {
                return new CheckResult(false, "普通类、限制类、草木竹类、特货类通用费率至少一个不能为空！");
            }
        }
        else if (logisticsModeInfo.Type == LogisticsModeType.HomeDelivery || logisticsModeInfo.Type == LogisticsModeType.TerminalPickup)
        {
            if (logisticsModeInfo.InnerFeeRate == null)
            {
                return new CheckResult(false, "内部通用费率不能为空！");
            }

            if (logisticsModeInfo.CustomerFeeRate == null)
            {
                return new CheckResult(false, "客户端通用费率不能为空！");
            }
        }
        else
        {
            if (logisticsModeInfo.FeeRate == null)
            {
                return new CheckResult(false, "通用费率不能为空！");
            }
        }

        if (innerMode == false)
        {
            if (logisticsModeInfo.Type != LogisticsModeType.TerminalPickup)
            {
                if (logisticsModeInfo.WarehouseFrom == null || logisticsModeInfo.WarehouseFrom.Id == 0)
                {
                    return new CheckResult(false, "发货仓库不能为空！");
                }
            }

            if (logisticsModeInfo.Type == LogisticsModeType.CnAuSea
                || logisticsModeInfo.Type == LogisticsModeType.CnAuAir)
            {
                // 检查发货仓库类型
                if (logisticsModeInfo.WarehouseFrom.Type != WarehouseType.Cn)
                {
                    return new CheckResult(false, "发货仓库应为中国仓库！");
                }

                // 到货仓库检查
                if (logisticsModeInfo.WarehouseTo == null || logisticsModeInfo.WarehouseTo.Id == 0)
                {
                    return new CheckResult(false, "到货仓库不能为空！");
                }

                // 检查到货仓库类型
                if (logisticsModeInfo.WarehouseTo.Type != WarehouseType.Au)
                {
                    return new CheckResult(false, "到货仓库应为澳洲仓库！");
                }
            }

            if (logisticsModeInfo.Type == LogisticsModeType.AuExpress)
            {
                // 检查发货仓库类型
                if (logisticsModeInfo.WarehouseFrom.Type != WarehouseType.Au)
                {
                    return new CheckResult(false, "发货仓库应为澳洲仓库！");
                }

                if (logisticsModeInfo.WarehouseTo != null && logisticsModeInfo.WarehouseTo.Id != 0)
                {
                    return new CheckResult(false, "到货仓库应该为空！");
                }
            }

            if (logisticsModeInfo.Type == LogisticsModeType.WarehousePickup
                || logisticsModeInfo.Type == LogisticsModeType.HomeDelivery)
            {
                // 检查发货仓库类型
                if (logisticsModeInfo.WarehouseFrom.Type != WarehouseType.Au)
                {
                    return new CheckResult(false, "仓库应为澳洲仓库！");
                }
            }
            
        }

        return new CheckResult(true, "");
    }

    #endregion

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