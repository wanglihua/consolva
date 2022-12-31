using Newtonsoft.Json;
using shunda.Client.Areas.Express.Models.ExpressCheckin;

namespace shunda.Client.Areas.Express.Models.ExpressCheckinPrice;

public class ExpressCheckinPriceModel {
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("trackingNo")]
    public string TrackingNo { get; set; } = string.Empty;

    [JsonProperty("checkinTrackingNo")]
    public string CheckinTrackingNo { get; set; } = string.Empty;

    [JsonProperty("goodsList")]
    public IList<ExpressCheckinGoodsListItem> GoodsList = new List<ExpressCheckinGoodsListItem>();
}