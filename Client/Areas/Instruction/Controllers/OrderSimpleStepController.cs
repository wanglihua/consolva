using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.OrderSimpleStep;
using shunda.Core.Web;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class OrderSimpleStepController : Controller {
    private ISession session;

    public OrderSimpleStepController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/order/simple/step/detail")]
    public IActionResult Detail() {
        var instruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == ClientInstructionType.CustomerService)
            .Take(1)
            .SingleOrDefault();

        string title = instruction?.Title ?? string.Empty;
        string url = instruction?.Url ?? string.Empty;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderSimpleStepDetailModel {
                Title = title,
                Url = url,
            },
        });
    }
}