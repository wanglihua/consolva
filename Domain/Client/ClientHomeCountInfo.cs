namespace shunda.Domain.Client;

/// <summary>
/// 用于接收 client_home_count 的结果
/// </summary>
public class ClientHomeCountInfo {
    /// <summary>
    /// 客户Id
    /// </summary>
    public int CustomerId { get; set; }

    public int ExpressClaimCount { get; set; }

    public int ExpressExpectCount { get; set; }

    public int ExpressExpectEnterCount { get; set; }

    public int ExpressCheckinCount { get; set; }

    public int ExpressReturnCount { get; set; }

    public int OrderPayCount { get; set; }

    public int OrderSubmittedCount { get; set; }

    public int OrderWaitingTransportCount { get; set; }

    public int OrderTransportingCount { get; set; }

    public int OrderDeliveredCount { get; set; }

    public int NewMessageCount { get; set; }

    /// <summary>
    /// 是否有可抢的优惠券批次
    /// </summary>
    public bool HasCouponBatchPick { get; set; }

/*
       v_new_message_count             AS new_message_count,
       v_coupon_batch_id IS NOT NULL   AS has_coupon_batch_pick;
           
 */


    #region override

    public override bool Equals(object? obj) {
        if (obj == null) {
            return false;
        }

        if (obj is CustomerMessageViewRecordInfo == false) {
            return false;
        }

        if (CustomerId == (obj as CustomerMessageViewRecordInfo)?.CustomerId) {
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