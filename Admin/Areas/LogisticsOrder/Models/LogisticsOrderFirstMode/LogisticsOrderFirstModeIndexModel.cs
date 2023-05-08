using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.Fee;
using shunda.Domain.Logistics;
using shunda.Domain.LogisticsTrack;

namespace shunda.Admin.Areas.LogisticsOrder.Models.LogisticsOrderFirstMode;

public class LogisticsOrderFirstModeIndexModel {
    [BindNever]
    public IList<LogisticsGoodsType> GoodsTypeList { get; set; } = new List<LogisticsGoodsType>();

    [BindNever]
    public IList<LogisticsModeType> FirstTypeList { get; set; } = new List<LogisticsModeType>();

    [BindNever]
    public IList<LogisticsModeChannel> ChannelList { get; set; } = new List<LogisticsModeChannel>();

    [BindNever]
    public IList<SeventeenTrackNumberTrackStatus> TrackStatusList { get; set; } = new List<SeventeenTrackNumberTrackStatus>();

    [BindNever]
    public IList<PayStatus> PayStatusList { get; set; } = new List<PayStatus>();
}