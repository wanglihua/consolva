using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderCreateAddressSend;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateAddressSendController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public OrderCreateAddressSendController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    [HttpPost]
    [Route("/client/order/create/address/send/list")]
    public IActionResult List() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/client/order/create/address/send/save")]
    public IActionResult Save([FromBody] OrderCreateAddressSendSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int addressId = param.AddressId;
        if (addressId <= 0) {
            throw new ValidationException("发货地址不能为空！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        logisticsOrderCreateSelectService.SaveSendAddress(loginCustomerId, addressId);

        return Json(new JsonResultData {
            Success = true,
            Message = "地址保存成功！",
        });
    }
}