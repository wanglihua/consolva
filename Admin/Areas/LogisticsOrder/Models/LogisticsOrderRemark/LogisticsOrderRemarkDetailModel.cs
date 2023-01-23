using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;

public class LogisticsOrderRemarkDetailModel : LogisticsOrderRemarkSaveModel {
    [BindNever]
    public string OrderCode { get; set; } = string.Empty;

    [BindNever]
    public IList<LogisticsOrderRemarkReasonInfo> RemarkReasonList { get; set; } = new List<LogisticsOrderRemarkReasonInfo>();
}