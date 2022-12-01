using shunda.Core.CodeType;

namespace shunda.Domain.Fee;

// todo 后面调整和精简

public class LogisticsOrderFeeType : CodeTypeBase
{
    /// <summary>
    /// 头程费
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFee = new LogisticsOrderFeeType(1000, "头程初始费", "头程初始费");

    /// <summary>
    /// 头程超长费
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeExtraLength = new LogisticsOrderFeeType(2000, "头程超长费", "头程超长费");

    /// <summary>
    /// 头程单箱超重费
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeSingleExtraWeight = new LogisticsOrderFeeType(3000, "头程单箱超重费", "头程单箱超重费");

    /// <summary>
    /// 头程平均单箱超重费
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeAverageExtraWeight = new LogisticsOrderFeeType(4000, "头程平均单箱超重费", "头程平均单箱超重费");

    /// <summary>
    /// 头程超重费
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeExtraWeight = new LogisticsOrderFeeType(5000, "独立报关费", "独立报关费"); // 头程超重费

    /// <summary>
    /// 头程额外包裹费
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeExtraPackage = new LogisticsOrderFeeType(6000, "头程额外包裹费", "头程额外包裹费");

    /// <summary>
    /// 中国仓额外仓储费
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeExtraStorageDay = new LogisticsOrderFeeType(7000, "中国仓额外仓储费", "中国仓额外仓储费");

    /// <summary>
    /// 头程限制类费用， 这个只有头程才有
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeRestrictType = new LogisticsOrderFeeType(8000, "头程限制类费用", "头程限制类费用");

    /// <summary>
    /// 头程草木竹费用，这个只有头程才有
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeBambooType = new LogisticsOrderFeeType(9000, "头程草木竹费用", "头程草木竹费用");

    /// <summary>
    /// 头程特货费用，这个只有头程才有
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsFeeSpecialType = new LogisticsOrderFeeType(10000, "头程特货费用", "头程特货费用");

    /// <summary>
    /// 头程超级VIP折扣，这个费用是负的
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsSuperVipDiscount = new LogisticsOrderFeeType(11000, "头程超级VIP折扣", "头程超级VIP折扣");

    /// <summary>
    /// 头程VIP折扣，这个费用是负的
    /// </summary>
    public static readonly LogisticsOrderFeeType FirstLogisticsVipDiscount = new LogisticsOrderFeeType(12000, "头程VIP折扣", "头程VIP折扣");

    /// <summary>
    /// 中澳快递费
    /// </summary>
    public static readonly LogisticsOrderFeeType CnAuExpressFee = new LogisticsOrderFeeType(13000, "中澳快递费", "中澳快递费");

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
    /// 订单拆分转入费用
    /// </summary>
    public static readonly LogisticsOrderFeeType OrderSplitIncrease = new LogisticsOrderFeeType(19000, "订单拆分转入费用", "订单拆分转入费用");

    /// <summary>
    /// 订单拆分转出费用
    /// </summary>
    public static readonly LogisticsOrderFeeType OrderSplitDecrease = new LogisticsOrderFeeType(20000, "订单拆分转出费用", "订单拆分转出费用");

    /// <summary>
    /// 订单合并转入费用
    /// </summary>
    public static readonly LogisticsOrderFeeType OrderCombineIncrease = new LogisticsOrderFeeType(21000, "订单合并转入费用", "订单合并转入费用");

    /// <summary>
    /// 订单合并转出费用
    /// </summary>
    public static readonly LogisticsOrderFeeType OrderCombineDecrease = new LogisticsOrderFeeType(22000, "订单合并转出费用", "订单合并转出费用");

    /// <summary>
    /// 问题件费用
    /// </summary>
    public static readonly LogisticsOrderFeeType IssueGoodsFee = new LogisticsOrderFeeType(23000, "问题件费用", "问题件费用");

    /// <summary>
    /// 退款
    /// 这是一个正的费用
    /// </summary>
    public static readonly LogisticsOrderFeeType RefundFee = new LogisticsOrderFeeType(24000, "退款", "退款");

