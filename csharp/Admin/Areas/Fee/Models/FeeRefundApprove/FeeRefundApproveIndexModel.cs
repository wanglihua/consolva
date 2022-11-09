using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Fee.Models.FeeRefundApprove;

public class FeeRefundApproveIndexModel
{
    [BindNever]
    public IList<FeeRefundType> TypeList { get; set; } = new List<FeeRefundType>();

    [BindNever]
    public IList<FeeRefundApplyStatus> StatusList { get; set; } = FeeRefundApplyStatus.GetList();

    [BindNever]
    public IList<UserInfo> ApplyUserList { get; set; } = new List<UserInfo>();
}