using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.InsuranceClause;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Service.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class InsuranceClauseController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerClientContentViewRecordService customerClientContentViewRecordService;

    public InsuranceClauseController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerClientContentViewRecordService customerClientContentViewRecordService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerClientContentViewRecordService = customerClientContentViewRecordService;
    }

    [HttpPost]
    [Route("/client/insurance/clause/detail")]
    [Transactional]
    public IActionResult Detail() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var instruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == ClientInstructionType.InsuranceClause)
            .Take(1)
            .SingleOrDefault();

        string title = instruction?.Title ?? string.Empty;
        string content = instruction?.Content ?? string.Empty;

        customerClientContentViewRecordService.RecordView(loginCustomerId, ClientContentType.InsuranceClause);

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