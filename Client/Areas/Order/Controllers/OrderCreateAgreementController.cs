using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateAgreementController : Controller {
    private readonly ISession session;
    
    public OrderCreateAgreementController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/order/create/agreement/list")]
    public IActionResult List() {
        
        
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}