using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderCreateCountry;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Express;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Logistics;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateCountryController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;
    private readonly LogisticsGoodsTypeService logisticsGoodsTypeService;

    public OrderCreateCountryController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService,
        LogisticsGoodsTypeService logisticsGoodsTypeService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
        this.logisticsGoodsTypeService = logisticsGoodsTypeService;
    }

    [HttpPost]
    [Route("/client/order/create/country/list")]
    [Transactional]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        // 保存上一步快递单选择形成的物品类型
        var selectExpressList = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == loginCustomerId)
            .ToList();
        var expressIdList = selectExpressList.Select(e => e.ExpressId).ToList();
        var expressList = session.Query<ExpressInfo>().Where(e => expressIdList.Contains(e.Id)).ToList();
        var goodsTypeList = expressList.Select(e => e.GoodsType).Distinct().ToList();
        var goodsType = logisticsGoodsTypeService.GetGoodsTypeFromGoodsTypeList(goodsTypeList);

        logisticsOrderCreateSelectService.SaveGoodsType(loginCustomerId, goodsType);

        // 当前选择
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);
        int selectCountryId = createSelect?.Country?.Id ?? 0;

        // 选择修正，这里实际不做也行的
        if (selectCountryId != 0 && Country.GetNoChinaList().Where(e => e.Id == selectCountryId).Count() == 0) {
            selectCountryId = 0;
        }

        // 显示国家列表
        var countryItemList = Country.GetNoChinaList()
            .Select(e => new OrderCreateCountryListItem {
                Id = e.Id,
                Name = e.Name,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateCountryListModel {
                CountryList = countryItemList,
                SelectCountryId = selectCountryId,
            }
        });
    }

    [HttpPost]
    [Route("/client/order/create/country/save")]
    [Transactional]
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
            Message = "国家保存成功！",
        });
    }
}