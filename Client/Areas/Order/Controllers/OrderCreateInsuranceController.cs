using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderCreateInsurance;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderCreateInsuranceController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public OrderCreateInsuranceController(ISession session,
        LoginCustomerService loginCustomerService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    [HttpPost]
    [Route("/client/order/create/insurance/list")]
    public IActionResult List() {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);

        int selectInsuranceId = createSelect?.InsuranceId ?? 0;
        var insuranceList = session.Query<LogisticsInsuranceInfo>()
            .Where(e => e.Status == LogisticsInsuranceStatus.Enabled)
            .ToList();

        if (selectInsuranceId != 0 && insuranceList.Where(e => e.Id == selectInsuranceId).Count() == 0) {
            selectInsuranceId = 0;
        }

        var insuranceItemList = insuranceList
            .Select(e => new OrderCreateInsuranceListItem {
                Id = e.Id
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderCreateInsuranceListModel {
                InsuranceList = insuranceItemList,
                SelectInsuranceId = selectInsuranceId,
            }
        });
    }

    [HttpPost]
    [Route("/client/order/create/insurance/save")]
    [Transactional]
    public IActionResult Save([FromBody] OrderCreateInsuranceSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int insuranceId = param.InsuranceId;
        if (insuranceId <= 0) {
            throw new ValidationException("保险方案不能为空！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        logisticsOrderCreateSelectService.SaveInsurance(loginCustomerId, insuranceId);

        return Json(new JsonResultData {
            Success = true,
            Message = "保险保存成功！",
        });
    }
}