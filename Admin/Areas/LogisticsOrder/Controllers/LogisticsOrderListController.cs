using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderList;
using shunda.Admin.Services;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Domain.Storage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderListController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;

    public LogisticsOrderListController(ISession session, LoginUserService loginUserService) {
        this.session = session;
        this.loginUserService = loginUserService;
    }

    [HttpGet]
    [Route("/admin/logistics/order/active")]
    public IActionResult IndexActive() {
        var model = new LogisticsOrderListIndexModel {
            MenuType = OrderListMenuType.Active,
        };

        return IndexInternal(model);
    }

    [HttpGet]
    [Route("/admin/logistics/order/complete")]
    public IActionResult IndexComplete() {
        var model = new LogisticsOrderListIndexModel {
            MenuType = OrderListMenuType.Complete,
        };

        return IndexInternal(model);
    }

    [HttpGet]
    [Route("/admin/logistics/order/cancel")]
    public IActionResult IndexCancel() {
        var model = new LogisticsOrderListIndexModel {
            MenuType = OrderListMenuType.Cancel,
        };

        return IndexInternal(model);
    }

    [NonAction]
    private IActionResult IndexInternal(LogisticsOrderListIndexModel model) {
        model.CountryList = Country.GetList();
        model.FirstModeTypeList = LogisticsModeType.GetList();
        model.OrderStatusList = LogisticsOrderStatus.GetList();
        model.PayStatusList = PayStatus.GetList();

        model.ReceivingWarehouseList = session
            .Query<WarehouseInfo>()
            .Where(e => e.InnerStatus == WarehouseStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .ThenBy(e => e.Id)
            .ToList();

        model.TimeRangeTypeList = TimeRangeType.GetList();

        bool isAdmin = loginUserService.IsAdmin;
        Dictionary<int, bool> passResIdMap = loginUserService.GetPassResIdMap();
        model.CanImportOrder = PassUtil.HasPassRes(passResIdMap, isAdmin, PassRes.LogisticsOrder.ImportOrder);

        return View("LogisticsOrderListIndex", model);
    }
}