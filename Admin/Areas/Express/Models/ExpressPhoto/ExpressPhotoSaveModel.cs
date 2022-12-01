using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Express;

namespace shunda.Admin.Areas.Express.Models.ExpressPhoto;

public class ExpressPhotoSaveModel
{
    public string? OrderCode { get; set; }

    [Required(ErrorMessage = "快递单号不能为空！")]
    public string TrackingNo { get; set; } = string.Empty;

    [Required(ErrorMessage = "订单图片类型不能为空！")]
    [ModelBinder(typeof(ExpressPhotoTypeModelBinder))]
    public ExpressPhotoType PhotoType { get; set; } = ExpressPhotoType.None;

    // [Required(ErrorMessage = "图片储存路径不能为空！")]
    [Required(ErrorMessage = "图片不能为空！")]
    public string StorePath { get; set; } = string.Empty;

    // [Required(ErrorMessage = "图片存储名称不能为空！")]
    [Required(ErrorMessage = "图片不能为空！")]
    public string StoreName { get; set; } = string.Empty;
}