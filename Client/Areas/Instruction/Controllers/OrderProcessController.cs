using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class OrderProcessController : Controller {
    private readonly ISession session;

    public OrderProcessController(ISession session) {
        this.session = session;
    }
}