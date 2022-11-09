using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundReason;

public class FeeRefundReasonDetailModel : FeeRefundReasonSaveModel
{
    public IList<FeeRefundReasonStatus> StatusList { get; set; } = new List<FeeRefundReasonStatus>();
}