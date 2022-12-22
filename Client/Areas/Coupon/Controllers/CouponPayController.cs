using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Coupon.Models.CouponPay;
using shunda.Client.Services;
using shunda.Core.Web;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

[CouponArea]
public class CouponPayController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;

    public CouponPayController(ISession session, LoginCustomerService loginCustomerService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
    }

    [HttpPost]
    [Route("/client/coupon/pay/list")]
    public IActionResult List([FromBody] CouponPayListParam param) {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }

    [HttpPost]
    [Route("/client/coupon/pay/save")]
    public IActionResult Save([FromBody] CouponPaySaveParam param) {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}