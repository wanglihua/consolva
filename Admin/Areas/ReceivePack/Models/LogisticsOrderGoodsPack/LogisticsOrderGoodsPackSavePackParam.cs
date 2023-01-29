using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.ReceivePack.Models.LogisticsOrderGoodsPack;

public class LogisticsOrderGoodsPackSavePackParam {
    public int PackId { get; set; }

    [Required(ErrorMessage = "订单不能为空！")]
    public int OrderId { get; set; }

    [Required(ErrorMessage = "长度不能为空！")]
    public decimal Length { get; set; }

    [Required(ErrorMessage = "宽度不能为空！")]
    public decimal Width { get; set; }

    [Required(ErrorMessage = "高度不能为空！")]
    public decimal Height { get; set; }

    [Required(ErrorMessage = "重量不能为空！")]
    public decimal Weight { get; set; }

    [Required(ErrorMessage = "箱数不能为空！")]
    public int PackageCount { get; set; }

    public IList<string> TrackingNoList { get; set; } = new List<string>();

    [Required(ErrorMessage = "物品类型不能为空！")]
    [ModelBinder(typeof(LogisticsGoodsTypeModelBinder))]
    public LogisticsGoodsType GoodsType { get; set; } = LogisticsGoodsType.None;

    [Required(ErrorMessage = "头程物流方式不能为空！")]
    public int FirstModeId { get; set; }
}

public class LogisticsOrderGoodsPackSavePackParamValidator : AbstractValidator<LogisticsOrderGoodsPackSavePackParam> {
    public LogisticsOrderGoodsPackSavePackParamValidator() {
        RuleFor(e => e.OrderId).Must(p => p > 0).WithMessage("订单不能为空！");
        RuleFor(e => e.Length).Must(p => p > 0).WithMessage("长宽高重量数据必须大于0！");
        RuleFor(e => e.Width).Must(p => p > 0).WithMessage("长宽高重量数据必须大于0！");
        RuleFor(e => e.Height).Must(p => p > 0).WithMessage("长宽高重量数据必须大于0！");
        RuleFor(e => e.Weight).Must(p => p > 0).WithMessage("长宽高重量数据必须大于0！");
        RuleFor(e => e.PackageCount).Must(p => p > 0).WithMessage("箱数必须大于0！");
        RuleFor(e => e.TrackingNoList).Must(p => p != null && p.Count > 0).WithMessage("请扫描或输入快递单号，再保存打包信息！");
        RuleFor(e => e.GoodsType).Must(p => p != LogisticsGoodsType.None).WithMessage("物品类型不能为空！");
        RuleFor(e => e.FirstModeId).Must(p => p > 0).WithMessage("头程物流方式不能为空！");
    }
}