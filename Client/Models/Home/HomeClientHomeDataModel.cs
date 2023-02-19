using Newtonsoft.Json;

namespace shunda.Client.Models.Home;

public class HomeClientHomeDataModel {
    [JsonProperty("expressClaimBadge")]
    public string ExpressClaimBadge { get; set; } = string.Empty;

    [JsonProperty("expressExpectBadge")]
    public string ExpressExpectBadge { get; set; } = string.Empty;

    [JsonProperty("expressCheckinBadge")]
    public string ExpressCheckinBadge { get; set; } = string.Empty;

    [JsonProperty("expressReturnBadge")]
    public string ExpressReturnBadge { get; set; } = string.Empty;

    [JsonProperty("orderPayBadge")]
    public string OrderPayBadge { get; set; } = string.Empty;

    [JsonProperty("orderSubmittedBadge")]
    public string OrderSubmittedBadge { get; set; } = string.Empty;

    [JsonProperty("orderWaitingTransportBadge")]
    public string OrderWaitingTransportBadge { get; set; } = string.Empty;

    [JsonProperty("orderTransportingBadge")]
    public string OrderTransportingBadge { get; set; } = string.Empty;

    [JsonProperty("orderDeliveredBadge")]
    public string OrderDeliveredBadge { get; set; } = string.Empty;

    [JsonProperty("newMessageBadge")]
    public string NewMessageBadge { get; set; } = string.Empty;

    [JsonProperty("couponBatchPickBadge")]
    public string CouponBatchPickBadge { get; set; } = string.Empty;

    [JsonProperty("useAgreementBadge")]
    public string UseAgreementBadge { get; set; } = string.Empty;

    [JsonProperty("insuranceClauseBadge")]
    public string InsuranceClauseBadge { get; set; } = string.Empty;

    [JsonProperty("freightDescBadge")]
    public string FreightDescBadge { get; set; } = string.Empty;

    [JsonProperty("orderProcessBadge")]
    public string OrderProcessBadge { get; set; } = string.Empty;

    [JsonProperty("promotionBadge")]
    public string PromotionBadge { get; set; } = string.Empty;

    [JsonProperty("storageInstructionBadge")]
    public string StorageInstructionBadge { get; set; } = string.Empty;

    [JsonProperty("transportLimitBadge")]
    public string TransportLimitBadge { get; set; } = string.Empty;

    [JsonProperty("hasFollowed")]
    public bool HasFollowed { get; set; }
}