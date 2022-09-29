using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流订单打包类型
/// </summary>
public class LogisticsRepackType : CodeTypeBase
{
    /// <summary>
    /// 直接打包
    /// </summary>
    public static readonly LogisticsRepackType DirectPacking = new LogisticsRepackType(100, "直接打包：不去快递包装；外观验货，尽量合箱", "直接打包：不去快递包装；外观验货，尽量合箱");

    /// <summary>
    /// 去掉快递包装盒打包
    /// </summary>
    public static readonly LogisticsRepackType RemoveExpressPackage = new LogisticsRepackType(200, "重新打包：去掉快递包装盒；外观验货，尽量合箱", "重新打包：去掉快递包装盒；外观验货，尽量合箱");

    /// <summary>
    /// 去掉产品包装盒打包
    /// </summary>
    public static readonly LogisticsRepackType RemoveProductPackage = new LogisticsRepackType(300, "重新打包：去掉商品包装盒；外观验货，合箱", "重新打包：去掉商品包装盒；外观验货，合箱");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsRepackType None = new LogisticsRepackType(0, "", "");

    private static readonly IList<LogisticsRepackType> codeTypeList = new List<LogisticsRepackType>() {
        DirectPacking, 
        RemoveExpressPackage, 
        RemoveProductPackage
    };    

    private static readonly IDictionary<int, LogisticsRepackType> LogisticsRepackTypeDictionary = new Dictionary<int, LogisticsRepackType>() {
        { DirectPacking.Id, DirectPacking }, { RemoveExpressPackage.Id, RemoveExpressPackage }, { RemoveProductPackage.Id, RemoveProductPackage }
    };

    public static LogisticsRepackType GetById(int id)
    {
        if (LogisticsRepackTypeDictionary.ContainsKey(id))
        {
            return LogisticsRepackTypeDictionary[id];
        }

        return None;
    }



    public static IList<LogisticsRepackType> GetList()
    {
        return codeTypeList;
    }

    private LogisticsRepackType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsRepackTypeHibernateType : CodeHibernateTypeBase<LogisticsRepackType>
{
    protected override LogisticsRepackType GetCodeTypeById(int id)
    {
        return LogisticsRepackType.GetById(id);
    }
}

public class LogisticsRepackTypeModelBinder : CodeTypeModelBinderBase<LogisticsRepackType>
{
    protected override LogisticsRepackType GetCodeTypeById(int id)
    {
        return LogisticsRepackType.GetById(id);
    }
}