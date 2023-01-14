using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.TransportLimit;
using shunda.Core.Web;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class TransportLimitController : Controller {
    private readonly ISession session;

    public TransportLimitController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/transport/limit/detail")]
    public IActionResult Detail() {
        var instruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == ClientInstructionType.TransportLimit)
            .Take(1)
            .SingleOrDefault();

        string title = instruction?.Title ?? string.Empty;
        string content = instruction?.Content ?? string.Empty;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new TransportLimitDetailModel {
                Title = title,
                Content = content,
            },
        });
    }
}