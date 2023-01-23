using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;

public class LogisticsOrderRemarkSaveModel {
    public int Id { get; set; }

    [Required(ErrorMessage = "订单不能为空！")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "原因不能为空！")]
    [MaxLength(100, ErrorMessage = "原因长度不能大于100！")]
    public string Reason { get; set; } = string.Empty;

    [Required(ErrorMessage = "备注不能为空！")]
    [MaxLength(300, ErrorMessage = "备注长度不能大于300！")]
    public string Content { get; set; } = string.Empty;
}