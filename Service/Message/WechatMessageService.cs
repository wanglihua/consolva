using NLog;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using shunda.Core.Config;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Message;
using shunda.Domain.Wechat;
using shunda.Service.Wechat;

namespace shunda.Service.Message;

[Service(ServiceType.Scoped)]
public class WechatMessageService {
    private static readonly Logger log = LogManager.GetCurrentClassLogger();

    private readonly WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService;

    public WechatMessageService(WechatOfficialAccountAccessTokenService wechatOfficialAccountAccessTokenService) {
        this.wechatOfficialAccountAccessTokenService = wechatOfficialAccountAccessTokenService;
    }

    /// <summary>
    /// 发送模板消息
    /// </summary>
    /// <param name="validOpenId">有效的openId，如果客户已经取消关注公众号，请将validOpenId置空。或者调用该方法前提前放弃调用。</param>
    /// <param name="templateId">模板Id</param>
    /// <param name="data"></param>
    /// <param name="url"></param>
    /// <returns>是否发送成功</returns>
    public CustomerMessageStatus SendTemplateMessage(string validOpenId, string templateId, object data, string url = "") {
        if (string.IsNullOrWhiteSpace(validOpenId) == false) {
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
                OpenId = validOpenId,
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

    private class SendTemplateMessageThreadParam {
        public string AccessToken { get; set; } = string.Empty;

        public string OpenId { get; set; } = string.Empty;

        public string TemplateId { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;

        public object Data { get; set; } = new();
    }

    private void SendTemplateMessageNewThread(object? paramObj) {
        SendTemplateMessageThreadParam? param = paramObj as SendTemplateMessageThreadParam;
        if (param == null) {
            return;
        }

        string accessToken = param.AccessToken;
        string openId = param.OpenId;
        string templateId = param.TemplateId;
        string url = param.Url;
        object data = param.Data;

        try {
            var miniProgram = new TemplateModel_MiniProgram {
                appid = WechatAppConfig.AppId,
                pagepath = url,
            };
            // todo a 小程序发布后改回来 TemplateApi.SendTemplateMessage(accessToken, openId, templateId, url, data, miniProgram);
            // TemplateApi.SendTemplateMessage(accessToken, openId, templateId, null, data, null);
            TemplateApi.SendTemplateMessage(accessToken, openId, templateId, url, data, miniProgram);
        }
        catch (Exception exp) {
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
    public CustomerMessageStatus SendOrderNoticeTemplateMessage(CustomerInfo customer, string first, string orderCode, string customerCode, string remark, string sender, string url = "") {
        string openId = customer.WechatOfficialAccountOpenId;
        if (string.IsNullOrWhiteSpace(openId) || customer.WechatOfficialAccountFollowStatus != WechatOfficialAccountFollowStatus.Followed) {
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
        /*
         
            {{ first.DATA }}
            订单号：{{ keyword1.DATA }}
            订单内容：{{ keyword2.DATA }}
            收货人：{{ keyword3.DATA }}
            {{ remark.DATA }}
         
         */
        var firstTemplateDataItem = new TemplateDataItem(first);
        var keyword1TemplateDataItem = new TemplateDataItem(orderCode);

        string keyword2Str = $"操作时间:北京时间{DateTime.Now:yyyy-MM-dd HH:mm}";
        if (string.IsNullOrWhiteSpace(sender) == false) {
            keyword2Str += $", 操作人:{sender}";
        }

        var keyword2TemplateDataItem = new TemplateDataItem(keyword2Str);

        var keyword3TemplateDataItem = new TemplateDataItem($"专属会员号 {customerCode}");
        // var remarkTemplateDataItem = new TemplateDataItem(remark, "#FF0000");
        var remarkTemplateDataItem = new TemplateDataItem(remark);

        var data = new {
            first = firstTemplateDataItem,
            keyword1 = keyword1TemplateDataItem,
            keyword2 = keyword2TemplateDataItem,
            keyword3 = keyword3TemplateDataItem,
            remark = remarkTemplateDataItem
        };

        return SendTemplateMessage(openId, WechatOfficialAccountConfig.OrderNoticeTemplateId, data, url);
#endif
    }
}