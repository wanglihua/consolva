using Newtonsoft.Json;

namespace shunda.Domain.Client;

/// <summary>
/// 用于接收 client_home_data 存储过程的输出结果
/// </summary>
public class ClientHomeDataInfo {
    /// <summary>
    /// 客户Id
    /// </summary>
    [JsonProperty("customerId")]
    public int CustomerId { get; set; }

    [JsonProperty("expressClaimCount")]
    public int ExpressClaimCount { get; set; }

    [JsonProperty("expressExpectCount")]
    public int ExpressExpectCount { get; set; }

    [JsonProperty("expressExpectEnterCount")]
    public int ExpressExpectEnterCount { get; set; }

    [JsonProperty("expressCheckinCount")]
    public int ExpressCheckinCount { get; set; }

    [JsonProperty("expressReturnCount")]
    public int ExpressReturnCount { get; set; }

    [JsonProperty("orderPayCount")]
    public int OrderPayCount { get; set; }

    [JsonProperty("orderSubmittedCount")]
    public int OrderSubmittedCount { get; set; }

    [JsonProperty("orderWaitingTransportCount")]
    public int OrderWaitingTransportCount { get; set; }

    [JsonProperty("orderTransportingCount")]
    public int OrderTransportingCount { get; set; }

    [JsonProperty("orderDeliveredCount")]
    public int OrderDeliveredCount { get; set; }

    [JsonProperty("newMessageCount")]
    public int NewMessageCount { get; set; }

    /// <summary>
    /// 是否有可抢的优惠券批次
    /// </summary>
    [JsonProperty("hasCouponBatchPick")]
    public bool HasCouponBatchPick { get; set; }

    #region override

    public override bool Equals(object? obj) {
        if (obj == null) {
            return false;
        }

        if (obj is ClientHomeDataInfo == false) {
            return false;
        }

        if (CustomerId == (obj as ClientHomeDataInfo)?.CustomerId) {
            return true;
        }

        return false;
    }

    public override int GetHashCode() {
        if (CustomerId == 0) {
            return base.GetHashCode();
        }

        return CustomerId.GetHashCode();
    }

    #endregion
}