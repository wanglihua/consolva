using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.Message;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 帮我赚钱提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageHelpMeMakeMoneyService
{
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerMessageClientUrlService customerMessageClientUrlService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageHelpMeMakeMoneyService(ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerMessageClientUrlService customerMessageClientUrlService,
        CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerMessageClientUrlService = customerMessageClientUrlService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.HelpMeMakeMoney;

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

            setting.WechatMessageTemplate1 = "嗨，老板，今天舜达帮您赚钱了吗？动动手指，将【帮我赚钱】里您的专属海报转发微信朋友圈，让澳拼马不停蹄帮您赚钱哦！";
            setting.WechatMessageTemplate2 = string.Empty;
            setting.WechatMessageTemplate3 = string.Empty;
            setting.SmsMessageTemplate = "嗨，老板，今天舜达帮您赚钱了吗？将【帮我赚钱】里您的专属海报转发朋友圈就可以赚钱哦！";
            setting.Placeholders = string.Empty;
            setting.ClientCustomizable = false;
            setting.ClientTitle = messageType.Name;
            setting.ClientDesc = messageType.Description;

            session.Save(setting);
            session.Flush();
        }
        
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

            string processUser = CustomerMessageCommonService.ShundaSecretary;

            if (clientShouldSend)
            {
                if (setting.WechatSend)
                {
                    messageStatus = CustomerMessageStatus.Sent;
                    // string? url = GetMessageUrl();
                    string url = customerMessageClientUrlService.GetUrl(TerminalType.WechatApp, messageType);

                    if (string.IsNullOrWhiteSpace(message1) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "帮我赚钱提醒", customerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "帮我赚钱提醒", customerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "帮我赚钱提醒", customerCode, message3, processUser, url);
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
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.CycleTiming, customer, null, message1, messageStatus, setting, processUser);
            }

            if (string.IsNullOrWhiteSpace(message2) == false)
            {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.CycleTiming, customer, null, message2, messageStatus, setting, processUser);
            }

            if (string.IsNullOrWhiteSpace(message3) == false)
            {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.CycleTiming, customer, null, message3, messageStatus, setting, processUser);
            }

            session.Flush();
        }
    }
}