using FluentValidation;
using shunda.Domain.LogisticsOrder;

namespace shunda.Admin.Areas.LogisticsOrder.Models.SeventeenTrackCarrier;

public class SeventeenTrackCarrierSaveModelValidator : AbstractValidator<SeventeenTrackCarrierSaveModel> {
    public SeventeenTrackCarrierSaveModelValidator() { 
        RuleFor(e => e.CarrierKey).Must(p => p > 0).WithMessage("编号不能为空！");

        RuleFor(e => e.CountryCode).Must(p => p > 0).WithMessage("国家编号不能为空！");

        RuleFor(e => e.CountryIso).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("国家代码不能为空！");
        RuleFor(e => e.CountryIso).MaximumLength(10).WithMessage("国家代码长度不能大于10！");

        RuleFor(e => e.Email).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("email不能为空！");
        RuleFor(e => e.Email).MaximumLength(200).WithMessage("email长度不能大于200！");

        RuleFor(e => e.Tel).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("电话不能为空！");
        RuleFor(e => e.Tel).MaximumLength(30).WithMessage("电话长度不能大于30！");

        RuleFor(e => e.Url).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("网址不能为空！");
        RuleFor(e => e.Url).MaximumLength(300).WithMessage("网址长度不能大于300！");

        RuleFor(e => e.CarrierName).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("名称不能为空！");
        RuleFor(e => e.CarrierName).MaximumLength(30).WithMessage("名称长度不能大于30！");

        RuleFor(e => e.NameZhCn).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("中文名称1不能为空！");
        RuleFor(e => e.NameZhCn).MaximumLength(30).WithMessage("中文名称1长度不能大于30！");

        RuleFor(e => e.NameZhHk).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("中文名称2不能为空！");
        RuleFor(e => e.NameZhHk).MaximumLength(30).WithMessage("中文名称2长度不能大于30！");

        RuleFor(e => e.CarrierGroup).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("分组编号不能为空！");
        RuleFor(e => e.CarrierGroup).MaximumLength(10).WithMessage("分组编号长度不能大于10！");

    }
}