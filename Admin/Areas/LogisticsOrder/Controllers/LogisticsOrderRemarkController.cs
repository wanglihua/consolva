using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Express;
using shunda.Service.LogisticsOrder;
using shunda.Service.Pass;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderRemarkController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderRemarkService logisticsOrderRemarkService;
    private readonly PassResService passResService;
    private readonly ExpressRemarkService expressRemarkService;

    public LogisticsOrderRemarkController(ISession session,
        LoginUserService loginUserService,
        LogisticsOrderRemarkService logisticsOrderRemarkService,
        PassResService passResService,
        ExpressRemarkService expressRemarkService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderRemarkService = logisticsOrderRemarkService;
        this.passResService = passResService;
        this.expressRemarkService = expressRemarkService;
    }

    [HttpPost]
    [Route("/admin/logistics/order/remark/detail")]
    public IActionResult Detail(int orderId) {
        session.DefaultReadOnly = true;

        LogisticsOrderInfo order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        LogisticsOrderRemarkInfo logisticsOrderRemark = new LogisticsOrderRemarkInfo();

        LogisticsOrderRemarkDetailModel model = new LogisticsOrderRemarkDetailModel {
            RemarkId = logisticsOrderRemark.Id,
            Content = logisticsOrderRemark.Content,
            OrderId = orderId,
            OrderCode = order.Code,
        };

        return View("LogisticsOrderRemarkDetail", model);
    }

    [AdminAuth(PassRes.LogisticsOrder.OrderRemarkEdit, PassRes.ReceivePack.OrderPack)]
    [HttpPost]
    [Route("/admin/logistics/order/remark/save")]
    [Transactional]
    public IActionResult Save(LogisticsOrderRemarkSaveModel model) {
        int orderId = model.OrderId;
        LogisticsOrderInfo logisticsOrder = session.Get<LogisticsOrderInfo>(orderId);
        if (logisticsOrder == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (!ModelState.IsValid) {
            throw new ValidationException(ValidationUtil.GetErrorMessages(ModelState));
        }

        int logisticsOrderRemarkId = model.RemarkId;

        bool isCreate = logisticsOrderRemarkId == 0;

        LogisticsOrderRemarkInfo logisticsOrderRemark;

        if (isCreate) {
            logisticsOrderRemark = new LogisticsOrderRemarkInfo();
        } else {
            logisticsOrderRemark = session.Get<LogisticsOrderRemarkInfo>(logisticsOrderRemarkId);
            if (logisticsOrderRemark == null) {
                throw new ValidationException($"指定的订单备注#{logisticsOrderRemarkId}不存在！");
            }
        }

        logisticsOrderRemark.Type = LogisticsOrderRemarkType.Service;
        logisticsOrderRemark.Order = logisticsOrder;
        logisticsOrderRemark.Content = model.Content;
        logisticsOrderRemark.AddUser = loginUserService.GetLoginUser();

        if (isCreate) {
            session.Save(logisticsOrderRemark);
        } else {
            session.Update(logisticsOrderRemark);
        }

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "保存成功！" }
        });
    }

    [HttpPost]
    [Route("/admin/logistics/order/remark/list")]
    public IActionResult OrderRemarkList(int orderId) {
        int loginUserId = loginUserService.GetLoginUserId();

        var model = new LogisticsOrderRemarkListModel {
            OrderId = orderId,
        };

        return View("LogisticsOrderRemarkList", model);
    }

    [AdminAuth(PassRes.LogisticsOrder.OrderRemarkEdit, PassRes.ReceivePack.OrderPack)]
    [HttpPost]
    [Route("/admin/logistics/order/remark/single/delete")]
    [Transactional]
    public IActionResult SingleDelete(int id) {
        var logisticsOrderRemark = session.Get<LogisticsOrderRemarkInfo>(id);
        if (logisticsOrderRemark == null) {
            throw new ValidationException($"指定的订单备注#{id}不存在！");
        }

        if (logisticsOrderRemark.CreateTime < DateTime.Now.AddHours(-12)) {
            throw new ValidationException("订单备注已创建很久，不能删除！");
        }

        if (loginUserService.IsAdmin == false && (logisticsOrderRemark.AddUser != null && logisticsOrderRemark.AddUser.Id != loginUserService.GetLoginUserId())) {
            throw new ValidationException("普通用户只能删除本人创建的订单备注！");
        }

        session.Delete(logisticsOrderRemark);

        session.Flush();

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "订单备注删除成功！" },
        });
    }
}