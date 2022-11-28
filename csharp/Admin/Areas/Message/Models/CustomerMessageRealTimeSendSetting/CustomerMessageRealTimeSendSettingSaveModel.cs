using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageRealTimeSendSetting;

public class CustomerMessageRealTimeSendSettingSaveModel
{
    public CustomerMessageRealTimeSendSettingInfo CustomerMessageRealTimeSendSetting { get; set; } = new CustomerMessageRealTimeSendSettingInfo();
}