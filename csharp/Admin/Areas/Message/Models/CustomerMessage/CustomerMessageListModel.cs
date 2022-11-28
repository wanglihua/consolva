using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Message;

namespace shunda.Admin.Areas.Message.Models.CustomerMessage;

public class CustomerMessageListModel : ListModel
{
    public CustomerMessageListModel(ListPager listPager) : base(listPager)
    {
    }

    [BindNever]
    public IList<CustomerMessageInfo> CustomerMessageList { get; set; } = new List<CustomerMessageInfo>();
}