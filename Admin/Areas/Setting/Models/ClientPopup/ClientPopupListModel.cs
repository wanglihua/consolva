using shunda.Core;
using shunda.Domain.Client;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace shunda.Admin.Areas.Setting.Models.ClientPopup;

public class ClientPopupListModel  {
    [BindNever]
    public string ClientPopupAlias { get; set; } = string.Empty;    

    [BindNever]
    public IList<ClientPopupInfo> ClientPopupList { get; set; } = new List<ClientPopupInfo>();
}