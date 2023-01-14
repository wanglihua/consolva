using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.FreightDesc;
using shunda.Core.Web;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class FreightDescController : Controller {
    private readonly ISession session;

    public FreightDescController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/freight/desc/detail")]
    public IActionResult Detail() {
        var instruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == ClientInstructionType.FreightDesc)
            .Take(1)
            .SingleOrDefault();

        string title = instruction?.Title ?? string.Empty;
        string content = instruction?.Title ?? string.Empty;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new FreightDescDetailModel {
                Title = title,
                Content = content,
            },
        });
    }
}