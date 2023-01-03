using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Express.Models.ExpressReturn;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Express.Controllers;

[ExpressArea]
public class ExpressReturnController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public ExpressReturnController(ISession session, LoginCustomerService loginCustomerService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/express/return/list")]
    public IActionResult List([FromBody] ExpressReturnListParam param) {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    /// <summary>
    /// todo 这里只做申请就可以退运，还没有集成退运支付
    /// </summary>
    [HttpPost]
    [Route("/client/express/return/apply")]
    [Transactional]
    public IActionResult Apply([FromBody] IdParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int expressId = param.Id;
        var express = session.Get<ExpressInfo>(expressId);
        if (express == null) {
            throw new ValidationException($"指定的快递单号#{expressId}不存在！");
        }

        if ((express.Customer?.Id ?? 0) != loginCustomerId) {
            throw new ValidationException($"指定的快递单号{express.TrackingNo}不属于当前登录会员#{loginCustomerId}！");
        }

        if (express.Status != ExpressStatus.CheckIn) {
            throw new ValidationException("指定的快递单号{express.TrackingNo}不是已入库状态！");
        }

        express.Status = ExpressStatus.WaitReturn;
        // express.ReturnApplyTime = DateTime.Now;

        session.Update(express);
        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}