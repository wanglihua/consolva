using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;

public class LogisticsOrderListQueryParam : ListQueryParam {
    [ModelBinder(typeof(CountryHibernateType))]
    public Country? Country { get; set; }

    public int OrderIdQuery { get; set; }

    public string? CodeQuery { get; set; }

    public string? CustomerCodeQuery { get; set; }

    public string? MobileQuery { get; set; }

    [ModelBinder(typeof(LogisticsOrderStatusModelBinder))]
    public LogisticsOrderStatus? OrderStatusQuery { get; set; }

    [ModelBinder(typeof(PayStatusHibernateType))]
    public PayStatus? PayStatusQuery { get; set; }

    public string? TrackingNoQuery { get; set; }

    public int ReceivingWarehouseIdQuery { get; set; }

    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public LogisticsModeType? FirstModeTypeQuery { get; set; }

    public OrderListMenuType MenuType { get; set; }

    public DateTime? BeginDateQuery { get; set; }

    public DateTime? EndDateQuery { get; set; }

    [ModelBinder(typeof(TimeRangeTypeModelBinder))]
    public TimeRangeType? TimeRangeTypeQuery { get; set; }

    [BindNever]
    public IList<int> OrderIdList { get; set; } = new List<int>();
}