using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageCycleTimingSendSetting;

public class CustomerMessageCycleTimingSendSettingListModel : ListModel
{
    public CustomerMessageCycleTimingSendSettingListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<CustomerMessageCycleTimingSendSettingInfo> CustomerMessageCycleTimingSendSettingList { get; set; } = new List<CustomerMessageCycleTimingSendSettingInfo>();
}