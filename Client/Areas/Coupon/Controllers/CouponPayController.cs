using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Coupon.Models.CouponPay;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

[CouponArea]
public class CouponPayController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponService couponService;
    private readonly CouponBatchService couponBatchService;

    public CouponPayController(ISession session,
        LoginCustomerService loginCustomerService,
        CouponService couponService,
        CouponBatchService couponBatchService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponService = couponService;
        this.couponBatchService = couponBatchService;
    }

    [HttpPost]
    [Route("/client/coupon/pay/list")]
    [Transactional]
    public IActionResult List([FromBody] CouponPayListParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        couponBatchService.CheckAndSetExpire();

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int orderId = param.OrderId;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单#{orderId}不属于当前登录会员#{loginCustomerId}");
        }

        var orderCoupon = session.Query<CouponInfo>()
            .Where(e => e.Order!.Id == orderId)
            .Take(1)
            .SingleOrDefault();

        int orderCouponId = orderCoupon?.Id ?? 0;

        var couponList = session.Query<CouponInfo>()
            .Fetch(e => e.Batch)
            .Where(e => e.Customer.Id == loginCustomerId
                        && (e.Status == CouponStatus.Pick || e.Order!.Id == orderId))
            .OrderByDescending(e => e.Id)
            .ToList();
        IList<int> couponIdList = couponList.Select(e => e.Id).ToList();

        var couponItemList = couponList
            .Select(e => new CouponPayListItem {
                Id = e.Id,
                Code = e.Code,
                Type = e.Batch.Type.Name,
                Rule = e.Batch.Rule,
                RuleDesc = e.Batch.RuleDesc,
                Expire = e.Batch.Expire.ToString("yyyy-MM-dd HH:mm")
            }).ToList();

        if (orderCoupon != null && couponIdList.Contains(orderCouponId) == false) {
            orderCouponId = 0;

            orderCoupon.Order = null;
            orderCoupon.UseTime = null;

            if (orderCoupon.Batch.Status == CouponBatchStatus.Expire) {
                orderCoupon.Status = CouponStatus.Expire;
            } else {
                orderCoupon.Status = CouponStatus.Pick;
            }

            session.Update(orderCoupon);
            session.Flush();
        }

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new CouponPayListModel {
                CouponList = couponItemList,
                CouponId = orderCouponId,
            },
        });
    }

    [HttpPost]
    [Route("/client/coupon/pay/save")]
    [Transactional]
    public IActionResult Save([FromBody] CouponPaySaveParam param) {
        if (ModelState.IsValid == false) {
            throw new ValidationException(ValidationUtil.GetErrorMessagesForMobile(ModelState));
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}