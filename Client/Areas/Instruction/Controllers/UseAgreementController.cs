using Microsoft.AspNetCore.Mvc;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class UseAgreementController : Controller {
    private readonly ISession session;

    public UseAgreementController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/use/agreement/detail")]
    public IActionResult Detail() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}