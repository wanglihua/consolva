using Microsoft.AspNetCore.Mvc;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateFirstModeController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public OrderCreateFirstModeController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    [HttpPost]
    [Route("/client/order/create/first/mode/list")]
    public IActionResult List() {
        // 物品类型的问题怎么解决
        

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}