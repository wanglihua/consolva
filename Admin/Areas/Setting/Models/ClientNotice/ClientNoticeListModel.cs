using shunda.Core;
using shunda.Domain.Client;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Setting.Models.ClientNotice;

public class ClientNoticeListModel : ListModel {
    public ClientNoticeListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public string ClientNoticeAlias { get; set; } = string.Empty;

    [BindNever]
    public string EditorUserAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<ClientNoticeInfo> ClientNoticeList { get; set; } = new List<ClientNoticeInfo>();
}