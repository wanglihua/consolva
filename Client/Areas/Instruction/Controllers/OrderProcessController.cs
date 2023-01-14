using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.OrderProcess;
using shunda.Core.Web;
using shunda.Domain.Client;
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
        var instruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == ClientInstructionType.OrderProcess)
            .Take(1)
            .SingleOrDefault();

        string title = instruction?.Title ?? string.Empty;
        string content = instruction?.Content ?? string.Empty;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderProcessDetailModel {
                Title = title,
                Content = content,
            }
        });
    }
}