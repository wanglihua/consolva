using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeRefund;

public class FeeRefundIndexModel
{
    public IList<FeeRefundType> TypeList { get; set; }

    public IList<FeeRefundStatus> StatusList { get; set; }
}