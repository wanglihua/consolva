using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.RefundReason;

public class RefundReasonDetailModel
{
    public FeeRefundReasonInfo RefundReason { get; set; }

    public IList<FeeRefundReasonStatus> StatusList { get; set; }
}