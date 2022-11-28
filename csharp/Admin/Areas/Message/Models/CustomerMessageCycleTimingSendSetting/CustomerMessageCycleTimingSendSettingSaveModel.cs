using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageCycleTimingSendSetting;

public class CustomerMessageCycleTimingSendSettingSaveModel
{
    public CustomerMessageCycleTimingSendSettingInfo CustomerMessageCycleTimingSendSetting { get; set; } = new CustomerMessageCycleTimingSendSettingInfo();
}