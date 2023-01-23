using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;

public class LogisticsOrderRemarkSaveModel {
    public int Id { get; set; }

    [Required(ErrorMessage = "订单不能为空！")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "备注不能为空！")]
    [MaxLength(300, ErrorMessage = "备注长度不能大于300！")]
    public string Content { get; set; } = string.Empty;
}