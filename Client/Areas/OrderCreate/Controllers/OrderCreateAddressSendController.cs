using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.OrderCreate.Models.OrderCreateAddressSend;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.OrderCreate.Controllers;

[OrderCreateArea]
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
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);

        int selectAddressId = createSelect?.AddressSendId ?? 0;
        var addressList = session.Query<CustomerSendAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId)
            .OrderByDescending(e => e.IsDefault)
            .ThenByDescending(e => e.Id)
            .ToList();

        if (selectAddressId != 0 && addressList.Where(e => e.Id == selectAddressId).Count() == 0) {
            selectAddressId = 0;
        }

        var addressItemList = addressList
            .Select(e => new OrderCreateAddressSendListItem {
                Id = e.Id,
                Country = e.Country.Name,
                Name = e.Name,
                Mobile = e.Mobile,
                Province = e.Province.Name,
                City = e.City.Name,
                Area = e.Area.Name,
                Street = e.Street.Name,
                Address = e.Address,
                Zipcode = e.Zipcode,
                IsDefault = e.IsDefault,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateAddressSendListModel {
                AddressList = addressItemList,
                SelectAddressId = selectAddressId,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/create/address/send/save")]
    [Transactional]
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