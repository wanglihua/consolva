using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Instruction.Models.WarehouseAddress;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Client;
using shunda.Domain.Storage;
using shunda.Service.Client;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Instruction.Controllers;

[InstructionArea]
public class WarehouseAddressController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CustomerCodeService customerCodeService;
    private readonly CustomerClientContentViewRecordService customerClientContentViewRecordService;

    public WarehouseAddressController(ISession session,
        LoginCustomerService loginCustomerService,
        CustomerCodeService customerCodeService,
        CustomerClientContentViewRecordService customerClientContentViewRecordService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.customerCodeService = customerCodeService;
        this.customerClientContentViewRecordService = customerClientContentViewRecordService;
    }

    [HttpPost]
    [Route("/client/warehouse/address/detail")]
    [Transactional]
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
        if (string.IsNullOrWhiteSpace(warehouse.Area)) {
            throw new ValidationException("仓库省市街道信息未设置，请联系管理员！");
        }

        if (string.IsNullOrWhiteSpace(warehouse.Address)) {
            throw new ValidationException("仓库省详细地址信息未设置，请联系管理员！");
        }

        if (string.IsNullOrWhiteSpace(warehouse.Phone)) {
            throw new ValidationException("仓库联系电话信息未设置，请联系管理员！");
        }

        if (string.IsNullOrWhiteSpace(warehouse.Zipcode)) {
            throw new ValidationException("仓库邮编信息未设置，请联系管理员！");
        }

        customerClientContentViewRecordService.RecordView(loginCustomerId, ClientContentType.WarehouseAddress);

        string customerCode = customerCodeService.GetCustomerCodeChinese(loginCustomerId);

        // 这个格式基本上大多数系统都能识别
        string fullAddress = @$"收件人:{customerCode}
手机:{warehouse.Phone}
收件地址:{warehouse.Area}{warehouse.Address}{customerCode}室 
邮编:{warehouse.Zipcode}";

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new WarehouseAddressDetailModel {
                CustomerCode = customerCode,
                Area = warehouse.Area,
                Address = $"{warehouse.Address}{customerCode}室",
                Phone = warehouse.Phone,
                Zipcode = warehouse.Zipcode,
                FullAddress = fullAddress,
            }
        });
    }
}