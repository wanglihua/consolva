using Microsoft.AspNetCore.Mvc;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateNewController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public OrderCreateNewController(ISession session, LoginCustomerService loginCustomerService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/order/create/new")]
    public IActionResult New() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var loginCustomer = loginCustomerService.GetLoginCustomer();


        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}