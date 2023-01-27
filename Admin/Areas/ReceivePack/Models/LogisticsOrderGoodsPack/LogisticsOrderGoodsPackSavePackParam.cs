using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackSavePackParam {
    public int? PackId { get; set; }

    [Required(ErrorMessage = "订单不能为空！")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "物品类型不能为空！")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    [Required(ErrorMessage = "长度不能为空！")]
    public decimal? Length { get; set; }

    [Required(ErrorMessage = "宽度不能为空！")]
    public decimal? Width { get; set; }

    [Required(ErrorMessage = "高度不能为空！")]
    public decimal? Height { get; set; }

    [Required(ErrorMessage = "重量不能为空！")]
    public decimal? Weight { get; set; }

    [Required(ErrorMessage = "箱数不能为空！")]
    public int? PackageCount { get; set; }

    public IList<string> TrackingNoList { get; set; } = new List<string>();
}