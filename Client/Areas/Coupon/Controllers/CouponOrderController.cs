using Microsoft.AspNetCore.Mvc;
using NHibernate.Persister.Entity;
using shunda.Client.Areas.Coupon.Models.CouponOrder;
using shunda.Client.Services;
using shunda.Core.Hibernate;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.LogisticsOrder;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

[CouponArea]
public class CouponOrderController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponBatchService couponBatchService;
    private readonly CouponService couponService;
    private readonly LogisticsOrderCreateSelectService logisticsOrderCreateSelectService;

    public CouponOrderController(
        ISession session,
        LoginCustomerService loginCustomerService,
        CouponBatchService couponBatchService,
        CouponService couponService,
        LogisticsOrderCreateSelectService logisticsOrderCreateSelectService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponBatchService = couponBatchService;
        this.couponService = couponService;
        this.logisticsOrderCreateSelectService = logisticsOrderCreateSelectService;
    }

    [HttpPost]
    [Route("/client/coupon/order/list")]
    [Transactional]
    public IActionResult List([FromBody] CouponOrderListParam param) {
        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        var couponList = session.Query<CouponInfo>()
            .Where(e => e.Customer.Id == loginCustomerId && e.Status == CouponStatus.Pick)
            .OrderByDescending(e => e.Id)
            .ToList();

        var couponItemList = couponList.Select(e => new CouponOrderListItem {
            Id = e.Id,
            Code = e.Code,
            Type = e.Batch.Type.Name,
            Rule = e.Batch.Rule,
            RuleDesc = e.Batch.RuleDesc,
            Expire = e.Batch.Expire.ToString("yyyy-MM-dd HH:mm")
        }).ToList();

        // 之前选择的 优惠券 如果不在当前可用优惠券列表中，就置空，选择已无效
        var createSelect = session.Get<LogisticsOrderCreateSelectInfo>(loginCustomerId);
        int preSelCouponId = createSelect?.CouponId ?? 0;
        if (createSelect != null) {
            IList<int> couponIdList = couponList.Select(e => e.Id).ToList();
            if (preSelCouponId != 0 && couponIdList.Contains(preSelCouponId) == false) {
                preSelCouponId = 0;
                createSelect.CouponId = 0;
                session.Update(createSelect);
                session.Flush();
            }
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CouponOrderListModel {
                CouponList = couponItemList,
                CouponId = preSelCouponId,
            }
        });
    }

    [HttpPost]
    [Route("/client/coupon/order/save")]
    public IActionResult Save([FromBody] CouponOrderSaveParam param) {
        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}