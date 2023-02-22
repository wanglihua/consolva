using shunda.Core;
using shunda.Core.Util;

namespace shunda.Domain.Logistics;

/// <summary>
/// 快递具体费率明细
/// </summary>
public class LogisticsFeeRateExpressEntryInfo : Entity {
    /// <summary>
    /// 快递具体费率，非空
    /// </summary>
    public virtual LogisticsFeeRateExpressInfo FeeRateExpress { get; set; } = new LogisticsFeeRateExpressInfo();

    /// <summary>
    /// 开始重量，非空
    /// </summary>
    public virtual decimal BeginWeight { get; set; } = 0m;

    /// <summary>
    /// 结束重量，非空
    /// </summary>
    public virtual decimal EndWeight { get; set; } = 0m;

    /// <summary>
    /// 是否按公斤计费，非空
    /// </summary>
    public virtual bool FeeByKilogram { get; set; } = false;

    public override void Validate() {
        ValidationUtil.ValidateEntity<LogisticsFeeRateExpressEntryInfoValidator,  LogisticsFeeRateExpressEntryInfo>(this);
    }
}