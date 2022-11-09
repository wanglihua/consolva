using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Fee.Models.FeeWriteOffApprove;

public class LogisticsOrderFeeWriteOffApproveDetailModel
{
    // todo 这个是不是改成字段放入 model 的做法
    public FeeWriteOffApplyInfo FeeWriteOffApply { get; set; } = new FeeWriteOffApplyInfo();
}