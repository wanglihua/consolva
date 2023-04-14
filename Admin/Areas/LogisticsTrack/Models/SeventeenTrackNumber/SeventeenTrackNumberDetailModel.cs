using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using shunda.Domain.LogisticsTrack;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumber;

public class SeventeenTrackNumberDetailModel {
    /// <summary>
    /// Id
    /// </summary>
    public int SeventeenTrackNumberId { get; set; }

    /// <summary>
    /// 快递单号，非空
    /// </summary>
    public string TrackingNo { get; set; } = string.Empty;

    /// <summary>
    /// 服务商代码，非空
    /// </summary>
    public int CarrierKey { get; set; } = 0;

    /// <summary>
    /// 服务商名称，非空
    /// </summary>
    public string CarrierName { get; set; } = string.Empty;

    /// <summary>
    /// 运输商识别，非空
    /// </summary>
    [ModelBinder(typeof(SeventeenTrackNumberOriginModelBinder))]
    public SeventeenTrackNumberOrigin Origin { get; set; } = SeventeenTrackNumberOrigin.None;

    /// <summary>
    /// 错误代码，非空
    /// </summary>
    public int ErrorCode { get; set; } = 0;

    /// <summary>
    /// 错误描述，非空
    /// </summary>
    public string ErrorMessage { get; set; } = string.Empty;

    /// <summary>
    /// 状态，非空
    /// </summary>
    [ModelBinder(typeof(SeventeenTrackNumberStatusModelBinder))]
    public SeventeenTrackNumberStatus Status { get; set; } = SeventeenTrackNumberStatus.None;

    [BindNever]
    public IList<SeventeenTrackNumberOrigin> OriginList { get; set; } = new List<SeventeenTrackNumberOrigin>();

    [BindNever]
    public IList<SeventeenTrackNumberStatus> StatusList { get; set; } = new List<SeventeenTrackNumberStatus>();
}