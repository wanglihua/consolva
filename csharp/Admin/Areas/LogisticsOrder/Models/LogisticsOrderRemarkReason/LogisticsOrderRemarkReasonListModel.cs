using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemarkReason;

public class LogisticsOrderRemarkReasonListModel : ListModel
{
    public LogisticsOrderRemarkReasonListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<LogisticsOrderRemarkReasonInfo> LogisticsOrderRemarkReasonList { get; set; } = new List<LogisticsOrderRemarkReasonInfo>();
}