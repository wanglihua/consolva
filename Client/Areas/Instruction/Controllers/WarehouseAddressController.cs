using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.WarehouseAddress;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Service.Client;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class WarehouseAddressController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerClientContentViewRecordService customerClientContentViewRecordService;

    public WarehouseAddressController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerClientContentViewRecordService customerClientContentViewRecordService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerClientContentViewRecordService = customerClientContentViewRecordService;
    }

    [HttpPost]
    [Route("/client/warehouse/address/detail")]
    public IActionResult Detail() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        
        

        customerClientContentViewRecordService.RecordView(loginCustomerId, ClientContentType.WarehouseAddress);
        
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new WarehouseAddressDetailModel {
                
            }
        });
    }
}