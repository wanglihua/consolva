using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.WarehouseAddress;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Domain.Storage;
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

        var warehouse = session.Query<WarehouseInfo>()
            .Where(e => e.OuterStatus == WarehouseStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .Take(1)
            .SingleOrDefault();
        if (warehouse == null) {
            throw new ValidationException("仓库信息未设置，请联系管理员！");
        }

        // 地址各个部分设置检查

        customerClientContentViewRecordService.RecordView(loginCustomerId, ClientContentType.WarehouseAddress);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new WarehouseAddressDetailModel {
            }
        });
    }
}