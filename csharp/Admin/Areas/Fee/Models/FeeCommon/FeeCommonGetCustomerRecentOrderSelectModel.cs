using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Fee.Models.FeeCommon;

public class FeeCommonGetCustomerRecentOrderSelectModel
{
    [BindNever]
    public IList<string> OrderCodeList { get; set; } = new List<string>();
}