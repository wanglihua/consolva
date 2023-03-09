using FluentValidation;

namespace shunda.Domain.Client;

/// <summary>
/// 广告横幅 实体验证器
/// </summary>
public class AdvertBannerInfoValidator : AbstractValidator<AdvertBannerInfo> {
    public AdvertBannerInfoValidator() {
        RuleFor(e => e.Loc).NotNull().Must(p => p != AdvertBannerLoc.None).WithMessage("位置不能为空！");

        RuleFor(e => e.Name).NotNull().WithMessage("名称不能为空！");
        RuleFor(e => e.Name).MaximumLength(200).WithMessage("名称长度不能大于200！");
    }
}