using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物品类型
/// </summary>
public class LogisticsGoodsType : CodeTypeBase {
    /// <summary>
    /// 物品类型的高低，用于确定物品类型混合时的整体物品类型
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// 简短代码
    /// </summary>
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 入库标签短类型
    /// </summary>
    public string CheckinLabelShortName { get; set; } = string.Empty;

    /// <summary>
    /// 普通类
    /// </summary>
    public static readonly LogisticsGoodsType Common = new LogisticsGoodsType {
        Id = 1000,
        Rank = 1000,
        Code = "普",
        Name = "普通类",
        CheckinLabelShortName = "【普】",
        Description = "普通类",
    };

    /// <summary>
    /// 品牌类
    /// </summary>
    public static readonly LogisticsGoodsType Brand = new LogisticsGoodsType {
        Id = 2000,
        Rank = 2000,
        Code = "牌",
        Name = "品牌类",
        CheckinLabelShortName = "【牌】",
        Description = "品牌类",
    };

    /// <summary>
    /// 食品类
    /// </summary>
    public static readonly LogisticsGoodsType Food = new LogisticsGoodsType {
        Id = 3000,
        Rank = 3000,
        Code = "食",
        Name = "食品药品",
        CheckinLabelShortName = "【食】",
        Description = "食品药品",
    };

    /// <summary>
    /// 含电类
    /// </summary>
    public static readonly LogisticsGoodsType Electric = new LogisticsGoodsType {
        Id = 4000,
        Rank = 4000,
        Code = "电",
        Name = "含电类",
        CheckinLabelShortName = "【电】",
        Description = "含电类",
    };

    /// <summary>
    /// 液体粉末
    /// </summary>
    public static readonly LogisticsGoodsType LiquidPowder = new LogisticsGoodsType {
        Id = 5000,
        Rank = 5000,
        Code = "液",
        Name = "液体粉末",
        CheckinLabelShortName = "【液】",
        Description = "液体粉末",
    };

    /// <summary>
    /// 禁止类
    /// </summary>
    public static readonly LogisticsGoodsType Forbidden = new LogisticsGoodsType {
        Id = 6000,
        Rank = 6000,
        Code = "禁",
        Name = "禁止类",
        CheckinLabelShortName = "【禁】",
        Description = "禁止类",
    };

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsGoodsType None = new LogisticsGoodsType {
        Id = 0,
        Rank = 0,
        Code = "",
        Name = "",
        CheckinLabelShortName = "",
        Description = "",
    };

    private static readonly IReadOnlyList<LogisticsGoodsType> codeTypeListCustomer = new List<LogisticsGoodsType>() {
        Common,
        Brand,
        Food,
        Electric,
        LiquidPowder,
    };

    private static readonly IReadOnlyList<LogisticsGoodsType> codeTypeListInner = new List<LogisticsGoodsType>() {
        Common,
        Brand,
        Food,
        Electric,
        LiquidPowder,
        Forbidden,
    };

    private static readonly Dictionary<int, LogisticsGoodsType> codeTypeMap = codeTypeListInner.ToDictionary(e => e.Id);

    private static readonly Dictionary<string, LogisticsGoodsType> codeTypeMapName = codeTypeListInner.ToDictionary(e => e.Name);

    public static LogisticsGoodsType GetById(int id) {
        if (codeTypeMap.TryGetValue(id, out LogisticsGoodsType? codeType)) {
            return codeType;
        }

        return None;
    }

    public static LogisticsGoodsType GetByName(string name) {
        if (codeTypeMapName.TryGetValue(name, out LogisticsGoodsType? codeType)) {
            return codeType;
        }

        return None;
    }

    /// <summary>
    /// 获取客户视图列表
    /// 只获取客户可以看到的类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsGoodsType> GetCustomerList() {
        return codeTypeListCustomer.ToList();
    }

    /// <summary>
    /// 获取内部视图列表
    /// 获取的是全部的类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsGoodsType> GetInnerList() {
        return codeTypeListInner.ToList();
    }
}

public class LogisticsGoodsTypeHibernateType : CodeHibernateTypeBase<LogisticsGoodsType> {
    protected override LogisticsGoodsType GetCodeTypeById(int id) {
        return LogisticsGoodsType.GetById(id);
    }
}

public class LogisticsGoodsTypeModelBinder : CodeTypeModelBinderBase<LogisticsGoodsType> {
    protected override LogisticsGoodsType GetCodeTypeById(int id) {
        return LogisticsGoodsType.GetById(id);
    }
}