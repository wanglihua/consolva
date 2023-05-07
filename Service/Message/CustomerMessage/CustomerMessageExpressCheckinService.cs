using NHibernate;
using shunda.Core;
using shunda.Core.Service;
using shunda.Domain;
using shunda.Domain.Customer;
using shunda.Domain.Express;
using shunda.Domain.Message;
using shunda.Service.Customer;

namespace shunda.Service.Message.CustomerMessage;

/// <summary>
/// 快递单入库提醒
/// </summary>
[Service(ServiceType.Scoped)]
public class CustomerMessageExpressCheckinService {
    private readonly ISession session;
    private readonly CustomerMessageCommonService customerMessageCommonService;
    private readonly CustomerCodeService customerCodeService;

    public CustomerMessageExpressCheckinService(ISession session,
        CustomerMessageCommonService customerMessageCommonService,
        CustomerCodeService customerCodeService) {
        this.session = session;
        this.customerMessageCommonService = customerMessageCommonService;
        this.customerCodeService = customerCodeService;
    }

    private static CustomerMessageType messageType => CustomerMessageType.ExpressCheckin;

    public static void InitSetting(ISession session) {
        var setting = session.Query<CustomerMessageRealTimeSendSettingInfo>()
            .Where(e => e.Type == messageType)
            .Take(1)
            .SingleOrDefault();
        if (setting == null) {
            setting = new CustomerMessageRealTimeSendSettingInfo {
                Type = messageType,
                Status = CustomerMessageSettingStatus.Disabled,
                WechatSend = true,
                SmsSend = false,
                WechatMessageTemplate1 = "亲，您快递单号【{tracking_no}】，已抵达国内收货仓，进入【已入库】完善货值即可立马完成入库哦！",
                SmsMessageTemplate = "亲，您快递单号【{tracking_no}】，已抵达国内收货仓，进入【已入库】完善货值即可立马完成入库哦！",
                Placeholders = "快递单号：{tracking_no}",
                ClientCustomizable = false,
                ClientTitle = messageType.Name,
                ClientDesc = messageType.Description
            };

            session.Save(setting);
            session.Flush();
        }
    }

    private string TransferMessage(string message, string trackingNo) {
        if (string.IsNullOrWhiteSpace(message)) {
            return string.Empty;
        }

        message = message.Replace("{tracking_no}", trackingNo);

        return message;
    }

    public string GetUrl(int expressId) {
        return $"pages/express/express_checkin/express_checkin_detail?id={expressId}";
    }

    public void SendMessage(CustomerInfo customer, ExpressInfo express) {
        if (customer == null) {
            throw new ValidationException("消息发送指定的会员不能为空！");
        }

        if (express == null) {
            throw new ValidationException("消息发送指定的快递单不能为空！");
        }

        int expressId = express.Id;

        string trackingNo = express.TrackingNo;

        if (customer.Status == CustomerStatus.Frozen) {
            return;
        }

        CustomerMessageRealTimeSendSettingInfo setting = customerMessageCommonService.GetCustomerMessageRealTimeSendSetting(messageType);
        if (setting.Status == CustomerMessageSettingStatus.Enabled) {
            string message1 = TransferMessage(setting.WechatMessageTemplate1, trackingNo);
            string message2 = TransferMessage(setting.WechatMessageTemplate2, trackingNo);
            string message3 = TransferMessage(setting.WechatMessageTemplate3, trackingNo);

            CustomerMessageStatus messageStatus = CustomerMessageStatus.NotSend;

            bool clientShouldSend = true;
            if (setting.ClientCustomizable && (setting.WechatSend || setting.SmsSend)) {
                clientShouldSend = customerMessageCommonService.CustomerShouldSend(customer.Id, messageType);
            }

            if (clientShouldSend && (setting.WechatSend || setting.SmsSend)) {
                messageStatus = CustomerMessageStatus.Success;
            }

            string processUser = CustomerMessageCommonService.ShundaSecretary;

            int message1Id = 0;
            if (string.IsNullOrWhiteSpace(message1) == false) {
                message1Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, expressId, message1, messageStatus, setting, processUser);
            }

            int message2Id = 0;
            if (string.IsNullOrWhiteSpace(message2) == false) {
                message2Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, expressId, message2, messageStatus, setting, processUser);
            }

            int message3Id = 0;
            if (string.IsNullOrWhiteSpace(message3) == false) {
                message3Id = customerMessageCommonService.SaveCustomerMessage(CustomerMessageSendType.RealTime, customer, null, expressId, message3, messageStatus, setting, processUser);
            }

            if (clientShouldSend) {
                if (setting.WechatSend) {
                    string messsageTitle = customerMessageCommonService.GetMessageTitle(messageType, "【快递单入库】提醒");
                    string customerCode = customerCodeService.GetCustomerCode(customer.Id);

                    if (string.IsNullOrWhiteSpace(message1) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message1Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messsageTitle, customerCode, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message2) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message2Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messsageTitle, customerCode, processUser, url);
                    }

                    if (string.IsNullOrWhiteSpace(message3) == false) {
                        string url = CustomerMessageClientUrlService.GetMessageDetailUrl(message3Id);
                        customerMessageCommonService.SendOrderNoticeTemplateMessage(customer, messsageTitle, customerCode, processUser, url);
                    }
                }

                if (setting.SmsSend) {
                    customerMessageCommonService.SendSms(customer.Mobile, setting.SmsMessageTemplate);
                }
            }
        }
    }
}