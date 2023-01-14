using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.InsuranceClause;
using shunda.Core.Web;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class InsuranceClauseController : Controller {
    private readonly ISession session;

    public InsuranceClauseController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/insurance/clause/detail")]
    public IActionResult Detail() {
        var instruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == ClientInstructionType.InsuranceClause)
            .Take(1)
            .SingleOrDefault();

        string title = instruction?.Title ?? string.Empty;
        string content = instruction?.Title ?? string.Empty;

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new InsuranceClauseDetailModel {
                Title = title,
                Content = content,
            },
        });
    }
}