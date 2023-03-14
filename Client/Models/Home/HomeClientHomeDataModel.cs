using Newtonsoft.Json;

namespace shunda.Client.Models.Home;

public class HomeClientHomeDataModel {
    [JsonProperty("topImageList")]
    public IList<HomeClientImageItem> TopImageList { get; set; } = new List<HomeClientImageItem>();

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

    [JsonProperty("newNoticeBadge")]
    public string NewNoticeBadge { get; set; } = string.Empty;

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

    [JsonProperty("goodsTypeBadge")]
    public string GoodsTypeBadge { get; set; } = string.Empty;

    [JsonProperty("hasFollowed")]
    public bool HasFollowed { get; set; }

    [JsonProperty("useAgreementUrl")]
    public string UseAgreementUrl { get; set; } = string.Empty;

    [JsonProperty("insuranceClauseUrl")]
    public string InsuranceClauseUrl { get; set; } = string.Empty;

    [JsonProperty("freightDescUrl")]
    public string FreightDescUrl { get; set; } = string.Empty;

    [JsonProperty("orderProcessUrl")]
    public string OrderProcessUrl { get; set; } = string.Empty;

    [JsonProperty("promotionUrl")]
    public string PromotionUrl { get; set; } = string.Empty;

    [JsonProperty("storageInstructionUrl")]
    public string StorageInstructionUrl { get; set; } = string.Empty;

    [JsonProperty("goodsTypeUrl")]
    public string GoodsTypeUrl { get; set; } = string.Empty;

    [JsonProperty("followTipUrl")]
    public string FollowTipUrl { get; set; } = string.Empty;

    [JsonProperty("beginnerGuideUrl")]
    public string BeginnerGuideUrl { get; set; } = string.Empty;

    [JsonProperty("faqUrl")]
    public string FaqUrl { get; set; } = string.Empty;
}