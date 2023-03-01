using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Order.Models.OrderPay;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderPayController : Controller {
    private readonly ISession session;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;

    public OrderPayController(
        ISession session,
        LogisticsOrderPaySumService logisticsOrderPaySumService
    ) {
        this.session = session;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
    }

    [HttpPost]
    [Route("/client/order/pay/type/list")]
    [Transactional]
    public IActionResult PayTypeList([FromBody] OrderPayTypeListParam param) {
        ValidationUtil.Validate<OrderPayTypeListParamValidator, OrderPayTypeListParam>(param);

        int orderId = param.OrderId;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderPayTypeListModel {
                OrderId = order.Id,
                OrderCode = order.Code,
                TotalAmount = order.OrderAmount.ToString("#0.00"),
                PaidAmount = order.PaidAmount.ToString("#0.00"),
                ArrearsAmount = (order.OrderAmount - order.PaidAmount).ToString("#0.00"), // 这里不能用 ArrearsAmountFormula
            }
        });
    }
}