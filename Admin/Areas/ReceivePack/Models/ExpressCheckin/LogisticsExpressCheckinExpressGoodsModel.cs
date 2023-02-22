using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressCheckin;

public class LogisticsExpressCheckinExpressGoodsModel {
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

    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

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
    public string? Unit { get; set; } // 放到 controller 中判断必填后，model里面只能先设置成 可空

    [BindNever]
    public IList<string> UnitList { get; set; } = new List<string>();

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetInnerList();
}