using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;

public class LogisticsExpressCheckinExpressGoodsModel
{
    public string UiIndex { get; set; } = string.Empty;

    /// <summary>
    /// ExpressGoodsInfo 的 Id
    /// </summary>
    public int ExpressGoodsId { get; set; }

    /// <summary>
    /// LogisticsGoodsInfo 的 Id
    /// </summary>
    // [Required(ErrorMessage = "物品扩展名不能为空！")] 放到 controller 里面去判断了
    public int GoodsId { get; set; }

    /// <summary>
    /// 物品名称
    /// 只是用于显示
    /// </summary>
    [BindNever]
    public string GoodsName { get; set; } = string.Empty;

    // [Required(ErrorMessage = "数量不能为空！")] 放到 controller 里面去判断了
    public int Quantity { get; set; }

    // [Required(ErrorMessage = "单位不能为空！")] 放到 controller 里面去判断了
    // [MaxLength(8, ErrorMessage = "单位长度不能大于8！")]
    public string Unit { get; set; } = string.Empty;

    [BindNever]
    public IList<string> UnitList { get; set; } = new List<string>();
}