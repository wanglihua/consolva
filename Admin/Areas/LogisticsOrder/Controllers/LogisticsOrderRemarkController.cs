using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderRemark;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass;
using shunda.Domain.Pass.PassRes;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderRemarkController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderRemarkService logisticsOrderRemarkService;

    public LogisticsOrderRemarkController(ISession session,
        LoginUserService loginUserService,
        LogisticsOrderRemarkService logisticsOrderRemarkService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderRemarkService = logisticsOrderRemarkService;
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
            Id = logisticsOrderRemark.Id,
            Content = logisticsOrderRemark.Content,
            OrderId = orderId,
            OrderCode = order.Code,
        };

        return View("LogisticsOrderRemarkDetail", model);
    }

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

        int logisticsOrderRemarkId = model.Id;

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
        LogisticsOrderRemarkListModel model = new LogisticsOrderRemarkListModel {
            OrderId = orderId,
            LogisticsOrderRemarkList = logisticsOrderRemarkService.GetOrderRemarkList(orderId),
        };

        return View("LogisticsOrderRemarkList", model);
    }

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