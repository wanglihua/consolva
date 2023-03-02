using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using shunda.Service.Express;
using shunda.Service.Logistics;

namespace shunda.Service.LogisticsOrder;

[Service(ServiceType.Scoped)]
public class LogisticsOrderCreateService {
    private readonly ISession session;
    private readonly LogisticsOrderCodeService logisticsOrderCodeService;
    private readonly CustomerCodeService customerCodeService;
    private readonly LogisticsGoodsTypeService logisticsGoodsTypeService;
    private readonly LogisticsOrderStorageLocService logisticsOrderStorageLocService;
    private readonly ExpressLogService expressLogService;
    private readonly LogisticsOrderActionService logisticsOrderActionService;
    private readonly LogisticsOrderPackService logisticsOrderPackService;

    public LogisticsOrderCreateService(ISession session,
        LogisticsOrderCodeService logisticsOrderCodeService,
        CustomerCodeService customerCodeService,
        LogisticsGoodsTypeService logisticsGoodsTypeService,
        LogisticsOrderStorageLocService logisticsOrderStorageLocService,
        ExpressLogService expressLogService,
        LogisticsOrderActionService logisticsOrderActionService,
        LogisticsOrderPackService logisticsOrderPackService
    ) {
        this.session = session;
        this.logisticsOrderCodeService = logisticsOrderCodeService;
        this.customerCodeService = customerCodeService;
        this.logisticsGoodsTypeService = logisticsGoodsTypeService;
        this.logisticsOrderStorageLocService = logisticsOrderStorageLocService;
        this.expressLogService = expressLogService;
        this.logisticsOrderActionService = logisticsOrderActionService;
        this.logisticsOrderPackService = logisticsOrderPackService;
    }

