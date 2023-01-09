using Newtonsoft.Json;
using shunda.Domain.Logistics;

namespace shunda.Client.Areas.Order.Models.FreightEstimateGoods;

public class FreightEstimateGoodsDetailModel : FreightEstimateGoodsSaveModel {
    [JsonProperty("goodsTypeList")]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = LogisticsGoodsType.GetCustomerList();
}