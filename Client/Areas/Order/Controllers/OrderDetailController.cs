using Microsoft.AspNetCore.Mvc;
using NHibernate;
using shunda.Client.Areas.Order.Models.OrderDetail;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderDetailController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly ExpressSizeWeightService expressSizeWeightService;

    public OrderDetailController(
        ISession session,
        LoginCustomerService loginCustomerService,
        ExpressSizeWeightService expressSizeWeightService) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.expressSizeWeightService = expressSizeWeightService;
    }

    [HttpPost]
    [Route("/client/order/detail")]
    public IActionResult Detail([FromBody] OrderDetailParam param) {
        ValidationUtil.Validate<OrderDetailParamValidator, OrderDetailParam>(param);

        int orderId = param.OrderId;
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        int loginCustomerId = loginCustomerService.GetLoginCustomerId();
        if (order.Customer.Id != loginCustomerId) {
            throw new ValidationException($"订单{order.Code}不属于当前登录客户！");
        }

        IList<LogisticsOrderPackDataInfo> orderPackDataList = session.Query<LogisticsOrderPackDataInfo>()
            .Where(e => e.Order.Id == orderId)
            .ToList();

        IList<ExpressInfo> expressList = session.Query<ExpressInfo>().Where(e => e.Order!.Id == orderId).ToList();

        IList<string> trackingNoList = expressList.Select(e => e.TrackingNo).ToList();
        IList<ExpressSizeWeightInfo> sizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        var orderPackData = orderPackDataList
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();

        int checkinPackageCount = expressSizeWeightService.GetTotalPackageCount(sizeWeightList);
        if (checkinPackageCount == 0) {
            checkinPackageCount = expressList.Sum(e => e.PackageCount);
        }

        NHibernateUtil.Initialize(order.Insurance);
        string insurance = order.Insurance.Name;

        NHibernateUtil.Initialize(order.Gst);
        string gst = order.Gst.Name;

        var repackType = order.RepackType.Name;

        // 订单基本信息
        var orderDetailItem = new OrderDetailItem {
            Id = order.Id,
            OrderCode = order.Code,
            OrderStatus = order.OrderStatus.Name,
            HasPacked = order.OrderStatus.HasPacked(),
            GoodsValue = order.GoodsValue.ToString("#0.00"),
            OrderAmount = order.OrderAmount.ToString("#0.00"),
            ArearsAmount = order.ArrearsAmountFormula.ToString("#0.00"),

            HasCheckinSizeWeightData = sizeWeightList.Count != 0,
            CheckinTotalPackageCount = checkinPackageCount.ToString(),
            CheckinTotalVolume = expressSizeWeightService.GetTotalVolume(sizeWeightList).ToString("#0.000000"),
            CheckinTotalVolumeWeight = expressSizeWeightService.GetTotalVolumeWeight(sizeWeightList).ToString("#0.000"),
            CheckinTotalActualWeight = expressSizeWeightService.GetTotalActualWeight(sizeWeightList).ToString("#0.000"),
            CheckinMaxSideLength = expressSizeWeightService.GetMaxSizeLength(sizeWeightList).ToString("#0.0"),

            HasPackedSizeWeightData = orderPackData != null,
            PackedTotalPackageCount = orderPackData?.TotalPackageCount.ToString() ?? string.Empty,
            PackedTotalVolume = orderPackData?.TotalVolume.ToString("#0.000000") ?? string.Empty,
            PackedTotalVolumeWeight = orderPackData?.TotalVolumeWeight.ToString("#0.000000") ?? string.Empty,
            PackedTotalActualWeight = orderPackData?.TotalActualWeight.ToString("#0.000000") ?? string.Empty,
            PackedMaxSideLength = orderPackData?.MaxSideLength.ToString("#0.0") ?? string.Empty,

            FirstModeType = order.FirstModeType.Name,
            FirstModeName = order.FirstMode.Name,
            CreateTime = order.CreateTime.ToString("yyyy-MM-dd HH:mm"),
            PackTime = order.CompletePackTime?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty,
            TransferTime = order.TransferTime?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty,
            DeliverTime = order.DeliverTime?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty,
            ShouldPay = order.ArrearsAmountFormula > 0,
            Paid = order.PayStatus == PayStatus.Paid,
            CanCancel = order.PayStatus == PayStatus.NotPaid,

            AirExpressTrackingNo = string.Empty, // todo 暂时为空，后面还要改      

            Insurance = insurance,
            Gst = gst,
            RepackType = repackType,
        };

        var model = new OrderDetailModel {
            Order = orderDetailItem,
        };

        return Json(new JsonResultData {
            Success = true,
            Message = "操作成功！",
            Data = new {
                orderDetail = model,
            },
        });
    }
}