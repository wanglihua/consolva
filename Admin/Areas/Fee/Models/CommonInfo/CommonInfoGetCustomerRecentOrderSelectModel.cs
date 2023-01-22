using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Fee.Models.CommonInfo;

public class CommonInfoGetCustomerRecentOrderSelectModel {
    [BindNever]
    public IList<string> OrderCodeList { get; set; } = new List<string>();
}