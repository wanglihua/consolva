using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.FreightEstimateCountry;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class FreightEstimateCountryController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly FreightEstimateSelectService freightEstimateSelectService;

    public FreightEstimateCountryController(ISession session,
        LoginCustomerService loginCustomerService,
        FreightEstimateSelectService freightEstimateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.freightEstimateSelectService = freightEstimateSelectService;
    }

    [HttpPost]
    [Route("/client/freight/estimate/country/list")]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var estimateSelect = session.Get<FreightEstimateSelectInfo>(loginCustomerId);
        int selectCountryId = estimateSelect?.Country.Id ?? 0;

        // 选择修正，这里实际不做也行的
        if (selectCountryId != 0 && Country.GetNoChinaList().Where(e => e.Id == selectCountryId).Count() == 0) {
            selectCountryId = 0;
        }

        var countryItemList = Country.GetNoChinaList()
            .Select(e => new FreightEstimateCountryListItem {
                Id = e.Id,
                Name = e.Name,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new FreightEstimateCountryListModel {
                CountryList = countryItemList,
                SelectCountryId = selectCountryId,
            },
        });
    }

    [HttpPost]
    [Route("/client/freight/estimate/country/save")]
    [Transactional]
    public IActionResult Save([FromBody] FreightEstimateCountrySaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        Country country = Country.GetById(param.CountryId);
        if (country == Country.None) {
            throw new ValidationException("国家未指定或值无效！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        freightEstimateSelectService.SaveCountry(loginCustomerId, country);

        return Json(new JsonResultData {
            Success = true,
            Message = "国家保存成功！",
        });
    }
}