using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.OrderFee.Models.LogisticsOrderPay;

public class LogisticsOrderPayIndexModel
{
    public IList<LogisticsModeType> LogisticsModeTypeList { get; set; } = new List<LogisticsModeType>();
}