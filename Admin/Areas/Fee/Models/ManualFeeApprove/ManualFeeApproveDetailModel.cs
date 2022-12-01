using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.ManualFeeApprove;

public class ManualFeeApproveDetailModel
{
    // todo 这个是不是改成字段放入 model 的做法
    public ManualFeeApplyInfo ManualFeeApply { get; set; } = new ManualFeeApplyInfo();
}