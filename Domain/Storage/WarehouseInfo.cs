using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Core;

namespace shunda.Domain.Storage;

/// <summary>
/// 仓库
/// </summary>
public class WarehouseInfo : Entity {
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
    public virtual string Name { get; set; } = String.Empty;

    /// <summary>
    /// 描述
    /// </summary>
    [Required(ErrorMessage = "描述不能为空！")]
    [MaxLength(500, ErrorMessage = "描述长度不能大于500！")]
    public virtual string Description { get; set; } = string.Empty;

    /// <summary>
    /// 省市县区
    /// </summary>
    [Required(ErrorMessage = "省市县区不能为空！")]
    [MaxLength(500, ErrorMessage = "省市县区长度不能大于300！")]
    public virtual string Area { get; set; } = string.Empty;

    /// <summary>
    /// 地址
    /// </summary>
    [Required(ErrorMessage = "地址不能为空！")]
    [MaxLength(500, ErrorMessage = "地址长度不能大于500！")]
    public virtual string Address { get; set; } = string.Empty;

    /// <summary>
    /// 联系电话
    /// </summary>
    [Required(ErrorMessage = "联系电话不能为空！")]
    [MaxLength(500, ErrorMessage = "联系电话长度不能大于30！")]
    public virtual string Phone { get; set; } = string.Empty;

    /// <summary>
    /// 邮编
    /// </summary>
    [Required(ErrorMessage = "邮编不能为空！")]
    [MaxLength(500, ErrorMessage = "邮编长度不能大于30！")]
    public virtual string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// 外部状态
    /// 针对前台客户能否使用的状态
    /// </summary>
    [Required(ErrorMessage = "外部状态不能为空！")]
    [ModelBinder(typeof(WarehouseStatusModelBinder))]
    public virtual WarehouseStatus OuterStatus { get; set; } = WarehouseStatus.Enabled;

    /// <summary>
    /// 内部状态
    /// 针对后台管理用户能否使用的状态
    /// </summary>
    [Required(ErrorMessage = "内部状态不能为空！")]
    [ModelBinder(typeof(WarehouseStatusModelBinder))]
    public virtual WarehouseStatus InnerStatus { get; set; } = WarehouseStatus.Enabled;

    /// <summary>
    /// 排序字段
    /// </summary>
    [Required(ErrorMessage = "排序字段不能为空！")]
    public virtual int OrderNum { get; set; }
}