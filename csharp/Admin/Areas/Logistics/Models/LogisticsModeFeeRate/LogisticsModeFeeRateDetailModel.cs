using System.ComponentModel.DataAnnotations;
using shunda.Domain.Logistics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeFeeRate;

public class LogisticsModeFeeRateDetailModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int LogisticsModeFeeRateId { get; set; }

    /// <summary>
    /// 物流方式，非空
    /// </summary>
    [Required(ErrorMessage = "物流方式不能为空！")]
    public int ModeId { get; set; } = 0;

    /// <summary>
    /// 物品类型，非空
    /// </summary>
    [Required(ErrorMessage = "物品类型不能为空！")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 通用费率，非空
    /// </summary>
    [Required(ErrorMessage = "通用费率不能为空！")]
    public int FeeRateId { get; set; } = 0;

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = new List<LogisticsGoodsType>();

}