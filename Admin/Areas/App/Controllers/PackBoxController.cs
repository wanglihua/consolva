using Microsoft.AspNetCore.Mvc;
using NHibernate;
using shunda.Admin.Areas.App.Models.PackBox;
using shunda.Admin.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Pass.PassRes;
using shunda.Service.Fee;
using shunda.Service.LogisticsOrder;
using shunda.Service.Message.CustomerMessage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Areas.App.Controllers;

[AppArea]
[AdminAuth]
public class PackBoxController : Controller {
    private readonly ISession session;
    private readonly LoginUserService loginUserService;
    private readonly LogisticsOrderPackBoxService logisticsOrderPackBoxService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly CustomerMessageTransferedService customerMessageTransferedService;

    public PackBoxController(
        ISession session,
        LoginUserService loginUserService,
        LogisticsOrderPackBoxService logisticsOrderPackBoxService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        LogisticsOrderPackBoxHistoryService logisticsOrderPackBoxHistoryService,
        LogisticsOrderActionService logisticsOrderActionService,
        CustomerMessageTransferedService customerMessageTransferedService
    ) {
        this.session = session;
        this.loginUserService = loginUserService;
        this.logisticsOrderPackBoxService = logisticsOrderPackBoxService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.logisticsOrderPackBoxHistoryService = logisticsOrderPackBoxHistoryService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.customerMessageTransferedService = customerMessageTransferedService;
    }

    [AdminAuth(PassRes.AdminApp.PackBoxShip)]
    [HttpPost]
    [Route("/admin/app/pack/box/ship")]
    [Transactional]
    public IActionResult Ship(PackBoxShipParam param) {
        ValidationUtil.Validate<PackBoxShipParamValidator, PackBoxShipParam>(param);

        LogisticsOrderPackBoxInfo? packBox = logisticsOrderPackBoxService.ParseBoxCode(param.PackBoxCode, throwException: false);
        if (packBox == null) {
            throw new ValidationException("箱号格式不对或箱号不存在！");
        }

        NHibernateUtil.Initialize(packBox.Order);
        var order = packBox.Order;
        int orderId = order.Id;

        // NHibernateUtil.Initialize(order.Customer);
        // CustomerInfo customer = order.Customer;

        // 判断欠费前先做下合计
        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(orderId);

        // 欠费检查
        // if (logisticsOrder.ArrearsAmountFormula > 0)
        // 这里不用 ArrearsAmountFormula 判断的原因是 ArrearsAmountFormula 是数据库表达式计算的结果，需要再查询一下数据库，才成让刚刚的合计起作用
        decimal arrearsAmount = order.OrderAmount - order.PaidAmount;
        bool hasArrears = arrearsAmount > 0;

        int loginUserId = loginUserService.GetLoginUserId();
        var loginUser = loginUserService.GetLoginUser();


        DateTime now = DateTime.Now;

        string message = "";
        if (hasArrears == true) {
            message = "订单有欠费，不能设置箱号已发货！";
        } else {
            if (packBox.Status == LogisticsOrderPackBoxStatus.Pack) {
                var packBoxStatusBefor = packBox.Status;
                packBox.Status = LogisticsOrderPackBoxStatus.Ship;
                var packBoxStatusAfter = packBox.Status;

                packBox.ShipTime = now;
                packBox.ShipUser = loginUser;

                session.Update(packBox);
                session.Flush();

                message = "箱号发货成功！";

                // 箱号日志还没有记录
                logisticsOrderPackBoxHistoryService.AddHistory(packBox, LogisticsOrderPackBoxHistoryType.Ship, packBoxStatusBefor, packBoxStatusAfter, loginUserId);

                // 检查是不是订单所有箱号都已经发货
                var orderPackBoxList = session.Query<LogisticsOrderPackBoxInfo>().Where(e => e.Order.Id == orderId).ToList();

                int notShipPackBoxCount = orderPackBoxList.Where(e => e.Status.IsShip() == false).Count();
                // 如果所有箱号都已经发货了
                if (notShipPackBoxCount == 0) {
                    order.OrderStatus = LogisticsOrderStatus.Shipped;
                    order.TransferTime = now;
                    session.Update(order);

                    logisticsOrderActionService.NewActionUser(orderId, LogisticsOrderActionType.Ship, loginUserId);

                    session.Flush();

                    message = "订单发货成功！";

                    customerMessageTransferedService.SendMessage(order.Customer, order);
                }

                session.Flush();
            } else {
                message = "箱号之前已发货成功！";
            }
        }

        string packBoxCode = logisticsOrderPackBoxService.GetBoxCode(packBox.Id);
        NHibernateUtil.Initialize(order.FirstMode);

        var model = new PackBoxShipModel {
            PackBoxCode = packBoxCode,
            OrderCode = order.Code,
            CustomerCode = order.CustomerCode,
            CountryName = order.Country.Name,
            FirstModeName = order.FirstMode.Name,
            ArrearsAmount = arrearsAmount.ToString("#0.00"),
            HasArrears = hasArrears,
        };

        return Json(new JsonResultData {
            Success = true,
            Message = message,
            Data = model,
        });
    }
}