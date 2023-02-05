using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.OrderCreate.Models.OrderCreateCountry;

public class OrderCreateCountrySaveParam {
    [Required(ErrorMessage = "目的国家不能为空！")]
    [JsonProperty("countryId")]
    public int CountryId { get; set; }
}