using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Message.CustomerMessage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class TestController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly CustomerMessageInternationalExpressTrackingNoService customerMessageInternationalExpressTrackingNoService;
    private readonly CustomerMessageInternationalExpressStateUpdateService customerMessageInternationalExpressStateUpdateService;
    private readonly CustomerMessageCouponFollowService customerMessageCouponFollowService;

    public TestController(
        ISession session,
        CustomerMessageInternationalExpressTrackingNoService customerMessageInternationalExpressTrackingNoService,
        CustomerMessageInternationalExpressStateUpdateService customerMessageInternationalExpressStateUpdateService,
        CustomerMessageCouponFollowService customerMessageCouponFollowService
    ) {
        this.session = session;
        this.customerMessageInternationalExpressTrackingNoService = customerMessageInternationalExpressTrackingNoService;
        this.customerMessageInternationalExpressStateUpdateService = customerMessageInternationalExpressStateUpdateService;
        this.customerMessageCouponFollowService = customerMessageCouponFollowService;
    }

    [HttpGet]
    [Route("/admin/test")]
    [Transactional]
    public IActionResult Index() {
        // 189
        // int orderId = 189;
        // var order = session.Get<LogisticsOrderInfo>(orderId);

        // customerMessageInternationalExpressTrackingNoService.SendMessage(order.Customer, order, "测试单号", "测试信息内容", "测试单号查询网址");
        // customerMessageInternationalExpressStateUpdateService.SendMessage(order.Customer, order, "测试单号", "测试单号更新内容");

        customerMessageCouponFollowService.SendMessage(session.Get<CustomerInfo>(1));

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}