using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class InsuranceClauseController : Controller {
    private readonly ISession session;

    public InsuranceClauseController(ISession session) {
        this.session = session;
    }
}