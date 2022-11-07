using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物品类型
/// </summary>
public class LogisticsGoodsType : CodeTypeBase
{
    public string Code { get; set; }

    /// <summary>
    /// 入库标签短类型
    /// </summary>
    public string CheckinLabelShortName { get; set; }

    /// <summary>
    /// 普通类
    /// </summary>
    public static readonly LogisticsGoodsType Common = new LogisticsGoodsType(1000, "普", "普通类", "【普】", "普通类");

    /// <summary>
    /// 品牌类
    /// </summary>
    public static readonly LogisticsGoodsType Brand = new LogisticsGoodsType(2000, "牌", "品牌类", "【牌】", "品牌类");

    /// <summary>
    /// 液体粉末
    /// </summary>
    public static readonly LogisticsGoodsType LiquidPowder = new LogisticsGoodsType(3000, "液", "液体粉末", "【液】", "液体粉末");

    /// <summary>
    /// 食品类
    /// </summary>
    public static readonly LogisticsGoodsType Food = new LogisticsGoodsType(4000, "食", "食品类", "【食】", "食品类");

    /// <summary>
    /// 含电类
    /// </summary>
    public static readonly LogisticsGoodsType Electric = new LogisticsGoodsType(5000, "电", "含电类", "【电】", "含电类");   

    /// <summary>
    /// 禁止类
    /// </summary>
    public static readonly LogisticsGoodsType Forbidden = new LogisticsGoodsType(6000, "禁", "禁止类", "【禁】", "禁止类");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsGoodsType None = new LogisticsGoodsType(0, "", "", "", "");

    private static readonly IReadOnlyList<LogisticsGoodsType> codeTypeListCustomer = new List<LogisticsGoodsType>() {
        Common,
        Brand,
        LiquidPowder,
        Food,
        Electric,
    };

    private static readonly IReadOnlyList<LogisticsGoodsType> codeTypeListInner = new List<LogisticsGoodsType>() {
        Common,
        Brand,
        LiquidPowder,
        Food,
        Electric,
        Forbidden,
    };

    private static readonly Dictionary<int, LogisticsGoodsType> codeTypeMap = codeTypeListInner.ToDictionary(e => e.Id);

    private static readonly Dictionary<string, LogisticsGoodsType> codeTypeMapName = codeTypeListInner.ToDictionary(e => e.Name);

    public static LogisticsGoodsType GetById(int id)
    {
        if (codeTypeMap.TryGetValue(id, out LogisticsGoodsType? codeType))
        {
            return codeType;
        }

        return None;
    }

    public static LogisticsGoodsType GetByName(string name)
    {
        if (codeTypeMapName.TryGetValue(name, out LogisticsGoodsType? codeType))
        {
            return codeType;
        }

        return None;
    }

    /// <summary>
    /// 获取客户视图列表
    /// 只获取客户可以看到的类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsGoodsType> GetCustomerList()
    {
        return codeTypeListCustomer.ToList();
    }

    /// <summary>
    /// 获取内部视图列表
    /// 获取的是全部的类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsGoodsType> GetInnerList()
    {
        return codeTypeListInner.ToList();
    }

    private LogisticsGoodsType(int id, string code, string name, string checkinLabelShortName, string description) : base(id, name, description)
    {
        this.Code = code;
        this.CheckinLabelShortName = checkinLabelShortName;
    }
}

public class LogisticsGoodsTypeHibernateType : CodeHibernateTypeBase<LogisticsGoodsType>
{
    protected override LogisticsGoodsType GetCodeTypeById(int id)
    {
        return LogisticsGoodsType.GetById(id);
    }
}

public class LogisticsGoodsTypeModelBinder : CodeTypeModelBinderBase<LogisticsGoodsType>
{
    protected override LogisticsGoodsType GetCodeTypeById(int id)
    {
        return LogisticsGoodsType.GetById(id);
    }
}