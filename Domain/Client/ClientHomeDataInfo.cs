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
    public virtual int CustomerId { get; set; }

    [JsonProperty("expressClaimCount")]
    public virtual int ExpressClaimCount { get; set; }

    [JsonProperty("expressExpectCount")]
    public virtual int ExpressExpectCount { get; set; }

    [JsonProperty("expressCheckinCount")]
    public virtual int ExpressCheckinCount { get; set; }

    [JsonProperty("expressReturnCount")]
    public virtual int ExpressReturnCount { get; set; }

    [JsonProperty("orderPayCount")]
    public virtual int OrderPayCount { get; set; }

    [JsonProperty("orderSubmittedCount")]
    public virtual int OrderSubmittedCount { get; set; }

    [JsonProperty("orderWaitingTransportCount")]
    public virtual int OrderWaitingTransportCount { get; set; }

    [JsonProperty("orderTransportingCount")]
    public virtual int OrderTransportingCount { get; set; }

    [JsonProperty("orderDeliveredCount")]
    public virtual int OrderDeliveredCount { get; set; }

    [JsonProperty("newMessageCount")]
    public virtual int NewMessageCount { get; set; }

    [JsonProperty("newNoticeCount")]
    public virtual int NewNoticeCount { get; set; }

    /// <summary>
    /// 是否有可抢的优惠券批次
    /// </summary>
    [JsonProperty("hasCouponBatchPick")]
    public virtual bool HasCouponBatchPick { get; set; }

    [JsonProperty("useAgreementHasNew")]
    public virtual bool UseAgreementHasNew { get; set; }

    [JsonProperty("insuranceClauseHasNew")]
    public virtual bool InsuranceClauseHasNew { get; set; }

    [JsonProperty("freightDescHasNew")]
    public virtual bool FreightDescHasNew { get; set; }

    [JsonProperty("orderProcessHasNew")]
    public virtual bool OrderProcessHasNew { get; set; }

    [JsonProperty("promotionHasNew")]
    public virtual bool PromotionHasNew { get; set; }

    [JsonProperty("storageInstructionHasNew")]
    public virtual bool StorageInstructionHasNew { get; set; }

    [JsonProperty("transportLimitHasNew")]
    public virtual bool TransportLimitHasNew { get; set; }

    /// <summary>
    /// 是否已经关注公众号
    /// </summary>
    public virtual bool HasFollowed { get; set; }

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