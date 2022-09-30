using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Storage;

namespace shunda.Domain.Logistics;

/// <summary>
/// 物流方式
/// </summary>
public class LogisticsModeInfo : Entity
{
    /// <summary>
    /// 类别
    /// 客户端费率物流，内部费率物流
    /// </summary>
    public virtual LogisticsModeCate Cate { get; set; } = LogisticsModeCate.Customer;

    /// <summary>
    /// 类型
    /// </summary>
    [Required(ErrorMessage = "物流类型不能为空！")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public virtual LogisticsModeType Type { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 包裹大小件类型
    /// </summary>
    [ModelBinder(typeof(LogisticsModePackSizeTypeModelBinder))]
    public virtual LogisticsModePackSizeType PackSizeType { get; set; } = LogisticsModePackSizeType.None;

    /// <summary>
    /// 编码
    /// </summary>
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(100, ErrorMessage = "编码长度不能大于100！")]
    public virtual string Code { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(200, ErrorMessage = "名称长度不能大于200！")]
    public virtual string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(500, ErrorMessage = "描述长度不能大于500！")]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 普通类通用费率
    /// </summary>
    [BindNever]
    public virtual LogisticsFeeRateInfo CommonFeeRate { get; set; } = new LogisticsFeeRateInfo();

    /// <summary>
    /// 特货类通用费率
    /// </summary>
    [BindNever]
    public virtual LogisticsFeeRateInfo SpecialFeeRate { get; set; } = new LogisticsFeeRateInfo();

    /// <summary>
    /// 费用计算描述
    /// </summary>
    public virtual string? FeeCalDesc { get; set; } = string.Empty;

    /// <summary>
    /// 最小重量
    /// </summary>
    public virtual decimal? MinWeight { get; set; }

    /// <summary>
    /// 最大重量
    /// </summary>
    public virtual decimal? MaxWeight { get; set; }

    /// <summary>
    /// 收货仓库
    /// </summary>
    [BindNever]
    public virtual WarehouseInfo Warehouse { get; set; } = new WarehouseInfo();

    /// <summary>
    /// 打包标签
    /// </summary>
    [ModelBinder(typeof(LogisticsPackLabelTypeModelBinder))]
    public virtual LogisticsPackLabelType PackLabelType { get; set; } = LogisticsPackLabelType.None;

    /// <summary>
    /// 快递公司名称
    /// </summary>
    public virtual string ExpressCompanyName { get; set; } = string.Empty;

    /// <summary>
    /// 快递公司网址
    /// </summary>
    public virtual string ExpressCompanyUrl { get; set; } = string.Empty;

    /// <summary>
    /// 对应的内部费率
    /// 现在是中澳空运和中澳快递在用
    /// </summary>
    public virtual int? InnerModeId { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [ModelBinder(typeof(LogisticsModeStatusModelBinder))]
    public virtual LogisticsModeStatus Status { get; set; } = LogisticsModeStatus.None;

    /// <summary>
    /// 内部状态
    /// </summary>
    [ModelBinder(typeof(LogisticsModeStatusModelBinder))]
    public virtual LogisticsModeStatus InnerStatus { get; set; } = LogisticsModeStatus.None;
}