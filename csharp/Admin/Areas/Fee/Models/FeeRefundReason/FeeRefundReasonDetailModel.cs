using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundReason;

public class FeeRefundReasonDetailModel
{
    public FeeRefundReasonInfo RefundReason { get; set; }

    public IList<FeeRefundReasonStatus> StatusList { get; set; }
}