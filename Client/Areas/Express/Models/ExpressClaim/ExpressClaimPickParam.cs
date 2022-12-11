using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Client.Areas.Express.Models.ExpressClaim;

public class ExpressClaimPickParam
{
    [Required(ErrorMessage = "快递单ID不能为空！")]
    [JsonProperty("expressId")]
    public int ExpressId { get; set; }

    [Required(ErrorMessage = "快递单号不能为空！")]
    [JsonProperty("trackingNoInput")]
    public string TrackingNoInput { get; set; } = string.Empty;

    [JsonProperty("goodsPriceList")]
    public IList<ExpressGoodsPrice> GoodsPriceList { get; set; } = new List<ExpressGoodsPrice>();

    public class ExpressGoodsPrice
    {
        [JsonProperty("expressGoodsId")]
        public int ExpressGoodsId { get; set; }

        [JsonProperty("goodsName")]
        public string GoodsName { get; set; } = string.Empty;

        [JsonProperty("price")]
        public decimal Price { get; set; }
    }
}