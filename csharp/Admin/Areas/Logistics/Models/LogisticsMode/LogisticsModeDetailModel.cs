using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;
using shunda.Domain.Storage;

namespace shunda.Admin.Areas.Logistics.Models.LogisticsMode;

public class LogisticsModeDetailModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 类型
    /// </summary>
    [Required(ErrorMessage = "物流类型不能为空！")]
    [ModelBinder(typeof(LogisticsModeTypeModelBinder))]
    public LogisticsModeType Type { get; set; } = LogisticsModeType.None;

    /// <summary>
    /// 包裹大小件类型
    /// </summary>
    [ModelBinder(typeof(LogisticsModePackSizeTypeModelBinder))]
    public LogisticsModePackSizeType PackSizeType { get; set; } = LogisticsModePackSizeType.None;

    /// <summary>
    /// 编码
    /// </summary>
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(100, ErrorMessage = "编码长度不能大于100！")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(200, ErrorMessage = "名称长度不能大于200！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(500, ErrorMessage = "描述长度不能大于500！")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 普通类通用费率
    /// </summary>
    public int? CommonFeeRateId { get; set; }

    /// <summary>
    /// 特货类通用费率
    /// </summary>
    public int? SpecialFeeRateId { get; set; }

    /// <summary>
    /// 费用计算描述
    /// </summary>
    public string? FeeCalDesc { get; set; }

    /// <summary>
    /// 收货仓库
    /// </summary>
    [Required(ErrorMessage = "收货仓库不能为空！")]
    public int WarehouseId { get; set; }

    /// <summary>
    /// 打包标签
    /// </summary>
    [ModelBinder(typeof(LogisticsPackLabelTypeModelBinder))]
    public LogisticsPackLabelType PackLabelType { get; set; } = LogisticsPackLabelType.None;

    [BindNever]
    public IList<LogisticsPackLabelType> PackLabelTypeList { get; } = LogisticsPackLabelType.GetList();

    /// <summary>
    /// 快递公司名称
    /// </summary>
    public string? ExpressCompanyName { get; set; }

    /// <summary>
    /// 快递公司网址
    /// </summary>
    public string? ExpressCompanyUrl { get; set; }

    /// <summary>
    /// 状态
    /// </summary>
    [ModelBinder(typeof(LogisticsModeStatusModelBinder))]
    public LogisticsModeStatus Status { get; set; } = LogisticsModeStatus.None;

    /// <summary>
    /// 内部状态
    /// </summary>
    [ModelBinder(typeof(LogisticsModeStatusModelBinder))]
    public LogisticsModeStatus InnerStatus { get; set; } = LogisticsModeStatus.None;


    // todo 这两者可不可以只保留一个s

    #region repack type 关联

    public IList<int> RepackTypeIdList { get; set; } = new List<int>();

    [BindNever]
    public IList<LogisticsRepackType> RepackTypeList { get; set; } = new List<LogisticsRepackType>();

    #endregion


    // todo 下面这两者可不可以只保留一个


    public IList<int> GoodsTypeIdList { get; set; } = new List<int>();

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = new List<LogisticsGoodsType>();


    [BindNever]
    public IList<LogisticsModeType> LogisticsModeTypeList { get; set; } = new List<LogisticsModeType>();

    [BindNever]
    public IList<LogisticsModePackSizeType> PackSizeTypeList { get; set; } = new List<LogisticsModePackSizeType>();

    [BindNever]
    public IList<LogisticsFeeRateInfo> LogisticsFeeRateList { get; set; } = new List<LogisticsFeeRateInfo>();

    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; } = new List<WarehouseInfo>();

    [BindNever]
    public IList<LogisticsModeStatus> StatusList { get; set; } = new List<LogisticsModeStatus>();

    /// <summary>
    /// 对应的内部计费物流方式列表
    /// </summary>
    public int? InnerModeId { get; set; }

    /// <summary>
    /// 对应的内部计费物流方式列表
    /// </summary>
    [BindNever]
    public IList<LogisticsModeInfo> InnerModeList { get; set; } = new List<LogisticsModeInfo>();
}