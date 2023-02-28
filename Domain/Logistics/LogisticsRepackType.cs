using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流订单打包类型
/// </summary>
public class LogisticsRepackType : CodeTypeBase {
    /// <summary>
    /// 直接打包, 保留全部包装
    /// </summary>
    public static readonly LogisticsRepackType DirectPack = new LogisticsRepackType(1000, "保留全部包装", "保留全部包装");

    /// <summary>
    /// 去掉快递包装盒
    /// </summary>
    public static readonly LogisticsRepackType RemoveExpressPackage = new LogisticsRepackType(2000, "去掉快递包装盒", "去掉快递包装盒");

    /// <summary>
    /// 去掉产品包装盒
    /// </summary>
    public static readonly LogisticsRepackType RemoveProductPackage = new LogisticsRepackType(3000, "去掉产品包装盒", "去掉商品包装盒");

    /// <summary>
    /// 由仓库决定
    /// </summary>
    public static readonly LogisticsRepackType FollowWarehouse = new LogisticsRepackType(4000, "由仓库决定", "由仓库决定");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsRepackType None = new LogisticsRepackType(0, "", "");

    private static readonly IReadOnlyList<LogisticsRepackType> codeTypeList = new List<LogisticsRepackType>() {
        DirectPack,
        RemoveExpressPackage,
        RemoveProductPackage,
        FollowWarehouse,
    };

    private static readonly IDictionary<int, LogisticsRepackType> codeTypeMap = codeTypeList.ToDictionary(e => e.Id);

    public static LogisticsRepackType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsRepackType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static IList<LogisticsRepackType> GetList() {
        return codeTypeList.ToList();
    }

    private LogisticsRepackType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsRepackTypeHibernateType : CodeHibernateTypeBase<LogisticsRepackType> {
    protected override LogisticsRepackType GetCodeTypeById(int id) {
        return LogisticsRepackType.GetById(id);
    }
}

public class LogisticsRepackTypeModelBinder : CodeTypeModelBinderBase<LogisticsRepackType> {
    protected override LogisticsRepackType GetCodeTypeById(int id) {
        return LogisticsRepackType.GetById(id);
    }
}