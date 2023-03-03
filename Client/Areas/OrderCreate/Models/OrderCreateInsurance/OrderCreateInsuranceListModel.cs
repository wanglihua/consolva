using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateInsurance;

public class OrderCreateInsuranceListModel {
    [JsonProperty("insuranceList")]
    public IList<OrderCreateInsuranceListItem> InsuranceList = new List<OrderCreateInsuranceListItem>();

    [JsonProperty("selectInsuranceId")]
    public int SelectInsuranceId { get; set; }

    [JsonProperty("insuranceClauseUrl")]
    public string InsuranceClauseUrl { get; set; } = string.Empty;
}