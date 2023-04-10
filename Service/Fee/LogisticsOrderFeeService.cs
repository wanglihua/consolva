using NHibernate.Linq;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Coupon;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Coupon;
using shunda.Service.Customer;
using shunda.Service.Logistics;
using ISession = NHibernate.ISession;

namespace shunda.Service.Fee;

[Service(ServiceType.Scoped)]
public class LogisticsOrderFeeService {
    private readonly ISession session;
    private readonly LogisticsFeeService logisticsFeeService;
    private readonly CustomerVipDiscountService customerVipDiscountService;
    private readonly LogisticsOrderPaySumService logisticsOrderPaySumService;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;
    private readonly CouponService couponService;
    private readonly LogisticsFeeInsuranceService logisticsFeeInsuranceService;
    private readonly LogisticsFeeGstService logisticsFeeGstService;
    private readonly LogisticsModeSettingService logisticsModeSettingService;

    public LogisticsOrderFeeService(ISession session,
        LogisticsFeeService logisticsFeeService,
        CustomerVipDiscountService customerVipDiscountService,
        LogisticsOrderPaySumService logisticsOrderPaySumService,
        CustomerReceiveAddressService customerReceiveAddressService,
        CouponService couponService,
        LogisticsFeeInsuranceService logisticsFeeInsuranceService,
        LogisticsFeeGstService logisticsFeeGstService,
        LogisticsModeSettingService logisticsModeSettingService
    ) {
        this.session = session;
        this.logisticsFeeService = logisticsFeeService;
        this.customerVipDiscountService = customerVipDiscountService;
        this.logisticsOrderPaySumService = logisticsOrderPaySumService;
        this.customerReceiveAddressService = customerReceiveAddressService;
        this.couponService = couponService;
        this.logisticsFeeInsuranceService = logisticsFeeInsuranceService;
        this.logisticsFeeGstService = logisticsFeeGstService;
        this.logisticsModeSettingService = logisticsModeSettingService;
    }

