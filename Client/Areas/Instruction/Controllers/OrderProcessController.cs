using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class OrderProcessController : Controller {
    private readonly ISession session;

    public OrderProcessController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/order/process/detail")]
    public IActionResult Detail() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}