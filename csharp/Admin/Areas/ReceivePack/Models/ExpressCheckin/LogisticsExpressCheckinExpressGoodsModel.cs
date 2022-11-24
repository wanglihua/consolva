using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;

public class LogisticsExpressCheckinExpressGoodsModel
{
    public string UiIndex { get; set; } = string.Empty;

    /// <summary>
    /// LogisticsClaimGoodsEntryInfo 的 Id
    /// todo a 这个修改
    /// </summary>
    public int EntryId { get; set; }

    /// <summary>
    /// LogisticsGoodsInfo 的 Id
    /// </summary>
    [Required(ErrorMessage = "物品扩展名不能为空！")]
    public int GoodsId { get; set; }

    /// <summary>
    /// 物品名称
    /// 只是用于显示
    /// </summary>
    [BindNever]
    public string GoodsName { get; set; } = string.Empty;

    [Required(ErrorMessage = "数量不能为空！")]
    public int Quantity { get; set; }

    [Required(ErrorMessage = "单位不能为空！")]
    [MaxLength(8, ErrorMessage = "单位长度不能大于8！")]
    public string Unit { get; set; } = string.Empty;

    [BindNever]
    public IList<string> UnitList { get; set; } = new List<string>();
}