    /// <summary>
    /// 计算订单物流费用，并返回计算结果
    /// </summary>
    public LogisticsFeeResult Calculate(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var feeData = OrderToFeeDataList(orderId);

        #region 计费数据验证

        if (feeData.TotalExpressPackageCount <= 0) {
            throw new ValidationException($"订单{order.Code}快递单包裹数量不能为空！");
        }

        if (feeData.FeeDataItemList.Count == 0) {
            throw new ValidationException("订单计费数据项列表不能为空！");
        }

        var feeDataItemList = feeData.FeeDataItemList;

        foreach (var feeDataItem in feeDataItemList) {
            ValidationUtil.Validate<LogisticsFeeDataOrderFeeValidator, LogisticsFeeDataItem>(feeDataItem);
        }

        #endregion

        #region 头程费用

        List<LogisticsFeeResultEntry> orderResultEntryList = new List<LogisticsFeeResultEntry>();
        decimal firstModeFeeNoPromotion = 0m;
        foreach (var feeDataItem in feeDataItemList) {
            var modeResultEntryList = logisticsFeeService.GetLogisticsFee(feeDataItem, FeeMessageType.User);
            orderResultEntryList.AddRange(modeResultEntryList);

            if (feeDataItem.LogisticsMode.Promotion == false) {
                foreach (var modeResultEntry in modeResultEntryList) {
                    if (modeResultEntry.EntryType == LogisticsFeeResultEntryType.Single) {
                        firstModeFeeNoPromotion += modeResultEntry.Fee;
                    }
                }
            }
        }

        decimal firstModeFee = 0m;
        foreach (var entry in orderResultEntryList) {
            if (entry.EntryType == LogisticsFeeResultEntryType.Single) {
                firstModeFee += entry.Fee;
            }
        }

        #endregion

        #region 额外包裹费

        // 按入库快递单包裹总数量计算
        var logisticsModeSetting = logisticsModeSettingService.GetSetting();
        decimal extraPackageCountFee = logisticsFeeService.GetExtraPackageCountFee(feeData.TotalExpressPackageCount, logisticsModeSetting, FeeMessageType.User);

        orderResultEntryList.Add(new LogisticsFeeResultEntry {
            OrderFirstMode = null,
            LogisticsMode = null,
            Title = LogisticsOrderFeeType.ExtraPackageFee.Name,
            FeeType = LogisticsOrderFeeType.ExtraPackageFee,
            Fee = extraPackageCountFee,
            GoodsType = LogisticsGoodsType.None,
            EntryType = LogisticsFeeResultEntryType.Single
        });

        #endregion

        #region vip 和 超级 vip折扣

        // 按物流费用计算，促销的不参与

        // 这里是超级VIP和VIP都计算了的，实际只计算VIP也可以的，现在的VIP就是VIP和超级VIP中折扣大的那个

        int customerId = order.Customer.Id;

        // 超级VIP折扣
        CustomerSuperVipDiscountInfo? superVipDiscount = session.Query<CustomerSuperVipDiscountInfo>()
            .Where(e => e.Customer.Id == customerId)
            .Take(1)
            .SingleOrDefault();

        // VIP折扣
        CustomerVipDiscountInfo? vipDiscount = customerVipDiscountService.GetDiscount(customerId);

        LogisticsOrderFeeType discountFeeType = LogisticsOrderFeeType.None;
        string discountFeeTitle = string.Empty;
        decimal discount = 0m;

        if (superVipDiscount != null) {
            decimal discountVip = -firstModeFeeNoPromotion * superVipDiscount.Discount / 100m; // 负的费用

            // 意思就是 <0 有折扣
            if (discountVip < discount) {
                discount = discountVip;
                discountFeeType = LogisticsOrderFeeType.SuperVipDiscount;
                discountFeeTitle = LogisticsOrderFeeType.SuperVipDiscount.Name;
            }
        }

        if (vipDiscount != null) {
            decimal discountSuperVip = -firstModeFeeNoPromotion * vipDiscount.Discount / 100m; // 负的费用

            // 意思就是有折扣，或者折扣更多
            if (discountSuperVip < discount) {
                discount = discountSuperVip;
                discountFeeType = LogisticsOrderFeeType.VipDiscount;
                discountFeeTitle = LogisticsOrderFeeType.VipDiscount.Name;
            }
        }

        if (discountFeeType != LogisticsOrderFeeType.None) {
            orderResultEntryList.Add(new LogisticsFeeResultEntry() {
                OrderFirstMode = null,
                LogisticsMode = null,
                Title = discountFeeTitle,
                Fee = discount,
                GoodsType = LogisticsGoodsType.None,
                FeeType = discountFeeType,
                EntryType = LogisticsFeeResultEntryType.Single
            });
        }

        #endregion

        #region 计算优惠券实

        // 这里计算的优惠券费用是无效的，因为现在创建时没有选择优惠券，支付时计算的才是有效的
        // 可能费用重新计算的时候，会有用

        // 按物流费用计算，促销的不参与

        var coupon = session.Query<CouponInfo>()
            .Where(e => e.Order!.Id == orderId)
            .Take(1) // 一个订单只能使用一张优惠券，如果可以使用多张，需要在这里修改
            .SingleOrDefault();
        if (coupon != null) {
            var couponId = coupon.Id;
            var couponDiscountResult = couponService.CalculateCouponDiscount(couponId, firstModeFeeNoPromotion);
            if (couponDiscountResult.Success == true && couponDiscountResult.Discount > 0) {
                decimal couponDiscount = couponDiscountResult.Discount;

                orderResultEntryList.Add(new LogisticsFeeResultEntry() {
                    OrderFirstMode = null,
                    LogisticsMode = null,
                    Title = $"{LogisticsOrderFeeType.CouponDiscountFee.Name},{coupon.Code}",
                    Fee = -couponDiscount, // 负的费用
                    GoodsType = LogisticsGoodsType.None,
                    FeeType = LogisticsOrderFeeType.CouponDiscountFee,
                    EntryType = LogisticsFeeResultEntryType.Single
                });

                couponService.UpdateActualDiscountAmount(couponId, couponDiscount);
            } else {
                couponService.SetCouponNotUsedAndSumSaveBatchTotalUsed(couponId);
            }
        }

        #endregion

        // 相当于把当前头程的数据复制一份
        IList<LogisticsFeeResultEntry> firstModeFeeEntryList = new List<LogisticsFeeResultEntry>(orderResultEntryList);

        #region 计算保险费

        decimal insuranceFee = logisticsFeeInsuranceService.GetInsuranceFee(order.GoodsValue, firstModeFee, order.Insurance.Id);

        orderResultEntryList.Add(new LogisticsFeeResultEntry() {
            OrderFirstMode = null,
            LogisticsMode = null,
            Title = LogisticsOrderFeeType.InsuranceFee.Name,
            Fee = insuranceFee,
            GoodsType = LogisticsGoodsType.None,
            FeeType = LogisticsOrderFeeType.InsuranceFee,
            EntryType = LogisticsFeeResultEntryType.Single
        });

        #endregion

        #region 计算GST

        decimal gstFee = logisticsFeeGstService.GetGstFee(order.GoodsValue, firstModeFee, order.Gst.Id);

        orderResultEntryList.Add(new LogisticsFeeResultEntry() {
            OrderFirstMode = null,
            LogisticsMode = null,
            Title = LogisticsOrderFeeType.GstFee.Name,
            Fee = gstFee,
            GoodsType = LogisticsGoodsType.None,
            FeeType = LogisticsOrderFeeType.GstFee,
            EntryType = LogisticsFeeResultEntryType.Single
        });

        #endregion

        return new LogisticsFeeResult {
            FirstModeFeeEntryList = firstModeFeeEntryList,
            FirstModeFee = firstModeFee,
            InsuranceFee = insuranceFee,
            GstFee = gstFee,
            EntryList = orderResultEntryList,
        };
    }

