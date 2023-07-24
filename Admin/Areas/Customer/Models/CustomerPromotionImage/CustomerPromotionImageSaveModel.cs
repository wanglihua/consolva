using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotionImage;

public class CustomerPromotionImageSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int CustomerPromotionImageId { get; set; }

    /// <summary>
    /// 图片序号，非空
    /// </summary>
    public int ImageNum { get; set; } = 0;

    /// <summary>
    /// 图片文件名，非空
    /// </summary>
    public string ImageFileName { get; set; } = string.Empty;

}