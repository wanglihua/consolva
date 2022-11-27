using shunda.Core;
using shunda.Core.Service;
using shunda.Domain.Customer;
using shunda.Domain.Message;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 取消关注公众号推送
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageCancelFollowService
{
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerClientUrlService customerClientUrlService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageCancelFollowService(ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerClientUrlService customerClientUrlService,
        CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerClientUrlService = customerClientUrlService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.CancelFollow;

    private string? GetMessageUrl()
    {
        return null;
    }

    public static void InitSetting(ISession session)
    {
        var setting = session.Query<CustomerMessageCycleTimingSendSettingInfo>()
            .Where(e => e.Type == messageType)
            .Take(1)
            .SingleOrDefault();
        if (setting == null)
        {
            setting = new CustomerMessageCycleTimingSendSettingInfo();
            setting.Type = messageType;
            setting.Status = CustomerMessageSettingStatus.Disabled;
            setting.WechatSend = true;
            setting.SmsSend = false;

            setting.WechatMessageTemplate1 = "亲，您取消关注舜达公众号，客服好伤心啊。关注公众号，登录系统免密码，实时通知更详细哦！";
            setting.SmsMessageTemplate = "亲，您取消关注舜达公众号，客服好伤心啊。关注公众号，登录系统免密码，实时通知更详细哦！";

            session.Save(setting);
            session.Flush();
        }

        setting.Placeholders = "";
        session.Update(setting);
        session.Flush();
    }

    public void SendMessage(CustomerInfo customer)
    {
        if (customer == null)
        {
            throw new ValidationException("消息发送指定的会员不能为空！");
        }

        if (customer.Status == CustomerStatus.Frozen)
        {
            return;
        }

        string customerCode = customerCodeService.GetCustomerCode(customer.Id);

        CustomerMessageCycleTimingSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageCycleTimingSendSetting(messageType);
        if (setting.Status == CustomerMessageSettingStatus.Enabled)
        {
            string message1 = setting.WechatMessageTemplate1;
            string message2 = setting.WechatMessageTemplate2;
            string message3 = setting.WechatMessageTemplate3;

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WechatSend || setting.SmsSend))
            {
                clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer.Id, messageType);
            }

            if (clientShouldSend)
            {
                if (setting.WechatSend)
                {
                    messageStatus = CustomerMessageStatus.Sent;
                    string processUser = CustomerMessageCommonService.ShundaSecretary;
                    string? url = GetMessageUrl();

                    if (string.IsNullOrWhiteSpace(message1) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "已取消关注提醒", customerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "已取消关注提醒", customerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "已取消关注提醒", customerCode, message3, processUser, url);
                    }
                }

                if (setting.SmsSend)
                {
                    messageStatus = CustomerMessageStatus.Sent;
                    customerMessageCommonService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                }
            }

            if (string.IsNullOrWhiteSpace(message1) == false)
            {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.CycleTiming, customer, null, message1, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message2) == false)
            {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.CycleTiming, customer, null, message2, messageStatus, setting);
            }

            if (string.IsNullOrWhiteSpace(message3) == false)
            {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.CycleTiming, customer, null, message3, messageStatus, setting);
            }

            session.Flush();
        }
    }
}