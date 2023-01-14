using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.Promotion;
using shunda.Core.Web;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class PromotionController : Controller {
    private readonly ISession session;

    public PromotionController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/promotion/detail")]
    public IActionResult Detail() {
        var instruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == ClientInstructionType.Promotion)
            .Take(1)
            .SingleOrDefault();

        string title = instruction?.Title ?? string.Empty;
        string content = instruction?.Content ?? string.Empty;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new PromotionDetailModel {
                Title = title,
                Content = content,
            },
        });
    }
}