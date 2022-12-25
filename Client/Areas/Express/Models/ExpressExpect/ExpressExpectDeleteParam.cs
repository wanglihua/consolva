using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressExpect;

public class ExpressExpectDeleteParam : ExpressExpectListQueryParam {
    [JsonProperty("id")]
    [Required(ErrorMessage = "id不能为空！")]
    public int Id { get; set; } = 0;
}