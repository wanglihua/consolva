using FluentValidation;

namespace shunda.Admin.Areas.Logistics.Models.HsGoods;

public class HsGoodsSaveModelValidator : AbstractValidator<HsGoodsSaveModel> {
    public HsGoodsSaveModelValidator() {
        RuleFor(e => e.CnHsCode).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("中国海关编码不能为空！");
        RuleFor(e => e.CnHsCode).MaximumLength(100).WithMessage("中国海关编码长度不能大于100！");

        RuleFor(e => e.CnHsName).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("中国海关申报名不能为空！");
        RuleFor(e => e.CnHsName).MaximumLength(200).WithMessage("中国海关申报名长度不能大于200！");

        RuleFor(e => e.EnHsCode).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("英文海关编码不能为空！");
        RuleFor(e => e.EnHsCode).MaximumLength(100).WithMessage("英文海关编码长度不能大于100！");

        RuleFor(e => e.EnHsName).Must(p => string.IsNullOrWhiteSpace(p) == false).WithMessage("英文海关申报名不能为空！");
        RuleFor(e => e.EnHsName).MaximumLength(200).WithMessage("英文海关申报名长度不能大于200！");

        RuleFor(e => e.Elem01).MaximumLength(300).WithMessage("要素1长度不能大于300！");

        RuleFor(e => e.Elem02).MaximumLength(300).WithMessage("要素2长度不能大于300！");

        RuleFor(e => e.Elem03).MaximumLength(300).WithMessage("要素3长度不能大于300！");

        RuleFor(e => e.Elem04).MaximumLength(300).WithMessage("要素4长度不能大于300！");

        RuleFor(e => e.Elem05).MaximumLength(300).WithMessage("要素5长度不能大于300！");

        RuleFor(e => e.Elem06).MaximumLength(300).WithMessage("要素6长度不能大于300！");

        RuleFor(e => e.Elem07).MaximumLength(300).WithMessage("要素7长度不能大于300！");

        RuleFor(e => e.Elem08).MaximumLength(300).WithMessage("要素8长度不能大于300！");

    }
}