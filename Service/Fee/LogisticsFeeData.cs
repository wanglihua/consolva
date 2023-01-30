using shunda.Domain.Logistics;

namespace shunda.Service.Fee;

/// <summary>
/// 运费计算时需要的数据
/// </summary>
public class LogisticsFeeData {
    public string Address { get; set; } = string.Empty;

    public string Zipcode { get; set; } = string.Empty;

    public decimal TotalPrice { get; set; }

    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    public IList<LogisticsFeePack> FeePackList { get; set; } = new List<LogisticsFeePack>();

    /// <summary>
    /// 快递单体积列表，包括子单号，单位立方
    /// 现阶段可以只包含打木架和清理树皮杂物的快递单
    /// </summary>
    public IList<LogisticsFeeExpress> FeeExpressList { get; set; } = new List<LogisticsFeeExpress>();

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
    public int Quantity { get; set; } = 1;
}

public class LogisticsFeeExpress {
    public string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 单位立方
    /// </summary>
    public decimal Volume { get; set; }

    /// <summary>
    ///打木架 
    /// </summary>
    [Obsolete]
    public bool WoodFrame { get; set; }

    /// <summary>
    /// 清理树皮杂物 
    /// </summary>
    [Obsolete]
    public bool BarkClean { get; set; }

    /// <summary>
    /// 箱数
    /// </summary>
    public int PackageCount { get; set; } = 1;
}