using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain;
using shunda.Domain.Client;

namespace shunda.Admin.Areas.Setting.Models.ClientAgreement;

public class ClientAgreementDetailModel : ClientAgreementSaveModel {
    [BindNever]
    public IList<Country> CountryList { get; set; } = Country.GetNoChinaList();

    [BindNever]
    public IList<ClientAgreementStatus> StatusList { get; set; } = ClientAgreementStatus.GetList();
}