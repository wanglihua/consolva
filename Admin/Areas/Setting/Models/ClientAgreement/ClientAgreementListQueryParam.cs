using shunda.Core;

namespace shunda.Admin.Areas.Setting.Models.ClientAgreement;

public class ClientAgreementListQueryParam : ListQueryParam {
    public string? TitleQuery { get; set; }

    public string? UrlQuery { get; set; }

    public string? CountryQuery { get; set; }
}