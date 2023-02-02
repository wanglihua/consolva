using FluentValidation;
using shunda.Domain;
using shunda.Domain.Logistics;

namespace shunda.Service.Fee;

// todo 加上这个数据结构的数据验证器

/// <summary>
/// 运费计算时需要的数据
/// </summary>
public class LogisticsFeeData {
    /// <summary>
    /// 目的国家
    /// </summary>
    public Country Country { get; set; } = Country.None;

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
    /// </summary>
    public decimal GoodsValue { get; set; }

    /// <summary>
    /// 物品类型
    /// </summary>
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    /// <summary>
    /// 包裹列表
    /// </summary>
    public IList<LogisticsFeePack> FeePackList { get; set; } = new List<LogisticsFeePack>();
}

/// <summary>
/// 运费计算中的物品信息
/// </summary>
public class LogisticsFeePack {
    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public decimal Height { get; set; }

    public decimal Weight { get; set; }

    /// <summary>
    /// 数量，默认为 1
    /// </summary>
    public int PackageCount { get; set; } = 1;
}

public class LogisticsFeeDataValidator : AbstractValidator<LogisticsFeeData> {
    public LogisticsFeeDataValidator() {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(e => e.Country).NotNull().NotEqual(Country.None).WithMessage("计费目的国家不能为空，或值不合法！");
        RuleFor(e => e.LogisticsMode).NotNull().Must(p => p.Id > 0).WithMessage("计费物流方式不能为空！");
        RuleFor(e => e.Address).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("计费收货地址不能为空！");
        RuleFor(e => e.Zipcode).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("计费收货邮编不能为空！");
        RuleFor(e => e.GoodsValue).Must(p => p > 0).WithMessage("计费物品货值不能为空！");
        RuleFor(e => e.GoodsType).NotNull().NotEqual(LogisticsGoodsType.None).WithMessage("计费物品类型不能为空，或值不合法！");
        RuleFor(e => e.FeePackList).NotNull().NotEmpty().WithMessage("计费包裹列表不能为空！");
    }
}