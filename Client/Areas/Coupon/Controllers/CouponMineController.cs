using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
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
        couponBatchService.CheckAndSetExpirePickBatch();

        var couponList = session.Query<CouponInfo>()
            .Fetch(e => e.Batch)
            .Where(e => e.Customer.Id == loginCustomerId
                        && e.Status == CouponStatus.Pick)
            .OrderByDescending(e => e.Id)
            .ToList();

        var couponItemList = couponList
            .Select(e => new CouponMineListItem {
                Id = e.Id,
                Code = e.Code,
                Type = e.Batch.Type.Name,
                Rule = e.Batch.Rule,
                RuleDesc = e.Batch.RuleDesc,
                Expire = e.Batch.Expire.ToString("yyyy-MM-dd"),
            })
            .ToList();

        int couponCount = couponItemList.Count;

        var hasBatchToPick = couponBatchService.HasBatchToPick(loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CouponMineListModel {
                CouponList = couponItemList,
                CouponCount = couponCount,
                HasBatchToPick = hasBatchToPick,
            },
        });
    }
}