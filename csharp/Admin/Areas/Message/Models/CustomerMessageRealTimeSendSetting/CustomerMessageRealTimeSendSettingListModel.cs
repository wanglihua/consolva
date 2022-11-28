using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageRealTimeSendSetting;

public class CustomerMessageRealTimeSendSettingListModel : ListModel
{
    public CustomerMessageRealTimeSendSettingListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<CustomerMessageRealTimeSendSettingInfo> CustomerMessageRealTimeSendSettingList { get; set; } = new List<CustomerMessageRealTimeSendSettingInfo>();
}