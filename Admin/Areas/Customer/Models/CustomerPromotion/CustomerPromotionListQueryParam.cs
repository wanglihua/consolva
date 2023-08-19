using shunda.Core;

namespace shunda.Admin.Areas.Customer.Models.CustomerPromotion;

public class CustomerPromotionListQueryParam : ListQueryParam {
    public string? ParentCustomerCode { get; set; }

    public string? ChildCustomerCode { get; set; }

    public string? ChildOpenId { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }
}