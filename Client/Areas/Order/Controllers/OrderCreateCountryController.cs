using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderCreateCountry;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateCountryController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public OrderCreateCountryController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    [HttpPost]
    [Route("/client/order/create/country/list")]
    public IActionResult List() {
        var countryItemList = Country.GetNoChinaList()
            .Select(e => new OrderCreateCountryListItem {
                CountryId = e.Id,
                CountryName = e.Name,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateCountryListModel {
                CountryList = countryItemList,
            }
        });
    }

    [HttpPost]
    [Route("/client/order/create/country/save")]
    public IActionResult Save([FromBody] OrderCreateCountrySaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        Country country = Country.GetById(param.CountryId);
        if (country == Country.None) {
            throw new ValidationException("国家未指定或值无效！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        logisticsOrderCreateSelectService.SaveCountry(loginCustomerId, country);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}