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

    /// <summary>
    /// 是否合箱
    /// </summary>
    public bool? MultiExpressOnePack { get; set; } // 后来这个值不传了，所有就总是null了

    public IList<string> ExpressTrackingNoList { get; set; } = new List<string>();

    public bool ContinueThoseExpress { get; set; }
}