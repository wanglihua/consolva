using Microsoft.AspNetCore.Mvc;
using NHibernate;
using shunda.Client.Areas.Order.Models.OrderDetail;
using shunda.Client.Services;
using shunda.Core;
using shunda.Core.Util;
using shunda.Core.Web;
using shunda.Domain.Express;
using shunda.Domain.Fee;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Express;
using ISession = NHibernate.ISession;

namespace shunda.Client.Areas.Order.Controllers;

[OrderArea]
public class OrderDetailController : Controller {
    private readonly ISession session;
    private readonly LoginCustomerService loginCustomerService;
    private readonly ExpressSizeWeightService expressSizeWeightService;
    private readonly ExpressService expressService;

    public OrderDetailController(
        ISession session,
        LoginCustomerService loginCustomerService,
        ExpressSizeWeightService expressSizeWeightService,
        ExpressService expressService
    ) {
        this.session = session;
        this.loginCustomerService = loginCustomerService;
        this.expressSizeWeightService = expressSizeWeightService;
        this.expressService = expressService;
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

        var expressList = expressService.GetExpressListForOrderDetail(orderId);

        IList<string> trackingNoList = expressList.Select(e => e.TrackingNo).ToList();
        IList<ExpressSizeWeightInfo> sizeWeightList = session.Query<ExpressSizeWeightInfo>()
            .Where(e => trackingNoList.Contains(e.TrackingNo))
            .ToList();

        int checkinPackageCount = expressSizeWeightService.GetTotalPackageCount(sizeWeightList);
        if (checkinPackageCount == 0) {
            checkinPackageCount = expressList.Sum(e => e.PackageCount);
        }

        NHibernateUtil.Initialize(order.Insurance);
        string insurance = order.Insurance.Name;

        NHibernateUtil.Initialize(order.Gst);
        string gst = order.Gst.Name;

        var repackType = order.RepackType.Name;

        var orderRemarkList = session.Query<LogisticsOrderRemarkInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.Id)
            .ToList();

        string customerRemark = string.Join(",", orderRemarkList.Where(e => e.Type == LogisticsOrderRemarkType.Customer).Select(e => e.Content));
        string warehouseRemark = string.Join(",", orderRemarkList.Where(e => e.Type == LogisticsOrderRemarkType.Warehouse).Select(e => e.Content));
        string serviceRemark = string.Join(",", orderRemarkList.Where(e => e.Type == LogisticsOrderRemarkType.Service).Select(e => e.Content));

        // 订单基本信息
        var orderDetailItem = new OrderDetailOrderItem {
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

            CustomerRemark = customerRemark,
            WarehouseRemark = warehouseRemark,
            ServiceRemark = serviceRemark,
        };

