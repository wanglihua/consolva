using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class FreightDescController : Controller {
    private readonly ISession session;

    public FreightDescController(ISession session) {
        this.session = session;
    }
}