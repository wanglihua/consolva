using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Core.Util;
using shunda.Domain.Customer;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;
using shunda.Service.Customer;

namespace shunda.Service.Fee;

/// <summary>
/// 运费估算
/// </summary>
[Service(ServiceType.Scoped)]
public class FreightEstimateService {
    private readonly ISession session;
    private readonly LogisticsFeeService logisticsFeeService;
    private readonly LogisticsFeeGstService logisticsFeeGstService;
    private readonly CustomerReceiveAddressService customerReceiveAddressService;

    public FreightEstimateService(ISession session,
        LogisticsFeeService logisticsFeeService,
        LogisticsFeeGstService logisticsFeeGstService,
        CustomerReceiveAddressService customerReceiveAddressService
    ) {
        this.session = session;
        this.logisticsFeeService = logisticsFeeService;
        this.logisticsFeeGstService = logisticsFeeGstService;
        this.customerReceiveAddressService = customerReceiveAddressService;
    }

    /// <summary>
    /// 估算费用
    /// </summary>
    /// <param name="customerId">预估客户</param>
    /// <returns>估算的各项费用结果列表</returns>
    public LogisticsFeeResult Calculate(int customerId) {
        IList<LogisticsFeeResultEntry> firstModeFeeEntryList = new List<LogisticsFeeResultEntry>();

        LogisticsFeeData logisticsFeeData = GetLogisticsFeeData(customerId);
        ValidationUtil.Validate<LogisticsFeeDataValidator, LogisticsFeeData>(logisticsFeeData);

        #region 头程

        IList<LogisticsFeeResultEntry> firstLogisticsFeeResultEntryList = logisticsFeeService.GetLogisticsFee(logisticsFeeData, LogisticsFeeRateType.Customer);
        foreach (var logisticsFeeResultEntry in firstLogisticsFeeResultEntryList) {
            firstModeFeeEntryList.Add(logisticsFeeResultEntry);
        }

        #endregion

        #region 计算GST

        decimal logisticsFee = 0m;
        foreach (LogisticsFeeResultEntry logisticsFeeResultEntry in firstModeFeeEntryList) {
            if (logisticsFeeResultEntry.EntryType == LogisticsFeeResultEntryType.Single) {
                logisticsFee += logisticsFeeResultEntry.Fee;
            }
        }

        // 查找一个默认的GST费用设置
        LogisticsGstStatus logisticsGstEnabledStatus = LogisticsGstStatus.Enabled;
        var logisticsGst = session
            .Query<LogisticsGstInfo>()
            .Where(e => e.Status == logisticsGstEnabledStatus)
            .Take(1)
            .SingleOrDefault();

        if (logisticsGst == null) {
            // 这个不用 ValidationException，需要记录到日志
            throw new LogisticsFeeException("未找到已启用的GST收费设置！");
        }

        int logisticsGstId = logisticsGst.Id;
        decimal gstFee = logisticsFeeGstService.GetGstFee(logisticsFeeData.OrderGoodsValue, logisticsFee, logisticsGstId);

        List<LogisticsFeeResultEntry> feeEntryList = new List<LogisticsFeeResultEntry>();
        feeEntryList.AddRange(firstModeFeeEntryList);

        feeEntryList.Add(new LogisticsFeeResultEntry() {
            Title = "GST费用",
            Fee = gstFee,
            GoodsType = LogisticsGoodsType.None,
            EntryType = LogisticsFeeResultEntryType.Gst,
            FeeType = LogisticsOrderFeeType.GstFee
        });

        #endregion

        #region 计算总费用返回结果

        return new LogisticsFeeResult {
            FirstModeFeeEntryList = firstModeFeeEntryList,
            FirstModeFee = logisticsFee,
            InsuranceFee = 0,
            GstFee = gstFee,
            EntryList = feeEntryList,
            TotalFee = logisticsFee + gstFee
        };

        #endregion
    }

    /// <summary>
    /// 获取运费预估数据，同时对预估数据进行验证 
    /// </summary>
    private LogisticsFeeData GetLogisticsFeeData(int customerId) {
        var freightEstimateSelect = session.Get<FreightEstimateSelectInfo>(customerId);
        if (freightEstimateSelect == null) {
            throw new ValidationException($"运费预估数据#{customerId}不存在，请填写选择数据后再预估！");
        }

        var freightEstimateSelectGoodsList = session.Query<FreightEstimateSelectGoodsInfo>()
            .Where(e => e.CustomerId == customerId)
            .ToList();
        if (freightEstimateSelectGoodsList.Count == 0) {
            throw new ValidationException("运费预估物品数据不存在，请填写数据后再预估！");
        }

        int firstModeId = freightEstimateSelect.FirstModeId;
        var firstMode = session.Get<LogisticsModeInfo>(firstModeId);
        if (firstMode == null) {
            throw new ValidationException($"运费预估数据#{customerId}中指定的物流方式#{firstModeId}不存在，或未指定！");
        }

        int addressReceiveId = freightEstimateSelect.AddressReceiveId;
        var addressReceive = session.Get<CustomerReceiveAddressInfo>(addressReceiveId);
        if (addressReceive == null) {
            throw new ValidationException($"指定的收货地址#{addressReceiveId}不存在！");
        }

        if (addressReceive.Customer.Id != customerId) {
            throw new ValidationException($"收货地址#{addressReceiveId}不属于当前会员#{customerId}，预估数据存在问题！");
        }

        return new LogisticsFeeData {
            LogisticsMode = firstMode,
            OrderGoodsValue = freightEstimateSelect.GoodsValue,
            GoodsType = freightEstimateSelect.GoodsType,
            FeePackList = freightEstimateSelectGoodsList
                .Select(e => new LogisticsFeePack {
                        Length = e.Length,
                        Width = e.Width,
                        Height = e.Height,
                        Weight = e.Weight,
                        PackageCount = e.PackageCount
                    }
                ).ToList(),
            Address = customerReceiveAddressService.GetFullAddress(addressReceive),
            Zipcode = addressReceive.Zipcode,
            // PackageCount = freightEstimateSelectGoodsList.Sum(e => e.PackageCount),
        };
    }
}