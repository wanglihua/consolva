using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Service.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class TestController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly CouponFollowService couponFollowService;

    public TestController(
        ISession session,
        CouponFollowService couponFollowService
    ) {
        this.session = session;
        this.couponFollowService = couponFollowService;
    }

    [HttpGet]
    [Route("/admin/test")]
    [Transactional]
    public IActionResult Index() {
        var customerList = session.Query<CustomerInfo>()
            .ToList();

        foreach (var customer in customerList) {
            couponFollowService.NewFellow(customer.Id);
        }

        session.Flush();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}