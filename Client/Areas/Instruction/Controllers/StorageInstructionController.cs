using Microsoft.AspNetCore.Mvc;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class StorageInstructionController : Controller {
    private readonly ISession session;

    public StorageInstructionController(ISession session) {
        this.session = session;
    }
}