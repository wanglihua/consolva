using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.ReceivePack.Models.ExpressRemarkPresetSelect;

public class ExpressRemarkPresetSelectSaveModel
{
    /// <summary>
    /// Id
    /// </summary>
    public int ExpressRemarkPresetSelectId { get; set; }

    /// <summary>
    /// 备注类型，非空
    /// </summary>
    [Required(ErrorMessage = "备注类型不能为空！")]
    [ModelBinder(typeof(ExpressRemarkTypeModelBinder))]
    public ExpressRemarkType Type { get; set; } = ExpressRemarkType.None;

    /// <summary>
    /// 备注，非空
    /// </summary>
    [Required(ErrorMessage = "备注不能为空！")]
    [MaxLength(300, ErrorMessage = "备注长度不能大于300！")]
    public string Remark { get; set; } = string.Empty;

    /// <summary>
    /// 排序，非空
    /// </summary>
    [Required(ErrorMessage = "排序不能为空！")]
    public int OrderNum { get; set; } = 0;
}