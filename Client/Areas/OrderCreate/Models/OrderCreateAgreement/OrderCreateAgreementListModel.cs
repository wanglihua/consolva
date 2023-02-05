using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateAgreement;

public class OrderCreateAgreementListModel {
    [JsonProperty("agreementList")]
    public IList<OrderCreateAgreementListItem> AgreementList { get; set; } = new List<OrderCreateAgreementListItem>();
}