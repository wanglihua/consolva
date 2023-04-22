using FluentValidation;
using shunda.Domain;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsOrder;

namespace shunda.Service.Fee;

public class LogisticsFeeData {
    public int TotalExpressPackageCount { get; set; }

    public IList<LogisticsFeeDataItem> FeeDataItemList { get; set; } = new List<LogisticsFeeDataItem>();
}

/// <summary>
/// 运费计算时需要的数据项
/// </summary>
public class LogisticsFeeDataItem {
    /// <summary>
    /// 目的国家
    /// </summary>
    public Country Country { get; set; } = Country.None;

    /// <summary>
    /// 订单头程物流方式记录
    /// 订单计费时才需要这个，其他时候不需要
    /// </summary>
    public LogisticsOrderFirstModeInfo OrderFirstMode { get; set; } = new();

    /// <summary>
    /// 物流方式
    /// </summary>
    public LogisticsModeInfo LogisticsMode { get; set; } = new();

    /// <summary>
    /// 收货地址
    /// </summary>
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// 收货邮编
    /// </summary>
    public string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// 物品货值
    /// 注意，这个是整个订单的货值
    /// </summary>
    public decimal OrderGoodsValue { get; set; }

    /// <summary>
    /// 物品类型
    /// </summary>
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 包裹列表，这里一般是打包后的包裹列表
    /// </summary>
    public IList<LogisticsFeePack> FeePackList { get; set; } = new List<LogisticsFeePack>();
}

/// <summary>
/// 运费计算中的包裹信息
/// </summary>
public class LogisticsFeePack {
    /// <summary>
    /// 用于按打包顺序排序，可以按箱号计费时算得箱号顺序号
    /// </summary>
    public int PackId { get; set; }
    
    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    /// <summary>
    /// 单位，kg
    /// </summary>
    public decimal Weight { get; set; }

    /// <summary>
    /// 数量，默认为 1
    /// </summary>
    public int PackageCount { get; set; } = 1;
}

public class LogisticsFeePackValidator : AbstractValidator<LogisticsFeePack> {
    public LogisticsFeePackValidator() {
        RuleFor(e => e.Length).Must(p => p > 0).WithMessage("计费包裹长度不能为空！");
        RuleFor(e => e.Width).Must(p => p > 0).WithMessage("计费包裹宽度不能为空！");
        RuleFor(e => e.Height).Must(p => p > 0).WithMessage("计费包裹高度不能为空！");
        RuleFor(e => e.Weight).Must(p => p > 0).WithMessage("计费包裹重量不能为空！");
        RuleFor(e => e.PackageCount).Must(p => p > 0).WithMessage("计费包裹数量不能为空！");
    }
}

/// <summary>
/// 一般验证的时候就用这个验证器
/// 订单计费时还需要验证字段 LogisticsOrderFirstModeInfo OrderFirstMode
/// </summary>
public class LogisticsFeeDataValidator : AbstractValidator<LogisticsFeeDataItem> {
    public LogisticsFeeDataValidator() {
        // 已经 ValidationUtil 中全局设置
        // ClassLevelCascadeMode = CascadeMode.Stop;
        // RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Country).NotNull().NotEqual(Country.None).WithMessage("计费目的国家不能为空，或值不合法！");
        RuleFor(e => e.LogisticsMode).NotNull().Must(p => p.Id > 0).WithMessage("计费物流方式不能为空！");
        RuleFor(e => e.Address).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("计费收货地址不能为空！");
        RuleFor(e => e.Zipcode).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("计费收货邮编不能为空！");
        RuleFor(e => e.OrderGoodsValue).Must(p => p > 0).WithMessage("计费物品货值不能为空！");
        RuleFor(e => e.GoodsType).NotNull().NotEqual(LogisticsGoodsType.None).WithMessage("计费物品类型不能为空，或值不合法！");
        RuleFor(e => e.FeePackList).NotNull().NotEmpty().WithMessage("计费包裹列表不能为空！");
        RuleForEach(e => e.FeePackList).SetValidator(new LogisticsFeePackValidator());
    }
}

/// <summary>
/// 订单计费验证的时候，用这个验证器
/// </summary>
public class LogisticsFeeDataOrderFeeValidator : LogisticsFeeDataValidator {
    public LogisticsFeeDataOrderFeeValidator() {
        RuleFor(e => e.OrderFirstMode).NotNull().Must(p => p.Id > 0).WithMessage("订单计费订单头程物流方式记录不能为空！");
    }
}