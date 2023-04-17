using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Logistics;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeDetailModel : LogisticsOrderFirstModeSaveModel {
    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = new List<LogisticsGoodsType>();

    [BindNever]
    public IList<LogisticsModeType> FirstTypeList { get; set; } = new List<LogisticsModeType>();

    [BindNever]
    public IList<LogisticsModeChannel> ChannelList { get; set; } = new List<LogisticsModeChannel>();
}