using Microsoft.AspNetCore.Mvc;
using NHibernate.Linq;
using shunda.Client.Areas.Coupon.Models.CouponPay;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Hibernate;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Coupon;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Fee;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Coupon.Controllers;

/// <summary>
/// 这个基本没用起来，后面删除掉
/// </summary>
[CouponArea]
public class CouponPayController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly CouponService couponService;
    private readonly CouponBatchService couponBatchService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;

    public CouponPayController(ISession session,
        LoginCustomerService loginCustomerService,
        CouponService couponService,
        CouponBatchService couponBatchService,
        LogisticsOrderPaySumService logisticsOrderPaySumService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.couponService = couponService;
        this.couponBatchService = couponBatchService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
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

        int orderId = param.OrderId;
        int selectCouponId = param.CouponId;

        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"订单#{orderId}不属于当前登录用户！");
        }

        CouponInfo? selectCoupon = null;
        if (selectCouponId != 0) {
            selectCoupon = session.Get<CouponInfo>(selectCouponId);
            if (selectCoupon == null) {
                throw new ValidationException($"指定的优惠券#{selectCouponId}不存在！");
            }

            // 检查有效性
            couponService.CheckCouponValidWhenPay(couponId: selectCouponId, orderId: orderId, customerId: loginCustomerId);
        }

        var orderCoupon = session.Query<CouponInfo>()
            .Where(e => e.Order!.Id == orderId)
            .Take(1)
            .SingleOrDefault();

        if ((selectCoupon?.Id ?? 0) == (orderCoupon?.Id ?? 0)) {
            // 如果当前的选择就是原来的，那就什么都不用做
            return Json(new Dictionary<string, object>() {
                { "success", true },
                { "message", "优惠券选择未修改，无需保存！" },
            });
        }

        var orderFeeList = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        var logisticsFeeTypeList = LogisticsOrderFeeType.GetLogisticsFeeTypeList();
        var firstAndSecondFeeList = orderFeeList.Where(e => logisticsFeeTypeList.Contains(e.Type)).ToList();
        decimal firstAndSecondFee = 0.0m;
        foreach (var fee in firstAndSecondFeeList) {
            firstAndSecondFee += fee.Fee;
        }

        decimal couponDiscount = 0m;
        if (selectCoupon != null) {
            var couponDiscountResult = couponService.CalculateCouponDiscount(selectCouponId, firstAndSecondFee);
            if (couponDiscountResult.Success == false) {
                throw new ValidationException(couponDiscountResult.Message);
            }

            couponDiscount = couponDiscountResult.Discount;
        }

        // 把原来的取消掉
        if (orderCoupon != null) {
            couponService.setCouponNotUsed(orderId: orderId);
        }

        // 把新的设置上
        if (selectCoupon != null) {
            couponService.SetCouponUsed(selectCouponId, order, couponDiscount);
        }

        var orderCouponBatchId = orderCoupon?.Batch.Id ?? 0;
        if (orderCouponBatchId != 0) {
            couponBatchService.SumAndSaveBatchTotalUsed(orderCouponBatchId);
        }

        var couponBatchId = selectCoupon?.Batch.Id ?? 0;
        if (couponBatchId != 0) {
            couponBatchService.SumAndSaveBatchTotalUsed(couponBatchId);
        }

        // 先把原来的优惠券费用删除掉
        var couponOrderFeeList = orderFeeList
            .Where(e => e.Type == LogisticsOrderFeeType.CouponDiscountFee)
            .ToList();
        if (couponOrderFeeList.Count != 0) {
            foreach (var couponOrderFee in couponOrderFeeList) // 一般也只有一个
            {
                session.Delete(couponOrderFee);
            }
        }

        session.Flush();

        if (selectCoupon != null) {
            // 插入新的优惠券费用
            var couponFee = new LogisticsOrderFeeInfo();
            couponFee.Customer = order.Customer;
            couponFee.Order = order;
            couponFee.OrderCode = order.Code;
            couponFee.GoodsType = LogisticsGoodsType.None;
            couponFee.Type = LogisticsOrderFeeType.CouponDiscountFee;
            couponFee.Description = "优惠券实扣," + (selectCoupon.Code);
            couponFee.Fee = -couponDiscount;

            session.Save(couponFee);
            session.Flush();
        }

        // 更新订单总费用和支付状态
        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(order.Id);

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
        });
    }
}