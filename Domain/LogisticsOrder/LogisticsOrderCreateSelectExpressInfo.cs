namespace shunda.Domain.LogisticsOrder;

/// <summary>
/// 订单创建选择快递单号记录
/// 跟原系统中的同名类是两个意思
/// </summary>
public class LogisticsOrderCreateSelectExpressInfo
{
    /// <summary>
    /// 客户，非空
    /// </summary>
    public virtual int CustomerId { get; set; }

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    public virtual int ExpressId { get; set; }

    #region override

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is LogisticsOrderCreateSelectExpressInfo == false)
        {
            return false;
        }

        LogisticsOrderCreateSelectExpressInfo? createSelectExpressAnother = obj as LogisticsOrderCreateSelectExpressInfo;
        if (createSelectExpressAnother == null)
        {
            return false;
        }

        if (createSelectExpressAnother.CustomerId == CustomerId
            && createSelectExpressAnother.ExpressId == ExpressId)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (ExpressId == 0 || CustomerId == 0)
        {
            return base.GetHashCode();
        }

        return (CustomerId.GetHashCode() + unchecked(3 * ExpressId.GetHashCode())) & 0x7FFFFFFF;
    }

    #endregion
}