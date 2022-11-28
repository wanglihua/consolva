namespace shunda.Admin.Areas.Message.Models.CustomerMessageManualSend;

public class CustomerMessageManualSendModel
{
    /// <summary>
    /// 客户ID列表，用 逗号 ， 隔开
    /// </summary>
    public string CustomerIds { get; set; } = String.Empty;

    public string RemarkMessage { get; set; } = String.Empty;

    /// <summary>
    /// 是否微信发送
    /// </summary>
    public bool SendByWechat { get; set; }

    /// <summary>
    /// 是否短信发送
    /// </summary>
    public bool SendBySms { get; set; }
}