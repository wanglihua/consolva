using shunda.Core;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Message;
using shunda.Service.Customer;
using ISession = NHibernate.ISession;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 快递单入库提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageExpressCheckinService
{
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerMessageClientUrlService customerMessageClientUrlService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageExpressCheckinService(ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerMessageClientUrlService customerMessageClientUrlService,
        CustomerCodeService customerCodeService)
    {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerMessageClientUrlService = customerMessageClientUrlService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.ExpressCheckin;

    public static void InitSetting(ISession session)
    {
        var setting = session.Query<CustomerMessageRealTimeSendSettingInfo>()
            .Where(e => e.Type == messageType)
            .Take(1)
            .SingleOrDefault();
        if (setting == null)
        {
            setting = new CustomerMessageRealTimeSendSettingInfo();
            setting.Type = messageType;
            setting.Status = CustomerMessageSettingStatus.Disabled;
            setting.WechatSend = true;
            setting.SmsSend = false;

            setting.WechatMessageTemplate1 = "亲，您快递单号【{tracking_no}】，已抵达国内收货仓，进入【已入库】完善货值即可立马完成入库哦！";
            setting.SmsMessageTemplate = "亲，您快递单号【{tracking_no}】，已抵达国内收货仓，进入【已入库】完善货值即可立马完成入库哦！";
            setting.Placeholders = "快递单号：{tracking_no}";

            setting.ClientCustomizable = false;
            setting.ClientTitle = messageType.Name;
            setting.ClientDesc = messageType.Description;

            session.Save(setting);
            session.Flush();
        }

        session.Update(setting);
        session.Flush();
    }

    private string TransferMessage(string message, string trackingNo)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return message;
        }

        message = message.Replace("{tracking_no}", trackingNo);

        return message;
    }

    public void SendMessage(CustomerInfo customer, ExpressInfo express)
    {
        if (customer == null)
        {
            throw new ValidationException("消息发送指定的会员不能为空！");
        }

        if (express == null)
        {
            throw new ValidationException("消息发送指定的快递单不能为空！");
        }

        string trackingNo = express.TrackingNo;

        if (customer.Status == CustomerStatus.Frozen)
        {
            return;
        }

        string customerCode = customerCodeService.GetCustomerCode(customer.Id);

        CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
        if (setting.Status == CustomerMessageSettingStatus.Enabled)
        {
            string message1 = TransferMessage(setting.WechatMessageTemplate1, trackingNo);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, trackingNo);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, trackingNo);

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
                    messageStatus = CustomerMessageStatus.Success;
                    // string? url = GetMessageUrl(express);
                    string url = customerMessageClientUrlService.GetUrl(TerminalType.WechatApp, messageType);

                    if (string.IsNullOrWhiteSpace(message1) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "【快递单入库】提醒", customerCode, message1, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "【快递单入库】提醒", customerCode, message2, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false)
                    {
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, "", "【快递单入库】提醒", customerCode, message3, processUser, url);
                    }
                }

                if (setting.SmsSend)
                {
                    messageStatus = CustomerMessageStatus.Success;
                    customerMessageCommonService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                }
            }

            if (string.IsNullOrWhiteSpace(message1) == false)
            {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message1, messageStatus, setting, processUser);
            }

            if (string.IsNullOrWhiteSpace(message2) == false)
            {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message2, messageStatus, setting, processUser);
            }

            if (string.IsNullOrWhiteSpace(message3) == false)
            {
                customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, message3, messageStatus, setting, processUser);
            }

            session.Flush();
        }
    }
}