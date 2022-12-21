using System.ComponentModel.DataAnnotations;

namespace shunda.Admin.Areas.Logistics.Models.ExpressReturnFeeRate;

public class ExpressReturnFeeRateIndexModel
{
    /// <summary>
    /// 每快递操作费，非空
    /// </summary>
    [Required(ErrorMessage = "每快递操作费不能为空！")]
    public decimal FeePerExpress { get; set; }
}