    /// <summary>
    /// 订单数据转计费数据
    /// </summary>
    /// <returns>计费数据列表，订单一个物流方式一个计费数据</returns>
    private LogisticsFeeData OrderToFeeDataList(int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        var expressList = session.Query<ExpressInfo>().Where(e => e.Order!.Id == orderId).ToList();
        if (expressList.Count == 0) {
            throw new ValidationException($"订单{order.Code}快递单列表不能为空！");
        }

        int totalExpressPackageCount = expressList.Sum(e => e.PackageCount);

        var orderFirstModeList = session.Query<LogisticsOrderFirstModeInfo>()
            .Fetch(e => e.FirstMode)
            .Where(e => e.Order.Id == orderId)
            .OrderBy(e => e.FirstMode.Id) // 排序跟下面的一致
            .ThenBy(e => e.GoodsType)
            .ThenBy(e => e.Id)
            .ToList();

        var orderPackList = session.Query<LogisticsOrderPackInfo>()
            .Fetch(e => e.OrderFirstMode)
            .Where(e => e.Order.Id == orderId)
            .ToList();

        var orderFirstModeListFromPack = orderPackList
            .Select(e => e.OrderFirstMode)
            .Distinct()
            .OrderBy(e => e.FirstMode.Id) // 排序跟上面的一致
            .ThenBy(e => e.GoodsType.Id)
            .ThenBy(e => e.Id)
            .ToList();

        if (orderFirstModeList.Count != orderFirstModeListFromPack.Count) {
            throw new ValidationException("订单头程物流方式记录和订单打包数据不匹配，数据有问题！");
        }

        for (int index = 0; index < orderFirstModeList.Count; index++) {
            var orderFirstMode = orderFirstModeList[index];
            var orderFirstModeFromPack = orderFirstModeListFromPack[index];
            if (orderFirstMode.Id != orderFirstModeFromPack.Id) {
                throw new ValidationException("订单头程物流方式记录和订单打包数据不一致，数据有问题！");
            }
        }

        var orderReceiveAddress = session.Query<LogisticsOrderReceiveAddressInfo>()
            .Where(e => e.Order.Id == orderId)
            .Take(1)
            .SingleOrDefault();
        if (orderReceiveAddress == null) {
            throw new ValidationException($"订单{order.Code}收货地址不存在，数据有问题！");
        }

        if (orderReceiveAddress.Country != order.Country) {
            throw new ValidationException($"订单{order.Code}收货地址国家和订单目的国家不一致，数据有问题！");
        }

        var feeDataItemList = new List<LogisticsFeeDataItem>();
        foreach (var orderFirstMode in orderFirstModeList) {
            int orderFirstModeId = orderFirstMode.Id;
            var orderFirstModeOrderPackList = orderPackList
                .Where(e => e.OrderFirstMode.Id == orderFirstModeId) // 这里不按物品类型过滤也是可以的，因为一个 orderFirstModeId 肯定只针对一个物品类型了 
                .OrderBy(e => e.Id)
                .ToList();

            feeDataItemList.Add(new LogisticsFeeDataItem {
                Country = order.Country,
                OrderFirstMode = orderFirstMode,
                LogisticsMode = orderFirstMode.FirstMode,
                GoodsType = orderFirstMode.GoodsType,
                Address = customerReceiveAddressService.GetFullAddress(orderReceiveAddress),
                Zipcode = orderReceiveAddress.Zipcode,
                OrderGoodsValue = order.GoodsValue,

                FeePackList = orderFirstModeOrderPackList.Select(e => new LogisticsFeePack {
                    Length = e.Length,
                    Width = e.Width,
                    Height = e.Height,
                    Weight = e.Weight,
                    PackageCount = e.PackageCount,
                }).ToList(),
            });
        }

        return new LogisticsFeeData {
            FeeDataItemList = feeDataItemList,
            TotalExpressPackageCount = totalExpressPackageCount,
        };
    }

