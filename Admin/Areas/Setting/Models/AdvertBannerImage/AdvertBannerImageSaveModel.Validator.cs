using FluentValidation;

namespace shunda.Admin.Areas.Setting.Models.AdvertBannerImage;

public class AdvertBannerImageSaveModelValidator : AbstractValidator<AdvertBannerImageSaveModel> {
    public AdvertBannerImageSaveModelValidator() {
        RuleFor(e => e.BannerId).Must(p => p > 0).WithMessage("广告横幅不能为空！");

        RuleFor(e => e.StorePath).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("存储路径不能为空！");
        RuleFor(e => e.StorePath).MaximumLength(255).WithMessage("存储路径长度不能大于255！");

        RuleFor(e => e.StoreName).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("存储名称不能为空！");
        RuleFor(e => e.StoreName).MaximumLength(50).WithMessage("存储名称长度不能大于50！");

        RuleFor(e => e.LinkUrl).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("链接地址不能为空！");
        RuleFor(e => e.LinkUrl).MaximumLength(300).WithMessage("链接地址长度不能大于300！");
    }
}