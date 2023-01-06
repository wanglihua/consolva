using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderCreateAddressReceive;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateAddressReceiveController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public OrderCreateAddressReceiveController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    [HttpPost]
    [Route("/client/order/create/address/receive/list")]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);
        if (createSelect == null || createSelect.Country == Country.None) {
            throw new ValidationException("订单创建选择数据不存在或者国家未选择！");
        }

        int selectAddressId = createSelect.AddressReceiveId;
        Country country = createSelect.Country ?? Country.None;
        var addressList = session.Query<CustomerReceiveAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && e.Country == country)
            .OrderByDescending(e => e.IsDefault)
            .ThenByDescending(e => e.Id)
            .ToList();

        // 选择修正
        if (addressList.Where(e => e.Id == selectAddressId).Count() == 0) {
            selectAddressId = 0;
        }

        var addressItemList = addressList
            .Select(e => new OrderCreateAddressReceiveListItem {
                Id = e.Id,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateAddressReceiveListModel {
                AddressList = addressItemList,
                SelectAddressId = selectAddressId,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/create/address/receive/save")]
    public IActionResult Save([FromBody] OrderCreateAddressReceiveSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int addressId = param.AddressId;
        if (addressId <= 0) {
            throw new ValidationException("收货地址不能为空！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        logisticsOrderCreateSelectService.SaveReceiveAddress(loginCustomerId, addressId);

        return Json(new JsonResultData {
            Success = true,
            Message = "地址保存成功！",
        });
    }
}