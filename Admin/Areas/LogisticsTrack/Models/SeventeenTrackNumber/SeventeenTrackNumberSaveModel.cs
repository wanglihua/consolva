using Microsoft.AspNetCore.Mvc;
using shunda.Domain.LogisticsTrack;

namespace shunda.Admin.Areas.LogisticsTrack.Models.SeventeenTrackNumber;

public class SeventeenTrackNumberSaveModel {
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

}