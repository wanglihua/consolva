using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Coupon;

namespace shunda.Admin.Areas.Fee.Models.CouponBatch;

public class CouponBatchDetailModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int CouponBatchId { get; set; }

    /// <summary>
    /// 名称，非空
    /// </summary>
    [Required(ErrorMessage = "名称不能为空！")]
    [MaxLength(200, ErrorMessage = "名称长度不能大于200！")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// 类型，非空
    /// </summary>
    [Required(ErrorMessage = "类型不能为空！")]
    [ModelBinder(typeof(CouponTypeModelBinder))]
    public CouponType Type { get; set; } = CouponType.None;

    /// <summary>
    /// 有效期，非空
    /// </summary>
    [Required(ErrorMessage = "有效期不能为空！")]
    public DateTime Expire { get; set; }

    /// <summary>
    /// 总数量，非空
    /// </summary>
    [Required(ErrorMessage = "总数量不能为空！")]
    public int TotalQuantity { get; set; }

    /// <summary>
    /// 规则，可空
    /// </summary>
    [BindNever]
    public string Rule { get; set; } = string.Empty;

    /// <summary>
    /// 规则描述，可空
    /// </summary>
    [BindNever]
    public string RuleDesc { get; set; } = string.Empty;

    /// <summary>
    /// 每客户可领取数量，非空
    /// </summary>
    [Required(ErrorMessage = "每客户可领取数量不能为空！")]
    public int MaxQtyPerCustomer { get; set; }

    /// <summary>
    /// 状态，非空
    /// </summary>
    // [Required(ErrorMessage = "状态不能为空！")]
    // [ModelBinder(typeof(CouponBatchStatusModelBinder))]
    [BindNever]
    public CouponBatchStatus Status { get; set; } = CouponBatchStatus.None;

    /// <summary>
    /// 是否已经投放，如果已经投放，有些条目已经不能修改
    /// </summary>
    [BindNever]
    public bool Released { get; set; }

    [BindNever]
    public IList<CouponType> TypeList { get; set; } = CouponType.GetList();

    [BindNever]
    public IList<CouponBatchStatus> StatusList { get; set; } = CouponBatchStatus.GetList();
}