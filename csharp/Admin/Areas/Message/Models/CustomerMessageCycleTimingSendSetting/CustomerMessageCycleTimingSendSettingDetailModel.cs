using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageCycleTimingSendSetting;

public class CustomerMessageCycleTimingSendSettingDetailModel : CustomerMessageCycleTimingSendSettingSaveModel
{
    [BindNever]
    public IList<CustomerMessageType> MessageTypeList { get; set; } = CustomerMessageType.GetList();

    [BindNever]
    public IList<CustomerMessageSettingStatus> StatusList { get; set; } = CustomerMessageSettingStatus.GetList();

    [BindNever]
    public IList<CustomerMessageCycleTimingSendIntervalType> IntervalTypeList { get; set; } = CustomerMessageCycleTimingSendIntervalType.GetList();

    [BindNever]
    public IList<int> IntervalWeeksList { get; set; } = new List<int>();

    [BindNever]
    public IList<int> IntervalDaysList { get; set; } = new List<int>();

    [BindNever]
    public IList<int> SendWeekDayList { get; set; } = new List<int>();

    [BindNever]
    public IList<int> SendHourList { get; set; } = new List<int>();
}