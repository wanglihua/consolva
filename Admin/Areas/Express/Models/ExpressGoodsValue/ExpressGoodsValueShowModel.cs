using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Express.Models.ExpressGoodsValue;

public class ExpressGoodsValueShowModel : ExpressGoodsValueSaveParam {
    /// <summary>
    /// 快递单号
    /// </summary>
    [BindNever]
    public string TrackingNo { get; set; } = string.Empty;
}