    /// <summary>
    /// 清除之前计算保存的订单费用数据
    /// 只清除物流费用和保险GST费用
    ///
    /// 这里有个问题积分抵扣费用是不移除的，计算的时候也是不计算的，积分抵扣只发生在支付阶段
    /// 可能重新计算费用后，这个抵扣会不准确
    /// </summary>
    public void RemovePreviousSavedFee(int orderId) {
        IList<LogisticsOrderFeeType> feeTypeList = LogisticsOrderFeeType.GetLogisticsFeeTypeList();

        // 这三个费用也不能忘了清除，否则会造成重新打包，重新计算费用时重复计算
        feeTypeList.Add(LogisticsOrderFeeType.ExtraPackageFee);
        feeTypeList.Add(LogisticsOrderFeeType.SuperVipDiscount);
        feeTypeList.Add(LogisticsOrderFeeType.VipDiscount);

        feeTypeList.Add(LogisticsOrderFeeType.InsuranceFee);
        feeTypeList.Add(LogisticsOrderFeeType.GstFee);
        feeTypeList.Add(LogisticsOrderFeeType.CouponDiscountFee);

        // 只清除头程的，优惠券折扣，保险，GST 费用，其他费用不清除
        var orderFeeListToRemove = session.Query<LogisticsOrderFeeInfo>()
            .Where(e => e.Order.Id == orderId && feeTypeList.Contains(e.Type))
            .ToList();

        // 清除可能的优惠券上记录的实际折扣数据
        var couponFeeEntry = orderFeeListToRemove
            .Where(e => e.Type == LogisticsOrderFeeType.CouponDiscountFee)
            .Take(1)
            .SingleOrDefault();
        if (couponFeeEntry != null) {
            var coupon = session.Query<CouponInfo>()
                .Where(e => e.Order!.Id == orderId)
                .Take(1)
                .SingleOrDefault();

            if (coupon != null) {
                coupon.ActualDiscountAmount = 0;
                session.Save(coupon);
                session.Flush();
            }
        }

        foreach (var orderFee in orderFeeListToRemove) {
            session.Delete(orderFee);
        }

        session.Flush();
    }

    /// <summary>
    /// 将费用计算结果保存到订单费用表中
    /// </summary>
    /// <param name="logisticsFeeResult">费用计算结果</param>
    /// <param name="orderId">物流订单Id</param>
    public void SaveOrderFee(LogisticsFeeResult logisticsFeeResult, int orderId) {
        var order = session.Get<LogisticsOrderInfo>(orderId);
        if (order == null) {
            throw new ValidationException($"指定的订单#{orderId}不存在！");
        }

        // 保存费用明细
        foreach (LogisticsFeeResultEntry resultEntry in logisticsFeeResult.EntryList) {
            if (resultEntry.EntryType == LogisticsFeeResultEntryType.Single) {
                LogisticsOrderFeeInfo orderFee = new LogisticsOrderFeeInfo {
                    Customer = order.Customer,
                    Order = order,
                    OrderCode = order.Code,
                    OrderFirstMode = resultEntry.OrderFirstMode,
                    GoodsType = resultEntry.GoodsType,
                    Type = resultEntry.FeeType,
                    Fee = resultEntry.Fee,
                    Description = resultEntry.Title
                };

                session.Save(orderFee);
            }
        }

        session.Flush();

        // 汇总订单费用，并更新订单支付状态
        logisticsOrderPaySumService.SumFeeAndUpdatePayStatus(order.Id);

        // 记录优惠券实际折扣
        var couponFeeEntry = logisticsFeeResult.EntryList
            .Where(e => e.FeeType == LogisticsOrderFeeType.CouponDiscountFee)
            .Take(1)
            .SingleOrDefault();
        if (couponFeeEntry != null) {
            var coupon = session.Query<CouponInfo>()
                .Where(e => e.Order!.Id == orderId)
                .Take(1)
                .SingleOrDefault();

            if (coupon != null) {
                coupon.ActualDiscountAmount = Math.Abs(couponFeeEntry.Fee);
                session.Save(coupon);
                session.Flush();
            }
        }
    }

    /// <summary>
    /// 计算并保存订单费用
    /// </summary>
    public void CalculateAndSaveFee(int orderId) {
        // 先删除
        RemovePreviousSavedFee(orderId);

        // 计算
        LogisticsFeeResult feeResult = Calculate(orderId);

        // 保存
        SaveOrderFee(feeResult, orderId);

        session.Flush();
    }
}