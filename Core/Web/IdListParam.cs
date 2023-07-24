using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace shunda.Core.Web;

public class IdListParam {
    [BindProperty(Name = "idList")]
    [JsonProperty("idList")]
    public IList<int> IdList { get; set; } = new List<int>();

    public void Validate(string message = "请选择要操作的数据项！") {
        if (IdList.Count == 0) {
            throw new ValidationException(message);
        }
    }
}

public class IdListParamValidator : AbstractValidator<IdListParam> {
    public IdListParamValidator() {
        RuleFor(e => e.IdList).NotNull().NotEmpty().WithMessage("请选择要操作的数据项！");
    }
}