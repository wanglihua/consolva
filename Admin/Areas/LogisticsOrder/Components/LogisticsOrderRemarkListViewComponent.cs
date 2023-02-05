using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;
using shunda.Admin.Services;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using shunda.Service.LogisticsOrder;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Components;

public class LogisticsOrderRemarkListViewComponent : ViewComponent {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderRemarkService logisticsOrderRemarkService;
    private readonly PassResService passResService;
    private readonly ExpressRemarkService expressRemarkService;

    public LogisticsOrderRemarkListViewComponent(ISession session,
        LoginUserService loginUserService,
        LogisticsOrderRemarkService logisticsOrderRemarkService,
        PassResService passResService,
        ExpressRemarkService expressRemarkService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderRemarkService = logisticsOrderRemarkService;
        this.passResService = passResService;
        this.expressRemarkService = expressRemarkService;
    }

    public IViewComponentResult Invoke(int orderId) {
        int loginUserId = loginUserService.GetLoginUserId();

        var model = new LogisticsOrderRemarkListViewComponentModel {
            OrderId = orderId,
            OrderRemarkList = logisticsOrderRemarkService.GetOrderRemarkList(orderId),
            ExpressRemarkList = expressRemarkService.GetOrderExpressRemarkList(orderId),

            CanOrderRemarkEdit = passResService.UserHasOneOfPassRes(loginUserId, PassRes.LogisticsOrder.OrderRemarkEdit, PassRes.ReceivePack.OrderPack),
        };

        return View("~/Areas/LogisticsOrder/Views/LogisticsOrderRemark/LogisticsOrderRemarkListViewComponent.cshtml", model);
    }
}