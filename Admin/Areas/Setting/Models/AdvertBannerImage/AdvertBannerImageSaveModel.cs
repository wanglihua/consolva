namespace shunda.Admin.Areas.Setting.Models.AdvertBannerImage;

public class AdvertBannerImageSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int AdvertBannerImageId { get; set; }

    /// <summary>
    /// 广告横幅，非空
    /// </summary>
    public int BannerId { get; set; } = 0;

    /// <summary>
    /// 图片序号，非空
    /// </summary>
    public int ImageNum { get; set; } = 0;

    /// <summary>
    /// 存储路径，非空
    /// </summary>
    public string StorePath { get; set; } = string.Empty;

    /// <summary>
    /// 存储名称，非空
    /// </summary>
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// 链接地址，非空
    /// </summary>
    public string LinkUrl { get; set; } = string.Empty;
}