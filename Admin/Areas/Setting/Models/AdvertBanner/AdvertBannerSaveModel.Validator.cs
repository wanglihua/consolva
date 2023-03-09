using FluentValidation;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.AdvertBanner;

public class AdvertBannerSaveModelValidator : AbstractValidator<AdvertBannerSaveModel> {
    public AdvertBannerSaveModelValidator() {
        RuleFor(e => e.Loc).NotNull().Must(p => p != AdvertBannerLoc.None).WithMessage("位置不能为空！");

        RuleFor(e => e.Name).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("名称不能为空！");
        RuleFor(e => e.Name).MaximumLength(200).WithMessage("名称长度不能大于200！");

        RuleFor(e => e.ClientShow).NotNull().NotEmpty().WithMessage("是否客户端显示不能为空！");
    }
}