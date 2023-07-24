using shunda.Domain.Customer;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotion;

public class CustomerPromotionSaveModel {
    /// <summary>
    /// Id
    /// </summary>
    public int CustomerPromotionId { get; set; }

    /// <summary>
    /// 父id，非空
    /// </summary>
    public int ParentId { get; set; } = 0;

    /// <summary>
    /// 子id，可空
    /// </summary>
    public int? ChildId { get; set; } = null;

    /// <summary>
    /// open id，可空
    /// </summary>
    public string? OpenId { get; set; } = null;

    /// <summary>
    /// 下家消费金额，可空
    /// </summary>
    public decimal? ChildPayAmount { get; set; } = null;

}