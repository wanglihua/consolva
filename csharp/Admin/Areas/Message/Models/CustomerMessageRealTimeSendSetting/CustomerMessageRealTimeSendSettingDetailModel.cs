using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageRealTimeSendSetting;

public class CustomerMessageRealTimeSendSettingDetailModel : CustomerMessageRealTimeSendSettingSaveModel
{
    /// <summary>
    /// 占位符
    /// </summary>
    public string Placeholders { get; set; } = string.Empty;

    [BindNever]
    public IList<CustomerMessageType> MessageTypeList { get; set; } = CustomerMessageType.GetList();

    [BindNever]
    public IList<CustomerMessageSettingStatus> StatusList { get; set; } = CustomerMessageSettingStatus.GetList();
}