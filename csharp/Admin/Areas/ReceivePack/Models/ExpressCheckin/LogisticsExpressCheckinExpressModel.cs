using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Express;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;

public class LogisticsExpressCheckinExpressModel
{
    #region bind never

    [BindNever]
    public WarehouseInfo Warehouse { get; set; } = new WarehouseInfo();

    [BindNever]
    public string OldExpressTrackingNo { get; set; } = string.Empty; // todo a 去掉

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
    public IList<ExpressRemarkPresetSelectInfo> RemarkList { get; set; } = new List<ExpressRemarkPresetSelectInfo>();

    [BindNever]
    public string StorageLocName { get; set; } = string.Empty;

    [BindNever]
    public string Message { get; set; } = string.Empty;

    [BindNever]
    public ExpressExpectInfo ExpressExpect { get; set; } = new ExpressExpectInfo();

    [BindNever]
    public bool AutoRecordCheckinVideo { get; set; }

    #endregion

    /// <summary>
    /// 子单号尺寸重量
    /// </summary>
    public IList<LogisticsExpressCheckinSizeWeightChildItem> ChildSizeWeightList { get; set; } = new List<LogisticsExpressCheckinSizeWeightChildItem>();

    public int ClaimId { get; set; }

    [Required(ErrorMessage = "快递入库仓库不能为空！")]
    public int? WarehouseId { get; set; }

    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    public string ExpressTrackingNo { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "快递公司长度不能大于100！")]
    public string ExpressCompany { get; set; } = string.Empty;

    [MaxLength(50, ErrorMessage = "面单客户长度不能大于50！")]
    public string BillCustomerCode { get; set; } = string.Empty;

    /// <summary>
    /// 这个是申领件的入库包裹数量
    /// </summary>
    [Required(ErrorMessage = "入库包裹数量不能为空！")]
    public int? CheckinPackageCount { get; set; }

    public bool WoodFrame { get; set; }

    public bool BarkClean { get; set; }

    [Required(ErrorMessage = "货架不能为空！")]
    public int? ShelfId { get; set; }

    [Required(ErrorMessage = "仓位规格不能为空！")]
    public int? StorageLocSpecId { get; set; }

    // [Required(ErrorMessage = "仓位不能为空！")]
    // public int? StorageLocId { get; set; }

    [MaxLength(500, ErrorMessage = "备注长度不能大于500！")]
    public string Description { get; set; } = string.Empty;

    public IList<LogisticsExpressCheckinExpressGoodsModel> GoodsList { get; set; } = new List<LogisticsExpressCheckinExpressGoodsModel>();
}