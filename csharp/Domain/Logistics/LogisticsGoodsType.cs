using shunda.Core.CodeType;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物品类型
/// todo 这里看情况是不是要改造
/// </summary>
public class LogisticsGoodsType : CodeTypeBase
{
    public string Code { get; set; }

    /// <summary>
    ///  空运名称
    /// </summary>
    public string AirName { get; set; }

    /// <summary>
    /// 入库标签短类型
    /// </summary>
    public string CheckinLabelShortName { get; set; }

    /// <summary>
    /// 普通类
    /// </summary>
    public static readonly LogisticsGoodsType Common = new LogisticsGoodsType(100, "普", "普通类", "普通物品", "【普】", "普通类, 普通物品");

    /// <summary>
    /// 限制类
    /// </summary>
    public static readonly LogisticsGoodsType Restrict = new LogisticsGoodsType(200, "限", "限制类", "食品干货", "【限】", "限制类, 食品干货");

    /// <summary>
    /// 草木竹
    /// </summary>
    public static readonly LogisticsGoodsType Bamboo = new LogisticsGoodsType(300, "草", "草木竹", "特殊渠道", "【草】", "草木竹, 特殊渠道");

    /// <summary>
    /// 限制+草木
    /// </summary>
    public static readonly LogisticsGoodsType RestrictBamboo = new LogisticsGoodsType(350, "限+草", "限制+草木", "食品干货+特殊渠道", "【草】", "限制+草木, 食品干货+特殊渠道");

    /// <summary>
    /// 特货
    /// </summary>
    public static readonly LogisticsGoodsType Special = new LogisticsGoodsType(380, "特", "特货", "特货", "【特】", "特货");

    /// <summary>
    /// 禁止类
    /// </summary>
    public static readonly LogisticsGoodsType Forbidden = new LogisticsGoodsType(400, "禁", "禁止类", "禁止类", "【禁】", "禁止类");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsGoodsType None = new LogisticsGoodsType(0, "", "", "", "", "");

    public static LogisticsGoodsType GetById(int id)
    {
        IList<LogisticsGoodsType> goodsTypeList = GetInnerList();
        foreach (LogisticsGoodsType goodsTypeInfo in goodsTypeList)
        {
            if (goodsTypeInfo.Id == id)
            {
                return goodsTypeInfo;
            }
        }

        return None;
    }

    public static LogisticsGoodsType GetByName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return None;
        }

        name = name.Trim();

        IList<LogisticsGoodsType> goodsTypeList = GetInnerList();
        foreach (LogisticsGoodsType goodsTypeInfo in goodsTypeList)
        {
            if (goodsTypeInfo.Name == name)
            {
                return goodsTypeInfo;
            }
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
        IList<LogisticsGoodsType> goodsTypeList = new List<LogisticsGoodsType>();
        goodsTypeList.Add(Common);
        goodsTypeList.Add(Restrict);
        goodsTypeList.Add(Bamboo);
        goodsTypeList.Add(RestrictBamboo);
        goodsTypeList.Add(Special);

        return goodsTypeList;
    }

    /// <summary>
    /// 获取缩减了的客户视图列表
    /// 这个主要是放在运费预估那里
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsGoodsType> GetCustomerCompactList()
    {
        IList<LogisticsGoodsType> goodsTypeList = new List<LogisticsGoodsType>();
        goodsTypeList.Add(Common);
        goodsTypeList.Add(Special);

        return goodsTypeList;
    }

    /// <summary>
    /// 获取内部视图列表
    /// 获取的是全部的类型列表
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsGoodsType> GetInnerList()
    {
        IList<LogisticsGoodsType> goodsTypeList = GetCustomerList();
        goodsTypeList.Add(Forbidden); // 内部视图列表加上禁止类的

        return goodsTypeList;
    }

    /// <summary>
    /// 获取缩减了的内部列表
    /// 当前该列表也只有在物品录入那里
    /// </summary>
    /// <returns></returns>
    public static IList<LogisticsGoodsType> GetInnerCompactList()
    {
        IList<LogisticsGoodsType> goodsTypeList = new List<LogisticsGoodsType>();
        goodsTypeList.Add(Common);
        goodsTypeList.Add(Special);
        goodsTypeList.Add(Forbidden);

        return goodsTypeList;
    }

    private LogisticsGoodsType(int id, string code, string name, string airName, string checkinLabelShortName, string description) : base(id, name, description)
    {
        this.Code = code;
        this.AirName = airName;
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