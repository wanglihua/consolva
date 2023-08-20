using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotionImage;

public class CustomerPromotionImageSettingModel {
    [Required(ErrorMessage = "是否客户端展现不能为空！")]
    public bool PromotionClientShow { get; set; }

    [Required(ErrorMessage = "推广图片描述不能为空！")]
    [MaxLength(10000, ErrorMessage = "推广图片描述不能大于10000！")]
    public string PromotionImageDesc { get; set; } = string.Empty;
}