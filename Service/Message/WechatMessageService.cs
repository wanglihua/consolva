using NLog;
using Senparc.Weixin.MP.AdvancedAPIs;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Message;
using shunda.Service.Wechat;
#if !DEBUG
using shunda.Core.Config;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
#endif

namespace shunda.Service.Message;

[Service(ServiceType.Scoped)]
public class WechatMessageService
{
    private static Logger log = LogManager.GetCurrentClassLogger();

    private readonly WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService;

    public WechatMessageService(WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService)
    {
        this.wechatOfficialAccountAccessTokenService = wechatOfficialAccountAccessTokenService;
    }

    /// <summary>
    /// 发送模板消息
    /// </summary>
    /// <param name="openId">openId</param>
    /// <param name="templateId">模板Id</param>
    /// <param name="data"></param>
    /// <param name="url"></param>
    /// <returns>是否发送成功</returns>
    public CustomerMessageStatus SendTemplateMessage(string openId, string templateId, object data, string? url = null)
    {
        if (string.IsNullOrWhiteSpace(openId) == false)
        {
            string accessToken = wechatOfficialAccountAccessTokenService.GetAccessToken();

            // 这是非多线程发送
            // try
            // {
            //     TemplateApi.SendTemplateMessage(accessToken, openId, templateId, url, data);
            //     return CustomerMessageStatus.Sent;
            // }
            // catch (Exception exp)
            // {
            //     log.Error($"发送模板消息失败，openid: ${openId}, templateId: ${templateId}, data: ${data}");
            //     log.Error(exp);
            //
            //     return CustomerMessageStatus.SendFail;
            // }

            // 这是多线程发送
            SendTemplateMessageThreadParam param = new SendTemplateMessageThreadParam {
                AccessToken = accessToken,
                OpenId = openId,
                TemplateId = templateId,
                Url = url,
                Data = data,
            };

            new Thread(new ParameterizedThreadStart(SendTemplateMessageNewThread)).Start(param);

            return CustomerMessageStatus.Success; // 不管线程中是否发送成功，这里暂时返回是已发送
        }

        return CustomerMessageStatus.Fail; // 一般这时还没关注公众号，也算发送失败
    }

    #region 多线程发送

    private class SendTemplateMessageThreadParam
    {
        public string AccessToken { get; set; } = string.Empty;

        public string OpenId { get; set; } = string.Empty;

        public string TemplateId { get; set; } = string.Empty;

        public string? Url { get; set; }

        public object Data { get; set; } = new();
    }

    private void SendTemplateMessageNewThread(object? paramObj)
    {
        SendTemplateMessageThreadParam? param = paramObj as SendTemplateMessageThreadParam;
        if (param == null)
        {
            return;
        }

        string accessToken = param.AccessToken;
        string openId = param.OpenId;
        string templateId = param.TemplateId;
        string? url = param.Url;
        object data = param.Data;

        if (string.IsNullOrWhiteSpace(url))
        {
            url = null;
        }

        try
        {
            TemplateApi.SendTemplateMessage(accessToken, openId, templateId, url, data);
        }
        catch (Exception exp)
        {
            log.Error($"发送模板消息失败，openid: ${openId}, templateId: ${templateId}, data: ${data}");
            log.Error(exp);
        }
    }

    #endregion

    /// <summary>
    /// 发送订单通知
    /// </summary>
    /// <param name="customer">customer</param>
    /// <param name="first">first</param>
    /// <param name="orderCode">orderCode</param>
    /// <param name="customerCode">customerCode</param>
    /// <param name="remark">remark</param>
    /// <param name="sender">sender</param>
    /// <param name="url"></param>
    /// <returns>是否发送成功</returns>
    public CustomerMessageStatus SendOrderNoticeTemplateMessage(CustomerInfo customer, string first, string orderCode, string customerCode, string remark, string sender, string? url = null)
    {
        string openId = customer.WechatOfficialAccountOpenId;
        if (string.IsNullOrWhiteSpace(openId))
        {
            return CustomerMessageStatus.Fail; // 一般这时还没关注公众号，也算发送失败
        }

#if DEBUG
        log.Trace("SendOrderNoticeTemplateMessage: ");
        log.Trace($"openId: {openId}");
        log.Trace($"first: {first}");
        log.Trace($"orderCode: {orderCode}");
        log.Trace($"customerCode: {customerCode}");
        log.Trace($"remark: {remark}");
        log.Trace($"sender: {sender}");
        log.Trace($"url: {url}");


        return CustomerMessageStatus.Success;
#else
            DateTime processTime = DateTime.Now;

            var data = new {
                first = new TemplateDataItem(first),
                keyword1 = new TemplateDataItem(orderCode),
                keyword2 = new TemplateDataItem(customerCode),
                keyword3 = new TemplateDataItem(processTime.ToString("yyyy-MM-dd HH:mm") + " 北京时间"),
                keyword4 = new TemplateDataItem(sender),
                remark = new TemplateDataItem(remark)
            };

            return SendTemplateMessage(openId, WechatOfficialAccountConfig.OrderNoticeTemplateId, data, url);
#endif
    }

    /// <summary>
    /// 订单处理异常通知
    /// </summary>
    /// <param name="customer">customer</param>
    /// <param name="first">first</param>
    /// <param name="abnormalCause">异常原因</param>
    /// <param name="remark">remark</param>
    /// <param name="sender">sender</param>
    /// <returns>是否发送成功</returns>
    public CustomerMessageStatus SendOrderAbnormalNoticeTemplateMessage(CustomerInfo customer, string first, string abnormalCause, string remark, string sender)
    {
        string openId = customer.WechatOfficialAccountOpenId;
#if DEBUG
        log.Trace("SendOrderAbnormalNoticeTemplateMessage: ");
        log.Trace($"openId: {openId}");
        log.Trace($"first: {first}");
        log.Trace($"abnormalCause: {abnormalCause}");
        log.Trace($"remark: {remark}");
        log.Trace($"sender: {sender}");

        return CustomerMessageStatus.Success;
#else
            DateTime abnormalTime = DateTime.Now;

            var data = new {
                first = new TemplateDataItem(first),
                keyword1 = new TemplateDataItem(abnormalCause),
                keyword2 = new TemplateDataItem(abnormalTime.ToString("yyyy-MM-dd HH:mm") + " 北京时间"),
                remark = new TemplateDataItem(remark + "【" + sender + "】")
            };

            return SendTemplateMessage(openId, WechatOfficialAccountConfig.OrderAbnormalTemplateId, data);
#endif
    }
}