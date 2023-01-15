using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderCreateAgreement;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain;
using shunda.Domain.Client;
using shunda.Domain.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateAgreementController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public OrderCreateAgreementController(ISession session, LoginCustomerService loginCustomerService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/order/create/agreement/list")]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var orderCreateSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);
        if (orderCreateSelect == null) {
            throw new ValidationException($"指定的订单创建选择记录#{loginCustomerId}不存在！");
        }

        Country country = orderCreateSelect.Country;
        if (country == Country.None) {
            throw new ValidationException($"订单选择记录#{loginCustomerId}中，目的国家未选择，数据有问题！");
        }

        var agreementList = session.Query<ClientAgreementInfo>()
            .Where(e => (e.Country == Country.None || e.Country == country) && e.Status == ClientAgreementStatus.Enabled)
            .OrderBy(e => e.OrderNum)
            .ThenBy(e => e.Id)
            .ToList();

        var agreementItemList = agreementList
            .Select(e => new OrderCreateAgreementListItem {
                Id = e.Id,
                Title = e.Title,
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateAgreementListModel {
                AgreementList = agreementItemList,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/create/agreement/detail")]
    public IActionResult Detail([FromBody] IdParam param) {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}