    /// <summary>
    /// 创建订单
    /// </summary>
    public LogisticsOrderInfo CreateOrder(int customerId) {
        var customer = session.Get<CustomerInfo>(customerId);
        if (customer == null) {
            throw new ValidationException($"指定的客户#{customerId}不存在！");
        }

        LogisticsOrderCreateSelectInfo createSelect = session.Get<LogisticsOrderCreateSelectInfo>(customerId);
        if (createSelect == null) {
            throw new ValidationException("订单创建数据不存在，请先在前面步骤录入创建数据！");
        }

        if (createSelect.Valid == false) {
            throw new ValidationException("订单创建数据在确认前，又在别的界面被修改过，数据已失效！");
        }

        // 检查内部费率问题
        int firstModeId = createSelect.FirstModeId;
        var firstMode = session.Get<LogisticsModeInfo>(firstModeId);
        if (firstMode == null) {
            throw new ValidationException($"指定的物流方式#{firstModeId}不存在！");
        }

        if (firstMode.Cate == LogisticsModeCate.Inner) {
            throw new Exception($"当前选择的物流方式{firstMode.Name}为内部费率，不能用于创建订单。可能是系统问题造成错误选择，请重新选择再创建订单！");
        }

        // 检查收件人和发货人是否一样
        int addressReceiveId = createSelect.AddressReceiveId;
        CustomerReceiveAddressInfo addressReceive = session.Get<CustomerReceiveAddressInfo>(addressReceiveId);
        if (addressReceive == null) {
            throw new Exception("订单创建收货地址未选择！");
        }

        if (createSelect.Country != firstMode.Country || createSelect.Country != addressReceive.Country) {
            throw new ValidationException("订单创建选择物流方式收货地址国家不一致，数据有问题，请重新选择！");
        }

        /*
        int addressSendId = createSelect.AddressSendId;
        CustomerSendAddressInfo addressSend = session.Get<CustomerSendAddressInfo>(addressSendId);
        if (addressSend == null) {
            throw new Exception("订单创建发货地址未选择！");
        }

        string receiverNameToCompare = StringUtil.GetPinyin(addressReceive.LastName + addressReceive.FirstName);
        receiverNameToCompare = receiverNameToCompare.Replace(" ", "").ToLower();

        string senderNameToCompare = StringUtil.GetPinyin(addressSend.Name);
        senderNameToCompare = senderNameToCompare.Replace(" ", "").ToLower();

        if (receiverNameToCompare == senderNameToCompare) {
            throw new ValidationException("因海关要求，收货人与发货人，姓名不可以相同，请在【我的地址】修改！");
        }
        */

        // 保险和GST
        var insurance = session.Get<LogisticsInsuranceInfo>(createSelect.InsuranceId);
        if (insurance == null) {
            throw new ValidationException($"指定的保险方案#{createSelect.InsuranceId}不存在！");
        }

        var gst = session.Get<LogisticsGstInfo>(createSelect.GstId);
        if (gst == null) {
            throw new ValidationException($"指定的GST方案#{createSelect.GstId}不存在！");
        }

        // 快递单列表
        var createSelectExpressList = session.Query<LogisticsOrderCreateSelectExpressInfo>()
            .Where(e => e.CustomerId == customerId)
            .ToList();

        IList<int> expressIdList = createSelectExpressList.Select(e => e.ExpressId).ToList();

        var expressList = session.Query<ExpressInfo>()
            .Where(e => expressIdList.Contains(e.Id))
            .OrderBy(e => e.Id)
            .ToList();
        if (expressList.Count == 0) {
            throw new Exception("快递单列表不能为空！");
        }

        // 检查禁止类物品类型
        foreach (var express in expressList) {
            if (express.GoodsType == LogisticsGoodsType.Forbidden) {
                throw new Exception("禁止类物品不能创建订单！");
            }
        }

        // 检查快递单号状态
        foreach (var express in expressList) {
            if (ExpressStatus.IsOrderCreate(express.Status)) {
                throw new ValidationException($"快递单号 {express.TrackingNo} 已创建订单，不能再次创建订单！");
            }
        }

        // 检查快递单收货仓库是否一致，一般当前都是一样的
        var warehouse = expressList.First().Warehouse;
        int warehouseId = warehouse.Id;
        foreach (var express in expressList) {
            if (express.Warehouse.Id != warehouseId) {
                throw new Exception("快递单收货仓库不一致，请选择同一个收货仓库的快递单号创建订单！");
            }
        }

        // 检查快递单货品总值
        foreach (var express in expressList) {
            if (express.GoodsValue <= 0) {
                string trackingNo = express.TrackingNo;
                throw new Exception($"快递单号 {trackingNo} 货品总值没有设置，请修改后继续！");
            }
        }

        // 快递单物品列表
        var expressGoodsList = session.Query<ExpressGoodsInfo>()
            .Where(e => expressIdList.Contains(e.Express.Id))
            .ToList();
        foreach (var express in expressList) {
            int expressId = express.Id;
            var expressGoods = expressGoodsList.Where(e => e.Express.Id == expressId).Take(1).SingleOrDefault();
            if (expressGoods == null) {
                throw new ValidationException($"快递单{express.TrackingNo}物品列表不能为空！");
            }
        }

        string orderCode = logisticsOrderCodeService.GenerateNewCode(customerId);
        string customerCode = customerCodeService.GetCustomerCode(customerId);
        LogisticsGoodsType goodsType = logisticsGoodsTypeService.GetGoodsTypeFromGoodsTypeList(expressList.Select(e => e.GoodsType).Distinct().ToList());
        var orderStorageLoc = logisticsOrderStorageLocService.GetOrderStorageLoc(expressList);
        decimal goodsValue = expressList.Sum(e => e.GoodsValue);
        LogisticsRepackType repackType = createSelect.RepackType;

        var order = new LogisticsOrderInfo {
            Code = orderCode,
            Country = createSelect.Country,
            Customer = customer,
            CustomerCode = customerCode,
            FeeMethod = LogisticsOrderFeeMethod.Pack, // 当前创建的订单都是打包后收费的
            CreateMethod = LogisticsOrderCreateMethod.CustomerCreate,
            Warehouse = warehouse,
            FirstModeType = createSelect.FirstType,
            FirstMode = firstMode,
            Insurance = insurance,
            Gst = gst,
            GoodsType = goodsType,
            OrderStatus = LogisticsOrderStatus.Create,
            PayStatus = PayStatus.NotPaid, // 这里改成 None 也是可以的
            PayType = PayType.None,
            GoodsValue = goodsValue,
            OrderAmount = 0m, // 打包时更新这个值
            PaidAmount = 0m,
            RepackType = repackType,
            StorageLoc = orderStorageLoc.FirstStorageLoc,
            StorageLocCount = orderStorageLoc.StorageLocCount,
            PickupCode = string.Empty, // 这个应该没用起来

            // 其他的都是可空字段
        };

        session.Save(order);
        session.Flush();

        // 订单地址
        var orderReceiveAddress = new LogisticsOrderReceiveAddressInfo {
            Order = order,
            Country = addressReceive.Country,
            FirstName = addressReceive.FirstName,
            LastName = addressReceive.LastName,
            Mobile = addressReceive.Mobile,
            State = addressReceive.State,
            City = addressReceive.City,
            Address = addressReceive.Address,
            UnitLoc = addressReceive.UnitLoc,
            Zipcode = addressReceive.Zipcode,
            Tfn = addressReceive.Tfn,
            Company = addressReceive.Company,
        };

        session.Save(orderReceiveAddress);

        session.Flush();

        // 订单客户备注
        if (string.IsNullOrWhiteSpace(createSelect.Remark) == false) {
            var orderRemark = new LogisticsOrderRemarkInfo {
                Order = order,
                Type = LogisticsOrderRemarkType.Customer,
                Content = createSelect.Remark,
            };

            session.Save(orderRemark);

            // 一定要清空原来的备注，防止下一个订单中
            createSelect.Remark = string.Empty;
            session.Update(createSelect);

            session.Flush();
        }

        /*
        var orderSendAddress = new LogisticsOrderSendAddressInfo {
            Order = order,
            Country = addressSend.Country,
            Name = addressSend.Name,
            Mobile = addressSend.Mobile,
            Province = addressSend.Province,
            City = addressSend.City,
            Area = addressSend.Area,
            Street = addressSend.Street,
            Address = addressSend.Address,
            Zipcode = addressSend.Zipcode,
            Tfn = addressSend.Tfn,
            Company = addressSend.Company,
        };

        session.Save(orderSendAddress);

        session.Flush();
        */

        // 订单快递单关联，当订单作废，或者作废订单恢复的时候，这个关联就是必需的了
        foreach (var express in expressList) {
            var orderExpress = new LogisticsOrderExpressInfo {
                Order = order,
                Express = express,
            };

            session.Save(orderExpress);
        }

        session.Flush();

        // 更新快递单状态
        foreach (var express in expressList) {
            express.Order = order;
            express.Status = ExpressStatus.Order;

            session.Update(express);
        }

        session.Flush();

        // 订单创建 快递单日志
        foreach (var express in expressList) {
            expressLogService.CustomerLog(express.Id, ExpressLogType.Order, customerId);
        }

        // 订单创建 订单日志
        logisticsOrderActionService.NewActionCustomer(order.Id, LogisticsOrderActionType.CustomerCreateOrder, order.OrderStatus, order.OrderStatus, order.PayStatus, order.PayStatus, customerId);

        // 清除快递单选择列表
        foreach (var createSelectExpress in createSelectExpressList) {
            session.Delete(createSelectExpress);
        }

        session.Flush();

        return order;
    }

    /// <summary>
    /// 创建订单并提交打包
    /// </summary>
    public LogisticsOrderInfo CreateOrderAndSubmitPack(int customerId) {
        // 创建订单
        var order = CreateOrder(customerId);

        // 提交打包
        int orderId = order.Id;
        logisticsOrderPackService.SubmitPack(orderId, customerId);

        return order;
    }
}