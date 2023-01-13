using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Core;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientAgreement;

public class ClientAgreementListModel : ListModel {
    public ClientAgreementListModel(ListPager listPager) : base(listPager) {
    }

    [BindNever]
    public IList<ClientAgreementInfo> ClientAgreementList { get; set; } = new List<ClientAgreementInfo>();
}