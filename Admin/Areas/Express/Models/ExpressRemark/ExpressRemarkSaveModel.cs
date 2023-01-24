using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Express.Models.ExpressRemark;

public class ExpressRemarkSaveModel {
    public int Id { get; set; }

    [Required(ErrorMessage = "快递单不能为空！")]
    public int ExpressId { get; set; }

    [Required(ErrorMessage = "备注不能为空！")]
    [MaxLength(300, ErrorMessage = "备注长度不能大于300！")]
    public string Content { get; set; } = string.Empty;
}