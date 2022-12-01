using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundReason;

public class FeeRefundReasonSaveModel
{
    public FeeRefundReasonInfo RefundReason { get; set; } = new FeeRefundReasonInfo();
}