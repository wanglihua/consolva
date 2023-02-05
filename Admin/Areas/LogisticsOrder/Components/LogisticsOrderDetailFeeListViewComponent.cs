using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailFee;
using shunda.Core;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Components;

public class LogisticsOrderDetailFeeListViewComponent : ViewComponent {
    private readonly ISession session;

    public LogisticsOrderDetailFeeListViewComponent(
        ISession session
    ) {
        this.session = session;
    }

    public IViewComponentResult Invoke(int orderId) {
        LogisticsOrderInfo logisticsOrder = session.Get<LogisticsOrderInfo>(orderId);
        if (logisticsOrder == null) {
            throw new ValidationException("指定的数据项不存在！");
        }

        var orderFeeList = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();

        var orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Fetch(e => e.FirstMode)
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();

        var model = new LogisticsOrderDetailFeeListViewComponentModel {
            Order = logisticsOrder,
            OrderFirstModeList = orderFirstModeList,
            OrderFeeList = orderFeeList
        };

        return View("~/Areas/LogisticsOrder/Views/LogisticsOrderDetailFee/LogisticsOrderDetailFeeListViewComponent.cshtml", model);
    }
}