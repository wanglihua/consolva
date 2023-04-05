using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderDetailOrderPack;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.LogisticsOrder.Controllers;

[LogisticsOrderArea]
[AdminAuth(PassRes.LogisticsOrder.OrderListActive, PassRes.LogisticsOrder.OrderListComplete, PassRes.LogisticsOrder.OrderListCancel)]
public class LogisticsOrderDetailOrderPackController : Controller {
    private readonly ISession session;
    private readonly LogisticsOrderFeeService logisticsOrderFeeService;
    private readonly LogisticsOrderPackService logisticsOrderPackService;

    public LogisticsOrderDetailOrderPackController(
        ISession session,
        LogisticsOrderFeeService logisticsOrderFeeService,
        LogisticsOrderPackService logisticsOrderPackService
    ) {
        this.session = session;
        this.logisticsOrderFeeService = logisticsOrderFeeService;
        this.logisticsOrderPackService = logisticsOrderPackService;
    }

    [HttpPost]
    [Route("/admin/logistics/order/detail/order/pack/list")]
    public IActionResult List(int orderId) {
        var model = new LogisticsOrderDetailOrderPackListModel {
            OrderId = orderId,
        };

        return View("LogisticsOrderDetailPackList", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/detail/show/order/pack/detail")]
    [AdminAuth(PassRes.LogisticsOrder.OrderPackDataModify)]
    public IActionResult ShowDetail(int orderPackId) {
        var orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException($"指定的的订单打包信息#{orderPackId}不存在！");
        }

        LogisticsOrderDetailOrderPackDetailModel model = new LogisticsOrderDetailOrderPackDetailModel();
        model.Width = orderPack.Width;
        model.Length = orderPack.Length;
        model.Height = orderPack.Height;
        model.Weight = orderPack.Weight;

        return View("LogisticsOrderDetailPackDetail", model);
    }

    [HttpPost]
    [Route("/admin/logistics/order/detail/save/order/pack")]
    [AdminAuth(PassRes.LogisticsOrder.OrderPackDataModify)]
    [Transactional]
    public IActionResult SavePack(LogisticsOrderDetailOrderPackSavelModel model) {
        ValidationUtil.Validate<LogisticsOrderDetailOrderPackSavelModelValidator, LogisticsOrderDetailOrderPackSavelModel>(model);

        if (model.Width <= 0 || model.Length <= 0 || model.Height <= 0 || model.Weight <= 0) {
            throw new ValidationException("长宽高重量数据不能小于0！");
        }

        int orderPackId = model.OrderPackId;
        var orderPack = session.Get<LogisticsOrderPackInfo>(orderPackId);
        if (orderPack == null) {
            throw new ValidationException($"指定的订单打包信息{orderPackId}不存在！");
        }

        orderPack.Width = model.Width;
        orderPack.Length = model.Length;
        orderPack.Height = model.Height;
        orderPack.Weight = model.Weight;

        session.Save(orderPack);

        // 重新计算费用
        int orderId = orderPack.Order.Id;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        logisticsOrderPackService.SaveOrderPackData(orderId);

        // 重新计费
        logisticsOrderFeeService.CalculateAndSaveFee(orderId);

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "打包信息保存成功！" }
        });
    }
}