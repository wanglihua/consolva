using shunda.Core;

namespace shunda.Admin.Areas.Message.Models.SmsSendRecord;

public class SmsSendRecordListQueryParam : ListQueryParam
{
    public string? MobileQuery { get; set; }
}