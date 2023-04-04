using Microsoft.AspNetCore.Mvc;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

[AppArea]
[AdminAuth]
public class PackBoxController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;

    public PackBoxController(
        ISession session,
        LoginUserService loginUserService,
        LogisticsOrderPackBoxService logisticsOrderPackBoxService,
        LogisticsOrderPaySumService logisticsOrderPaySumService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderPackBoxService = logisticsOrderPackBoxService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
    }

    [AdminAuth(PassRes.AdminApp.PackBoxQuery)]
    [HttpPost]
    [Route("/admin/app/pack/box/query")]
    public IActionResult Query(string packBoxCode) {
        LogisticsOrderInfo? order = null;

        LogisticsOrderPackBoxInfo? packBox = logisticsOrderPackBoxService.ParseBoxCode(packBoxCode, throwException: false);
        if (packBox != null) {
            int orderId = packBox.Order.Id;

            order = session.Get<LogisticsOrderInfo>(orderId);
            if (order == null) {
                throw new ValidationException("箱号对应的订单不存在！");
            }
        }

        if (order == null) {
            throw new ValidationException("箱号格式不对，箱号不存在，对应订单不存在！");
        }

        CustomerInfo customer = session.Get<CustomerInfo>(order.Customer.Id);
        if (customer == null) {
            throw new ValidationException("订单对应的客户不存在！");
        }

        // 判断欠费前先做下合计
        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(order.Id);

        // 欠费检查
        // if (logisticsOrder.ArrearsAmountFormula > 0)
        // 这里不用 ArrearsAmountFormula 判断的原因是 ArrearsAmountFormula 是数据库表达式计算的结果，需要再查询一下数据库，才成让刚刚的合计起作用
        decimal arrearsAmount = order.OrderAmount - order.PaidAmount;

        string orderPackBoxCode = string.Empty;
        if (packBox != null) {
            orderPackBoxCode = logisticsOrderPackBoxService.GetBoxCode(packBox.Id);
        }

        bool hasArrearsAmount = arrearsAmount > 0;
        
        // todo 改成data方式，加上 物流方式，地址等等信息

        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "箱号信息获取成功！" },
            { "orderPackBoxCode", orderPackBoxCode },
            { "orderCode", order.Code },
            { "customerCode", order.CustomerCode },
            { "arrearsAmount", arrearsAmount.ToString("#0.00") },
            { "hasArrearsAmount", hasArrearsAmount },
            { "customerMobile", customer.Mobile },
        });
    }

    [AdminAuth(PassRes.AdminApp.PackBoxShip)]
    [HttpPost]
    [Route("/admin/app/pack/box/ship")]
    public IActionResult Ship(int packBoxId) {

        /*
        if (hasArrearsAmount == false && logisticsOrder.Cate == LogisticsOrderCate.Air) {
            // 不欠费的空运订单，相当于是进行了出库操作
            int orderId = logisticsOrder.Id;
            var orderExtra = session.Query<LogisticsOrderExtraInfo>()
                .Where(e => e.LogisticsOrder.Id == orderId)
                .Take(1)
                .SingleOrDefault();
            if (orderExtra != null) // 正常情况下，这个数据肯定是存在的。相当于这个判断是有点冗余的
            {
                orderExtra.CheckoutTime = DateTime.Now;
                session.Update(orderExtra);
                session.Flush();

                int loginUserId = userApiTokenService.User?.Id ?? 0;
                logisticsOrderActionService.NewAction(logisticsOrder, LogisticsOrderActionType.AirOrderCheckout, loginUserId);

                // 空运订单转运
                if (logisticsOrder.OrderStatus == LogisticsOrderStatus.CnPacked || logisticsOrder.OrderStatus == LogisticsOrderStatus.CnWaitTransfer) {
                    logisticsOrder.OrderStatus = LogisticsOrderStatus.CnTransferred;
                    session.Update(logisticsOrder);
                    session.Flush();

                    logisticsOrderActionService.NewAction(logisticsOrder, LogisticsOrderActionType.Transfer, loginUserId);
                }
            }
        }
        */
    
        return Json(new Dictionary<string, object>() {
            { "success", true },
            { "message", "箱号发货成功！" },
        });
    }
}