    /// <summary>
    /// 打木架费用
    /// </summary>
    public static readonly LogisticsOrderFeeType WoodFrameFee = new LogisticsOrderFeeType(25000, "打木架费用", "打木架费用");

    /// <summary>
    /// 清理树皮杂物费用
    /// </summary>
    public static readonly LogisticsOrderFeeType BarkCleanFee = new LogisticsOrderFeeType(26000, "清理树皮杂物费用", "清理树皮杂物费用");

    /// <summary>
    /// 超大超重附加费
    /// </summary>
    public static readonly LogisticsOrderFeeType ExtraSizeWeightFee = new LogisticsOrderFeeType(27000, "超大超重附加费", "超大超重附加费");

    /// <summary>
    /// 手工修正费用
    /// </summary>
    public static readonly LogisticsOrderFeeType ManualAdjustFee = new LogisticsOrderFeeType(28000, "手工修正", "手工修正");

    /// <summary>
    /// 其它费用
    /// </summary>
    public static readonly LogisticsOrderFeeType OtherFee = new LogisticsOrderFeeType(880_000, "其它费用", "其它费用");

    /// <summary>
    /// None
    /// </summary>
    public static readonly LogisticsOrderFeeType None = new LogisticsOrderFeeType(0, "None", "None");

    private static readonly List<LogisticsOrderFeeType> logisticsFeeTypeList = new List<LogisticsOrderFeeType>() {
        FirstLogisticsFee,
        FirstLogisticsFeeExtraLength,
        FirstLogisticsFeeSingleExtraWeight,
        FirstLogisticsFeeAverageExtraWeight,
        FirstLogisticsFeeExtraWeight,
        FirstLogisticsFeeRestrictType,
        FirstLogisticsFeeBambooType,
        FirstLogisticsFeeSpecialType,
        FirstLogisticsFeeExtraPackage,
        FirstLogisticsFeeExtraStorageDay,
        CnAuExpressFee,

        FirstLogisticsSuperVipDiscount,
        FirstLogisticsVipDiscount,
    };

    private static readonly List<LogisticsOrderFeeType> otherFeetypeList = new List<LogisticsOrderFeeType>() {
        WoodFrameFee,
        BarkCleanFee,
        ExtraSizeWeightFee,
        GstFee,
        InsuranceFee,
        CouponDiscountFee,
        ManualIncrease,
        ManualDecrease,
        OrderSplitIncrease,
        OrderSplitDecrease,
        OrderCombineIncrease,
        OrderCombineDecrease,
        IssueGoodsFee,
        RefundFee,
        ManualAdjustFee,
        OtherFee,
    };

    private static readonly List<LogisticsOrderFeeType> feeTypeList = logisticsFeeTypeList.Concat(otherFeetypeList).ToList();

    private static readonly Dictionary<int, LogisticsOrderFeeType> feeTypeMap = feeTypeList.ToDictionary(e => e.Id);

    public static LogisticsOrderFeeType GetById(int typeId)
    {
        if (feeTypeMap.TryGetValue(typeId, out LogisticsOrderFeeType? feeType))
        {
            return feeType;
        }

        return None;
    }

    public static IList<LogisticsOrderFeeType> GetList()
    {
        return feeTypeList.ToList();
    }

    public static IList<LogisticsOrderFeeType> GetLogisticsFeeTypeList()
    {
        return logisticsFeeTypeList.ToList();
    }

    public static IList<LogisticsOrderFeeType> GetOtherFeeTypeList()
    {
        return otherFeetypeList.ToList();
    }

    private LogisticsOrderFeeType(int id, string name, string description) : base(id, name, description)
    {
    }
}

public class LogisticsOrderFeeTypeHibernateType : CodeHibernateTypeBase<LogisticsOrderFeeType>
{
    protected override LogisticsOrderFeeType GetCodeTypeById(int id)
    {
        return LogisticsOrderFeeType.GetById(id);
    }
}

public class LogisticsOrderFeeTypeModelBinder : CodeTypeModelBinderBase<LogisticsOrderFeeType>
{
    protected override LogisticsOrderFeeType GetCodeTypeById(int id)
    {
        return LogisticsOrderFeeType.GetById(id);
    }
}