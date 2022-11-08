using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.WriteOffFee;

public class WriteOffFeeIndexModel
{
    /// <summary>
    /// 核销费用原因列表
    /// </summary>
    [BindNever]
    public IList<ManualFeeReasonInfo> FeeReasonList { get; set; } = new List<ManualFeeReasonInfo>();
}