using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Order.Models.OrderPayCoupon;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderPayCouponController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponService couponService;

    public OrderPayCouponController(ISession session,
        LoginCustomerService loginCustomerService,
        CouponService couponService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponService = couponService;
    }

    [HttpPost]
    [Route("/client/order/pay/coupon/has")]
    [Transactional] // 调用的方式里面有操作数据库的
    public IActionResult Has([FromBody] OrderPayCouponHasParam param) {
        ValidationUtil.Validate<OrderPayCouponHasParamValidator, OrderPayCouponHasParam>(param);

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        int orderId = param.OrderId;

        bool hasCoupon = couponService.HasCouponWhenPay(customerId: loginCustomerId, orderId: orderId);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new OrderPayCouponHasModel {
                HasCoupon = hasCoupon,
            },
        });
    }

    [HttpPost]
    [Route("/client/order/pay/coupon/list")]
    [Transactional] // 有可能的更新操作
    public IActionResult List([FromBody] OrderPayCouponListParam param) {
        ValidationUtil.Validate<OrderPayCouponListParamValidator, OrderPayCouponListParam>(param);

        // couponBatchService.CheckAndSetExpire(); Has() 的时候刚刚判断过，这里不判断也可以的s

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();

        int orderId = param.OrderId;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"指定的订单#{orderId}不属于当前登录客户#{loginCustomerId}");
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
            .Select(e => new OrderPayCouponListItem {
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
            orderCoupon.OrderCode = string.Empty;
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
            Data = new OrderPayCouponListModel {
                CouponCount = couponItemList.Count,
                CouponList = couponItemList,
                CouponId = orderCouponId,
            },
        });
    }
}