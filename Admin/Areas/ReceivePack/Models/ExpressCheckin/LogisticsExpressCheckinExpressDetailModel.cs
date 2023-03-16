using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Logistics;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;

public class LogisticsExpressCheckinExpressDetailModel : LogisticsExpressCheckinExpressSaveModel {
    /// <summary>
    /// 快递单状态，数据中还没的是None
    /// </summary>
    [BindNever]
    public ExpressStatus ExpressStatus { get; set; } = ExpressStatus.None;

    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new();

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetInnerList();

    [BindNever]
    public decimal? Length { get; set; }

    [BindNever]
    public decimal? Width { get; set; }

    [BindNever]
    public decimal? Height { get; set; }

    [BindNever]
    public decimal? Weight { get; set; }

    /// <summary>
    /// 这个是主单号尺寸的包裹数量
    /// </summary>
    public int? ExpressPackageCount { get; set; }

    [BindNever]
    public IList<ShelfInfo> ShelfList { get; set; } = new List<ShelfInfo>();

    [BindNever]
    public IList<StorageLocSpecInfo> StorageLocSpecList { get; set; } = new List<StorageLocSpecInfo>();

    [BindNever]
    public IList<ExpressRemarkPresetSelectInfo> RemarkPresetList { get; set; } = new List<ExpressRemarkPresetSelectInfo>();

    [BindNever]
    public string StorageLocName { get; set; } = string.Empty;

    [BindNever]
    public string Message { get; set; } = string.Empty;

    [BindNever]
    public ExpressExpectInfo? ExpressExpect { get; set; } = null;

    [BindNever]
    public bool AutoRecordCheckinVideo { get; set; }

    /// <summary>
    /// 子单号尺寸重量
    /// </summary>
    [BindNever]
    public IList<LogisticsExpressCheckinSizeWeightChildItem> ChildSizeWeightList { get; set; } = new List<LogisticsExpressCheckinSizeWeightChildItem>();
}