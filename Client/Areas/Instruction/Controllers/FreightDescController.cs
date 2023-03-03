using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.FreightDesc;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Service.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class FreightDescController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerClientContentViewRecordService customerClientContentViewRecordService;

    public FreightDescController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerClientContentViewRecordService customerClientContentViewRecordService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerClientContentViewRecordService = customerClientContentViewRecordService;
    }

    /// <summary>
    /// 这个接口已经不用了，就用来保存查看记录
    /// </summary>
    [HttpPost]
    [Route("/client/freight/desc/detail")]
    [Transactional]
    public IActionResult Detail() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var instruction = session.Query<ClientInstructionInfo>()
            .Where(e => e.Type == ClientInstructionType.FreightDesc)
            .Take(1)
            .SingleOrDefault();

        string title = instruction?.Title ?? string.Empty;
        string url = instruction?.Url ?? string.Empty;

        customerClientContentViewRecordService.RecordView(loginCustomerId, ClientContentType.FreightDesc);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new FreightDescDetailModel {
                Title = title,
                Url = url,
            },
        });
    }
}