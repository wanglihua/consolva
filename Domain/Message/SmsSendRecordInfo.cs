using shunda.Core;

namespace shunda.Domain.Message;

/// <summary>
/// 短信发送记录
/// </summary>
public class SmsSendRecordInfo : EntityNoUpdate
{
    /// <summary>
    /// 手机号，非空
    /// </summary>
    public virtual string Mobile { get; set; } = string.Empty;

    /// <summary>
    /// 消息内容，非空
    /// </summary>
    public virtual string Message { get; set; } = string.Empty;

    /// <summary>
    /// 消息ID，非空
    /// </summary>
    public virtual string SendMsgId { get; set; } = string.Empty;

    /// <summary>
    /// 返回代码，非空
    /// </summary>
    public virtual string SendCode { get; set; } = string.Empty;

    /// <summary>
    /// 返回错误，非空
    /// </summary>
    public virtual string SendError { get; set; } = string.Empty;
}