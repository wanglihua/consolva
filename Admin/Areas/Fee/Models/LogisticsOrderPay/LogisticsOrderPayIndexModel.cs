using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Fee.Models.LogisticsOrderPay;

public class LogisticsOrderPayIndexModel
{
    public IList<LogisticsModeType> LogisticsModeTypeList { get; set; } = new List<LogisticsModeType>();
}