using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class UseAgreementController : Controller {
    private readonly ISession session;

    public UseAgreementController(ISession session) {
        this.session = session;
    }
}