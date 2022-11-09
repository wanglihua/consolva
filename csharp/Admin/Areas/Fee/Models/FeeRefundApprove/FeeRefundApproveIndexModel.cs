using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApprove;

public class FeeRefundApproveIndexModel
{
    [BindNever]
    public IList<FeeRefundType> TypeList { get; set; }

    [BindNever]
    public IList<FeeRefundApplyStatus> StatusList { get; set; }

    [BindNever]
    public IList<UserInfo> ApplyUserList { get; set; }
}