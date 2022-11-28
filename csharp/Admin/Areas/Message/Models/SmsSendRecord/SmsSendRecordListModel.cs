using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.SmsSendRecord;

public class SmsSendRecordListModel : ListModel
{
    public SmsSendRecordListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<SmsSendRecordInfo> SmsSendRecordList { get; set; } = new List<SmsSendRecordInfo>();
}