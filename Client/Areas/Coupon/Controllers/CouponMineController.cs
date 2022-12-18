using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Coupon.Models.CouponMine;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Service.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

[CouponArea]
public class CouponMineController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponBatchService couponBatchService;

    public CouponMineController(ISession session,
        LoginCustomerService loginCustomerService,
        CouponBatchService couponBatchService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponBatchService = couponBatchService;
    }

    [HttpPost]
    [Route("/client/coupon/mine/list")]
    public IActionResult List([FromBody] CouponMineListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        couponBatchService.CheckAndSetExpire();

        session.Query<CouponInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && e.Status == CouponStatus.Pick)
            .ToList();
        
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}