using shunda.Domain;
using shunda.Domain.Logistics;

namespace shunda.Service.Fee;

// todo LogisticsMode 已经放入这个类中，用到这个类又用到物流方式那里，就不用多一个参数了

// todo 加上这个数据结构的数据验证器

/// <summary>
/// 运费计算时需要的数据
/// </summary>
public class LogisticsFeeData {
    public Country Country { get; set; } = Country.None;

    public LogisticsModeInfo LogisticsMode { get; set; } = new();

    public string Address { get; set; } = string.Empty;

    public string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// 货值
    /// </summary>
    public decimal GoodsValue { get; set; }

    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    public IList<LogisticsFeePack> FeePackList { get; set; } = new List<LogisticsFeePack>();

    /// <summary>
    /// 包裹数量，快递单号数量
    /// </summary>
    public int PackageCount { get; set; }
}

/// <summary>
/// 运费计算中的物品信息
/// </summary>
public class LogisticsFeePack {
    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }

    /// <summary>
    /// 数量，默认为 1
    /// </summary>
    public int PackageCount { get; set; } = 1;
}