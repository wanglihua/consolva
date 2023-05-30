using Microsoft.AspNetCore.Mvc;
using NLog;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Message.CustomerMessage;
using ISession = NHibernate.ISession;

namespace shunda.Admin.Controllers;

[NoLoginRequired]
public class TestController : Controller {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly ISession session;
    private readonly CouponService couponService;

    public TestController(
        ISession session,
        CouponService couponService
    ) {
        this.session = session;
        this.couponService = couponService;
    }

    [HttpGet]
    [Route("/admin/test")]
    [Transactional]
    public IActionResult Index() {
        var result = couponService.CalculateCouponDiscount(3927, 703);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}