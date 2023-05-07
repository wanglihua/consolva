using Microsoft.Extensions.Caching.Memory;
using NHibernate;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.LogisticsOrder;
using shunda.Domain.Message;
using shunda.Domain.Pass;
using shunda.Service.CustomerMessage;
using ISession = NHibernate.ISession;

namespace shunda.Service.Message;

/// <summary>
/// 客户消息共用处理
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageCommonService {
    public static readonly string ShundaSecretary = "舜达小秘";

    public static readonly string PhotoRobot = "舜达智能拍照机器人";

    private readonly ISession session;
    private readonly IMemoryCache memoryCache;
    private SmsService smsService;
    private WechatMessageService wechatMessageService;
    private CustomerMessageClientCustomizeService customerMessageClientCustomizeService;

    public CustomerMessageCommonService(ISession session,
        IMemoryCache memoryCache,
        SmsService smsService,
        WechatMessageService wechatMessageService,
        CustomerMessageClientCustomizeService customerMessageClientCustomizeService) {
        this.session = session;
        this.memoryCache = memoryCache;
        this.smsService = smsService;
        this.wechatMessageService = wechatMessageService;
        this.customerMessageClientCustomizeService = customerMessageClientCustomizeService;
    }

    private static readonly string MESSAGE_SETTING_CACHE_KEY = "CUSTOMER_MESSAGE_SETTING_";

    public CustomerMessageRealTimeSendSettingInfo GetCustomerMessageRealTimeSendSetting(CustomerMessageType messageType) {
        string cacheKey = MESSAGE_SETTING_CACHE_KEY + messageType.Id;

        if (memoryCache.TryGetValue(cacheKey, out CustomerMessageRealTimeSendSettingInfo? setting) == false) {
            setting = session
                .Query<CustomerMessageRealTimeSendSettingInfo>()
                .Where(e => e.Type == messageType)
                .Take(1)
                .SingleOrDefault();

            if (setting == null) {
                // throw new Exception("指定的消息发送配置不存在！");
                setting = new CustomerMessageRealTimeSendSettingInfo() {
                    // 这样，新添加一个消息类型的时候，设置条目数据库还没生成，也不会在这里报错
                    Status = CustomerMessageSettingStatus.Disabled
                };
            }

            memoryCache.Set(cacheKey, setting, TimeSpan.FromMinutes(5)); // 5 分钟是否合适，消息频繁发送的时候会有收益
        }

        return setting!;
    }

    public CustomerMessageCycleTimingSendSettingInfo GetCustomerMessageCycleTimingSendSetting(CustomerMessageType messageType) {
        string cacheKey = MESSAGE_SETTING_CACHE_KEY + messageType.Id;

        if (memoryCache.TryGetValue(cacheKey, out CustomerMessageCycleTimingSendSettingInfo? setting)) {
            setting = session
                .Query<CustomerMessageCycleTimingSendSettingInfo>()
                .Where(e => e.Type == messageType)
                .Take(1)
                .SingleOrDefault();

            if (setting == null) {
                // throw new Exception("指定的消息发送配置不存在！");
                setting = new CustomerMessageCycleTimingSendSettingInfo() {
                    // 这样，新添加一个消息类型的时候，设置条目数据库还没生成，也不会在这里报错
                    Status = CustomerMessageSettingStatus.Disabled
                };
            }

            memoryCache.Set(cacheKey, setting, TimeSpan.FromMinutes(5)); // 5 分钟是否合适，消息频繁发送的时候会有收益
        }

        return setting!;
    }

    public void MakeSettingCacheInvalid() {
        var messageTypeList = CustomerMessageType.GetList();
        foreach (var messageType in messageTypeList) {
            memoryCache.Remove(MESSAGE_SETTING_CACHE_KEY + messageType.Id);
        }
    }

    public int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, LogisticsOrderInfo? order, int relatedId, string message, CustomerMessageStatus messageStatus, CustomerMessageRealTimeSendSettingInfo setting, string sender) {
        CustomerMessageInfo customerMessage = new CustomerMessageInfo();
        customerMessage.SendType = sendType;
        customerMessage.Type = setting.Type;
        customerMessage.Customer = customer;

        // 其实可以合成一句 customerMessage.Order = order;

        if (order != null) {
            customerMessage.Order = order;
        } else {
            customerMessage.Order = null;
        }

        customerMessage.RelatedId = relatedId;

        customerMessage.Content = message;
        customerMessage.SmsSend = setting.SmsSend;
        customerMessage.WechatSend = setting.WechatSend;
        customerMessage.SendTime = DateTime.Now;
        customerMessage.Status = messageStatus;
        customerMessage.Sender = sender;

        session.Save(customerMessage);
        session.Flush();

        return customerMessage.Id;
    }

    public int SaveCustomerMessage(CustomerMessageSendType sendType, CustomerInfo customer, LogisticsOrderInfo? order, int relatedId, string message, CustomerMessageStatus messageStatus, CustomerMessageCycleTimingSendSettingInfo setting, string sender) {
        CustomerMessageInfo customerMessage = new CustomerMessageInfo();
        customerMessage.SendType = sendType;
        customerMessage.Type = setting.Type;
        customerMessage.Customer = customer;

        // 其实可以合成一句 customerMessage.Order = order;

        if (order != null) {
            customerMessage.Order = order;
        } else {
            customerMessage.Order = null;
        }

        customerMessage.RelatedId = relatedId;

        customerMessage.Content = message;
        customerMessage.SmsSend = setting.SmsSend;
        customerMessage.WechatSend = setting.WechatSend;
        customerMessage.SendTime = DateTime.Now;
        customerMessage.Status = messageStatus;
        customerMessage.Sender = sender;

        session.Save(customerMessage);
        session.Flush();

        return customerMessage.Id;
    }

    /// <summary>
    /// 发送订单通知
    /// </summary>
    /// <param name="customer">customer</param>
    /// <param name="orderCode">orderCode</param>
    /// <param name="customerCode">customerCode</param>
    /// <param name="sender">sender</param>
    /// <param name="url"></param>
    public void SendOrderNoticeTemplateMessage(CustomerInfo customer, string orderCode, string customerCode, string sender, string url = "") {
        orderCode = orderCode + ",点击查看详情";
        wechatMessageService.SendOrderNoticeTemplateMessage(customer, string.Empty, orderCode, customerCode, string.Empty, sender, url);
    }

    /// <summary>
    /// 发送短信
    /// </summary>
    /// <param name="mobile">不加国家代号的手机号码</param>
    /// <param name="message">message</param>
    /// <returns></returns>
    public SendSmsResult SendSms(string mobile, string message) {
        return smsService.SendSms(mobile, message);
    }

    /// <summary>
    /// 按客户消息定制，是否发送
    /// </summary>
    /// <param name="customerId"></param>
    /// <param name="messageType"></param>
    /// <returns></returns>
    public bool CustomerShouldSend(int customerId, CustomerMessageType messageType) {
        return customerMessageClientCustomizeService.ShouldSend(customerId, messageType);
    }

    public string GetMessageTitle(CustomerMessageType messageType, string assignedTitle = "") {
        if (string.IsNullOrWhiteSpace(assignedTitle) == false) {
            return assignedTitle;
        }

        return messageType.Name;
    }
}