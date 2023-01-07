using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.FreightEstimateAddressReceive;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class FreightEstimateAddressReceiveController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly FreightEstimateSelectService freightEstimateSelectService;

    public FreightEstimateAddressReceiveController(ISession session,
        LoginCustomerService loginCustomerService,
        FreightEstimateSelectService freightEstimateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.freightEstimateSelectService = freightEstimateSelectService;
    }

    [HttpPost]
    [Route("/client/freight/estimate/address/receive/list")]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var estimateSelect = session.Get<FreightEstimateSelectInfo>(loginCustomerId);
        if (estimateSelect == null || estimateSelect.Country == Country.None) {
            throw new ValidationException("运费预估选择数据不存在或者国家未选择！");
        }

        int selectAddressId = estimateSelect.AddressReceiveId;
        Country country = estimateSelect.Country;
        var addressList = session.Query<CustomerReceiveAddressInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && e.Country == country)
            .OrderByDescending(e => e.IsDefault)
            .ThenByDescending(e => e.Id)
            .ToList();

        // 选择修正
        if (selectAddressId != 0 && addressList.Where(e => e.Id == selectAddressId).Count() == 0) {
            selectAddressId = 0;
        }

        var addressItemList = addressList
            .Select(e => new FreightEstimateAddressReceiveListItem {
                Id = e.Id,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new FreightEstimateAddressReceiveListModel {
                AddressList = addressItemList,
                SelectAddressId = selectAddressId,
            },
        });
    }

    [HttpPost]
    [Route("/client/freight/estimate/address/receive/save")]
    [Transactional]
    public IActionResult Save([FromBody] FreightEstimateAddressReceiveSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int addressId = param.AddressId;
        if (addressId <= 0) {
            throw new ValidationException("收货地址不能为空！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        freightEstimateSelectService.SaveReceiveAddress(loginCustomerId, addressId);

        return Json(new JsonResultData {
            Success = true,
            Message = "地址保存成功！",
        });
    }
}