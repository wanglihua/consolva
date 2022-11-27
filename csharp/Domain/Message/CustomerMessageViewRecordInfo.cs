using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace shunda.Domain.Message;

/// <summary>
/// 客户消息查看
/// </summary>
public class CustomerMessageViewRecordInfo
{
    /// <summary>
    /// 客户，非空
    /// </summary>
    [Required(ErrorMessage = "客户不能为空！")]
    [JsonProperty("customerId")]
    public virtual int CustomerId { get; set; }

    /// <summary>
    /// 最后查看消息ID，非空
    /// </summary>
    [Required(ErrorMessage = "最后查看消息ID不能为空！")]
    [JsonProperty("lastViewMessageId")]
    public virtual int LastViewMessageId { get; set; }

    /// <summary>
    /// 最后查看时间，非空
    /// </summary>
    [Required(ErrorMessage = "最后查看时间不能为空！")]
    [JsonProperty("lastViewTime")]
    public virtual DateTime LastViewTime { get; set; } = DateTime.MinValue;

    #region override

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj is CustomerMessageViewRecordInfo == false)
        {
            return false;
        }

        CustomerMessageViewRecordInfo? another = obj as CustomerMessageViewRecordInfo;
        if (another == null)
        {
            return false;
        }

        if (CustomerId == another.CustomerId)
        {
            return true;
        }

        return false;
    }

    public override int GetHashCode()
    {
        if (CustomerId == 0)
        {
            return base.GetHashCode();
        }

        return CustomerId.GetHashCode();
    }

    #endregion
}