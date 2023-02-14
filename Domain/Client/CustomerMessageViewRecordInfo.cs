namespace shunda.Domain.Client;

public class CustomerMessageViewRecordInfo {
    /// <summary>
    /// 客户Id
    /// </summary>
    public int CustomerId { get; set; }

    /// <summary>
    /// 最后客户查看消息Id
    /// </summary>
    public int LastViewMessageId { get; set; }

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