        // 物流信息
        var firstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.FirstMode.Id)
            .ThenBy(e => e.GoodsType)
            .ThenBy(e => e.Id)
            .ToList();

        var firstModeItemList = firstModeList
            .Select(e => new OrderDetailFirstModeListItem {
                Id = e.Id,
                ModeName = e.FirstMode.Name,
                GoodsType = e.GoodsType.Name,
                ModeType = e.FirstMode.Type.Name,
                Channel = e.FirstMode.Channel.Name,
            })
            .ToList();

        // 收货地址
        var orderReceiveAddress = session.Query<LogisticsOrderReceiveAddressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();
        if (orderReceiveAddress == null) {
            throw new ValidationException($"订单{order.Code}收货地址不存在！");
        }

        string receiveAddressDetail = orderReceiveAddress.Address;
        if (string.IsNullOrWhiteSpace(orderReceiveAddress.UnitLoc) == false) {
            receiveAddressDetail = $"{orderReceiveAddress.UnitLoc}, {receiveAddressDetail}";
        }

        var receiveAddressItem = new OrderDetailReceiveAddressItem {
            Country = orderReceiveAddress.Country.Name,
            LastName = orderReceiveAddress.LastName,
            FirstName = orderReceiveAddress.FirstName,
            Mobile = orderReceiveAddress.Mobile,
            State = orderReceiveAddress.State,
            City = orderReceiveAddress.City,
            Address = receiveAddressDetail,
            Zipcode = orderReceiveAddress.Zipcode,
        };

        // 发货地址
        var orderSendAddress = session.Query<LogisticsOrderSendAddressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();
        if (orderSendAddress == null) {
            throw new ValidationException($"订单{order.Code}发货地址不存在！");
        }

        NHibernateUtil.Initialize(orderSendAddress.Province);
        NHibernateUtil.Initialize(orderSendAddress.City);
        NHibernateUtil.Initialize(orderSendAddress.Area);
        NHibernateUtil.Initialize(orderSendAddress.Street);

        var sendAddressItem = new OrderDetailSendAddressItem {
            Name = orderSendAddress.Name,
            Mobile = orderSendAddress.Mobile,
            Province = orderSendAddress.Province.Name,
            City = orderSendAddress.City.Name,
            Area = orderSendAddress.Area.Name,
            Street = orderSendAddress.Street.Name,
            Address = orderSendAddress.Address,
            Zipcode = orderSendAddress.Zipcode,
        };

        // 订单商品
        var expressItemList = new List<OrderDetailExpressItem>();

        foreach (var express in expressList) {
            var expressItem = new OrderDetailExpressItem {
                Id = express.Id,
                TrackingNo = express.TrackingNo,
                CheckinTrackingNo = express.CheckinTrackingNo,
                GoodsType = express.GoodsType.Name,
                Status = express.Status.Name,
                CheckinTime = express.AddTime.ToString("yyyy-MM-dd HH:mm"),
                GoodsValue = express.GoodsValue.ToString("#0.00"),
            };

            var expressGoodsItemList = new List<OrderDetailExpressGoodsItem>();
            foreach (var expressGoods in express.ExpressGoodsListCache) {
                var expressGoodsItem = new OrderDetailExpressGoodsItem {
                    Id = expressGoods.Id,
                    GoodsName = expressGoods.Goods.Name,
                    GoodsType = expressGoods.GoodsType.Name,
                    Quantity = expressGoods.Quantity,
                    Unit = expressGoods.Unit,
                };

                expressGoodsItemList.Add(expressGoodsItem);
            }

            expressItem.ExpressGoodsList = expressGoodsItemList;

            expressItemList.Add(expressItem);
        }

        // 订单打包
        var orderPackList = session.Query<LogisticsOrderPackInfo>()
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.OrderFirstMode.Id)
            .ThenBy(e => e.Id)
            .ToList();

        // 这个循环里面，很多的 order first mode 都是相同的，所以也不大会影响性能
        foreach (var orderPack in orderPackList) {
            NHibernateUtil.Initialize(orderPack.OrderFirstMode);
            NHibernateUtil.Initialize(orderPack.OrderFirstMode.FirstMode);
        }

        var orderPackItemList = new List<OrderDetailOrderPackItem>();
        foreach (var orderPack in orderPackList) {
            var orderPackItem = new OrderDetailOrderPackItem {
                Id = orderPack.Id,
                FirstModeName = orderPack.OrderFirstMode.FirstMode.Name,
                GoodsType = orderPack.GoodsType.Name,
                SingleSize = $"{orderPack.Length}×{orderPack.Width}×{orderPack.Height}",
                SingleVolume = ((orderPack.Length * orderPack.Width * orderPack.Height) / 100_00_00).ToString("#0.000000"),
                SingleWeight = orderPack.Weight.ToString("#0.000"),
                PackageCount = orderPack.PackageCount,
            };

            orderPackItemList.Add(orderPackItem);
        }

        // 打包汇总数据
        var orderPackData = session.Query<LogisticsOrderPackDataInfo>()
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();

        var orderPackDataItem = new OrderDetailOrderPackDataItem {
            TotalActualWeight = orderPackData?.TotalActualWeight.ToString("#0.000") ?? string.Empty,
            TotalVolumeWeight = orderPackData?.TotalVolumeWeight.ToString("#0.000") ?? string.Empty,
            TotalVolume = orderPackData?.TotalVolume.ToString("#0.000000") ?? string.Empty,
            TotalPackageCount = orderPackData?.TotalPackageCount.ToString() ?? string.Empty,
            MaxSideLength = orderPackData?.MaxSideLength.ToString("#0.0") ?? string.Empty,
            MaxVolumeSinglePack = orderPackData?.MaxVolumeSinglePack.ToString("#0.000000") ?? string.Empty,
            MaxWeightSinglePack = orderPackData?.MaxWeightSinglePack.ToString("#0.000") ?? string.Empty,
        };
        
        // 订单费用

        // 订单详细数据
        var model = new OrderDetailModel {
            Order = orderDetailItem,
            FirstMode = new OrderDetailFirstModeItem {
                FirstModeList = firstModeItemList,
            },
            ReceiveAddress = receiveAddressItem,
            SendAddress = sendAddressItem,
            ExpressList = expressItemList,
            OrderPackList = orderPackItemList,
            HasOrderPackData = orderPackData != null,
            OrderPackData = orderPackDataItem,
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