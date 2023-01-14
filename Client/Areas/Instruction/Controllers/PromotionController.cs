using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers; 

[InstructionArea]
public class PromotionController : Controller {
    private readonly ISession session;

    public PromotionController(ISession session) {
        this.session = session;
    }
}