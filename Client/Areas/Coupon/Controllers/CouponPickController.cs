using Microsoft.AspNetCore.Mvc;
using shunda.Client.Areas.Coupon.Models.CouponPick;
using shunda.Client.Services;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Service.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

public class CouponPickController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponBatchService couponBatchService;

    public CouponPickController(ISession session,
        LoginCustomerService loginCustomerService,
        CouponBatchService couponBatchService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponBatchService = couponBatchService;
    }

    [HttpPost]
    [Route("/client/coupon/pick/list")]
    public IActionResult List([FromBody] CouponPickListParam param) {
        DateTime today = DateTime.Today;
        var batchList = session.Query<CouponBatchInfo>()
            .Where(e => e.Status == CouponBatchStatus.Release
                        && e.Expire > today)
            .OrderByDescending(e => e.Id)
            .ToList();

        var batchItemList = batchList
            .Select(e => new CouponPickListItem {
                Id = e.Id,
                Name = e.Name,
                Type = e.Type.Name,
                Rule = e.Rule,
                RuleDesc = e.RuleDesc,
                Expire = e.Expire.ToString("yyyy-MM-dd"),
            })
            .ToList();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CouponPickListModel {
                BatchList = batchItemList,
                BatchCount = batchList.Count,
            },
        });
    }

    [HttpPost]
    [Route("/client/coupon/pick/save")]
    public IActionResult Save([FromBody] CouponPickSaveParam param) {
        int batchId = param.BatchId;
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        couponBatchService.Pick(batchId: batchId, customerId: loginCustomerId);

        return Json(new JsonResultData {
            Success = true,
            Message = "优惠券领取成功！",
        });
    }
}