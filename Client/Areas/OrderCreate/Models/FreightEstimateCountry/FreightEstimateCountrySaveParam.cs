using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.FreightEstimateCountry;

public class FreightEstimateCountrySaveParam {
    [Required(ErrorMessage = "目的国家不能为空！")]
    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}