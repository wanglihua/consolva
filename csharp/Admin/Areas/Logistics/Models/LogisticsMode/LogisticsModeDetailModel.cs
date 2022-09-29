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
    [DisplayName("物流类型")]
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
    [DisplayName("编码")]
    [Required(ErrorMessage = "编码不能为空！")]
    [MaxLength(100, ErrorMessage = "编码长度不能大于100！")]
    public string Code { get; set; } = string.Empty;

    /// <summary>
    /// 名称
    /// </summary>
    [DisplayName("名称")]
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(200, ErrorMessage = "名称长度不能大于200！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [DisplayName("描述")]
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(500, ErrorMessage = "描述长度不能大于500！")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// 普通类通用费率
    /// </summary>
    [DisplayName("普通类通用费率")]
    public int? CommonFeeRateId { get; set; }

    /// <summary>
    /// 特货类通用费率
    /// </summary>
    [DisplayName("特货类通用费率")]
    public int? SpecialFeeRateId { get; set; }

    /// <summary>
    /// 费用计算描述
    /// </summary>
    public string? FeeCalDesc { get; set; }

    /// <summary>
    /// 最小重量
    /// </summary>
    public decimal? MinWeight { get; set; }

    /// <summary>
    /// 最大重量
    /// </summary>
    public decimal? MaxWeight { get; set; }

    /// <summary>
    /// 收货仓库
    /// </summary>
    public int? WarehouseId { get; set; }

    /// <summary>
    /// 打包标签
    /// </summary>
    [ModelBinder(typeof(LogisticsPackLabelTypeModelBinder))]
    public LogisticsPackLabelType? PackLabelType { get; set; }

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


    #region repack type 关联

    public IList<int> RepackTypeIdList { get; set; }

    [BindNever]
    public IList<LogisticsRepackType> RepackTypeList { get; set; }

    #endregion





    public IList<int> GoodsTypeIdList { get; set; }

    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; }

    [BindNever]
    public IList<LogisticsModeType> LogisticsModeTypeList { get; set; }

    [BindNever]
    public IList<LogisticsModePackSizeType> PackSizeTypeList { get; set; }

    [BindNever]
    public IList<LogisticsFeeRateInfo> LogisticsFeeRateList { get; set; }

    [BindNever]
    public IList<WarehouseInfo> WarehouseList { get; set; }

    [BindNever]
    public IList<LogisticsModeStatus> StatusList { get; set; }

    /// <summary>
    /// 对应的内部计费物流方式列表
    /// </summary>
    public int? InnerModeId { get; set; }

    /// <summary>
    /// 对应的内部计费物流方式列表
    /// </summary>
    [BindNever]
    public IList<LogisticsModeInfo> InnerModeList { get; set; }
}