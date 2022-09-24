using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Pass;

namespace shunda.Admin.Areas.Pass.Models.PassRes;

public class PassResIndexModel
{
    [BindNever]
    public IList<PassResCateInfo> CateList { get; set; } = new List<PassResCateInfo>();

    [BindNever]
    public string PassResCateAlias { get; set; } = string.Empty;
}