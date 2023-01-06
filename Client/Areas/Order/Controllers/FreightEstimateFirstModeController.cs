using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.FreightEstimateFirstMode;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class FreightEstimateFirstModeController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly FreightEstimateSelectService freightEstimateSelectService;

    public FreightEstimateFirstModeController(ISession session,
        LoginCustomerService loginCustomerService,
        FreightEstimateSelectService freightEstimateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.freightEstimateSelectService = freightEstimateSelectService;
    }

    [HttpPost]
    [Route("/client/freight/estimate/first/mode/list")]
    public IActionResult List() {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/client/freight/estimate/first/mode/save")]
    [Transactional]
    public IActionResult Save([FromBody] FreightEstimateFirstModeSaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int firstModeId = param.FirstModeId;
        if (firstModeId <= 0) {
            throw new ValidationException("头程物流不能为空！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        freightEstimateSelectService.SaveFirstMode(loginCustomerId, firstModeId);

        return Json(new JsonResultData {
            Success = true,
            Message = "物流保存成功！",
        });
    }
}