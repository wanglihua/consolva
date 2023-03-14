using FluentValidation;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Pay.Models.WechatAppExpressReturnWechatPay;

public class WechatAppExpressReturnWechatPayCreateParam {
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }
}

public class WechatAppExpressReturnWechatPayCreateParamValidator : AbstractValidator<WechatAppExpressReturnWechatPayCreateParam> {
    public WechatAppExpressReturnWechatPayCreateParamValidator() {
        RuleFor(e => e.ExpressId).Must(p => p > 0).WithMessage("快递单号ID不能为空！");
    }
}