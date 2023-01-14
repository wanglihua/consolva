using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class TransportLimitController : Controller {
    private readonly ISession session;

    public TransportLimitController(ISession session) {
        this.session = session;
    }
}