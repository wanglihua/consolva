using FluentValidation;

namespace shunda.Domain.Client;

/// <summary>
/// 广告横幅图片 实体验证器
/// </summary>
public class AdvertBannerImageInfoValidator : AbstractValidator<AdvertBannerImageInfo> {
    public AdvertBannerImageInfoValidator() {
        RuleFor(e => e.Banner).Must(p => p != null).WithMessage("广告横幅不能为空！");

        RuleFor(e => e.StorePath).NotNull().WithMessage("存储路径不能为空！");
        RuleFor(e => e.StorePath).MaximumLength(255).WithMessage("存储路径长度不能大于255！");

        RuleFor(e => e.StoreName).NotNull().WithMessage("存储名称不能为空！");
        RuleFor(e => e.StoreName).MaximumLength(50).WithMessage("存储名称长度不能大于50！");

        RuleFor(e => e.LinkUrl).NotNull().WithMessage("链接地址不能为空！");
        RuleFor(e => e.LinkUrl).MaximumLength(300).WithMessage("链接地址长度不能大于300！");
    }
}