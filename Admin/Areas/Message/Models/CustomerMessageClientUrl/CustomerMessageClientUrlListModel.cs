using shunda.Core;
using shunda.Domain.Message;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Message.Models.CustomerMessageClientUrl;

public class CustomerMessageClientUrlListModel : ListModel
{
    public CustomerMessageClientUrlListModel(ListPager listPager) : base(listPager)
    {
    }

    public string CustomerMessageClientUrlAlias = string.Empty;    

    [BindNever]
    public IList<CustomerMessageClientUrlInfo> CustomerMessageClientUrlList { get; set; } = new List<CustomerMessageClientUrlInfo>();
}