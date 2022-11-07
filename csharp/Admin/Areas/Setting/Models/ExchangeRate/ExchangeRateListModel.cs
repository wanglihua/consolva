using shunda.Core;
using shunda.Domain.Fee;

namespace shunda.Admin.Areas.Setting.Models.ExchangeRate;

public class ExchangeRateListModel : ListModel
{
    public ExchangeRateListModel(ListPager listPager) : base(listPager)
    {
    }

    public IList<ExchangeRateInfo> ExchangeRateList { get; set; } = new List<ExchangeRateInfo>();
}