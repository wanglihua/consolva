using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

public class LogisticsOrderFeeType : CodeTypeBase {
    /// <summary>
    /// 初始费
    /// </summary>
    public static readonly LogisticsOrderFeeType InitFee = new LogisticsOrderFeeType(1000, "基础费用", "基础费用");

    /// <summary>
    /// 超长费
    /// </summary>
    public static readonly LogisticsOrderFeeType ExtraLengthFee = new LogisticsOrderFeeType(2000, "超长费", "超长费");

    /// <summary>
    /// 单箱超重费
    /// </summary>
    public static readonly LogisticsOrderFeeType SingleExtraWeightFee = new LogisticsOrderFeeType(3000, "单箱超重费", "单箱超重费");

    /// <summary>
    /// 平均单箱超重费
    /// 已基本不用
    /// </summary>
    public static readonly LogisticsOrderFeeType AverageExtraWeightFee = new LogisticsOrderFeeType(4000, "平均单箱超重费", "平均单箱超重费");

    /// <summary>
    /// 超重费
    /// 已基本不用
    /// </summary>
    public static readonly LogisticsOrderFeeType ExtraWeightFee = new LogisticsOrderFeeType(5000, "独立报关费", "独立报关费"); // 超重费

    /// <summary>
    /// 合箱打包费
    /// </summary>
    public static readonly LogisticsOrderFeeType ExtraPackageFee = new LogisticsOrderFeeType(6000, "合箱打包费", "合箱打包费");

    /// <summary>
    /// 中国仓额外仓储费
    /// </summary>
    public static readonly LogisticsOrderFeeType ExtraStorageDayFee = new LogisticsOrderFeeType(7000, "中国仓额外仓储费", "中国仓额外仓储费");

    /// <summary>
    /// 超级VIP折扣，这个费用是负的
    /// 客户端显示时，也显示VIP折扣
    /// </summary>
    public static readonly LogisticsOrderFeeType SuperVipDiscount = new LogisticsOrderFeeType(11000, "VIP折扣", "VIP折扣");

    /// <summary>
    /// VIP折扣，这个费用是负的
    /// </summary>
    public static readonly LogisticsOrderFeeType VipDiscount = new LogisticsOrderFeeType(12000, "VIP折扣", "VIP折扣");

    /// <summary>
    /// 中澳快递费
    /// </summary>
    public static readonly LogisticsOrderFeeType ExpressFee = new LogisticsOrderFeeType(13000, "跨国快递费", "跨国快递费");

    /// <summary>
    /// GST费用
    /// </summary>
    public static readonly LogisticsOrderFeeType GstFee = new LogisticsOrderFeeType(14000, "GST费用", "GST费用");

    /// <summary>
    /// 优惠券实扣s
    /// </summary>
    public static readonly LogisticsOrderFeeType CouponDiscountFee = new LogisticsOrderFeeType(15000, "优惠券实扣", "优惠券实扣");

    /// <summary>
    /// 保险费用
    /// </summary>
    public static readonly LogisticsOrderFeeType InsuranceFee = new LogisticsOrderFeeType(16000, "保险费用", "保险费用");

    /// <summary>
    /// 手动增加费用
    /// </summary>
    public static readonly LogisticsOrderFeeType ManualIncrease = new LogisticsOrderFeeType(17000, "手动增加", "手动增加费用");

    /// <summary>
    /// 手动减少费用
    /// </summary>
    public static readonly LogisticsOrderFeeType ManualDecrease = new LogisticsOrderFeeType(18000, "手动减少", "手动减少费用");

    /// <summary>
    /// 退款
    /// 这是一个正的费用
    /// </summary>
    public static readonly LogisticsOrderFeeType RefundFee = new LogisticsOrderFeeType(24000, "退款", "退款");

    /// <summary>
    /// 手工修正费用
    /// </summary>
    public static readonly LogisticsOrderFeeType ManualAdjustFee = new LogisticsOrderFeeType(28000, "手工修正", "手工修正");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderFeeType None = new LogisticsOrderFeeType(0, "None", "None");

    private static readonly List<LogisticsOrderFeeType> logisticsFeeTypeList = new List<LogisticsOrderFeeType>() {
        InitFee,
        ExtraLengthFee,
        SingleExtraWeightFee,
        AverageExtraWeightFee,
        ExtraWeightFee,
        ExtraStorageDayFee,
        ExpressFee,
    };

    private static readonly List<LogisticsOrderFeeType> otherFeetypeList = new List<LogisticsOrderFeeType>() {
        ExtraPackageFee,
        SuperVipDiscount,
        VipDiscount,
        GstFee,
        InsuranceFee,
        CouponDiscountFee,
        ManualIncrease,
        ManualDecrease,
        RefundFee,
        ManualAdjustFee,
    };

    private static readonly List<LogisticsOrderFeeType> feeTypeList = logisticsFeeTypeList.Concat(otherFeetypeList).ToList();

    private static readonly Dictionary<int, LogisticsOrderFeeType> feeTypeMap = feeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderFeeType GetById(int typeId) {
        if (feeTypeMap.TryGetValue(typeId, out LogisticsOrderFeeType? feeType)) {
            return feeType;
        }

        return None;
    }

    public static IList<LogisticsOrderFeeType> GetList() {
        return feeTypeList.ToList();
    }

    public static IList<LogisticsOrderFeeType> GetLogisticsFeeTypeList() {
        return logisticsFeeTypeList.ToList();
    }

    public static IList<LogisticsOrderFeeType> GetOtherFeeTypeList() {
        return otherFeetypeList.ToList();
    }

    private LogisticsOrderFeeType(int id, string name, string description) : base(id, name, description) {
    }
}

public class LogisticsOrderFeeTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderFeeType> {
    protected override LogisticsOrderFeeType GetCodeTypeById(int id) {
        return LogisticsOrderFeeType.GetById(id);
    }
}

public class LogisticsOrderFeeTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderFeeType> {
    protected override LogisticsOrderFeeType GetCodeTypeById(int id) {
        return LogisticsOrderFeeType.GetById(id);
    }
}