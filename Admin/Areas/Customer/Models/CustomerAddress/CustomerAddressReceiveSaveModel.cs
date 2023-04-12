using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using shunda.Domain;

namespace shunda.Admin.Areas.Customer.Models.CustomerAddress;

public class CustomerAddressReceiveSaveModel {
    public int Id { get; set; }
    
    /// <summary>
    /// 客户id，非空
    /// </summary>
    [Required(ErrorMessage = "客户id不能为空！")]
    public int CustomerId { get; set; } = new();

    /// <summary>
    /// 国家，非空
    /// </summary>
    [Required(ErrorMessage = "国家不能为空！")]
    [ModelBinder(typeof(CountryModelBinder))]
    public Country Country { get; set; } = Country.None;

    /// <summary>
    /// 收货人名，非空
    /// </summary>
    [Required(ErrorMessage = "收货人名不能为空！")]
    [MaxLength(60, ErrorMessage = "收货人名长度不能大于60！")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// 收货人姓，非空
    /// </summary>
    [Required(ErrorMessage = "收货人姓不能为空！")]
    [MaxLength(60, ErrorMessage = "收货人姓长度不能大于60！")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// 手机，非空
    /// </summary>
    [Required(ErrorMessage = "手机不能为空！")]
    [MaxLength(60, ErrorMessage = "手机长度不能大于60！")]
    public string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// 省份，非空
    /// </summary>
    [Required(ErrorMessage = "省份不能为空！")]
    [MaxLength(50, ErrorMessage = "省份长度不能大于50！")]
    public string State { get; set; } = string.Empty;

    /// <summary>
    /// 城市，非空
    /// </summary>
    [Required(ErrorMessage = "城市不能为空！")]
    [MaxLength(50, ErrorMessage = "城市长度不能大于50！")]
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// 地址，非空
    /// </summary>
    [Required(ErrorMessage = "地址不能为空！")]
    [MaxLength(200, ErrorMessage = "地址长度不能大于200！")]
    public string Address { get; set; } = string.Empty;

    /// <summary>
    /// Unit号，非空
    /// </summary>
    [MaxLength(20, ErrorMessage = "Unit号长度不能大于20！")]
    public string UnitLoc { get; set; } = string.Empty;

    /// <summary>
    /// 邮政编码，非空
    /// 在英国，澳大利亚等，可能用 postcode 更合适
    /// 在美国 用 zipcode 较适合
    /// </summary>
    [Required(ErrorMessage = "邮政编码不能为空！")]
    [MaxLength(10, ErrorMessage = "邮政编码长度不能大于10！")]
    public string Zipcode { get; set; } = string.Empty;

    /// <summary>
    /// ABN/ACN，非空
    /// </summary>
    [MaxLength(100, ErrorMessage = "ABN/ACN长度不能大于100！")]
    public string Tfn { get; set; } = string.Empty;

    /// <summary>
    /// 公司，非空
    /// </summary>
    [MaxLength(100, ErrorMessage = "公司长度不能大于100！")]
    public string Company { get; set; } = string.Empty;
}

public class CustomerAddressReceiveSaveModelValidator : AbstractValidator<CustomerAddressReceiveSaveModel> {
    public CustomerAddressReceiveSaveModelValidator() {
        RuleFor(e => e.CustomerId).Must(p => p > 0).WithMessage("客户id不能为空！");
        RuleFor(e => e.Country).Must(p => p != null && p != Country.None).WithMessage("国家不能为空！");
        RuleFor(e => e.FirstName).NotNull().NotEmpty().WithMessage("收货人名不能为空！");
        RuleFor(e => e.FirstName).MaximumLength(60).WithMessage("收货人名长度不能大于60！");
        RuleFor(e => e.LastName).NotNull().NotEmpty().WithMessage("收货人姓不能为空！");
        RuleFor(e => e.LastName).MaximumLength(60).WithMessage("收货人姓长度不能大于60！");
        RuleFor(e => e.Mobile).NotNull().NotEmpty().WithMessage("手机不能为空！");
        RuleFor(e => e.Mobile).MaximumLength(60).WithMessage("手机长度不能大于60！");
        RuleFor(e => e.State).NotNull().NotEmpty().WithMessage("省份不能为空！");
        RuleFor(e => e.State).MaximumLength(50).WithMessage("省份长度不能大于50！");
        RuleFor(e => e.City).NotNull().NotEmpty().WithMessage("城市不能为空！");
        RuleFor(e => e.City).MaximumLength(50).WithMessage("城市长度不能大于50！");
        RuleFor(e => e.Address).NotNull().NotEmpty().WithMessage("地址不能为空！");
        RuleFor(e => e.Address).MaximumLength(200).WithMessage("地址长度不能大于200！");
        RuleFor(e => e.UnitLoc).MaximumLength(20).WithMessage("Unit号长度不能大于20！");
        RuleFor(e => e.Zipcode).NotNull().NotEmpty().WithMessage("邮政编码不能为空！");
        RuleFor(e => e.Zipcode).MaximumLength(10).WithMessage("邮政编码长度不能大于10！");
        RuleFor(e => e.Tfn).MaximumLength(100).WithMessage("ABN/ACN长度不能大于100！");
        RuleFor(e => e.Company).MaximumLength(100).WithMessage("公司长度不能大于100！");
    }
}