using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsOrderFeeService {
    private readonly ISession session;
    private readonly LogisticsFeeService logisticsFeeService;
    private readonly CustomerVipDiscountService customerVipDiscountService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;

    public LogisticsOrderFeeService(ISession session,
        LogisticsFeeService logisticsFeeService,
        CustomerVipDiscountService customerVipDiscountService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        CustomerReceiveAddressService customerReceiveAddressService
    ) {
        this.session = session;
        this.logisticsFeeService = logisticsFeeService;
        this.customerVipDiscountService = customerVipDiscountService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.customerReceiveAddressService = customerReceiveAddressService;
    }

    /// <summary>
    /// 计算订单物流费用，并返回计算结果
    /// </summary>
    public LogisticsFeeResult Calculate(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        int customerId = order.Customer.Id;

        CustomerSuperVipDiscountInfo? superVipDiscount = session.Query<CustomerSuperVipDiscountInfo>()
            .Where(e => e.Customer.Id == customerId)
            .Take(1)
            .SingleOrDefault();

        CustomerVipDiscountInfo? vipDiscount = null;
        // 不是超级VIP的才需要查看VIP折扣
        if (superVipDiscount == null) {
            vipDiscount = customerVipDiscountService.GetDiscount(customerId);
        }

        var feeDataList = OrderToFeeDataList(orderId);
        foreach (var feeData in feeDataList) {
            ValidationUtil.Validate<LogisticsFeeDataValidator, LogisticsFeeData>(feeData);
        }

        return new LogisticsFeeResult {
        };
    }

    /// <summary>
    /// 订单数据转计费数据
    /// </summary>
    /// <returns>计费数据列表，订单一个物流方式一个计费数据</returns>
    private IList<LogisticsFeeData> OrderToFeeDataList(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Fetch(e => e.FirstMode)
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.GoodsType)
            .ThenBy(e => e.Id)
            .ToList();

        var orderPackList = session.Query<LogisticsOrderPackInfo>()
            .Fetch(e => e.OrderFirstMode)
            .Where(e => e.Order.Id == orderId)
            .ToList();

        var orderFirstModeListFromPack = orderPackList
            .Select(e => e.OrderFirstMode)
            .Distinct()
            .OrderBy(e => e.GoodsType)
            .ThenBy(e => e.Id)
            .ToList();

        if (orderFirstModeList.Count != orderFirstModeListFromPack.Count) {
            throw new ValidationException("订单头程物流方式记录和订单打包数据不匹配，数据有问题！");
        }

        for (int index = 0; index < orderFirstModeList.Count; index++) {
            var orderFirstMode = orderFirstModeList[index];
            var orderFirstModeFromPack = orderFirstModeListFromPack[index];
            if (orderFirstMode.Id != orderFirstModeFromPack.Id) {
                throw new ValidationException("订单头程物流方式记录和订单打包数据不匹配，数据有问题！");
            }
        }

        var orderReceiveAddress = session.Query<LogisticsOrderReceiveAddressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();
        if (orderReceiveAddress == null) {
            throw new ValidationException($"订单{order.Code}收货地址不存在，数据有问题！");
        }

        var feeDataList = new List<LogisticsFeeData>();
        foreach (var orderFirstMode in orderFirstModeList) {
            int orderFirstModeId = orderFirstMode.Id;
            var orderFirstModeOrderPackList = orderPackList
                .Where(e => e.OrderFirstMode.Id == orderFirstModeId)
                .OrderBy(e => e.GoodsType)
                .ThenBy(e => e.Id)
                .ToList();
            var feeData = new LogisticsFeeData {
                Address = customerReceiveAddressService.GetFullAddress(orderReceiveAddress),
                Zipcode = orderReceiveAddress.Zipcode,
                GoodsValue = order.GoodsValue, // todo 这里还有问题
                GoodsType = orderFirstMode.GoodsType,
            };
        }

        return feeDataList;
    }
}