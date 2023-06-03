using shunda.Core;
using shunda.Domain.Customer;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Customer.Models.CustomerChargeAwardSetting;

public class CustomerChargeAwardSettingListModel : ListModel {
    public CustomerChargeAwardSettingListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string CustomerChargeAwardSettingAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<CustomerChargeAwardSettingInfo> CustomerChargeAwardSettingList { get; set; } = new List<CustomerChargeAwardSettingInfo>();
}