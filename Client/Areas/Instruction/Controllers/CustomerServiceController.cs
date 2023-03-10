using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.CustomerService;
using shunda.Core.Web;
using shunda.Domain.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class CustomerServiceController : Controller {
    private readonly ISession session;

    public CustomerServiceController(ISession session) {
        this.session = session;
    }

    [HttpPost]
    [Route("/client/customer/service/detail")]
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
            Data = new CustomerServiceDetailModel {
                Title = title,
                Url = url,
            },
        });
    }
}