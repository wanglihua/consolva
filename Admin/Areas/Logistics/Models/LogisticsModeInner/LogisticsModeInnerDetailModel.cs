using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsModeInner;

public class LogisticsModeInnerDetailModel
{
    /// <summary>
    ///     Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    ///     类型
    /// </summary>
    [Required(ErrorMessage = "物流类型不能为空！")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public LogisticsModeType Type { get; set; } = LogisticsModeType.None;

    /// <summary>
    ///     编码
    /// </summary>
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(100, ErrorMessage = "编码长度不能大于100！")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    ///     名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(200, ErrorMessage = "名称长度不能大于200！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    ///     描述
    /// </summary>
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(500, ErrorMessage = "描述长度不能大于500！")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    ///     状态
    /// </summary>
    [ModelBinder(typeof(LogisticsModeStatusModelBinder))]
    public LogisticsModeStatus Status { get; set; } = LogisticsModeStatus.None;

    // todo 这里要修改

    /// <summary>
    ///     普通类通用费率
    /// </summary>
    public int? CommonFeeRateId { get; set; }

    /// <summary>
    ///     特货类通用费率
    /// </summary>
    public int? SpecialFeeRateId { get; set; }


    [BindNever]
    public IList<LogisticsFeeRateInfo> LogisticsFeeRateList { get; set; } = new List<LogisticsFeeRateInfo>();

    [BindNever]
    public IList<LogisticsModeStatus> StatusList { get; set; } = new List<LogisticsModeStatus>();
}