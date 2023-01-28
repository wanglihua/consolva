using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderCreateNew;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateNewController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateService logisticsOrderCreateService;

    public OrderCreateNewController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateService logisticsOrderCreateService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateService = logisticsOrderCreateService;
    }

    [HttpPost]
    [Route("/client/order/create/new")]
    [Transactional]
    public IActionResult New() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var order = logisticsOrderCreateService.CreateOrderAndSubmitPack(loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateNewModel {
                OrderId = order.Id,
            },
        });
    }
}