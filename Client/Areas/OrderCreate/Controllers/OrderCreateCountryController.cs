using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.OrderCreate.Models.OrderCreateCountry;
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

namespace shunda.Client.Areas.OrderCreate.Controllers;

[OrderCreateArea]
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
        var expressList = session.Query<ExpressInfo>()
            .Where(e => expressIdList.Contains(e.Id))
            .ToList();

        var expressIdListToUnselect = expressList.Where(e => e.Status != ExpressStatus.Checkin).Select(e => e.Id).ToList();
        foreach (var expressIdToUnselect in expressIdListToUnselect) {
            var expressToUnselect = selectExpressList
                .Where(e => e.ExpressId == expressIdToUnselect)
                .Take(1)
                .SingleOrDefault();
            if (expressToUnselect != null) {
                session.Delete(expressToUnselect);
            }
        }

        session.Flush();

        expressList = expressList.Where(e => e.Status == ExpressStatus.Checkin).ToList();

        if (expressList.Count == 0) {
            throw new ValidationException("快递单列表不能为空，请先选择快递单号转运！");
        }

        foreach (ExpressInfo express in expressList) {
            // 这个检查实际已经没用了
            if (express.Status != ExpressStatus.Checkin) {
                throw new ValidationException($"快递单号#{express.TrackingNo}不是已入库状态，快递单选择选择数据有误！");
            }
        }

        var goodsTypeList = expressList.Select(e => e.GoodsType).Distinct().ToList();
        var goodsType = logisticsGoodsTypeService.GetGoodsTypeFromGoodsTypeList(goodsTypeList);

        logisticsOrderCreateSelectService.SaveGoodsType(loginCustomerId, goodsType);

        // 当前选择
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);
        int selectCountryId = createSelect?.Country.Id ?? 0;

        // 选择修正，这里实际不做也行的
        if (selectCountryId != 0 && Country.GetNoChinaList().Where(e => e.Id == selectCountryId).Count() == 0) {
            selectCountryId = 0;
        }

        // 显示国家列表
        var countryItemList = new List<OrderCreateCountryListItem>();

        var countryList = Country.GetNoChinaList();
        foreach (var c in countryList) {
            string name = c.Name;
            if (c == Country.Australia || c == Country.America || c == Country.Canada) {
                name = $"{name}(已开通)";
            }

            countryItemList.Add(new OrderCreateCountryListItem {
                Id = c.Id,
                Name = name,
            });
        }

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