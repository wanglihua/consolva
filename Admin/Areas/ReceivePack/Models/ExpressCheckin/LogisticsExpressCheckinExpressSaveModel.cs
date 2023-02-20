using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;

public class LogisticsExpressCheckinExpressSaveModel {
    public int ExpressId { get; set; }

    [Required(ErrorMessage = "快递入库仓库不能为空！")]
    public int WarehouseId { get; set; }

    [Required(ErrorMessage = "快递单号不能为空！")]
    [MaxLength(30, ErrorMessage = "快递单号长度不能大于30！")]
    public string TrackingNo { get; set; } = string.Empty;

    [MaxLength(100, ErrorMessage = "快递公司长度不能大于100！")]
    public string ExpressCompany { get; set; } = string.Empty;

    [Required(ErrorMessage = "面单客户不能为空！")]
    [MaxLength(50, ErrorMessage = "面单客户长度不能大于50！")]
    public string BillCustomerCode { get; set; } = string.Empty;

    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 这个是申领件的入库包裹数量
    /// </summary>
    [Required(ErrorMessage = "入库包裹数量不能为空！")]
    public int? PackageCount { get; set; }

    [Required(ErrorMessage = "货架不能为空！")]
    public int? ShelfId { get; set; }

    [Required(ErrorMessage = "仓位规格不能为空！")]
    public int? StorageLocSpecId { get; set; }

    // [Required(ErrorMessage = "仓位不能为空！")]
    // public int? StorageLocId { get; set; }

    [MaxLength(300, ErrorMessage = "备注长度不能大于300！")]
    public string? WarehouseRemark { get; set; }

    [Required(ErrorMessage = "物品列表不能为空！")]
    public IList<LogisticsExpressCheckinExpressGoodsModel> GoodsList { get; set; } = new List<LogisticsExpressCheckinExpressGoodsModel>();
}